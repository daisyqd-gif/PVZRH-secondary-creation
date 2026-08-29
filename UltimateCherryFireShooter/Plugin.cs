global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using System.Reflection;
global using UnityEngine;
global using System.Collections;
global using CustomPlantClass;
global using Random = UnityEngine.Random;
global using CustomPlantClass.Main;
using CustomPlantClass.Examples;

namespace UltimateCherryFireShooter_Remade
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public static ID PlantTypeFire = DataMgr.AllocateID();
        public static ID PlantTypeFinal = DataMgr.AllocateID();
        public static ID Bullet_FireCherry = DataMgr.AllocateID();
        public static ID Bullet_FireCherryFire = DataMgr.AllocateID();
        public static ID Bullet_FireCherryFinal = DataMgr.AllocateID();
        public static ID Bullet_FireCherryFinalFire = DataMgr.AllocateID();
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "cherryfireshooter"
            );
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = PlantTypeFire, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("UltimateCherryFireShooterPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("UltimateCherryFireShooterPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorTuple((PlantType.UltimateGatling,PlantType.Jalapeno)), // Optional fusion recipes

                AttackInterval = 1.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 300,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 1025,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = false,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

                CardColor = CardLevel.Gold, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menus
                IsUltimatePlant = true, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "究极樱火射手 • 重制版",           // Plant name (shown in UI)
                AlmanacEntry = "“别看咱披着樱桃皮，骨子里可是辣椒的暴脾气！”\n"+
                "<color=#3D1400>画师：</color><color=red>@ys羽衫</color>\n"+
                "<color=#3D1400>攻击：</color><color=red>300 * 4/1.5秒</color>\n"+
                "<color=#3D1400>特点：</color><color=red>出场、死亡时附带全屏火焰。自身免疫樱桃爆炸。每次可以向前方三行各射出四枚子弹，子弹对击中的僵尸附加红温效果，子弹可以过究极窝炬，伤害翻倍。3%的概率在本行生成火焰，造成1800伤害并使该行僵尸红温。</color>\n\n"+
                "<color=#3D1400>融合配方：</color><color=red>究极樱桃射手+火爆辣椒</color><color=#3D1400>警告：本植物过于热情,可能导致草坪温度过高,请僵尸们自备冰镇饮料</color>"    // Almanac description (CN + EN recommended)
            };

            // Register the plant and retrieve its ID
            ID plantID = DataMgr.RegisterCustomPlant<UltimateGatling, UltimateCherryFireShooter_Remade>(Data);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(plantID);

            // Fill out the plant metadata
            BaseCustomPlantData Data_final = new BaseCustomPlantData()
            {
                PlantId = PlantTypeFinal, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("FinalCherryFireGatlingPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("FinalCherryFireGatlingPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorTuple((PlantTypeFire,PlantType.Jalapeno)), // Optional fusion recipes

                AttackInterval = 1.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 900,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 1200,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

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
                IsUltimatePlant = true, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "终极樱火射手 • 重制版",           // Plant name (shown in UI)
                AlmanacEntry = "究极樱火射手的初版数值\n\n"+
                "<color=#3D1400>攻击：</color><color=red>900×4×3/2秒</color>\n"+
                "<color=#3D1400>特点：</color><color=red>①出场、死亡时附带全屏火焰。自身免疫樱桃爆炸；</color>\n"+
                "<color=#3D1400>②</color><color=red>每次可以向前方3行各射出4枚子弹，子弹对击中的僵尸附加红温效果，子弹可以过究极窝炬，究极星炬，伤害×3；</color>\n"+
                "<color=#3D1400>③</color><color=red>每射出1颗子弹有3%的概率射出大量子弹，持续5秒；</color>\n\n"+
                "<color=#3D1400>融合配方：</color><color=red>究极樱火射手+火爆辣椒 樱桃机枪射手+浴火三线射手</color>\n"+
                "<color=#3D1400>“你为什么能这么受欢迎？”其他究极樱桃射手都对这位新来的植物充满了好奇。“那可能是我的‘厨艺高超’吧。”究极樱火射手这样说着，其实他自己心知肚明，受欢迎的是红温效果，而不是自己。</color>"    // Almanac description (CN + EN recommended)
            };

            // Register the plant and retrieve its ID
            ID plantID_final = DataMgr.RegisterCustomPlant<UltimateGatling, FinalCherryFireShooter_Remade>(Data_final);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(plantID_final);


            CustomCore.RegisterCustomBullet<Bullet_superCherry>(Bullet_FireCherry, assetBundle.GetAsset<GameObject>("UltimateCherryFireShooterBullet1"));
            CustomCore.RegisterCustomBullet<Bullet_superCherry>(Bullet_FireCherryFire, assetBundle.GetAsset<GameObject>("UltimateCherryFireShooterBullet2"));
            CustomCore.RegisterCustomBullet<Bullet_superCherry>(Bullet_FireCherryFinal, assetBundle.GetAsset<GameObject>("FinalCherryFireGatlingBullet"));
            CustomCore.RegisterCustomBullet<Bullet_superCherry>(Bullet_FireCherryFinalFire, assetBundle.GetAsset<GameObject>("FinalCherryFireGatlingBullet2"));
            
            UltimateTorchBehaviour.AddBulletToPool(Bullet_FireCherry,Bullet_FireCherryFire);
            UltimateTorchBehaviour.AddBulletToPool(Bullet_FireCherryFinal,Bullet_FireCherryFinalFire);
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateCherryFireShooter_Remade.Bepinex";
        public const string PluginName = "UltimateCherryFireShooter_Remade";
        public const string PluginVersion = "3.9";
    }
}
