namespace UltimateSniper
{
    public class IceStar : CustomBigStar
    {
        public override Vector3 SetSize() => new Vector3(4f, 4f, 4f);
        public override void GlobalEvent()
        {
            Doom.SetDoom(InstanceManager.Board,transform.position,DoomType.IceDoom_big);
        }
        public override void PerZombieEvent(Zombie zombie, DamageType theDamageType, int theDamage, PlantType reportType = PlantType.Nothing, bool fix = false)
        {
            int dmg = 3600;
            zombie.SetPortaled(30f); //bosses are immune
            // PortalEffect bonus
            if (zombie.TryGetEffect<PortalEffect>(EffectType.Portal, out var portal) && portal.duration > 0)
                dmg += 1500;
            // Boss multiplier
            if (TypeMgr.IsBossZombie(zombie.theZombieType))
                dmg *= 3;
            // Ulti buff multiplier
            if (Lawnf.TravelAdvanced(Plugin.Buff1))
                dmg *= 3;
            base.PerZombieEvent(zombie, DamageType.IceAll, dmg, reportType, fix);
            zombie.AddBurn();
        }
        public override void CreateCustomStars(BulletType theBulletType = BulletType.Bullet_star)
        {
            bool hasUltiBuff = Lawnf.TravelAdvanced(Plugin.Buff1);

            int bulletCount = hasUltiBuff ? 90 : 30;
            int angleStep = hasUltiBuff ? 4 : 12;

            Vector3 pos = transform.position;
            int row = Mouse.Instance.GetRowFromY(pos.x, pos.y);

            // Spawn radial bullets
            for (int i = 0; i < bulletCount; i++)
            {
                Bullet b = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, row,
                    Plugin.BType_Explode_Ice,
                    BulletMoveWay.Free,
                    false
                );

                b.Damage = 3600;
                if(UltimateFlameGatling_Remade.IsRogue) b.Damage*=1000;
                b.transform.Rotate(0,0,i * angleStep);
            }
        }
    }
}
