global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using Il2CppInterop.Runtime.Injection;
global using System;
global using System.Reflection;
global using UnityEngine;
global using TMPro;
global using System.Collections;
global using System.Collections.Generic;
global using CustomPlantClass;
global using CustomPlantClass.Main;
global using Unity.VisualScripting;
global using UnityEngine.EventSystems;
global using UnityEngine.UI;
namespace StoreMgr
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public class DataContainer
        {
            public static ID PlantId = -1;
        }
        public override void InitializeMod()
        {
            assetBundle = AssetMgr.LoadBundleFromResource(
                Assembly.GetExecutingAssembly(),
                "abname",
                false
            );
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
        public static void LoadStoreMgr()
        {

        }
        public class MyPluginInfo
        {
            public const string PluginGuid = "Template.Bepinex";
            public const string PluginName = "Template";
            public const string PluginVersion = "3.7";
        }
    }
}
