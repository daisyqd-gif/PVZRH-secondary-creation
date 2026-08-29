using BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using CustomPlantClass.Main;
using GameLevel.RogueShooting;
using UltimateSniper;
using UI;
using System.Reflection;
using UnityEngine;
using Unity.VisualScripting;

namespace UltimateSniper_FlameGatlingRShooting
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        public override void InitializeMod()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<JalaGatling_Rogue>();
            ClassInjector.RegisterTypeInIl2Cpp<UltimateFlameGatlingUniqueUpgrade>();
            ClassInjector.RegisterTypeInIl2Cpp<UltimateFlameGatlingUniqueUpgrade2>();
            ClassInjector.RegisterTypeInIl2Cpp<UltimateFlameGatling_Shooting>();
            ClassInjector.RegisterTypeInIl2Cpp<WildfireBuff>();
            ClassInjector.RegisterTypeInIl2Cpp<MeteorBuff>();
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }
    public class JalaGatling_Rogue : BaseConfig
    {
        // 实现IntPtr构造方法
        public JalaGatling_Rogue(IntPtr ptr) : base(ptr) { }
        public JalaGatling_Rogue() : base(ClassInjector.DerivedConstructorPointer<JalaGatling_Rogue>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public PlantType PlantType => PlantType.JalaGatling;
        public Il2CppSystem.Collections.Generic.List<BaseBuff> Buffs
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
            plant.ModifyDamage(PlantDamageAdder.Shooting, 19.0f, false, new Il2CppSystem.Nullable<float>(float.MaxValue));
        }
        // 自定义的方法
        public override string Role => "输出, 控制";
        private List<BaseBuff> CustomBuffs = new List<BaseBuff> { new UpgradeBuff(PlantType.JalaGatling, UltimateSniper.Plugin.UFlameGatling) };
    }

    public class UltimateFlameGatling_Shooting : BaseConfig
    {
        // 实现IntPtr构造方法
        public UltimateFlameGatling_Shooting(IntPtr ptr) : base(ptr) { }
        public UltimateFlameGatling_Shooting() : base(ClassInjector.DerivedConstructorPointer<UltimateFlameGatling_Shooting>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public override PlantType PlantType => UltimateSniper.Plugin.UFlameGatling;
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
        private List<BaseBuff> CustomBuffs = new List<BaseBuff> { new DamageBuff(UltimateSniper.Plugin.UFlameGatling), new SpeedBuff(UltimateSniper.Plugin.UFlameGatling), 
            new UltimateFlameGatlingUniqueUpgrade(), new UltimateFlameGatlingUniqueUpgrade2(), new WildfireBuff(), new MeteorBuff(), new StarUpBuff(UltimateSniper.Plugin.UFlameGatling)};
    }

    public class UltimateFlameGatlingUniqueUpgrade : BaseBuff
    {
        // 实现IntPtr构造方法
        public UltimateFlameGatlingUniqueUpgrade(IntPtr ptr) : base(ptr) { }
        public UltimateFlameGatlingUniqueUpgrade() : base(ClassInjector.DerivedConstructorPointer<UltimateFlameGatlingUniqueUpgrade>()) => ClassInjector.DerivedConstructorBody(this);

        // 实现抽象类的方法
        public override PlantType ShowType => UltimateSniper.Plugin.UFlameGatling;
        public override string Title => "强化：赤焰陨星";
        public override string Description => "赤焰陨星所需子弹数量-10";
        public override void OnGet()
        {
            if (ShootingManager.Instance.TryGetPlant(UltimateSniper.Plugin.UFlameGatling, out var plant) && plant != null && plant.TryGetComponent<UltimateFlameGatling_Remade>(out var p))
            {
                p.MeteorCD-=10;
            }
        }
        public override int MaxCount => 10;
        public override float AppearWeight => 0.33f;
        public override Quality Rarity => Quality.gold;
    }

    public class UltimateFlameGatlingUniqueUpgrade2 : BaseBuff
    {
        // 实现IntPtr构造方法
        public UltimateFlameGatlingUniqueUpgrade2(IntPtr ptr) : base(ptr) { }
        public UltimateFlameGatlingUniqueUpgrade2() : base(ClassInjector.DerivedConstructorPointer<UltimateFlameGatlingUniqueUpgrade2>()) => ClassInjector.DerivedConstructorBody(this);

        // 实现抽象类的方法
        public override PlantType ShowType => UltimateSniper.Plugin.UFlameGatling;
        public override string Title => "强化：灼烧";
        public override string Description => "灼烧区域大小+1";
        public override void OnGet()
        {
            ZombieBurn_2.Radius+=1;
        }
        public override int MaxCount => 9;
        public override float AppearWeight => 0.33f;
        public override Quality Rarity => Quality.gold;
    }

    public class WildfireBuff : BaseBuff
    {
        // 实现IntPtr构造方法
        public WildfireBuff(IntPtr ptr) : base(ptr) { }
        public WildfireBuff() : base(ClassInjector.DerivedConstructorPointer<WildfireBuff>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public override PlantType ShowType => UltimateSniper.Plugin.UFlameGatling;
        public override string Title => "质变：山火";
        public override string Description => "灼烧将传播";
        public override void OnGet()
        {
            TravelMgr.Instance.GetNormalBuff(UltimateSniper.Plugin.Buff2);
        }
        public override int MaxCount => 1;
        public override float AppearWeight => 0.025f;
        public override Quality Rarity => Quality.diamond;
    }

    public class MeteorBuff : BaseBuff
    {
        // 实现IntPtr构造方法
        public MeteorBuff(IntPtr ptr) : base(ptr) { }
        public MeteorBuff() : base(ClassInjector.DerivedConstructorPointer<MeteorBuff>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public override PlantType ShowType => UltimateSniper.Plugin.UFlameGatling;
        public override string Title => "质变：飞沙走石";
        public override string Description => "获得词条：飞沙走石";
        public override void OnGet()
        {
            TravelMgr.Instance.GetNormalBuff((AdvBuff)UltimateSniper.Plugin.Buff1);
        }
        public override int MaxCount => 1;
        public override float AppearWeight => 0.025f;
        public override Quality Rarity => Quality.diamond;
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateSniper_FlameGatlingRShooting.Bepinex";
        public const string PluginName = "UltimateSniper_FlameGatlingRShooting";
        public const string PluginVersion = "3.7";
    }
    [HarmonyPatch(typeof(ShootingManager))]
    public static class ShootingManagerPatch
    {
        [HarmonyPatch(nameof(ShootingManager.ShowBuff))]
        [HarmonyPrefix]
        public static void ShowBuff()
        {
            if (Config.configs != null)
            {
                if (!Config.configs.ContainsKey(PlantType.JalaGatling))
                {
                    // Config.configs[PlantType.Peashooter] = new CustomGatlingPea();
                    Config.configs.Add(PlantType.JalaGatling, new JalaGatling_Rogue());
                    Config.configs.Add(UltimateSniper.Plugin.UFlameGatling, new UltimateFlameGatling_Shooting());
                }
                else
                    Config.configs[UltimateSniper.Plugin.UFlameGatling].Cast<UltimateFlameGatling_Shooting>().ResetQuality();
            }
        }
    }

    [HarmonyPatch(typeof(Peashooter))]
    public static class PeashooterPatch
    {
        [HarmonyPatch(nameof(Peashooter.Buffs), MethodType.Getter)]
        [HarmonyPostfix]
        public static void PostGetBuffs(ref Il2CppSystem.Collections.Generic.List<BaseBuff> __result)
        {
            __result.Add(new UpgradeBuff(PlantType.Peashooter, PlantType.JalaGatling));
        }
    }

    [HarmonyPatch(typeof(PlantDataMenu))]
    public static class PlantDataMenuPatch
    {
        [HarmonyPatch(nameof(PlantDataMenu.Start))]
        [HarmonyPostfix]
        public static void PostStart(PlantDataMenu __instance)
        {
            if (__instance != null && __instance.gameObject != null && !__instance.IsDestroyed() && !__instance.gameObject.IsDestroyed() &&
                __instance.plant != null && __instance.plant.gameObject != null && !__instance.plant.IsDestroyed() && !__instance.plant.gameObject.IsDestroyed()
                && __instance.plant.thePlantType == UltimateSniper.Plugin.UFlameGatling && ShootingManager.Instance != null && __instance.plant.TryGetComponent<UltimateFlameGatling_Remade>(out var a))
            {
                var str = $"陨星所需子弹数量：{a.MeteorCD}\n灼烧区域大小：{ZombieBurn_2.Radius}\n";
                foreach (var text in __instance.infoText)
                    text.text += str;
            }
        }
    }
}
