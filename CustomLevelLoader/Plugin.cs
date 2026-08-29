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
global using System.IO;
global using System.Linq;
global using CustomPlantClass;

namespace CustomLevelLoader
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Core : BasePlugin
    {
        public static AssetBundle assetBundle;
        public override void Load()
        {
            // Apply all Harmony patches in this assembly
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            // Register the custom plant class with IL2CPP
            // (Required for all custom MonoBehaviours)
            ClassInjector.RegisterTypeInIl2Cpp<CustomBgComponent>();
            ClassInjector.RegisterTypeInIl2Cpp<CustomBoardComponent>();
            ClassInjector.RegisterTypeInIl2Cpp<CustomGameLoseComponent>();
            ClassInjector.RegisterTypeInIl2Cpp<CustomLevelLoader>();
            ClassInjector.RegisterTypeInIl2Cpp<DefaultBoardData>();
            ClassInjector.RegisterTypeInIl2Cpp<AssetBuilder>();


            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "abname"
            );

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }
    public static class MyPluginInfo
    {
        public const string PluginGuid = "CustomLevelLoader.Bepinex";
        public const string PluginName = "CustomLevelLoader";
        public const string PluginVersion = "3.5";
        
    }
}
