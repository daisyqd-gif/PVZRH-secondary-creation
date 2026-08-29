using BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using CustomPlantClass.Main;
using GameLevel.RogueShooting;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using CustomizeLib.BepInEx;
using System.Collections;
namespace UltimateRedLunar_RogueShooting
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        public override void InitializeMod()
        {
            ClassInjector.RegisterTypeInIl2Cpp<UltimateRedLunar_Shooting>();
            ClassInjector.RegisterTypeInIl2Cpp<RSHootingSave_LunarEclipse>();
            ClassInjector.RegisterTypeInIl2Cpp<LunarEclipseBuff>();
            ClassInjector.RegisterTypeInIl2Cpp<LunarEclipseBuff2>();
        }
    }
    public class RSHootingSave_LunarEclipse : MonoBehaviour
    {
        public int SelectBuffTimes = 0;
        public bool ZhiBian = false;
        public float Timer { get => Mathf.Max(0.25f, 10 - SelectBuffTimes); }
        public static List<ZombieType> NormalTypes = new()
        {
            ZombieType.BlackFootball,
            ZombieType.JackboxJumpZombie,
            ZombieType.CherryPaperZ95,
            ZombieType.GatlingBlackFootball,
            ZombieType.RedGargantuar
        };
        public static List<ZombieType> ZhiBianTypes = new()
        {
            ZombieType.BlackFootball_b,
            ZombieType.BlackFootball_c,
            ZombieType.GatlingPaper_b,
            ZombieType.GatlingPaper_c,
            ZombieType.Kirov_b,
            ZombieType.Kirov_c,
            ZombieType.BlackFootball_c2,
            ZombieType.ArmedGargantuar
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
    public class UltimateRedLunar_Shooting : BaseConfig
    {
        // 实现IntPtr构造方法
        public UltimateRedLunar_Shooting(IntPtr ptr) : base(ptr) { }
        public UltimateRedLunar_Shooting() : base(ClassInjector.DerivedConstructorPointer<UltimateRedLunar_Shooting>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public override PlantType PlantType => PlantType.UltimateRedLunar;
        public override Il2CppSystem.Collections.Generic.List<BaseBuff> Buffs
        {
            get
            {
                var result = new Il2CppSystem.Collections.Generic.List<BaseBuff>();
                foreach (var item in CustomBuffs) result.Add(item);
                return result;
            }
        }
        public override void ReinforcePlant(Plant plant)
        {
            plant.ModifyDamage(PlantDamageAdder.Shooting, 14.0f, false, new Il2CppSystem.Nullable<float>(float.MaxValue));
            //plant.ModifySpeed(PlantSpeedAdder.Shooting, 5f);
            plant.StartCoroutine(rPlant());
            IEnumerator rPlant(){
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                if (plant.TryGetComponent<RedLunarCabbage>(out var comp))
                {
                    comp.SuperSkill();
                }
            }
        }
        // 自定义的方法

        public void ResetQuality()
        {
            CustomBuffs[0].Cast<DamageBuff>().randomQuality = ShootingManager.Instance.GetRandomQuality();
            CustomBuffs[1].Cast<SpeedBuff>().randomQuality = ShootingManager.Instance.GetRandomQuality();
        }
        public override string Role => "召唤";

        private List<BaseBuff> CustomBuffs = new List<BaseBuff> { new DamageBuff(PlantType.UltimateRedLunar), new SpeedBuff(PlantType.UltimateRedLunar),
            new LunarEclipseBuff(),new LunarEclipseBuff2()};
    }

    public class LunarEclipseBuff : BaseBuff
    {
        // 实现IntPtr构造方法
        public LunarEclipseBuff(IntPtr ptr) : base(ptr) { }
        public LunarEclipseBuff() : base(ClassInjector.DerivedConstructorPointer<LunarEclipseBuff>()) => ClassInjector.DerivedConstructorBody(this);

        // 实现抽象类的方法
        public override PlantType ShowType => PlantType.UltimateRedLunar;
        public override string Title => "强化：月食";
        public override string Description => "血月召唤冷却-1秒 (最低 1/4 秒), 血月召唤的僵尸攻击伤害x2";
        public override void OnGet()
        {
            if (ShootingManager.Instance.TryGetPlant(PlantType.UltimateRedLunar, out var plant))
            {
                Board.Instance.GetOrAddComponent<RSHootingSave_LunarEclipse>().SelectBuffTimes++;
            }
        }
        public override int MaxCount => 10;
        public override float AppearWeight => 0.33f;
        public override Quality Rarity => Quality.gold;
    }

    public class LunarEclipseBuff2 : BaseBuff
    {
        // 实现IntPtr构造方法
        public LunarEclipseBuff2(IntPtr ptr) : base(ptr) { }
        public LunarEclipseBuff2() : base(ClassInjector.DerivedConstructorPointer<LunarEclipseBuff2>()) => ClassInjector.DerivedConstructorBody(this);

        // 实现抽象类的方法
        public override PlantType ShowType => PlantType.UltimateRedLunar;
        public override string Title => "质变：赤渊蚀月";
        public override string Description => "血月召唤的僵尸将升级为异次元二级和三级僵尸";
        public override void OnGet()
        {
            if (ShootingManager.Instance.TryGetPlant(PlantType.UltimateRedLunar, out var plant))
            {
                Board.Instance.GetOrAddComponent<RSHootingSave_LunarEclipse>().ZhiBian=true;
            }
        }
        public override int MaxCount => 1;
        public override float AppearWeight => 0.05f;
        public override Quality Rarity => Quality.diamond;
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateRedLunar_RogueShooting.Bepinex";
        public const string PluginName = "UltimateRedLunar_RogueShooting";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
    [HarmonyPatch(typeof(ShootingManager))]
    public static class ShootingManagerPatch
    {
        [HarmonyPatch(nameof(ShootingManager.Awake))]
        [HarmonyPostfix]
        public static void PostShootingManager(ShootingManager __instance)
        {
            __instance.ExpertPlants.Add(PlantType.UltimateRedLunar);
        }
        [HarmonyPatch(nameof(ShootingManager.ShowBuff))]
        [HarmonyPrefix]
        public static void ShowBuff()
        {
            if (Config.configs != null)
            {
                if (!Config.configs.ContainsKey(PlantType.UltimateRedLunar))
                {
                    Config.configs.Add(PlantType.UltimateRedLunar, new UltimateRedLunar_Shooting());
                }
                else
                    Config.configs[PlantType.UltimateRedLunar].Cast<UltimateRedLunar_Shooting>().ResetQuality();
            }
        }
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
                int boxIndex = UnityEngine.Random.Range(0, freeBoxes.Count);
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
                else if (zombieType == ZombieType.CherryPaperZ95 || zombieType == ZombieType.GatlingPaper_c)
                {
                    // remove second armor and force damage
                    zombie.theSecondArmorHealth = 0;
                    zombie.TakeDamage(1, null, DamageType.Normal);
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
