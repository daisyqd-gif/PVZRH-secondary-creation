using System.Threading.Tasks;
using CustomPlantClass.Runtime.Tasks;
using GameLevel.RogueShooting;

namespace MoreMinigun
{
    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class Minigun : BaseCustomPlant
    {
        public UltimateMinigun plant => GetComponent<UltimateMinigun>();
        public int shootCount = 0;
        public override Transform FindShoot() => transform.FindChild("GatlingPea_head/Shoot");
        public virtual int Damage => Lawnf.TravelAdvanced(Plugin.theCurseID) ? Mathf.CeilToInt(plant.attackDamage/2) : Mathf.CeilToInt(plant.attackDamage*plant.damageMultiplier*plant.damageMultiplier);
        public override BulletType GetBulletType() => isPF ?
            new List<BulletType>(){
                BulletType.Bullet_pea,
                BulletType.Bullet_snowPea,
                BulletType.Bullet_firePea_yellow,
                BulletType.Bullet_hypnoPea
            }.GetRandomItem() : 
            BulletType.Bullet_pea;

        public override Bullet Shoot_Custom()
        {
            if(isPF) return null;
            if ((PlantMgr.GetPercent(0.5f) && !plant.board.boardTag.rogueShooting || plant.starUp) && !isPF) StartPF();

            Bullet bullet = CreateBullet.Instance.SetBullet(
                plant.shoot.transform.position.x,
                plant.shoot.transform.position.y,
                plant.thePlantRow,
                GetBulletType(),
                BulletMoveWay.MoveRight,
                false);

            if (bullet != null)
            {
                bullet.Damage = Damage;
                bullet.normalSpeed *= 2f;
                if (bullet.theBulletType == BulletType.Bullet_doom_big || bullet.theBulletType == BulletType.Bullet_doom_big_ulti)
                {
                    bullet.theStatus = BulletStatus.Doom_big;
                    bullet.Damage *= 6;
                }
                bullet.from=plant;
                bullet.fromType=plant.thePlantType;
            }

            if(plant.board.boardTag.rogueShooting) goto RogueShootingReturn;

            Bullet bullet1 = CreateBullet.Instance.SetBullet(
                plant.shoot.transform.position.x,
                plant.shoot.transform.position.y,
                plant.thePlantRow,
                GetBulletType(),
                BulletMoveWay.Free,
                false);

            if (bullet1 != null)
            {
                bullet1.Damage = Damage;
                bullet1.normalSpeed *= 2f;
                if (bullet1.theBulletType == BulletType.Bullet_doom_big || bullet1.theBulletType == BulletType.Bullet_doom_big_ulti)
                {
                    bullet1.theStatus = BulletStatus.Doom_big;
                    bullet1.Damage *= 6;
                }
                bullet1.transform.Rotate(0, 0, 15);
                bullet1.from=plant;
                bullet1.fromType=plant.thePlantType;
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
                bullet2.Damage = Damage;
                bullet2.normalSpeed *= 2f;
                if (bullet2.theBulletType == BulletType.Bullet_doom_big || bullet2.theBulletType == BulletType.Bullet_doom_big_ulti)
                {
                    bullet2.theStatus = BulletStatus.Doom_big;
                    bullet2.Damage *= 6;
                }
                bullet2.transform.Rotate(0, 0, -15);
                bullet2.from=plant;
                bullet2.fromType=plant.thePlantType;
            }
            RogueShootingReturn:
            // Play sound
            GameAPP.PlaySound(Random.Range(3, 5), 0.5f, 1.0f); // Random sound

            return bullet;
        }
        public override void StartPF()
        {
            plant.anim.SetBoolString("shooting",true);
            base.StartPF();
        }
        protected override bool IsAsyncPF => true;
        protected async override Task SuperShoot_Async()
        {
            for (int i = 0; i < 500; i++)
            {
                if (plant.attributeCountdown <= 0f)
                {
                    plant.attributeCountdown = 1.0f;
                }
                if(plant.board.boardTag.rogueShooting)
                {
                    Bullet bullet = CreateBullet.Instance.SetBullet(
                        plant.shoot.transform.position.x,
                        plant.shoot.transform.position.y,
                        plant.thePlantRow,
                        GetBulletType(),
                        BulletMoveWay.SuperGatling,
                        false);

                    if (bullet != null)
                    {
                        bullet.Damage = Damage;
                        bullet.normalSpeed *= 2f;
                        if (bullet.theBulletType == BulletType.Bullet_doom_big || bullet.theBulletType == BulletType.Bullet_doom_big_ulti)
                        {
                            bullet.theStatus = BulletStatus.Doom_big;
                            bullet.Damage *= 6;
                        }
                        bullet.transform.Rotate(0, 0, Random.Range(-30f, 30f));
                        bullet.from=plant;
                        bullet.fromType=plant.thePlantType;
                    }
                }
                else
                {
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
                            bullet.Damage = Damage;
                            bullet.normalSpeed *= 2f;
                            if (bullet.theBulletType == BulletType.Bullet_doom_big || bullet.theBulletType == BulletType.Bullet_doom_big_ulti)
                            {
                                bullet.theStatus = BulletStatus.Doom_big;
                                bullet.Damage *= 6;
                            }
                            bullet.transform.Rotate(0, 0, Random.Range(-30f, 30f));
                            bullet.from=plant;
                            bullet.fromType=plant.thePlantType;
                        }
                    }
                }
                await DelayTask.WaitForFixedUpdate(token);
            }
        }
        public override string GetTextString() => Lawnf.TravelAdvanced(Plugin.theCurseID) ? 
        $"{Mathf.CeilToInt(plant.damageMultiplier) - 100} / 500" : "";
        public override void SuperEnd()
        {
            if(plant.starUp) StartPF();
            else base.SuperEnd();
        }
    }
    public class SnowMinigun : Minigun
    {
        public override BulletType GetBulletType() =>
        Lawnf.Get3x3Plants(plant.thePlantColumn, plant.thePlantRow).ToSystemList()
            .Any(p => p.thePlantType == PlantType.IceBean)
            ? BulletType.Bullet_extremeSnowPea
            : BulletType.Bullet_snowPea;
    }
    public class JalaMinigun : Minigun
    {
        public override BulletType GetBulletType() => BulletType.Bullet_pea_jala;
    }
    public class CherryMinigun : Minigun
    {
        public override BulletType GetBulletType() => 
        isPF ? BulletType.Bullet_pea_bombCherry : BulletType.Bullet_pea_threeCherry;
    }
    public class CherryMinigun_Ulti : Minigun
    {
        public override Transform FindShoot() => transform.FindChild("Shoot");
        public override BulletType GetBulletType() => 
        Lawnf.TravelAdvanced(Plugin.theSuperID) ? BulletType.Bullet_cherryJalapeno :
        (
            Lawnf.TravelAdvanced(Plugin.theCurseID) ? 
            BulletType.Bullet_pea_bombCherry : 
            BulletType.Bullet_superCherry
        );
    }
    public class DoomMinigun : Minigun
    {
        public override BulletType GetBulletType()
        {
            if (shootCount >= (Lawnf.TravelAdvanced((AdvBuff)3) ? 4 : 16))
            {
                shootCount = 0;
                return BulletType.Bullet_doom_big;
            }
            else
            {
                shootCount++;
                return BulletType.Bullet_doom;
            }
        }
    }
    public class UltimateDoomMinigun : Minigun
    {
        public override BulletType GetBulletType()
        {
            if (shootCount >= (Lawnf.TravelAdvanced((AdvBuff)3) ? 4 : 16))
            {
                shootCount = 0;
                return BulletType.Bullet_doom_big_ulti;
            }
            else
            {
                shootCount++;
                return BulletType.Bullet_doom_ulti;
            }
        }
    }

    [HarmonyPatch(typeof(UltimateMinigun), nameof(UltimateMinigun.Shootable))]
    public static class UltimateMinigun_Shootable_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(UltimateMinigun __instance)
        {
            if (__instance == null)
                return true;
            
            var a = __instance.GetOrAddComponent<Minigun>();

            if(a.isPF || __instance.starUp)
            {
                __instance.anim.SetBoolString("shooting",true);
            }
            else
            {
                var b=CanShoot(__instance);
                __instance.anim.SetBoolString("shooting",b);
                if (b)
                {
                    if (__instance.attributeCountdown <= 0f)
                    {
                        __instance.attributeCountdown = 1.0f;
                    }
                }
                else
                {
                    __instance.damageMultiplier = 1;
                }
                if(b && Lawnf.TravelAdvanced(AdvBuff.EnumValue2) && (__instance.TryGetComponent<DoomMinigun>(out var _) || __instance.TryGetComponent<UltimateDoomMinigun>(out var _)))
                {
                    __instance.anim.Play("shooting");
                }
            }
            return false;
        }
        public static bool CanShoot(UltimateMinigun a)
        {
            // 1. Try to find a normal zombie
            var z = a.SearchZombie();
            if (z != null)
                return true;

            // 2. Try to find a boss
            var boss = a.SearchBoss();
            if (boss != null)
                return true;

            // 3. Try to find a gold magnet target
            if (a.SearchGoldMagnet())
                return true;

            // 4. Otherwise, only shoot if the plant is a ghost
            return false;
        }
    }

    [HarmonyPatch(typeof(GameLevel.RogueShooting.UltimateMinigun))]
    public static class UltimateMinigun_Buffs_Patch
    {
        [HarmonyPatch(nameof(GameLevel.RogueShooting.UltimateMinigun.Buffs), MethodType.Getter)]
        [HarmonyPostfix]
        public static void Postfix(ref Il2CppSystem.Collections.Generic.List<BaseBuff> __result)
        {
            __result.Add(new StarUpBuff(PlantType.UltimateMinigun));
            __result.Add(Plugin.realCherryBomb);
            __result.Add(Plugin.theBuff);
        }
    }
}
