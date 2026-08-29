namespace MoreSuper
{
    public class ThreePeaSuper : MoreSuper
    {

        public override IEnumerator SuperShoot()
        {
            int peaCount;
            bool iskelp = false;
            switch (plant.thePlantType)
            {
                default:
                    peaCount = 30;
                    break;

                case PlantType.AllPeater:
                case PlantType.UltimateKelp:
                    peaCount = 60;
                    break;
            }

            BulletType btype;
            switch (plant.thePlantType)
            {
                default:
                    btype = BulletType.Bullet_pea;
                    break;

                case PlantType.SuperThreePeater:
                    btype = BulletType.Bullet_firePea_super;
                    break;

                case PlantType.GarlicThreePeater:
                    btype = BulletType.Bullet_pea_garlic;
                    break;

                case PlantType.CherryThreePeater:
                    btype = BulletType.Bullet_pea_threeCherry;
                    break;

                case PlantType.ThreeSquash:
                    btype = BulletType.Bullet_squash;
                    break;

                case PlantType.SuperKelp:
                    iskelp = true;
                    btype = BulletType.Bullet_squashKelp;
                    break;
                
                case PlantType.UltimateKelp:
                    iskelp = true;
                    btype = BulletType.Bullet_goldSquashKelp;
                    break;
            }

            // Each phase is 15 peas
            int phaseSize = 15;

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

                // Determine which phase we are in
                int phase = i / phaseSize;
                int idx = i % phaseSize;

                float mainAngle;

                switch (phase)
                {
                    case 0: // 0 → 90
                        mainAngle = 90f / (phaseSize - 1) * idx;
                        break;

                    case 1: // 90 → 0
                        mainAngle = 90f - 90f / (phaseSize - 1) * idx;
                        break;

                    case 2: // repeat 0 → 90 (AllPeater only)
                        mainAngle = 90f / (phaseSize - 1) * idx;
                        break;

                    case 3: // repeat 90 → 0 (AllPeater only)
                        mainAngle = 90f - 90f / (phaseSize - 1) * idx;
                        break;

                    default:
                        mainAngle = 0f;
                        break;
                }

                // Three angles: up, straight, down
                float[] angles = { mainAngle, 0f, -mainAngle };

                foreach (float angle in angles)
                {
                    float y=pos.y;
                    if(iskelp) y+=2;
                    BulletMoveWay move =
                        angle == 0f ? BulletMoveWay.MoveRight : BulletMoveWay.Free;

                    Bullet b = CreateBullet.Instance.SetBullet(
                        pos.x, y,
                        plant.thePlantRow,
                        btype,
                        move,
                        false
                    );

                    if (b == null)
                        continue;

                    b.Damage = plant.attackDamage;
                    b.fromType = plant.thePlantType;
                    b.normalSpeed = 5f;

                    // Free bullets use rotation as movement direction
                    b.transform.Rotate(0f, 0f, angle);
                }

                // Your triple WaitForFixedUpdate preserved
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
