global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using Il2CppInterop.Runtime.Injection;
global using System;
global using UnityEngine;
global using System.Collections.Generic;
namespace CustomPlantClass.Runtime.Tasks
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<DelayScheduler>();
            ClassInjector.RegisterTypeInIl2Cpp<WaitUntilScheduler>();
            AddComponent<DelayScheduler>();
            AddComponent<WaitUntilScheduler>();
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.Runtime.Tasks.Bepinex";
        public const string PluginName = "CustomPlantClass.Runtime.Tasks";
        public const string PluginVersion = "1.0.0";
    }
}
