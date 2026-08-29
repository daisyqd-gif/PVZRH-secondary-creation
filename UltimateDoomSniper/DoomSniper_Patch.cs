namespace UltimateDoomSniper
{
    [HarmonyPatch(typeof(DoomSniper))]
    [HarmonyPriority(Priority.Last)]
    public static class DoomSniper_Patch
    {
        [HarmonyPatch(nameof(DoomSniper.CheckZombie))]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPrefix]
        public static bool CheckZombie_Prefix(DoomSniper __instance, Zombie zombie, ref bool __result)
        {
            if (__instance == null || __instance.IsDestroyed() || zombie == null || zombie.IsDestroyed()) return true;
            if (__instance.GetComponent<UltimateDoomSniper>() == null && __instance.thePlantType != PlantType.DoomSniper) return true;
            if (zombie == null || zombie.isMindControlled || zombie.beforeDying)
            {
                __result = false;
                return false;
            }

            var col = zombie.col;
            if (col == null)
            {
                __result = false;
                return false;
            }
            if (!col.enabled)
            {
                __result = false;
                return false;
            }

            bool dir = zombie.axis.position.x <= __instance.shoot.position.x;
            if (__instance.TryGetComponent<UltimateDoomSniper>(out var c))
            {
                c.Flip(dir);
            }
            else
            {
                if (dir)
                {
                    __result = false;
                    return false;
                }
            }
            __result = Lawnf.InLandStatus(zombie.theStatus) || Lawnf.TravelAdvanced(Plugin.Buff1);
            return false;
        }

        [HarmonyPatch(nameof(DoomSniper.SearchZombie))]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPrefix]
        public static bool SearchZombie_Prefix(DoomSniper __instance, ref GameObject __result)
        {
            if (__instance == null || __instance.IsDestroyed()) return true;
            if (__instance.GetComponent<UltimateDoomSniper>() == null && __instance.thePlantType != PlantType.DoomSniper) return true;
            foreach (var zombie in Lawnf.GetAllZombies())
            {
                if (zombie != null && !zombie.beforeDying)
                {
                    Vector3 zombiePos = zombie.transform.position;
                    if (__instance.vision > zombiePos.x || __instance.TryGetComponent<UltimateDoomSniper>(out var _))
                    {
                        Vector3 shootPos = __instance.shoot.position;
                        if (__instance.TryGetComponent<UltimateDoomSniper>(out var _))
                        {
                            if (Lawnf.InLandStatus(zombie.theStatus) || zombie.theStatus==ZombieStatus.Flying && Lawnf.TravelAdvanced(Plugin.Buff1))
                            {
                                __result = zombie.gameObject;
                                return false;
                            }
                        }
                        else if (zombiePos.x > shootPos.x || __instance.TryGetComponent<UltimateDoomSniper>(out var _))
                        {
                            if ((__instance.SearchUniqueZombie(zombie) && Lawnf.InLandStatus(zombie.theStatus)) || (zombie.theStatus == ZombieStatus.Flying && Lawnf.TravelAdvanced(Plugin.Buff1)))
                            {
                                __result = zombie.gameObject;
                                return false;
                            }
                        }
                    }
                }
            }
            __result = null;
            return false;
        }
        [HarmonyPatch(nameof(DoomSniper.Shoot1))]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPrefix]
        public static bool Shoot1_Prefix(DoomSniper __instance)
        {
            if (__instance != null)
            {
                if (__instance.GetComponent<UltimateDoomSniper>() == null || __instance.thePlantType != PlantType.DoomSniper) return true;
                GameAPP.PlaySound(140, 0.5f, Random.Range(0.9f, 1.1f));

                Func<Zombie, bool> func = __instance.CheckZombie;
                Zombie nearestZombie = Lawnf.GetNearestZombie(
                    __instance.board,
                    __instance.shoot.position,
                    func);

                var shootFire = ParticleManager.Instance.SetParticle(ParticleType.ShootFire, __instance.shoot.position, 11);

                if (nearestZombie != null)
                {
                    if (nearestZombie.col == null)
                        return false;
                    Vector2 direction = (nearestZombie.col.bounds.center - __instance.shoot.position).normalized;

                    RaycastHit2D[] hits = Physics2D.RaycastAll(
                        __instance.shoot.position,
                        direction,
                        float.MaxValue,
                        __instance.zombieLayer);

                    shootFire.transform.rotation = MathHelper.DirectionToRotation(direction);

                    int damage = __instance.attackDamage;
                    if (Random.Range(0, 10) == 5)
                        damage *= 6;

                    var hitCount = 0;
                    foreach (var hit in hits)
                    {
                        if (hit.collider.TryGetComponent(out Zombie zombie))
                        {
                            var hasEmber = zombie.TryGetEffect<EmberEffect>(EffectType.Ember, out var _);
                            int finalDamage = hasEmber || Lawnf.TravelAdvanced(AdvBuff.EnumValue12002) ? damage * 6 : damage;
                            zombie.TakeDamage(DmgType.NormalAll, finalDamage, __instance.thePlantType);
                            hitCount++;

                            if (MathHelper.ApproximatelyZero(__instance.crazeTimer))
                                __instance.craze++;
                        }
                    }

                    __instance.attributeCount += hitCount;

                    if (__instance.attributeCount >= (Lawnf.TravelAdvanced(Plugin.Buff2) ? 151 : 301))
                    {
                        __instance.attributeCount = 0;
                        var dmg=__instance.attackDamage * 72;
                        __instance.board.boardAction.SetDoom(nearestZombie.Column, nearestZombie.theZombieRow, false, false, default, dmg*__instance.shootingLevel+dmg,
                            0, null, true, __instance.thePlantType);
                    }

                    if (__instance.craze >= (Lawnf.TravelAdvanced(Plugin.Buff2) ? 50 : 100))
                    {
                        __instance.crazeTimer = Lawnf.TravelAdvanced(Plugin.Buff2) ? 16 : 8;
                        __instance.craze = 0;
                    }

                }

                return false;
            }
            return true;
        }
    }
}
