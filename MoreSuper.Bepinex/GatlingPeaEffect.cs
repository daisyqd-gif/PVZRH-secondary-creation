namespace MoreSuper
{
    public class GatlingPeaSuper : MoreSuper
    {
        public override IEnumerator SuperShoot()
        {
            int peaCount=90;

            BulletType btype;
            switch (plant.thePlantType)
            {
                case PlantType.GatlingPea:
                default:
                    btype = BulletType.Bullet_pea;
                    break;

                case PlantType.JalaGatling:
                    btype = BulletType.Bullet_pea_jala;
                    break;

                case PlantType.SnowGatling:
                    btype = BulletType.Bullet_snowPea;
                    break;

                case PlantType.CherryGatling:
                    btype = BulletType.Bullet_cherry;
                    break;

                case PlantType.HypnoGatling:
                    btype = BulletType.Bullet_hypnoPea;
                    break;

                case PlantType.DoomGatling:
                    btype = BulletType.Bullet_doom;
                    break;

                case PlantType.UltimateDoomGatling:
                    btype = BulletType.Bullet_doom_ulti;
                    break;

                case PlantType.UltimateGatling:
                    btype = BulletType.Bullet_superCherry;
                    break;

                case PlantType.SnowGatlingPuff:
                    btype = BulletType.Bullet_snowPuff;
                    break;
                
                case PlantType.SunGatlingPuff:
                    btype = BulletType.Bullet_sunSpike;
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

                // Fire lower lane if not bottom row
                if (plant.thePlantRow < plant.board.rowNum - 1)
                {
                    Bullet low = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y,
                        plant.thePlantRow + 1,
                        btype,
                        BulletMoveWay.Three_down
                    );
                    low.Damage = plant.attackDamage;
                }
                else ExtraBullet(btype);

                // Fire upper lane if not top row
                if (plant.thePlantRow > 0)
                {
                    Bullet up = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y,
                        plant.thePlantRow - 1,
                        btype,
                        BulletMoveWay.Three_up
                    );
                    up.Damage = plant.attackDamage;
                }
                else ExtraBullet(btype);
                if (b == null)
                    yield break;

                b.Damage = plant.attackDamage;
                b.fromType = plant.thePlantType;
                b.normalSpeed = Random.Range(5f, 7.5f);

                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
        }
        public void ExtraBullet(BulletType btype)
        {
            // Ensure we have a shoot transform
            var shootTf = plant.shoot;
            if (shootTf == null)
                return;

            var tf = shootTf.transform;
            if (tf == null)
                return;

            // Get position and row
            Vector3 pos = tf.position;
            int row = plant.thePlantRow;

            // Get CreateBullet instance and bullet type
            var inst = CreateBullet.Instance;
            var bulletType = btype;

            if (inst == null)
                return;

            // Spawn one extra bullet slightly in front
            Bullet b = inst.SetBullet(
                pos.x + 0.1f,
                pos.y,
                row,
                bulletType,
                BulletMoveWay.MoveRight,
                false
            );

            if (b == null)
                return;

            b.Damage = plant.attackDamage;
            b.fromType = plant.thePlantType;
            b.normalSpeed = Random.Range(5f, 7.5f);

            // Unlock achievement
            AchievementManager.UnlockAchievement(Achievement.ThreePeater);
        }
    }
}
