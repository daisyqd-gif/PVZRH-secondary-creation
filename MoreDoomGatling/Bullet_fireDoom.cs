namespace MoreDoomGatling
{
    public class Bullet_fireDoom : MonoBehaviour
    {
        public static ID BULLET_ID=3150;
        public static ID BULLET_ID_BIG=3151;
        public static ID BULLET_ID_CHERRY=3152;
        public static ID BULLET_ID_CHERRY_BIG=3153;
        public static ID BULLET_ID_CHERRY_SQUASH=3154;
        public static ID BULLET_ID_CHERRY_SQUASH_BIG=3155;
        public Bullet b => gameObject.GetComponent<Bullet>();
        public void HitZombie(Zombie zombie)
        {
            if(b.board==null)return;
            Board board=b.board;
            Transform tr = transform;
            if (tr == null)
                return;

            Vector3 pos = tr.position;

            // Hit particle (0x1B → 27)
            GameObject particle = CreateParticle.SetParticle(
                27,
                pos,
                b.theBulletRow,
                true
            );

            // BIG DOOM BULLET (Status == 6)
            if (b.theStatus == BulletStatus.Doom_big)
            {
                if (particle == null)
                    return;

                Transform pTr = particle.transform;
                if (pTr == null)
                    return;

                // Double scale
                pTr.localScale *= 2f;

                pos = tr.position;

                // Big AOE explosion
                Action<Zombie> action = (Zombie z) => {z.SetJalaed();};
                board.boardAction.SetDoom(Mouse.Instance.GetColumnFromX(pos.x),b.theBulletRow,false,false,default,b.Damage,0,action,false,b.fromType);

                // Big doom sound (0x29 → 41)
                GameAPP.PlaySound(41, 0.5f, 1f);
            }
            else
            {
                // Normal doom hit sound (0x46 → 70)
                GameAPP.PlaySound(70, 0.5f, 1f);
            }

            // Doom column nuke chance (TravelAdvanced(2000))
            if (Lawnf.TravelAdvanced(AdvBuff.EnumValue2000) && b.ToIDamageMaker().Team!=Team.AI)
            {
                if (zombie != null && zombie.axis != null)
                {
                    Vector3 zPos = zombie.axis.position;
                    int col = Mouse.Instance.GetColumnFromX(zPos.x);
                    Action<Zombie> action = (Zombie z) => {z.SetJalaed();};
                    // Doom type 0x708 → 1800
                    board.boardAction.SetDoom(
                        col,
                        zombie.theZombieRow,
                        false,
                        false,
                        Vector2.zero,
                        1800,
                        0,
                        action,
                        true,
                        b.fromType
                    );

                    b.Die();
                    return;
                }
            }
            else if (zombie != null)
            {
                // Normal hit
                zombie.TakeDamage(
                    DmgType.Carred,         // damage reason
                    b.Damage,
                    b.fromType,
                    false
                );
                if(b.theBulletType==BULLET_ID_CHERRY || b.theBulletType==BULLET_ID_CHERRY_BIG)board.boardAction.CreateCherryExplode(particle.transform.position,b.theBulletRow,CherryBombType.Bullet,b.Damage,b.fromType);

                b.Die();

                // Chance to fire extra peas
                if (Lawnf.TravelAdvanced(AdvBuff.EnumValue31)) // 0x1F → 31
                {
                    int r = Random.Range(0, 50); // 0x32 → 50
                    if (r == 0  && b.ToIDamageMaker().Team!=Team.AI)
                    {
                        // Damage * 11 (0xB → 11)
                        // Effect ID 0xB5 → 181
                        board.boardAction.FirePeas(
                            b,
                            null,
                            b.Damage * 11,
                            BulletType.Bullet_nuclear
                        );
                    }
                }

                return;
            }
        }
    }
    /*
    [HarmonyPatch(typeof(Bullet_squash))]
    public class Bullet_Squash_Patch
    {
        [HarmonyPatch(nameof(Bullet_squash.HitZombie))]
        [HarmonyPrefix]
        public static void HitZombie_Prefix(Bullet_squash __instance, Zombie zombie)
        {
            if(__instance==null || __instance.board==null || zombie==null || (__instance.theBulletType!=Bullet_fireDoom.BULLET_ID_CHERRY_SQUASH && __instance.theBulletType!=Bullet_fireDoom.BULLET_ID_CHERRY_SQUASH_BIG))return;
            __instance.board.boardAction.CreateCherryExplode(__instance.transform.position,__instance.theBulletRow,CherryBombType.Normal,__instance.Damage,__instance.fromType);
            if(__instance.theBulletType!=Bullet_fireDoom.BULLET_ID_CHERRY_SQUASH_BIG) return;
            __instance.board.boardAction.SetDoom(Mouse.Instance.GetColumnFromX(__instance.transform.position.x),__instance.theBulletRow,false,damage:__instance.Damage,existParticle:false,fromType:__instance.fromType);
        }
        [HarmonyPatch(nameof(Bullet_squash.AttackZombie))]
        [HarmonyPrefix]
        public static void AttackZombie_Prefix(Bullet_squash __instance)
        {
            if(__instance==null || __instance.board==null || (__instance.theBulletType!=Bullet_fireDoom.BULLET_ID_CHERRY_SQUASH && __instance.theBulletType!=Bullet_fireDoom.BULLET_ID_CHERRY_SQUASH_BIG))return;
            __instance.board.boardAction.CreateCherryExplode(__instance.transform.position,__instance.theBulletRow,CherryBombType.Normal,__instance.Damage,__instance.fromType);
            if(__instance.theBulletType!=Bullet_fireDoom.BULLET_ID_CHERRY_SQUASH_BIG) return;
            __instance.board.boardAction.SetDoom(Mouse.Instance.GetColumnFromX(__instance.transform.position.x),__instance.theBulletRow,false,damage:__instance.Damage,existParticle:false,fromType:__instance.fromType);
        }
    }
    */
    /*
    [HarmonyPatch(typeof(Bullet))]
    public class Bullet_Patch
    {
        [HarmonyPatch(nameof(Bullet.HitZombie))]
        [HarmonyPrefix]
        public static void HitZombie_Prefix(Bullet __instance, Zombie zombie)
        {
            if(__instance==null)return;
            if(__instance.TryGetComponent<Bullet_fireDoom>(out var b))
            {
                b.HitZombie(zombie);
            }
        }
    }
    */
    /*
    [HarmonyPatch(typeof(UltimateTorch), "OnTriggerEnter2D")]
    public static class UltimateTorch_OnTriggerEnter2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(UltimateTorch __instance, Collider2D collision)
        {
            if (collision == null) return true;

            if (!collision.TryGetComponent<Bullet>(out var bullet))
                return true;

            if (bullet.theBulletType == Bullet_fireDoom.BULLET_ID_CHERRY)
            {
                int damage = bullet._damage*3;
                var newType = Bullet_fireDoom.BULLET_ID_CHERRY_SQUASH;
                __instance.board.boardAction.FirePeas(bullet, __instance, damage, newType);
                return false;
            }
            else if (bullet.theBulletType == Bullet_fireDoom.BULLET_ID_CHERRY_BIG)
            {
                int damage = bullet._damage*3;
                var newType = Bullet_fireDoom.BULLET_ID_CHERRY_SQUASH_BIG;
                __instance.board.boardAction.FirePeas(bullet, __instance, damage, newType);
                return false;
            }
            return true;
        }
    }
    */
}