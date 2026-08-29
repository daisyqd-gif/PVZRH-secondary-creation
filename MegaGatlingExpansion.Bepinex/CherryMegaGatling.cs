namespace MegaGatlingExpansion
{
    public class CherryMegaGatlingPea : MegaGatlingPea
    {
        public override BulletType GetBulletType_Custom() => BulletType.Bullet_superCherry;
        public override string GetShootPath()
        {
            return "Shoot";
        }
        public override string GetTextString() => "充能:" + AttributeCount_Custom + "\n过热:" + Mathf.Ceil(Heat) + "/" + MaxHeat() + "\n过热时间:" + Mathf.Ceil(OverHeatTmr) + "/5";
        public override Bullet Shoot_Custom()
        {
            if (OverHeatTmr > 0f)
            {
                return null;
            }
            base.Shoot_Custom();
            return null;
        }
        // -------------------------
        //  CUSTOM SHOOT LOGIC
        // -------------------------
        public override IEnumerator Shooting_Custom()
        {
            for (int i = 0; i < 4 + AttributeCount_Custom; i++)
            {
                if (plant == null || plant.IsDestroyed()) yield break;
                Vector3 pos = plant.shoot.position;
                Bullet b = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, plant.thePlantRow,
                    GetBulletType_Custom(),
                    BulletMoveWay.MoveRight
                );
                b.Damage = GetDmg();
                b.fromType = plant.thePlantType;
                if (Lawnf.TravelAdvanced(AdvBuff.EnumValue3002))
                {
                    Bullet b1 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow,
                        GetBulletType_Custom(),
                        BulletMoveWay.Sin
                    );
                    b1.Damage = GetDmg();
                    b1.fromType = plant.thePlantType;
                    Bullet b2 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow,
                        GetBulletType_Custom(),
                        BulletMoveWay.Sin
                    );
                    b2.Damage = GetDmg();
                    b2.fromType = plant.thePlantType;
                    b2.theExistTime += 0.5f;
                }
                count++;
                TryAddHeat(1);
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
        }
        public override Bullet Shoot2_Custom()
        {
            if (OverHeatTmr > 0f)
            {
                return null;
            }
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
            return b;
        }

        // -------------------------
        //  CUSTOM SPRITE LOGIC
        // -------------------------
        public override void ReplaceSprite_Custom()
        {
            Transform p4 = transform.FindChild("Shoot/GatlingPea_barrel/GatlingPea_barrel_4");
            Transform p5 = transform.FindChild("Shoot/GatlingPea_barrel/GatlingPea_barrel_5");
            Transform p6 = transform.FindChild("Shoot/GatlingPea_barrel/GatlingPea_barrel_6");

            switch (AttributeCount_Custom)
            {
                case 0:
                    p4.gameObject.SetActive(false);
                    p5.gameObject.SetActive(false);
                    p6.gameObject.SetActive(false);
                    break;
                case 1:
                    p4.gameObject.SetActive(true);
                    p5.gameObject.SetActive(false);
                    p6.gameObject.SetActive(false);
                    break;
                case 2:
                    p4.gameObject.SetActive(true);
                    p5.gameObject.SetActive(true);
                    p6.gameObject.SetActive(false);
                    break;
                case 3:
                    p4.gameObject.SetActive(true);
                    p5.gameObject.SetActive(true);
                    p6.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
