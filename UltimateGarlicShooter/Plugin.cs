global using BepInEx;
global using HarmonyLib;
global using System;
global using System.Reflection;
global using UnityEngine;
global using CustomPlantClass;
global using CustomPlantClass.CustomizeLib;
global using CustomPlantClass.Main;
global using CustomPlantClass.Runtime.Core;
global using CustomPlantClass.Runtime.Tasks;
global using CustomPlantClass.CustomizeLib.ExtensionData.Basic;
global using CustomPlantClass.RogueShootingManager;
global using GameLevel.RogueShooting;
global using Object = UnityEngine.Object;
global using static UltimateGarlicShooter.Plugin.DataContainer;
namespace UltimateGarlicShooter
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public class DataContainer
        {
            public static ID Bullet1=-1;
            public static ID Bullet2=-1;
            public static ID Bullet_throw1=-1;
            public static ID Bullet_throw2=-1;
            public static ID Bullet3=-1;
            public static ID PlantId=-1;
            public static BuffID BulletBuff=-1;
            public static BuffID UnlockUpgradesBuff=-1;
            public static AdvBuff QBuff;
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "UltimateGarlicShooter.bundle"
            );
            PlantId = DataMgr.AllocateID();
            Bullet1 = DataMgr.AllocateID();
            Bullet2 = DataMgr.AllocateID();
            Bullet_throw1 = DataMgr.AllocateID();
            Bullet_throw2 = DataMgr.AllocateID();
            Bullet3 = DataMgr.AllocateID();
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData
            (
                PlantId, // Automatically assigns a unique ID

                assetBundle.GetAsset<GameObject>("UltimateGarlicShooterPrefab"),   // Main plant prefab, must copy an original prefab and delete the script component and then edit
                assetBundle.GetAsset<GameObject>("UltimateGarlicShooterPreview") // Card preview prefab, hirearchy must be this
                /*
                    root-> transform, spriterenderer
                    nothing else
                */
            )
            {

                Fusions = ListHelper.MirrorList([(PlantType.GarlicRepeater, PlantType.CherryThreePeater),(PlantType.DoubleCherry, PlantType.GarlicThreePeater)]), // Optional fusion recipes

                AttackInterval = 1f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 300,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 30f,               // Card cooldown
                Sun = 675,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, use GetBulletType to retrieve in the basecustomplant class

                CanPF = true,     // Enable PF ability if the plant has one: override the ienumerator for a pf with damage immunity or override StartPF for a instant pf
                CanStarUp = true, // Enable Star-Up ability if the plant has one, retrieve using _plant.starUp

                CardColor = CardLevel.Gold, // Determines card rarity and UI color
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

                Name = "究极病毒双发射手",           // Plant name (shown in UI)
                AlmanacEntry = "临时描述无法反映当前属性"+DataMgr.CreateAlmanacEntry("“病毒传染，生生不息”", null, ("樱桃三线射手", "蒜毒双发射手"), ("300x5x2", 1f), default, [
                    "每1秒向前方3行各发射2枚子弹，并投掷2枚子弹；",
                    "子弹击中后分裂10枚伤害为主子弹10%的子弹和1枚同主子弹伤害的半熟樱桃子弹，并留下毒瘴；",
                    "究极病毒双发射手在场时，所有毒瘴获得以下效果:\n"+
                    "·同时拥有病毒狙击手毒瘴和普通毒瘴的特性，并且每0.02秒释放1次伤害为300的小樱桃爆炸；\n"+
                    "·毒瘴伤害和存在时间×3；</color>\n",
                    "究极病毒双发射手初始等级为1级，每存在15秒进行1次升级，叠种豌豆射手则立即进行1次升级，等级最高为3级；"
                ],
                usageconditions:"旅行模式购买配方",
                flavor:"作为一个被所有植物避之唯恐不及的“零号传染源”，究极病毒双发射手总是自卑地缩在草坪的最角落，生怕自己的存在污染了这片净土。看着同伴们惊恐后退的身影，他难过地低下了头，小声嗫嚅道：“对不起……我只是想和大家在一起，没想到却成了挥之不去的瘟疫……”。但出乎意料的是，僵尸们还挺喜欢这种味道的……仅限小范围内")
                // Almanac description, use DataMgr.CreateAlmanacEntry for automatic formatting
            };

            DataMgr.RegisterCustomPlant<Shooter, UltimateGarlicShooter>(Data);

            CustomCore.RegisterCustomBullet<Bullet_pea,Bullet_garlicCherryPea>
            (
                Bullet1,
                assetBundle.GetAsset<GameObject>("Bullet_garlicCherryPea")
            );

            CustomCore.RegisterCustomBullet<Bullet_pea,Bullet_garlicCherryPea_split>
            (
                Bullet2,
                assetBundle.GetAsset<GameObject>("Bullet_garlicCherryPea2")
            );

            CustomCore.RegisterCustomBullet<Bullet_cabbage,Bullet_garlicCherryPea_throw>
            (
                Bullet_throw1,
                assetBundle.GetAsset<GameObject>("Bullet_garlicCherryPea_throw")
            );

            CustomCore.RegisterCustomBullet<Bullet_cabbage,Bullet_garlicCherryPea_throw2>
            (
                Bullet_throw2,
                assetBundle.GetAsset<GameObject>("Bullet_garlicCherryPea_throw2")
            );

            CustomCore.RegisterCustomBullet<Bullet_cabbage,Bullet_garlicCherryPea_split_2>
            (
                Bullet3,
                assetBundle.GetAsset<GameObject>("Bullet_garlicCherryPea2_throw")
            );

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
        public override void InitializeBuffs()
        {
            UnlockUpgradesBuff = CustomCore.RegisterCustomBuff
            (
                DataMgr.FormatPlantBuff(
                    "平步青云",
                    ("究极病毒双发射手","解锁升级能力")
                ),BuffType.UltimateBuff,()=>true,5000,PlantId
            );

            BulletBuff = CustomCore.RegisterCustomBuff(
                DataMgr.FormatPlantBuff(
                    "薪火相传",
                    ("究极病毒双发射手","母子弹可以分裂为2个同母子弹效果的子一代子弹，子一代可以分裂为2个子二代子弹，子二代子弹击中后释放1次樱桃爆炸")
                ),BuffType.UltimateBuff,()=>true,5000,PlantId
            );

            DataMgr.AddCustomStrongUltimatePlant(PlantId,DataMgr.FormatStrongUltimateUnlockBuff("究极病毒双发射手", "樱桃三线射手", "蒜毒双发射手"),UnlockUpgradesBuff,BulletBuff);

            BaseBuff uBuff1 = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = PlantId,
                CustomTitle = "强化：远程打击",
                CustomDescription = "锁定目标+1\n究极病毒双发射手将挑选僵尸并在其后方射出子弹",
                CustomBuffType = ShootingBuffType.UniqueUpgrade,
                CustomOnGet = () =>
                {
                    if (ShootingManager.Instance.TryGetPlant(PlantId, out var p))
                    {
                        p.shootingLevel ++;
                    }
                }
            });
            
            BaseBuff qBuff = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = PlantId,
                CustomTitle = "质变：薪火相传",
                CustomDescription = "获得词条：薪火相传",
                CustomBuffType = ShootingBuffType.QualitativeChange,
                CustomOnGet = () =>
                {
                    BulletBuff.ActivateUlti();
                }
            });
            BaseConfig cfg = RegistryHelper.MakeConfigType(new()
            {
                CustomPlantType = PlantId,
                CustomBuffs = () => [new DamageBuff(PlantId), new SpeedBuff(PlantId), uBuff1, qBuff],
                CustomReinforcePlant = p =>
                {
                    p.ModifyDamage(PlantDamageAdder.Shooting,5f,false,new());
                    if (!UnlockUpgradesBuff.IsActiveUlti())
                    {
                        UnlockUpgradesBuff.ActivateUlti();
                    }
                },
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            BaseConfig cfg_base = RegistryHelper.MakeConfigType(new()
            {
                CustomPlantType = PlantType.GarlicRepeater,
                CustomBuffs = () => [new UpgradeBuff(PlantType.GarlicRepeater,PlantId)],
                CustomReinforcePlant = p =>
                {
                    p.AddSpeed(PlantSpeedAdder.Shooting,5f, new());
                    p.ModifyDamage(PlantDamageAdder.Shooting,14f,false,new());
                },
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            RegistryHelper.AddCustomRogueShootingPlant(PlantType.GarlicRepeater,cfg_base);
            RegistryHelper.AddCustomRogueShootingPlant(PlantId,cfg);
            RegistryHelper.InjectUpgradeBuff(RSConfigType.Peashooter,PlantType.GarlicRepeater);
            RegistryHelper.AddCustomEvolutionPathway(PlantType.Peashooter,PlantType.Peashooter,PlantType.GarlicRepeater,PlantId);
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateGarlicShooter.Bepinex";
        public const string PluginName = "UltimateGarlicShooter";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
