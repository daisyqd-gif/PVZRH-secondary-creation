namespace MoreDoomGatling
{
    public class GatlingDoomPaper_a : MonoBehaviour
    {
        public GatlingPaperZombie_a zombie => GetComponent<GatlingPaperZombie_a>();
        public void Awake()
        {
            zombie.head_default=transform.FindChild("Zombie_head/head1").gameObject;
            zombie.head_angry=transform.FindChild("Zombie_head/head2").gameObject;
            zombie.shoot=transform.FindChild("Zombie_head/Shoot");
            zombie.theSecondArmor=transform.FindChild("Zombie_paper_paper1").gameObject;
            zombie.theSecondArmorMaxHealth=3000;
            zombie.theSecondArmorHealth=3000;
            zombie.UpdateHealthText();
        }
        public void AnimShoot_Custom()
        {
            try
            {
                if(zombie.beforeDying || zombie.theStatus!=ZombieStatus.Paper_angry || zombie.isChangingRow) return;
                BulletMoveWay moveway=BulletMoveWay.Left;
                if (zombie.isMindControlled)
                {
                    moveway=BulletMoveWay.MoveRight;
                }
                Vector2 pos=zombie.shoot.position;
                Bullet b=CreateBullet.Instance.SetBullet(pos.x,pos.y,zombie.theZombieRow,BulletType.Bullet_doom_ulti,moveway,!zombie.isMindControlled);
                b.Damage=1000;
                b.shootByZombie=!zombie.isMindControlled;
                SoundType iVar4 = (SoundType)Random.RandomRangeInt(3,5);
                GameAPP.PlaySound(iVar4, 0.5f, 1.0f);
            }
            catch(Exception){}
        }
        public void Angry() => zombie.ChangeStatus(ZombieStatus.Paper_angry);
        public void TakeGun()
        {
            zombie.Angry();
            Angry();
        }
    }
    
    public class GatlingDoomPaper_b : MonoBehaviour
    {
        public GatlingPaperZombie_b zombie => GetComponent<GatlingPaperZombie_b>();
        public void Awake()
        {
            zombie.head_default=transform.FindChild("Zombie_head/head1").gameObject;
            zombie.head_angry=transform.FindChild("Zombie_head/head2").gameObject;
            zombie.shoot=transform.FindChild("Zombie_head/Shoot");
            zombie.theSecondArmor=transform.FindChild("Zombie_paper_paper1").gameObject;
            zombie.theSecondArmorMaxHealth=6000;
            zombie.theSecondArmorHealth=6000;
            zombie.UpdateHealthText();
        }
        public void AnimShoot_Custom()
        {
            try
            {
                if(zombie.beforeDying || zombie.theStatus!=ZombieStatus.Paper_angry || zombie.isChangingRow) return;
                BulletMoveWay moveway=BulletMoveWay.Left;
                if (zombie.isMindControlled)
                {
                    moveway=BulletMoveWay.MoveRight;
                }
                Vector2 pos=zombie.shoot.position;
                Bullet b=CreateBullet.Instance.SetBullet(pos.x,pos.y,zombie.theZombieRow,BulletType.Bullet_doom_ulti,moveway,!zombie.isMindControlled);
                b.Damage=1000;
                b.shootByZombie=!zombie.isMindControlled;
                SoundType iVar4 = (SoundType)Random.RandomRangeInt(3,5);
                GameAPP.PlaySound(iVar4, 0.5f, 1.0f);
            }
            catch(Exception){}
        }
        public void Angry() => zombie.ChangeStatus(ZombieStatus.Paper_angry);
        public void TakeGun()
        {
            zombie.Angry();
            Angry();
        }
    }
    public class GatlingDoomPaper_c : MonoBehaviour
    {
        public GatlingPaperZombie_c zombie => GetComponent<GatlingPaperZombie_c>();
        public void Awake()
        {
            zombie.head_default=transform.FindChild("Zombie_head/head1").gameObject;
            zombie.head_angry=transform.FindChild("Zombie_head/head2").gameObject;
            zombie.shoot=transform.FindChild("Zombie_head/Shoot");
            zombie.shoot_gun=transform.FindChild("Zombie_paper_body/gun/Shoot2");
            zombie.gun=transform.FindChild("Zombie_paper_body/gun").GetComponent<SortingGroup>();
            zombie.theSecondArmor=transform.FindChild("Zombie_paper_paper1").gameObject;
            zombie.theSecondArmorMaxHealth=12000;
            zombie.theSecondArmorHealth=12000;
            zombie.UpdateHealthText();
        }
        public void AnimShoot_Custom()
        {
            try
            {
                if(zombie.beforeDying || zombie.theStatus!=ZombieStatus.Paper_angry || zombie.isChangingRow) return;
                BulletMoveWay moveway=BulletMoveWay.Left;
                if (zombie.isMindControlled)
                {
                    moveway=BulletMoveWay.MoveRight;
                }
                Vector2 pos=zombie.shoot.position;
                Bullet b=CreateBullet.Instance.SetBullet(pos.x,pos.y,zombie.theZombieRow,BulletType.Bullet_doom_ulti,moveway,!zombie.isMindControlled);
                b.Damage=1000;
                b.shootByZombie=!zombie.isMindControlled;
                SoundType iVar4 = (SoundType)Random.RandomRangeInt(3,5);
                GameAPP.PlaySound(iVar4, 0.5f, 1.0f);
            }
            catch(Exception){}
        }
        public void AnimShootGun_Custom()
        {
            try
            {
                if(zombie.beforeDying || zombie.theStatus!=ZombieStatus.Paper_angry || zombie.isChangingRow) return;
                BulletMoveWay moveway=BulletMoveWay.Left;
                if (zombie.isMindControlled)
                {
                    moveway=BulletMoveWay.MoveRight;
                }
                Vector2 pos=zombie.shoot_gun.position;
                Bullet b=CreateBullet.Instance.SetBullet(pos.x,pos.y,zombie.theZombieRow,BulletType.Bullet_doom_ulti,moveway,!zombie.isMindControlled);
                b.Damage=1000;
                b.shootByZombie=!zombie.isMindControlled;
                SoundType iVar4 = (SoundType)Random.RandomRangeInt(3,5);
                GameAPP.PlaySound(iVar4, 0.5f, 1.0f);
            }
            catch(Exception){}
        }
        public void Angry() => zombie.ChangeStatus(ZombieStatus.Paper_angry);
        public void TakeGun()
        {
            zombie.Angry();
            Angry();
        }
    }
}