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
global using CustomPlantClass.Level;
global using Unity.VisualScripting;
namespace CustomPlantClass.Unmanaged
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Plugin : ModPlugin
    {
        public override void InitializeMod()
        {
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded as an expansion to CustomPlantClass.\nDo not misuse this tool otherwise legal action will be pursued. It is against law to deliberately hack other people.");
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.Unmanaged.Bepinex";
        public const string PluginName = "CustomPlantClass.Unmanaged";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
