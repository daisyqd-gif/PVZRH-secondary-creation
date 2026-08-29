global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using System;
global using System.Collections;
global using System.Reflection;
global using UnityEngine;
global using CustomPlantClass;
global using Unity.VisualScripting;
global using Il2CppInterop.Runtime.Injection;
global using UnityEngine.Rendering;
global using CustomPlantClass.Main;

namespace UltimatePlanternSkin
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, CustomPlantClass.MyPluginInfo.TargetVersion)]
    public class Core : BasePlugin
    {
        public static ID particletype;
        public override void Load()
        {
            try
            {
                // Apply all Harmony patches in this assembly
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
                particletype = DataMgr.AllocateID();
                ClassInjector.RegisterTypeInIl2Cpp<LightComp>();

                // Load the AssetBundle containing your plant prefab(s)
                // Replace "abname" with your actual bundle name
                AssetBundle assetBundle = CustomCore.GetAssetBundle(
                    Assembly.GetExecutingAssembly(),
                    "skin_949_0"
                );

                CustomCore.RegisterCustomPlantSkin<UltimatePlantern,LightComp>((int)PlantType.UltimatePlantern, assetBundle.GetAsset<GameObject>("UltimatePlanternPrefab"), assetBundle.GetAsset<GameObject>("UltimatePlanternPreview"),
                (UltimatePlantern p) =>
                {
                    SpriteRenderer sr = p.transform.FindChild("UltimateLight").GetComponent<SpriteRenderer>();
                    sr.material = Resources.Load<Material>("shaders/Brightness.mat");
                    p.lanternLight = p.transform.FindChild("Light").gameObject;
                    p.shoot = p.transform.FindChild("Shoot");
                    p.shoot2 = p.transform.FindChild("Shoot2");
                    p.laserPrefab = Resources.Load<GameObject>("plants/peashooter/lanternpea/Laser.prefab");
                    p.ultimateLight = sr;
                });
                GameObject obj=assetBundle.GetAsset<GameObject>("AuroraVision");
                CustomCore.RegisterCustomParticle(particletype, obj);
                SortingGroup group=obj.GetComponent<SortingGroup>();
                group.sortingLayerName="UI";
                group.sortingOrder=100000000;
                Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
            }
            catch (Exception e)
            {
                DataMgr.StartUpMessages.Add(MyPluginInfo.PluginName + " load failed.\n" + e.ToString());
            }
        }
    }
    public class LightComp : PlantSkinComponent
    {
        public bool isstarted=false;
        public IEnumerator AuroraSpam()
        {
            int count = 0;

            while (count < 500)
            {
                // Spawn 1 aurora
                int row = PlantMgr.GetRandomBoardRow();
                int col = PlantMgr.GetRandomBoardColumn();

                if (row >= 0 && col >= 0 && ParticleManager.Instance != null)
                {
                    var pos = PlantMgr.GetPos(row, col);
                    ParticleManager.Instance.SetParticle(Core.particletype, pos);
                }

                count++;

                // Wait 1 second
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    [HarmonyPatch(typeof(UltimatePlantern), nameof(UltimatePlantern.Shrink))]
    public static class Shrink_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(UltimatePlantern __instance)
        {
            try
            {
                if(__instance == null || __instance.board==null) return;
                if (__instance.TryGetComponent<LightComp>(out var _))
                {
                    Board b=__instance.board;
                    LightComp l=b.GetOrAddComponent<LightComp>();
                    b.StartCoroutine(l.AuroraSpam());
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimatePlanternSkin.Bepinex";
        public const string PluginName = "UltimatePlanternSkin";
        public const string PluginVersion = "3.7";
    }
}
