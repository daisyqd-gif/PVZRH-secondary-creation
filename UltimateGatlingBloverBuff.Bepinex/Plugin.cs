using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using System.Collections;
using UnityEngine;
using CustomPlantClass;
using CustomPlantClass.Main;


namespace UltimateGatlingBloverBuff{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "UltimateGatlingBloverBuff.Bepinex";
        public const string PluginName = "UltimateGatlingBloverBuff";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            ClassInjector.RegisterTypeInIl2Cpp<UltimateGatlingBloverBuff>();
            DataMgr.RegisterCustomStarUp(PlantType.UltimateGatlingBlover);
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }

    public class UltimateGatlingBloverBuff : PlantSkinComponent
    {
        public IEnumerator Shooting()
        {
            Transform shoot = plant.shoot;
            if (shoot == null) yield break;

            Vector2 pos = shoot.position;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var cb = CreateBullet.Instance;
                    if (cb == null) yield break;

                    float x = pos.x + Random.Range(-0.1f, 0.1f);
                    float y = pos.y + Random.Range(-0.1f, 0.1f);

                    var bullet = cb.SetBullet(
                        x, y,
                        plant.thePlantRow,
                        BulletType.Bullet_superCherry,
                        BulletMoveWay.SuperGatling,
                        false
                    );

                    if (bullet == null) yield break;

                    bullet.transform.Rotate(0f, 0f, Random.Range(-15f, 15f));
                    bullet.fromType = plant.thePlantType;
                    bullet.Damage = plant.attackDamage;
                    bullet.normalSpeed = Random.Range(12f, 14f);
                }

                yield return new WaitForFixedUpdate();
            }
        }
    }
    [HarmonyPatch(typeof(UltimateGatlingBlover), nameof(UltimateGatlingBlover.AttributeEvent))]
    public static class Shoot1_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(UltimateGatlingBlover __instance)
        {
            if (__instance != null && __instance.thePlantType==PlantType.UltimateGatlingBlover)
            {
                var component=__instance.GetOrAddComponent<UltimateGatlingBloverBuff>();
                if (__instance.starUp)
                {
                    component.StartCoroutine(component.Shooting());
                }
            }
        }
    }
    [HarmonyPatch(typeof(UltimateGatlingBlover), nameof(UltimateGatlingBlover.Start))]
    public static class Start_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(UltimateGatlingBlover __instance)
        {
            if(__instance.board==null) return true;
            __instance.board?.OnPlantCreate(__instance);
            __instance.UpdateText();
            __instance.ReplaceSprite();
            return false;
        }
    }
}
