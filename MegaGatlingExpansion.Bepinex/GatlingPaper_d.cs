namespace MegaGatlingExpansion
{
    public class GatlingPaperZombie_d : MonoBehaviour
    {
        public GatlingPaperZombie_c zombie => gameObject.GetComponent<GatlingPaperZombie_c>();
        public void Awake()
        {
            zombie.shoot = transform.FindChild("Zombie_head/Shoot");
            zombie.theSecondArmor = transform.FindChild("Zombie_paper_paper1").gameObject;
            zombie.theSecondArmorHealth = 6000;
            zombie.theSecondArmorMaxHealth = 6000;
            zombie.theSecondArmorType = Zombie.SecondArmorType.Paper;
            zombie.head_default = transform.FindChild("Zombie_head/head1").gameObject;
            zombie.head_angry = transform.FindChild("Zombie_head/head2").gameObject;
            zombie.gun = transform.FindChild("Zombie_paper_body/gun").gameObject.GetComponent<SortingGroup>();
            zombie.UpdateHealthText();
        }
        public virtual BulletType GetBulletType() => BulletType.Bullet_superCherry;
        public void AnimShootGun_Custom()
        {
            if (zombie.beforeDying || zombie.isChangingRow) return;
            for (int i = 0; i < 15; i++)
            {
                if (zombie == null || zombie.IsDestroyed()) return;
                Vector3 pos = zombie.shoot.position;
                Bullet b;
                {
                    b = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, zombie.theZombieRow,
                        GetBulletType(),
                        BulletMoveWay.Free,
                        !zombie.isMindControlled
                    );
                    b.Damage = zombie.theAttackDamage;
                }
                if (Lawnf.TravelDebuff((TravelDebuff)Plugin.Buff3)) b.SetData("UnReboundable", true);
                if (!zombie.isMindControlled) b.transform.Rotate(0, 0, Random.Range(180f - 15f, 180f + 15f));
                else b.transform.Rotate(0, 0, Random.Range(-15f, 15f));
                b.normalSpeed *= Random.Range(0.75f, 2f);
            }
            var iVar4 = Random.RandomRangeInt(3, 5);
            GameAPP.PlaySound(iVar4, 0.5f, 1.0f);
        }
        public virtual ZombieType GetZombieType() => ZombieType.GatlingPaper_c;
        public void TakeGun()
        {
            if (zombie == null || zombie.board == null || zombie.board.zombieArray == null)
                return;

            zombie.ChangeStatus(ZombieStatus.Paper_angry);
            zombie.Angry();

            var zombies = zombie.board.zombieArray.ToSystemList();
            int count = 0;

            foreach (Zombie z in zombies)
            {
                if (z == null) continue;
                if (z.IsDestroyed()) continue;
                if (z.board == null) continue;
                if (TypeMgr.IsBossZombie(z.theZombieType)) continue;
                if (TypeMgr.BigZombie(z.theZombieType)) continue;

                // spawn replacement
                Zombie newZombie;
                if (zombie.isMindControlled)
                {
                    newZombie = CreateZombie.Instance.SetZombieWithMindControl(
                        z.theZombieRow,
                        GetZombieType(),
                        z.axis.position.x
                    );
                }
                else
                {
                    newZombie = CreateZombie.Instance.SetZombie(
                        z.theZombieRow,
                        GetZombieType(),
                        z.axis.position.x
                    );
                }

                if (newZombie != null)
                {
                    // strip second armor immediately so it matches “gun taken” state
                    if (newZombie.theSecondArmorMaxHealth > 0)
                        newZombie.TakeDamage(DmgType.Normal, newZombie.theSecondArmorMaxHealth);
                }

                z.Die(1);
                count++;
                if (count >= 30)
                    return;
            }
        }
        public void AnimShoot()
        {
            if (GameAPP.theGameStatus == GameStatus.InGame && zombie != null && zombie.board != null && zombie.theStatus == ZombieStatus.Paper_angry)
            {
                AnimShootGun_Custom();
            }
        } 
    }
    public class DoomPaperZombie_d : GatlingPaperZombie_d
    {
        public override BulletType GetBulletType() => BulletType.Bullet_doom_ulti;
        public override ZombieType GetZombieType() => ZombieType.DoomPaper;
    }
}
