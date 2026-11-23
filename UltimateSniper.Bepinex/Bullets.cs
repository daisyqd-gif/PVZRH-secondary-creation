using UnityEngine.Rendering.Universal;

namespace UltimateSniper
{
    public class FlamePea : BaseCustomBullet
    {
        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.IsObjExist()) return;
            if (!collision.TryGetComponent<SuperTorch>(out var plant) && !collision.TryGetComponent<UltimateTorch>(out var _)) return;
            if (collision.TryGetComponent<UltimateTorch>(out var torch2))
            {
                torch2.fireTimes++;
                if (torch2.fireTimes >= 6)
                {
                    torch2.fireTimes = 0;
                    torch2.SummonPlant();
                }
            }
            if (!plant.IsObjExist()) return;
            if (_bullet.torchWood == plant) return;
            var next = Plugin.BType_FireFlame;
            var addDamage = 60;
            _bullet.board.boardAction.FirePeas(_bullet, plant, addDamage, next);
        }
        public override bool HitZombie(Zombie zombie)
        {
            if (zombie != null && !zombie.beforeDying && zombie.col != null && !zombie.isMindControlled)
            {
                zombie.AddBurn();

                // Portal slow
                if(!Lawnf.TravelAdvanced(Plugin.Buff_Curse))
                {
                    zombie.SetPortaled(1.5f);
                    if (!TypeMgr.IsBossZombie(zombie.theZombieType))
                        zombie.SetPortaled(5f);
                }
            }
            zombie.TakeDamage(_bullet.Damage, _bullet.Cast<IDamageMaker>(), DamageType.NormalAll, _bullet.fromType);
            ParticleManager.Instance.SetParticle(ParticleType.Flame,transform.position,_bullet.theBulletRow);
            GameAPP.PlaySound(SoundType.FirePea);
            return false;
        }
    }
    public class FlamePea_Explosive : BaseCustomBullet
    {
        public override bool HitZombie(Zombie zombie)
        {
            Doom.SetDoom(_bullet.board, transform.position, 0, null, !GameAPP.config.distablexplodeFlash);
            foreach (Zombie z in Lawnf.GetAllZombies(false)) //locks out mind controlled zombies
            {
                if (z != null && !z.beforeDying && z.col != null)
                {
                    z.AddBurn();

                    // Portal slow
                    if(!Lawnf.TravelAdvanced(Plugin.Buff_Curse))
                    {
                        zombie.SetPortaled(1.5f);
                        if (!TypeMgr.IsBossZombie(zombie.theZombieType))
                            zombie.SetPortaled(5f);
                    }
                }
            }
            zombie.TakeDamage(_bullet.Damage, _bullet.ToIDamageMaker(), DamageType.NormalAll, _bullet.fromType);
            GameAPP.PlaySound(SoundType.FirePea);
            return false;
        }
    }
    public class FlamePea_Explosive_Ice : MonoBehaviour {}
    /*
    [HarmonyPatch(typeof(Bullet_sword), nameof(Bullet_sword.HitZombie))]
    public class Bullet_sword_HitZombie_patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Bullet_sword __instance, ref Zombie zombie)
        {
            if (__instance == null) return true;
            if (zombie == null) return true; //Run original logic
            Board board = __instance.board;
            if (board == null) return true; //Run original logic
            Vector2 pos = __instance.transform.position;
            if (__instance.TryGetComponent<FlamePea_Explosive>(out var _))
            {
                //__instance.Die();
                return false;
            }
            else if (__instance.TryGetComponent<FlamePea_Explosive_Ice>(out var _))
            {
                Doom.SetDoom(__instance.board, __instance.transform.position, DoomType.IceDoom, null, !GameAPP.config.distablexplodeFlash);
                foreach (Zombie z in Lawnf.GetAllZombies(false)) //locks out mind controlled zombies
                {
                    if (z != null && !z.beforeDying && z.col != null)
                    {
                        z.AddBurn();

                        // Portal slow
                        z.SetPortaled(1.5f);
                        if (!TypeMgr.IsBossZombie(z.theZombieType))
                            z.SetPortaled(5f);
                    }
                }
                zombie.TakeDamage(__instance.Damage, __instance.ToIDamageMaker(), DamageType.NormalAll, __instance.fromType);
                GameAPP.PlaySound(61, 0.5f, 1f);
                //__instance.Die();
                return false;
            }

            return true;
        }
    }
    */
}