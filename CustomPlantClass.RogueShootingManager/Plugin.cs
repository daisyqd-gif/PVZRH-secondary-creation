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
            AddComponent<RegistryHelper>();
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded as an expansion to CustomPlantClass.");
        }
    }

    public enum ShootingBuffType
    {
        UniqueUpgrade = 0,
        QualitativeChange = 1,
        SuperUpgrade = 2,
        General = 3,
        CurseBuff = 4
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.RogueShootingManager.Bepinex";
        public const string PluginName = "CustomPlantClass.RogueShootingManager";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.PluginVersion;
    }
}
