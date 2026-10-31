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
    /// <summary>Initializes the Rogue Shooting expansion and its IL2CPP types.</summary>
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        /// <summary>Registers the mod's runtime components and IL2CPP config and buff types.</summary>
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
            global::CustomPlantClass.Runtime.Core.ModLogger.LogError("Release notes", "This mod is not ready yet! Please wait until the game's 4.0.1 update.");
        }
    }
    /// <summary>Identifies this plugin to BepInEx.</summary>
    public class MyPluginInfo
    {
        /// <summary>The plugin's unique identifier.</summary>
        public const string PluginGuid = "CustomPlantClass.RogueShootingManager.Bepinex";
        /// <summary>The plugin's display name.</summary>
        public const string PluginName = "CustomPlantClass.RogueShootingManager";
        /// <summary>The plugin's version.</summary>
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.PluginVersion;
    }
    /// <summary>Provides fallback config data for a plant whose Rogue Shooting data is unavailable.</summary>
    public class PlaceHolderConfig : BaseConfig
    {
        /// <summary>Wraps an existing IL2CPP config pointer.</summary>
        /// <param name="ptr">Pointer to the native config.</param>
        public PlaceHolderConfig(IntPtr ptr) : base(ptr) { }
        /// <summary>Creates a placeholder config for a plant type.</summary>
        /// <param name="thePlantType">The plant represented by the config.</param>
        public PlaceHolderConfig(PlantType thePlantType) : base(ClassInjector.DerivedConstructorPointer<PlaceHolderConfig>())
        {
            ClassInjector.DerivedConstructorBody(this);
            plantType = thePlantType;
        }
        #pragma warning disable CS0114
        private PlantType plantType {get;}
        /// <summary>Gets the plant represented by this config.</summary>
        public PlantType PlantType => plantType;
        /// <summary>Gets the buffs available to the placeholder plant.</summary>
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
        /// <summary>Reinforces the plant; the placeholder config applies no changes.</summary>
        /// <param name="plant">The plant to reinforce.</param>
        public override void ReinforcePlant(Plant plant)
        {
        }

        /// <summary>Gets the fallback role label for the placeholder config.</summary>
        public override string Role => "数据异常";

        // 自定义的方法

        private List<BaseBuff> CustomBuffs = new();
    
    }
    /// <summary>Adapts custom Rogue Shooting buff data to the game's buff API.</summary>
    public class RogueClass_BUFF : BaseBuff
    {
        /// <summary>Wraps an existing IL2CPP buff pointer.</summary>
        /// <param name="ptr">Pointer to the native buff.</param>
        public RogueClass_BUFF(IntPtr ptr) : base(ptr) { }

        /// <summary>Creates a game buff from custom Rogue Shooting buff data.</summary>
        /// <param name="buff">The custom buff definition.</param>
        public RogueClass_BUFF(CustomRogueShootingBuff buff)
            : base(ClassInjector.DerivedConstructorPointer<RogueClass_BUFF>())
        {
            ClassInjector.DerivedConstructorBody(this);
            this.buff = buff;
        }
        /// <summary>Gets the custom definition represented by this buff.</summary>
        public virtual CustomRogueShootingBuff buff { get; }

        /// <summary>Gets the plant type associated with this buff.</summary>
        public override PlantType ShowType => buff.CustomPlantType;
        /// <summary>Gets the buff's display title.</summary>
        public override string Title => buff.CustomTitle;
        /// <summary>Gets the buff's display description.</summary>
        public override string Description => buff.CustomDescription;

        /// <summary>Invokes the custom callback when the buff is acquired.</summary>
        public override void OnGet()
        {
            buff.CustomOnGet?.Invoke();
        }

        /// <summary>Gets the maximum number of times this buff can be acquired.</summary>
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

        /// <summary>Gets the relative probability that this buff appears.</summary>
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

        /// <summary>Chooses a buff quality using the current run's luck.</summary>
        /// <returns>The randomly selected buff quality.</returns>
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

        /// <summary>Gets the generated quality used for general buffs.</summary>
        protected Quality randomquality = GetRandomQuality();

        /// <summary>Gets the quality associated with this buff category.</summary>
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
    /// <summary>Adapts custom Rogue Shooting config data to the game's config API.</summary>
    public class RogueClass_CONFIG : BaseConfig
    {
        /// <summary>Wraps an existing IL2CPP config pointer.</summary>
        /// <param name="ptr">Pointer to the native config.</param>
        public RogueClass_CONFIG(IntPtr ptr) : base(ptr) { }

        /// <summary>Creates a game config from custom Rogue Shooting config data.</summary>
        /// <param name="cfg">The custom plant config definition.</param>
        public RogueClass_CONFIG(CustomRogueShootingConfig cfg)
            : base(ClassInjector.DerivedConstructorPointer<RogueClass_CONFIG>())
        {
            ClassInjector.DerivedConstructorBody(this);
            config = cfg;
        }

        /// <summary>Gets the custom config data represented by this config.</summary>
        public CustomRogueShootingConfig config { get; }

    #pragma warning disable CS0114
        /// <summary>Gets the plant type represented by this config.</summary>
        public PlantType PlantType => config.CustomPlantType;

        /// <summary>Gets the custom buffs available to this plant.</summary>
        public Il2CppSystem.Collections.Generic.List<BaseBuff> Buffs => config.CustomBuffs().ToIl2CppList();
    #pragma warning restore CS0114

        /// <summary>Invokes the custom reinforcement callback for the plant.</summary>
        /// <param name="plant">The plant to reinforce.</param>
        public override void ReinforcePlant(Plant plant)
        {
            if (config.CustomReinforcePlant != null)
                config.CustomReinforcePlant(plant);
        }

        /// <summary>Gets the custom role label for this plant.</summary>
        public override string Role => config.CustomRole;
    }
    /// <summary>Validates the almanac catalog and falls back to its last valid directory.</summary>
    [HarmonyPatch(typeof(ShootingAlmanacCatalog))]
	public static class ShootingAlmanacCatalogFixer
	{
        /// <summary>Allows the original catalog build only when its referenced configs exist.</summary>
        /// <param name="__result">Receives the last valid directory when the original build is skipped.</param>
        /// <returns><see langword="true"/> to run the original build; otherwise, uses the cached directory.</returns>
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
                global::CustomPlantClass.Runtime.Core.ModLogger.LogError("Something happened");
                return true;
            }
            __result = ShootingAlmanacCatalogPatch.LastGoodDir;
            return false;
        }
	}
}
