global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using HarmonyLib;
global using UnityEngine;
global using Unity.VisualScripting;
global using Il2CppInterop.Runtime.Injection;
global using System.Reflection;

namespace UltimateSwordZombie_NoWudi
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<UltimateSwordZombie_NoWudi>();
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class UltimateSwordZombie_NoWudi : MonoBehaviour
    {
        UltimateSwordZombie z=>GetComponent<UltimateSwordZombie>();
        public void Update()
        {
            z.wudi=false;
            if(z.theHealth<=0) z.Die(1);
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateSwordZombie_NoWudi.Bepinex";
        public const string PluginName = "UltimateSwordZombie_NoWudi";
        public const string PluginVersion = "3.6.1";
    }
    [HarmonyPatch(typeof(UltimateSwordZombie))]
    public class UltimateSwordZombie_Patch
    {
        [HarmonyPatch(nameof(UltimateSwordZombie.Awake))]
        [HarmonyPrefix]
        public static void Awake_Prefix(UltimateSwordZombie __instance)
        {
            if(__instance!=null && !__instance.IsDestroyed()) __instance.AddComponent<UltimateSwordZombie_NoWudi>();
        }
    }
}
