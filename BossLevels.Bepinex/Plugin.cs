using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Reflection;
using System.Reflection.Metadata;
using UnityEngine;
using BepInEx.Unity.IL2CPP.Utils.Collections;
//using Unity.VisualScripting;
using static Il2CppSystem.Globalization.CultureInfo;
using Random = UnityEngine.Random;
using System.Threading;


namespace BossLevels{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "BossLevels.Bepinex";
        public const string PluginName = "BossLevels";
        public const string PluginVersion = "3.4.1";
        public static int ID=-1;
        public static int ID2=-1;
        public static CustomLevelData Boss44Level;
        public static CustomLevelData SuperConveyLevel;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            var level = new CustomLevelData
            {
                Name = () => "Zombie Battle : Boss",
                Sun = () => 50000,
                BoardTag = new Board.BoardTag
                {
                    isConvey = true,
                    isSeedRain = false,
                    isSuperRandom = true,
                    enableAllTravelPlant = true,
                    HorseBoss = false,
                    Iz_ai = false,
                    rShowHealth = false,
                    zombieDropSun = false,
                    disableNormalSun = false,
                    zombieRevive = false,
                    isScaredyDream = false,
                    isTowerDefence = false,
                    isShooting = false,
                    rogueShooting = false,
                    newShooting = false,
                    isIndestructible = false,
                    isColumn = false,
                    isElementRandom = false,
                    isDrawCards = false,
                    isUltimateSuperRandom = false,
                    isNight = false,
                    isBigMap = false,
                    freeCamera = false,
                    isEndless = false,
                    isTravel = false,
                    randomTravel = false,
                    quickTravel = false,
                    isCustom = false,
                    isEditor = false,
                    enableTravelPlant = true,
                    enableTravelBuff = true,
                    isRoof = false,
                    isGarden = false,
                    isMirror = false,
                    isExchange = false,
                    shooting_loon = false,
                    isBoss = false,
                    isBoss2 = false,
                    isFreeCardSelect = false,
                    isTutor = false,
                    isObsidianImp = false,
                    isDixMix = false,
                    isSingle = false,
                    bungiBattle = false,
                    isBejeweled = false,
                    isBubbleGame = false,
                    isScaryPot = false,
                    isMidMap = false,
                    isChess = false,
                    isMidMap2 = false,
                    isLookStar = false,
                    isGardenBattle = false,
                    isRandomMix = false,
                    isRandomMix2 = false,
                    freeGloveZombie = false,
                    disableMower = false,
                    isHappyRandom = false,
                    oppsiteBuff = false,
                    pvpScaryPot = false,
                    ultimateEndless = false,
                    isHammerZombie = false,
                    fastZombie = false,
                    isHugeGravity = false,
                    zombieSplit = false,
                    fullStrike = false,
                    billiardBall = false,
                    isSnake = false,
                    isSquash = false,
                    zombieBattle = false,
                    is2048 = false,
                    newTower = false,
                    isRogue = false,
                    isFruitNinjia = false,
                    isFruitNinjia2 = false,
                    lightShadow = false,
                    isLoonGame = false,
                    snowBoss = false,
                    playerShooting = false,
                    smallZombie = false,
                    isFlagGame = false,
                    isTreasure = false,
                    isBrick = false
                },
                WaveCount = () => 100,
                RowCount = 6,
                SceneType = (SceneType)6,        // Day_6
                BgmType = MusicType.Boss,         // Boss music
                NeedSelectCard = false,

                // Pre-selected cards
                /*PreSelectCards = () => new List<PlantType>
                {
                    (PlantType)256, (PlantType)256, (PlantType)256,
                    (PlantType)256, (PlantType)256,
                    (PlantType)256, (PlantType)256, (PlantType)256, (PlantType)250,
                    (PlantType)250, (PlantType)227, (PlantType)227, (PlantType)245,
                    (PlantType)245
                },*/

                //AdvBuffs = () => new List<int> { GetAdvBuffId("拆分"), GetAdvBuffId("至极手速"), GetAdvBuffId("全息制冷"), GetAdvBuffId("极速战备") },
                AdvBuffs = () => new List<int> { 1003, 1014, 1000 },
                // ⭐ Instant Boss 44 spawn — no delay needed
                PostBoard = (board) =>
                {
                    board.musicType = (int)MusicType.Loon;
                    TravelMgr.Instance.GetNormalBuff(AdvBuff.EnumValue1003);
                    TravelMgr.Instance.GetNormalBuff(AdvBuff.EnumValue1014);
                    TravelMgr.Instance.GetNormalBuff(AdvBuff.EnumValue1000);
                    GameAPP.Instance.StartCoroutine(BepInEx.Unity.IL2CPP.Utils.Collections.CollectionExtensions.WrapToIl2Cpp(delayspawn()));
                },
                ConveyBeltPlantTypes = () => new List<PlantType> {
                    (PlantType)256,(PlantType)1173,(PlantType)253,(PlantType)240,(PlantType)245,(PlantType)227,(PlantType)2018,(PlantType)1931,(PlantType)5084
                },
                ZombieList = () => new List<ZombieType>()
                {
                    ZombieType.RandomZombie,
                    ZombieType.RandomPlusZombie,
                    ZombieType.DiamondRandomZombie
                }
            };
            Boss44Level =level;
            ID=CustomCore.RegisterCustomLevel(level);

            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
        public IEnumerator delayspawn()
        {
            yield return new WaitForSeconds (15f);
            SpawnBoss44();
            GameAPP.Instance?.PlayMusic(MusicType.Boss2);
        }
        public static void SpawnBoss44()
        {
            int row = 0;
            float axisX = 9.9f;

            var cz = CreateZombie.Instance;
            if (cz == null)
                return;

            /*//GameObject zombot = cz.SetZombie(row, (ZombieType)44, axisX, false);
            GameObject horse = cz.SetZombie(row, ZombieType.HorseBoss, axisX, false);

            if (horse != null)
            {
                Zombie z = horse.GetComponent<Zombie>();
                if (z != null)
                {
                    z.theMaxHealth *= 15;
                    z.theHealth *= 15;
                    z.UpdateHealthText();
                }
            }*/
            GameObject zombot = cz.SetZombie(row, ZombieType.ZombieBoss, axisX, false);
            //GameObject zombot = cz.SetZombie(row, ZombieType.HorseBoss, axisX, false);

            if (zombot != null)
            {
                Zombie z1 = zombot.GetComponent<Zombie>();
                if (z1 != null)
                {
                    z1.theMaxHealth *= 1000;
                    z1.theHealth *= 1000;
                    z1.UpdateHealthText();
                }
            }
            GameObject zombot2 = cz.SetZombieWithMindControl(row, ZombieType.ZombieBoss, Mouse.Instance.GetBoxXFromColumn(-1), false);
            //GameObject zombot = cz.SetZombie(row, ZombieType.HorseBoss, axisX, false);

            if (zombot2 != null)
            {
                Zombie z2 = zombot2.GetComponent<Zombie>();
                if (z2 != null)
                {
                    z2.theMaxHealth *= 1000;
                    z2.theHealth *= 1000;
                    z2.UpdateHealthText();
                }
            }
            //GameAPP.Instance?.PlayMusic((MusicType)18);
        }

    }
    [HarmonyPatch(typeof(ConveyManager), nameof(ConveyManager.GetCardType))]
    public static class ConveyMaxSun_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ConveyManager __instance)
        {
            __instance.Board.theSun=__instance.Board.maxSun;
            
            return true;
        }
    }
    [HarmonyPatch(typeof(ZombieBoss))]
    public static class ZombieBoss_Mind_Controlled_Fix_Patch
    {
        [HarmonyPatch(nameof(ZombieBoss.AnimSpawn))]
        [HarmonyPrefix]
        public static bool AnimSpawn_Patch(ZombieBoss __instance)
        {
            if(__instance.isMindControlled==false) return true;
            int random=Random.Range(0,8);
            switch (random)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                AnimSpawn_Patch2(__instance);
                break;
                case 4:
                case 5:
                Ball(__instance);
                break;
                default:
                Garg_Spawn(__instance);
                break;
            }
            return false;
        }
        public static bool AnimSpawn_Patch2(ZombieBoss __instance)
        {
            int theRow; //Row
            CreateZombie __this_00; //Create Zombie
            Transform __this_01; //Transform
            ZombieType theZombieType; //Zombie Type
            Vector3 pUVar3; //Transform Pos
            
            theRow = __instance.targetRow;
            __this_00 = CreateZombie.Instance;
            theZombieType = __instance.GetZombieType();
            __this_01 = __instance.spawnPosition;
            if (__this_01 != null) {
                pUVar3 = __this_01.position;
                if (__this_00 != null) {
                    __this_00.SetZombieWithMindControl(theRow,theZombieType,pUVar3.x,false);
                    __instance.spawnCount+=1;
                    __instance.summonCount+=1;
                    return false;
                }
            }
            return true;
        }
        [HarmonyPatch(nameof(ZombieBoss.GetZombieType))]
        [HarmonyPrefix]
        public static bool GetZombieType_Patch(ZombieBoss __instance,ref ZombieType __result)
        {
            if(__instance.board.boardTag.isSuperRandom==true){
                if (__instance.summonCount < 5)
                {
                    __result=ZombieType.RandomZombie;
                    return false;
                }
                ZombieType[] pool={ZombieType.RandomZombie,ZombieType.RandomPlusZombie,ZombieType.DiamondRandomZombie};

                __result=pool[Random.Range(0,pool.Length)];
                return false;
            }
            return false;
        }
        public static bool Ball(ZombieBoss __instance)
        {
            if(!__instance.isMindControlled)return true;
            for(int i = 0; i < __instance.board.rowNum; i++)
            {
                __instance.board.boardAction.CreateFireLine(i,3600,fromType:PlantType.Jalapeno);
            }
            __instance.board.boardAction.CreateFreeze(__instance.transform.position,4);
            return false;
        }
        public static bool Garg_Spawn(ZombieBoss __instance)
        {
            if(!__instance.isMindControlled)return true;
            Board board = __instance.board;
            if (board == null) return false;

            int rowCount = board.rowNum;   // usually 5 or 6 depending on level
            float x = Mouse.Instance.GetBoxXFromColumn(0);// left side of the board

            var cz = CreateZombie.Instance;
            if (cz == null) return false;

            // Spawn a Gargantuar on every lane
            for (int row = 0; row < rowCount; row++)
            {
                GameObject garg = cz.SetZombieWithMindControl(row, ZombieType.ArmedGargantuar, x, false);

                if (garg != null)
                {
                    Zombie z = garg.GetComponent<Zombie>();
                    if (z != null)
                    {
                        // Optional: buff garg health or damage
                        z.theMaxHealth *= 2;
                        z.theHealth *= 2;
                        z.UpdateHealthText();
                        z.theSpeed*=3;
                    }
                }
            }

            // Play smash sound (same as vanilla)
            int sound = Random.Range(8, 10);
            GameAPP.PlaySound(sound, 0.5f, 1f);

            // Screen shake (same as vanilla)
            ScreenShake.TriggerShake(0.15f);

            // Crash sound (same as vanilla)
            GameAPP.PlaySound(0x4A, 0.5f, 1f);

            // Skip original plant-smashing logic
            return false;
        }
        [HarmonyPatch(nameof(ZombieBoss.AnimBungi))]
        [HarmonyPrefix]
        public static bool BungiBlocker(ZombieBoss __instance)
        {
            if(__instance.isMindControlled) return false;
            return true;
        }
        [HarmonyPatch(nameof(ZombieBoss.AnimCrash))]
        [HarmonyPrefix]
        public static bool CrashBlocker(ZombieBoss __instance)
        {
            if(__instance.isMindControlled) return false;
            return true;
        }
        [HarmonyPatch(nameof(ZombieBoss.AnimPutBall))]
        [HarmonyPrefix]
        public static bool BallBlocker(ZombieBoss __instance)
        {
            if(__instance.isMindControlled) return false;
            return true;
        }
        [HarmonyPatch(nameof(ZombieBoss.AnimRv))]
        [HarmonyPrefix]
        public static bool RVBlocker(ZombieBoss __instance)
        {
            if(__instance.isMindControlled) return false;
            return true;
        }
    }
}
