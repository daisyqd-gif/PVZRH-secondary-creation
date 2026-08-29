using GameLevel.RogueShooting;

namespace UltimateSniper
{
    public class UltimateFlameGatling_Remade : BaseCustomPlant
    {
        public int energy = 0;
        public bool isSkill = false;
        public int MeteorCD = 200;
        public override string GetTextString() => "充能:" + energy; 
        public static bool IsRogue{get => ShootingManager.Instance != null || Board.Instance.boardTag.rogueShooting;}
        public override Transform FindShoot() => transform.FindChild("GatlingPea_head/Shoot");
        public IEnumerator Shooting()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    
                    SetBullet(new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)),Random.Range(-15f, 15f)).normalSpeed = Random.Range(12f, 14f);
                }

                yield return new WaitForFixedUpdate();
            }
        }
        public override void Update()
        {
            base.Update();
        }
        public override void OnFixedUpdate()
        {
            if (energy >= 200 && !isSkill && Lawnf.TravelAdvanced(Plugin.Buff1))
            {
                energy=125;
                SuperStart_Custom();
                MakeMeteor();
            }
            base.OnFixedUpdate();
        }
        public override Bullet Shoot_Custom()
        {
            if(IsRogue)
            {
                SetBullet(default,15);
                SetBullet(default,-15);
            }
            if (_plant.starUp) _plant.StartCoroutine(Shooting());
            return SetBullet();
        }
        public void Clicked()
        {
            if (energy >= 50 && !isSkill && !IsRogue)
            {
                isSkill = true;
                energy -= 50;
                MakeMeteor();
            }
        }
        public Bullet SetBullet(Vector2 offset=default, float rotation=0)
        {
            energy++;
            if(energy>=MeteorCD && IsRogue && !isSkill)
            {
                SuperStart_Custom();
                energy=0;
            }
            PlantMgr.GetPlantIn3x3(_plant.thePlantColumn,_plant.thePlantRow,(Plant p) =>
            {
                if(p.TryGetComponent<UltimateFlameSniper>(out var c))
                {
                    c.hitCount++;
                    return true;
                }
                return false;
            });
            return PlantMgr.SetBullet(
                        _plant,
                        Plugin.BType_Flame,
                        BulletMoveWay.SuperGatling,
                        offset,
                        rotation
                    );
        }
        public void MakeMeteor()
        {
            isSkill = true;
            _plant.anim.SetTriggerString("super");
        }
        public void SuperStart_Custom()
        {
            CustomBigStar.SetStar(Plugin.FireStar).GetComponent<FireStar>().fromType = _plant.thePlantType;
            isSkill = false;
        }
    }
}