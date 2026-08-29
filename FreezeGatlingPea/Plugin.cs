global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using System.Reflection;
global using UnityEngine;
global using CustomPlantClass;
global using CustomPlantClass.Main;

namespace FreezeGatlingPea
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Core : ModPlugin
    {
        private AssetBundle assetBundle;
        private ID plantType;
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "freezegatlingpea"
            );
        }
        public override void OnGameInit()
        {
            TypeData.SnowPlants.Add(plantType);
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = DataMgr.AllocateID(), // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("FreezeGatlingPeaPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("FreezeGatlingPeaPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorTuple((PlantType.GatlingPea,PlantType.WaterAloes)), // Optional fusion recipes

                AttackInterval = 1.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 20,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 475,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = false,     // Enable PF ability if the plant has one
                CanStarUp = false, // Enable Star-Up ability if the plant has one

                CardColor = CardLevel.Green, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = false, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "极冰机枪射手",           // Plant name (shown in UI)
                AlmanacEntry = "机枪射手一次可以发射四颗豌豆。当充能超过4时，可以发射四颗极冰豆。\n\n"+    // Almanac description (CN + EN recommended)
                "<color=#3D1400>融合配方：</color><color=red>机枪射手 + 水滴芦荟</color>\n"+
                "<color=#3D1400>伤害：</color><color=red>20×4/1.5秒</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①免疫冻结和冰封，受到雪球/雪叉/寒冰菇/旗帜波暴风雪效果时获得1/1/15/60层充能。可消耗1层充能投出极冰豆。</color>\n"
            };

            // Register the plant and retrieve its ID
            plantType = DataMgr.RegisterCustomPlant<GatlingPea, FreezeGatlingPea>(Data);

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class FreezeGatlingPea : BaseCustomPlant
    {
        public override Transform FindShoot() => _plant.transform.FindChild("GatlingPea_head/Shoot");
        public override Bullet Shoot_Custom()
        {
            ReplaceSprite();
            Vector2 pos=_plant.shoot.position;
            if (_plant.attributeCount >= 1)
            {
                Bullet b1=CreateBullet.Instance.SetBullet(pos.x,pos.y,_plant.thePlantRow,BulletType.Bullet_extremeSnowPea,BulletMoveWay.MoveRight);
                b1.Damage=_plant.attackDamage*2;
                b1.fromType=_plant.thePlantType;
                Bullet b4 = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, _plant.thePlantRow,
                    BulletType.Bullet_extremeSnowPea,
                    BulletMoveWay.Free
                );
                b4.Damage = _plant.attackDamage*2;
                b4.fromType = _plant.thePlantType;
                b4.transform.Rotate(0, 0, 45);
                Bullet b5 = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, _plant.thePlantRow,
                    BulletType.Bullet_extremeSnowPea,
                    BulletMoveWay.Free
                );
                b5.Damage = _plant.attackDamage*2;
                b5.fromType = _plant.thePlantType;
                b5.transform.Rotate(0, 0, 30);
                Bullet b2 = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, _plant.thePlantRow,
                    BulletType.Bullet_extremeSnowPea,
                    BulletMoveWay.Free
                );
                b2.Damage = _plant.attackDamage*2;
                b2.fromType = _plant.thePlantType;
                b2.transform.Rotate(0, 0, -30);
                Bullet b3 = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, _plant.thePlantRow,
                    BulletType.Bullet_extremeSnowPea,
                    BulletMoveWay.Free
                );
                b3.Damage = _plant.attackDamage*2;
                b3.fromType = _plant.thePlantType;
                b3.transform.Rotate(0, 0, -45);
                _plant.attributeCount=-1;
                return b1;
            }
            Bullet b=CreateBullet.Instance.SetBullet(pos.x,pos.y,_plant.thePlantRow,BulletType.Bullet_pea,BulletMoveWay.MoveRight);
            b.Damage=_plant.attackDamage;
            b.fromType=_plant.thePlantType;
            return b;
        }
        public override string GetTextString() => "充能: "+_plant.attributeCount;
        public void ReplaceSprite()
        {
            var head1=transform.FindChild("GatlingPea_head");
            var head2=transform.FindChild("SnowGatling_head");
            if (_plant.attributeCount >= 1)
            {
                head1.gameObject.SetActive(false);
                head2.gameObject.SetActive(true);
            }
            else
            {
                head1.gameObject.SetActive(true);
                head2.gameObject.SetActive(false);
            }
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "FreezeGatlingPea.Bepinex";
        public const string PluginName = "FreezeGatlingPea";
        public const string PluginVersion = "3.7";
    }

    [HarmonyPatch(typeof(Plant))]
    public static class Plant_Patch
    {
        // -------------------------
        //  UpdateAttackCountDown()
        // -------------------------
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Plant.UpdateAttackCountDown))]
        public static void UpdateAttackCountDown_Postfix(Plant __instance)
        {
            if (__instance.TryGetComponent<FreezeGatlingPea>(out _) &&
                __instance.attributeCount >= 1)
            {
                __instance.thePlantAttackCountDown -= Time.deltaTime;
            }
        }
    }
}
