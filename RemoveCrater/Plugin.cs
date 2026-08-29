global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using HarmonyLib;
global using System.Reflection;


namespace RemoveCrater{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "RemoveCrater.Bepinex";
        public const string PluginName = "RemoveCrater";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }
    }
    [HarmonyPatch(typeof(Crater),nameof(Crater.Awake))]
    public class Crater_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Crater __instance)
        {
            __instance.Die();
        }
    }
    [HarmonyPatch(typeof(Lawnf),nameof(Lawnf.TravelAdvanced))]
    public class Lawnf_TravelAdvanced_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(AdvBuff buff, ref bool __result)
        {
            if(buff==AdvBuff.EnumValue18) __result=true;
        }
    }
}
