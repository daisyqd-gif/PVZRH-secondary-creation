global using BepInEx;
global using CustomizeLib.BepInEx;
//global using HarmonyLib;
global using UnityEngine;
global using System.Collections;
global using System.Collections.Generic;
global using CustomPlantClass;
global using Random = UnityEngine.Random;
global using CustomPlantClass.Main;

namespace SuperCherryThreeGatling
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Core : ModPlugin
    {
        public AssetBundle assetBundle;
        public override void InitializeMod()
        {
            assetBundle = CustomCore.GetAssetBundle(
                Tools.GetAssembly(),
                "supercherrythree"
            );
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = DataMgr.AllocateID(), // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("SuperThreeGatlingPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("SuperThreeGatlingPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorList(new List<(ID,ID)>() { (PlantType.SuperThreeGatling,PlantType.CherryBomb), (PlantType.SuperGatling,PlantType.CherryThreePeater), (PlantType.SuperCherryGatling,PlantType.ThreePeater)}), // Optional fusion recipes

                AttackInterval = 1.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 20,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 7.5f,               // Card cooldown
                Sun = 1025,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea_threeCherry, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = false, // Enable Star-Up ability if the plant has one

                CardColor = CardLevel.Purple, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = true, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "樱桃三线超级机枪射手",            // Plant name (shown in UI)
                AlmanacEntry = "向三行发射樱桃的超级机枪射手。\n" +
                "<color=#3D1400>融合配方：</color><color=red>三线超级机枪射手+樱桃炸弹</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>（20×6）×3/1.5秒</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①每次攻击有2%概率触发大招，5秒内，每0.02秒散射3发半熟樱桃，命中时立即爆炸，并为伤害范围内所有僵尸挂载1个半熟樱桃\n" +
                "<color=#3D1400>特点：</color><color=red>②子弹会挂载在目标身上，6秒后造成半径1格物理爆炸。每个目标身上最多挂载1个半熟樱桃\n" +
                "<color=#3D1400>特点：</color><color=red>③子弹挂载期间，目标僵尸受到的各类樱桃子弹和樱桃爆炸伤害的25%会累计到子弹伤害上\n" +
                "<color=#3D1400>词条1:</color><color=red>极速爆发：各种超级机枪射手和究极机枪射手释放大招的概率x3</color>\n" +
                "<color=#3D1400>词条2:</color><color=red>精准射击：各种超级机枪射手和究极机枪射手释放大招的子弹伤害x2，且不再散射</color>\n\n" +
                "<color=#3D1400>这三人尤其在豌豆小队训练营中展现了极高的敬业精神，他们总是齐声射击，并且以创纪录的速度解决彼此的争执。他们赢得了“第一三连突击队员”的称号，九颗星分别镶嵌在他们各自的头盔上。</color>"    // Almanac description (CN + EN recommended)
            };

            // Register the plant and retrieve its ID
            ID plantID = DataMgr.RegisterCustomPlant<SuperThreeGatling, SuperCherryThreeGatling>(Data);

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class SuperCherryThreeGatling : BaseCustomPlant
    {
        public override Transform FindShoot() => transform.FindChild("headPos2/ThreePeater_head2/ThreePeater_mouth/Shoot");
        public override int GetDamage()
        {
            if(Lawnf.TravelUltimate(UltiBuff.EnumValue51)) return _plant.attackDamage*2;
            return _plant.attackDamage;
        }
        public float GetOffset()
        {
            if(Lawnf.TravelUltimate(UltiBuff.EnumValue51)) return 0;
            return Random.Range(-0.25f,0.25f);
        }
        public override IEnumerator SuperShoot()
        {
            _plant.anim.SetBoolString("shooting",true);
            for (int i = 0; i < 250; i++)
            {
                Vector2 pos=_plant.shoot.position;
                // 3 bullets per burst
                if (_plant.thePlantRow == 0)
                {
                    var b1=CreateBullet.Instance.SetBullet(pos.x,pos.y+GetOffset()+0.15f,_plant.thePlantRow,BulletType.Bullet_pea_bombCherry,BulletMoveWay.MoveRight);
                    b1.Damage=GetDamage();
                    b1.fromType=_plant.thePlantType;
                    b1.normalSpeed=Random.Range(12f,14f);
                }
                else
                {
                    var b1=CreateBullet.Instance.SetBullet(pos.x,pos.y+GetOffset(),_plant.thePlantRow-1,BulletType.Bullet_pea_bombCherry,BulletMoveWay.MoveRight_threePeater);
                    b1.Damage=GetDamage();
                    b1.fromType=_plant.thePlantType;
                    b1.normalSpeed=Random.Range(12f,14f);
                }
                var b2=CreateBullet.Instance.SetBullet(pos.x,pos.y+GetOffset(),_plant.thePlantRow,BulletType.Bullet_pea_bombCherry,BulletMoveWay.MoveRight);
                b2.Damage=GetDamage();
                b2.fromType=_plant.thePlantType;
                b2.normalSpeed=Random.Range(12f,14f);
                if (_plant.thePlantRow == _plant.board.rowNum-1)
                {
                    var b3=CreateBullet.Instance.SetBullet(pos.x,pos.y+GetOffset()-0.15f,_plant.thePlantRow,BulletType.Bullet_pea_bombCherry,BulletMoveWay.MoveRight);
                    b3.Damage=GetDamage();
                    b3.fromType=_plant.thePlantType;
                    b3.normalSpeed=Random.Range(12f,14f);
                }
                else
                {
                    var b3=CreateBullet.Instance.SetBullet(pos.x,pos.y+GetOffset(),_plant.thePlantRow+1,BulletType.Bullet_pea_bombCherry,BulletMoveWay.MoveRight_threePeater);
                    b3.Damage=GetDamage();
                    b3.fromType=_plant.thePlantType;
                    b3.normalSpeed=Random.Range(12f,14f);
                }
                _plant.thePlantAttackCountDown=10f;

                yield return new WaitForSeconds(0.01f);
            }
            _plant.thePlantAttackCountDown=0.05f;
            _plant.anim.SetBoolString("shooting",false);
        }
        public override Bullet Shoot_Custom()
        {
            if(PlantMgr.GetPercent(2f) || (PlantMgr.GetPercent(6f) && Lawnf.TravelUltimate(UltiBuff.EnumValue50)))
            {
                this.StartCoroutine(SuperShoot());
                return null;
            }
            Vector2 pos=_plant.shoot.position;
            if (_plant.thePlantRow == 0)
            {
                var b1=CreateBullet.Instance.SetBullet(pos.x,pos.y+0.15f,_plant.thePlantRow,BulletType.Bullet_pea_threeCherry,BulletMoveWay.MoveRight);
                b1.Damage=GetDamage();
                b1.fromType=_plant.thePlantType;
            }
            else
            {
                var b1=CreateBullet.Instance.SetBullet(pos.x,pos.y,_plant.thePlantRow-1,BulletType.Bullet_pea_threeCherry,BulletMoveWay.MoveRight_threePeater);
                b1.Damage=GetDamage();
                b1.fromType=_plant.thePlantType;
            }
            var b2=CreateBullet.Instance.SetBullet(pos.x,pos.y,_plant.thePlantRow,BulletType.Bullet_pea_threeCherry,BulletMoveWay.MoveRight);
            b2.Damage=GetDamage();
            b2.fromType=_plant.thePlantType;
            if (_plant.thePlantRow == _plant.board.rowNum)
            {
                var b3=CreateBullet.Instance.SetBullet(pos.x,pos.y-0.15f,_plant.thePlantRow,BulletType.Bullet_pea_threeCherry,BulletMoveWay.MoveRight);
                b3.Damage=GetDamage();
                b3.fromType=_plant.thePlantType;
            }
            else
            {
                var b3=CreateBullet.Instance.SetBullet(pos.x,pos.y,_plant.thePlantRow+1,BulletType.Bullet_pea_threeCherry,BulletMoveWay.MoveRight_threePeater);
                b3.Damage=GetDamage();
                b3.fromType=_plant.thePlantType;
            }
            return b2;
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "SuperCherryThreeGatling.Bepinex";
        public const string PluginName = "SuperCherryThreeGatling";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
