namespace UltimateDoomSniper
{
    public class UltimateDoomSniper : BaseCustomPlant
    {
        public DoomSniper plant => GetComponent<DoomSniper>();
        public bool isFlipped = false;
        public override Transform FindShoot() => transform.FindChild("PeaShooter_Head/gun_lower/Shoot");
        public override string GetTextString() =>
        $"大招充能 : {plant.attributeCount}/{(Lawnf.TravelAdvanced(Plugin.Buff2) ? 150 : 300)}\n" +
        $"狂热能量 : {plant.craze}/{(Lawnf.TravelAdvanced(Plugin.Buff2) ? 50 : 100)}\n" +
        $"狂热时间 : {Mathf.CeilToInt(plant.crazeTimer)}/{(Lawnf.TravelAdvanced(Plugin.Buff2) ? 16 : 8)}";
        public override Bullet Shoot_Custom()
        {
            float pitch = Random.Range(0.9f, 1.1f);
            GameAPP.PlaySound(0x8C, 0.5f, pitch);

            Vector3 shootPos = plant.shoot.position;
            Func<Zombie, bool> condition = z => plant.CheckZombie(z);
            Zombie target = Lawnf.GetNearestZombie(plant.board, shootPos, condition);
            if (target == null || target.IsDestroyed()) return null;

            Vector2 dir = target.col.bounds.center - shootPos;

            GameObject particle = CreateParticle.SetParticle(Plugin.ParticleID, shootPos, 11);
            particle.transform.rotation = MathHelper.DirectionToRotation(dir);
            if (plant.starUp)
            {
                particle.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                var b = PlantMgr.SetBullet(plant,BulletType.Bullet_doom_big, BulletMoveWay.Free,plant.attackDamage * 12);
                b.transform.rotation = MathHelper.DirectionToRotation(dir);
                b.theStatus = BulletStatus.Doom_big;
            }

            RaycastHit2D[] hits = Physics2D.RaycastAll(shootPos, dir, float.PositiveInfinity, LayerMask.GetMask("Zombie"));

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.TryGetComponent<Zombie>(out var z))
                {
                    int dmg = plant.attackDamage * 6;
                    z.SetEmbered(true);
                    if (z.HasBuff(EffectType.Ember))
                    {
                        if (z.HasBuff(EffectType.Jala))
                        {
                            z.JalaedExplode(true, dmg);
                        }
                        if (z.HasBuff(EffectType.Poison))
                        {
                            z.DamagedByPoison(dmg / 40f);
                        }
                        if (z.HasBuff(EffectType.Cold))
                        {
                            z.SetFreeze(2);
                            dmg *= 4;
                        }
                    }
                    z.TakeDamage(DmgType.NormalAll, dmg, plant.thePlantType); //100 % crit chance
                    if (MathHelper.ApproximatelyZero(plant.crazeTimer)) plant.craze++;
                    if (plant.attributeCount >= (Lawnf.TravelAdvanced(Plugin.Buff2) ? 151 : 301))
                    {
                        var dmg_doom=plant.attackDamage * 72;
                        int col = z.Column;
                        plant.board.boardAction.SetDoom(
                            col,
                            z.theZombieRow,
                            false,
                            false,
                            Vector2.zero,
                            dmg_doom*plant.shootingLevel+dmg_doom,
                            0,
                            null,
                            true,
                            plant.thePlantType
                        );
                    }
                    else
                    {
                        plant.attributeCount++;
                    }
                }
            }
            if (plant.attributeCount >= (Lawnf.TravelAdvanced(Plugin.Buff2) ? 151 : 301))
            {
                plant.attributeCount = 0;
            }

            if (plant.craze >= (Lawnf.TravelAdvanced(Plugin.Buff2) ? 50 : 100))
            {
                plant.crazeTimer = Lawnf.TravelAdvanced(Plugin.Buff2) ? 16 : 8;
                plant.craze = 0;
            }

            return null;
        }
        public void Flip(bool backwards)
        {
            isFlipped = backwards;
            plant.anim.SetBoolString("backwards", backwards);
        }
    }
    public class DoomSniperComponent : MonoBehaviour, IPlantGetTextStringHandler
    {
        public DoomSniper plant => GetComponent<DoomSniper>();
        public void Start()
        {
            plant.shoot=transform.FindChild("PeaShooter_Head/gun_lower/Shoot");
        }
        public string GetTextString() =>
        $"大招充能 : {plant.attributeCount}/{(Lawnf.TravelAdvanced(Plugin.Buff2) ? 150 : 300)}\n" +
        $"狂热能量 : {plant.craze}/{(Lawnf.TravelAdvanced(Plugin.Buff2) ? 50 : 100)}\n" +
        $"狂热时间 : {Mathf.CeilToInt(plant.crazeTimer)}/{(Lawnf.TravelAdvanced(Plugin.Buff2) ? 16 : 8)}";
    }
}
