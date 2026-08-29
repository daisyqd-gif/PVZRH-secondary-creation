namespace MegaGatlingExpansion
{
    public class RedGatlingPea : PlantSkinComponent
    {
        public BulletType GetBulletType_Custom() => BulletType.Bullet_pea;
        public void Awake()
        {
            plant.shoot = transform.FindChild("GatlingPea_head/Shoot");
        }
        public int GetDmg() => plant.attackDamage;
        public void AnimShoot()
        {
            if (Random.Range(0, 100) < 75) return;
            Vector3 pos = plant.shoot.position;
            Bullet b = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, plant.thePlantRow,
                GetBulletType_Custom(),
                BulletMoveWay.Free
            );
            b.Damage = GetDmg();
            b.fromType = plant.thePlantType;
            b.transform.Rotate(0, 0, 45);
            Bullet b1 = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, plant.thePlantRow,
                GetBulletType_Custom(),
                BulletMoveWay.Free
            );
            b1.Damage = GetDmg();
            b1.fromType = plant.thePlantType;
            b1.transform.Rotate(0, 0, 30);
            Bullet b2 = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, plant.thePlantRow,
                GetBulletType_Custom(),
                BulletMoveWay.Free
            );
            b2.Damage = GetDmg();
            b2.fromType = plant.thePlantType;
            b2.transform.Rotate(0, 0, -30);
            Bullet b3 = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, plant.thePlantRow,
                GetBulletType_Custom(),
                BulletMoveWay.Free
            );
            b3.Damage = GetDmg();
            b3.fromType = plant.thePlantType;
            b3.transform.Rotate(0, 0, -45);
        }
        public void FixedUpdate()
        {
            if (plant == null) return;
            plant.attackDamage = 80;
        }
    }
}
