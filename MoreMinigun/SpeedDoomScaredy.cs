using System.Threading.Tasks;
using CustomPlantClass.Runtime.Tasks;

namespace MoreMinigun
{
    public class DoomMinigunScaredy : BaseCustomPlant
    {
        public GatlingDoomScaredy plant => gameObject.GetComponent<GatlingDoomScaredy>();

        public override Transform FindShoot() => plant.shoot = transform.FindChild("Shoot");
        public override Bullet Shoot_Custom()
        {
            ScaredyShoot();
            return null;
        }
        public override BulletType GetBulletType()
        {
            if (plant.doomTimes >= (Lawnf.TravelAdvanced((AdvBuff)3) ? 4 : 16))
            {
                plant.doomTimes = 0;
                return BulletType.Bullet_doom_big;
            }
            else
            {
                plant.doomTimes++;
                return BulletType.Bullet_doom;
            }
        }
        public virtual void ScaredyShoot()
        {
            if (plant.thePlantAttackInterval > 0.1f)
            {
                plant.thePlantAttackInterval -= 0.02f;
                plant.anim.speed += 0.375f;
            }

            plant.anim.speed = 1 + (0.5f - plant.thePlantAttackInterval) / 0.02f * 0.375f;

            plant.doomTimes++;

            if ((PlantMgr.GetPercent(0.5f) || plant.starUp) && !isPF) StartPF();

            int damage = plant.attackDamage;
            Bullet bullet = CreateBullet.Instance.SetBullet(
                plant.shoot.transform.position.x,
                plant.shoot.transform.position.y,
                plant.thePlantRow,
                GetBulletType(),
                BulletMoveWay.MoveRight,
                false);

            if (bullet != null)
            {
                bullet.Damage = damage;
                bullet.normalSpeed *= 2f;
                if (bullet.theBulletType == BulletType.Bullet_doom_big || bullet.theBulletType == BulletType.Bullet_doom_big_ulti)
                {
                    bullet.theStatus = BulletStatus.Doom_big;
                    bullet.Damage *= 6;
                }

            }

            Bullet bullet1 = CreateBullet.Instance.SetBullet(
                plant.shoot.transform.position.x,
                plant.shoot.transform.position.y,
                plant.thePlantRow,
                GetBulletType(),
                BulletMoveWay.Free,
                false);

            if (bullet1 != null)
            {
                bullet1.Damage = damage;
                bullet1.normalSpeed *= 2f;
                if (bullet1.theBulletType == BulletType.Bullet_doom_big || bullet1.theBulletType == BulletType.Bullet_doom_big_ulti)
                {
                    bullet1.theStatus = BulletStatus.Doom_big;
                    bullet1.Damage *= 6;
                }
                bullet1.transform.Rotate(0, 0, 15);

            }

            Bullet bullet2 = CreateBullet.Instance.SetBullet(
                plant.shoot.transform.position.x,
                plant.shoot.transform.position.y,
                plant.thePlantRow,
                GetBulletType(),
                BulletMoveWay.Free,
                false);

            if (bullet2 != null)
            {
                bullet2.Damage = damage;
                bullet2.normalSpeed *= 2f;
                if (bullet2.theBulletType == BulletType.Bullet_doom_big || bullet2.theBulletType == BulletType.Bullet_doom_big_ulti)
                {
                    bullet2.theStatus = BulletStatus.Doom_big;
                    bullet2.Damage *= 6;
                }
                bullet2.transform.Rotate(0, 0, -15);
            }

            GameAPP.PlaySound(Random.Range(3, 5), 0.5f, 1.0f);
        }
        protected override bool IsAsyncPF => true;
        protected override async Task SuperShoot_Async()
        {
            for (int i = 0; i < 500; i++)
            {
                int damage = plant.attackDamage;
                for (float j = -0.3f; j <= 0.3f; j += 0.3f)
                {
                    Bullet bullet = CreateBullet.Instance.SetBullet(
                        plant.shoot.transform.position.x,
                        plant.shoot.transform.position.y + j,
                        plant.thePlantRow,
                        GetBulletType(),
                        BulletMoveWay.SuperGatling,
                        false);

                    if (bullet != null)
                    {
                        bullet.Damage = damage;
                        bullet.normalSpeed *= 2f;
                        if (bullet.theBulletType == BulletType.Bullet_doom_big || bullet.theBulletType == BulletType.Bullet_doom_big_ulti)
                        {
                            bullet.theStatus = BulletStatus.Doom_big;
                            bullet.Damage *= 6;
                        }
                        bullet.transform.Rotate(0, 0, Random.Range(-30f, 30f));

                    }

                    Bullet bullet1 = CreateBullet.Instance.SetBullet(
                        plant.shoot.transform.position.x,
                        plant.shoot.transform.position.y + j,
                        plant.thePlantRow,
                        GetBulletType(),
                        BulletMoveWay.SuperGatling,
                        false);

                    if (bullet1 != null)
                    {
                        bullet1.Damage = damage;
                        bullet1.normalSpeed *= 2f;
                        if (bullet1.theBulletType == BulletType.Bullet_doom_big || bullet1.theBulletType == BulletType.Bullet_doom_big_ulti)
                        {
                            bullet1.theStatus = BulletStatus.Doom_big;
                            bullet1.Damage *= 6;
                        }
                        bullet1.transform.Rotate(0, 0, Random.Range(-30f, 30f));

                    }

                    Bullet bullet2 = CreateBullet.Instance.SetBullet(
                        plant.shoot.transform.position.x,
                        plant.shoot.transform.position.y + j,
                        plant.thePlantRow,
                        GetBulletType(),
                        BulletMoveWay.SuperGatling,
                        false);

                    if (bullet2 != null)
                    {
                        bullet2.Damage = damage;
                        bullet2.normalSpeed *= 2f;
                        if (bullet2.theBulletType == BulletType.Bullet_doom_big || bullet2.theBulletType == BulletType.Bullet_doom_big_ulti)
                        {
                            bullet2.theStatus = BulletStatus.Doom_big;
                            bullet2.Damage *= 6;
                        }
                        bullet2.transform.Rotate(0, 0, Random.Range(-30f, 30f));
                    }
                }
                await DelayTask.WaitForFixedUpdate(token);
            }
        }
        public override void SuperEnd()
        {
            if(plant.starUp) StartPF();
            else base.SuperEnd();
        }
    }


    public class UltimateDoomMinigunScaredy : DoomMinigunScaredy
    {
        public override BulletType GetBulletType()
        {
            if (plant.doomTimes >= (Lawnf.TravelAdvanced((AdvBuff)3) ? 4 : 16))
            {
                plant.doomTimes = 0;
                return BulletType.Bullet_doom_big_ulti;
            }
            else
            {
                plant.doomTimes++;
                return BulletType.Bullet_doom_ulti;
            }
        }
    }

    [HarmonyPatch(typeof(ScaredyShroom), nameof(ScaredyShroom.Shootable))]
    public static class ScaredyShroomPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ScaredyShroom __instance, ref bool __result)
        {
            if (__instance == null)
                return;
            if (__instance.TryGetComponent<DoomMinigunScaredy>(out var a))
            {
                if(a.isPF || __instance.starUp)
                {
                    __result=true;
                }
                __instance.anim.SetBool("shooting", __result);
                if (!__result)
                {
                    __instance.anim.speed = 1f;
                }
                if (Lawnf.TravelAdvanced((AdvBuff)2) && __result)
                    __instance.anim.Play("shooting");
                var clipInfo = __instance.anim.GetCurrentAnimatorClipInfo(1);
                if (clipInfo.Length > 0 && clipInfo[0].clip.name == "shooting" && __instance.thePlantAttackInterval <= 0.1f)
                    __instance.anim.speed = 2f;
            }
        }
    }
    [HarmonyPatch(typeof(ScaredyDoom), nameof(ScaredyDoom.ScaredEvent))]
    public static class ScaredyDoom_ScaredEvent_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ScaredyDoom __instance)
        {
            if (__instance == null)
                return true;

            if(__instance.TryGetComponent<DoomMinigunScaredy>(out var a) && a.isPF)
            {
                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(UltimateDoomScared), nameof(UltimateDoomScared.ScaredEvent))]
    public static class UltimateDoomScared_ScaredEvent_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(UltimateDoomScared __instance)
        {
            if (__instance == null)
                return true;

            if(__instance.TryGetComponent<UltimateDoomMinigunScaredy>(out var _))
            {
                return false;
            }

            return true;
        }
    }
}
