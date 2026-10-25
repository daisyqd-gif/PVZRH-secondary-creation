using System.Threading.Tasks;
using CustomPlantClass.Runtime.Tasks;

namespace MegaGatlingExpansion
{
    public class ThreeMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 11;
        private static readonly BulletType[] ThreeCustomBullets =
        {
            BulletType.Bullet_extremeSnowPea,
            BulletType.Bullet_ironPea,
            BulletType.Bullet_snowPea,
            BulletType.Bullet_firePea_orange,
            BulletType.Bullet_firePea_yellow,
            BulletType.Bullet_cherry,
            BulletType.Bullet_pea,
            BulletType.Bullet_pea
        };
        public override BulletType GetBulletType_Custom()
            => ThreeCustomBullets.GetRandomItem();
        public override string GetShootPath()
        {
            return "headPos2/ThreePeater_head2/ThreePeater_mouth/Shoot";
        }
        protected override async Task SuperShoot_Async()
        {
            try
            {
                plant.anim.SetBool("shooting", true);
                if (UltiBuff.EnumValue51.IsActive())
                {
                    plant.flashCountDown = 5f;
                    plant.isFlashing = true;

                    int total = 210;

                    for (int i = 0; i < total; i++)
                    {
                        if (plant == null || plant.IsDestroyed()) return;
                        Vector3 pos = plant.shoot.position;
                        Bullet b = CreateBullet.Instance.SetBullet(
                            pos.x + Random.Range(-0.25f,0.25f), pos.y + Random.Range(-0.25f,0.25f), plant.thePlantRow,
                            GetBulletType_Custom(),
                            GetBulletMoveWay()
                        );

                        b.Damage = plant.attackDamage*2;
                        b.fromType = plant.thePlantType;
                        
                        if(plant.thePlantRow < 0)
                        {
                            Bullet b1 = CreateBullet.Instance.SetBullet(
                                pos.x + Random.Range(-0.25f,0.25f), pos.y + Random.Range(-0.25f,0.25f), plant.thePlantRow,
                                GetBulletType_Custom(),
                                BulletMoveWay.MoveRight
                            );

                            b1.Damage = plant.attackDamage*2;
                            b1.fromType = plant.thePlantType;
                        }
                        else
                        {
                            Bullet b1 = CreateBullet.Instance.SetBullet(
                                pos.x + Random.Range(-0.25f,0.25f), pos.y + Random.Range(-0.25f,0.25f), plant.thePlantRow-1,
                                GetBulletType_Custom(),
                                BulletMoveWay.MoveRight_threePeater
                            );

                            b1.Damage = plant.attackDamage*2;
                            b1.fromType = plant.thePlantType;
                        }
                        
                        if(plant.thePlantRow > plant.board.rowNum - 1)
                        {
                            Bullet b1 = CreateBullet.Instance.SetBullet(
                                pos.x + Random.Range(-0.25f,0.25f), pos.y + Random.Range(-0.25f,0.25f), plant.thePlantRow,
                                GetBulletType_Custom(),
                                BulletMoveWay.MoveRight
                            );

                            b1.Damage = plant.attackDamage*2;
                            b1.fromType = plant.thePlantType;
                        }
                        else
                        {
                            Bullet b1 = CreateBullet.Instance.SetBullet(
                                pos.x + Random.Range(-0.25f,0.25f), pos.y + Random.Range(-0.25f,0.25f), plant.thePlantRow+1,
                                GetBulletType_Custom(),
                                BulletMoveWay.MoveRight_threePeater
                            );

                            b1.Damage = plant.attackDamage*2;
                            b1.fromType = plant.thePlantType;
                        }

                        await DelayTask.DelayScaled(0.1f,() => _plant.attributeSpeed,token);
                    }
                }
                else
                {
                    int total = 42;
                    const int phaseSize = 14;

                    for (int i = 0; i < total; i++)
                    {
                        if (plant == null || plant.IsDestroyed()) return;
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

                            case 4: // repeat 0 → 90 (AllPeater only)
                                mainAngle = 90f / (phaseSize - 1) * idx;
                                break;

                            case 5: // repeat 90 → 0 (AllPeater only)
                                mainAngle = 90f - 90f / (phaseSize - 1) * idx;
                                break;

                            default:
                                mainAngle = 0f;
                                break;
                        }
                        Vector3 pos = plant.shoot.position;
                        float[] angles = [ 90-mainAngle, mainAngle, 0f, -mainAngle, 270+mainAngle ];
                        
                        angles.ActionPerItem( (angle =>
                        {
                            Bullet b = CreateBullet.Instance.SetBullet(
                                pos.x, pos.y, plant.thePlantRow,
                                GetBulletType_Custom(),
                                GetBulletMoveWay()
                            );

                            b.Damage = plant.attackDamage;
                            b.fromType = plant.thePlantType;
                            b.transform.Rotate(0, 0, angle);
                        }));

                        await DelayTask.DelayScaled(0.1f,() => _plant.attributeSpeed,token);
                    }
                }
                ReplaceSprite_Custom();

                plant.anim.SetBool("shooting", false);
            }
            catch ( Exception e)
            {
                ModLogger.LogError(e.Message);
            }
        }
        public override void ReplaceSprite_Custom() { }
        public override IEnumerator Shooting_Custom()
        {
            if (plant.board == null) yield break;
            // First loop: normal forward shots
            for (int i = 0; i < 4 + AttributeCount_Custom; i++)
            {
                if (plant == null || plant.IsDestroyed()) yield break;
                if(_mode == FireMode.Accurate)
                {
                    Vector3 pos = plant.shoot.position;
                    Bullet b = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y,
                        plant.thePlantRow,
                        GetBulletType_Custom(),
                        BulletMoveWay.MoveRight
                    );
                    b.Damage = plant.attackDamage;
                    Bullet b1 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y+0.15f,
                        plant.thePlantRow,
                        GetBulletType_Custom(),
                        BulletMoveWay.MoveRight
                    );
                    b1.Damage = plant.attackDamage;
                    Bullet b2 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y-0.15f,
                        plant.thePlantRow,
                        GetBulletType_Custom(),
                        BulletMoveWay.MoveRight
                    );
                    b2.Damage = plant.attackDamage;
                }
                else if(_mode == FireMode.Three)
                {
                    GameObject zg = plant.SearchZombie();
                    if(zg.TryGetComponent<Zombie>(out var z))
                    {
                        Vector3 pos = plant.shoot.position;
                        Bullet b = CreateBullet.Instance.SetBullet(
                            pos.x, pos.y,
                            z.theZombieRow,
                            GetBulletType_Custom(),
                            BulletMoveWay.MoveRight_threePeater
                        );
                        b.Damage = plant.attackDamage;
                    }
                }
                else
                {
                    for(int j = 0; j<plant.board.rowNum; j++)
                    {
                        Vector3 pos = plant.shoot.position;
                        Bullet b = CreateBullet.Instance.SetBullet(
                            pos.x, pos.y, j,
                            GetBulletType_Custom(),
                            BulletMoveWay.MoveRight_threePeater
                        );
                        b.Damage = plant.board.rowNum == 5 ? (int)(plant.attackDamage*1.2f) : plant.attackDamage;
                    }
                }

                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }

            if(_mode != FireMode.Spread)
            {
                // Second loop: Threepeater-style 3-lane firing
                for (int i = 0; i < 4 + AttributeCount_Custom; i++)
                {
                    Vector3 pos = plant.shoot.position;
                    int row = plant.thePlantRow;

                    // Always fire middle lane
                    Bullet mid = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y,
                        row,
                        GetBulletType_Custom(),
                        BulletMoveWay.MoveRight
                    );
                    mid.Damage = plant.attackDamage;
                    mid.fromType = plant.thePlantType;

                    if(_mode == FireMode.Accurate)
                    {
                        Bullet low = CreateBullet.Instance.SetBullet(
                            pos.x, pos.y+0.15f,
                            row,
                            GetBulletType_Custom(),
                            BulletMoveWay.MoveRight
                        );
                        low.Damage = plant.attackDamage;
                        low.fromType = plant.thePlantType;
                    }
                    // Fire lower lane if not bottom row
                    else if (row < plant.board.rowNum - 1)
                    {
                        Bullet low = CreateBullet.Instance.SetBullet(
                            pos.x, pos.y,
                            row + 1,
                            GetBulletType_Custom(),
                            BulletMoveWay.MoveRight_threePeater
                        );
                        low.Damage = plant.attackDamage;
                        low.fromType = plant.thePlantType;
                    }
                    else ExtraBullet();

                    if(_mode == FireMode.Accurate)
                    {
                        Bullet low = CreateBullet.Instance.SetBullet(
                            pos.x, pos.y-0.15f,
                            row,
                            GetBulletType_Custom(),
                            BulletMoveWay.MoveRight
                        );
                        low.Damage = plant.attackDamage;
                        low.fromType = plant.thePlantType;
                    }
                    // Fire upper lane if not top row
                    else if (row > 0)
                    {
                        Bullet up = CreateBullet.Instance.SetBullet(
                            pos.x, pos.y,
                            row - 1,
                            GetBulletType_Custom(),
                            BulletMoveWay.MoveRight_threePeater
                        );
                        up.Damage = plant.attackDamage;
                        up.fromType = plant.thePlantType;
                    }
                    else ExtraBullet();

                    yield return new WaitForFixedUpdate();
                    yield return new WaitForFixedUpdate();
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        public void ExtraBullet()
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
            var bulletType = GetBulletType_Custom();

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
        }
        public override string GetTextString()=>$"充能: {AttributeCount_Custom}\n模式:{
            _mode switch
            {
                FireMode.Spread => "散射",
                FireMode.Accurate => "单行",
                FireMode.Three => "普通",
                _ => "普通"
            }
        }";
        internal FireMode _mode = FireMode.Three;
        public override void OnClicked(Mouse mouse)
        {
            switch (_mode)
            {
                case FireMode.Spread: _mode = FireMode.Accurate; break;
                case FireMode.Accurate: _mode = FireMode.Three; break;
                case FireMode.Three: _mode = FireMode.Spread; break;
            }
        }
        internal enum FireMode
        {
            Spread,
            Accurate,
            Three
        }
    }
}
