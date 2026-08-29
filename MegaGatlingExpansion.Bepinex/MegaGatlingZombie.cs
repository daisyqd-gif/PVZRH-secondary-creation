namespace MegaGatlingExpansion
{
    public class MegaGatlingPeaZombie : MonoBehaviour
    {
        public bool isPF = false;
        public PeaShooterZ zombie => GetComponent<PeaShooterZ>();
        public bool dead = false;
        public int attr;
        // -------------------------
        //  CUSTOM START LOGIC
        // -------------------------
        public virtual void Awake()
        {
            zombie.shoot = transform.FindChild(GetShootPath());
        }

        public virtual string GetShootPath()
        {
            return "Zombie_head/NormalHead/Shoot";
        }

        public virtual void Update()
        {
            if (zombie != null && dead == false && (zombie.theHealth <= 0 || zombie.beforeDying))
            {
                dead = true;
                zombie.Die();
            }
        }

        private static readonly BulletType[] AllCustomBullets =
        {
            BulletType.Bullet_extremeSnowPea,
            BulletProfile.ExtremeFirePea,
            BulletProfile.PrimalPea,
            BulletProfile.GooPea,
            BulletProfile.ElectricPea,
            BulletType.Bullet_cherry,
            BulletProfile.HypnoMegaPeaZ,
            BulletType.Bullet_portalPea,
            BulletType.Bullet_pea,
            BulletType.Bullet_pea,
            BulletType.Bullet_pea,
            BulletType.Bullet_pea
        };

        public virtual BulletType GetBulletType_Custom()
        {
            if (Plugin.Buff2 != -1 && Lawnf.TravelDebuff((TravelDebuff)Plugin.Buff2)) return AllCustomBullets[Random.Range(0, AllCustomBullets.Length)];
            else return BulletType.Bullet_pea;
        }


        // -------------------------
        //  CUSTOM SHOOT LOGIC
        // -------------------------
        public virtual IEnumerator Shooting_Custom()
        {
            BulletMoveWay moveway;
            if (zombie.isMindControlled)
            {
                moveway = BulletMoveWay.MoveRight;
            }
            else
            {
                moveway = BulletMoveWay.Left;
            }
            for (int i = 0; i < 4 + attr; i++)
            {
                if (zombie == null || zombie.IsDestroyed() || zombie.shoot==null || zombie.beforeDying)
                {
                    yield break;
                } 
                Vector3 pos = zombie.shoot.position;
                Bullet b;
                {
                    b = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, zombie.theZombieRow,
                        GetBulletType_Custom(),
                        moveway,
                        !zombie.isMindControlled
                    );
                    b.Damage = zombie.theAttackDamage;
                    if (Lawnf.TravelDebuff((TravelDebuff)Plugin.Buff3)) b.SetData("UnReboundable", true);
                }
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
        }

        public virtual IEnumerator SuperShoot_Custom()
        {
            isPF = true;
            attr = Math.Min(attr + 1, 3);
            zombie.anim.SetBool("isSuper", true);
            if (zombie.theZombieType == (ZombieType)9002)
            {
                for (int i = 0; i < zombie.board.rowNum; i++)
                {
                    if (!zombie.isMindControlled) CreateZombie.Instance.SetZombie(i, (ZombieType)9000);
                    else CreateZombie.Instance.SetZombieWithMindControl(i, (ZombieType)9000, Mouse.Instance.GetBoxXFromColumn(-1));
                }
            }

            int total = 180;
            if (Plugin.Buff2 != -1 && Lawnf.TravelDebuff((TravelDebuff)Plugin.Buff3)) total += 30;

            for (int i = 0; i < total; i++)
            {
                if (zombie == null || zombie.IsDestroyed() || zombie.shoot==null || zombie.beforeDying) yield break;
                Vector3 pos = zombie.shoot.position;
                Bullet b;
                {
                    b = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, zombie.theZombieRow,
                        GetBulletType_Custom(),
                        BulletMoveWay.Free,
                        !zombie.isMindControlled
                    );
                    b.Damage = zombie.theAttackDamage;
                    if (Lawnf.TravelDebuff((TravelDebuff)Plugin.Buff3)) b.SetData("UnReboundable", true);
                }
                if (!zombie.isMindControlled) b.transform.Rotate(0, 0, Random.Range(180f - 15f, 180f + 15f));
                else b.transform.Rotate(0, 0, Random.Range(-15f, 15f));
                yield return new WaitForFixedUpdate();
            }
            zombie.anim.SetBool("isSuper", false);
            isPF = false;
        }

        public Bullet AnimShoot_Custom()
        {
            if ((Lawnf.TravelUltimate(UltiBuff.EnumValue50) && Random.Range(0, 100) <= 30) || Random.Range(0, 100) <= 10)
            {
                isPF = true;
                zombie.StartCoroutine(SuperShoot_Custom());
                return null;
            }

            zombie.StartCoroutine(Shooting_Custom());
            return null;
        }
    }
}
