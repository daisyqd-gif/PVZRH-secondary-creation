global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using System;
global using System.Collections.Generic;
global using BepInEx.Logging;
global using System.Reflection;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis;
global using System.IO;
global using System.Threading;
global using System.Linq;
namespace CustomPlantClass.Runtime.CSharp
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public ManualLogSource Logger => Log;
        public static Plugin Instance { get; private set; }
        public override void Load()
        {
            Instance = this;
            var assemblies = Compile.LoadAllScripts();
            Compile.ExecuteAllScripts(assemblies);
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.Runtime.CSharp.Bepinex";
        public const string PluginName = "CustomPlantClass.Runtime.CSharp";
        public const string PluginVersion = "1.0.0";
    }
}
