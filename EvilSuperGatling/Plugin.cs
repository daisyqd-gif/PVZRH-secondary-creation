global using BepInEx;
global using CustomizeLib.BepInEx;
global using CustomPlantClass;
global using CustomPlantClass.Main;
global using CustomPlantClass.Examples;
global using CustomPlantClass.Runtime.Tasks;
global using System;
global using System.Reflection;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using UnityEngine;
global using Unity.VisualScripting;
global using Random = UnityEngine.Random;
namespace EvilSuperGatling
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public static class DataContainer
        {
            public static ID PlantId=-1;
            public static ID BulletId=-1;
            public static ID BulletId_Fire=-1;
            public static ID ParticleId=-1;
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "evilsupergatling"
            );
            DataContainer.PlantId = DataMgr.AllocateID();
            DataContainer.BulletId = DataMgr.AllocateID();
            DataContainer.BulletId_Fire = DataMgr.AllocateID();
            DataContainer.ParticleId = DataMgr.AllocateID();
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = DataContainer.PlantId, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("EvilSuperGatlingPrefab"),   // Main plant prefab, must copy an original prefab and delete the script component and then edit
                Preview = assetBundle.GetAsset<GameObject>("EvilSuperGatlingPreview"), // Card preview prefab, hirearchy must be this
                /*
                    root-> transform, spriterenderer
                    nothing else
                */

                Fusions = new List<(ID, ID)>(), // Optional fusion recipes

                AttackInterval = 1f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 1000,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 5f,               // Card cooldown
                Sun = 2000,               // Sun cost

                DefaultBullet = DataContainer.BulletId, // Shooter bullet type, use GetBulletType to retrieve in the basecustomplant class

                CanPF = true,     // Enable PF ability if the plant has one: override the ienumerator for a pf with damage immunity or override StartPF for a instant pf
                CanStarUp = true, // Enable Star-Up ability if the plant has one, retrieve using _plant.starUp

                CardColor = CardLevel.Red, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong / Final ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = true, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "究极魔化机枪射手",           // Plant name (shown in UI)
                AlmanacEntry = DataMgr.CreateAlmanacEntry("一轮发射六次豌豆，有概率一次发射大量随机豌豆。",usageconditions:"神秘模式",attackinterval:("1000×6",1),specialeffects : ["普通攻击时，每发子弹有2%概率触发大招：回复1倍韧性血量，5秒内无敌，每0.02秒散射3发伤害为1000的子弹"])    // Almanac description, use DataMgr.CreateAlmanacEntry for automatic formatting
            };

            DataMgr.RegisterCustomPlant<Shooter, EvilSuperGatling>(Data);
            CustomCore.RegisterCustomBullet<Bullet_sword,EvilPea>(DataContainer.BulletId,assetBundle.GetAsset<GameObject>("Bullet_pea_evil"));
            CustomCore.RegisterCustomBullet<Bullet_sword,EvilPeaFire>(DataContainer.BulletId_Fire,assetBundle.GetAsset<GameObject>("Bullet_pea_fireEvil"));
            UltimateTorchBehaviour.AddBulletToPool(DataContainer.BulletId,DataContainer.BulletId_Fire);
            CustomCore.RegisterCustomParticle(DataContainer.ParticleId,assetBundle.GetAsset<GameObject>("EvilPeaSplat"));

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class EvilSuperGatling : BaseCustomPlant
    {
        public bool entered = false;
        protected override bool IsAsyncPF => true;
        public override int AttackDamage => Lawnf.TravelUltimate(UltiBuff.EnumValue51) && isPF ? _plant.attackDamage * 2 : _plant.attackDamage;
        public override Transform FindShoot() => transform.FindChild("GatlingPea_head/GatlingPea_mouth_overlay");
        public override BulletType GetBulletType() => Plugin.DataContainer.BulletId;
        public override void Awake()
        {
            base.Awake();
            _plant.invincible = true;
            _prevUncrashable = _plant.uncrashable;
            _plant.uncrashable = true;
            isPF = true;
            _plant.thePlantAttackCountDown = 99999999;
        }
        public void Entered()
        {
            try
            {
                entered = _plant != null && !_plant.IsDestroyed();
                if(! entered) return;
                _plant.uncrashable = _prevUncrashable;
                StartPF();
            }
            catch
            {
                entered = false;
            }
        }
        public override Bullet Shoot_Custom()
        {
            if (PlantMgr.GetPercent(2f) || (Lawnf.TravelUltimate(UltiBuff.EnumValue50) && PlantMgr.GetPercent(6f)))
            {
                StartPF();
            }
            return PlantMgr.SetBullet(_plant, GetBulletType(), BulletMoveWay.MoveRight);
        }
        protected override async Task SuperShoot_Async()
        {
            try
            {
                _plant.anim.SetBoolString("shooting", true);
                for (int i = 0; i < 250; i++)
                {
                    for (int j = 0; j < 5; j++)
                        PlantMgr.SetBullet
                        (
                            _plant,
                            GetBulletType(),
                            GetBulletMoveWayPF_SuperGatling(),
                            AttackDamage,
                            new Vector2(0, Random.Range(-0.15f, 0.15f)), Random.Range(-15f, 15f)
                        ).normalSpeed = Random.Range(12f, 14f);
                    _plant.thePlantAttackCountDown = 10f;
                    await DelayTask.DelayScaled(0.02f,()=>_plant.attributeSpeed,token);
                }
                _plant.thePlantAttackCountDown = 0.05f;
                _plant.anim.SetBoolString("shooting", false);
            }
            catch( Exception e)
            {
                ModLogger.LogError(e.ToString());
            }
        }
        public override void SuperEnd()
        {
            if (_plant.starUp) StartPF();
            else base.SuperEnd();
        }
    }

    public class EvilPea : BaseCustomBullet
    {
        public override bool HitZombie(Zombie zombie)
        {
            zombie.TakeDamage(_bullet.Damage,_bullet.Cast<IDamageMaker>(),DamageType.RealDamage,_bullet.fromType);
            zombie.AddEffect<EvilEffect>();
            CreateParticle.SetParticle(Plugin.DataContainer.ParticleId,transform.position,_bullet.theBulletRow);
            GameAPP.PlaySound(Random.Range(0,2));
            return false;
        }
    }
    public class EvilPeaFire : BaseCustomBullet
    {
        public override bool HitZombie(Zombie zombie)
        {
            zombie.TakeDamage(_bullet.Damage,_bullet.Cast<IDamageMaker>(),DamageType.RealDamage,_bullet.fromType);
            zombie.AddEffect<EvilEffect>();
            ParticleManager.Instance.SetParticle(ParticleType.Fire,transform.position,_bullet.theBulletRow);
            GameAPP.PlaySound(SoundType.FirePea);
            return false;
        }
    }

    public class EvilEffect : CustomEffect
    {
        public override bool CanCountDown => true;
        public override bool TimedEffect => true;
        public override float CycleCountDown { get; set; } = 1f;
        public override float RemoveCountDown { get; set; } = 15f;
        public override CustomEffectUsage Usage { get => CustomEffectUsage.Zombie; }
        Zombie z => GetComponent<Zombie>();
        public override void OnTimerZero()
        {
            if (z.theMaxHealth / 10 - 100000 >= int.MaxValue)
            {
                z.TakeDamage(2147383647, null, DamageType.RealDamage, (PlantType)Plugin.DataContainer.PlantId);
                return;
            }
            z.TakeDamage((int)z.theMaxHealth / 10, null, DamageType.RealDamage, (PlantType)Plugin.DataContainer.PlantId);
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "EvilSuperGatling.Bepinex";
        public const string PluginName = "EvilSuperGatling";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
