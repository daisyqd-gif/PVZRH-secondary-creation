#pragma warning disable CS1591 // Missing XML comment for publicly visible type or memberusing System.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomPlantClass.Registry;
using HarmonyLib;
[assembly: InternalsVisibleTo("CustomPlantClass")]

namespace CustomPlantClass.Runtime.Core
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public sealed class Plugin : BasePlugin
    {
        public static bool Loaded = false;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
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
            Loaded = true;
            new RegistryManager(Config);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static Queue<Action> queued = new();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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
        public const string PluginGuid = "CustomPlantClass.Runtime.Tasks.Bepinex";
        public const string PluginName = "CustomPlantClass.Runtime.Tasks";
        public const string PluginVersion = "1.0.0";
    }
}