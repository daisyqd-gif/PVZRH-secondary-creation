namespace MegaGatlingExpansion
{
    public class HypnoMegaGatlingPeaZombie : MegaGatlingPeaZombie
    {
        public override BulletType GetBulletType_Custom()
        {
            if (Plugin.Buff2 != -1 && Lawnf.TravelDebuff((TravelDebuff)Plugin.Buff2) || zombie.isMindControlled) return BulletProfile.HypnoMegaPeaZ;
            else return BulletType.Bullet_hypnoPea;
        }
    }

    public class SnowMegaGatlingPeaZombie : MegaGatlingPeaZombie
    {
        public override BulletType GetBulletType_Custom()
        {
            if (Plugin.Buff2 != -1 && Lawnf.TravelDebuff((TravelDebuff)Plugin.Buff2) || zombie.isMindControlled) return BulletType.Bullet_snowBall;
            else return BulletType.Bullet_snowPea;
        }
    }
    public class ElectricMegaGatlingPeaZombie : MegaGatlingPeaZombie
    {
        public override BulletType GetBulletType_Custom()
        {
            return BulletProfile.ElectricPea;
        }
    }
    public class DoomMegaGatlingPeaZombie : MegaGatlingPeaZombie
    {
        public override BulletType GetBulletType_Custom()
        {
            return BulletType.Bullet_doom_ulti;
        }
    }
}
