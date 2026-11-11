#pragma warning disable CS1591 // Missing XML comment for publicly visible type or memberusing System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using CustomPlantClass.Registry;
using HarmonyLib;
using UnityEngine;
[assembly: InternalsVisibleTo("CustomPlantClass")]

namespace CustomPlantClass.Runtime.Core
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public sealed class Plugin : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            new RegistryManager(Config);
            PluginBehaviour.Initialize(this);
        }
        /// <summary>
        /// Queues an action to run if called before CustomPlantClass loads and runs it if CustomPlantClass is loaded already.
        /// </summary>
        /// <param name="a">The action to run when the plugin loads</param>
        public static void QueueOrExecute(Action a)
        {
            PluginBehaviour.QueueOrExecute(a);
        }
    }
    public sealed class PluginBehaviour : MonoBehaviour
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static PluginBehaviour Instance { get; internal set; }
        public static GameObject BehaviourObj { get; internal set; }
        public static GameObject ManagerObj { get; internal set; }
        public static Queue<Action> queued = new();
        public static bool Loaded = false;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        internal static void Initialize(BasePlugin plugin)
        {
            Instance=plugin.AddComponent<PluginBehaviour>();
            Instance.StartCoroutine(Init().WrapToIl2Cpp());
            Loaded = true;
            while (queued.Count > 0)
            {
                try
                {
                    queued.Dequeue()();
                }
                catch (Exception e)
                {
                    ModLogger.LogError(e.ToString());
                }
            }
        }
        internal static IEnumerator Init()
        {
            yield return null;
            BehaviourObj = new GameObject("CustomPlantClassBehaviour");
            DontDestroyOnLoad(BehaviourObj);
            BehaviourObj.hideFlags |= HideFlags.HideAndDontSave;
            BehaviourObj.transform.position = new Vector3(0f, 0f, 1f);
        }
        /// <summary>
        /// Queues an action to run if called before CustomPlantClass loads and runs it if CustomPlantClass is loaded already.
        /// </summary>
        /// <param name="a">The action to run when the plugin loads</param>
        public static void QueueOrExecute(Action a)
        {
            if (Loaded) a();
            else queued.Enqueue(a);
        }
    }
    internal static class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.Runtime.Core.Bepinex";
        public const string PluginName = "CustomPlantClass.Runtime.Core";
        public const string PluginVersion = "1.0.7";
    }
}