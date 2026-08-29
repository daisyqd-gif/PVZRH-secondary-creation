using GameLevel.RogueShooting;

namespace UltimateDoomSniper
{
    public class DestroySniper : BaseCustomPlant
    {
        public SniperPea plant => gameObject.GetComponent<SniperPea>();
        public static bool IsRogue { get => ShootingManager.Instance != null; }
        private float RogueCountDown = 0;
        public float RogueMaxCountDown = 5;
        private bool CanSuperShot = false;
        private float shootspeed = -1;
        public override void OnFixedUpdate()
        {
            if (!IsRogue) return;
            if (!CanSuperShot) RogueCountDown -= Time.deltaTime;
            if (RogueCountDown <= 0) CanSuperShot = true;
        }
        public override Bullet Shoot_Custom()
        {
            if (plant == null || plant.board == null)
                return null;

            Zombie target = plant.targetZombie;

            if (target == null || !plant.SearchUniqueZombie(target))
            {
                var z = plant.SearchZombie();
                if (z == null) return null;
                target = z.GetComponent<Zombie>();
            }

            if (target == null)
                return null;

            plant.attackCount++;
            GameAPP.PlaySound(0x28, 0.2f, 1f);

            int damage = plant.attackDamage;
            if ((plant.attackCount % 6 == 0 && !IsRogue) || (CanSuperShot && IsRogue) || Lawnf.TravelAdvanced(Plugin.Buff2))
            {
                damage = 1000000;
                RogueCountDown = RogueMaxCountDown;
                CanSuperShot = false;
            }

            AttackEffect(target, damage);

            if (target.theStatus == ZombieStatus.Dying || target.beforeDying)
                plant.targetZombie = null;
            if (shootspeed == -1) shootspeed = plant.thePlantAttackInterval;
            plant.thePlantAttackInterval = shootspeed / (Lawnf.TravelAdvanced(Plugin.Buff1) ? 2 : 1);
            return null;
        }
        public virtual Zombie AttackEffect(Zombie zombie, int dmg)
        {
            plant.board.boardAction.SetDoom(PlantMgr.getCol(zombie.col.bounds.center.x), zombie.theZombieRow, false, false, default, dmg, 0, null, !GameAPP.config.distablexplodeFlash, plant.thePlantType);
            return zombie;
        }
        public override string GetTextString() => $"充能 : {plant.attackCount % 6}";
    }
    public class UltimateDestroySniper : DestroySniper
    {
        public override Zombie AttackEffect(Zombie zombie, int dmg)
        {
            var a = (Zombie z) =>
            {
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
            };
            plant.board.boardAction.SetDoom(PlantMgr.getCol(zombie.col.bounds.center.x), zombie.theZombieRow, false, false, default, dmg, 0, a, !GameAPP.config.distablexplodeFlash, plant.thePlantType);
            return zombie;
        }
    }
}
