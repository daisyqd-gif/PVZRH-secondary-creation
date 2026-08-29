using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace Starupmgr{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "Starupmgr.Bepinex";
        public const string PluginName = "Starupmgr";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    [HarmonyPatch(typeof(Plant), nameof(Plant.Start))]
    [HarmonyPriority(Priority.Last)]
    public static class StarUp_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __instance)
        {
            if(__instance==null || Random.Range(0, 100) > 10 || __instance.board==null || __instance.board.boardTag.rogueShooting || GameAPP.theGameStatus!=GameStatus.InGame || __instance.thePlantType==PlantType.Nothing)return;
            __instance.StarUp();
        }
    }
    [HarmonyPatch(typeof(Plant), nameof(Plant.Update))]
    public static class KeyInstantStarUp
    {
        [HarmonyPrefix]
        public static void Prefix(Plant __instance)
        {
            if(__instance==null || __instance.board==null || GameAPP.theGameStatus!=GameStatus.InGame || __instance.thePlantType==PlantType.Nothing)return;
            if(__instance.starUp)return;
            // Only run when key is held
            if (Input.GetKey(KeyCode.Insert))
            {
                __instance.starUp=true;
                __instance.UpdateStarIcon();
                return;
            }
            if (Input.GetKey(KeyCode.End))
            {
                __instance.StarUp();
                return;
            }
        }
    }
}