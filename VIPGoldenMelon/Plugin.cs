global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using Il2CppInterop.Runtime.Injection;
global using System.Reflection;
global using UnityEngine;
global using System.Collections.Generic;
global using CustomPlantClass.Main;

namespace VIPGoldenMelon
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "VIPGoldenMelon.Bepinex";
        public const string PluginName = "VIPGoldenMelon";
        public const string PluginVersion = "3.7";
        public static ID bulletType = BulletType.Bullet_melon;
        public override void Load()
        {
            // Apply all Harmony patches in this assembly
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            // Register the custom plant class with IL2CPP
            // (Required for all custom MonoBehaviours)
            ClassInjector.RegisterTypeInIl2Cpp<VIPGoldenMelon>();
            ClassInjector.RegisterTypeInIl2Cpp<TrophyComp>();

            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            AssetBundle assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "goldenmelon"
            );

            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = DataMgr.AllocateID(), // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("MelonpultPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("MelonpultPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorList(new List<(ID,ID)>{(PlantType.GoldMelon,PlantType.Marigold)}), // Optional fusion recipes

                AttackInterval = 3f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 150,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 600,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type

                CanPF = false,     // Enable PF ability if the plant has one
                CanStarUp = false, // Enable Star-Up ability if the plant has one

                CardColor = CardLevel.Blue, // Determines card rarity and UI color
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

                Name = "黄金西瓜投手",           // Plant name (shown in UI)
                AlmanacEntry = "黄金西瓜投手向敌人投掷黄金西瓜！\n韧性：1000\n伤害：150×2/3s\n特点：每枚金瓜砸中僵尸将进行一次“抽奖”：\n幸运奖50%：降落一枚西瓜砸中该僵尸；\n四等奖35%：降落一枚冰瓜砸中该僵尸；\n三等奖10%：掉落100阳光；\n二等奖4%：摧毁其防具；\n一等奖0.999%：将其魅惑；\n特等奖0.001%：获得通关奖杯。"    // Almanac description (Copied from hybrid)
            };

            // Register the plant and retrieve its ID
            ID plantID = DataMgr.RegisterCustomPlant<Melonpult, VIPGoldenMelon>(Data);
            CustomCore.TypeMgrExtra.IsSpecialPlant.Add(plantID);
            VIPGoldenMelon.thePlantType=plantID;

            CustomCore.RegisterCustomParticle(plantID,assetBundle.GetAsset<GameObject>("MelonSplat"));
            bulletType=DataMgr.AllocateID();

            CustomCore.RegisterCustomBullet<Bullet_melon>(bulletType,assetBundle.GetAsset<GameObject>("Bullet_melon_gold"));

            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class VIPGoldenMelon : MonoBehaviour
    {
        public Melonpult plant=>gameObject.GetComponent<Melonpult>();
        public void Start()
        {
            plant.shoot = plant.gameObject.transform.GetChild(0);
        }
        public static ID thePlantType=PlantType.Nothing;
    } //melonpult uses shooter.getbullettype so it can be patched
    [HarmonyPatch(typeof(Melonpult),nameof(Melonpult.GetBulletType))]
    public class Melonpult_GetBulletType_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Melonpult __instance, ref BulletType __result)
        {
            if (__instance.thePlantType == VIPGoldenMelon.thePlantType)
            {
                __result=Core.bulletType;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Bullet_melon))]
    public class GoldenMelon_HitZombie_Patch
    {
        [HarmonyPatch(nameof(Bullet_melon.HitZombie))]
        [HarmonyPostfix]
        public static void HitZombie_Prefix(Bullet_melon __instance, Zombie zombie)
        {
            // Safety checks
            if (__instance == null || zombie == null)
                return;

            // Only run for Golden Melon bullets
            if (__instance.theBulletType != Core.bulletType)
                return;

            // Run jackpot logic
            RunGoldenMelonJackpot(__instance, zombie);
        }

        [HarmonyPatch(nameof(Bullet_melon.HitLand))]
        [HarmonyPostfix]
        public static void HitLand_Prefix(Bullet_melon __instance)
        {
            // Safety checks
            if (__instance == null)
                return;

            // Only run for Golden Melon bullets
            if (__instance.theBulletType != Core.bulletType)
                return;

            // Run jackpot logic
            SpawnMelon(__instance, BulletType.Bullet_melon);
        }

        private static void RunGoldenMelonJackpot(Bullet_melon bullet, Zombie zombie)
        {
            float roll = Random.value;

            // 49%: Drop a normal melon
            if (roll < 0.49f)
            {
                SpawnMelon(bullet, BulletType.Bullet_melonCannon);
                return;
            }

            // 35%: Drop an ice melon
            if (roll < 0.84f)
            {
                SpawnMelon(bullet, BulletType.Bullet_melonCannon);
                zombie.SetFreeze(3f);
                return;
            }

            // 10%: Drop 100 sun
            if (roll < 0.94f)
            {
                CreateItem.Instance.SetCoin(Mouse.Instance.GetColumnFromX(bullet.transform.position.x), bullet.theBulletRow, 0, 0);
                return;
            }

            // 4%: Destroy armor
            if (roll < 0.98f)
            {
                zombie.theFirstArmorHealth=0;
                zombie.theSecondArmorHealth=0;
                return;
            }

            // 1%: Charm
            if (roll < 0.999f)
            {
                zombie.SetMindControl();
                return;
            }

            // 0.001%: Trophy
            DropTrophy();
        }

        private static void SpawnMelon(Bullet_melon bullet, BulletType type)
        {
            var newBullet = CreateBullet.Instance.SetBullet(bullet.transform.position.x,bullet.transform.position.y+2f,bullet.theBulletRow,type,BulletMoveWay.Cannon);
            var pos2 = newBullet.cannonPos;
            pos2.x = bullet.transform.position.x - 0.15f;
            pos2.y = bullet.transform.position.y;
            newBullet.cannonPos = pos2;
            newBullet.Damage=bullet.Damage;
            newBullet.fromType = bullet.fromType;
        }

        private static void DropTrophy()
        {
            SpawnItem("Board/Award/TrophyPrefab");
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002134 File Offset: 0x00000334
        public static void SpawnItem(string resourcePath)
        {
            GameObject gameObject = Resources.Load<GameObject>(resourcePath);
            bool flag = gameObject != null;
            if (flag)
            {
                UnityEngine.Object.Instantiate(gameObject, new Vector2(0f, 0f), Quaternion.identity, GameAPP.board.transform).AddComponent<TrophyComp>();
            }
        }
    }
    public class TrophyComp : MonoBehaviour
    {
        public void Update()
        {
            bool flag = GameAPP.board != null && GameAPP.theGameStatus == 0;
            if (flag)
            {
                bool flag2 = GameAPP.board.TryGetComponent<Board>(out var board);
                if (flag2)
                {
                    bool flag3 = board.zombieArray != null;
                    if (flag3)
                    {
                        for (int i = 0; i < board.zombieArray.Count; i++)
                        {
                            Zombie zombie = board.zombieArray[i];
                            bool flag4 = zombie != null && !zombie.isMindControlled;
                            if (flag4)
                            {
                                zombie.Die(0);
                                bool flag5 = zombie != null;
                                if (flag5)
                                {
                                    Destroy(zombie.gameObject);
                                    Board component = GameAPP.board.GetComponent<Board>();
                                    int theTotalNumOfZombie = component.theTotalNumOfZombie;
                                    component.theTotalNumOfZombie = theTotalNumOfZombie - 1;
                                }
                            }
                        }
                    }
                    bool flag6 = board.boardEntity.plantArray != null;
                    if (flag6)
                    {
                        for (int j = 0; j < board.boardEntity.plantArray.Count; j++)
                        {
                            Plant plant = board.boardEntity.plantArray[j];
                            bool flag7 = plant != null;
                            if (flag7)
                            {
                                plant.thePlantHealth = 1145141919;
                                plant.theShieldHealth = 810;
                                plant.thePlantMaxHealth = 1145141919;
                            }
                        }
                    }
                }
            }
        }
    }
}
