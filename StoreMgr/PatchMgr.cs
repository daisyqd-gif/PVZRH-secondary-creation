namespace StoreMgr
{
    [HarmonyPatch(typeof(GameAPP))]
    public class GameAPP_Patch
    {
        [HarmonyPatch(nameof(GameAPP.Awake))]
        [HarmonyPriority(Priority.VeryLow)]
        [HarmonyPrefix]
        public static void Awake_Prefix()
        {
            Plugin.LoadStoreMgr();
        }
    }
}