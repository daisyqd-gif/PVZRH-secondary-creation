using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Reflection;

namespace Starupmgr{
    [BepInPlugin("FixCards.Bepinex", "FixCards", "3.4.1")]
    public class Core : BasePlugin
    {
        public const string PluginVersion = "3.4.1";
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }
    }
    [HarmonyPatch(typeof(CardUI), nameof(CardUI.OnMouseDown))]
    [HarmonyPriority(Priority.First)]
    public static class StarUp_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(CardUI __instance)
        {
            if (!GameAPP.resourcesManager.plantPreviews.TryGetValue((PlantType)__instance.theSeedType, out var _)) {
                __instance.thePlantType = PlantType.Present;
                __instance.ChangeCardSprite();
            }
        }
    }
}