using BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using CustomPlantClass.Main;
using GameLevel.RogueShooting;
using MegaGatlingExpansion;
using UI;
using System.Reflection;
using UnityEngine;
using Unity.VisualScripting;

namespace MegaGatlingExpansionRShooting
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Core : ModPlugin
    {
        public override void InitializeMod()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<PeaGatling>();
            ClassInjector.RegisterTypeInIl2Cpp<MegaGatlingPeaUniqueUpgrade>();
            ClassInjector.RegisterTypeInIl2Cpp<MegaGatlingPea_Shooting>();
            ClassInjector.RegisterTypeInIl2Cpp<StarUpElectricBuff>();
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }
    public class PeaGatling : BaseConfig
    {
        // 实现IntPtr构造方法
        public PeaGatling(IntPtr ptr) : base(ptr) { }
        public PeaGatling() : base(ClassInjector.DerivedConstructorPointer<PeaGatling>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public PlantType PlantType => PlantType.GatlingPea;
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
        public override string Role => "输出";

        private List<BaseBuff> CustomBuffs = new List<BaseBuff> { new UpgradeBuff(PlantType.GatlingPea, PlantTypeExpand.MegaGatlingPea) };
    }

    public class MegaGatlingPea_Shooting : BaseConfig
    {
        // 实现IntPtr构造方法
        public MegaGatlingPea_Shooting(IntPtr ptr) : base(ptr) { }
        public MegaGatlingPea_Shooting() : base(ClassInjector.DerivedConstructorPointer<MegaGatlingPea_Shooting>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public override PlantType PlantType => PlantTypeExpand.MegaGatlingPea;
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

        private List<BaseBuff> CustomBuffs = new List<BaseBuff> { new DamageBuff(PlantTypeExpand.MegaGatlingPea), new SpeedBuff(PlantTypeExpand.MegaGatlingPea), 
            new MegaGatlingPeaUniqueUpgrade(), new StarUpElectricBuff() };
        public override string Role => "请输入文本";
    }

    public class MegaGatlingPeaUniqueUpgrade : BaseBuff
    {
        // 实现IntPtr构造方法
        public MegaGatlingPeaUniqueUpgrade(IntPtr ptr) : base(ptr) { }
        public MegaGatlingPeaUniqueUpgrade() : base(ClassInjector.DerivedConstructorPointer<MegaGatlingPeaUniqueUpgrade>()) => ClassInjector.DerivedConstructorBody(this);

        // 实现抽象类的方法
        public override PlantType ShowType => PlantTypeExpand.MegaGatlingPea;
        public override string Title => "强化：PF";
        public override string Description => "植物PF几率+10%";
        public override void OnGet()
        {
            if (ShootingManager.Instance.TryGetPlant(PlantTypeExpand.MegaGatlingPea, out var plant) && plant != null && plant.TryGetComponent<MegaGatlingPea>(out var p))
            {
                p.PFChance+=10f;
            }
        }
        public override int MaxCount => 9;
        public override float AppearWeight => 0.33f;
        public override Quality Rarity => Quality.gold;
    }

    public class StarUpElectricBuff : BaseBuff
    {
        // 实现IntPtr构造方法
        public StarUpElectricBuff(IntPtr ptr) : base(ptr) { }
        public StarUpElectricBuff() : base(ClassInjector.DerivedConstructorPointer<StarUpElectricBuff>()) => ClassInjector.DerivedConstructorBody(this);
        // 实现抽象类的方法
        public override PlantType ShowType => PlantTypeExpand.MegaGatlingPea;
        public override string Title => "质变：星辉";
        public override string Description => "星辉 + 只能发射电击子弹";
        public override void OnGet()
        {
            if(ShootingManager.Instance.TryGetPlant(ShowType,out var p))
            {
                p.starUp=true;
                p.UpdateStarIcon();
                if(p.TryGetComponent<MegaGatlingPea>(out var a))
                {
                    a.isElectric=true;
                }
            }
        }
        public override int MaxCount => 1;
        public override float AppearWeight => 0.05f;
        public override Quality Rarity => Quality.diamond;
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "MegaGatlingExpansionRShooting.Bepinex";
        public const string PluginName = "MegaGatlingExpansionRShooting";
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
                if (!Config.configs.ContainsKey(PlantType.GatlingPea))
                {
                    // Config.configs[PlantType.Peashooter] = new CustomGatlingPea();
                    Config.configs.Add(PlantType.GatlingPea, new PeaGatling());
                    Config.configs.Add(PlantTypeExpand.MegaGatlingPea, new MegaGatlingPea_Shooting());
                }
                else
                    Config.configs[PlantTypeExpand.MegaGatlingPea].Cast<MegaGatlingPea_Shooting>().ResetQuality();
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
            __result.Add(new UpgradeBuff(PlantType.Peashooter, PlantType.GatlingPea));
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
                && __instance.plant.thePlantType == PlantTypeExpand.MegaGatlingPea && ShootingManager.Instance != null && __instance.plant.TryGetComponent<MegaGatlingPea>(out var a))
            {
                var str = $"PF几率：{a.PFChance}%\n";
                foreach (var text in __instance.infoText)
                    text.text += str;
            }
        }
    }
}
