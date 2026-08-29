using UnityEngine.Rendering.Universal;

namespace UltimateSniper
{
    public class UltimateFirePea : MonoBehaviour
    {
        public static ID BULLET_ID = 23035;
    }
    public class UltimateCherryPea : MonoBehaviour
    {
        public static ID BULLET_ID = 23036;
    }
    public class UltimateExplosivePea : UltimateCherryPea
    {
        public static new ID BULLET_ID = 23037;
    }
    public class FlamePea : BulletComponent
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
                    torch2.fireTimes=0;
                    torch2.SummonPlant();
                }
            }
            if (!plant.IsObjExist()) return;
            if (bullet.torchWood == plant) return;
            var next = Plugin.BType_FireFlame;
            var addDamage = 60;
            bullet.board.boardAction.FirePeas(bullet, plant, addDamage, next);
        }
    }
    public class FlamePea_Explosive : FlamePea
    {
        //this is just a marker too
    }
    public class FlamePea_Explosive_Ice : MonoBehaviour
    {
        //this is just a marker too
    }
    [HarmonyPatch(typeof(Bullet_superCherry), nameof(Bullet_superCherry.HitZombie))]
    public class UltimateCherryPea_HitZombie
    {
        [HarmonyPrefix]
        public static bool Prefix(Bullet_superCherry __instance, ref Zombie zombie)
        {
            if (__instance == null) return true;
            if (zombie == null) return true; //Run original logic
            Board board = __instance.board;
            if (board == null) return true; //Run original logic
            Vector2 pos = __instance.transform.position;
            if (__instance.TryGetComponent<UltimateCherryPea>(out _))
            {
                Burn.TryAddZombieBurn(zombie).damage = __instance.Damage;
                if (__instance.TryGetComponent<UltimateExplosivePea>(out _))
                {
                    if(!GameAPP.config.distablexplodeFlash)Doom.SetDoom(__instance.board, __instance.transform.position, DoomType.Fire);
                    for (int i = 0; i < 9; i++)
                    {
                        Bullet b = InstanceManager.CreateBullet.SetBullet(pos.x, pos.y, zombie.theZombieRow, UltimateFirePea.BULLET_ID, BulletMoveWay.Free, false);
                        b.transform.Rotate(0f, 0f, i * 40f);
                        b.normalSpeed *= 2;
                        b.Damage = 1000;
                    }
                }
            }

            return true;
        }
    }
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
            if (__instance.TryGetComponent<FlamePea>(out var _))
            {
                if (zombie != null && !zombie.beforeDying && zombie.col != null && !zombie.isMindControlled)
                {
                    zombie.AddBurn();

                    // Portal slow
                    zombie.SetPortaled(1.5f);
                    if (!TypeMgr.IsBossZombie(zombie.theZombieType))
                        zombie.SetPortaled(5f);
                }
                zombie.TakeDamage(__instance.Damage,__instance.ToIDamageMaker(),DamageType.NormalAll,__instance.fromType);
                CreateParticle.SetParticle(73, __instance.transform.position, __instance.theBulletRow, true);
                GameAPP.PlaySound(61, 0.5f, 1f);
                //__instance.Die();
                return false;
            }
            else if (__instance.TryGetComponent<FlamePea_Explosive>(out var _))
            {
                Doom.SetDoom(__instance.board, __instance.transform.position, 0, null,!GameAPP.config.distablexplodeFlash);
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
                zombie.TakeDamage(__instance.Damage,__instance.ToIDamageMaker(),DamageType.NormalAll,__instance.fromType);
                GameAPP.PlaySound(61, 0.5f, 1f);
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
                zombie.TakeDamage(__instance.Damage,__instance.ToIDamageMaker(),DamageType.NormalAll,__instance.fromType);
                GameAPP.PlaySound(61, 0.5f, 1f);
                //__instance.Die();
                return false;
            }

            return true;
        }
    }
}