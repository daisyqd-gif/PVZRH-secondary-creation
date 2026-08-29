using System.Threading.Tasks;
using CustomPlantClass.Runtime.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace MegaGatlingExpansion
{
    public class MegaGatlingPea : BaseCustomPlant
    {
        public int AttributeCount_Custom = 0;
        public PlantTypeExpand type;
        public Plant plant => GetComponent<Shooter>();
        public bool startPF = true;
        public int count = 0;
        public float Heat = 0f;
        public float OverHeatTmr = 0f;
        public float PFChance = 10f;
        public bool isElectric=false;
        public virtual int MaxHeat() => Lawnf.TravelUltimate(UltiBuff.EnumValue3) ? 100 : 50;

        public virtual int GetDmg() => Mathf.RoundToInt(plant.attackDamage * Mathf.Max(1, Heat * 12.5f));

        // -------------------------
        //  CUSTOM START LOGIC
        // -------------------------
        public override void OnSpawn()
        {
            plant.shoot = transform.FindChild(GetShootPath());
            type = plant.thePlantType;

            if (GameAPP.theGameStatus == GameStatus.InGame && CreateBullet.Instance != null && plant.board != null && plant.anim != null && CreateItem.Instance != null && Board.Instance != null)
            {
                /*int coinCount = (int)type switch
                {
                    13081 => 3,
                    13082 => 5,
                    13083 => 4,
                    13084 => 2,
                    13085 => 9,
                    13088 => 13,
                    13089 => 16,
                    13091 => 6,
                    13092 => 3,
                    _ => 0
                };*/
                for (int i = 0; i < GetSunCount(); i++)
                    CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 0, 0);
                isPF = true;
                StartPF();
            }
        }

        public virtual int GetSunCount() => 0;

        public override BulletMoveWay GetBulletMoveWay()
        {
            if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) return BulletMoveWay.MoveRight;
            else return BulletMoveWay.Free;
        }

        public override string GetShootPath()
        {
            return "GatlingPea_head/Shoot";
        }
        public override string GetTextString()=>"充能:" + AttributeCount_Custom;

        // -------------------------
        //  CUSTOM BULLET TYPE
        // -------------------------
        public virtual BulletType GetBulletType_Custom()
        {
            if(isElectric) return BulletProfile.ElectricPea;
            return AllCustomBullets[Random.Range(0, AllCustomBullets.Length)];
        }

        public virtual BulletType GetBulletType_Custom_PF()
        {
            if(isElectric) return BulletProfile.ElectricPea;
            return GetBulletType_Custom();
        }

        private static readonly BulletType[] AllCustomBullets =
        {
            BulletType.Bullet_extremeSnowPea,
            BulletProfile.ExtremeFirePea,
            BulletProfile.PrimalPea,
            BulletProfile.GooPea,
            BulletProfile.ElectricPea,
            BulletType.Bullet_cherry,
            BulletType.Bullet_pea_threeCherry,
            BulletProfile.HypnoMegaPea,
            BulletType.Bullet_portalPea,
            BulletType.Bullet_pea,
            BulletType.Bullet_pea,
            BulletType.Bullet_pea,
            BulletType.Bullet_pea
        };

        // -------------------------
        //  CUSTOM SHOOT LOGIC
        // -------------------------
        public virtual IEnumerator Shooting_Custom()
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
                if (b.theBulletType == BulletType.Bullet_doom_big || b.theBulletType == BulletType.Bullet_doom_big_ulti) { b.theStatus = BulletStatus.Doom_big; b.Damage = GetDmg() * 6; }
                else b.Damage = GetDmg();
                b.fromType = plant.thePlantType;
                count++;
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
        }
        public override void StartPF()
        {
            AttributeCount_Custom = Mathf.Min(3, AttributeCount_Custom + 1);
            base.StartPF();
        }
        public override void OnFixedUpdate()
        {
            Heat = Mathf.Max(0, Heat - Time.deltaTime / 2);
            OverHeatTmr = Mathf.Max(0, OverHeatTmr - Time.deltaTime / 2);
            if (OverHeatTmr > 0f) plant.thePlantAttackCountDown = 1f;
        }
        public virtual Bullet ShootInDream()
        {
            Vector3 pos = plant.shoot.position;
            Bullet b = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, plant.thePlantRow,
                GetBulletType_Custom(),
                BulletMoveWay.MoveRight
            );
            b.Damage = GetDmg();
            b.fromType = plant.thePlantType;
            return b;
        }
        /*public virtual IEnumerator SuperShoot_Custom(int attr)
        {
            StartPF();
            yield return null;
        }//*/
        public virtual int BulletCountPF => 1;
        protected override bool IsAsyncPF => true;
        protected override async Task SuperShoot_Async()
        {
            plant.anim.SetBool("shooting", true);

            int total = 150 + 10;

            if (plant.skinType != 0)
            {
                total += 30;
            }

            for (int i = 0; i < total/5; i++)
            {
                if (plant == null || plant.IsDestroyed()) return;
                Vector3 pos = plant.shoot.position;
                for (int j = 0; j < BulletCountPF * 5; j++)
                {
                    Bullet b = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow,
                        GetBulletType_Custom_PF(),
                        GetBulletMoveWay()
                    );
                    if (b.theBulletType == BulletType.Bullet_doom_big || b.theBulletType == BulletType.Bullet_doom_big_ulti) { b.theStatus = BulletStatus.Doom_big; b.Damage = GetDmg() * 6; }
                    else b.Damage = GetDmg();
                    b.fromType = plant.thePlantType;
                    b.transform.Rotate(0, 0, Random.Range(-15f, 15f));
                    if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) b.Damage *= 2;
                }
                count++;

                await DelayTask.DelayScaled(0.1f,() => _plant.attributeSpeed,token);
            }

            if (!startPF) AttributeCount_Custom = Mathf.Min(3, AttributeCount_Custom + 1);
            ReplaceSprite_Custom();

            plant.anim.SetBool("shooting", false);
        }
        public override void SuperEnd()
        {
            base.SuperEnd();
            if(plant.starUp || PFChance>=100) Restart();
        }

        public void Restart()
        {
            isPF = true;
            StartPF();
        }

        public override Bullet Shoot_Custom()
        {
            if ((Lawnf.TravelUltimate(UltiBuff.EnumValue50) && Random.Range(0, 100) <= PFChance*3) || Random.Range(0, 100) <= PFChance || plant.starUp)
            {
                isPF = true;
                StartPF();
                return null;
            }

            plant.StartCoroutine(Shooting_Custom());
            return null;
        }

        // -------------------------
        //  CUSTOM SPRITE LOGIC
        // -------------------------
        public virtual void ReplaceSprite_Custom()
        {
            Transform p4 = transform.FindChild("GatlingPea_head/paokou_mid/paokou4");
            Transform p5 = transform.FindChild("GatlingPea_head/paokou_mid/paokou5");
            Transform p6 = transform.FindChild("GatlingPea_head/paokou_mid/paokou6");

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
        public virtual bool TryAddHeat(int amt)
        {
            Heat += amt;
            if (Heat >= MaxHeat())
            {
                OverHeatTmr = 2.5f;
                Heat = 0f;
                plant.board.boardAction.CreateCherryExplode(PlantMgr.GetPos(plant.thePlantRow, plant.thePlantColumn), plant.thePlantRow);
                return true;
            }
            return false;
        }
        public override Bullet Shoot2_Custom()
        {
            return null;
        }
    }
}
