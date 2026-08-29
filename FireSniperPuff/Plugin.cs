global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using Il2CppInterop.Runtime.Injection;
global using System;
global using System.Reflection;
global using UnityEngine;
global using System.Collections.Generic;
global using CustomPlantClass;
global using Unity.VisualScripting;
global using CustomPlantClass.Main;
global using CustomPlantClass.RogueShootingManager;
global using GameLevel.RogueShooting;

namespace FireSniperPuff
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public static BuffID superBuffID;
        public static BuffID curseBuffID;
        public static BuffID curseReverseBuffID;
        public static BaseConfig config;
        public static BaseConfig config_2;
        public static PlantType thePlantType;

        public override void Load()
        {
            try{
                // Apply all Harmony patches in this assembly
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

                // Register the custom plant class with IL2CPP
                // (Required for all custom MonoBehaviours)
                ClassInjector.RegisterTypeInIl2Cpp<FireSniperPuff>();

                // Load the AssetBundle containing your plant prefab(s)
                // Replace "abname" with your actual bundle name
                AssetBundle assetBundle = CustomCore.GetAssetBundle(
                    Assembly.GetExecutingAssembly(),
                    "firesniperpuff"
                );

                // Fill out the plant metadata
                BaseCustomPlantData Data = new BaseCustomPlantData()
                {
                    PlantId = DataMgr.AllocateID(), // Automatically assigns a unique ID

                    Prefab = assetBundle.GetAsset<GameObject>("SniperPuffPrefab"),   // Main plant prefab
                    Preview = assetBundle.GetAsset<GameObject>("SniperPuffPreview"), // Card preview prefab

                    Fusions = DataMgr.MirrorList(new List<(ID, ID)>(){(PlantType.SniperPuff,PlantType.Jalapeno)}), // Optional fusion recipes

                    AttackInterval = 3f,   // Time between attacks (shooters only)
                    ProduceInterval = 0f,  // Time between sun/production cycles
                    AttackDamage = 1000,      // Damage per attack
                    MaxHealth = 300,       // Plant HP
                    Cd = 7.5f,               // Card cooldown
                    Sun = 375,               // Sun cost

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
                    IsUltimatePlant = true, // Travel-locked ultimate plant
                    CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                    Name = "火焰狙击小队",           // Plant name (shown in UI)
                    AlmanacEntry = "<color=#3D1400>火焰狙击小队常说：“话说天下大势，分久必合，合久必分……你。”他在战斗之余，总是会阅读《三绷演义》。“但当涉猎，见往事耳。我可是最聪明的狙击手！”</color>\n"+
                    "狙击小喷菇连队，分可狙击点杀，合可爆破群攻。\n\n<color=#3D1400>使用条件：</color><color=red>旅行模式</color>\n"+
                    "<color=#3D1400>伤害：</color><color=red>500×3/3秒（分散）\n1500/0.5秒（合体）</color>\n"+
                    "<color=#3D1400>特性：</color><color=red>低矮</color>\n<color=#3D1400>特点：①</color><color=red>分散状态下，每第6次攻击爆头，造成100万伤害</color>\n"+
                    "<color=#3D1400>②</color><color=red>攻击15次后，进入15秒的合体状态</color>\n<color=#3D1400>③</color><color=red>合体状态下，攻击对半径1格范围造成爆炸伤害</color>\n"+
                    "<color=#3D1400>融合配方：</color>\n<color=red>火爆辣椒+狙击小队</color>"    // Almanac description (CN + EN recommended)
                };

                // Register the plant and retrieve its ID
                ID plantID = DataMgr.RegisterCustomPlant<SniperPuff, FireSniperPuff>(Data);
                CustomCore.RegisterCustomParticle(FireSniperPuff.pt=DataMgr.AllocateID(),assetBundle.GetAsset<GameObject>("BombCloud_vision_pea"));

                (superBuffID, BaseBuff buff) = RegistryHelper.RegisterCustomQualitativeChangeBuff("枪神降临","火焰狙击小队始终保持合体状态",plantID);
                (curseBuffID, curseReverseBuffID, BaseBuff curseBuff) = RegistryHelper.RegisterCustomCurseBuff("射击练习","火焰狙击小队爆头所需攻击次数固定在20, 当打出100次爆头时翻转诅咒","攻击对半径5格范围造成爆炸伤害和前方全场索敌",plantID,(Plant p) => FireSniperPuff.CD >= 100);
                

                BaseBuff uniqueUpgradeBuff = RegistryHelper.MakeBuffType(new CustomRogueShootingBuff()
                {
                    CustomPlantType = plantID,
                    CustomTitle = "爆头",
                    CustomDescription = "爆头所需攻击次数 - 1",
                    CustomBuffType = ShootingBuffType.UniqueUpgrade,
                    CustomOnGet = () =>
                    {
                        if(ShootingManager.Instance.TryGetPlant(plantID, out var plant))
                        {
                            plant.shootingLevel += 1;
                        }
                    }
                });
                Func<List<BaseBuff>> buffs = () => new List<BaseBuff>()
                    {
                        new DamageBuff(plantID),
                        new SpeedBuff(plantID),
                        buff, uniqueUpgradeBuff, curseBuff
                    };
                config = RegistryHelper.MakeConfigType(
                    new CustomRogueShootingConfig()
                {
                    CustomPlantType = plantID,
                    CustomBuffs=buffs,
                    CustomReinforcePlant = (Plant plant) =>
                    {
                        
                    },
                    CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
                });
                RegistryHelper.AddCustomRogueShootingPlant(plantID,config);
                var buff2 = () => new List<BaseBuff>()
                    {
                        new UpgradeBuff(PlantType.SniperPuff,plantID)
                    };
                config_2 = RegistryHelper.MakeConfigType(new CustomRogueShootingConfig()
                {
                    CustomPlantType = PlantType.SniperPuff, 
                    CustomBuffs=buff2,
                    CustomReinforcePlant = (Plant plant) =>
                    {
                        
                    },
                    CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
                });
                RegistryHelper.AddCustomRogueShootingPlant(PlantType.SniperPuff,config_2);
                RegistryHelper.InjectUpgradeBuff(RSConfigType.SmallPuff,PlantType.SniperPuff);
                Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
            }
            catch (Exception e)
            {
                DataMgr.StartUpMessages.Add(MyPluginInfo.PluginName+" load failed.\n"+e.ToString());
            }
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class FireSniperPuff : BaseCustomPlant
    {
        public static ParticleType pt;
        public static int CD = 0;
        public SniperPuff plant => GetComponent<SniperPuff>();
        public override void OnSpawn()
        {
            plant.isShort=true;
            if (Lawnf.TravelAdvanced(Plugin.superBuffID))
            {
                plant.anim.SetBoolString("gather",true);
                plant.attributeCount=0;
                plant.attributeCountdown=99999f;
                plant.theStatus=PlantStatus.Raised;
            }
        }
        public override void OnFixedUpdate()
        {
            if (Lawnf.TravelAdvanced(Plugin.superBuffID))
            {
                plant.anim.SetBoolString("gather",true);
                plant.attributeCount=0;
                plant.attributeCountdown=99999f;
                plant.theStatus=PlantStatus.Raised;
            }
        }
        public override Bullet Shoot_Custom()
        {
            if (Lawnf.TravelAdvanced(Plugin.superBuffID))
            {
                plant.anim.SetBoolString("gather",true);
                plant.attributeCount=0;
                plant.attributeCountdown=99999f;
                plant.theStatus=PlantStatus.Raised;
                goto returning;
            }
            Vector2 pos=plant.axis.position;
            var check = (Zombie z) => plant.CheckZombie(z);
            Zombie target = Lawnf.GetNearestZombie(plant.board,pos,check);
            if(target==null || target.IsDestroyed() || target.isMindControlled) return null;
            ParticleManager.Instance.SetParticle(0,target.col.bounds.center,target.theZombieRow,true);
            plant.shootCount++;
            target.SetJalaed();
            target.JalaedExplode(true,100);
            if (plant.shootCount >= GetHeadShot())
            {
                plant.shootCount=0;
                target.TakeDamage(2100000000,_plant.Cast<IDamageMaker>(),DamageType.MaxDamage,_plant.thePlantType,false);
                //target.TakeDamage(DamageType.MaxDamage, 2100000000, plant.thePlantType, false);
                if(Lawnf.TravelAdvanced(Plugin.curseBuffID)) CD++;
            }
            else
            {
                target.TakeDamage(_plant.attackDamage,_plant.Cast<IDamageMaker>(),DamageType.NormalAll,_plant.thePlantType,false);
            }
            GameAPP.PlaySound(0x28, 0.2f, 1f);
            plant.attributeCount++;
            if (plant.attributeCount == 14 || Lawnf.TravelAdvanced(Plugin.superBuffID))
            {
                plant.anim.SetBoolString("gather",true);
                plant.attributeCount=0;
                plant.attributeCountdown=Lawnf.TravelAdvanced(Plugin.superBuffID) ? 99999f : 15f;
                plant.theStatus=PlantStatus.Raised;
            }
            returning:
            return null;
        }
        public override Bullet Shoot2_Custom()
        {
            if (Lawnf.TravelAdvanced(Plugin.superBuffID))
            {
                plant.anim.SetBoolString("gather",true);
                plant.attributeCount=0;
                plant.attributeCountdown=99999f;
                plant.theStatus=PlantStatus.Raised;
            }
            // 1. Get axis + board
            Transform axis = plant.axis;
            Board board = plant.board;

            if (axis == null || board == null)
                return null;

            // 2. Get shoot position
            Vector2 shootPos = axis.position;

            // 3. Find nearest valid zombie
            var variable= (Zombie z) => plant.CheckZombie(z);
            Zombie target = Lawnf.GetNearestZombie(
                board,
                shootPos,
                variable
            );

            if (!target)
                return null;

            // 4. Get target position (its axis)
            Transform zAxis = target.axis;
            if (zAxis == null)
                return null;

            Vector2 center = zAxis.position;

            // 5. Physics AoE around target
            LayerMask mask = plant.zombieLayer;
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, Lawnf.TravelAdvanced(Plugin.curseReverseBuffID) ? 10f : 2f, mask);
            var c = plant.shootCount;
            if(plant.shootCount >= (plant.board.boardTag.rogueShooting ? 11 - plant.shootingLevel : 6) && Lawnf.TravelAdvanced(Plugin.curseBuffID)) CD++;
            if (hits != null)
            {
                foreach (var hit in hits)
                {
                    if (!hit) continue;

                    if (hit.TryGetComponent(out Zombie z))
                    {
                        // Rough reconstruction of the type / state checks:
                        // (IL2CPP was switching on zombie type + flags)
                        if (!z.isMindControlled && !z.beforeDying && z.enabled)
                        {
                            z.SetJalaed();
                            z.JalaedExplode(true);
                            if (plant.shootCount >= GetHeadShot())
                            {
                                c=0;
                                z.TakeDamage(
                                    DamageType.Carred,   // 0x12
                                    2100000000,
                                    plant.thePlantType,
                                    false
                                );
                            }
                            else
                            {
                                // AoE damage: attackDamage * 3
                                // (FUN_180002e60(...) in IL2CPP)
                                z.TakeDamage(
                                    DamageType.Carred,   // 0x12
                                    plant.attackDamage * 3,
                                    plant.thePlantType,
                                    false
                                );
                            }

                        }
                    }
                }
            }
            plant.shootCount = c;
            plant.shootCount++;

            // 6. Hit particle + sound on main target
            if (target.col != null && ParticleManager.Instance != null)
            {
                Vector2 hitPos = target.col.bounds.center;
                ParticleManager.Instance.SetParticle(
                    pt,              // particle ID from IL2CPP
                    hitPos,
                    target.theZombieRow,
                    true
                );

                GameAPP.PlaySound(0x28, 0.2f, 1f);
            }

            return null; // no bullet spawned
        }
        private int GetHeadShot()
        {
            if(Lawnf.TravelAdvanced(Plugin.curseBuffID)) return 20;
            return plant.board.boardTag.rogueShooting ? 11 - plant.shootingLevel : 6;
        }
        public override string GetTextString() => $"充能 : {plant.shootCount} / {GetHeadShot()}\n大招冷却 : {plant.attributeCount} / 14\n大招时间 : {(Lawnf.TravelAdvanced(Plugin.superBuffID) ? "∞" : (int)plant.attributeCountdown)} / 15{(Lawnf.TravelAdvanced(Plugin.curseBuffID)?$"\n{CD} / 200" : "")}";
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "FireSniperPuff.Bepinex";
        public const string PluginName = "FireSniperPuff";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
    /*
    [HarmonyPatch(typeof(GameLevel.RogueShooting.SmallPuff))]
    public static class SmallPuff_Patch
    {
        [HarmonyPatch(nameof(GameLevel.RogueShooting.SmallPuff.Buffs), MethodType.Getter)]
        [HarmonyPostfix]
        public static void PostGetBuffs(ref Il2CppSystem.Collections.Generic.List<BaseBuff> __result)
        {
            __result.Add(new UpgradeBuff(PlantType.SmallPuff, PlantType.SniperPuff));
        }
    }
    */
    [HarmonyPatch(typeof(Board))]
    public static class Board_Die_Patch
    {
        [HarmonyPatch(nameof(Board.Die))]
        [HarmonyPostfix]
        public static void Postfix()
        {
            FireSniperPuff.CD = 0;
        }
    }
    [HarmonyPatch(typeof(SniperPuff))]
    public static class SniperPuff_SearchZombie_Patch
    {
        [HarmonyPatch(nameof(SniperPuff.SearchZombie))]
        [HarmonyPostfix]
        public static void Postfix(SniperPuff __instance, ref GameObject __result)
        {
            if(!Lawnf.TravelAdvanced(Plugin.curseReverseBuffID)) return;
            __instance.zombieList.Clear();
            float bestDist = float.MaxValue;
            Zombie best = null;
            var pos = __instance.axis.position;
            foreach (var x in Lawnf.GetAllZombies())
            {
                if(x.IsNotNull() && __instance.CheckZombie(x))
                {
                    float dist = Vector2.Distance(new Vector2(pos.x,pos.y),new Vector2(x.axis.position.x,x.axis.position.y));
                    if(dist < bestDist)
                    {
                        bestDist = dist;
                        best = x;
                    }
                }
            }
            if(best != null)
            {
                __result=best.gameObject;
            }
        }
    }
}
