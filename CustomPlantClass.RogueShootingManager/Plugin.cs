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
