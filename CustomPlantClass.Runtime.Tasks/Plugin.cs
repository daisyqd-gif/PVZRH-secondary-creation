#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using Il2CppInterop.Runtime.Injection;
global using System;
global using UnityEngine;
global using System.Collections.Generic;
global using System.Threading.Tasks;
global using UnityEngine.LowLevel;
global using HarmonyLib;
global using System.Reflection;
global using Il2CppInterop.Runtime;
global using UnityEngine.PlayerLoop;
global using System.Runtime.CompilerServices;
global using Unity.VisualScripting;
global using BepInEx.Configuration;
namespace CustomPlantClass.Runtime.Tasks
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        static DelayScheduler s;
        public static AllowedDelayTypes delayType = AllowedDelayTypes.Update;
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<DelayScheduler>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerLoopTask>();
            ClassInjector.RegisterTypeInIl2Cpp<WaitUntilScheduler>();
            ClassInjector.RegisterTypeInIl2Cpp<CancellationTokenExt.MonobehaviourCancellationToken>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            s = AddComponent<DelayScheduler>();
            AddComponent<WaitUntilScheduler>();
            PlayerLoopTask.AddToPlayerLoop(() => PlayerLoopTask.OnPlayerLoop());
            PlayerLoopTask.AddRunAction_internal
            (
                () => 
                {
                    if(delayType == AllowedDelayTypes.PlayerLoop)
                    {
                        DelayScheduler.OnPlayerLoop();
                        WaitUntilScheduler.OnPlayerLoop();
                    } 
                }
            );
            var cfg = Config.Bind("Settings", "Delay type", AllowedDelayTypes.PlayerLoop, new ConfigDescription("Sets the delay type (no game restart required)"));
            delayType = cfg.Value;
            cfg.SettingChanged += (s,e) => delayType = cfg.Value;
        }
    }
    public enum AllowedDelayTypes
    {
        PlayerLoop,
        Update
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.Runtime.Tasks.Bepinex";
        public const string PluginName = "CustomPlantClass.Runtime.Tasks";
        public const string PluginVersion = "1.0.7";
    }
}
