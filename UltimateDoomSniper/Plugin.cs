global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using System;
global using System.Reflection;
global using UnityEngine;
global using CustomPlantClass;
global using Unity.VisualScripting;
global using Random = UnityEngine.Random;
global using CustomPlantClass.Main;
using Core;
[assembly: CustomMod("UltimateDoomSniper")]
namespace UltimateDoomSniper
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public static ID PlantID = -1;
        public static ID PlantID_Sniper1 = -1;
        public static ID PlantID_Sniper2 = -1;
        public static ID ParticleID = -1;
        public static BuffID Buff1 = -1;
        public static BuffID Buff2 = -1;
        public override void InitializeMod()
        {
            //AttributeMgr.LoadAllAttributes(Assembly.GetExecutingAssembly());
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ultimatedoomsniper");
            PlantID = DataMgr.AllocateID();
            PlantID_Sniper1 = DataMgr.AllocateID();
            PlantID_Sniper2 = DataMgr.AllocateID();
            ParticleID = DataMgr.AllocateID();
            CustomCore.RegisterCustomParticle(ParticleID, assetBundle.GetAsset<GameObject>("ShootFire"));
        }
        public override void InitializeBuffs()
        {
            Buff1 = CustomCore.RegisterCustomBuff("防空炮：解锁死神狙击射手的锁空锁空并将解锁毁灭狙击射手, 毁灭狙击射手的攻击间隔缩短至4秒", BuffType.AdvancedBuff, () => true, 5000, PlantType.DoomSniper, 1, BuffBgType.Day);
            Buff2 = CustomCore.RegisterCustomBuff("死神之力：死神狙击射手狂热时间*2, 狂热能量需求/2, 大招需求/2, 毁灭狙击射手只会爆头", BuffType.AdvancedBuff, () => true, 5000, PlantType.DoomSniper, 1, BuffBgType.Day);
        }
        public override void InitializePlants()
        {

            // Fill out the plant metadata
            BaseCustomPlantData Data2 = new BaseCustomPlantData()
            {
                PlantId = PlantID_Sniper1, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("ExplodeDoomSniperPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("ExplodeDoomSniperPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorList(new()
                {
                    (PlantType.DoomSniper,PlantType.Peashooter)
                }), // Optional fusion recipes

                AttackInterval = 3f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 1800,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 725,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = false,     // Enable PF ability if the plant has one
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
                IsUltimatePlant = false, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "毁灭狙击射手",           // Plant name (shown in UI)
                AlmanacEntry = DataMgr.CreateAlmanacEntry("全自动毁灭菇投送器，如果遇到炸不死的敌人就用枪托砸死它！",
                recipe: ("狙击射手", "毁灭菇+豌豆射手"),
                attackinterval: (1800, 3f),
                specialeffects: new string[]
                {
                    "每3秒对目标释放一次毁灭菇效果，每两次攻击会对索敌主目标爆头一次。词条：枕戈待旦：攻速变为四秒 核能威慑：每次攻击会对主目标爆头"
                },
                usageconditions: "旅行模式")    // Almanac description (CN + EN recommended)
            };
            DataMgr.RegisterCustomPlant<FireSniper,DestroySniper>(Data2);
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = PlantID, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("UltimateDoomSniperPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("UltimateDoomSniperPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorList(new()
                {
                    (PlantType.DoomSniper,PlantType.DoomShroom),
                    (PlantID_Sniper1,PlantType.DoomPeashooter),
                    (PlantID_Sniper2,PlantType.Peashooter)
                }), // Optional fusion recipes

                AttackInterval = 0.3f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 100,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 800,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = false,     // Enable PF ability if the plant has one
                CanStarUp = false, // Enable Star-Up ability if the plant has one

                CardColor = CardLevel.Gold, // Determines card rarity and UI color
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

                Name = "究极死神猎手",           // Plant name (shown in UI)
                AlmanacEntry = DataMgr.CreateAlmanacEntry("强劲贯穿和狂热射速如死神呼啸，蓄能后更可造成大范围杀伤。",
                recipe: ("狙击射手", "毁灭菇*2"),
                attackinterval: (100, 0.3f),
                specialeffects: new string[]
                {
                    "攻击无限穿透且无视僵尸身位",
                    "每命中1个僵尸，获得1点大招充能和1点狂热能量",
                    "消耗300点大招充能，在命中的首个僵尸的位置释放毁灭菇效果，伤害为(72×自身攻击力)",
                    "消耗100点狂热能量，在8秒内攻速+100%",
                    "有10%概率击中弱点，造成6倍伤害。必定击中余烬状态的僵尸的弱点"
                },
                usageconditions: "旅行模式", flavor: "但是我们的死神狙大人还是不能锁空(除非他有词条)")    // Almanac description (CN + EN recommended)
            };

            // Register the plant and retrieve its ID
            CustomCore.RegisterCustomBanMix(DataMgr.RegisterCustomPlant<DoomSniper, UltimateDoomSniper>(Data), () => (Lawnf.TravelAdvanced(Buff1) && Lawnf.TravelAdvanced(Buff2)) || Utils.EnableTravelPlant(), null, () => InGameText.Instance.ShowText("该配方需要抽取", 3f));
            DataMgr.AddGameStartAction(() =>
            {
                GameAPP.resourcesManager.plantPrefabs[PlantType.DoomSniper].AddComponent<DoomSniperComponent>();
            });
            // Fill out the plant metadata
            BaseCustomPlantData Data3 = new BaseCustomPlantData()
            {
                PlantId = PlantID_Sniper2, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("UltimateExplodeDoomSniperPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("UltimateExplodeDoomSniperPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorList(new()
                {
                    (PlantID_Sniper1,PlantType.DoomShroom),
                    (PlantType.DoomSniper,PlantType.DoomPeashooter),
                    (PlantID,PlantType.Peashooter)
                }), // Optional fusion recipes

                AttackInterval = 3f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 1800,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 725,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = false,     // Enable PF ability if the plant has one
                CanStarUp = false, // Enable Star-Up ability if the plant has one

                CardColor = CardLevel.Gold, // Determines card rarity and UI color
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

                Name = "究极毁灭狙击射手",           // Plant name (shown in UI)
                AlmanacEntry = DataMgr.CreateAlmanacEntry("全自动毁灭菇投送器，如果遇到炸不死的敌人就用枪托砸死它！",
                recipe: ("狙击射手", "毁灭菇*2+豌豆射手"),
                attackinterval: (1800, 3f),
                specialeffects: new string[]
                {
                    "每3秒对目标释放一次毁灭菇效果，每两次攻击会对索敌主目标爆头一次。词条：枕戈待旦：攻速变为四秒 核能威慑：每次攻击会对主目标爆头"
                },
                usageconditions: "旅行模式")    // Almanac description (CN + EN recommended)
            };
            CustomCore.RegisterCustomBanMix(DataMgr.RegisterCustomPlant<SniperPea, UltimateDestroySniper>(Data3), () => (Lawnf.TravelAdvanced(Buff1) && Lawnf.TravelAdvanced(Buff2)) || Utils.EnableTravelPlant(), null, () => InGameText.Instance.ShowText("该配方需要抽取", 3f));

            DataMgr.AddCustomWeakUltimatePlant(PlantType.DoomSniper,true,PlantID_Sniper1,() => Lawnf.TravelAdvanced(Buff1) || Utils.EnableTravelPlant());
            DataMgr.AddCustomWeakUltimatePlant(PlantID,false);
            DataMgr.AddCustomWeakUltimatePlant(PlantID_Sniper2,true);
            
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateDoomSniper.Bepinex";
        public const string PluginName = "UltimateDoomSniper";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
