namespace MegaGatlingExpansion
{
    public class WildMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 400;
        public override BulletType GetBulletType_Custom() => BulletType.Bullet_pea;
        public override int BulletCountPF => 4;
        public override IEnumerator Shooting_Custom()
        {
            for (int i = 0; i < 4 + (AttributeCount_Custom * 2); i++)
            {
                if (plant == null || plant.IsDestroyed()) yield break;
                Vector3 pos = plant.shoot.position;
                Bullet b = CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, GetBulletType_Custom(), BulletMoveWay.MoveRight);
                b.Damage = plant.attackDamage;
                b.fromType = plant.thePlantType;
                Bullet b1 = CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, GetBulletType_Custom(), BulletMoveWay.Free);
                b1.Damage = plant.attackDamage;
                b1.fromType = plant.thePlantType;
                b1.transform.Rotate(0, 0, 15);
                Bullet b2 = CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, GetBulletType_Custom(), BulletMoveWay.Free);
                b2.Damage = plant.attackDamage;
                b2.fromType = plant.thePlantType;
                b2.transform.Rotate(0, 0, 7.5f);
                Bullet b3 = CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, GetBulletType_Custom(), BulletMoveWay.Free);
                b3.Damage = plant.attackDamage;
                b3.fromType = plant.thePlantType;
                b3.transform.Rotate(0, 0, -7.5f);
                Bullet b4 = CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, GetBulletType_Custom(), BulletMoveWay.Free);
                b4.Damage = plant.attackDamage;
                b4.fromType = plant.thePlantType;
                b4.transform.Rotate(0, 0, -15);
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
        }
        public override void ReplaceSprite_Custom()
        {
            Transform p6 = transform.FindChild("GatlingPea_head/paokou_mid/paokou6");
            Transform p7 = transform.FindChild("GatlingPea_head/paokou_mid/paokou7");
            Transform p8 = transform.FindChild("GatlingPea_head/paokou_mid/paokou8");
            switch (AttributeCount_Custom)
            {
                case 0:
                    p6.gameObject.SetActive(false);
                    p7.gameObject.SetActive(false);
                    p8.gameObject.SetActive(false);
                    break;
                case 1:
                    p6.gameObject.SetActive(true);
                    p7.gameObject.SetActive(false);
                    p8.gameObject.SetActive(false);
                    break;
                case 2:
                    p6.gameObject.SetActive(true);
                    p7.gameObject.SetActive(true);
                    p8.gameObject.SetActive(false);
                    break;
                case 3:
                    p6.gameObject.SetActive(true);
                    p7.gameObject.SetActive(true);
                    p8.gameObject.SetActive(true);
                    break;
                default:
                    AttributeCount_Custom = 3;
                    break;
            }
        }
    }
}
