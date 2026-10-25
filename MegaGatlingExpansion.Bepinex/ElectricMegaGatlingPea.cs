namespace MegaGatlingExpansion
{
    public class ElectricMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 5;
        public override string GetShootPath()
        {
            return "GatlingPea_head/paokou_mid";
        }

        public override Bullet Shoot_Custom()
        {
            if (isPF)
            {
                Bullet b = null;
                for (int i = 0; i < 3; i++)
                {
                    if (plant == null || plant.IsDestroyed()) return null;
                    Vector3 pos = plant.shoot.position;
                    b = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow,
                        GetBulletType_Custom(),
                        BulletMoveWay.Free
                    );
                    b.Damage = GetDmg();
                    b.fromType = plant.thePlantType;
                    b.normalSpeed *= 2;
                    b.transform.Rotate(0,0,UltiBuff.EnumValue51.IsActive() ? 0 : Random.Range(-15,15) );
                    plant.shootingCurse ++;
                    b.GetComponent<ElectricPea>().isPF = true;
                }
                SuperEnd();
                return b;
            }
            if (((Lawnf.TravelUltimate(UltiBuff.EnumValue50) && Random.Range(0, 100) <= PFChance*3) || Random.Range(0, 100) <= PFChance || plant.starUp) && !((AdvBuff)Plugin.CurseBuff).IsActive())
            {
                StartPF();
            }

            plant.StartCoroutine(Shooting_Custom());
            return null;
        }
        public override IEnumerator Shooting_Custom()
        {
            for (int i = 0; i < 1 + (AttributeCount_Custom+_plant.shootingLevel)/2; i++)
            {
                if (plant == null || plant.IsDestroyed()) yield break;
                Vector3 pos = plant.shoot.position;
                Bullet b = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, plant.thePlantRow,
                    GetBulletType_Custom(),
                    BulletMoveWay.MoveRight
                );
                if (b.theBulletType == BulletType.Bullet_doom_big || b.theBulletType == BulletType.Bullet_doom_big_ulti) { b.theStatus = BulletStatus.Doom_big; b.Damage = GetDmg() * 6; }
                else b.Damage = GetDmg();
                b.fromType = plant.thePlantType;
                b.normalSpeed /= 4;
                b.GetComponent<ElectricPea>().isPF = false;
                plant.shootingCurse ++;
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
        }
        public override void StartPF()
        {
            _plant.invincible = true;
            _prevUncrashable = _plant.uncrashable;
            _plant.uncrashable = true;
            isPF = true;
            _plant.isFlashing = true;
            AttributeCount_Custom = Mathf.Min(3, AttributeCount_Custom + 1);
            plant.anim.SetTriggerString("shoot");
        }
        public override BulletType GetBulletType_Custom() => BulletProfile.ElectricPea;
    }
}