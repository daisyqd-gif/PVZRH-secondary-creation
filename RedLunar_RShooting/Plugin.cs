using BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using CustomPlantClass.Main;
using GameLevel.RogueShooting;
using Unity.VisualScripting;
using UnityEngine;
using CustomizeLib.BepInEx;
using CustomPlantClass.RogueShootingManager;
using CustomPlantClass.Runtime;
using CustomPlantClass;
using CustomPlantClass.Runtime.Tasks;
namespace UltimateRedLunar_RogueShooting
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        public static AdvBuff ZhiBianBuff = (AdvBuff)(-1);
        public override void InitializeMod()
        {
            ClassInjector.RegisterTypeInIl2Cpp<RSHootingSave_LunarEclipse>();

            (ZhiBianBuff,BaseBuff QBuff) = RegistryHelper.RegisterCustomQualitativeChangeBuff("质变：赤渊蚀月","血月召唤的僵尸将升级为异次元二级和三级僵尸",PlantType.UltimateRedLunar);
            BaseBuff UBuff = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = PlantType.UltimateRedLunar,
                CustomBuffType = ShootingBuffType.UniqueUpgrade,
                CustomTitle = "强化：月食",
                CustomDescription = "血月召唤冷却-1秒 (最低 1/2 秒), 血月召唤的僵尸攻击伤害x2",
                CustomOnGet = () =>
                {
                    if (ShootingManager.Instance.TryGetPlant(PlantType.UltimateRedLunar, out var plant))
                    {
                        Board.Instance.GetOrAddComponent<RSHootingSave_LunarEclipse>().SelectBuffTimes++;
                    }
                }
            });
            BaseConfig cfg = RegistryHelper.MakeConfigType(new()
            {
                CustomPlantType = PlantType.UltimateRedLunar,
                CustomBuffs = () => new List<BaseBuff>() { new DamageBuff(PlantType.UltimateRedLunar), new SpeedBuff(PlantType.UltimateRedLunar), UBuff, QBuff },
                CustomReinforcePlant = (plant) =>
                {
                    plant.ModifyDamage(PlantDamageAdder.Shooting, 14.0f, false, new Il2CppSystem.Nullable<float>(float.MaxValue));
                    RPlant(plant);
                },
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker),
            });
            RegistryHelper.AddCustomExpertPlant(PlantType.UltimateRedLunar,cfg);
        }
        public static async void RPlant(Plant p)
        {
            try
            {
                await DelayTask.WaitForFixedUpdate(2,p.CreateCancellationToken());
                if (p != null && p.TryGetComponent<RedLunarCabbage>(out var comp))
                {
                    comp.SuperSkill();
                }    
            }
            catch(Exception e)
            {
                ModLogger.LogError(e.ToString());
            }
        }
    }
    public class RSHootingSave_LunarEclipse : MonoBehaviour
    {
        public int SelectBuffTimes = 0;
        public bool ZhiBian => Plugin.ZhiBianBuff.IsActive();
        public float Timer { get => Mathf.Max(0.5f, 10 - SelectBuffTimes); }
        public static List<ZombieType> NormalTypes = new()
        {
            ZombieType.BlackFootball,
            ZombieType.JackboxJumpZombie,
            ZombieType.CherryPaperZ95,
            ZombieType.GatlingBlackFootball,
            ZombieType.RedGargantuar,
            ZombieType.ObsidianTallNutZombie
        };
        public static List<ZombieType> ZhiBianTypes = new()
        {
            ZombieType.BlackFootball_b,
            ZombieType.BlackFootball_c,
            ZombieType.GatlingPaper_b,
            ZombieType.GatlingPaper_c,
            ZombieType.BlackFootball_c2,
            ZombieType.Jackbox_b,
            ZombieType.ArmedGargantuar,
            ZombieType.Driver_b,
            ZombieType.Driver_c,
            ZombieType.ElephantZombie_b,
            ZombieType.ElephantZombie_c,
            ZombieType.ObsidianTallNutZombie
        };
        public List<ZombieType> SummonTypes
        {
            get
            {
                if (ZhiBian) return ZhiBianTypes;
                else return NormalTypes;
            }
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateRedLunar_RogueShooting.Bepinex";
        public const string PluginName = "UltimateRedLunar_RogueShooting";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
    [HarmonyPatch(typeof(Lunar))]
    public static class LunarPatch
    {
        [HarmonyPatch(nameof(Lunar.God), MethodType.Getter)]
        [HarmonyPostfix]
        public static void PostGod(ref bool __result)
        {
            __result = __result || Board.Instance.boardTag.rogueShooting;
        }
        [HarmonyPatch(nameof(Lunar.Update))]
        [HarmonyPrefix]
        public static bool Update_Prefix(Lunar __instance)
        {
            if (GameAPP.theGameStatus != GameStatus.InGame || !Board.Instance.boardTag.rogueShooting) return true;
            if (!PlantMgr.IsNotNullMonoBehaviour(__instance, out var lunar)) return true;
            if (!lunar.red) return true;
            lunar.summonTimer -= Time.deltaTime;
            if (lunar.summonTimer > 0f) return false;
            var save = Board.Instance.GetOrAddComponent<RSHootingSave_LunarEclipse>();
            lunar.summonTimer = save.Timer;
            // get free tiles
            List<Vector2Int> freeBoxes = [.. lunar.GetFreeBoxes()];
            if (freeBoxes == null || freeBoxes.Count == 0)
                return false;

            // health/damage scaling based on plant count (0x12e)
            int plantCount = 10;
            float scale = plantCount * 0.5f + 1f;
            if (scale < 1f) scale = 1f;

            // number of summons = freeBoxes.Count / 4 (same as IL2CPP loop)
            int summonCount = freeBoxes.Count / 4;

            for (int i = 0; i < summonCount; i++)
            {
                // pick random tile
                int boxIndex = UnityEngine.Random.Range(3, freeBoxes.Count);
                Vector2Int box = freeBoxes[boxIndex];

                ZombieType zombieType = save.SummonTypes.GetRandomItem();

                // convert tile to world X coordinate
                float worldX = Mouse.Instance.GetBoxXFromColumn(box.x);

                // spawn mind‑controlled zombie
                Zombie z = CreateZombie.Instance.SetZombieWithMindControl(
                    box.y, zombieType, worldX, false
                );

                if (z == null)
                    continue;

                // get actual Zombie component
                Zombie zombie = z.GetComponent<Zombie>();
                if (zombie == null)
                    continue;

                // particle effect
                var pos = zombie.axis.position;
                ParticleManager.Instance.SetParticle(
                    ParticleType.RandomCloud,
                    new Vector2(pos.x, pos.y + 0.5f),
                    zombie.theZombieRow,
                    true,
                    0f
                );

                // scale health and damage
                Lawnf.SetZombieHealth(zombie, scale);
                zombie.theAttackDamage = (int)(zombie.theAttackDamage * scale);

                // special cases based on zombieType
                if (zombieType == ZombieType.RedGargantuar || zombieType == ZombieType.ArmedGargantuar)
                {
                    // speed ×3
                    zombie.theOriginSpeed *= 3f;
                }
                else if (zombie is GatlingPaperZombie_a a)
                {
                    a.LosePaper();
                }
                else if (zombie is PaperCherryZ95 p)
                {
                    p.LosePaper();
                }
                else if (zombieType == ZombieType.BlackFootball)
                {
                    // speed ×3
                    zombie.theOriginSpeed *= 3f;
                }
                else if (zombieType == ZombieType.JackboxJumpZombie || zombieType == ZombieType.Jackbox_b)
                {
                    // set first armor to 1
                    zombie.theFirstArmorHealth = 1;
                    zombie.UpdateHealthText();
                }
                if(save.ZhiBian) zombie.theOriginSpeed*=6f;
                if(save.ZhiBian) zombie.theMaxHealth*=3;
                if(save.ZhiBian) zombie.theHealth*=3;
                zombie.theAttackDamage = (int)(zombie.theAttackDamage * Mathf.Pow(2f, save.SelectBuffTimes));

                // lunar color tint
                zombie.UpdateColor(Zombie.ZombieColor.Lunar);
            }
            return false;
        }
    }
}
