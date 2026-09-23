global using BepInEx;
global using HarmonyLib;
global using UnityEngine;
global using System.Linq;
global using BepInEx.Unity.IL2CPP;
global using System.Collections.Generic;
global using System.Reflection;
namespace MachineNutBuff
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "MachineNutBuff.Bepinex";
        public const string PluginName = "MachineNutBuff";
        public const string PluginVersion = "4.0";
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
                __instance.thePlantHealth=Mathf.Clamp(__instance.thePlantHealth,0,int.MaxValue - short.MaxValue);
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
                var list = new List<Plant>([..Lawnf.GetAllPlants()])
                    .Where((Plant p)=>p.thePlantType==PlantType.SuperMachineNut);

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
            var canCrash = 
                new List<Plant>([..Lawnf.Get3x3Plants(__instance.thePlantColumn,__instance.thePlantRow)])
                    .Any((p) => p != null && p.thePlantType == PlantType.SuperMachineNut) || 
                new List<Plant>([..Lawnf.GetAllPlants()])
                    .Any((p) => p != null&& p.thePlantType == PlantType.UltimateMachineNut);
            __instance.uncrashable=canCrash;
            return !canCrash;
        }
    }
}
