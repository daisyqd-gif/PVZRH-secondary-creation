using System.Threading.Tasks;
using CustomPlantClass.Runtime.Tasks;
using Cysharp.Threading.Tasks;

namespace MegaGatlingExpansion
{
    public class IceMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 3;
        public override BulletType GetBulletType_Custom() => BulletType.Bullet_extremeSnowPea;
        public override Bullet Shoot_Custom()
        {
            if ((Lawnf.TravelUltimate(UltiBuff.EnumValue50) && Random.Range(0, 100) <= PFChance*3) || Random.Range(0, 100) <= PFChance || plant.starUp)
            {
                isPF = true;
                plant.board.boardAction.CreateFreeze(plant.axis.position);
                StartPF();
                return null;
            }

            plant.StartCoroutine(Shooting_Custom());
            return null;
        }
    }
    public class FireMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 5;
        public override BulletType GetBulletType_Custom() => BulletProfile.ExtremeFirePea;
        public override Bullet Shoot_Custom()
        {
            if ((Lawnf.TravelUltimate(UltiBuff.EnumValue50) && Random.Range(0, 100) <= PFChance*3) || Random.Range(0, 100) <= PFChance || plant.starUp)
            {
                isPF = true;
                plant.board.boardAction.CreateFireLine(plant.thePlantRow);
                StartPF();
                return null;
            }

            plant.StartCoroutine(Shooting_Custom());
            return null;
        }
    }
    public class GooMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 1;
        public override BulletType GetBulletType_Custom() => BulletProfile.GooPea;
    }
    public class SunMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 2;
        public override string GetShootPath() => "PeaShooter_Head/Shoot";
        public override BulletType GetBulletType_Custom() => BulletType.Bullet_smallSun;
        public override Bullet Shoot_Custom()
        {
            if ((Lawnf.TravelUltimate(UltiBuff.EnumValue50) && Random.Range(0, 100) <= PFChance*3) || Random.Range(0, 100) <= PFChance || plant.starUp)
            {
                isPF = true;
                StartPF();
                _ = MakeSun();
                return null;
            }

            plant.StartCoroutine(Shooting_Custom());
            return null;
        }
        public virtual async Task MakeSun()
        {
            for (int i = 0; i < 15 + AttributeCount_Custom; i++)
            {
                if (plant == null || plant.IsDestroyed()) return;
                CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 0, 0);
                await DelayTask.Delay(0.1f,token);
            }
        }

        // -------------------------
        //  CUSTOM SPRITE LOGIC
        // -------------------------
        public override void ReplaceSprite_Custom()
        {
            Transform p4 = transform.FindChild("PeaShooter_Head/Shoot/paokou_mid/paokou4");
            Transform p5 = transform.FindChild("PeaShooter_Head/Shoot/paokou_mid/paokou5");
            Transform p6 = transform.FindChild("PeaShooter_Head/Shoot/paokou_mid/paokou6");

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
    public class ElectricMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 5;
        public override string GetShootPath()
        {
            return "GatlingPea_head/paokou_mid";
        }
        public override BulletType GetBulletType_Custom() => BulletProfile.ElectricPea;
        public override string GetTextString() => "充能:" + AttributeCount_Custom + "\n请输入文本";
    }
    public class PrimalMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 5;
        public override BulletType GetBulletType_Custom() => BulletProfile.PrimalPea;
    }
    public class ChronoMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 5;
        public override BulletType GetBulletType_Custom() => BulletType.Bullet_portalPea;
        public override Bullet Shoot_Custom()
        {
            if (Random.Range(0, 100) <= GetChance() || plant.starUp)
            {
                isPF = true;
                StartPF();
                return null;
            }

            plant.StartCoroutine(Shooting_Custom());
            return null;
        }
        public float GetChance()
        {
            if (Lawnf.TravelUltimate(UltiBuff.EnumValue50)) return PFChance*3 + plant.magnetCount * 3;
            else return PFChance + plant.magnetCount;
        }
        public override string GetTextString() => "充能:" + AttributeCount_Custom + "\n大招概率:" + Mathf.Min(100, GetChance()) + "%";
    }
    public class RegularCherryMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 6;
        public override BulletType GetBulletType_Custom() => BulletType.Bullet_pea_threeCherry;
        public override BulletType GetBulletType_Custom_PF() => BulletType.Bullet_pea_bombCherry;
    }
    public class HypnoCherryMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 3;
        public override BulletType GetBulletType_Custom() => BulletProfile.HypnoMegaPea;
    }
    public class SmallMegaGatlingPea : MegaGatlingPea
    {
        public override int GetSunCount() => 0;
        public override BulletType GetBulletType_Custom() => BulletType.Bullet_puffPea;

        public override BulletType GetBulletType_Custom_PF()
        {
            return bulletTypes[Random.Range(0, bulletTypes.Length)];
        }
        public override void Start()
        {
            base.Start();
            plant.isShort = true;
        }
        private static readonly BulletType[] bulletTypes =
        {
            BulletType.Bullet_puffIronPea,
            BulletType.Bullet_puffPea,
            BulletType.Bullet_snowPuffPea,
            BulletType.Bullet_firePea_small
        };
        public override string GetShootPath() => "Shoot";

        // -------------------------
        //  CUSTOM SPRITE LOGIC
        // -------------------------
        public override void ReplaceSprite_Custom()
        {
            Transform p4 = transform.FindChild("Shoot/GatlingPea_barrel_mid/GatlingPea_barrel_5");
            Transform p5 = transform.FindChild("Shoot/GatlingPea_barrel_mid/GatlingPea_barrel_6");
            Transform p6 = transform.FindChild("Shoot/GatlingPea_barrel_mid/GatlingPea_barrel_7");

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
