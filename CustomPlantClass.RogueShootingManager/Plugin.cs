global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using Il2CppInterop.Runtime.Injection;
global using System;
global using System.Reflection;
global using UnityEngine;
global using System.Collections.Generic;
global using CustomPlantClass.Main;
global using GameLevel.RogueShooting;
using UI;
namespace CustomPlantClass.RogueShootingManager
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        public override void InitializeMod()
        {
            PluginBehaviour.AddComponentToPlugin<RegistryHelper>();
            if (!ClassInjector.IsTypeRegisteredInIl2Cpp<PlaceHolderConfig>())
            {
                ClassInjector.RegisterTypeInIl2Cpp<PlaceHolderConfig>();
            }
            if (!ClassInjector.IsTypeRegisteredInIl2Cpp<RogueClass_BUFF>())
            {
                ClassInjector.RegisterTypeInIl2Cpp<RogueClass_BUFF>();
            }
            if (!ClassInjector.IsTypeRegisteredInIl2Cpp<RogueClass_CONFIG>())
            {
                ClassInjector.RegisterTypeInIl2Cpp<RogueClass_CONFIG>();
            }
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded as an expansion to CustomPlantClass.");
            ModLogger.LogError("Release notes", "This mod is not ready yet! Please wait until the game's 4.0.1 update.");
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.RogueShootingManager.Bepinex";
        public const string PluginName = "CustomPlantClass.RogueShootingManager";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.PluginVersion;
    }
    public class PlaceHolderConfig : BaseConfig
    {
        public PlaceHolderConfig(IntPtr ptr) : base(ptr) { }
        public PlaceHolderConfig(PlantType thePlantType) : base(ClassInjector.DerivedConstructorPointer<PlaceHolderConfig>())
        {
            ClassInjector.DerivedConstructorBody(this);
            plantType = thePlantType;
        }
        #pragma warning disable CS0114
        private PlantType plantType {get;}
        public PlantType PlantType => plantType;
        public Il2CppSystem.Collections.Generic.List<BaseBuff> Buffs
        {
            get
            {
                var result = new Il2CppSystem.Collections.Generic.List<BaseBuff>();
                foreach (var item in CustomBuffs) result.Add(item);
                return result;
            }
        }
        #pragma warning restore CS0114
        public override void ReinforcePlant(Plant plant)
        {
        }

        public override string Role => "数据异常";

        // 自定义的方法

        private List<BaseBuff> CustomBuffs = new();
    
    }
    public class RogueClass_BUFF : BaseBuff
    {
        public RogueClass_BUFF(IntPtr ptr) : base(ptr) { }

        public RogueClass_BUFF(CustomRogueShootingBuff buff)
            : base(ClassInjector.DerivedConstructorPointer<RogueClass_BUFF>())
        {
            ClassInjector.DerivedConstructorBody(this);
            this.buff = buff;
        }
        public virtual CustomRogueShootingBuff buff { get; }

        public override PlantType ShowType => buff.CustomPlantType;
        public override string Title => buff.CustomTitle;
        public override string Description => buff.CustomDescription;

        public override void OnGet()
        {
            buff.CustomOnGet?.Invoke();
        }

        public override int MaxCount
        {
            get
            {
                switch (buff.CustomBuffType)
                {
                    case ShootingBuffType.UniqueUpgrade: return 10;
                    case ShootingBuffType.QualitativeChange:
                    case ShootingBuffType.SuperUpgrade: return 1;
                    case ShootingBuffType.General: return 250;
                    case ShootingBuffType.CurseBuff: return 1;
                }
                return 1;
            }
        }

        public override float AppearWeight
        {
            get
            {
                switch (buff.CustomBuffType)
                {
                    case ShootingBuffType.UniqueUpgrade: return 0.33f;
                    case ShootingBuffType.QualitativeChange: return 0.05f;
                    case ShootingBuffType.SuperUpgrade: return 0.003f;
                    case ShootingBuffType.General: return 1f;
                    case ShootingBuffType.CurseBuff: return 0.1f;
                }
                return 1f;
            }
        }

        public static Quality GetRandomQuality()
        {
            if(ShootingManager.Instance == null)
            {
                return Quality.gold;
            }
            float luck = ShootingManager.Instance.Lucky;

            float wWood    = 65f * (1f + 1f * luck / 100f);
            float wSilver  = 23f * (1f + 2f * luck / 100f);
            float wGold    = 10f * (1f + 3f * luck / 100f);
            float wDiamond =  2f * (1f + 5f * luck / 100f);

            float total = wWood + wSilver + wGold + wDiamond;
            float roll = UnityEngine.Random.Range(0f, total);

            if (roll < wWood) return Quality.Default;
            roll -= wWood;

            if (roll < wSilver) return Quality.silver;
            roll -= wSilver;

            if (roll < wGold) return Quality.gold;
            roll -= wGold;

            return Quality.diamond;
        }

        protected Quality randomquality = GetRandomQuality();

        public override Quality Rarity
        {
            get
            {
                switch (buff.CustomBuffType)
                {
                    case ShootingBuffType.UniqueUpgrade: return Quality.gold;
                    case ShootingBuffType.QualitativeChange: return Quality.diamond;
                    case ShootingBuffType.SuperUpgrade: return Quality.iridescent;
                    case ShootingBuffType.General: return randomquality;
                    case ShootingBuffType.CurseBuff: return Quality.curse;
                }
                return Quality.Default;
            }
        }
    }
    public class RogueClass_CONFIG : BaseConfig
    {
        public RogueClass_CONFIG(IntPtr ptr) : base(ptr) { }

        public RogueClass_CONFIG(CustomRogueShootingConfig cfg)
            : base(ClassInjector.DerivedConstructorPointer<RogueClass_CONFIG>())
        {
            ClassInjector.DerivedConstructorBody(this);
            config = cfg;
        }

        public CustomRogueShootingConfig config { get; }

    #pragma warning disable CS0114
        public PlantType PlantType => config.CustomPlantType;

        public Il2CppSystem.Collections.Generic.List<BaseBuff> Buffs => config.CustomBuffs().ToIl2CppList();
    #pragma warning restore CS0114

        public override void ReinforcePlant(Plant plant)
        {
            if (config.CustomReinforcePlant != null)
                config.CustomReinforcePlant(plant);
        }

        public override string Role => config.CustomRole;
    }
    [HarmonyPatch(typeof(ShootingAlmanacCatalog))]
	public static class ShootingAlmanacCatalogFixer
	{
		[HarmonyPatch(nameof(ShootingAlmanacCatalog.Build))]
		[HarmonyPrefix]
		public static bool Build_Prefix(ref ShootingAlmanacDirectory __result)
        {
            var configs = Config.configs;
            if (configs == null)
                goto fail;

            // Load the index file the same way Build() does
            var indexAsset = Resources.Load<TextAsset>(ShootingAlmanacCatalog.ResourcePath);
            if (indexAsset == null)
                goto fail;

            var index = JsonUtility.FromJson<ShootingAlmanacIndexFile>(indexAsset.ToString());
            if (index == null || index.plants == null)
                goto fail;

            // Convert plant strings to PlantType
            foreach (var plantStr in index.plants)
            {
                var plant = ShootingAlmanacCatalog.ParsePlant(plantStr.plant);

                if (!configs.ContainsKey(plant))
                {
                    goto fail;
                }

            }
            return true;
            fail:
            if(ShootingAlmanacCatalogPatch.LastGoodDir == null)
            {
                ModLogger.LogError("Something happened");
                return true;
            }
            __result = ShootingAlmanacCatalogPatch.LastGoodDir;
            return false;
        }
	}
}
