namespace MoreSuper
{
    public class PeaSuper : MoreSuper
    {
        public override IEnumerator SuperShoot()
        {
            int peaCount;
            bool isRepeater = false;

            switch (plant.thePlantType)
            {
                default:
                    peaCount = 60;
                    break;

                case PlantType.DoubleShooter:
                case PlantType.JalaDoubleshooter:
                case PlantType.DoubleSnow:
                case PlantType.DoubleCherry:
                case PlantType.HypnoRepeater:
                    peaCount = 90;
                    isRepeater = true;   // mark repeater family
                    break;
            }

            BulletType btype;
            switch (plant.thePlantType)
            {
                case PlantType.Peashooter:
                case PlantType.DoubleShooter:
                default:
                    btype = BulletType.Bullet_pea;
                    break;

                case PlantType.JalaPeashooter:
                case PlantType.JalaDoubleshooter:
                    btype = BulletType.Bullet_pea_jala;
                    break;

                case PlantType.SnowPeaShooter:
                case PlantType.DoubleSnow:
                    btype = BulletType.Bullet_snowPea;
                    break;

                case PlantType.Cherryshooter:
                case PlantType.DoubleCherry:
                    btype = BulletType.Bullet_cherry;
                    break;

                case PlantType.HypnoPeashooter:
                case PlantType.HypnoRepeater:
                    btype = BulletType.Bullet_hypnoPea;
                    break;

                case PlantType.DoomPeashooter:
                    btype = BulletType.Bullet_pea_doom;
                    break;
            }

            // ------------------------------
            // NORMAL PF LOOP (unchanged)
            // ------------------------------
            for (int i = 0; i < peaCount; i++)
            {
                Vector2 pos;
                try
                {
                    pos = plant.shoot.position;
                }
                catch
                {
                    pos = plant.transform.position;
                    pos=new Vector2(pos.x,pos.y+1.5f);
                }

                float x = pos.x + Random.Range(-0.3f, 0.3f);
                float y = pos.y + Random.Range(-0.3f, 0.3f);

                Bullet b = CreateBullet.Instance.SetBullet(
                    x, y,
                    plant.thePlantRow,
                    btype,
                    BulletMoveWay.MoveRight,
                    false
                );

                if (b == null)
                    yield break;

                b.Damage = plant.attackDamage;
                b.fromType = plant.thePlantType;
                b.normalSpeed = Random.Range(5f, 7.5f);

                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }

            // ------------------------------
            // FINAL GIANT PEA (Repeater only)
            // ------------------------------
            if (isRepeater)
            {
                // Ensure ALL PF bullets are done
                // 10 FixedUpdates AFTER the loop
                for (int j = 0; j < 25; j++)
                    yield return new WaitForFixedUpdate();

                Vector2 pos;
                try
                {
                    pos = plant.shoot.position;
                }
                catch
                {
                    pos = plant.transform.position;
                }
                float x = pos.x;
                float y = pos.y;

                Bullet b = CreateBullet.Instance.SetBullet(
                    x, y,
                    plant.thePlantRow,
                    btype,
                    BulletMoveWay.MoveRight,
                    false
                );

                if (b == null)
                    yield break;

                b.Damage = plant.attackDamage;
                b.fromType = plant.thePlantType;
                b.normalSpeed = 5f;

                x+=0.15f;

                Bullet b1 = CreateBullet.Instance.SetBullet(
                    x, y,
                    plant.thePlantRow,
                    btype,
                    BulletMoveWay.MoveRight,
                    false
                );

                if (b1 == null)
                    yield break;

                b1.Damage = plant.attackDamage;
                b1.fromType = plant.thePlantType;
                b1.normalSpeed = 5f;

                x-=0.3f;

                Bullet b2 = CreateBullet.Instance.SetBullet(
                    x, y,
                    plant.thePlantRow,
                    btype,
                    BulletMoveWay.MoveRight,
                    false
                );

                if (b2 == null)
                    yield break;

                b2.Damage = plant.attackDamage;
                b2.fromType = plant.thePlantType;
                b2.normalSpeed = 5f;

                x+=0.15f;
                y+=0.15f;

                Bullet b3 = CreateBullet.Instance.SetBullet(
                    x, y,
                    plant.thePlantRow,
                    btype,
                    BulletMoveWay.MoveRight,
                    false
                );

                if (b3 == null)
                    yield break;

                b3.Damage = plant.attackDamage;
                b3.fromType = plant.thePlantType;
                b3.normalSpeed = 5f;

                y-=0.3f;

                Bullet b4 = CreateBullet.Instance.SetBullet(
                    x, y,
                    plant.thePlantRow,
                    btype,
                    BulletMoveWay.MoveRight,
                    false
                );

                if (b4 == null)
                    yield break;

                b4.Damage = plant.attackDamage;
                b4.fromType = plant.thePlantType;
                b4.normalSpeed = 5f;
            }
        }
    }
}
