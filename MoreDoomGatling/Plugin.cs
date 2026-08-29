global using BepInEx;
//global using BepInEx.Unity.IL2CPP;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
//global using Il2CppInterop.Runtime.Injection;
global using System;
global using System.Reflection;
global using UnityEngine;
global using UnityEngine.Rendering;
global using System.Collections;
global using System.Collections.Generic;
global using Random=UnityEngine.Random;
global using System.Linq;
global using CustomPlantClass.Main;


namespace MoreDoomGatling{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : CorePlugin
    {
        public const string PluginGuid = "MoreDoomGatling.Bepinex";
        public const string PluginName = "MoreDoomGatling";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
        public static ID gatlingA_ID=DataMgr.AllocateID();
        public static ID gatlingB_ID=DataMgr.AllocateID();
        public static ID gatlingC_ID=DataMgr.AllocateID();

        [Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override void OnStart()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
            //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            Tools.InitMod();
            UltimateFireDoomGatling.PLANT_ID=DataMgr.AllocateID();
            UltimateFireDoomScared.PLANT_ID=DataMgr.AllocateID();
            /*ClassInjector.RegisterTypeInIl2Cpp<UltimateFireDoomGatling>();
            ClassInjector.RegisterTypeInIl2Cpp<UltimateFireDoomScared>();
            ClassInjector.RegisterTypeInIl2Cpp<DestroyerCherryGatling>();
            ClassInjector.RegisterTypeInIl2Cpp<DestroyerCherryMinigun>();
            ClassInjector.RegisterTypeInIl2Cpp<Bullet_fireDoom>();
            ClassInjector.RegisterTypeInIl2Cpp<GatlingDoomPaper_a>();
            ClassInjector.RegisterTypeInIl2Cpp<GatlingDoomPaper_b>();
            ClassInjector.RegisterTypeInIl2Cpp<GatlingDoomPaper_c>();*/
            AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "moredoomgatling");

            CustomCore.RegisterCustomPlant<UltimateDoomGatling, UltimateFireDoomGatling>(
                UltimateFireDoomGatling.PLANT_ID,
                assetBundle.GetAsset<GameObject>("UltimateFireDoomGatlingPrefab"),
                assetBundle.GetAsset<GameObject>("UltimateDoomGatlingPreview"),
                new List<(int, int)> { ((int)PlantType.UltimateDoomGatling , (int)PlantType.SuperThreePeater) , ((int)PlantType.SuperThreePeater , (int)PlantType.UltimateDoomGatling)},
                1.5f,
                0f,
                1000,
                300,
                0f,
                1000
            );
            CustomCore.AddUltimatePlant (UltimateFireDoomGatling.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(UltimateFireDoomGatling.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add (UltimateFireDoomGatling.PLANT_ID, (CardLevel)4);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(UltimateFireDoomGatling.PLANT_ID);
            CustomCore.AddPlantAlmanacStrings(
                UltimateFireDoomGatling.PLANT_ID,
                $"初版•究极毁火射手 ({UltimateFireDoomGatling.PLANT_ID})",
                "<color=#3D1400>名称：</color><color=red>初版•究极毁火射手</color>\n" +
                "<color=#3D1400>融合配方：</color><color=red>SP究极毁灭机枪 + 浴火三线射手</color>\n\n" +

                "<color=#3D1400>登场效果：</color><color=red>出场时附带全屏火焰，自身免疫樱桃爆炸。</color>\n" +
                "<color=#3D1400>攻击方式：</color><color=red>每次向前方 3 行各行射出 4 枚子弹，覆盖宽广火力扇面。</color>\n" +
                "<color=#3D1400>特殊机制：</color><color=red>每射出 1 颗子弹有 3% 的概率触发“弹幕暴走”，瞬间额外射出 500 发子弹。</color>\n\n" +

                "<color=#3D1400>特点：</color><color=red>\n" +
                "1 - 出场即点燃全场，清理大范围杂兵与残血僵尸。\n" +
                "2 - 三行覆盖的毁火弹幕，适合正面压制与残局收割。\n" +
                "3 - 弹幕暴走触发时，能在短时间内倾泻极端火力，瞬间清空一整片战线。\n" +
                "4 - 免疫樱桃爆炸，适合与樱桃系、毁灭系组合使用。</color>\n\n" +

                "<color=#3D1400>词条1：</color><color=red>毁火降临 - 出场时释放全屏火焰。</color>\n" +
                "<color=#3D1400>词条2：</color><color=red>三线焚烧 - 同时攻击前方 3 行，每行 4 发子弹。</color>\n" +
                "<color=#3D1400>词条3：</color><color=red>弹幕暴走 - 每颗子弹有几率引发 500 发额外弹幕。</color>\n\n" +

                "<color=#3D1400>初版•究极毁火射手说：</color><color=red>“我不只是点火，我是把整条战线一起送进焚化炉。”</color>\n\n" +

                // English section
                "<color=#3D1400>English Name:</color> <color=red>Prototype Ultimate Fire Doom Gatling</color>\n" +
                "<color=#3D1400>Fusion Recipe:</color> <color=red>Ultimate Doom Gatling + Super Threepeater</color>\n\n" +

                "<color=#3D1400>On Spawn:</color> <color=red>Creates full-screen flames and is immune to cherry explosions.</color>\n" +
                "<color=#3D1400>Attack Pattern:</color> <color=red>Fires 4 bullets in each of the 3 forward rows per volley.</color>\n" +
                "<color=#3D1400>Special:</color> <color=red>Each bullet has a 5% chance to trigger a 500-bullet barrage.</color>\n\n" +

                "<color=#3D1400>Highlights:</color><color=red>\n" +
                "1 - Full-screen ignition on entry to wipe out weak or damaged zombies.\n" +
                "2 - Three-lane coverage makes it ideal for lane control and late-wave cleanup.\n" +
                "3 - Barrage trigger can instantly flood the board with firepower.\n" +
                "4 - Immune to cherry explosions, perfect with cherry and doom-based setups.</color>\n\n" +

                "<color=#3D1400>Ultimate Fire Doom Gatling says:</color><color=red>“If the screen is still visible, I’m not done shooting yet.”</color>\n\n" +

                "Thanks to @ys羽衫, @高数带我飞, @屿秋MirAcle, Google Translate, and @林秋-AutumnLin."
            );

            CustomCore.RegisterCustomPlant<UltimateDoomScared, UltimateFireDoomScared>(
                UltimateFireDoomScared.PLANT_ID,
                assetBundle.GetAsset<GameObject>("UltimateFireDoomScaredPrefab"),
                assetBundle.GetAsset<GameObject>("UltimateDoomScaredPreview"),
                new List<(int, int)> { ((int)PlantType.UltimateDoomScaredy , (int)PlantType.SuperThreePeater) , ((int)PlantType.SuperThreePeater , (int)PlantType.UltimateDoomScaredy)},
                1.5f,
                0f,
                1000,
                300,
                0f,
                1000
            );
            CustomCore.AddUltimatePlant (UltimateFireDoomScared.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(UltimateFireDoomScared.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add (UltimateFireDoomScared.PLANT_ID, (CardLevel)4);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(UltimateFireDoomScared.PLANT_ID);
            CustomCore.AddPlantAlmanacStrings(
                UltimateFireDoomScared.PLANT_ID,
                $"初版•究极毁火胆小菇 ({UltimateFireDoomScared.PLANT_ID})",
                "<color=#3D1400>名称：</color><color=red>初版•究极毁火胆小菇</color>\n" +
                "<color=#3D1400>融合配方：</color><color=red>SP究极毁灭胆小菇 + 浴火三线射手</color>\n\n" +

                "<color=#3D1400>登场效果：</color><color=red>出场时附带全屏火焰，自身免疫樱桃爆炸。</color>\n" +
                "<color=#3D1400>攻击方式：</color><color=red>以毁灭系火力进行远程输出，胆小却极具毁灭性。</color>\n" +
                "<color=#3D1400>特殊机制：</color><color=red>每射出 1 颗子弹有 3% 的概率触发 500 发额外弹幕。</color>\n\n" +

                "<color=#3D1400>特点：</color><color=red>\n" +
                "1 - 出场即点燃全场，适合开局清线或残局反打。\n" +
                "2 - 胆小外表下隐藏毁灭火力，适合后排保护与火力支援。\n" +
                "3 - 弹幕暴走可瞬间覆盖大范围，形成高密度火力网。\n" +
                "4 - 与毁灭系、樱桃系、三线系组合时协同极佳。</color>\n\n" +

                "<color=#3D1400>词条1：</color><color=red>焚世胆怯 - 出场即全屏点燃，但本人依旧会“害怕”。</color>\n" +
                "<color=#3D1400>词条2：</color><color=red>毁灭弹雨 - 子弹有几率引发 500 发毁灭弹幕。</color>\n\n" +

                "<color=#3D1400>初版•究极毁火胆小菇说：</color><color=red>“我只是有点紧张……可一紧张，世界就会烧起来。”</color>\n\n" +

                // English section
                "<color=#3D1400>English Name:</color> <color=red>Prototype Ultimate Fire Doom Scaredy</color>\n" +
                "<color=#3D1400>Fusion Recipe:</color> <color=red>Ultimate Doom Scaredy + Super Threepeater</color>\n\n" +

                "<color=#3D1400>On Spawn:</color> <color=red>Creates full-screen flames and is immune to cherry explosions.</color>\n" +
                "<color=#3D1400>Attack Pattern:</color> <color=red>Fires doom-flavored projectiles from the backline.</color>\n" +
                "<color=#3D1400>Special:</color> <color=red>Each bullet has a chance to unleash a 500-shot barrage.</color>\n\n" +

                "<color=#3D1400>Highlights:</color><color=red>\n" +
                "1 - Great for opening waves or emergency board clears.\n" +
                "2 - Looks timid, but its damage output is anything but.\n" +
                "3 - Barrage procs can erase entire pushes in seconds.\n" +
                "4 - Synergizes well with doom, cherry, and lane-control setups.</color>\n\n" +

                "<color=#3D1400>Ultimate Fire Doom Scaredy says:</color><color=red>“Please don’t look at me… look at the fire instead.”</color>\n\n" +

                "Thanks to @ys羽衫, @高数带我飞, @屿秋MirAcle, Google Translate, and @林秋-AutumnLin."
            );

            CustomCore.AddFusion(UltimateFireDoomScared.PLANT_ID,UltimateFireDoomGatling.PLANT_ID,(int)PlantType.ScaredyShroom);
            CustomCore.AddFusion(UltimateFireDoomScared.PLANT_ID,(int)PlantType.ScaredyShroom,UltimateFireDoomGatling.PLANT_ID);
            CustomCore.AddFusion(UltimateFireDoomGatling.PLANT_ID,UltimateFireDoomScared.PLANT_ID,(int)PlantType.Peashooter);
            CustomCore.AddFusion(UltimateFireDoomGatling.PLANT_ID,(int)PlantType.Peashooter,UltimateFireDoomScared.PLANT_ID);
            /*
            CustomCore.RegisterCustomPlant<UltimateGatling, DestroyerCherryGatling>(
                DestroyerCherryGatling.PLANT_ID,
                assetBundle.GetAsset<GameObject>("UltimateGatlingPrefab"),
                assetBundle.GetAsset<GameObject>("UltimateGatlingPreview"),
                new List<(int, int)> { ((int)PlantType.UltimateGatling , (int)PlantType.DoomShroom) , ((int)PlantType.DoomShroom , (int)PlantType.UltimateGatling)},
                1.5f,
                0f,
                500,
                300,
                0f,
                1000
            );
            CustomCore.RegisterCustomPlantSkin<UltimateGatling, DestroyerCherryGatling>(
                DestroyerCherryGatling.PLANT_ID,
                assetBundle.GetAsset<GameObject>("SkinUltimateGatlingPrefab"),
                assetBundle.GetAsset<GameObject>("UltimateGatlingPreview"),
                new List<(int, int)> { ((int)PlantType.UltimateGatling , (int)PlantType.DoomShroom) , ((int)PlantType.DoomShroom , (int)PlantType.UltimateGatling)},
                1.5f,
                0f,
                500,
                300,
                0f,
                1000
            );
            CustomCore.AddUltimatePlant (DestroyerCherryGatling.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(DestroyerCherryGatling.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add (DestroyerCherryGatling.PLANT_ID, (CardLevel)4);
            CustomCore.AddPlantAlmanacStrings(
                DestroyerCherryGatling.PLANT_ID,
                $"究极樱桃毁灭机枪射手 ({DestroyerCherryGatling.PLANT_ID})",
                "<color=#3D1400>名称：</color><color=red>究极樱桃毁灭机枪射手</color>\n" +
                "<color=#3D1400>融合配方：</color><color=red>究极机枪射手 + 毁灭菇</color>\n\n" +

                "<color=#3D1400>攻击方式：</color><color=red>每 1.5 秒发射 4 颗毁灭樱桃子弹。</color>\n" +
                "<color=#3D1400>伤害范围：</color><color=red>每颗子弹的爆炸范围与毁灭菇相同，伤害为 500。</color>\n\n" +

                "<color=#3D1400>特点：</color><color=red>\n" +
                "1 - 将毁灭菇的范围爆炸与机枪射手的高频输出结合在一起。\n" +
                "2 - 适合对付高血量密集僵尸群，持续轰炸整片战线。\n" +
                "3 - 与毁灭系、樱桃系、减速系组合时，清场效率极高。\n" +
                "4 - 需要一定经济支撑，但回报是极其稳定的范围压制力。</color>\n\n" +

                "<color=#3D1400>词条1：</color><color=red>毁灭樱桃弹 - 每颗子弹都拥有毁灭菇级别的爆炸范围。</color>\n" +
                "<color=#3D1400>词条2：</color><color=red>节奏轰炸 - 固定节奏持续输出范围爆炸火力。</color>\n\n" +

                "<color=#3D1400>究极樱桃毁灭机枪射手说：</color><color=red>“我不按扳机，我是在敲末日的节拍。”</color>\n\n" +

                // English section
                "<color=#3D1400>English Name:</color> <color=red>Ultimate Cherry Doom Gatling</color>\n" +
                "<color=#3D1400>Fusion Recipe:</color> <color=red>Ultimate Gatling + Doom Shroom</color>\n\n" +

                "<color=#3D1400>Attack Pattern:</color> <color=red>Fires 4 Destruction Cherry bullets every 1.5 seconds.</color>\n" +
                "<color=#3D1400>Explosion Radius:</color> <color=red>Each bullet explodes with Doom Shroom-sized area, dealing 500 damage.</color>\n\n" +

                "<color=#3D1400>Highlights:</color><color=red>\n" +
                "1 - Combines Doom Shroom’s area damage with Gatling’s sustained fire.\n" +
                "2 - Excellent against clustered, high-HP zombie waves.\n" +
                "3 - Pairs well with slow, stun, or armor-stripping plants.\n" +
                "4 - High-cost, high-impact artillery-style lane anchor.</color>\n\n" +

                "<color=#3D1400>Cherry Doom Gatling says:</color><color=red>“Every 1.5 seconds, I ask the same question: are you still alive?”</color>\n\n" +

                "Thanks to @无忆清为, @b站也干了, @机鱼吐司, Google Translate, and @林秋-AutumnLin."
            );

            CustomCore.RegisterCustomPlant<UltimateMinigun, DestroyerCherryMinigun>(
                DestroyerCherryMinigun.PLANT_ID,
                assetBundle.GetAsset<GameObject>("UltimateMinigunPrefab"),
                assetBundle.GetAsset<GameObject>("UltimateMinigunPreview"),
                new List<(int, int)> { ((int)PlantType.UltimateMinigun , (int)PlantType.DoomShroom) , ((int)PlantType.DoomShroom , (int)PlantType.UltimateMinigun)}, //ultimate minigun is not fuseable
                1.5f,
                0f,
                500,
                300,
                0f,
                1000
            );
            CustomCore.AddUltimatePlant (DestroyerCherryMinigun.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(DestroyerCherryMinigun.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add (DestroyerCherryMinigun.PLANT_ID, (CardLevel)5);
            CustomCore.AddPlantAlmanacStrings(
                DestroyerCherryMinigun.PLANT_ID,
                $"终极樱桃毁灭速射机枪射手 ({DestroyerCherryMinigun.PLANT_ID})",
                "<color=#3D1400>名称：</color><color=red>终极樱桃毁灭速射机枪射手</color>\n" +
                "<color=#3D1400>基础形态：</color><color=red>究极机枪射手的毁灭樱桃强化版本。</color>\n\n" +

                "<color=#3D1400>攻击方式：</color><color=red>持续发射樱桃毁灭子弹，部分子弹升级为大号毁灭樱桃弹。</color>\n" +
                "<color=#3D1400>子弹类型：</color><color=red>普通毁灭樱桃弹与强化大毁灭樱桃弹交替出现，大弹伤害为普通的数倍。</color>\n" +
                "<color=#3D1400>火力爆发：</color><color=red>在特定条件或增益下，可触发高密度散射与多重弹幕形态。</color>\n\n" +

                "<color=#3D1400>特点：</color><color=red>\n" +
                "1 - 以樱桃毁灭弹为核心的持续火力机枪，兼具范围与频率。\n" +
                "2 - 大号毁灭樱桃弹拥有更高伤害与毁灭效果，适合处理高血量目标。\n" +
                "3 - 在特定增益下可额外发射多组散射弹幕，形成极高密度火力覆盖。\n" +
                "4 - 与毁灭菇、樱桃系、火力增幅类植物搭配时，能构成极具观赏性的“樱桃弹幕秀”。</color>\n\n" +

                "<color=#3D1400>词条1：</color><color=red>樱桃连锁 - 普通毁灭樱桃弹与大毁灭樱桃弹交替出现。</color>\n" +
                "<color=#3D1400>词条2：</color><color=red>毁灭扫射 - 多重散射与高频弹幕覆盖整条战线。</color>\n\n" +

                "<color=#3D1400>终极樱桃毁灭速射机枪射手说：</color><color=red>“一颗樱桃是爆炸，五百颗樱桃是艺术。”</color>\n\n" +

                // English section
                "<color=#3D1400>English Name:</color> <color=red>Ultimate Cherry Doom Minigun</color>\n" +
                "<color=#3D1400>Core Concept:</color> <color=red>A doom-empowered minigun that fires cherry-based destruction rounds.</color>\n\n" +

                "<color=#3D1400>Attack Pattern:</color> <color=red>Continuously fires cherry doom bullets, occasionally upgrading shots into larger, stronger doom cherries.</color>\n" +
                "<color=#3D1400>Burst Potential:</color> <color=red>Under certain buffs, unleashes dense scatter fire and multi-layered barrages.</color>\n\n" +

                "<color=#3D1400>Highlights:</color><color=red>\n" +
                "1 - Sustained cherry-based doom fire with periodic high-damage spikes.\n" +
                "2 - Large doom cherries excel at deleting priority targets.\n" +
                "3 - Synergizes with doom, cherry, and firepower-boosting plants.\n" +
                "4 - Turns the lane into a continuous cherry-flavored bullet storm.</color>\n\n" +

                "<color=#3D1400>Cherry Doom Minigun says:</color><color=red>“If one cherry is overkill, imagine what a thousand can do.”</color>\n\n" +

                "Thanks to @无忆清为, @b站也干了, @机鱼吐司, Google Translate, and @林秋-AutumnLin."
            );
            */
            // Doom bullets
            CustomCore.RegisterCustomBullet<Bullet,Bullet_fireDoom>(
                Bullet_fireDoom.BULLET_ID,
                assetBundle.GetAsset<GameObject>("Bullet_fireDoom")
            );

            CustomCore.RegisterCustomBullet<Bullet,Bullet_fireDoom>(
                Bullet_fireDoom.BULLET_ID_BIG,
                assetBundle.GetAsset<GameObject>("Bullet_fireDoom_big")
            );

            // Cherry doom bullets
            CustomCore.RegisterCustomBullet<Bullet,Bullet_fireDoom>(
                Bullet_fireDoom.BULLET_ID_CHERRY,
                assetBundle.GetAsset<GameObject>("Bullet_superCherry")
            );

            CustomCore.RegisterCustomBullet<Bullet,Bullet_fireDoom>(
                Bullet_fireDoom.BULLET_ID_CHERRY_BIG,
                assetBundle.GetAsset<GameObject>("Bullet_superCherry_big")
            );

            // Squash doom bullets
            CustomCore.RegisterCustomBullet<Bullet_squash>(
                Bullet_fireDoom.BULLET_ID_CHERRY_SQUASH,
                assetBundle.GetAsset<GameObject>("Bullet_cherrySquash")
            );

            CustomCore.RegisterCustomBullet<Bullet_squash>(
                Bullet_fireDoom.BULLET_ID_CHERRY_SQUASH_BIG,
                assetBundle.GetAsset<GameObject>("Bullet_cherrySquash_big")
            );
            
            
            CustomCore.RegisterCustomZombie<GatlingPaperZombie_a,GatlingDoomPaper_a>(gatlingA_ID,assetBundle.GetAsset<GameObject>("DoomGatlingPaper_a"),assetBundle.GetAsset<Sprite>("DoomGatlingPaper_a_0"),500,3000,0,3000);
            CustomCore.TypeMgrExtra.UltimateZombie.Add(gatlingA_ID);
            DataMgr.AddCustomZombieSpawnRatio(gatlingA_ID,4,1000);
            CustomCore.RegisterCustomZombie<GatlingPaperZombie_b,GatlingDoomPaper_b>(gatlingB_ID,assetBundle.GetAsset<GameObject>("DoomGatlingPaper_b"),assetBundle.GetAsset<Sprite>("DoomGatlingPaper_b_0"),500,6000,0,6000);
            CustomCore.TypeMgrExtra.UltimateZombie.Add(gatlingB_ID);
            DataMgr.AddCustomZombieSpawnRatio(gatlingB_ID,5,600);
            CustomCore.RegisterCustomZombie<GatlingPaperZombie_c,GatlingDoomPaper_c>(gatlingC_ID,assetBundle.GetAsset<GameObject>("DoomGatlingPaper_c"),assetBundle.GetAsset<Sprite>("DoomGatlingPaper_c_0"),500,12000,0,12000);
            CustomCore.TypeMgrExtra.UltimateZombie.Add(gatlingC_ID);
            DataMgr.AddCustomZombieSpawnRatio(gatlingC_ID,6,300);
            DataMgr.AddLevelZombie(gatlingA_ID,gatlingB_ID,gatlingC_ID);
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
}
