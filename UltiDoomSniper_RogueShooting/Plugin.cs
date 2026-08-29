using BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using CustomPlantClass.Main;
using GameLevel.RogueShooting;
using UI;
namespace UltimateDoomSniper_RogueShooting
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        public override void InitializeMod()
        {
            ClassInjector.RegisterTypeInIl2Cpp<UltimateDoomSniper_Shooting>();
            ClassInjector.RegisterTypeInIl2Cpp<UltimateDestroySniper_Shooting>();
            ClassInjector.RegisterTypeInIl2Cpp<DevilPowerBuff>();
            ClassInjector.RegisterTypeInIl2Cpp<UDoomUniqueUpgrade>();
            ClassInjector.RegisterTypeInIl2Cpp<UDoomHeartUpgrade>();
        }
    }

    public class UltimateDestroySniper_Shooting : BaseConfig
    {
        // 实现IntPtr构造方法
        public UltimateDestroySniper_Shooting(IntPtr ptr) : base(ptr) { }
        public UltimateDestroySniper_Shooting() : base(ClassInjector.DerivedConstructorPointer<UltimateDestroySniper_Shooting>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public override PlantType PlantType => UltimateDoomSniper.Plugin.PlantID_Sniper2;
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
        }
        // 自定义的方法

        public void ResetQuality()
        {
            CustomBuffs[0].Cast<DamageBuff>().randomQuality = ShootingManager.Instance.GetRandomQuality();
            CustomBuffs[1].Cast<SpeedBuff>().randomQuality = ShootingManager.Instance.GetRandomQuality();
        }
        public override string Role => "输出, 控制";

        private List<BaseBuff> CustomBuffs = new List<BaseBuff> { new DamageBuff(UltimateDoomSniper.Plugin.PlantID_Sniper2), new SpeedBuff(UltimateDoomSniper.Plugin.PlantID_Sniper2), 
            new DevilPowerBuff()};
    }

    public class DevilPowerBuff : BaseBuff
    {
        // 实现IntPtr构造方法
        public DevilPowerBuff(IntPtr ptr) : base(ptr) { }
        public DevilPowerBuff() : base(ClassInjector.DerivedConstructorPointer<DevilPowerBuff>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public override PlantType ShowType => UltimateDoomSniper.Plugin.PlantID_Sniper2;
        public override string Title => "质变：死神之力";
        public override string Description => "获得词条：死神之力";
        public override void OnGet()
        {
            TravelMgr.Instance.GetNormalBuff((AdvBuff)UltimateDoomSniper.Plugin.Buff2);
        }
        public override int MaxCount => 1;
        public override float AppearWeight => 0.025f;
        public override Quality Rarity => Quality.diamond;
    }

    public class UltimateDoomSniper_Shooting : BaseConfig
    {
        // 实现IntPtr构造方法
        public UltimateDoomSniper_Shooting(IntPtr ptr) : base(ptr) { }
        public UltimateDoomSniper_Shooting() : base(ClassInjector.DerivedConstructorPointer<UltimateDoomSniper_Shooting>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public override PlantType PlantType => UltimateDoomSniper.Plugin.PlantID;
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
        }
        // 自定义的方法

        public void ResetQuality()
        {
            CustomBuffs[0].Cast<DamageBuff>().randomQuality = ShootingManager.Instance.GetRandomQuality();
            CustomBuffs[1].Cast<SpeedBuff>().randomQuality = ShootingManager.Instance.GetRandomQuality();
        }
        public override string Role => "输出, 控制";

        private List<BaseBuff> CustomBuffs = new List<BaseBuff> { new DamageBuff(UltimateDoomSniper.Plugin.PlantID), new SpeedBuff(UltimateDoomSniper.Plugin.PlantID), 
            new UDoomUniqueUpgrade(), new UDoomHeartUpgrade()};
    }

    public class UDoomUniqueUpgrade : GameLevel.RogueShooting.DoomSniper.UniqueUpgrade
    {
        // 实现IntPtr构造方法
        public UDoomUniqueUpgrade(IntPtr ptr) : base(ptr) { }
        public UDoomUniqueUpgrade() : base(ClassInjector.DerivedConstructorPointer<UDoomUniqueUpgrade>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public override PlantType ShowType => UltimateDoomSniper.Plugin.PlantID;
    }

    public class UDoomHeartUpgrade : GameLevel.RogueShooting.DoomSniper.HeartUpgrade
    {
        // 实现IntPtr构造方法
        public UDoomHeartUpgrade(IntPtr ptr) : base(ptr) { }
        public UDoomHeartUpgrade() : base(ClassInjector.DerivedConstructorPointer<UDoomHeartUpgrade>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public override PlantType ShowType => UltimateDoomSniper.Plugin.PlantID;
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateDoomSniper_RogueShooting.Bepinex";
        public const string PluginName = "UltimateDoomSniper_RogueShooting";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
    [HarmonyPatch(typeof(ShootingManager))]
    public static class ShootingManagerPatch
    {
        [HarmonyPatch(nameof(ShootingManager.Awake))]
        [HarmonyPostfix]
        public static void PostShootingManager(ShootingManager __instance)
        {
            __instance.ExpertPlants.Add(UltimateDoomSniper.Plugin.PlantID_Sniper2);
        }
        [HarmonyPatch(nameof(ShootingManager.ShowBuff))]
        [HarmonyPrefix]
        public static void ShowBuff()
        {
            if (Config.configs != null)
            {
                if (!Config.configs.ContainsKey(UltimateDoomSniper.Plugin.PlantID))
                {
                    Config.configs.Add(UltimateDoomSniper.Plugin.PlantID, new UltimateDoomSniper_Shooting());
                }
                else
                    Config.configs[UltimateDoomSniper.Plugin.PlantID].Cast<UltimateDoomSniper_Shooting>().ResetQuality();
                if (!Config.configs.ContainsKey(UltimateDoomSniper.Plugin.PlantID_Sniper2))
                {
                    Config.configs.Add(UltimateDoomSniper.Plugin.PlantID_Sniper2, new UltimateDestroySniper_Shooting());
                }
                else
                    Config.configs[UltimateDoomSniper.Plugin.PlantID_Sniper2].Cast<UltimateDestroySniper_Shooting>().ResetQuality();
            }
        }
    }
    [HarmonyPatch(typeof(GameLevel.RogueShooting.DoomSniper))]
    public static class DoomSniperPatch
    {
        [HarmonyPatch(nameof(GameLevel.RogueShooting.DoomSniper.Buffs), MethodType.Getter)]
        [HarmonyPostfix]
        public static void PostGetBuffs(ref Il2CppSystem.Collections.Generic.List<BaseBuff> __result)
        {
            __result.Clear();
            __result.Add(new UpgradeBuff(PlantType.DoomSniper,UltimateDoomSniper.Plugin.PlantID));
        }
    }
}
