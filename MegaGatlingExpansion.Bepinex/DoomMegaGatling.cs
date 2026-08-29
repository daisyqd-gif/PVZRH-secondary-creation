namespace MegaGatlingExpansion
{
    public class DoomMegaGatlingPea : MegaGatlingPea
    {
        public int counter = 0;
        public override BulletType GetBulletType_Custom()
        {
            if (counter >= 16 || (counter >= 4 && Lawnf.TravelAdvanced(AdvBuff.EnumValue3)) || Random.Range(0, 100) < 5)
            {
                if (!Lawnf.TravelAdvanced(AdvBuff.EnumValue2))
                {
                    plant.thePlantAttackCountDown = 3f;
                }
                counter = 0;
                return BulletType.Bullet_doom_big;
            }
            else
            {
                counter++;
                return BulletType.Bullet_doom;
            }
        }
    }
    public class UltimateDoomMegaGatlingPea : DoomMegaGatlingPea
    {
        public override BulletType GetBulletType_Custom()
        {
            if (counter >= 16 || (counter >= 4 && Lawnf.TravelAdvanced(AdvBuff.EnumValue3)) || Random.Range(0, 100) < 5)
            {
                if (!Lawnf.TravelAdvanced(AdvBuff.EnumValue2))
                {
                    plant.thePlantAttackCountDown = 3f;
                }
                counter = 0;
                return BulletType.Bullet_doom_big_ulti;
            }
            else
            {
                counter++;
                return BulletType.Bullet_doom_ulti;
            }
        }
    }
}
