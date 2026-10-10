using CustomPlantClass.Runtime.Tasks;

namespace CustomPlantClass.Examples
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class SuperHypnoGatling_Example : CustomShooter
    {
        public override Transform FindShoot() => transform.FindChild("GatlingPea_head/GatlingPea_mouth_overlay");
        public override BulletType GetBulletType()
        {
            return BulletType.Bullet_hypnoPea;
        }
        public override int AttackDamage => Lawnf.TravelUltimate(UltiBuff.EnumValue51) ? _plant.attackDamage * 2 : _plant.attackDamage;
        public override Bullet Shoot_Custom()
        {
            if (PlantMgr.GetPercent(2f) || (Lawnf.TravelUltimate(UltiBuff.EnumValue50) && PlantMgr.GetPercent(6f)))
            {
                StartPF();
            }
            return PlantMgr.SetBullet(_plant, GetBulletType(), BulletMoveWay.MoveRight);
        }
        protected override bool IsAsyncPF => true;
        protected override async Task SuperShoot_Async()
        {
            try
            {
                _plant.anim.SetBoolString("shooting", true);
                for (int i = 0; i < 250; i++)
                {
                    for (int j = 0; j < 5; j++)
                        PlantMgr.SetBullet
                        (
                            _plant,
                            GetBulletType(),
                            GetBulletMoveWayPF_SuperGatling(),
                            AttackDamage,
                            new Vector2(0, Random.Range(-0.15f, 0.15f)), Random.Range(-15f, 15f)
                        ).normalSpeed = Random.Range(12f, 14f);
                    _plant.thePlantAttackCountDown = 10f;
                    await DelayTask.DelayScaled(0.02f,()=>_plant.attributeSpeed,token);
                }
                _plant.thePlantAttackCountDown = 0.05f;
                _plant.anim.SetBoolString("shooting", false);
            }
            catch( Exception e)
            {
                ModLogger.LogError(e.ToString());
            }
        }
        public override void SuperEnd()
        {
            if (_plant.starUp) StartPF();
            else base.SuperEnd();
        }
    }
}