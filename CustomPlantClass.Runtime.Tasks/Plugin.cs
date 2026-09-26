global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using Il2CppInterop.Runtime.Injection;
global using System;
global using UnityEngine;
global using System.Collections.Generic;
global using static CustomPlantClass.Runtime.Tasks.CancellationTokenExt;
namespace CustomPlantClass.Runtime.Tasks
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<DelayScheduler>();
            ClassInjector.RegisterTypeInIl2Cpp<WaitUntilScheduler>();
            ClassInjector.RegisterTypeInIl2Cpp<MonobehaviourCancellationToken>();
            AddComponent<DelayScheduler>();
            AddComponent<WaitUntilScheduler>();

            /*
            ClassInjector.RegisterTypeInIl2Cpp<PlayerLoopMgr>();
            var insert = new PlayerLoopSystem();
            var action = PlayerLoopMgr.Do;
            insert.updateDelegate = action;
            insert.type = Il2CppType.From(typeof(PlayerLoopMgr));
            insert.subSystemList = null;
            insert.loopConditionFunction = IntPtr.Zero;
            InsertPlayerLoop(insert, typeof(Update));
        }
        private static void InsertPlayerLoop(PlayerLoopSystem loopSystem, Type targetType)
        {
            var origin = PlayerLoop.GetCurrentPlayerLoop();
            if (origin == null) return;
            if (origin.subSystemList == null) origin.subSystemList = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<PlayerLoopSystem>(0);
            for (int i = 0; i < origin.subSystemList.Length; i++)
            {
                var item = origin.subSystemList[i];
                if (item.type == Il2CppType.From(targetType))
                {
                    var oldSystems = item.subSystemList;
                    var newSystems = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<PlayerLoopSystem>(oldSystems.Length + 1);
                    newSystems[0] = loopSystem;
                    oldSystems.CopyTo(newSystems, 1);
                    item.subSystemList = newSystems;
                    origin.subSystemList[i] = item;
                    break;
                }
            }
            PlayerLoop.SetPlayerLoop(origin);//*/
        }
    }
    /*
    public class PlayerLoopMgr : Il2CppSystem.Object
    {
        public static void Do()
        {
            DelayScheduler.OnPlayerLoop();
            WaitUntilScheduler.OnPlayerLoop();
        }
    }//*/

    public class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.Runtime.Tasks.Bepinex";
        public const string PluginName = "CustomPlantClass.Runtime.Tasks";
        public const string PluginVersion = "1.0.0";
    }
}
