namespace MoreSuper
{
    public class MoreSuper : MonoBehaviour
    {
        public bool isPF = false;
        public Plant plant => gameObject.GetComponent<Plant>();

        public void FixedUpdate()
        {
            if(isPF) plant.thePlantHealth = plant.thePlantMaxHealth;
        }

        public virtual void StartPF()
        {
            plant.invincible = true;
            plant.uncrashable = true;
            isPF = true;
            plant.flashCountDown = 5f;
            plant.isFlashing = true;

            // Wrap the coroutine so we can detect when it finishes
            StartCoroutine(PFWrapper().WrapToIl2Cpp());
        }

        private IEnumerator PFWrapper()
        {
            // Run the user‑overridable supershoot
            yield return SuperShoot().WrapToIl2Cpp();

            // When it finishes, call the overridable end hook
            SuperEnd();
        }

        // Overridable supershoot logic
        public virtual IEnumerator SuperShoot()
        {
            yield return null;
        }

        // Overridable end hook
        public virtual void SuperEnd()
        {
            plant.invincible = false;
            plant.uncrashable = false;
            isPF = false;
            plant.flashCountDown = 5f;
            plant.isFlashing = false;
        }
    }
    [HarmonyPatch(typeof(Plant))]
    public static class Plant_Patches
    {
        // -------------------------
        //  Die()
        // -------------------------
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Plant.Die))]
        public static bool Die_Prefix(Plant __instance, Plant.DieReason reason)
        {
            if (__instance.TryGetComponent<MoreSuper>(out var p) &&
                p.isPF &&
                (reason == Plant.DieReason.CrashInWater ||
                 reason == Plant.DieReason.ByFreeze ||
                 reason == Plant.DieReason.Default))
            {
                __instance.thePlantHealth = __instance.thePlantMaxHealth;
                return false;
            }

            if (reason == Plant.DieReason.BySteal)
            {
                CreatePlant.Instance.SetPlant(
                    __instance.thePlantColumn,
                    __instance.thePlantRow,
                    __instance.thePlantType,
                    null,
                    default,
                    true,
                    false,
                    null
                );
            }

            return true;
        }

        // -------------------------
        //  Crashed()
        // -------------------------
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Plant.Crashed))]
        public static bool Crashed_Prefix(Plant __instance)
        {
            if (__instance.TryGetComponent<MoreSuper>(out var p) && p.isPF)
            {
                __instance.thePlantHealth = __instance.thePlantMaxHealth;
                return false;
            }
            return true;
        }

        // -------------------------
        //  PlantShootUpdate()
        // -------------------------
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Plant.PlantShootUpdate))]
        public static bool PlantShootUpdate_Prefix(Plant __instance)
        {
            if (__instance.TryGetComponent<MoreSuper>(out var p) && p.isPF)
                return false;

            return true;
        }
    }
}
