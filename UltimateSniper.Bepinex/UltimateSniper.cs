using System;

namespace UltimateSniper
{
    public class UltimateSniper : BaseCustomPlant
    {
        public static ID PLANT_ID = 13035;//ID is a customizelib type used in all modern plant mods
        public int hitCount = 0;
        public FireSniper plant => gameObject.GetComponent<FireSniper>();
        public override void OnSpawn()
        {
            // Heart is now child index 0
            Transform heart = plant.transform.GetChild(0);

            plant.ac = heart.gameObject;
            plant.ac.SetActive(false);

            // SortingGroup
            var sg = plant.ac.GetComponent<SortingGroup>();
            if (sg == null)
                sg = plant.ac.AddComponent<SortingGroup>();

            sg.sortAtRoot = true;
            plant.r = sg;
        }
        public override void Update()
        {
            try{base.Update();}
            catch(Exception){}
        }
        public void AttackZombie(Zombie zombie,int damage)
        {
            if (hitCount >= 12 || (hitCount >= 12 && Lawnf.TravelUltimate(UltiBuff.EnumValue51)) || (plant.starUp && hitCount >= 4) || (plant.starUp && hitCount >= 2 && Lawnf.TravelUltimate(UltiBuff.EnumValue51)))
            {
                if (Lawnf.TravelUltimate(UltiBuff.EnumValue50)) plant.anim.SetTriggerString("shoot");
                if (zombie.theHealth >= 100000000)
                {
                    zombie.TakeDamage(DmgType.Carred, 100000000, plant.thePlantType);
                }
                else
                {
                    zombie.theHealth=0;
                    zombie.theFirstArmorHealth=0;
                    zombie.theSecondArmorHealth=0;
                    zombie.UpdateHealthText();
                    zombie.FirstArmorFall();
                    zombie.SecondArmorFall();
                    zombie.Die();
                }
                hitCount = 0;
            }
            else
            {
                hitCount++;
            }
            Vector2 pos = plant.ac.transform.position;
            pos = new Vector2(pos.x, Mouse.Instance.GetBoxYFromRow(plant.thePlantRow));
            var a = (Zombie z) => { z.SetPortaled(); z.SetJalaed(); z.JalaedExplode(); Burn.TryAddZombieBurn(z); };
            plant.board.boardAction.CreateCherryExplode(pos, Mouse.Instance.GetColumnFromX(pos.x), Plugin.cherryType, damage, plant.thePlantType, a);
            if (!plant.starUp) return;
            Doom.SetDoom(plant.board, pos, DoomType.Fire);
            for (int i = 0; i < 6; i++)
            {
                PlantMgr.SetBullet(plant,UltimateExplosivePea.BULLET_ID,BulletMoveWay.Free,Vector2.zero,i*60f).normalSpeed*=2;
            }
        }
        public override void AnimShoot_Custom() => plant.Shoot1();//use original shooting pipeline
        public override string GetTextString() => "充能:" + hitCount;
    }
    [HarmonyPatch(typeof(FireSniper), nameof(FireSniper.AttackZombie))]
    public static class FireSniper_AttackZombie_CustomCherry
    {
        [HarmonyPrefix]
        public static void Prefix(FireSniper __instance, Zombie zombie, int damage, int theDamageType)
        {
            // Only affect Ultimate Sniper, let vanilla FireSniper behave normally
            if (__instance == null)
                return;

            if (zombie == null)
                return;

            if(__instance.TryGetComponent<UltimateSniper>(out var a))
            {
                a.AttackZombie(zombie,damage);
            }

            if(__instance.TryGetComponent<UltimateFlameSniper>(out var b))
            {
                b.AttackZombie(zombie,damage);
            }
            return;
        }
    }
}