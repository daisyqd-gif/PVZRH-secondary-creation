global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using System.Reflection;
global using System.Collections;
global using UnityEngine;
global using UnityEngine.Rendering;
global using Unity.VisualScripting;
global using CustomPlantClass;
global using System.Collections.Generic;
global using CustomPlantClass.Main;
using CustomPlantClass.RogueShootingManager;
using GameLevel.RogueShooting;
using CustomPlantClass.Examples;


namespace UltimateSniper
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : ModPlugin
    {
        public const string PluginGuid = "UltimateFlameGatling_Remade.Bepinex";
        public const string PluginName = "UltimateFlameGatling_Remade";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;

        #region Custom GameObjects
        public static GameObject FireMeteor2 = null;
        public static GameObject ZombieBurn = null;
        public static GameObject FireStar = null;
        public static GameObject IceStar = null;
        //public static GameObject Doom1 = null;
        //public static GameObject Doom_Big = null;
        public static GameObject Doom_Big_Fire = null;
        #endregion

        #region Custom buffs
        public static BuffID Buff0 = -1;
        public static BuffID Buff1 = -1;
        public static BuffID Buff2 = -1;
        public static BuffID Buff_Curse = -1;
        public static BuffID Buff_Reverse = -1;
        #endregion
        public static AssetBundle assetBundle;
        #region ID getters
        public static ID UFlameGatling = DataMgr.AllocateID();
        public static ID UFlameSniper = DataMgr.AllocateID();
        public static ID BType_Flame = DataMgr.AllocateID();
        public static ID BType_FireFlame = DataMgr.AllocateID();
        public static ID BType_Explode = DataMgr.AllocateID();
        public static ID BType_Explode_Ice = DataMgr.AllocateID();
        public static ID Placeholder = DataMgr.AllocateID();
        public static ID Placeholder_2 = DataMgr.AllocateID();
        public static ID Doom_Big_Fire_ID = DataMgr.AllocateID();
        public static ID cherryType = DataMgr.AllocateID();
        public static ID ParticleID = DataMgr.AllocateID();
        #endregion
        public override void InitializeMod()
        {
            assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ultimatesniper");

            UltimateSniper.PLANT_ID = DataMgr.AllocateID();
            UltimateMegaGatlingPea.PLANT_ID = DataMgr.AllocateID();
            UltimateFirePea.BULLET_ID = DataMgr.AllocateID();
            UltimateCherryPea.BULLET_ID = DataMgr.AllocateID();
            UltimateExplosivePea.BULLET_ID = DataMgr.AllocateID();

            #region Ultimate Sniper
            CustomCore.RegisterCustomPlant<FireSniper, UltimateSniper>(
                UltimateSniper.PLANT_ID,
                assetBundle.GetAsset<GameObject>("FireSniperPrefab"),
                assetBundle.GetAsset<GameObject>("FireSniperPreview"),
                new List<(int, int)> { },
                3f,
                0f,
                4000,
                300,
                0f,
                1000
            );

            CustomCore.AddUltimatePlant(UltimateSniper.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(UltimateSniper.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add(UltimateSniper.PLANT_ID, (CardLevel)5);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(UltimateSniper.PLANT_ID);
            CustomCore.AddPlantAlmanacStrings(
                UltimateSniper.PLANT_ID,
                $"终极狙击豌豆 ({UltimateSniper.PLANT_ID})",
                "精准狙杀型终极火力单位，每次射击都在积蓄能量。\n" +
                "<color=#3D1400>伤害：</color><color=red>高额单体伤害，具备处决能力</color>\n" +
                "<color=#3D1400>特性：</color><color=red>精准狙击 / 充能处决 / 星级强化后更快触发</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①每次命中都会累积充能。\n" +
                "②达到阈值后可直接处决被狙击的僵尸（无视血量与伤害上限）。\n" +
                "③星级强化后处决所需命中数减少，爆发更频繁。</color>\n" +
                "<color=#3D1400>词条1：</color><color=red>终极瞄准：命中数达到上限后自动处决目标</color>\n" +
                "<color=#3D1400>词条2：</color><color=red>火线连锁：星级强化后触发小范围火焰爆炸</color>\n\n" +

                "<color=#3D1400>终极狙击豌豆说：</color><color=red>“我不需要扫射，也不需要乱枪。\n" +
                "只要一颗子弹，就能让战场安静下来。\n" +
                "僵尸们永远不知道下一发会落在谁的头上。”</color>\n\n" +

                // English Section
                "<color=#3D1400>English Description:</color>\n" +
                "<color=red>The Ultimate Sniper is a precision execution unit.\n" +
                "Each shot builds charge, and upon reaching the threshold,\n" +
                "it instantly eliminates the sniped zombie regardless of health.</color>\n\n" +
                "<color=#3D1400>Traits:</color><color=red> Precision / Charge Execution / Star‑Up Boost</color>\n" +
                "<color=#3D1400>Notes:</color><color=red> Builds charge with each hit.\n" +
                "Executes the target on full charge.\n" +
                "Star‑Up reduces the required hits.</color>\n\n" +
                "<color=#3D1400>Ultimate Sniper says:</color><color=red>“One shot. One silence.\n" +
                "I don’t spray — I decide.”</color>"
            );
            #endregion

            #region Ultimate Mega Gatling Pea
            CustomCore.RegisterCustomPlant<SuperSnowGatling, UltimateMegaGatlingPea>(
                UltimateMegaGatlingPea.PLANT_ID,
                assetBundle.GetAsset<GameObject>("SuperSnowGatlingPrefab"),
                assetBundle.GetAsset<GameObject>("SuperSnowGatlingPreview"),
                new List<(int, int)> { (UltimateSniper.PLANT_ID, (int)PlantType.Peashooter), ((int)PlantType.Peashooter, UltimateSniper.PLANT_ID) },
                1.5f,
                0f,
                4000,
                300,
                0f,
                1000
            );

            CustomCore.AddUltimatePlant(UltimateMegaGatlingPea.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(UltimateMegaGatlingPea.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add(UltimateMegaGatlingPea.PLANT_ID, (CardLevel)5);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(UltimateMegaGatlingPea.PLANT_ID);
            CustomCore.AddPlantAlmanacStrings(
                UltimateMegaGatlingPea.PLANT_ID,
                $"终极机枪 ({UltimateMegaGatlingPea.PLANT_ID})",
                "终极火力压制单位，能在短时间内倾泻毁灭性弹幕。\n" +
                "<color=#3D1400>伤害：</color><color=red>超高速连射 + 火樱桃混合弹</color>\n" +
                "<color=#3D1400>特性：</color><color=red>火力压制 / 终极扫射 / 终极陨石召唤</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①每次命中都会累积全局充能。\n" +
                "②充能满后可召唤终极火陨石，造成毁灭性范围伤害。\n" +
                "③星级强化后可触发终极狂暴扫射（PF）。</color>\n" +
                "<color=#3D1400>词条1：</color><color=red>火樱混弹：随机发射火焰或樱桃爆裂弹</color>\n" +
                "<color=#3D1400>词条2：</color><color=red>终极狂暴：小概率进入超高速扫射模式</color>\n\n" +

                "<color=#3D1400>终极大哥豌豆说：</color><color=red>“我不是普通的加特林，我是战场的终极火力点。\n" +
                "当我开始扫射时，僵尸们最好祈祷自己不在射程内。”</color>\n\n" +

                // English Section
                "<color=#3D1400>English Description:</color>\n" +
                "<color=red>The Ultimate Mega Gatling Pea unleashes overwhelming firepower.\n" +
                "Every hit builds global charge, allowing it to summon a devastating fire meteor.\n" +
                "Star‑Up grants a chance to enter a rapid‑fire frenzy.</color>\n\n" +
                "<color=#3D1400>Traits:</color><color=red> Rapid Fire / Fire‑Cherry Hybrid / Meteor Summon</color>\n" +
                "<color=#3D1400>Notes:</color><color=red> Builds global charge.\n" +
                "Can summon a fire meteor.\n" +
                "May enter frenzy mode.</color>\n\n" +
                "<color=#3D1400>Ultimate Mega Gatling says:</color><color=red>“When I spin up, the battlefield belongs to me.”</color>"
            );
            CustomCore.CustomPlantClicks.Add(UltimateMegaGatlingPea.PLANT_ID, (Plant p) =>
            {
                p.FlashOnce();
                if (UltimateMegaGatlingPea.HitCount >= 10 && !FireMeteor.exists)
                {
                    UltimateMegaGatlingPea.MakeMeteor();
                }
            });
            CustomCore.AddFusion(UltimateSniper.PLANT_ID, (int)PlantType.Peashooter, UltimateMegaGatlingPea.PLANT_ID);
            CustomCore.AddFusion(UltimateSniper.PLANT_ID, UltimateMegaGatlingPea.PLANT_ID, (int)PlantType.Peashooter);
            #endregion

            #region Ultimate Flame Gatling
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = UFlameGatling, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("UltimateFlameGatlingPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("UltimateFlameGatlingPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorList(new([(PlantType.JalaGatling, PlantType.PortalDoom),(UFlameSniper, PlantType.Peashooter)])), // Optional fusion recipes

                AttackInterval = 1.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 750,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 850,               // Sun cost

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

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = true, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "究极炽焰机枪",           // Plant name (shown in UI)
                AlmanacEntry = "“燃尽，此身！”\n\n" +
                "<color=#3D1400>使用条件：</color><color=red>旅行模式</color>\n<color=#3D1400>融合配方：</color><color=red>火辣机枪射手+超时空毁灭菇</color>\n\n" +
                "<color=#3D1400>伤害:</color><color=red>450×4/1秒\n" +
                "<color=#3D1400>特点:</color>\n"+
                "<color=#3D1400>①</color><color=red>每次射击增加1点充能，充能达到50时点击召唤赤焰陨星；</color>\n" +
                "<color=#3D1400>②</color><color=red>子弹为僵尸附加灼烧状态，增加1点灼烧值，并使僵尸进入传送状态，增加5秒已等待时间,对进入传送状态的僵尸增加20×已等待时间的伤害,最多增加1500伤害，对不可进入传送状态的僵尸造成3倍伤害；\n" +
                "<color=#3D1400>③</color><color=red>灼烧状态:每1秒对僵尸所在格内的所有僵尸造成100×灼烧值（灼烧值最高18点）的灰烬伤害，若僵尸不处于红温状态，则受到的伤害×1.5倍（怒火攻心改为2.5倍）；</color>\n" +
                "<color=#3D1400>④</color><color=red>赤焰陨星:落地对全场僵尸造成3600点灰烬伤害，对进入传送状态的僵尸增加20×已等待时间的伤害,最多增加1500伤害，对不可进入传送状态的僵尸造成3倍伤害，并为僵尸附加灼烧和传送状态，增加1点灼烧值和10秒已等待时间，然后留下30个子弹；子弹同赤焰陨星，但伤害为1800。</color>\n" +
                "<color=#3D1400>词条1</color><color=red>:星火燎原:灼烧范围提升至3×3，灼烧值上限×3</color>\n<color=#3D1400>词条2:</color><color=red>飞沙走石:赤焰陨星的留下的子弹数量×3，所有子弹伤害×3</color>\n\n" +
                "<color=#3D1400>究极炽焰机枪原本只是个普通机枪射手，某天被僵尸踩了叶子后，气得把豌豆换成了火球，还拿了个全息底座让自己“飞”了起来。</color>"    // Almanac description (CN + EN recommended)
            };
            BaseCustomPlantData Data2 = new BaseCustomPlantData()
            {
                PlantId = UFlameSniper, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("UltimateFlameSniperPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("UltimateFlameSniperPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorTuple((UFlameGatling, PlantType.Peashooter)), // Optional fusion recipes

                AttackInterval = 1f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 450,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 850,               // Sun cost

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

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = true, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "究极炽焰狙击射手",           // Plant name (shown in UI)
                AlmanacEntry = "“燃尽，此身！”\n\n" +
                "尚未撰写\n<color=#3D1400>融合配方：</color><color=red>火辣机枪射手+超时空毁灭菇</color>"
            };

            // Register the plant and retrieve its ID
            DataMgr.RegisterCustomPlant<UltimateGatling, UltimateFlameGatling_Remade>(Data);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(UFlameGatling);
            CustomCore.CustomPlantClicks.Add(UFlameGatling, (Plant p) =>
            {
                if (p.TryGetComponent<UltimateFlameGatling_Remade>(out var a)) a.Clicked();
            });
            DataMgr.RegisterCustomPlant<FireSniper, UltimateFlameSniper>(Data2);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(UFlameSniper);
            #endregion

            #region Bullets
            CustomCore.RegisterCustomBullet<Bullet_firePea, UltimateFirePea>(UltimateFirePea.BULLET_ID, assetBundle.GetAsset<GameObject>("Bullet_firePea_ultimate"));
            CustomCore.RegisterCustomBullet<Bullet_superCherry, UltimateCherryPea>(UltimateCherryPea.BULLET_ID, assetBundle.GetAsset<GameObject>("Bullet_cherryPea_ultimate"));
            CustomCore.RegisterCustomBullet<Bullet_superCherry, UltimateExplosivePea>(UltimateExplosivePea.BULLET_ID, assetBundle.GetAsset<GameObject>("Bullet_cherryPea_ultimate_explosive"));

            CustomCore.RegisterCustomBullet<Bullet_sword, FlamePea>(BType_Flame, assetBundle.GetAsset<GameObject>("Bullet_flamePea"));
            CustomCore.RegisterCustomBullet<Bullet_sword, FlamePea>(BType_FireFlame, assetBundle.GetAsset<GameObject>("Bullet_flamePea_fire"));
            CustomCore.RegisterCustomBullet<Bullet_sword, FlamePea_Explosive>(BType_Explode, assetBundle.GetAsset<GameObject>("Bullet_flamePea_Star"));
            #endregion

            #region misc
            CustomCore.RegisterSuperSkill(UltimateMegaGatlingPea.PLANT_ID, (Plant p) => 1000, (Plant p) =>
            {
                if (p.TryGetComponent<UltimateMegaGatlingPea>(out var comp))
                {
                    comp.plant.StartCoroutine(comp.SuperShoot_Custom());
                }
            });

            DataMgr.RegisterCustomStarUp(UltimateSniper.PLANT_ID);
            DataMgr.RegisterCustomStarUp(UltimateMegaGatlingPea.PLANT_ID);DataMgr.AddCustomStrongUltimatePlant
            (
                UFlameGatling,
                DataMgr.FormatStrongUltimateUnlockBuff("究极炽焰机枪","火辣机枪射手","超时空毁灭菇","究极炽焰狙击射手","豌豆射手","豌豆射手"),
                Buff0,Buff1,BuffBgType.Day,UFlameSniper
            );

            //DataMgr.AddCustomPlantUpgrade(PlantType.FireSniper, UltimateSniper.PLANT_ID, 5f);
            //DataMgr.AddCustomPlantUpgrade(UFlameGatling, UltimateMegaGatlingPea.PLANT_ID, 5f);
            #endregion

            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
        public override void InitializeBuffs()
        {
            Buff0 = Compatibility.CustomCore_Old.RegisterCustomBuff("星火燎原：究极炽焰机枪灼烧范围提升至3×3，灼烧值上限×3", BuffType.UltimateBuff, () => true, 3000, UFlameGatling, 1, BuffBgType.Day);
            Buff1 = Compatibility.CustomCore_Old.RegisterCustomBuff("飞沙走石：究极炽焰机枪赤焰陨星的留下的子弹数量×3，所有子弹伤害×3，当充能超过200时究极炽焰机枪将使用75充能召唤2个赤焰陨星", BuffType.UltimateBuff, () => true, 3000, UFlameGatling, 1, BuffBgType.Day);
            (Buff2,BaseBuff wildfire1) = RegistryHelper.RegisterCustomQualitativeChangeBuff("山火","灼烧将传播",UFlameGatling);
            (AdvBuff _,BaseBuff Buff1_2) = RegistryHelper.RegisterCustomQualitativeChangeBuff("飞沙走石","究极炽焰机枪赤焰陨星的留下的子弹数量×3，所有子弹伤害×3，当充能超过200时究极炽焰机枪将使用75充能召唤2个赤焰陨星",UFlameGatling,()=>TravelMgr.Instance.GetNormalBuff(Buff1));
            BaseBuff uBuff = RegistryHelper.MakeBuffType(new CustomRogueShootingBuff(){
                CustomPlantType = UFlameGatling,
                CustomTitle = "强化：灼烧",
                CustomDescription = "灼烧区域大小+1",
                CustomOnGet = delegate
                {
                    ZombieBurn_2.Radius+=1;
                },
                CustomBuffType = ShootingBuffType.UniqueUpgrade
            });
            BaseBuff uBuff2 = RegistryHelper.MakeBuffType(new CustomRogueShootingBuff(){
                CustomPlantType = UFlameGatling,
                CustomTitle = "强化：赤焰陨星",
                CustomDescription = "赤焰陨星所需子弹数量-10",
                CustomOnGet = delegate
                {
                    if (ShootingManager.Instance.TryGetPlant(UFlameGatling, out var plant) && plant != null && plant.TryGetComponent<UltimateFlameGatling_Remade>(out var p))
                    {
                        p.MeteorCD-=10;
                    }
                },
                CustomBuffType = ShootingBuffType.UniqueUpgrade
            });
            BaseConfig config1 = RegistryHelper.MakeConfigType(new CustomRogueShootingConfig(){
                CustomPlantType = UFlameGatling,

                CustomBuffs = () => new(){wildfire1,Buff1_2,uBuff,uBuff2,new DamageBuff(UFlameGatling), new SpeedBuff(UFlameGatling), new StarUpBuff(UFlameGatling)},

                CustomReinforcePlant = (Plant plant) =>
                {
                    plant.ModifyDamage(PlantDamageAdder.Shooting, 14.0f, false, new Il2CppSystem.Nullable<float>(float.MaxValue));
                },

                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            BaseConfig config2 = RegistryHelper.MakeConfigType(new CustomRogueShootingConfig(){
                CustomPlantType = PlantType.JalaGatling,

                CustomBuffs = () => new(){new UpgradeBuff(PlantType.JalaGatling,UFlameGatling)},

                CustomReinforcePlant = (Plant plant) =>
                {
                    plant.ModifyDamage(PlantDamageAdder.Shooting, 14.0f, false, new Il2CppSystem.Nullable<float>(float.MaxValue));
                },

                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            RegistryHelper.AddCustomRogueShootingPlant(UFlameGatling,config1);
            RegistryHelper.AddCustomRogueShootingPlant(PlantType.JalaGatling,config2);
            RegistryHelper.InjectUpgradeBuff(RSConfigType.Peashooter,PlantType.JalaGatling);
        }
        public override void OnStart()
        {

            #region GameObject assignment
            FireMeteor2 = assetBundle.GetAsset<GameObject>("BigStar");
            FireMeteor2.AddComponent<FireMeteor>();
            FireMeteor2.GetComponent<SortingGroup>().sortingLayerName = "fog";

            ZombieBurn = assetBundle.GetAsset<GameObject>("ZombieBurn");
            ZombieBurn.AddComponent<Burn>();
            ZombieBurn.GetComponent<SortingGroup>().sortingLayerName = "fog";

            //Doom1 = assetBundle.GetAsset<GameObject>("Doom_bright");
            //Doom1.AddComponent<CustomParticle>();
            //CustomCore.RegisterCustomParticle(Doom1_ID, Doom1);

            //Doom_Big = assetBundle.GetAsset<GameObject>("Doom_bright_big");
            //Doom_Big.AddComponent<CustomParticle>();
            //CustomCore.RegisterCustomParticle(Doom_Big_ID, Doom_Big);

            Doom_Big_Fire = assetBundle.GetAsset<GameObject>("Doom_fire_big");
            Doom_Big_Fire.AddComponent<CustomParticle>();
            CustomCore.RegisterCustomParticle(Doom_Big_Fire_ID, Doom_Big_Fire);

            FireStar = assetBundle.GetAsset<GameObject>("BigFlameStar");
            FireStar.AddComponent<FireStar>();
            FireStar.GetComponent<SortingGroup>().sortingLayerName = "fog";

            CustomCore.RegisterCustomParticle(ParticleID,assetBundle.GetAsset<GameObject>("ShootFire"));

            CustomCore.RegisterCustomCherry(cherryType, assetBundle.GetAsset<GameObject>("BombCloud_bright"));
            #endregion

            UltimateTorchBehaviour.AddBulletToPool(BType_Flame,BType_FireFlame);
        }
    }
    // Token: 0x02000023 RID: 35
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.GetDamage))]
    public class ZombieGetDamagePatch
    {
        // Token: 0x0600011C RID: 284 RVA: 0x00005920 File Offset: 0x00003B20
        [HarmonyPrefix]
        public static void Prefix(Zombie __instance, DmgType theDamageType, ref int theDamage)
        {
            if (__instance.TryGetComponent<ZombieBurn_2>(out var _) && !__instance.effects.ContainsKey(EffectType.Freeze))
            {
                if (Lawnf.TravelAdvanced(AdvBuff.EnumValue15))
                {
                    theDamage = Mathf.CeilToInt(theDamage * 2.5f);
                    return;
                }
                theDamage = Mathf.CeilToInt(theDamage * 1.5f);
            }
        }
    }
}
