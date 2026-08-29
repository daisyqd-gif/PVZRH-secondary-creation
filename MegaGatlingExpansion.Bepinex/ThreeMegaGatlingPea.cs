using System.Threading.Tasks;
using CustomPlantClass.Runtime.Tasks;
using Cysharp.Threading.Tasks;

namespace MegaGatlingExpansion
{
    public class ThreeMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 11;
        public override BulletType GetBulletType_Custom() => BulletType.Bullet_pea;
        public override string GetShootPath()
        {
            return "headPos2/ThreePeater_head2/ThreePeater_mouth/Shoot";
        }
        protected override async Task SuperShoot_Async()
        {
            isPF = true;
            plant.invincible = true;
            plant.uncrashable = true;
            plant.anim.SetBool("shooting", true);
            plant.flashCountDown = 5f;
            plant.isFlashing = true;

            int total = 150 + 10 * BulletCountPF;

            if (plant.skinType != 0)
            {
                total += 30;
            }

            for (int i = 0; i < total; i++)
            {
                if (plant == null || plant.IsDestroyed()) return;
                Vector3 pos = plant.shoot.position;
                Bullet b = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, plant.thePlantRow,
                    GetBulletType_Custom(),
                    GetBulletMoveWay()
                );

                b.Damage = plant.attackDamage;
                b.fromType = plant.thePlantType;
                b.transform.Rotate(0, 0, Random.Range(-15f, 15f));
                if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) b.Damage *= 2;
                
                if(plant.thePlantRow < 0)
                {
                    Bullet b1 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow,
                        GetBulletType_Custom(),
                        Lawnf.TravelUltimate(UltiBuff.EnumValue51) ? BulletMoveWay.Free : BulletMoveWay.MoveRight
                    );

                    b1.Damage = plant.attackDamage;
                    b1.fromType = plant.thePlantType;
                    b1.transform.Rotate(0, 0, Random.Range(-15f, 15f));
                    if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) b1.Damage *= 2;
                }
                else
                {
                    Bullet b1 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow-1,
                        GetBulletType_Custom(),
                        Lawnf.TravelUltimate(UltiBuff.EnumValue51) ? BulletMoveWay.Free : BulletMoveWay.MoveRight_threePeater
                    );

                    b1.Damage = plant.attackDamage;
                    b1.fromType = plant.thePlantType;
                    b1.transform.Rotate(0, 0, Random.Range(-15f, 15f));
                    if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) b1.Damage *= 2;
                }
                
                if(plant.thePlantRow > plant.board.rowNum - 1)
                {
                    Bullet b1 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow,
                        GetBulletType_Custom(),
                        Lawnf.TravelUltimate(UltiBuff.EnumValue51) ? BulletMoveWay.Free : BulletMoveWay.MoveRight
                    );

                    b1.Damage = plant.attackDamage;
                    b1.fromType = plant.thePlantType;
                    b1.transform.Rotate(0, 0, Random.Range(-15f, 15f));
                    if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) b1.Damage *= 2;
                }
                else
                {
                    Bullet b1 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow+1,
                        GetBulletType_Custom(),
                        Lawnf.TravelUltimate(UltiBuff.EnumValue51) ? BulletMoveWay.Free : BulletMoveWay.MoveRight_threePeater
                    );

                    b1.Damage = plant.attackDamage;
                    b1.fromType = plant.thePlantType;
                    b1.transform.Rotate(0, 0, Random.Range(-15f, 15f));
                    if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) b1.Damage *= 2;
                }

                await DelayTask.DelayScaled(0.1f,() => _plant.attributeSpeed,token);
            }

            if (!startPF) AttributeCount_Custom = Mathf.Min(3, AttributeCount_Custom + 1);
            ReplaceSprite_Custom();

            plant.anim.SetBool("shooting", false);
            plant.invincible = false;
            plant.uncrashable = false;
            plant.isFlashing = false;
            isPF = false;
            startPF = false;
        }
        public override void ReplaceSprite_Custom() { }
        public override IEnumerator Shooting_Custom()
        {
            if (plant.board == null) yield break;
            // First loop: normal forward shots
            for (int i = 0; i < 4 + AttributeCount_Custom; i++)
            {
                if (plant == null || plant.IsDestroyed()) yield break;
                Vector3 pos = plant.shoot.position;
                Bullet b = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y,
                    plant.thePlantRow,
                    GetBulletType_Custom(),
                    BulletMoveWay.MoveRight
                );
                b.Damage = plant.attackDamage;

                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }

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

                // Fire lower lane if not bottom row
                if (row < plant.board.rowNum - 1)
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

                // Fire upper lane if not top row
                if (row > 0)
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
    }
}
