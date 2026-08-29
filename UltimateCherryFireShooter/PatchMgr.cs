namespace UltimateCherryFireShooter_Remade
{
    [HarmonyPatch(typeof(Plant))]
    public class Plant_Patch
    {
        [HarmonyPatch(nameof(Plant.PlantShootUpdate))]
        [HarmonyPrefix]
        public static bool PlantShootUpdate_Prefix(Plant __instance)
        {
            if (__instance.TryGetComponent<UltimateCherryFireShooter_Remade>(out var a))
            {
                if(a.isPF) return false;
            }
            if (__instance.TryGetComponent<FinalCherryFireShooter_Remade>(out var b))
            {
                if(b.isPF) return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Bullet_superCherry), nameof(Bullet_superCherry.HitZombie))]
    public static class SuperCherryBulletHitZombiePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Bullet_superCherry __instance, Zombie zombie)
        {
            if (zombie == null)
                return true;

            // FireCherry & FireCherryFire → apply Jalaed
            if (__instance.theBulletType == Plugin.Bullet_FireCherry ||
                __instance.theBulletType == Plugin.Bullet_FireCherryFire)
            {
                zombie.SetJalaed();
                return true;
            }

            // FireCherryFinal & FireCherryFinalFire → Jalaed + explode
            if (__instance.theBulletType == Plugin.Bullet_FireCherryFinal ||
                __instance.theBulletType == Plugin.Bullet_FireCherryFinalFire)
            {
                zombie.SetJalaed();
                zombie.JalaedExplode(true, 100);
                return true;
            }

            return true;
        }
    }
}
