using CustomPlantClass.Runtime;

namespace UltimateSniper
{
    public class UltimateMegaGatlingPea : BaseCustomPlant
    {
        public static ID PLANT_ID = 13036;//ID is a customizelib type used in all modern plant mods
        public SuperSnowGatling plant => gameObject.GetComponent<SuperSnowGatling>();
        [ResetOnBoardDestroy(0)]
        public static int HitCount = 0;
        [ResetOnBoardDestroy(1)]
        public static int Multiplier = 1;
        //private Animator animator;
        //public TextMeshPro extraText;
        //public bool isPF=false;
        public bool isSummoning = false;
        public override Transform FindShoot() => plant.gameObject.transform.FindChild("GatlingPea_head/Shoot");
        public override BulletType GetBulletType()
        {
            if (plant.starUp) HitCount++;
            int random = Random.RandomRange(0, 3);
            switch (random)
            {
                case 0:
                case 1:
                    return UltimateFirePea.BULLET_ID;
                default:
                    return UltimateCherryPea.BULLET_ID;
            }
        }
        public override Bullet Shoot_Custom()
        {
            Vector3 pos = plant.shoot.position;
            Bullet b = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, plant.thePlantRow,
                GetBulletType(),
                GetBulletMoveWay()
            );
            b.Damage = plant.attackDamage;
            b.fromType = plant.thePlantType;
            if (plant.starUp) HitCount++;
            if (PlantMgr.GetPercent(2f) || (plant.starUp && PlantMgr.GetPercent(15f)) || (Lawnf.TravelUltimate(UltiBuff.EnumValue50) && PlantMgr.GetPercent(6f)) || (plant.starUp && Lawnf.TravelUltimate(UltiBuff.EnumValue50) && PlantMgr.GetPercent(45f)))
            {
                plant.StartCoroutine(SuperShoot_Custom());
            }
            return null;
        }
        public override BulletMoveWay GetBulletMoveWay()
        {
            if (Lawnf.TravelUltimate(UltiBuff.EnumValue51) || !isPF) return BulletMoveWay.MoveRight;
            return BulletMoveWay.SuperGatling;
        }
        public virtual IEnumerator SuperShoot_Custom()
        {
            if(isPF) yield break;
            isPF = true;
            plant.invincible = true;
            plant.Recover(plant.thePlantMaxHealth);
            plant.uncrashable = true;
            plant.flashCountDown = 5f;
            plant.isFlashing = true;
            plant.anim.SetBool("shooting", true);

            int total = 180;

            for (int i = 0; i < total; i++)   
            {
                if (plant == null || plant.IsDestroyed()) yield break;
                Vector3 pos = plant.shoot.position;
                Bullet b = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, plant.thePlantRow,
                    GetBulletType(),
                    GetBulletMoveWay()
                );
                b.Damage = plant.attackDamage;
                if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) b.Damage *= 3;
                b.fromType = plant.thePlantType;
                b.transform.Rotate(0, 0, Random.Range(-15f, 15f));
                if (plant.starUp) HitCount++;

                if (Time.timeScale > 0f)
                {
                    yield return new WaitForFixedUpdate();
                }
                else
                    yield return null;
            }

            plant.anim.SetBool("shooting", false);
            plant.invincible = false;
            plant.uncrashable = false;
            plant.isFlashing = false;
            isPF = false;
        }
        public override string GetTextString() => "充能:" + HitCount;
        public override void OnUpdate()
        {
            if (HitCount >= 200)
            {
                MakeMeteor();
            }
        }
        public void SuperStart_Custom()
        {
            Multiplier *= 2;
            SetMeteor();
            isSummoning = false;
        }
        public static bool SetMeteor()
        {
            if (FireMeteor.exists) return true;
            if (Lawnf.GetPlantCount(PLANT_ID, InstanceManager.Board) >= 1)
            {
                FireMeteor f = FireMeteor.SetStar().GetComponent<FireMeteor>();
                f._damage = HitCount / 2;
                HitCount = 0;
                return false;
            }
            return true;
        }
        public void Explode()
        {
            if (plant.starUp) HitCount++;
        }
        public static bool MakeMeteor()
        {
            if (FireMeteor.exists) return true;
            if (Lawnf.GetPlantCount(PLANT_ID, InstanceManager.Board) <= 0) return true;
            foreach (Plant p in Lawnf.GetAllPlants())
            {
                if (p.TryGetComponent<UltimateMegaGatlingPea>(out var b))
                {
                    if (b.isSummoning) continue;
                    b.isSummoning = true;
                    p.anim.SetTriggerString("summon");
                    continue;
                }
            }
            return false;
        }
        public void AttributeEvent_Custom()
        {
            Vector3 pos = plant.shoot.position;
            Bullet b1 = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, plant.thePlantRow,
                GetBulletType(),
                BulletMoveWay.Free
            );
            b1.Damage = plant.attackDamage;
            b1.fromType = plant.thePlantType;
            b1.transform.Rotate(0, 0, 15);
            Bullet b2 = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, plant.thePlantRow,
                GetBulletType(),
                BulletMoveWay.Free
            );
            b2.Damage = plant.attackDamage;
            b2.fromType = plant.thePlantType;
            b2.transform.Rotate(0, 0, -15);
            HitCount++;
            if (Random.Range(0, 100) <= 2 || (plant.starUp && Random.Range(0, 100) <= 25))
            {
                isPF = true;
                plant.StartCoroutine(SuperShoot_Custom());
            }
        }
    }
}