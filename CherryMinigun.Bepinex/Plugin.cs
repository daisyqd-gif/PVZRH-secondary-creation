using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using System.Reflection;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using System.Collections;
using System;
using Random=UnityEngine.Random;


namespace CherryMinigun
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "CherryMinigun.Bepinex";
        public const string PluginName = "CherryMinigun";
        public const string PluginVersion = "3.5.1";

        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            ClassInjector.RegisterTypeInIl2Cpp<Minigun>();
            ClassInjector.RegisterTypeInIl2Cpp<CherryMinigun>();
            ClassInjector.RegisterTypeInIl2Cpp<UltimateMinigunPlus>();
            ClassInjector.RegisterTypeInIl2Cpp<PeaMinigun>();
            ClassInjector.RegisterTypeInIl2Cpp<SnowMinigun>();
            ClassInjector.RegisterTypeInIl2Cpp<JalaMinigun>();

            AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "cherryminigun");

            CustomCore.RegisterCustomPlant<UltimateMinigun, PeaMinigun>(
                PeaMinigun.PLANT_ID,
                assetBundle.GetAsset<GameObject>("PeaMinigunPrefab"),
                assetBundle.GetAsset<GameObject>("PeaMinigunPreview"),
                new List<(int, int)> { ((int)PlantType.GatlingPea, (int)PlantType.GatlingPea) },
                0.2f,
                0f,
                20,
                300,
                0f,
                1000
            );
            CustomCore.AddUltimatePlant(PeaMinigun.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(PeaMinigun.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add(PeaMinigun.PLANT_ID, CardLevel.Purple);
            
            CustomCore.RegisterCustomPlant<UltimateMinigun, SnowMinigun>(
                SnowMinigun.PLANT_ID,
                assetBundle.GetAsset<GameObject>("SnowMinigunPrefab"),
                assetBundle.GetAsset<GameObject>("SnowMinigunPreview"),
                new List<(int, int)> { ((int)PlantType.GatlingPea, (int)PlantType.SnowGatling), ((int)PlantType.SnowGatling, (int)PlantType.GatlingPea), (PeaMinigun.PLANT_ID, (int)PlantType.IceShroom), ((int)PlantType.IceShroom, PeaMinigun.PLANT_ID) },
                0.2f,
                0f,
                20,
                300,
                0f,
                1000
            );
            CustomCore.AddUltimatePlant(SnowMinigun.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(SnowMinigun.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add(SnowMinigun.PLANT_ID, CardLevel.Purple);
            CustomCore.TypeMgrExtra.IsIcePlant.Add(SnowMinigun.PLANT_ID);

            CustomCore.RegisterCustomPlant<UltimateMinigun, JalaMinigun>(
                JalaMinigun.PLANT_ID,
                assetBundle.GetAsset<GameObject>("JalaMinigunPrefab"),
                assetBundle.GetAsset<GameObject>("CherryMinigunPreview"), //intentional
                new List<(int, int)> { ((int)PlantType.GatlingPea, (int)PlantType.JalaGatling), ((int)PlantType.JalaGatling, (int)PlantType.GatlingPea), (PeaMinigun.PLANT_ID, (int)PlantType.Jalapeno), ((int)PlantType.Jalapeno, PeaMinigun.PLANT_ID) },
                0.2f,
                0f,
                40,
                300,
                0f,
                1000
            );
            CustomCore.AddUltimatePlant(JalaMinigun.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(JalaMinigun.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add(JalaMinigun.PLANT_ID, CardLevel.Purple);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(JalaMinigun.PLANT_ID);

            CustomCore.RegisterCustomPlant<UltimateMinigun, CherryMinigun>(
                CherryMinigun.PLANT_ID,
                assetBundle.GetAsset<GameObject>("CherryMinigunPrefab"),
                assetBundle.GetAsset<GameObject>("CherryMinigunPreview"),
                new List<(int, int)> { ((int)PlantType.GatlingPea, (int)PlantType.CherryGatling), ((int)PlantType.CherryGatling, (int)PlantType.GatlingPea), (PeaMinigun.PLANT_ID, (int)PlantType.CherryBomb), ((int)PlantType.CherryBomb, PeaMinigun.PLANT_ID) },
                0.2f,
                0f,
                80,
                300,
                0f,
                1000
            );

            CustomCore.AddUltimatePlant(CherryMinigun.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(CherryMinigun.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add(CherryMinigun.PLANT_ID, CardLevel.Purple);
            CustomCore.AddPlantAlmanacStrings(
                CherryMinigun.PLANT_ID,
                $"速射樱桃机枪({CherryMinigun.PLANT_ID})",
                "高速樱桃散弹机枪，能在短时间内倾泻大量火力，越战越勇\n" +
                "<color=#3D1400>融合配方：</color><color=red>樱桃机枪射手+超级机枪射手</color>\n" +
                "<color=#3D1400>转化配方：</color><color=red>铲除←→樱桃</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>多发散射樱桃弹，持续输出极高</color>\n" +
                "<color=#3D1400>特性：</color><color=red>樱桃散射 / 高速连射 / 小概率触发樱桃狂热（PF）</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①每次射击发射三向樱桃散弹，覆盖范围广。\n" +
                "②有小概率进入樱桃狂热状态，短时间内疯狂扫射。\n" +
                "③与攻速类植物、增伤类植物协同效果极佳。</color>\n" +
                "<color=#3D1400>词条1：</color><color=red>樱桃过载：攻击速度提升，散射角度扩大</color>\n" +
                "<color=#3D1400>词条2：</color><color=red>樱爆连锁：樱桃子弹有概率变为超级樱桃弹</color>\n\n" +
                "<color=#3D1400>速射樱桃机枪说：</color><color=red>“我不是普通的樱桃，我是高速旋转的樱桃风暴！" +
                "当战场混乱、敌人蜂拥而至时，我的枪管才真正开始发光。" +
                "有人说我太吵，有人说我太疯，但我只知道一件事：" +
                "只要我还在旋转，僵尸就别想靠近半步。”</color>\n\n" +

                // ---------- English Section ----------
                "<color=#3D1400>English Description:</color>\n" +
                "<color=red>Cherry Minigun is a high-speed cherry scatter-gun that unleashes a storm of bullets.\n" +
                "The longer the fight lasts, the faster it fires.\n" +
                "It excels when paired with attack-speed or damage-boosting plants.</color>\n\n" +
                "<color=#3D1400>Traits:</color><color=red> Cherry spread / Rapid fire / Chance to trigger Cherry Frenzy (PF)</color>\n" +
                "<color=#3D1400>Notes:</color><color=red> Fires three-way cherry shots.\n" +
                "Occasionally enters a frenzy mode, spraying bullets at extreme speed.\n" +
                "Synergizes extremely well with buff-heavy or fast-firing setups.</color>\n\n" +
                "<color=#3D1400>Cherry Minigun says:</color><color=red>“I'm not just a cherry — I'm a rotating cherry storm." +
                "When chaos hits the battlefield, that's when my barrels really start to shine." +
                "Some say I'm too loud, some say I'm too wild… but as long as I'm spinning," +
                "no zombie is getting past me.”</color>"
            );
            CustomCore.AddFusion(301,CherryMinigun.PLANT_ID,(int)PlantType.UltimateGatling);
            CustomCore.AddFusion(301,(int)PlantType.UltimateGatling,CherryMinigun.PLANT_ID);
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    public class CherryMinigun : Minigun
    {
        public static ID PLANT_ID = 10301;
        public override BulletType getBType()
        {
            BulletType btype;
            if (isPlantFooding)
            {
                btype=BulletType.Bullet_pea_bombCherry;
            }
            else
            {
                btype=BulletType.Bullet_pea_threeCherry;
            }
            return btype;
        }
    }
    public class PeaMinigun : Minigun
    {
        public static ID PLANT_ID = 10302;
        public override BulletType getBType()
        {
            return BulletType.Bullet_pea;
        }
    }
    public class SnowMinigun : Minigun
    {
        public static ID PLANT_ID = 10303;
        public override BulletType getBType()
        {
            try
            {
                foreach(Plant i in Lawnf.Get3x3Plants(plant.thePlantColumn,plant.thePlantRow))
                {
                    if (i.thePlantType == PlantType.IceBean)
                    {
                        return BulletType.Bullet_extremeSnowPea;
                    }
                }
                return BulletType.Bullet_snowPea;
            }
            catch (Exception)
            {
                return BulletType.Bullet_snowPea;
            }
        }
    }
    public class JalaMinigun : Minigun
    {
        public static ID PLANT_ID = 10304;
        public override BulletType getBType()
        {
            return BulletType.Bullet_pea_jala;
        }
    }
    public class UltimateMinigunPlus : Minigun
    {
        public override BulletType getBType()
        {
            return BulletType.Bullet_superCherry;
        }
    }
    public class Minigun : MonoBehaviour
    {
        public UltimateMinigun plant => gameObject.GetComponent<UltimateMinigun>();
        public bool isPlantFooding=false;
        public void Start()
        {
            plant.shoot = plant.GetComponentInChildren<Transform>(true)?.Find("Shoot");
        }
        public Bullet CreateSpreadBullets(Transform source, int row, BulletMoveWay dir, float yOffset, BulletType bullet, float rotation, float speed)
        {
            var cb = CreateBullet.Instance;
            if (cb == null || plant == null)
                return null;

            Vector3 pos = source.position;

            Bullet b = cb.SetBullet(
                pos.x,
                pos.y + yOffset,
                row,
                bullet,
                dir,
                false
            );

            if (b == null)
                return null;

            b.Damage  = plant.attackDamage;
            b.fromType = plant.thePlantType;
            b.transform.Rotate (0f, 0f, rotation);
            b.normalSpeed*=speed;

            return b;
        }
        public virtual BulletType getBType()
        {
            return BulletType.Bullet_pea;
        }
        public IEnumerator PlantFood()
        {
            isPlantFooding=true;
            for(int i=0; i < 500; i++)
            {
                if(plant==null) yield break;
                CreateSpreadBullets(
                    plant.shoot,
                    plant.thePlantRow,
                    BulletMoveWay.Right_free,
                    0f,
                    getBType(),
                    Random.Range(-30f,30f),
                    2f
                );
                CreateSpreadBullets(
                    plant.shoot,
                    plant.thePlantRow,
                    BulletMoveWay.Right_free,
                    0.3f,
                    getBType(),
                    Random.Range(-30f,30f),
                    2f
                );
                CreateSpreadBullets(
                    plant.shoot,
                    plant.thePlantRow,
                    BulletMoveWay.Right_free,
                    -0.3f,
                    getBType(),
                    Random.Range(-30f,30f),
                    2f
                );
                CreateSpreadBullets(
                    plant.shoot,
                    plant.thePlantRow,
                    BulletMoveWay.Right_free,
                    0f,
                    getBType(),
                    Random.Range(-30f,30f),
                    2f
                );
                CreateSpreadBullets(
                    plant.shoot,
                    plant.thePlantRow,
                    BulletMoveWay.Right_free,
                    0.3f,
                    getBType(),
                    Random.Range(-30f,30f),
                    2f
                );
                CreateSpreadBullets(
                    plant.shoot,
                    plant.thePlantRow,
                    BulletMoveWay.Right_free,
                    -0.3f,
                    getBType(),
                    Random.Range(-30f,30f),
                    2f
                );
                CreateSpreadBullets(
                    plant.shoot,
                    plant.thePlantRow,
                    BulletMoveWay.Right_free,
                    0f,
                    getBType(),
                    Random.Range(-30f,30f),
                    2f
                );
                CreateSpreadBullets(
                    plant.shoot,
                    plant.thePlantRow,
                    BulletMoveWay.Right_free,
                    0.3f,
                    getBType(),
                    Random.Range(-30f,30f),
                    2f
                );
                CreateSpreadBullets(
                    plant.shoot,
                    plant.thePlantRow,
                    BulletMoveWay.Right_free,
                    -0.3f,
                    getBType(),
                    Random.Range(-30f,30f),
                    2f
                );
                yield return new WaitForSeconds(0.01f);
            }
            isPlantFooding=false;
        }
        public void AnimShoot_Custom()
        {
            if(isPlantFooding) return;
            CreateSpreadBullets(
                plant.shoot,
                plant.thePlantRow,
                BulletMoveWay.Free,
                0f,
                getBType(),
                15f,
                2f
            );
            CreateSpreadBullets(
                plant.shoot,
                plant.thePlantRow,
                BulletMoveWay.Right_free,
                0f,
                getBType(),
                0f,
                2f
            );
            CreateSpreadBullets(
                plant.shoot,
                plant.thePlantRow,
                BulletMoveWay.Free,
                0f,
                getBType(),
                -15f,
                2f
            );
            if(Random.Range(0f,100f)<7.5f) plant.StartCoroutine (PlantFood());
        }
    }
    [HarmonyPatch(typeof(UltimateMinigun), "Shoot1")]
    public static class CherryMinigun_Shoot1_ReplaceBullet
    {
        [HarmonyPostfix]
        public static void Postfix(UltimateMinigun __instance, ref Bullet __result)
        {
            if (__result == null){
                Debug.Log("null detected!");
                return;
            }
            if(__instance.thePlantType == PlantType.UltimateMinigun)
            {
                var logic2 = __instance.GetOrAddComponent<UltimateMinigunPlus>();
                logic2.AnimShoot_Custom();
                if (Lawnf.TravelAdvanced(AdvBuff.EnumValue3002))
                {
                    var b1=CreateBullet.Instance.SetBullet(__result.transform.position.x,__result.transform.position.y,__result.theBulletRow,__result.theBulletType,BulletMoveWay.Sin);
                    b1.Damage=__result.Damage;
                    b1.fromType=__result.fromType;
                    var b2=CreateBullet.Instance.SetBullet(__result.transform.position.x,__result.transform.position.y,__result.theBulletRow,__result.theBulletType,BulletMoveWay.Sin);
                    b2.Damage=__result.Damage;
                    b2.fromType=__result.fromType;
                    b2.theExistTime+=0.5f;
                }
                __result.Die();
                return;
            }
        }
    }
}