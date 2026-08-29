namespace UltimateSolarCoronaCabbage_Remade
{
    [HarmonyPatch(typeof(Solar), nameof(Solar.Awake))]
    public static class Solar_Awake_Patch
    {
        public static GameObject cachedobject;
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static void Prefix(Solar __instance)
        {
            cachedobject=__instance.gameObject;
        }
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        public static void Postfix()
        {
            if (Board.Instance == null)
                return;
            foreach (var plant in Board.Instance.boardEntity.plantArray)
                if ((plant != null && plant.thePlantType == Plugin.DataContainer.thePlantType) || SuperSolar.Instance != null)
                {
                    var superSolar = Object.Instantiate(Plugin.DataContainer.superSolar, new Vector3(-25f, 33f, 0f), Quaternion.identity, plant.board.transform).GetComponent<SuperSolar>();
                    superSolar.targetPosition = new Vector3(-7.88f, 3.24f);
                    if (cachedobject != null && !cachedobject.IsDestroyed())
                    {
                        Object.Destroy(cachedobject);
                    }
                    return;
                }
        }
    }
    [HarmonyPatch(typeof(Lawnf))]
    public static class LawnfPatch
    {
        [HarmonyPatch(nameof(Lawnf.GetPlantCount), new Type[] { typeof(PlantType), typeof(Board) })]
        public static void Postfix(ref PlantType theSeedType, ref Board board, ref int __result)
        {
            if (theSeedType == PlantType.UltimateCabbage)
                __result += Lawnf.GetPlantCount(Plugin.DataContainer.thePlantType, board);
        }
    }
    [HarmonyPatch (typeof(Board))]
	public class BoardGetSunPatch
	{
		[HarmonyPatch ("GetSun")]
		[HarmonyPostfix]
		public static void Postfix (Board __instance, float count)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			List<Plant> val = Lawnf.GetPlants(Plugin.DataContainer.thePlantType, __instance).ToSystemList();
			if (val != null && val.Count > 0) {
				__instance.theSun= __instance.theSun + (int)count;
			}
		}
	}
    [HarmonyPatch (typeof(Board), "SunUpdate")]
	public class BoardSunUpdatePatch
	{
		[HarmonyPostfix]
		public static void Postfix (Board __instance)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			if (Lawnf.GetPlants(Plugin.DataContainer.thePlantType, __instance).ToSystemList().Count > 0) {
				StaticMethod3 (__instance, StaticMethod1 (__instance) - StaticMethod2 ());
			}
		}
        //proxy calls
		static float StaticMethod1 (Board A_0)
		{
			return A_0.theFallingSunCountDown;
		}

		static float StaticMethod2 ()
		{
			return Time.deltaTime;
		}

		static void StaticMethod3 (Board A_0, float A_1)
		{
			A_0.theFallingSunCountDown = A_1;
		}
	}
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.DestoryZombie))]
    public static class ZombieDestroyZombiePatch
    {
        [HarmonyPrefix]
        public static void Prefix(Zombie __instance)
        {
            // Conditions:
            // 1. zombie != null
            // 3. Solar.Instance != null
            // 5. Game is in GameStatus.InGame (== 0)
            if (PlantMgr.IsNotNullMonoBehaviour(__instance,out var zombie) &&
                SuperSolar.Instance != null &&
                GameAPP.theGameStatus == GameStatus.InGame)
            {
                SuperSolarEmit.SetSuperSolarEmit(zombie.transform.position,zombie.theZombieRow,InstanceManager.Board.theSun*100);
            }
        }
    }
}
