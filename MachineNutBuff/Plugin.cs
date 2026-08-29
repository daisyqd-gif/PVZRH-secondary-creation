global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using UnityEngine;
global using CustomPlantClass.Main;
global using System.Linq;
global using BepInEx.Unity.IL2CPP;
namespace MachineNutBuff
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Tools.GetAssembly());
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "MachineNutBuff.Bepinex";
        public const string PluginName = "MachineNutBuff";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
    [HarmonyPatch(typeof(SuperMachineNut))]
    public static class SuperMachineNut_Patch
    {
        [HarmonyPatch(nameof(SuperMachineNut.LimHealth))]
        [HarmonyPrefix]
        public static bool LimHealth_Prefix(SuperMachineNut __instance)
        {
            if (__instance.board.boardTag.isSuperRandom)
            {
                __instance.thePlantHealth=Mathf.Clamp(__instance.thePlantHealth,0,1000000000);
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Plant))]
    public class Plant_Patch
    {
        [HarmonyPatch(nameof(Plant.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(Plant __instance)
        {
            if (__instance.thePlantType==PlantType.SuperMachineNut && __instance.board.boardTag.isSuperRandom)
            {
                __instance.thePlantHealth*=40;
            }
        }
        [HarmonyPatch(nameof(Plant.Die))]
        [HarmonyPostfix]
        public static void DieEvent_Postfix(Plant __instance)
        {
            if (__instance.thePlantType==PlantType.SuperMachineNut)
            {
                var list = Lawnf.GetAllPlants().ToSystemList().Where((Plant p)=>p.thePlantType==PlantType.SuperMachineNut);

                float avgHealth = 0f;
                var health = Mathf.Max(8000, __instance.thePlantHealth);
                if (list.Count() != 0)
                    avgHealth = health / list.Count();
                foreach (var plant in list)
                    plant.Recover(avgHealth);
            }
        }
        [HarmonyPatch(nameof(Plant.Crashed))]
        [HarmonyPrefix]
        public static bool Crashed_Prefix(Plant __instance)
        {
            var a = PlantMgr.GetPlantIn3x3(__instance.thePlantColumn,__instance.thePlantRow,PlantType.SuperMachineNut) != null;
            __instance.uncrashable=a;
            return !a;
        }
    }
}
