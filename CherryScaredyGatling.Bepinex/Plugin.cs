using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using DoomGatlingBlover.BepInEx;

namespace CherryScaredyGatling{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "CherryScaredyGatling.Bepinex";
        public const string PluginName = "CherryScaredyGatling";
        public const string PluginVersion = "3.5.1";
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            ClassInjector.RegisterTypeInIl2Cpp<CherryScaredyGatling>();
            ClassInjector.RegisterTypeInIl2Cpp<UltimateCherryScaredyGatling>();
            AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "cherryscaredygatling");

            CustomCore.RegisterCustomPlant<SuperDoomScaredy, CherryScaredyGatling>(
                CherryScaredyGatling.PLANT_ID,
                assetBundle.GetAsset<GameObject>("SuperDoomScaredyPrefab"),
                assetBundle.GetAsset<GameObject>("SuperDoomScaredyPreview"),
                new System.Collections.Generic.List<(int, int)> { ((int)PlantType.UltimateGatling,(int)PlantType.ScaredyShroom), ((int)PlantType.ScaredyShroom,(int)PlantType.UltimateGatling) },
                1.5f,
                0f,
                1500,
                300,
                0f,
                1025
            );
            CustomCore.AddUltimatePlant (CherryScaredyGatling.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(CherryScaredyGatling.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add (CherryScaredyGatling.PLANT_ID, CardLevel.Gold);
            CustomCore.AddPlantAlmanacStrings(
                CherryScaredyGatling.PLANT_ID,
                $"究极樱桃胆小菇({CherryScaredyGatling.PLANT_ID})",
                "融合了樱桃火力与胆小菇特性的高速散射机枪，实验室事故产物之一。\n" +
                "<color=#3D1400>融合配方：</color><color=red>究极樱桃射手 + 胆小菇</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>三向超级樱桃弹，持续输出强劲</color>\n" +
                "<color=#3D1400>特性：</color><color=red>胆小触发爆炸 / 攻速随战斗加快 / 樱桃散射</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①每次射击发射三向超级樱桃弹。\n" +
                "②胆小触发时会引发樱桃爆炸，造成范围伤害。\n" +
                "③攻击间隔会随射击逐渐缩短，越打越快。</color>\n" +
                "<color=#3D1400>词条1：</color><color=red>胆怯爆破：胆小触发时造成樱桃爆炸</color>\n" +
                "<color=#3D1400>词条2：</color><color=red>樱桃加速：攻击间隔随射击逐渐缩短</color>\n\n" +

                "<color=#3D1400>速射樱桃胆小菇说：</color><color=red>“我知道我看起来很害怕……但那只是引爆前的预兆！别靠太近，我紧张的时候会‘砰’的一声。”</color>\n\n" +

                "<color=#3D1400>English Description:</color>\n" +
                "<color=red>A hybrid of cherry firepower and Scaredy-Shroom instincts, created during one of Dave's less-controlled lab experiments.</color>\n\n" +
                "<color=#3D1400>Traits:</color><color=red> Cherry spread / Fear-triggered explosions / Increasing fire rate</color>\n" +
                "<color=#3D1400>Notes:</color><color=red> Fires three-way super-cherry shots. When frightened, triggers a cherry explosion dealing area damage. Attack interval decreases over time, allowing rapid-fire output.</color>\n\n" +
                "<color=#3D1400>Cherry Scaredy Gatling says:</color><color=red>“I know I look nervous… but that's just the warning before I explode. Literally.”</color>"
            );

            CustomCore.RegisterCustomPlant<SuperDoomScaredy, UltimateCherryScaredyGatling>(
                UltimateCherryScaredyGatling.PLANT_ID,
                assetBundle.GetAsset<GameObject>("SuperDoomScaredyPrefab 1"),
                assetBundle.GetAsset<GameObject>("SuperDoomScaredyPreview"),
                new System.Collections.Generic.List<(int, int)> {  },
                1.5f,
                0f,
                1500,
                300,
                0f,
                9999
            );
            CustomCore.AddUltimatePlant (UltimateCherryScaredyGatling.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(UltimateCherryScaredyGatling.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add (UltimateCherryScaredyGatling.PLANT_ID, CardLevel.Red);
            CustomCore.AddPlantAlmanacStrings(
                UltimateCherryScaredyGatling.PLANT_ID,
                $"终极速射樱桃胆小菇({UltimateCherryScaredyGatling.PLANT_ID})",
                "实验室事故的最终稳定形态，胆小菇与樱桃火力完全融合后的究极体。\n" +
                "<color=#0000FF>究极樱桃胆小菇的限定形态</color>\n\n" +
                "<color=#3D1400>获得方式：</color><color=red>究极樱桃胆小菇有10%概率进化</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>三向超级樱桃弹 + 狂暴樱桃风暴（PF）</color>\n" +
                "<color=#3D1400>特性：</color><color=red>胆小爆破 / 核爆协同 / PF超级扫射 / 攻速无限加快</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①每次射击发射三向超级樱桃弹。\n" +
                "②胆小触发时造成樱桃爆炸，并可引发核爆（Doom）。\n" +
                "③PF时进入樱桃风暴模式，500连射覆盖全场。\n" +
                "④攻击间隔随射击不断缩短，极限攻速可达疯狂级别。</color>\n" +
                "<color=#3D1400>词条1：</color><color=red>核爆胆怯：胆小触发时引发核爆</color>\n" +
                "<color=#3D1400>词条2：</color><color=red>樱桃风暴：PF时进行500连射超级扫射</color>\n\n" +

                "<color=#3D1400>究极速射樱桃胆小菇说：</color><color=red>“我已经不害怕了……因为现在，是僵尸该害怕我。”</color>\n\n" +

                "<color=#3D1400>English Description:</color>\n" +
                "<color=red>The final stabilized form of the Cherry-Scaredy fusion experiment. Its power output is considered 'unsafe' by every known standard.</color>\n\n" +
                "<color=#3D1400>Traits:</color><color=red> Cherry spread / Doom synergy / PF cherry storm / Extreme fire-rate scaling</color>\n" +
                "<color=#3D1400>Notes:</color><color=red> Fires three-way super-cherry shots. Fear triggers cherry explosions and can cause nuclear Doom blasts. PF mode unleashes a 500-shot cherry storm. Attack interval decreases continuously, reaching absurd speeds.</color>\n\n" +
                "<color=#3D1400>Ultimate Cherry Scaredy Gatling says:</color><color=red>“I'm not scared anymore… because now the zombies should be.”</color>"
            );

            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    public class CherryScaredyGatling : MonoBehaviour
    {
        public static ID PLANT_ID = 3010;
        public int explodeCooling=0;
        public SuperDoomScaredy plant => gameObject.GetComponent<SuperDoomScaredy>();
        public void Start()
        {
            plant.shoot = transform.FindChild("Shoot");
        }
        public void Update()
        {
            if (plant != null && GameAPP.theGameStatus == GameStatus.InGame)
            {
                if (Lawnf.TravelUltimate(UltiBuff.EnumValue3))
                    plant.thePlantAttackCountDown -= Time.deltaTime;
            }
        }
        public void AnimShoot_Custom()
        {
            if (plant == null) Debug.LogError("plant is NULL");
            if (plant.shoot == null) Debug.LogError("plant.shoot is NULL");

            if (CreateBullet.Instance == null)
                return;
            Transform t = plant.shoot;
            Vector3 pos = t.position;

            Bullet b =CreateBullet.Instance.SetBullet(
                pos.x,
                pos.y,
                plant.thePlantRow,
                BulletType.Bullet_superCherry,
                BulletMoveWay.MoveRight,
                false
            );
            b.Damage = plant.attackDamage;
            b.fromType=plant.thePlantType;

            Bullet b1 = CreateBullet.Instance.SetBullet(
                pos.x,
                pos.y,
                plant.thePlantRow,
                BulletType.Bullet_superCherry,
                BulletMoveWay.Free,
                false
            );
            b1.Damage = plant.attackDamage;
            b1.fromType=plant.thePlantType;
            b1.transform.Rotate(0f, 0f, 15f);

            Bullet b2 = CreateBullet.Instance.SetBullet(
                pos.x,
                pos.y,
                plant.thePlantRow,
                BulletType.Bullet_superCherry,
                BulletMoveWay.Free,
                false
            );
            b2.Damage = plant.attackDamage;
            b2.fromType=plant.thePlantType;
            b2.transform.Rotate(0f, 0f, -15f);
            if(explodeCooling<50)explodeCooling++;
            float interval = plant.thePlantAttackInterval;
            if (interval > 0.2f)
                interval = interval - 0.1f;
            plant.thePlantAttackInterval = interval;
        }
    }
    public class UltimateCherryScaredyGatling : MonoBehaviour
    {
        public static ID PLANT_ID = 3011;//please update ID
        public int explodeCooling=0;
        public int counter=0;
        public bool isPF=false;
        public SuperDoomScaredy plant => gameObject.GetComponent<SuperDoomScaredy>();
        public void Start()
        {
            plant.shoot = transform.FindChild("Shoot");
        }
        public void Update()
        {
            if (plant != null && GameAPP.theGameStatus == GameStatus.InGame)
            {
                if (Lawnf.TravelUltimate(UltiBuff.EnumValue3))
                    plant.thePlantAttackCountDown -= Time.deltaTime;
            }
        }
        public virtual IEnumerator SuperShoot_Custom()
        {
            if (plant.shoot == null || isPF)
            {
                yield break;
            }
            isPF = true;
            plant.invincible = true;
            plant.uncrashable = true;
            plant.flashCountDown = 5f;
            plant.isFlashing = true;

            for (int i = 0; i < 500; i++)
            {
                Vector3 pos = plant.shoot.position;
                Bullet b=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, plant.thePlantRow,
                    BulletType.Bullet_superCherry, BulletMoveWay.Right_free
                );
                switch (b.theBulletType)
                {
                    case (BulletType)3153: b.Damage=plant.attackDamage*6; b.theStatus = BulletStatus.Doom_big; break;
                    default: b.Damage=plant.attackDamage; break;
                }
                b.transform.Rotate(0,0,Random.Range(-15f,15f));
                Bullet b1=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y+0.3f, plant.thePlantRow,
                    BulletType.Bullet_superCherry, BulletMoveWay.Right_free
                );
                switch (b1.theBulletType)
                {
                    case (BulletType)3153: b1.Damage=plant.attackDamage*6; b1.theStatus = BulletStatus.Doom_big; break;
                    default: b1.Damage=plant.attackDamage; break;
                }
                b1.transform.Rotate(0,0,Random.Range(-15f,15f));
                Bullet b2=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y-0.3f, plant.thePlantRow,
                    BulletType.Bullet_superCherry, BulletMoveWay.Right_free
                );
                switch (b2.theBulletType)
                {
                    case (BulletType)3153: b2.Damage=plant.attackDamage*6; b2.theStatus = BulletStatus.Doom_big; break;
                    default: b2.Damage=plant.attackDamage; break;
                }
                b2.transform.Rotate(0,0,Random.Range(-15f,15f));

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
        public IEnumerator StarShoot(Transform firePosition)
        {
            Vector3 pos = firePosition.position;
            for(int i = 0; i < counter; i++)
            {
                Bullet b =CreateBullet.Instance.SetBullet(
                    pos.x,
                    pos.y,
                    plant.thePlantRow,
                    BulletType.Bullet_superCherry,
                    BulletMoveWay.MoveRight,
                    false
                );
                b.Damage = plant.attackDamage;
                b.fromType=plant.thePlantType;

                Bullet b1 = CreateBullet.Instance.SetBullet(
                    pos.x,
                    pos.y,
                    plant.thePlantRow,
                    BulletType.Bullet_superCherry,
                    BulletMoveWay.Free,
                    false
                );
                b1.Damage = plant.attackDamage;
                b1.fromType=plant.thePlantType;
                b1.transform.Rotate(0f, 0f, 15f);

                Bullet b2 = CreateBullet.Instance.SetBullet(
                    pos.x,
                    pos.y,
                    plant.thePlantRow,
                    BulletType.Bullet_superCherry,
                    BulletMoveWay.Free,
                    false
                );
                b2.Damage = plant.attackDamage;
                b2.fromType=plant.thePlantType;
                b2.transform.Rotate(0f, 0f, -15f);
                if(Random.Range(0,100)<5)plant.StartCoroutine(SuperShoot_Custom());
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
        }
        public void AnimShoot_Custom()
        {
            if (plant == null) Debug.LogError("plant is NULL");
            if (plant.shoot == null) Debug.LogError("plant.shoot is NULL");
            Transform t = plant.shoot;
            Vector3 pos = t.position;
            if (CreateBullet.Instance == null)
                return;

            Bullet b =CreateBullet.Instance.SetBullet(
                pos.x,
                pos.y,
                plant.thePlantRow,
                BulletType.Bullet_superCherry,
                BulletMoveWay.MoveRight,
                false
            );
            b.Damage = plant.attackDamage;
            b.fromType=plant.thePlantType;

            Bullet b1 = CreateBullet.Instance.SetBullet(
                pos.x,
                pos.y,
                plant.thePlantRow,
                BulletType.Bullet_superCherry,
                BulletMoveWay.Free,
                false
            );
            b1.Damage = plant.attackDamage;
            b1.fromType=plant.thePlantType;
            b1.transform.Rotate(0f, 0f, 15f);

            Bullet b2 = CreateBullet.Instance.SetBullet(
                pos.x,
                pos.y,
                plant.thePlantRow,
                BulletType.Bullet_superCherry,
                BulletMoveWay.Free,
                false
            );
            b2.Damage = plant.attackDamage;
            b2.fromType=plant.thePlantType;
            b2.transform.Rotate(0f, 0f, -15f);
            plant.StartCoroutine(
                    StarShoot (t)
            );
            explodeCooling++;
            float interval = plant.thePlantAttackInterval;
            if (interval > 0.2f)
                interval = interval - 0.1f;
            else if (counter < 10)
                counter++;
            plant.thePlantAttackInterval = interval;
            if(Random.Range(0,100)<5)plant.StartCoroutine(SuperShoot_Custom());
        }
    }
    [HarmonyPatch(typeof(SuperDoomScaredy), nameof(SuperDoomScaredy.ScaredEvent))]
    public static class Patch_CherryScaredyGatling_ScaredEvent
    {
        [HarmonyPrefix]
        public static bool Prefix(SuperDoomScaredy __instance)
        {
            if (__instance == null)
                return true;

            int type = (int)__instance.thePlantType;
            bool isCherry   = type == CherryScaredyGatling.PLANT_ID;
            bool isUltimate = type == UltimateCherryScaredyGatling.PLANT_ID;

            if (!isCherry && !isUltimate)
                return true; // let original run for other plants

            var board = Board.Instance;
            if (board == null)
                return true;

            var cherryComp   = __instance.GetComponent<CherryScaredyGatling>();
            var ultimateComp = __instance.GetComponent<UltimateCherryScaredyGatling>();

            int cooling =
                cherryComp   != null ? cherryComp.explodeCooling :
                ultimateComp != null ? ultimateComp.explodeCooling : 0;

            if (cooling >= 2)
            {
                int col = __instance.thePlantColumn;
                int row = __instance.thePlantRow;

                Vector2 val = new Vector2(
                    Mouse.Instance.GetBoxXFromColumn(col),
                    Mouse.Instance.GetBoxYFromRow(row)
                );

                Board.Instance.boardAction.CreateCherryExplode(val, row, 0, 3600, 0, null);

                if (isUltimate)
                {
                    Vector2 doomPos = val;
                    Doom.SetDoom(board, doomPos, DoomType.Nuclear, null);
                }

                if (cherryComp != null)
                    cherryComp.explodeCooling -=2;
                if (ultimateComp != null)
                    ultimateComp.explodeCooling -=2;
            }

            // skip original ScaredEvent so coordination is NOT reset
            return false;
        }
    }
    [HarmonyPatch(typeof(Plant), nameof(Plant.Start))]
    public static class Plant_Start_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SuperDoomScaredy __instance)
        {
            if (__instance == null)
                return true;
            
            Board board = Board.Instance;
            if (board == null)
                return true;
            
            bool isGatling =
                __instance.thePlantType == (PlantType)CherryScaredyGatling.PLANT_ID;

            if (!isGatling || Random.value >= 0.1f)
                return true;

            var text = Object.FindObjectOfType<InGameText>();
            if (text != null)
                text.ShowText("Upgrade Successful", 3f);
            int row=__instance.thePlantRow;
            int col=__instance.thePlantColumn;
            __instance.Die();
            var createplants = CreatePlant.Instance;
            if (createplants == null)
                return true;
            createplants.SetPlant(col, row, (PlantType)UltimateCherryScaredyGatling.PLANT_ID, null, Vector2.zero, true, true, null);
            return false;
        }
    }
    [HarmonyPatch(typeof(Bullet_superCherry))]
    public static class Bullet_superCherryPatch
    {
        [HarmonyPatch(nameof(Bullet_superCherry.HitZombie))]
        [HarmonyPrefix]
        public static void Prefix(Bullet_superCherry __instance, ref Zombie zombie)
        {
            if (Lawnf.TravelAdvanced(UltimateDoomGatlingBlover.BuffID) && (__instance.fromType == CherryScaredyGatling.PLANT_ID || __instance.fromType == UltimateCherryScaredyGatling.PLANT_ID))
            {
                __instance.Damage *= 6;
                if (zombie.HasBuff(EffectType.Cold))
                {
                    if (zombie.freezeSpeed != 0f)
                        zombie.SetFreeze(1f);
                    __instance.Damage *= 4;
                }

                if (zombie.HasBuff(EffectType.Jala))
                    zombie.JalaedExplode(true, __instance.Damage);

                if (zombie.HasBuff(EffectType.Poison))
                    zombie.DamagedByPoison(__instance._damage / 40f);
                if (DoomBomb.HasBomb(zombie))
                {
                    var bomb = DoomBomb.TryAddBomb(zombie);
                    if (bomb == null) return;
                    bomb.AddDamage(__instance.Damage * 5);
                    bomb.SmallBomb();
                }
            }
        }
    }
    [HarmonyPatch(typeof(Bullet_cherrySquash))]
    public static class Bullet_cherrySquashPatch
    {
        [HarmonyPatch(nameof(Bullet_cherrySquash.HitZombie))]
        [HarmonyPrefix]
        public static void Prefix(Bullet_cherrySquash __instance, ref Zombie zombie)
        {
            if (Lawnf.TravelAdvanced(UltimateDoomGatlingBlover.BuffID) && (__instance.fromType == CherryScaredyGatling.PLANT_ID || __instance.fromType == UltimateCherryScaredyGatling.PLANT_ID))
            {
                if (zombie.HasBuff(EffectType.Cold))
                {
                    if (zombie.freezeSpeed != 0f)
                        zombie.SetFreeze(1f);
                    __instance.Damage *= 4;
                }

                if (zombie.HasBuff(EffectType.Jala))
                    zombie.JalaedExplode(true, __instance.Damage);

                if (zombie.HasBuff(EffectType.Poison))
                    zombie.DamagedByPoison(__instance._damage / 40f);
                if (DoomBomb.HasBomb(zombie))
                {
                    var bomb = DoomBomb.TryAddBomb(zombie);
                    if (bomb == null) return;
                    bomb.AddDamage(__instance.Damage * 5);
                    bomb.SmallBomb();
                }
            }
        }
    }
}
