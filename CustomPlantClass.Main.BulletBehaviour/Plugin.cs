global using BepInEx;
global using HarmonyLib;
global using Il2CppInterop.Runtime.Injection;
global using System;
global using System.Reflection;
global using UnityEngine;
global using System.Collections.Generic;
global using System.IO;
global using System.Reflection.PortableExecutable;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
namespace CustomPlantClass.Main.BulletBehaviour
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        public override void InitializeMod()
        {
            PluginBehaviour.AddComponentToPlugin<BulletRegistryManager>();
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded as an expansion to CustomPlantClass.");
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.Main.BulletBehaviour.Bepinex";
        public const string PluginName = "CustomPlantClass.Main.BulletBehaviour";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.PluginVersion;
    }
}
