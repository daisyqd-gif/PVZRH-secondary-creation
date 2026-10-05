using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using BepInEx.Configuration;

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
            ConfigEntry<bool> cfg1 = Config.Bind("CustomPlantClass", "StarUpMgr : Increase star up chance.", false);
            cfg1.SettingChanged += (_,_) => StarUp_Patch.enable = cfg1.Value;
            StarUp_Patch.enable = cfg1.Value;
            ConfigEntry<KeyCode> cfg2 = Config.Bind("CustomPlantClass", "StarUpMgr : Force instant star up all plants botton.", KeyCode.Insert);
            cfg2.SettingChanged += (_,_) => KeyInstantStarUp.forceenable = cfg2.Value;
            KeyInstantStarUp.forceenable = cfg2.Value;
            ConfigEntry<KeyCode> cfg3 = Config.Bind("CustomPlantClass", "StarUpMgr : Try star up all plants botton.", KeyCode.End);
            cfg3.SettingChanged += (_,_) => KeyInstantStarUp.tryenable = cfg3.Value;
            KeyInstantStarUp.tryenable = cfg3.Value;
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    [HarmonyPatch(typeof(Plant), nameof(Plant.Start))]
    [HarmonyPriority(Priority.Last)]
    public static class StarUp_Patch
    {
        public static bool enable = false;
        [HarmonyPostfix]
        public static void Postfix(Plant __instance)
        {
            if(__instance==null || Random.Range(0, 100) > 10 || !enable || __instance.board==null || __instance.board.boardTag.rogueShooting || GameAPP.theGameStatus!=GameStatus.InGame || __instance.thePlantType==PlantType.Nothing)return;
            __instance.StarUp();
        }
    }
    [HarmonyPatch(typeof(Plant), nameof(Plant.FixedUpdate))]
    public static class KeyInstantStarUp
    {
        public static KeyCode forceenable = KeyCode.Insert;
        public static KeyCode tryenable = KeyCode.End;
        [HarmonyPrefix]
        public static void Prefix(Plant __instance)
        {
            if(__instance==null || __instance.board==null || GameAPP.theGameStatus!=GameStatus.InGame || __instance.thePlantType==PlantType.Nothing)return;
            if(__instance.starUp)return;
            // Only run when key is held
            if (Input.GetKey(forceenable))
            {
                __instance.starUp=true;
                __instance.UpdateStarIcon();
                return;
            }
            if (Input.GetKey(tryenable))
            {
                __instance.StarUp();
                return;
            }
        }
    }
}