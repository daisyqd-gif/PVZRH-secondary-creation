global using BepInEx;
global using Il2CppInterop.Runtime.Injection;
global using UnityEngine;
global using BepInEx.Unity.IL2CPP;
global using BepInEx.Logging;
namespace CustomPlantClass.Networking
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Logger;
        public override void Load()
        {
            Logger=Log;
            ClassInjector.RegisterTypeInIl2Cpp<TCPManager.TCPBehaviour>();
            AddComponent<TCPManager.TCPBehaviour>();
            TCPManager.TCPBehaviour.OnLoad();
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded as an expansion to CustomPlantClass.");
        }
        public override bool Unload()
        {
            TCPManager.StopCommunication();
            return base.Unload();
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.Networking.Bepinex";
        public const string PluginName = "CustomPlantClass.Networking";
        public const string PluginVersion = "1.0.0";
    }
}
