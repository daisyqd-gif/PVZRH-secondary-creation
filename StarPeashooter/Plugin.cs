global using BepInEx;
global using CustomizeLib.BepInEx;
global using System.Reflection;
global using UnityEngine;
global using CustomPlantClass;
global using CustomPlantClass.Main;
global using CustomPlantClass.Examples;
namespace StarPeashooter
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public class DataContainer
        {
            public static ID PlantId_Double=-1;
            public static ID PlantId_Split=-1;
            public static ID PlantId_Gatling=-1;
            public static ID PlantId_Super=-1;
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "starshooter"
            );
            DataContainer.PlantId_Double=DataMgr.AllocateID();
            DataContainer.PlantId_Split=DataMgr.AllocateID();
            DataContainer.PlantId_Gatling=DataMgr.AllocateID();
            DataContainer.PlantId_Super=DataMgr.AllocateID();
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data_Double = new BaseCustomPlantData()
            {
                PlantId = DataContainer.PlantId_Double, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("StarDoubleShooterPrefab"),   // Main plant prefab, must copy an original prefab and delete the script component and then edit
                Preview = assetBundle.GetAsset<GameObject>("StarDoubleShooterPreview"), // Card preview prefab, hirearchy must be this
                /*
                    root-> transform, spriterenderer
                    nothing else
                */

                Fusions = DataMgr.MirrorList
                (
                    new([(PlantType.StarPea,PlantType.Peashooter),(PlantType.DoubleShooter,PlantType.StarFruit)])
                ), // Optional fusion recipes

                AttackInterval = 1.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 20,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 325,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea_star, // Shooter bullet type, use GetBulletType to retrieve in the basecustomplant class

                CanPF = false,     // Enable PF ability if the plant has one: override the ienumerator for a pf with damage immunity or override StartPF for a instant pf
                CanStarUp = false, // Enable Star-Up ability if the plant has one, retrieve using _plant.starUp

                CardColor = CardLevel.Green, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong / Final ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = false, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "杨桃双发射手",           // Plant name (shown in UI)
                AlmanacEntry = "xxx"    // Almanac description, use DataMgr.CreateAlmanacEntry for automatic formatting
            };
            // Fill out the plant metadata
            BaseCustomPlantData Data_Split = new BaseCustomPlantData()
            {
                PlantId = DataContainer.PlantId_Split, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("StarSplitPeaPrefab"),   // Main plant prefab, must copy an original prefab and delete the script component and then edit
                Preview = assetBundle.GetAsset<GameObject>("StarSplitPeaPreview"), // Card preview prefab, hirearchy must be this
                /*
                    root-> transform, spriterenderer
                    nothing else
                */

                Fusions = DataMgr.MirrorList
                (
                    new([(PlantType.StarPea,PlantType.DoubleShooter),(PlantType.SplitPea,PlantType.StarFruit),
                    (DataContainer.PlantId_Double,PlantType.Peashooter)])
                ), // Optional fusion recipes

                AttackInterval = 1.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 20,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 425,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea_star, // Shooter bullet type, use GetBulletType to retrieve in the basecustomplant class

                CanPF = false,     // Enable PF ability if the plant has one: override the ienumerator for a pf with damage immunity or override StartPF for a instant pf
                CanStarUp = false, // Enable Star-Up ability if the plant has one, retrieve using _plant.starUp

                CardColor = CardLevel.Green, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong / Final ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = false, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "杨桃裂荚射手",           // Plant name (shown in UI)
                AlmanacEntry = "xxx"    // Almanac description, use DataMgr.CreateAlmanacEntry for automatic formatting
            };
            // Fill out the plant metadata
            BaseCustomPlantData Data_Gatling = new BaseCustomPlantData()
            {
                PlantId = DataContainer.PlantId_Gatling, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("StarGatlingPeaPrefab"),   // Main plant prefab, must copy an original prefab and delete the script component and then edit
                Preview = assetBundle.GetAsset<GameObject>("StarGatlingPeaPreview"), // Card preview prefab, hirearchy must be this
                /*
                    root-> transform, spriterenderer
                    nothing else
                */

                Fusions = DataMgr.MirrorList
                (
                    new([(PlantType.StarPea,PlantType.SplitPea),(PlantType.GatlingPea,PlantType.StarFruit),
                    (DataContainer.PlantId_Double,PlantType.DoubleShooter),(DataContainer.PlantId_Split,PlantType.Peashooter)])
                ), // Optional fusion recipes

                AttackInterval = 1.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 20,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 525,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea_star, // Shooter bullet type, use GetBulletType to retrieve in the basecustomplant class

                CanPF = false,     // Enable PF ability if the plant has one: override the ienumerator for a pf with damage immunity or override StartPF for a instant pf
                CanStarUp = false, // Enable Star-Up ability if the plant has one, retrieve using _plant.starUp

                CardColor = CardLevel.Green, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong / Final ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = false, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "杨桃机枪射手",           // Plant name (shown in UI)
                AlmanacEntry = "xxx"    // Almanac description, use DataMgr.CreateAlmanacEntry for automatic formatting
            };
            // Fill out the plant metadata
            BaseCustomPlantData Data_Super = new BaseCustomPlantData()
            {
                PlantId = DataContainer.PlantId_Super, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("StarSuperGatlingPrefab"),   // Main plant prefab, must copy an original prefab and delete the script component and then edit
                Preview = assetBundle.GetAsset<GameObject>("StarSuperGatlingPreview"), // Card preview prefab, hirearchy must be this
                /*
                    root-> transform, spriterenderer
                    nothing else
                */

                Fusions = DataMgr.MirrorList
                (
                    new([(PlantType.SniperPea,PlantType.StarPea),(PlantType.SuperGatling,PlantType.StarFruit),
                    (PlantType.StarSniper,PlantType.Peashooter)])
                ), // Optional fusion recipes

                AttackInterval = 1.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 20,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 725,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea_star, // Shooter bullet type, use GetBulletType to retrieve in the basecustomplant class

                CanPF = true,     // Enable PF ability if the plant has one: override the ienumerator for a pf with damage immunity or override StartPF for a instant pf
                CanStarUp = false, // Enable Star-Up ability if the plant has one, retrieve using _plant.starUp

                CardColor = CardLevel.Purple, // Determines card rarity and UI color
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

                Name = "超级杨桃机枪射手",           // Plant name (shown in UI)
                AlmanacEntry = "xxx"    // Almanac description, use DataMgr.CreateAlmanacEntry for automatic formatting
            };

            DataMgr.RegisterCustomPlant<Shooter, StarPeashooter_Base>(Data_Double);
            DataMgr.RegisterCustomPlant<Shooter, SplitStarPea>(Data_Split);
            DataMgr.RegisterCustomPlant<Shooter, GatlingStarPea>(Data_Gatling);
            DataMgr.RegisterCustomPlant<Shooter, SuperStarGatling>(Data_Super);
            CustomCore.AddFusion((int)PlantType.StarSniper,DataContainer.PlantId_Super,(int)PlantType.Peashooter);
            CustomCore.AddFusion((int)PlantType.StarSniper,(int)PlantType.Peashooter,DataContainer.PlantId_Super);

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
        public override void OnGameInit()
        {
            foreach (var pair in CustomCore.CustomPlantNames)
            {
                Logger.LogMessage(pair.Value);
                if (pair.Value == $"初版杨桃超级机枪射手")
                {
                    MixData.AddRecipe(DataContainer.PlantId_Double,PlantType.SplitPea,pair.Key);
                    MixData.AddRecipe(PlantType.SplitPea,DataContainer.PlantId_Double,pair.Key);
                    MixData.AddRecipe(DataContainer.PlantId_Split,PlantType.DoubleShooter,pair.Key);
                    MixData.AddRecipe(PlantType.DoubleShooter,DataContainer.PlantId_Split,pair.Key);
                    MixData.AddRecipe(DataContainer.PlantId_Gatling,PlantType.Peashooter,pair.Key);
                    MixData.AddRecipe(PlantType.Peashooter,DataContainer.PlantId_Gatling,pair.Key);
                }
            }
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class StarPeashooter_Base : BaseCustomPlant
    {
        public override Transform FindShoot() => transform.GetChild(0);
        public override BulletType GetBulletType()
        {
            return BulletType.Bullet_pea_star;
        }
    }
    public class GatlingStarPea : StarPeashooter_Base
    {
        public override Transform FindShoot() => transform.GetChild(0);
    }
    public class SplitStarPea : StarPeashooter_Base
    {
        public override void OnSpawn()
        {
            _plant.shoot2=transform.FindChild("PeaShooter_Head/Shoot2");
        }
        public override Bullet AnimShoot_Custom()
        {
            Shoot2_Custom();
            return base.AnimShoot_Custom();
        }
        public override Bullet Shoot2_Custom()
        {
            return PlantMgr.SetBullet(_plant,_plant.shoot2.position,GetBulletType(),BulletMoveWay.Split_left);
        }
    }
    public class SuperStarGatling : SuperHypnoGatling_Example
    {
        public override string GetShootPath() => "GatlingPea_head/Shoot";
        public override BulletType GetBulletType()
        {
            return BulletType.Bullet_pea_star;
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "StarPeashooter.Bepinex";
        public const string PluginName = "StarPeashooter";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
