global using BepInEx;
global using CustomizeLib.BepInEx;
global using System.Reflection;
global using UnityEngine;
global using CustomPlantClass.Main;
namespace MoreBossSlider
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "morebossslider"
            );
            //DataMgr.RegisterCustomBossHealthSlider(ZombieType.UltimateDolphin,assetBundle.GetAsset<GameObject>("DolphinSlider"));
            DataMgr.RegisterCustomBossHealthSlider(ZombieType.UltimateHorse2,assetBundle.GetAsset<GameObject>("Horse2Slider"));
            DataMgr.RegisterCustomBossHealthSlider(ZombieType.ZombieBoss,assetBundle.GetAsset<GameObject>("ZBossSlider"));
            DataMgr.RegisterCustomBossHealthSlider(ZombieType.ZombieBoss2,assetBundle.GetAsset<GameObject>("GoldenZBoss"));
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "MoreBossSlider.Bepinex";
        public const string PluginName = "MoreBossSlider";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
