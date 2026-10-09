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
namespace CustomPlantClass.Runtime.Tasks
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<DelayScheduler>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerLoopTask>();
            ClassInjector.RegisterTypeInIl2Cpp<WaitUntilScheduler>();
            ClassInjector.RegisterTypeInIl2Cpp<CancellationTokenExt.MonobehaviourCancellationToken>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            AddComponent<DelayScheduler>();
            AddComponent<WaitUntilScheduler>();
            PlayerLoopTask.AddToPlayerLoop(() => PlayerLoopTask.OnPlayerLoop());
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.Runtime.Tasks.Bepinex";
        public const string PluginName = "CustomPlantClass.Runtime.Tasks";
        public const string PluginVersion = "1.0.0";
    }
    /// <summary>
    /// Helper for registering events that run on player loop update
    /// </summary>
    public class PlayerLoopTask : Il2CppSystem.Object
    {
        private static readonly List<Action> Run = new();

        /// <summary>
        /// Registers an even that runs every 
        /// </summary>
        /// <param name="run"></param>
        public static void AddRunAction(Action run)
        {
            if (run != null) Run.Add(run);
        }
        public static void AddToPlayerLoop(Action run, Type type = null)
        {
            bool useUniqueType = false;
            if(type != null && type.IsAssignableFrom(typeof(Il2CppSystem.Object)))
            {
                if(ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                    ClassInjector.RegisterTypeInIl2Cpp(type);
                useUniqueType = true;
            }
            PlayerLoopSystem pl = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystem playerLoopSystem = new()
            {
                updateDelegate = run,
                type = Il2CppType.From(useUniqueType ? type : typeof(PlayerLoopTask))
            };
            PlayerLoop.SetPlayerLoop(InsertSystemAfter<Update>(in pl,playerLoopSystem));
            AddRunAction(DelayScheduler.OnPlayerLoop);
            AddRunAction(WaitUntilScheduler.OnPlayerLoop);
        }

        public static void OnPlayerLoop()
        {
            var count = Run.Count;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    Run[i]?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
            }
        }
        
        internal static PlayerLoopSystem InsertSystemAfter<T>(in PlayerLoopSystem loopSystem, PlayerLoopSystem newSystem) where T : struct
        {
            // Create a new root PlayerLoopSystem
            PlayerLoopSystem newPlayerLoop = new()
            {
                loopConditionFunction = loopSystem.loopConditionFunction,
                type = loopSystem.type,
                updateDelegate = loopSystem.updateDelegate,
                updateFunction = loopSystem.updateFunction
            };
            // Create a new list to populate with subsystems, including the custom system
            List<PlayerLoopSystem> newSubSystemList = new();

            //Iterate through the subsystems in the existing loop we passed in and add them to the new list
            if (loopSystem.subSystemList != null)
            {
                for (var i = 0; i < loopSystem.subSystemList.Length; i++)
                {
                    newSubSystemList.Add(loopSystem.subSystemList[i]);
                    // If the previously added subsystem is of the type to add after, add the custom system
                    if (loopSystem.subSystemList[i].type == Il2CppType.From(typeof(T)))
                    {
                        newSubSystemList.Add(newSystem);
                        Debug.Log("Added system to playerloop");
                    }
                }
            }

            newPlayerLoop.subSystemList = newSubSystemList.ToArray();
            return newPlayerLoop;
        }
    }
}
