global using BepInEx;
global using CustomizeLib.BepInEx;
global using System.Reflection;
global using UnityEngine;
global using System.Collections;
global using System.Collections.Generic;
global using CustomPlantClass;
global using Unity.VisualScripting;
global using Random = UnityEngine.Random;
global using CustomPlantClass.Main;
//global using Object = UnityEngine.Object;

namespace UltimatePortalGatling
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Core : ModPlugin
    {
        private AssetBundle assetBundle;
        public static ID PortalPlantType=DataMgr.AllocateID();
        public static ID BulletType_Portal=DataMgr.AllocateID();
        /*reserved
        public static ID HelmetPlantType=DataMgr.AllocateID();
        public static ID CherryPlantType=DataMgr.AllocateID();
        public static ID SniperPlantType=DataMgr.AllocateID();
        public static ID BulletType_Cherry=DataMgr.AllocateID();
        public static ID BulletType_CherrySquash=DataMgr.AllocateID();
        public static ID BulletType_Nuclear=DataMgr.AllocateID();
        public static ID ParticleType_Doom=DataMgr.AllocateID();
        public static ID ParticleType_Bomb=DataMgr.AllocateID();
        public static ID ParticleType_SmallBomb=DataMgr.AllocateID();
        */
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "portalsupergatling"
            );
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData PortalData = new BaseCustomPlantData()
            {
                PlantId = PortalPlantType, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("PortalSuperGatlingPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("PortalSuperGatlingPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorList(new List<(ID, ID)>(){(PlantType.UltimatePortalSniper,PlantType.Peashooter),(PlantType.UltimateHelmetGatling,PlantType.PortalPea)}), // Optional fusion recipes

                AttackInterval = 1f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 300,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 800,               // Sun cost

                DefaultBullet = BulletType.Bullet_portalPea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

                CardColor = CardLevel.Gold, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong ultimate plants or Final ultimate plants
                    Red    = Harvest/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = false, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "终极超时空机枪射手",           // Plant name (shown in UI)  
                AlmanacEntry =
                    "发射终极时空豌豆的超级机枪射手。\n\n" +
                    "<color=#3D1400>使用条件：</color><color=red>旅行模式</color>\n" +
                    "<color=#3D1400>伤害：</color><color=red>300×6/1.5秒</color>\n" +
                    "<color=#3D1400>特点：</color><color=red>" +
                    "①每次攻击充能1点，达到12点后发射终极时空豌豆\n" +
                    "②攻击时对最近的僵尸施加10秒的时空传送效果\n" +
                    "③大招期间5秒内每0.01秒散射1发（共 250 发）\n" +
                    "</color>\n" +
                    "<color=#3D1400>融合配方：</color><color=red>究极超时空狙击射手 + 豌豆射手</color>"
                    // Almanac description (CN + EN recommended)
            };

            // Register the plant and retrieve its ID
            ID plantID1 = DataMgr.RegisterCustomPlant<Shooter, SuperPortalGatling>(PortalData);
            CustomCore.TypeMgrExtra.IsMagnetPlants.Add(plantID1);

            CustomCore.RegisterCustomBullet<Bullet_pea,UltimatePortalPea>(BulletType_Portal,assetBundle.GetAsset<GameObject>("Bullet_portalPea"));

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class SuperPortalGatling : BaseCustomPlant
    {
        public int counter = 0;
        public int counter2 = 0;
        public override Transform FindShoot() => transform.FindChild("GatlingPea_head/GatlingPea_mouth_overlay");
        public override void OnSpawn()
        {
            _plant.anim.enabled=true;
        }
        public override string GetTextString() => $"充能 : {counter}";
        public override BulletType GetBulletType()
        {
            if (counter >= 12 || (Lawnf.TravelUltimate(UltiBuff.EnumValue50) && counter >= 6))
            {
                counter = 0;
                return Core.BulletType_Portal;
            }
            else
            {
                counter++;
                return BulletType.Bullet_portalPea;
            }
        }
        public void ApplyPortalAura()
        {
            GameObject go = _plant.SearchZombie();
            if (go == null)
                return;

            if (!go.TryGetComponent<Zombie>(out var z))
                return;

            if (z.IsDestroyed() || z.beforeDying)
                return;

            z.SetPortaled(10f);
        }
        public override int GetDamage()
        {
            if(Lawnf.TravelUltimate(UltiBuff.EnumValue51)) return _plant.attackDamage*2;
            return _plant.attackDamage;
        }
        public override Bullet Shoot_Custom()
        {
            ApplyPortalAura();
            if (PlantMgr.GetPercent(2f) || (Lawnf.TravelUltimate(UltiBuff.EnumValue50) && PlantMgr.GetPercent(6f)))
            {
                StartPF();
            }
            return PlantMgr.SetBullet(_plant,GetBulletType(),BulletMoveWay.MoveRight);
        }
        public override IEnumerator SuperShoot()
        {
            _plant.anim.SetBoolString("shooting",true);
            for (int i = 0; i < 50; i++)
            {
                counter2++;
                if(counter2>=10) ApplyPortalAura();
                for(int j = 0; j < 5 ; j++)
                {
                    PlantMgr.SetBullet(_plant,GetBulletType(),GetBulletMoveWayPF_SuperGatling(),GetDamage(),new Vector2(0,Random.Range(-0.1f,0.1f)),Random.Range(-15f,15f)).normalSpeed=Random.Range(12f,14f);
                }
                _plant.thePlantAttackCountDown=10f;
                yield return new WaitForSeconds(0.02f);
            }
            _plant.thePlantAttackCountDown=0.05f;
            _plant.anim.SetBoolString("shooting",false);
        }
        public override void SuperEnd()
        {
            if(_plant.starUp) StartPF();
            else base.SuperEnd();
        }
    }
    public class UltimatePortalPea : BaseCustomBullet
    {
        public override bool HitZombie(Zombie zombie)
        {
            if(zombie==null) return true;
            zombie.SetPortaled(1.5f);
            if(!zombie.TryGetEffect<PortalEffect>(EffectType.Portal,out var _)) //deals 1 million damage to immune zombies
            {
                zombie.TakeDamage(DmgType.MaxDamage,1000000,_bullet.fromType);
            }
            else
            {
                zombie.TakeDamage(DmgType.Normal,_bullet.Damage,_bullet.fromType);
            }
            InstanceManager.ParticleManager.SetParticle(ParticleType.ProtalPeaSplat,_bullet.col.bounds.center);
            _bullet.PlaySound(zombie);
            _bullet.Die();
            return false;
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "SuperPortalGatling.Bepinex";
        public const string PluginName = "SuperPortalGatling";
        public const string PluginVersion = "3.6.1";
    }
}
