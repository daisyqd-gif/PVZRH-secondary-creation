global using BepInEx;
global using CustomPlantClass.CustomizeLib;
global using HarmonyLib;
global using System.Reflection;
global using UnityEngine;
global using System.Collections;
global using System.Collections.Generic;
global using CustomPlantClass;
global using Random = UnityEngine.Random;
global using System.Linq;
global using CustomPlantClass.Main;
using GameLevel.RogueShooting;
using CustomPlantClass.RogueShootingManager;

namespace MoreMinigun
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public static BuffID theCurseID;
        public static BuffID theSuperID;
        public static BaseBuff theBuff;
        public static BaseBuff realCherryBomb;

        public static ID NormalID;
        public static ID FireID;
        public static ID IceID;
        public static ID CherryID;
        public static ID DoomID;
        public static ID DoomScaredyID;
        public static ID DoomID_sp;
        public static ID DoomScaredyID_sp;
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "moreminigun"
            );
            NormalID = DataMgr.AllocateID();
            FireID = DataMgr.AllocateID();
            IceID = DataMgr.AllocateID();
            CherryID = DataMgr.AllocateID();
            DoomID = DataMgr.AllocateID();
            DoomScaredyID = DataMgr.AllocateID();
            DoomID_sp = DataMgr.AllocateID();
            DoomScaredyID_sp = DataMgr.AllocateID();
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData SuperMinigunData = new BaseCustomPlantData<UltimateMinigun, Minigun>
            (
                NormalID, // Automatically assigns a unique ID

                assetBundle.GetAsset<GameObject>("SuperMinigunPrefab"),   // Main plant prefab
                assetBundle.GetAsset<GameObject>("SuperMinigunPreview") // Card preview prefab
            )
            {
                Fusions = ListHelper.MirrorTuple((PlantType.GatlingPea, PlantType.SuperGatling)), // Optional fusion recipes

                AttackInterval = 0.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 20,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 1000,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

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

                Name = "速射机枪射手",           // Plant name (shown in UI)
                AlmanacEntry =
                    "高速连射的加特林机枪射手，火力稳定且持续。\n" +
                    "<color=#0000FF>加特林射手的强化形态</color>\n\n" +
                    "<color=#3D1400>特点：</color><color=red>0.5秒一次射击，持续输出强劲火力。</color>\n" +
                    "<color=#3D1400>PF效果：</color><color=red>进入短暂的超高速射击状态，射速提升并造成额外伤害。</color>\n\n" +
                    "<color=#3D1400>“速度不是全部，但在我这里，它能解决全部。”速射机枪射手自信地说道。</color>"
                // Almanac description (CN + EN recommended)
            }.Register();

            // Fill out the plant metadata
            BaseCustomPlantData SuperSnowMinigunData = new BaseCustomPlantData<UltimateMinigun, SnowMinigun>
            (
                IceID, // Automatically assigns a unique ID

                assetBundle.GetAsset<GameObject>("SuperSnowMinigunPrefab"),   // Main plant prefab
                assetBundle.GetAsset<GameObject>("SuperSnowMinigunPreview"))
            { // Card preview prefab

                Fusions = ListHelper.MirrorList(new List<(ID, ID)>() { (PlantType.GatlingPea, PlantType.SuperSnowGatling), (PlantType.SnowGatling, PlantType.SuperGatling), (PlantType.IceShroom, NormalID) }), // Optional fusion recipes

                AttackInterval = 0.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 20,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 1075,               // Sun cost

                DefaultBullet = BulletType.Bullet_snowPea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

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

                Name = "寒冰速射机枪射手",           // Plant name (shown in UI)
                AlmanacEntry =
                    "发射寒冰子弹的速射机枪射手，能迅速冻结战场节奏。\n" +
                    "<color=#0000FF>寒冰加特林的强化形态</color>\n\n" +
                    "<color=#3D1400>特点：</color><color=red>高速射击并附带寒冰减速效果。</color>\n" +
                    "<color=#3D1400>PF效果：</color><color=red>短时间内大幅提升射速，并发射强化寒冰子弹，显著降低僵尸移动速度。</color>\n\n" +
                    "<color=#3D1400>“冷静一点？我天生就很冷静。”寒冰速射机枪射手淡淡地说。</color>"
                // Almanac description (CN + EN recommended)
            }.Register();

            // Register the plant and retrieve its ID
            CustomCore.TypeMgrExtra.IsIcePlant.Add(IceID);

            // Fill out the plant metadata
            BaseCustomPlantData SuperFireMinigunData = new BaseCustomPlantData<UltimateMinigun, JalaMinigun>
            (
                FireID, // Automatically assigns a unique ID

                assetBundle.GetAsset<GameObject>("SuperJalaMinigunPrefab"),   // Main plant prefab
                assetBundle.GetAsset<GameObject>("SuperJalaMinigunPreview"))
            { // Card preview prefab

                Fusions = ListHelper.MirrorList(new List<(ID, ID)>() { (PlantType.JalaGatling, PlantType.SuperGatling), (PlantType.Jalapeno, NormalID) }), // Optional fusion recipes

                AttackInterval = 0.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 30,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 1125,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea_jala, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

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

                Name = "火爆速射机枪射手",           // Plant name (shown in UI)
                AlmanacEntry =
                    "以辣椒之怒驱动的速射机枪射手，每一发都带着灼热的火焰。\n" +
                    "<color=#0000FF>火爆加特林的强化形态</color>\n\n" +
                    "<color=#3D1400>特点：</color><color=red>高速射击并附带火焰灼烧效果。</color>\n" +
                    "<color=#3D1400>PF效果：</color><color=red>进入火焰狂暴状态，射速提升并发射高温灼烧子弹。</color>\n\n" +
                    "<color=#3D1400>“我不是脾气火爆，我只是射得太快。”火爆速射机枪射手解释道。</color>"
                // Almanac description (CN + EN recommended)
            }.Register();

            // Register the plant and retrieve its ID
            CustomCore.TypeMgrExtra.IsFirePlant.Add(FireID);

            // Fill out the plant metadata
            BaseCustomPlantData SuperCherryMinigunData = new BaseCustomPlantData<UltimateMinigun, CherryMinigun>
            (
                CherryID, // Automatically assigns a unique ID

                assetBundle.GetAsset<GameObject>("SuperCherryMinigunPrefab"),   // Main plant prefab
                assetBundle.GetAsset<GameObject>("SuperCherryMinigunPreview"))
            { // Card preview prefab

                Fusions = ListHelper.MirrorList(new List<(ID, ID)>() { (PlantType.GatlingPea, PlantType.SuperCherryGatling), (PlantType.CherryGatling, PlantType.SuperGatling), (PlantType.CherryBomb, NormalID) }), // Optional fusion recipes

                AttackInterval = 0.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 40,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 1150,               // Sun cost

                DefaultBullet = BulletType.Bullet_cherry, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

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

                Name = "樱桃速射机枪射手",           // Plant name (shown in UI)
                AlmanacEntry =
                    "融合樱桃爆炸能量的速射机枪射手，火力凶猛且不稳定。\n" +
                    "<color=#0000FF>樱桃加特林的强化形态</color>\n\n" +
                    "<color=#3D1400>特点：</color><color=red>高速射击并有概率触发小范围爆炸。</color>\n" +
                    "<color=#3D1400>PF效果：</color><color=red>短时间内射速暴涨，并发射爆裂樱桃子弹造成范围伤害。</color>\n\n" +
                    "<color=#3D1400>“爆炸是艺术，而我只是艺术家。”樱桃速射机枪射手笑着说。</color>"
                // Almanac description (CN + EN recommended)
            }.Register();

            // Fill out the plant metadata
            BaseCustomPlantData DoomMinigunData = new BaseCustomPlantData<UltimateMinigun, DoomMinigun>
            (
                DoomID, // Automatically assigns a unique ID

                assetBundle.GetAsset<GameObject>("DoomMinigunPrefab"),   // Main plant prefab
                assetBundle.GetAsset<GameObject>("DoomMinigunPreview"))
            { // Card preview prefab

                Fusions = ListHelper.MirrorList(new List<(ID, ID)>() { (PlantType.DoomGatling, PlantType.SuperGatling), (PlantType.DoomShroom, NormalID), (PlantType.Peashooter, DoomScaredyID) }), // Optional fusion recipes

                AttackInterval = 0.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 300,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 1150,               // Sun cost

                DefaultBullet = BulletType.Bullet_doom, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

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

                Name = "毁灭速射机枪射手",           // Plant name (shown in UI)
                AlmanacEntry = "发射毁灭子弹的加特林速射机枪\n" +
                "<color=#0000FF>毁灭菇机枪射手的限定形态</color>\n\n" +
                "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                "<color=#3D1400>使用条件：</color><color=red>①融合或转化毁灭机枪射手时有2%概率变异\n" +
                "②神秘模式\n" +
                "*可使用豌豆射手切回毁灭机枪射手\n" +
                "*可使用胆小菇切换究极速射毁灭机枪胆小菇</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>300x6/0.5秒</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①每次发射有5%概率发射大毁灭菇子弹，每第16发发射伤害1800的大毁灭菇子弹，造成半径3格无衰减溅射。\n" +
                "②启动射击需要预热1.5秒。</color>\n\n" +
                "<color=#3D1400>“你问我成功的秘卷是什么？是看到机会就立马抓住的反应，是成功路上不断披荆斩棘的勇气，是成功后与他人分享到喜悦！”究极速射毁灭菇机枪射手整理了下衣领，接着说道，“我们都曾遇到过困境，经历过迷惘，那是什么带领我们击败困境穿越迷惘的呢？是曾经的自己，所以为了感谢曾经的自己，好好对待当下的每一天吧。哦对了！别感冒！”</color>"    // Almanac description (CN + EN recommended)
            }.Register();

            // Register the plant and retrieve its ID

            // Fill out the plant metadata
            BaseCustomPlantData DoomMinigunScaredyData = new BaseCustomPlantData<GatlingDoomScaredy, DoomMinigunScaredy>
            (
                DoomScaredyID, // Automatically assigns a unique ID

                assetBundle.GetAsset<GameObject>("DoomMinigunScaredyPrefab"),   // Main plant prefab
                assetBundle.GetAsset<GameObject>("DoomMinigunScaredyPreview"))
            { // Card preview prefab

                Fusions = ListHelper.MirrorList(new List<(ID, ID)>() { (DoomID, PlantType.ScaredyShroom), (PlantType.ScaredyDoom, NormalID) }), // Optional fusion recipes

                AttackInterval = 0.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 300,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 1150,               // Sun cost

                DefaultBullet = BulletType.Bullet_doom, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

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

                Name = "毁灭速射机枪胆小菇",           // Plant name (shown in UI)
                AlmanacEntry = "发射毁灭菇的加特林速射机枪胆小菇\n" +
                "<color=#0000FF>毁灭机枪胆小菇的限定形态</color>\n\n" +
                "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                "<color=#3D1400>使用条件：</color><color=red>①融合或转化毁灭机枪胆小菇时有2%概率变异\n" +
                "②神秘模式\n" +
                "*可使用胆小菇切回毁灭机枪胆小菇\n" +
                "*可使用豌豆射手切换究极毁灭速射机枪</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>300/0.5秒</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①每攻击1次减少0.02秒攻击间隔，最低0.1秒\n" +
                "②启动射击需要预热1秒\n" +
                "③每次发射有5%概率发射大毁灭菇，每第16发为大毁灭菇\n" +
                "④3x3范围内有僵尸会害怕自爆并释放毁灭菇效果</color>\n\n" +
                "<color=#3D1400>究极毁灭速射机枪胆小菇经营着植物界最大的服装店，“一株植物，根据他的穿着就能看出他的性格或是爱好，我喜欢有个性的植物，他们勇敢又正义。”他曾荣获植物界服装设计绿叶奖，这是所有设计师们梦寐以求的奖项，每当有人问起，他总会说“不知道啊，我去参赛他们给我的～”</color>"    // Almanac description (CN + EN recommended)
            }.Register();

            // Fill out the plant metadata
            BaseCustomPlantData UDoomMinigunData = new BaseCustomPlantData<UltimateMinigun, UltimateDoomMinigun>
            (
                DoomID_sp, // Automatically assigns a unique ID

                assetBundle.GetAsset<GameObject>("UltimateDoomMinigunPrefab"),   // Main plant prefab
                assetBundle.GetAsset<GameObject>("UltimateDoomMinigunPreview"))
            { // Card preview prefab

                Fusions = ListHelper.MirrorList(new List<(ID, ID)>() { (DoomID, PlantType.DoomShroom), (DoomScaredyID, PlantType.DoomPeashooter) }), // Optional fusion recipes

                AttackInterval = 0.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 300,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 1150,               // Sun cost

                DefaultBullet = BulletType.Bullet_doom_ulti, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

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

                Name = "究极毁灭速射机枪射手",           // Plant name (shown in UI)
                AlmanacEntry =
                    "毁灭速射机枪射手的究极形态，火力、爆发与毁灭性全面提升。\n" +
                    "<color=#0000FF>毁灭加特林的究极形态</color>\n\n" +
                    "<color=#3D1400>特点：</color><color=red>高速连射、周期性大毁灭菇、极高爆发。</color>\n" +
                    "<color=#3D1400>PF效果：</color><color=red>进入究极毁灭形态，射速达到极限并发射超大毁灭菇子弹。</color>\n\n" +
                    "<color=#3D1400>“毁灭不是目的，是过程。”究极毁灭速射机枪射手低声说道。</color>"
                // Almanac description (CN + EN recommended)
            }.Register();

            // Fill out the plant metadata
            BaseCustomPlantData UDoomMinigunScaredyData = new BaseCustomPlantData<UltimateDoomScared, UltimateDoomMinigunScaredy>
            (
                DoomScaredyID_sp, // Automatically assigns a unique ID

                assetBundle.GetAsset<GameObject>("UltimateDoomMinigunScaredyPrefab"),   // Main plant prefab
                assetBundle.GetAsset<GameObject>("UltimateDoomMinigunScaredyPreview"))
            { // Card preview prefab

                Fusions = ListHelper.MirrorList(new List<(ID, ID)>() { (DoomScaredyID, PlantType.DoomShroom), (DoomID, PlantType.ScaredyDoom) }), // Optional fusion recipes

                AttackInterval = 0.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 300,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 1150,               // Sun cost

                DefaultBullet = BulletType.Bullet_doom_ulti, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

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

                Name = "究极毁灭速射机枪胆小菇",           // Plant name (shown in UI)
                AlmanacEntry =
                    "毁灭速射机枪射手的胆小变异体，融合了毁灭火力与胆小菇的恐惧机制。\n" +
                    "<color=#0000FF>毁灭机枪胆小菇的究极限定形态</color>\n\n" +
                    "<color=#3D1400>特点：</color><color=red>高速连射、恐惧自爆、攻击越久越快、周期性大毁灭菇。</color>\n" +
                    "<color=#3D1400>PF效果：</color><color=red>进入恐惧狂暴状态，射速突破极限并发射究极毁灭菇子弹。</color>\n\n" +
                    "<color=#3D1400>“勇气不是没有恐惧，而是带着恐惧继续开火。”究极毁灭速射机枪胆小菇颤抖着说。</color>"
                // Almanac description (CN + EN recommended)
            }.Register();

            CustomCore.RegisterCustomPlantSkin<UltimateMinigun, CherryMinigun_Ulti>
            (
                PlantType.UltimateMinigun,
                assetBundle.GetAsset<GameObject>("UltimateMinigunPrefab"),
                assetBundle.GetAsset<GameObject>("UltimateMinigunPreview"),
                (UltimateMinigun) => { }
            );
            
            CustomCore.AddFusion(PlantType.UltimateMinigun, PlantType.UltimateGatling, CherryID);
            CustomCore.AddFusion(PlantType.UltimateMinigun, CherryID, PlantType.UltimateGatling);

            CustomCore.RegisterSuperSkill(PlantType.UltimateMinigun, (Plant p) => 1000, (Plant p) =>
            {
                p.GetOrAddComponent<CherryMinigun_Ulti>().StartPF();
            });
            DataMgr.RegisterCustomStarUp(PlantType.UltimateMinigun);
            //DataMgr.AddGameAppInitAction(() => GameAPP.resourcesManager.plantPrefabs[PlantType.UltimateMinigun].AddComponent<CherryMinigun_Ulti>());
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
        public override void InitializeBuffs()
        {
            (theCurseID, theSuperID, theBuff) = RegistryHelper.RegisterCustomCurseBuff("数学的力量", "伤害增加不会影响子弹以及攻击伤害/2, 子弹变成, 当伤害叠加到500%时, 反转诅咒", "伤害增加从线性转换为二次及子弹变成小爆竹子弹", PlantType.UltimateMinigun, (Plant p) => p.TryGetComponent<UltimateMinigun>(out var b) && b.damageMultiplier > 6);
            (_, realCherryBomb) = RegistryHelper.RegisterCustomQualitativeChangeBuff("核能射线", "获得词条：核能射线", PlantType.UltimateMinigun, () => TravelMgr.Instance.GetNormalBuff((AdvBuff)30));
        }
        public override void OnGameInit()
        {
            GameAPP.resourcesManager.plantPrefabs[PlantType.UltimateMinigun].AddComponent<CherryMinigun_Ulti>();
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "MoreMinigun.Bepinex";
        public const string PluginName = "MoreMinigun";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
