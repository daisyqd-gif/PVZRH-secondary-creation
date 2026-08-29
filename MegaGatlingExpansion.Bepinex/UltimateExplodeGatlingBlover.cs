namespace MegaGatlingExpansion
{
    public class UltimateExplodeGatlingBlover : BaseCustomPlant
    {
        public UltimateGatlingBlover plant => gameObject.GetComponent<UltimateGatlingBlover>();
        public override Transform FindShoot() => transform.FindChild("PeaShooter_Head/Shoot");
        public override BulletMoveWay GetBulletMoveWay()
        {
            if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) return BulletMoveWay.MoveRight;
            else return BulletMoveWay.Free;
        }
        public override Bullet Shoot_Custom()
        {
            var pos = plant.shoot.position;

            var bullet1 = CreateBullet.Instance.SetBullet(pos.x - 0.1f, pos.y, plant.thePlantRow, BulletType.Bullet_superCherry, BulletMoveWay.MoveRight);

            bullet1.Damage = plant.attackDamage;
            bullet1.fromType = plant.thePlantType;
            var bullet2 = CreateBullet.Instance.SetBullet(pos.x + 0.1f, pos.y, plant.thePlantRow, BulletType.Bullet_superCherry, BulletMoveWay.MoveRight);

            bullet2.Damage = plant.attackDamage;
            bullet2.fromType = plant.thePlantType;
            var plants = Lawnf.Get1x1Plants(plant.thePlantColumn, plant.thePlantRow);

            // IL2CPP predicate
            var predicate = UltimateGatlingBlover.__c.__9__4_0;

            // Convert IL2CPP Func → System Func
            Func<Plant, bool> sysPredicate = p =>
            {
                if (p == null)
                    return false;

                try
                {
                    // Call the patched predicate exactly like vanilla
                    return UltimateGatlingBlover.__c.__9._AttributeEvent_b__4_0(p);
                }
                catch (Exception e)
                {
                    // Fallback to original behavior
                    Debug.LogError("Using fallback\n" + e.ToString());
                    return p.thePlantType == PlantTypeExpand.ExplodeGatlingBlover || p.thePlantType == PlantType.UltimateGatlingBlover;
                }
            };


            // Convert IL2CPP list → System list
            var sysPlants = plants.ToSystemList();

            // Use System LINQ (compatible with other mods)
            Plant target = sysPlants.FirstOrDefault(sysPredicate);

            if (Lawnf.TravelAdvanced(AdvBuff.EnumValue3006) && target != null)
            {
                for (int i = 0; i < plant.coordination; i++)
                {
                    var b2 = CreateBullet.Instance.SetBullet(
                        pos.x + Random.Range(-0.5f, 0.5f),
                        pos.y + Random.Range(-0.5f, 0.5f),
                        plant.thePlantRow,
                        BulletType.Bullet_superCherry,
                        BulletMoveWay.MoveRight
                    );

                    b2.fromType = plant.thePlantType;

                    if (i > 4)
                        b2.Damage = plant.attackDamage - (int)((plant.coordination - 5f) * plant.attackDamage * 0.3f);
                    else
                        b2.Damage = plant.attackDamage;
                }
                plant.coordination += 0.1f;
            }

            if (Random.Range(0, 100) < 10) plant.StartCoroutine(SuperShoot_Custom());

            GameAPP.PlaySound(Random.Range(3, 5), 0.5f, 1.0f);
            return bullet1;
        }
        public virtual IEnumerator SuperShoot_Custom()
        {
            isPF = true;
            plant.invincible = true;
            plant.uncrashable = true;
            plant.flashCountDown = 5f;
            plant.isFlashing = true;

            int total = 60;

            for (int i = 0; i < total; i++)
            {
                if (plant == null || plant.IsDestroyed()) yield break;
                Vector3 pos = plant.shoot.position;
                for (int j = 0; j < Math.Clamp(plant.coordination,1,4); j++)
                {
                    Bullet b = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow,
                        BulletType.Bullet_superCherry,
                        GetBulletMoveWay()
                    );
                    b.Damage = plant.attackDamage;
                    b.fromType = plant.thePlantType;
                    b.transform.Rotate(0, 0, Random.Range(-15f, 15f));
                    if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) b.Damage *= 2;
                }

                if (Time.timeScale > 0f)
                {
                    yield return new WaitForFixedUpdate();
                }
                else
                    yield return null;
            }

            plant.invincible = false;
            plant.uncrashable = false;
            plant.isFlashing = false;
            isPF = false;
        }
    }
}
