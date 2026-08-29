using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.IO;
using System.Text.Json;
using Core;


namespace DouQuQu{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "DouQuQu.Bepinex";
        public const string PluginName = "DouQuQu";
        public const string PluginVersion = "3.5.0";
        public static CustomLevelData _Level;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            string folder = Path.Combine(Paths.PluginPath, "DouQuQu");
            Directory.CreateDirectory(folder);

            ClassInjector.RegisterTypeInIl2Cpp<DouQuQuMgr>();

            var level = new CustomLevelData
            {
                Name = () => "斗蛐蛐",
                Sun = () => 50000,
                BoardTag = new Board.BoardTag
                {
                    isSuperRandom = true,
                    enableAllTravelPlant = true,
                    enableTravelBuff = true,
                    isFreeCardSelect = true
                },
                WaveCount = () => 100,
                RowCount = 5,
                SceneType = SceneType.Day,
                BgmType = MusicType.Day,
                NeedSelectCard = false,
                AdvBuffs = () => new List<int> { 1003, 1014, 1000, 1006 },
                PostBoard = (board) =>
                {
                    var douququ=board.GetOrAddComponent<DouQuQuMgr>();
                },
                ZombieList = () => new List<ZombieType>() {ZombieType.LandSubmarine} //this zombie does not move at all and does no damage
            };
            _Level =level;
            CustomCore.RegisterCustomLevel(level);
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    public class DouQuQuMgr : MonoBehaviour
    {
        public bool stopSpawn = false;
        private Board board;
        public int zcount=0;
        public bool auto=false;
        public bool displayed=false;
        public int pscore=0;
        public int zscore=0;
        public bool displayscore=false;
        public bool altLang=false;
        public bool isLoading=false;
        public bool fullMower=false;


        public void Awake()
        {
            board = GetComponent<Board>();
            EnsureArenaFile();
            LogScore("===New Session Started===");
        }

        public void Update()
        {
            // Hotkey: reload arena
            if (Lawnf.GetKeyDown(KeyCode.Home))
            {
                reset();
            }
            if (Lawnf.GetKeyDown(KeyCode.PageUp))
            {
                ExportBoard();
            }
            if (zcount == 0 && auto)
            {
                LoadArena(Path.Combine(Paths.PluginPath, "DouQuQu/arena.json"));
            }
            if (!stopSpawn || board == null)
                return;

            // Freeze wave countdown so no new zombies spawn
            board.hugeWaveCountDown = 9999f;
            board.theSun=50000;
            board.theMoney=99999;
            board.GetMoney(1f);
            zcount=Lawnf.GetAllZombies().Count;
            if(zcount==0&&displayed==false&&isLoading==false)
            {
                pscore++;
                if(displayscore){
                    if (altLang)
                    {
                        InGameText.Instance?.ShowText($"植物胜利! 植物: {pscore}   僵尸: {zscore}",3f);
                    }
                    else
                    {
                        InGameText.Instance?.ShowText($"Plants win! Plants: {pscore}   Zombies: {zscore}",3f);
                    }
                }
                if (altLang)
                {
                    LogScore($"植物胜利 | 植物:{pscore} 僵尸:{zscore}");
                }
                else
                {
                    LogScore($"Plants win | P:{pscore} Z:{zscore}");
                }
                displayed=true;
            }

        }
        public void reset()
        {
            var comp = Board.Instance.GetComponent<DouQuQuMgr>();
            if (comp != null)
                Destroy(comp);
            var dm=Board.Instance.gameObject.AddComponent<DouQuQuMgr>();
            if(!dm.isLoading)dm.LoadArena(Path.Combine(Paths.PluginPath, "DouQuQu/arena.json"));
        }
        public void LogScore(string message)
        {
            string dir = Path.Combine(Paths.PluginPath, "DouQuQu");
            string path = Path.Combine(dir, "scores.log");

            // Ensure folder exists
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Ensure file exists
            if (!File.Exists(path))
                File.WriteAllText(path, "=== DouQuQu Score Log ===\n");

            // Append entry
            File.AppendAllText(path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}\n");
        }

        public void ClearBoard()
        {
            try{
                // Kill all plants
                var plants = new List<Plant>();

                foreach (var item in Lawnf.GetAllPlants())
                {
                    plants.Add(item);
                }

                foreach (var item in plants)
                {
                    item?.Die(Plant.DieReason.ByShovel);
                }
            }catch(NullReferenceException){}
            try{
                // Kill all zombies
                var zombies = new List<Zombie>();

                foreach (var item in Board.Instance.zombieArray)
                {
                    zombies.Add(item);
                }

                foreach (var item in zombies)
                {
                    KillZombieWithMower(item);
                }
            }catch(NullReferenceException){}
            try{
                // Clear all mowers
                var mowers = new List<Mower>();

                foreach (var item in Board.Instance.mowerArray)
                {
                    mowers.Add(item);
                }

                foreach (var item in mowers)
                {
                    item?.Die();
                }
            }catch(NullReferenceException){}
            try{
                // Kill all grid items
                var items = new List<GridItem>();

                foreach (var item in Board.Instance.griditemArray)
                {
                    items.Add(item);
                }

                foreach (var item in items)
                {
                    item?.Die();
                }
            }catch(NullReferenceException){}
            try{
                // Kill all grid items
                var bullets = new List<Bullet>();

                foreach (var item in Board.Instance.boardEntity.bulletArray)
                {
                    bullets.Add(item);
                }

                foreach (var item in bullets)
                {
                    item?.Die();
                }
            }catch(NullReferenceException){}
            var allSolar = FindObjectsOfType<Solar>();
            foreach (var s in allSolar)
            {
                if (s == null) continue;
                s.deathTime = 0f;   // triggers self-destruction in Update()
            }
            var allLunar = FindObjectsOfType<Lunar>();
            foreach (var l in allLunar)
            {
                if (l == null) continue;
                l.lifeTimer = 0f;
            }
            for(int i = 0; i < 5; i++)
            {
                Board.Instance.boardAction.CreateFireLine(i,0,false,false,false);//fire line to remove ice roads
            }
            
        }
        public ArenaConfig LoadArenaConfig(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ArenaConfig>(json);
        }
        void KillZombieWithMower(Zombie z)
        {
            if (z == null) return;

            int row = z.theZombieRow;
            float x = z.transform.position.x;

            // Spawn mower at zombie position
            var mower = CreateMower._instance.SetMower(MowerType.LawnMower, x, row);

            // Force mower to immediately hit the zombie
            mower.AttackZombie(z);

            // Remove mower so it doesn't sweep the whole lane
            mower.Die();
        }

        public void LoadArena(string path)
        {
            if(isLoading)return;
            isLoading=true;
            // Load JSON
            var config = LoadArenaConfig(path);

            // Clear board
            ClearBoard();

            // Enable stop-spawn
            stopSpawn = true;
            auto=config.Automatic;
            displayscore=config.DisplayScore;

            board.StartCoroutine(delayRead(config));
        }
        public void EnsureArenaFile()
        {
            string dir = Path.Combine(Paths.PluginPath, "DouQuQu");
            string path = Path.Combine(dir, "arena.json");

            // Create folder if missing
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Create file if missing
            if (!File.Exists(path))
            {
                var defaultArena = new ArenaConfig
                {
                    Plants = new List<PlantEntry>
                    {
                        new PlantEntry { Row = 2, Column = 0, type = 256 },
                        new PlantEntry { Row = 2, Column = 1, type = 256 },
                        new PlantEntry { Row = 2, Column = 2, type = 256 },
                        new PlantEntry { Row = 2, Column = 3, type = 256 },
                        new PlantEntry { Row = 2, Column = 4, type = 256 }
                    },
                    Zombies = new List<ZombieEntry>
                    {
                        new ZombieEntry { Row = 2, Column = 5, type = 215 },
                        new ZombieEntry { Row = 2, Column = 6, type = 215 },
                        new ZombieEntry { Row = 2, Column = 7, type = 215 },
                        new ZombieEntry { Row = 2, Column = 8, type = 215 },
                        new ZombieEntry { Row = 2, Column = 9, type = 215 }
                    },
                    FullMower=false,
                    Automatic=true,
                    DisplayScore=true,
                    AltLang=false
                };
                string defaultjson = JsonSerializer.Serialize(defaultArena, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(path, defaultjson);
            }
        }
        public IEnumerator delaySpawn()
        {
            yield return new WaitForSeconds(5f);
            LoadArena(Path.Combine(Paths.PluginPath, "DouQuQu/arena.json"));
        }
        public IEnumerator buffer()
        {
            yield return new WaitForSeconds(1f);
            isLoading=false;
        }
        public IEnumerator delayRead(ArenaConfig config)
        {
            yield return new WaitForSeconds(5f);
            // Spawn mower in lane 3
            CreateMower._instance.SetMowerOnRoad(BoxType.Grass,2);

            // Spawn plants
            foreach (var p in config.Plants)
            {
                CreatePlant.Instance.SetPlant(p.Column, p.Row, (PlantType)p.type,isFreeSet:true);
            }

            // Spawn zombies
            foreach (var z in config.Zombies)
            {
                CreateZombie.Instance.SetZombie(z.Row, (ZombieType)z.type, Mouse.Instance.GetBoxXFromColumn(z.Column));
            }
            if (config.FullMower)
            {
                CreateMower._instance.SetMowerOnRoad(BoxType.Grass,0);
                CreateMower._instance.SetMowerOnRoad(BoxType.Grass,1);
                CreateMower._instance.SetMowerOnRoad(BoxType.Grass,3);
                CreateMower._instance.SetMowerOnRoad(BoxType.Grass,4);
                fullMower=true;
            }
            else
            {
                fullMower=false;
            }
            displayed=false;
            board.StartCoroutine(buffer());
        }
        public void ExportBoard()
        {
            var config = LoadArenaConfig(Path.Combine(Paths.PluginPath, "DouQuQu/arena.json"));
            config.Plants.Clear();
            config.Zombies.Clear();

            // Export plants
            foreach (var p in Lawnf.GetAllPlants())
            {
                if (p == null) continue;

                config.Plants.Add(new PlantEntry
                {
                    Row = p.thePlantRow,
                    Column = p.thePlantColumn,
                    type = (int)p.thePlantType
                });
            }

            // Export zombies
            foreach (var z in Board.Instance.zombieArray)
            {
                if (z == null) continue;

                config.Zombies.Add(new ZombieEntry
                {
                    Row = z.theZombieRow,
                    Column = Mouse.Instance.GetColumnFromX(z.transform.position.x),
                    type = (int)z.theZombieType
                });
            }

                string defaultjson = JsonSerializer.Serialize(config, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(Path.Combine(Paths.PluginPath, "DouQuQu/arena.json"), defaultjson);
        }

    }
    public class DouQuQuMower : MonoBehaviour
    {
        public void Start(){}
    }
    public class ArenaConfig
    {
        public List<PlantEntry> Plants { get; set; }
        public List<ZombieEntry> Zombies { get; set; }
        public bool FullMower { get; set; }
        public bool Automatic { get; set; }
        public bool DisplayScore { get; set; }
        public bool AltLang { get; set; }
    }

    public class PlantEntry
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int type { get; set; }
    }

    public class ZombieEntry
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int type { get; set; }
    }

    [HarmonyPatch(typeof(GameLose), "OnTriggerEnter2D")]
    public class GameLoseOnTriggerEnter2DPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(GameLose __instance, Collider2D collision)
        {
            // If no collider, let the original run
            if (collision == null)
                return true;

            // Get the GameObject that entered the lose trigger
            var go = collision.gameObject;
            if (go == null)
                return true;

            // Native code checks CompareTag("Zombie")
            if (!go.CompareTag("Zombie"))
                return true;

            // Get the Zombie component exactly like the engine does
            var zombie = collision.GetComponent<Zombie>();
            if (zombie == null)
                return true;

            // Now you have the zombie component and can branch on it
            var board = Board.Instance;
            if (board != null && board.TryGetComponent<DouQuQuMgr>(out var douQuQu))
            {
                if(zombie.theZombieRow==2||douQuQu.fullMower) return false;
                if(douQuQu.displayed)return false;
                douQuQu.displayed=true;
                douQuQu.zscore++;
                if(douQuQu.displayscore&&douQuQu.isLoading==false){
                    if (douQuQu.altLang)
                    {
                        InGameText.Instance?.ShowText($"僵尸胜利! 植物: {douQuQu.pscore}   僵尸: {douQuQu.zscore}",3f);
                    }
                    else
                    {
                        InGameText.Instance?.ShowText($"Zombies win! Plants: {douQuQu.pscore}   Zombies: {douQuQu.zscore}",3f);
                    }
                }
                if (douQuQu.altLang)
                {
                    douQuQu.LogScore($"僵尸胜利 | 植物:{douQuQu.pscore} 僵尸:{douQuQu.zscore}");
                }
                else
                {
                    douQuQu.LogScore($"Zombies win | P:{douQuQu.pscore} Z:{douQuQu.zscore}");
                }
                if (!douQuQu.auto) return false;
                douQuQu.LoadArena(Path.Combine(Paths.PluginPath, "DouQuQu/arena.json"));
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(Mower), nameof(Mower.AttackZombie))]
    public class MowerAttackZombiePatch
    {
        [HarmonyPostfix]
        public static void Postfix(Mower __instance, Zombie zombie)
        {
            var board = Board.Instance;
            if (board == null) return;

            if (!board.TryGetComponent<DouQuQuMgr>(out var douQuQu)) return;
            if (!board.TryGetComponent<DouQuQuMower>(out var _)) return;
            if(douQuQu.displayed)return;
            douQuQu.displayed=true;
            douQuQu.zscore++;
            if(douQuQu.displayscore&&douQuQu.isLoading==false){
                if (douQuQu.altLang)
                {
                    InGameText.Instance?.ShowText($"僵尸胜利! 植物: {douQuQu.pscore}   僵尸: {douQuQu.zscore}",3f);
                }
                else
                {
                    InGameText.Instance?.ShowText($"Zombies win! Plants: {douQuQu.pscore}   Zombies: {douQuQu.zscore}",3f);
                }
            }
            if (douQuQu.altLang)
            {
                douQuQu.LogScore($"僵尸胜利 | 植物:{douQuQu.pscore} 僵尸:{douQuQu.zscore}");
            }
            else
            {
                douQuQu.LogScore($"Zombies win | P:{douQuQu.pscore} Z:{douQuQu.zscore}");
            }
            if (!douQuQu.auto) return;

            __instance.Die();
            douQuQu.LoadArena(Path.Combine(Paths.PluginPath, "DouQuQu/arena.json"));
        }
    }

    [HarmonyPatch(typeof(CreatePlant), nameof(CreatePlant.SetPlant))]
    public class PlantStartPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __result)
        {
            if(__result==null) return;
            if(!__result.TryGetComponent<Plant>(out var plant)) return;
            var board = plant.board;
            if (board == null) return;

            if(!board.TryGetComponent<DouQuQuMgr>(out _)) return;

            plant.StarUp();
        }
    }
    /*
    [HarmonyPatch(typeof(InitBoard), nameof(InitBoard.ReadySetPlant))]
    public class ReadySetPlantPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            var board = Board.Instance;
            if (board == null) return;

            if (!board.TryGetComponent<DouQuQuMgr>(out var douQuQu)) return;
            douQuQu.reset();
        }
    }
    */
    [HarmonyPatch(typeof(UIMgr), "EnterLoseMenu")]
    public static class UIMgr_EnterLoseMenu_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            var board = Board.Instance;
            if (board != null && board.TryGetComponent<DouQuQuMgr>(out var douQuQu))
            {
                if(douQuQu.displayed)return false;
                douQuQu.displayed=true;
                douQuQu.zscore++;
                if(douQuQu.displayscore&&douQuQu.isLoading==false){
                    if (douQuQu.altLang)
                    {
                        InGameText.Instance?.ShowText($"僵尸胜利! 植物: {douQuQu.pscore}   僵尸: {douQuQu.zscore}",3f);
                    }
                    else
                    {
                        InGameText.Instance?.ShowText($"Zombies win! Plants: {douQuQu.pscore}   Zombies: {douQuQu.zscore}",3f);
                    }
                }
                if (douQuQu.altLang)
                {
                    douQuQu.LogScore($"僵尸胜利 | 植物:{douQuQu.pscore} 僵尸:{douQuQu.zscore}");
                }
                else
                {
                    douQuQu.LogScore($"Zombies win | P:{douQuQu.pscore} Z:{douQuQu.zscore}");
                }
                if (!douQuQu.auto) return false;
                douQuQu.LoadArena(Path.Combine(Paths.PluginPath, "DouQuQu/arena.json"));
            }

            return true; // allow normal lose
        }
    }
}
