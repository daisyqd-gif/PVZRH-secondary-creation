#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace CustomPlantClass.Main
{
    /// <summary>
    /// Central utility manager for all custom plant registration,
    /// fusion helpers, skin helpers, ID allocation, and IL2CPP type registration.
    /// This class is the backbone of the new custom plant framework.
    /// </summary>
    public sealed class DataMgr : MonoBehaviour
    {
        #region Fields
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static HashSet<int> ID_List = new();
        public static int CustomPlantCount = 0;
        public static int CustomSkinCount = 0;
        public static int CustomBigStarCount = 0;
        public static int CustomBulletCount = 0;
        public static Dictionary<int, CardLevel> CustomCardLevel = new();
        public static Dictionary<PlantType, (PlantType, Func<Plant, bool>)> replaceList = new();
        public static Dictionary<PlantType, BulletType> plantBulletTypes = new();
        public static Dictionary<ZombieType, (ZombieType, Func<Zombie, bool>)> onZombieTypeSpawnActionList = new();
        public static Dictionary<ZombieType, GameObject> bossHealthSliders = new();
        public static Dictionary<ZombieType, BaseCustomZombieData> zombieDatas = new();
        public static Dictionary<ZombieType, (int, int)> CustomZombieSpawns = new();
        public static Dictionary<int, Type> CustomLevelComponents = new();
        public static HashSet<BaseCustomLevelData> LoadedCustomLevels = new();
        public static HashSet<int> UsedLevelIDs = new();
        public static HashSet<int> CustomStarUps = new();
        public static HashSet<int> CustomGridItemTypes = new();
        public static HashSet<PlantType> RegisteredPlants = new();
        public static List<string> StartUpMessages = new();
        public static List<string> StartUpErrors = new();
        public static List<string> StartUpWarnings = new();
        public static List<Action> GameStartActions = new();
        public static List<Action> GameAppInitActions = new();
        public static Dictionary<(LevelType, int), List<ZombieType>> AddedZombiesInLevel = new();
        public static bool IsGameStarted = false;
        static readonly string FreezePath = Path.Combine(Paths.ConfigPath, "IDFreeze.json");
        static Dictionary<string, int> FreezeTable = new();
        static bool FreezeLoaded = false;
        static readonly Dictionary<string, int> GuidCallIndex = new();
        static readonly JsonSerializerOptions FreezeJsonOptions = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true
        };
        internal static HashSet<PlantType> CustomUltiPlants = new();
        public static Dictionary<int, (BuffID buff, TravelUnlocks unlock, string desc)> CustomStrongUltiPlants = new();
        public static Dictionary<int, int> UnlockValueToPlantType = new();
        public static HashSet<TravelUnlocks> CustomTravelUnlocks = new();
        public static Dictionary<PlantType, (PlantType?, UltiBuff, UltiBuff)> CustomStrongUltimateInfo = new();
        public static Dictionary<ZombieType, ZombieType> Level4Zombies = new();
        public static Dictionary<int, bool> CustomWeakUltiPlants = new();
        #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion
        /// <summary>
        /// Initializes startup logging and adds the manager component
        /// to the plugin's game object.
        /// </summary>
        [OnLoad]
        public static void OnLoad()
        {
            StartUpMessages.Add($"Thank you for using {MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion}!");
            PluginBehaviour.AddComponentToPlugin<DataMgr>();
        }
        ////*
        #region ID Allocation
        // ---------------------------------------------------------
        //  ID ALLOCATION
        // ---------------------------------------------------------

        static void LoadFreezeTable()
        {
            if (FreezeLoaded) return;

            try
            {
                if (File.Exists(FreezePath))
                {
                    string json = File.ReadAllText(FreezePath);
                    FreezeTable = JsonSerializer.Deserialize<Dictionary<string, int>>(json, FreezeJsonOptions)
                                ?? new Dictionary<string, int>();
                }
            }
            catch (Exception e)
            {
                ModLogger.LogError($"[IDFreeze] Failed to load freeze table: {e}");
                FreezeTable = new Dictionary<string, int>();
            }

            FreezeLoaded = true;
        }

        static void SaveFreezeTable()
        {
            try
            {
                string? dir = Path.GetDirectoryName(FreezePath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                string json = JsonSerializer.Serialize(FreezeTable, FreezeJsonOptions);
                File.WriteAllText(FreezePath, json);
            }
            catch (Exception e)
            {
                ModLogger.LogError("[IDFreeze]", $"Failed to save freeze table: {e}");
            }
        }

        static Assembly? ResolveModAssembly()
        {
            // 1. Fast path: direct callers (Fireworks, etc.)
            var calling = Assembly.GetCallingAssembly();
            if (calling != null &&
                calling != typeof(DataMgr).Assembly &&
                calling.GetCustomAttribute<BepInPlugin>() != null)
            {
                return calling;
            }

            // 2. Fallback: walk the stack for any assembly with BepInPlugin
            var st = new StackTrace();
            var frames = st.GetFrames();
            if (frames != null)
            {
                foreach (var frame in frames)
                {
                    var type = frame.GetMethod()?.DeclaringType;
                    if (type == null) continue;

                    var asm = type.Assembly;
                    if (asm.GetCustomAttribute<BepInPlugin>() != null)
                        return asm;
                }
            }

            // 3. Unknown
            return null;
        }

        // === DROP‑IN REPLACEMENT ===
        /// <summary>
        /// Allocates a deterministic ID for the calling mod
        /// while avoiding collisions with built-in and custom IDs.
        /// </summary>
        public static ID AllocateID()
        {
            LoadFreezeTable();

            // 1. Resolve mod identity
            Assembly? asm = ResolveModAssembly();

            string guidBase;

            if (asm != null)
            {
                var pluginAttr = asm.GetCustomAttribute<BepInPlugin>();
                guidBase = pluginAttr?.GUID ?? asm.FullName!;
            }
            else
            {
                // Last‑resort fallback: calling assembly identity
                var calling = Assembly.GetCallingAssembly();
                guidBase = calling?.FullName ?? "__IDFREEZE_UNKNOWN__";
            }

            // 2. Multi‑ID‑per‑mod: assign per‑mod call index
            if (!GuidCallIndex.TryGetValue(guidBase, out int index))
                index = 0;

            string freezeKey = $"{guidBase}::{index}";
            GuidCallIndex[guidBase] = index + 1;

            // 3. If this specific call already has a frozen ID, return it
            if (FreezeTable.TryGetValue(freezeKey, out int frozen))
                return frozen;

            // 4. Build used set (your original logic)
            HashSet<int> used = new();

            foreach (PlantType pt in Enum.GetValues(typeof(PlantType)))
                used.Add((int)pt);
            foreach (BulletType bt in Enum.GetValues(typeof(BulletType)))
                used.Add((int)bt);
            foreach (ZombieType zt in Enum.GetValues(typeof(ZombieType)))
                used.Add((int)zt);
            foreach (ParticleType pt in Enum.GetValues(typeof(ParticleType)))
                used.Add((int)pt);
            foreach (CherryBombType dt in Enum.GetValues(typeof(CherryBombType)))
                used.Add((int)dt);
            foreach (GridItemType gt in Enum.GetValues(typeof(GridItemType)))
                used.Add((int)gt);
            foreach (int id in CustomGridItemTypes)
                used.Add(id);

            foreach (var id in CustomCore.CustomPlantTypes)
                used.Add((int)id);
            foreach (var id in CustomCore.CustomZombieTypes)
                used.Add((int)id);
            foreach (var id in CustomCore.CustomBullets.Keys)
                used.Add((int)id);
            foreach (var id in CustomCore.CustomParticles.Keys)
                used.Add((int)id);
            foreach (var id in CustomCore.CustomCherrys.Keys)
                used.Add((int)id);
            //customcore does not have grid items

            foreach (var id in ID_List)
                used.Add(id);

            // 5. Deterministic base ID from freezeKey, then +1 until free
            int baseId = Math.Abs(freezeKey.GetHashCode()) % 50000 + 10000;
            int candidate = baseId;
            while (used.Contains(candidate))
                candidate++;

            // 6. Freeze and return
            FreezeTable[freezeKey] = candidate;
            SaveFreezeTable();

            ID_List.Add(candidate);
            return candidate;
        }

        // Wrappers
        /// <summary>
        /// Allocates an ID through <see cref="AllocateID"/>.
        /// This legacy plant-specific wrapper is obsolete.
        /// </summary>
        [Obsolete("AllocatePlantID is deprecated. Use AllocateID.", false)]
        public static ID AllocatePlantID() => AllocateID();
        /// <summary>
        /// Allocates an ID through <see cref="AllocateID"/>.
        /// This legacy zombie-specific wrapper is obsolete.
        /// </summary>
        [Obsolete("AllocateZombieID is deprecated. Use AllocateID.", false)]
        public static ID AllocateZombieID() => AllocateID();
        /// <summary>
        /// Allocates an ID through <see cref="AllocateID"/>.
        /// This legacy bullet-specific wrapper is obsolete.
        /// </summary>
        [Obsolete("AllocateBulletID is deprecated. Use AllocateID.", false)]
        public static ID AllocateBulletID() => AllocateID();/**/

        #endregion
        #region Fusion Helpers

        // ---------------------------------------------------------
        //  FUSION HELPERS
        // ---------------------------------------------------------

        /// <summary>
        /// Returns the original fusion pair and its mirrored version
        /// with the pair elements in reversed order.
        /// </summary>
        public static List<(ID, ID)> MirrorTuple((ID, ID) input) => ListHelper.MirrorTuple(input);

        /// <summary>
        /// Flattens an array of fusion lists into one list
        /// while preserving the order of the pairs.
        /// </summary>
        public static List<(ID, ID)> FlattenFusionArray(List<(ID, ID)>[] input) => ListHelper.FlattenFusionArray(input);

        /// <summary>
        /// Mirrors every fusion pair in a list
        /// by reversing the order of each pair's elements.
        /// </summary>
        public static List<(ID, ID)> MirrorList(List<(ID, ID)> input) => ListHelper.MirrorList(input);

        /// <summary>
        /// Removes duplicate fusion pairs
        /// from the supplied collection.
        /// </summary>
        public static List<(ID, ID)> DeduplicateFusions(List<(ID, ID)> input) => ListHelper.DeduplicateFusions(input);

        /// <summary>
        /// Creates a fusion list from simple pair definitions
        /// and includes the mirrored form of each pair.
        /// </summary>
        public static List<(ID, ID)> Fusion(params (ID, ID)[] pairs) => ListHelper.Fusion(pairs);

        #endregion
        #region Bullet Skin

        // ---------------------------------------------------------
        //  BULLET SKIN HELPERS
        // ---------------------------------------------------------

        /// <summary>
        /// Creates bullet skin mappings for plant skins
        /// from the supplied bullet and prefab entries.
        /// </summary>
        public static List<(BulletType, List<GameObject?>)> BulletSkin(params (BulletType, GameObject?[])[] entries)
        {
            var result = new List<(BulletType, List<GameObject?>)>();
            foreach (var e in entries)
                result.Add((e.Item1, [.. e.Item2]));
            return result;
        }
        #endregion
        #region Boss Slider
        /// <summary>
        /// Registers a custom boss health slider using a prefab
        /// after validating its required UI components.
        /// </summary>
        public static void RegisterCustomBossHealthSlider(ID zombieType, GameObject slider)
        {
            EnsureGameNotStarted();
            if (slider == null)
            {
                ModLogger.LogError($"Boss slider prefab for {zombieType} is null.");
                return;
            }

            // Must have RectTransform
            if (slider.GetComponent<RectTransform>() == null)
            {
                ModLogger.LogError($"Boss slider prefab for {zombieType} must have a RectTransform.");
                return;
            }

            // Must have Fill Area/Back
            var back = slider.transform.Find("Fill Area/Back");
            if (back == null || back.GetComponent<Image>() == null)
            {
                ModLogger.LogError($"Boss slider prefab for {zombieType} is missing Fill Area/Back with an Image component.");
                return;
            }

            // Slider optional — but validate if present
            var slider2 = slider.GetComponent<Slider>();
            if (slider2 != null)
            {
                // Validate required fields
                if (slider2.fillRect == null)
                    ModLogger.LogWarn($"Slider on prefab for {zombieType} has no Fill Rect. It will be auto‑assigned at runtime.");

                if (slider2.handleRect == null)
                    ModLogger.LogWarn($"Slider on prefab for {zombieType} has no Handle Rect. It will be auto‑assigned at runtime.");
            }
            if (bossHealthSliders.ContainsKey(zombieType))
            {
                ModLogger.LogError("Duplicate zombie type : " + zombieType);
            }
            bossHealthSliders[zombieType] = slider;
        }
        /// <summary>
        /// Registers a custom boss health slider from metadata
        /// and applies its icon and fill appearance.
        /// </summary>
        public static void RegisterCustomBossHealthSlider(CustomBossHealthSliderData data)
        {
            Action a = () =>
            {
                var prefab = Plugin.assetBundle?.GetAsset<GameObject>("HealthSlider");
                if (prefab == null)
                {
                    ModLogger.LogError("HealthSlider prefab not found in asset bundle.");
                    return;
                }

                GameObject gameObject = Instantiate(prefab);

                var fill = gameObject.transform.Find("Fill Area/Fill")?.GetComponent<Image>();
                if (fill != null)
                {
                    if (data.FillIcon != null)
                    {
                        fill.sprite = data.FillIcon;
                    }
                    else
                    {
                        fill.color = data.FillColor;
                    }
                }

                var icon = gameObject.transform.Find("IconBank/Icon")?.GetComponent<Image>();
                if (icon != null) icon.sprite = data.Icon;

                RegisterCustomBossHealthSlider(data.theZombieType, gameObject);
            };
            AddGameStartAction(a);
        }
        #endregion
        #region Data Builder

        // ---------------------------------------------------------
        //  DATA BUILDERS
        // ---------------------------------------------------------

        /// <summary>
        /// Creates plant registration data with default settings
        /// and the supplied ID, prefab, and preview.
        /// </summary>
        public static BaseCustomPlantData CreatePlantData(ID id, GameObject prefab, GameObject preview)
        {
            return new BaseCustomPlantData
            {
                PlantId = id,
                Prefab = prefab,
                Preview = preview,
                Fusions = new List<(ID, ID)>(),
                AttackInterval = 0f,
                ProduceInterval = 0f,
                AttackDamage = 0,
                MaxHealth = 300,
                Cd = 0f,
                Sun = 0,
                DefaultBullet = BulletType.Bullet_pea,
                CanPF = false,
                CanStarUp = false,
                CardColor = CardLevel.White,
                IsRainbowCard = false,
                IsUltimatePlant = false,
                CardRepeatAmt = 1,
                Name = "",
                AlmanacEntry = ""
            };
        }

        /// <summary>
        /// Creates plant skin data from the base plant data
        /// and the supplied skin prefab and preview.
        /// </summary>
        public static BasePlantSkinData CreateSkin(BaseCustomPlantData data, GameObject skinPrefab, GameObject skinPreview)
        {
            return new BasePlantSkinData
            {
                data = data,
                SkinPrefab = skinPrefab,
                SkinPreview = skinPreview,
                BulletSkinList = new List<(BulletType, List<GameObject?>)>()
            };
        }
        #endregion
        #region Il2cpp Types

        // ---------------------------------------------------------
        //  IL2CPP TYPE REGISTRATION
        // ---------------------------------------------------------

        /// <summary>
        /// Registers all eligible derived types from the calling assembly
        /// with the IL2CPP class injector.
        /// </summary>
        public static void AutoRegisterTypes() => AutoRegisterTypes(Assembly.GetCallingAssembly());
        /// <summary>
        /// Registers eligible derived types from an assembly
        /// with the IL2CPP class injector.
        /// </summary>
        public static void AutoRegisterTypes([NotNull] Assembly asm)
        {
            foreach (var type in SystemTools.GetAllDerivedTypes<Il2CppSystem.Object>(asm, containBase: false))
            {
                if (!type.IsAbstract && !ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                {
                    ClassInjector.RegisterTypeInIl2Cpp(type);
                }
            }
        }

        /// <summary>
        /// Ensures the specified custom component type
        /// is registered with the IL2CPP class injector.
        /// </summary>
        public static void EnsureTypeRegistered<TClass>() where TClass : MonoBehaviour
        {
            var type = typeof(TClass);
            if (!ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                ClassInjector.RegisterTypeInIl2Cpp(type);
        }
        #endregion
        #region Plants

        // ---------------------------------------------------------
        //  PLANT REGISTRATION
        // ---------------------------------------------------------

        /// <summary>
        /// Registers a custom plant using BaseCustomPlantData.
        /// Automatically registers TClass in IL2CPP.
        /// </summary>
        internal class PlantTagAdder : MonoBehaviour
        {
            Plant plant => GetComponent<Plant>();
            /// <summary>
            /// Applies the plant's registered tag on the first game update,
            /// then removes this temporary helper component.
            /// </summary>
            public void Update()
            {
                if (GameAPP.theGameStatus != GameStatus.InGame) return;
                TypeMgr.GetPlantTag(plant);
                Destroy(this);
            }
        }
        /// <summary>
        /// Registers a custom plant using BaseCustomPlantData.
        /// Automatically registers TClass in IL2CPP.
        /// </summary>
        public static ID RegisterCustomPlant<TBase, TClass>(BaseCustomPlantData data)
            where TBase : Plant
            where TClass : MonoBehaviour
        {
            EnsureGameNotStarted();
            EnsureTypeRegistered<TClass>(); // Auto IL2CPP registration

            var fusions = data.Fusions?
                .ConvertAll(p => ((int)p.Item1, (int)p.Item2))
                ?? new List<(int, int)>();
            if (RegisteredPlants.Contains(data.PlantId))
            {
                throw new InvalidOperationException($"Duplicate Plant ID {(int)data.PlantId}");
            }
            RegisteredPlants.Add(data.PlantId);
            /*
            CustomCore.RegisterCustomPlant<TBase, TClass>(
                data.PlantId,
                data.Prefab,
                data.Preview,
                fusions,
                data.AttackInterval,
                data.ProduceInterval,
                data.AttackDamage,
                data.MaxHealth,
                data.Cd,
                data.Sun
            );
            */
            AddGameStartAction(delegate
            {
                //Prefab
                Il2CppSystem.Collections.Generic.List<GameObject> lst = new();
                GameAPP.resourcesManager.plantPrefabs.Add(data.PlantId, data.Prefab);
                GameAPP.resourcesManager.plantPrefabs[data.PlantId].AddComponent<TClass>();
                GameAPP.resourcesManager.plantPrefabs[data.PlantId].AddComponent<PlantTagAdder>();
                lst.Add(GameAPP.resourcesManager.plantPrefabs[data.PlantId]);
                GameAPP.resourcesManager._plantPrefabs.Add(data.PlantId, lst);
                GameAPP.resourcesManager.allPlants.Add(data.PlantId);
                TBase plant = GameAPP.resourcesManager.plantPrefabs[data.PlantId].AddComponent<TBase>();
                plant.thePlantType = data.PlantId;
                plant.tag = "Plant";
                plant.gameObject.layer = LayerMask.NameToLayer("Plant");
                //Preview
                Il2CppSystem.Collections.Generic.List<GameObject> lst2 = new();
                GameAPP.resourcesManager.plantPreviews.Add(data.PlantId, data.Preview);
                GameAPP.resourcesManager.plantPreviews[data.PlantId].tag = "Preview";
                lst2.Add(GameAPP.resourcesManager.plantPreviews[data.PlantId]);
                GameAPP.resourcesManager._plantPreviews.Add(data.PlantId, lst2);
                //Data
                PlantDataManager.PlantData_Default[data.PlantId] = new()
                {
                    thePlantType = data.PlantId,
                    maxHealth = data.MaxHealth,
                    cost = data.Sun,
                    attackInterval = data.AttackInterval,
                    produceInterval = data.ProduceInterval,
                    cd = data.Cd,
                    attackDamage = data.AttackDamage
                };
                PlantDataManager.unlocked.Add(data.PlantId, true);
                if (data.Fusions != null)
                    foreach (var recipe in data.Fusions)
                    {
                        MixData.AddOrderedRecipe(recipe.Item1, recipe.Item2, data.PlantId);
                    }
            });

            if (data.CanPF)
            {
                CustomCore.RegisterSuperSkill(
                    data.PlantId,
                    (Plant p) => 1000,
                    (Plant p) =>
                    {
                        if (p.TryGetInterface<ICustomPF>(out var plant))
                            plant.StartPF();
                    }
                );
            }

            if (data.CanStarUp)
                CustomStarUps.Add(data.PlantId);

            if (data.IsRainbowCard)
                CustomCore.RegisterCustomCardToColorfulCards(data.PlantId, data.CardRepeatAmt);

            if (data.IsUltimatePlant)
                AddCustomUltiPlant(data.PlantId);

            plantBulletTypes.TryAdd(data.PlantId, data.DefaultBullet);

            CustomCore.TypeMgrExtra.IsCustomPlant.Add(data.PlantId);
            AddLevelPlant(data.PlantId, data.CardColor);
            CustomCore.AddPlantAlmanacStrings(data.PlantId, data.Name, data.AlmanacEntry);
            CustomPlantCount++;

            return data.PlantId;
        }
        /// <summary>
        /// Registers a custom plant using BaseCustomPlantData.
        /// Automatically registers TClass in IL2CPP.
        /// </summary>
        public static ID RegisterCustomPlant<TBase>(BaseCustomPlantData data)
            where TBase : Plant
        {
            EnsureGameNotStarted();

            var fusions = data.Fusions?
                .ConvertAll(p => ((int)p.Item1, (int)p.Item2))
                ?? new List<(int, int)>();
            if (RegisteredPlants.Contains(data.PlantId))
            {
                throw new InvalidOperationException($"Duplicate Plant ID {(int)data.PlantId}");
            }
            RegisteredPlants.Add(data.PlantId);
            /*
            CustomCore.RegisterCustomPlant<TBase, TClass>(
                data.PlantId,
                data.Prefab,
                data.Preview,
                fusions,
                data.AttackInterval,
                data.ProduceInterval,
                data.AttackDamage,
                data.MaxHealth,
                data.Cd,
                data.Sun
            );
            */
            AddGameStartAction(delegate
            {
                //Prefab
                Il2CppSystem.Collections.Generic.List<GameObject> lst = new();
                GameAPP.resourcesManager.plantPrefabs.Add(data.PlantId, data.Prefab);
                GameAPP.resourcesManager.plantPrefabs[data.PlantId].AddComponent<PlantTagAdder>();
                lst.Add(GameAPP.resourcesManager.plantPrefabs[data.PlantId]);
                GameAPP.resourcesManager._plantPrefabs.Add(data.PlantId, lst);
                GameAPP.resourcesManager.allPlants.Add(data.PlantId);
                TBase plant = GameAPP.resourcesManager.plantPrefabs[data.PlantId].AddComponent<TBase>();
                plant.thePlantType = data.PlantId;
                plant.tag = "Plant";
                plant.gameObject.layer = LayerMask.NameToLayer("Plant");
                //Preview
                Il2CppSystem.Collections.Generic.List<GameObject> lst2 = new();
                GameAPP.resourcesManager.plantPreviews.Add(data.PlantId, data.Preview);
                GameAPP.resourcesManager.plantPreviews[data.PlantId].tag = "Preview";
                lst2.Add(GameAPP.resourcesManager.plantPreviews[data.PlantId]);
                GameAPP.resourcesManager._plantPreviews.Add(data.PlantId, lst2);
                //Data
                PlantDataManager.PlantData_Default[data.PlantId] = new()
                {
                    thePlantType = data.PlantId,
                    maxHealth = data.MaxHealth,
                    cost = data.Sun,
                    attackInterval = data.AttackInterval,
                    produceInterval = data.ProduceInterval,
                    cd = data.Cd,
                    attackDamage = data.AttackDamage
                };
                PlantDataManager.unlocked.Add(data.PlantId, true);
                if (data.Fusions != null)
                    foreach (var recipe in data.Fusions)
                    {
                        MixData.AddOrderedRecipe(recipe.Item1, recipe.Item2, data.PlantId);
                    }
            });

            if (data.CanPF)
            {
                CustomCore.RegisterSuperSkill(
                    data.PlantId,
                    (Plant p) => 1000,
                    (Plant p) =>
                    {
                        if (p.TryGetInterface<ICustomPF>(out var plant))
                            plant.StartPF();
                    }
                );
            }

            if (data.CanStarUp)
                CustomStarUps.Add(data.PlantId);

            if (data.IsRainbowCard)
                CustomCore.RegisterCustomCardToColorfulCards(data.PlantId, data.CardRepeatAmt);

            if (data.IsUltimatePlant)
                AddCustomUltiPlant(data.PlantId);

            plantBulletTypes.TryAdd(data.PlantId, data.DefaultBullet);

            CustomCore.TypeMgrExtra.IsCustomPlant.Add(data.PlantId);
            AddLevelPlant(data.PlantId, data.CardColor);
            CustomCore.AddPlantAlmanacStrings(data.PlantId, data.Name, data.AlmanacEntry);
            CustomPlantCount++;

            return data.PlantId;
        }
        /// <summary>
        /// Adds a plant type to the custom ultimate plant set
        /// so it is recognized as an ultimate plant.
        /// </summary>
        public static void AddCustomUltiPlant(PlantType thePlantType) => CustomUltiPlants.Add(thePlantType);
        /// <summary>
        /// Registers a custom plant and its skin in one call.
        /// Automatically registers TClass in IL2CPP.
        /// </summary>
        public static ID RegisterCustomPlant<TBase, TClass>(BasePlantSkinData skinData)
            where TBase : Plant
            where TClass : MonoBehaviour
        {
            EnsureTypeRegistered<TClass>(); // Auto IL2CPP registration

            ID id = RegisterCustomPlant<TBase, TClass>(skinData.data);
            RegisterCustomPlantSkin<TBase, TClass>(skinData);
            return id;
        }

        /// <summary>
        /// Registers a custom plant skin.
        /// Automatically registers TClass in IL2CPP.
        /// </summary>
        public static void RegisterCustomPlantSkin<TBase, TClass>(BasePlantSkinData skin)
            where TBase : Plant
            where TClass : MonoBehaviour
        {
            EnsureTypeRegistered<TClass>(); // Auto IL2CPP registration

            var data = skin.data;

            var fusions = data.Fusions?
                .ConvertAll(p => ((int)p.Item1, (int)p.Item2))
                ?? new List<(int, int)>();

            CustomCore.RegisterCustomPlantSkin<TBase, TClass>(
                data.PlantId,
                skin.SkinPrefab,
                skin.SkinPreview,
                fusions,
                data.AttackInterval,
                data.ProduceInterval,
                data.AttackDamage,
                data.MaxHealth,
                data.Cd,
                data.Sun,
                skin.BulletSkinList
            );
            CustomSkinCount++;
        }
        #endregion
        #region Plant Helper
        /// <summary>
        /// Assigns a card level to a plant type
        /// for use in treasure mode.
        /// </summary>
        public static void AddLevelPlant(ID type, CardLevel level)
        {
            int type_internal = type;
            if (CustomCardLevel.ContainsKey(type_internal)) ModLogger.LogError(MyPluginInfo.PluginName, "Duplicate ID type: " + type_internal);
            CustomCardLevel.Add(type_internal, level);
        }
        /// <summary>
        /// Maps a plant progression level to its card level
        /// for use in card and treasure systems.
        /// </summary>
        public static CardLevel GetCardLevel(PlantLevelData data) =>
            data switch
            {
                PlantLevelData.Basic => CardLevel.White,
                PlantLevelData.Secondary => CardLevel.Green,
                PlantLevelData.Super => CardLevel.Blue,
                PlantLevelData.WeakUltimate => CardLevel.Purple,
                PlantLevelData.StrongUltimate => CardLevel.Gold,
                PlantLevelData.FinalUltimate => CardLevel.Gold,
                PlantLevelData.TreasurePlant => CardLevel.Red,
                _ => CardLevel.White
            };
        /// <summary>
        /// Builds formatted almanac text for a plant
        /// from its introduction and optional details.
        /// </summary>
        public static string CreateAlmanacEntry(
            string introduction,
            string specialtext = "removeifthisisdefaulted",
            (string, string) recipe = default,
            (int damage, float interval) attackinterval = default,
            (int amount, float interval, string unit) produceinterval = default,
            string[]? specialeffects = null,
            (string, string) variantswitch = default,
            string feature = "removeifthisisdefaulted",
            string creator = "removeifthisisdefaulted",
            string usageconditions = "removeifthisisdefaulted",
            string flavor = "removeifthisisdefaulted")
        {
            specialeffects ??= Array.Empty<string>();

            string[] skillNumber =
            {
                "①","②","③","④","⑤","⑥","⑦","⑧","⑨","⑩",
                "⑪","⑫","⑬","⑭","⑮","⑯","⑰","⑱","⑲","⑳"
            };

            var sb = new StringBuilder();

            // Introduction
            sb.AppendLine(introduction);
            sb.AppendLine();

            // Special text (blue highlight)
            if (specialtext != "removeifthisisdefaulted")
            {
                sb.AppendLine($"<color=#0000FF>{specialtext}</color>");
                sb.AppendLine();
            }

            // Creator
            if (creator != "removeifthisisdefaulted")
                sb.AppendLine($"<color=#3D1400>作者：</color><color=red>{creator}</color>");

            // Usage conditions
            if (usageconditions != "removeifthisisdefaulted")
                sb.AppendLine($"<color=#3D1400>使用条件：</color><color=red>{usageconditions}</color>");

            // Recipe (optional for infuseable plants)
            if (recipe != default)
                sb.AppendLine($"<color=#3D1400>融合配方：</color><color=red>{recipe.Item1}+{recipe.Item2}</color>");

            // Variant switch
            if (variantswitch != default)
                sb.AppendLine($"<color=#3D1400>转化配方：</color><color=red>{variantswitch.Item1}←→{variantswitch.Item2}</color>");

            // Attack interval
            if (attackinterval != default)
                sb.AppendLine($"<color=#3D1400>伤害：</color><color=red>{attackinterval.damage}/{attackinterval.interval}秒</color>");

            // Produce interval
            if (produceinterval != default)
                sb.AppendLine($"<color=#3D1400>生产：</color><color=red>{produceinterval.amount}{produceinterval.unit}/{produceinterval.interval}秒</color>");

            // Feature
            if (feature != "removeifthisisdefaulted")
                sb.AppendLine($"<color=#3D1400>特性：</color><color=red>{feature}</color>");

            // Special effects list
            if (specialeffects.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("<color=#3D1400>特点：</color><color=red>");
                for (int i = 0; i < specialeffects.Length; i++)
                {
                    string num = i < skillNumber.Length ? skillNumber[i] : $"({i + 1})";
                    sb.AppendLine($"{num}{specialeffects[i]}");
                }
                sb.AppendLine("</color>");
            }

            // Flavor text
            if (flavor != "removeifthisisdefaulted")
            {
                sb.AppendLine();
                sb.AppendLine($"<color=#3D1400>{flavor}</color>");
            }

            return sb.ToString();
        }
        /// <summary>
        /// Registers a chance-based transformation from one plant type
        /// to another using the specified percentage.
        /// </summary>
        public static void AddCustomPlantUpgrade(ID fromType, ID toType, float percentChance)
        {
            percentChance = Mathf.Clamp(percentChance, 0f, 100f);

            // 0% = never, 100% = always, everything else correct
            AddCustomPlantUpgrade(fromType, toType,
                (Plant p) => Random.value <= percentChance / 100f);
        }
        #endregion
        #region Misc Registration

        /// <summary>
        /// Registers a custom big star component
        /// and assigns it to the fog sorting layer.
        /// </summary>
        public static void RegisterCustomBigStar<TClass>(ref GameObject Star) where TClass : CustomBigStar
        {
            EnsureGameNotStarted();
            Star.AddComponent<TClass>();
            Star.GetComponent<SortingGroup>().sortingLayerName = "fog";
            CustomBigStarCount++;
        }

        /// <summary>
        /// Registers a custom bullet type and its prefab
        /// from the supplied bullet data.
        /// </summary>
        public static ID RegisterCustomBullet<TBase, TClass>(BaseCustomBulletData data) where TBase : Bullet where TClass : MonoBehaviour
        {
            EnsureGameNotStarted();
            CustomCore.RegisterCustomBullet<TBase, TClass>(data.BulletId, data.Prefab);
            CustomBulletCount++;
            return data.BulletId;
        }

        /// <summary>
        /// Marks a plant type as supporting Star-Up
        /// by adding it to the custom Star-Up set.
        /// </summary>
        public static void RegisterCustomStarUp(ID thePlantType)
            => CustomStarUps.Add(thePlantType);

        /// <summary>
        /// Registers a plant transformation that succeeds when
        /// the supplied plant-based condition returns true.
        /// </summary>
        public static void AddCustomPlantUpgrade(ID fromType, ID toType, Func<Plant, bool> condition)
            => replaceList.TryAdd(fromType, (toType, condition));

        /// <summary>
        /// Registers a plant transformation that succeeds when
        /// the supplied condition returns true.
        /// </summary>
        public static void AddCustomPlantUpgrade(ID fromType, ID toType, Func<bool> condition)
            => replaceList.TryAdd(fromType, (toType, (Plant p) => condition.Invoke()));

        /// <summary>
        /// Registers a custom zombie from its data
        /// and records its spawn settings.
        /// </summary>
        public static ID RegisterCustomZombie<TBase, TClass>(BaseCustomZombieData data) where TBase : Zombie where TClass : MonoBehaviour
        {
            CustomCore.RegisterCustomZombie<TBase, TClass>(data.theZombieType, data.Prefab, data.Preview, data.theAtackDamage, data.maxHealth, data.theFirstArmorHealth, data.theSecondArmorHealth);
            AddCustomZombieSpawnRatio(data.theZombieType, data.SpawnLevel, data.SpawnWeight);
            zombieDatas.TryAdd(data.theZombieType, data);
            return data.theZombieType;
        }

        /// <summary>
        /// Registers a condition that maps one zombie type to another
        /// when the source zombie spawns.
        /// </summary>
        public static void AddCustomOnZombieSpawnEvent(ID fromType, ID toType, Func<Zombie, bool> condition)
            => onZombieTypeSpawnActionList.TryAdd(fromType, (toType, condition));

        /// <summary>
        /// Queues an action to run when the game starts,
        /// or runs it immediately if the game has already started.
        /// </summary>
        public static void AddGameStartAction(Action action)
        {
            if (IsGameStarted) action();
            else GameStartActions.Add(action);
        }

        /// <summary>
        /// Queues an action for game application initialization
        /// so it can run during the startup sequence.
        /// </summary>
        public static void AddGameAppInitAction(Action action)
        {
            GameAppInitActions.Add(action);
        }
        #endregion
        /// <summary>
        /// Registers a custom strong ultimate plant and marks it as ultimate
        /// when necessary, then creates its unlock buff.
        /// </summary>
        public static void AddCustomStrongUltimatePlant(
            ID thePlantType,
            string BuffDescription,
            UltiBuff buff1,
            UltiBuff buff2,
            BuffBgType bg = default,
            PlantType? theVariantType = null)
        {
            if (!CustomCore.CustomUltimatePlants.Contains(thePlantType))
                AddCustomUltiPlant(thePlantType);
            string desc = BuffDescription;
            int forcedID = CustomCore.CustomBuffStartID + CustomCore.CustomUnlockBuffs.Count;
            CoreTools.InitBuffDic();
            int i = CustomCore.CustomBuffStartID + CustomCore.CustomUnlockBuffs.Count;
            CustomCore.CustomUnlockBuffs.Add(i, (thePlantType, desc, 2000));
            TravelDictionary.unlocksText.Add((TravelUnlocks)i, desc);
            CustomCore.CustomBuffCost.Add((BuffType.UnlockPlant, i), 2000);
            CustomCore.CustomBuffText.Add((BuffType.UnlockPlant, i), desc);
            CustomCore.CustomBuffIcon.Add((BuffType.UnlockPlant, i), thePlantType);
            if (!CustomCore.CustomBuffsBg.ContainsKey((BuffType.UnlockPlant, i)))
                CustomCore.CustomBuffsBg.Add((BuffType.UnlockPlant, i), bg);
            CustomCore.CustomBuffs.Add((BuffType.UnlockPlant, i), (desc, thePlantType, ZombieType.Nothing));
            CustomStrongUltimateInfo.TryAdd(thePlantType, (theVariantType, buff1, buff2));
            //CustomCore.RegisterCustomStrongUltimatePlant(thePlantType, i);
            /*GameAppInitActions.Add(() =>
            {
                TravelDictionary.travelPackages.Add(new TravelPackage
                {
                    type = TravelBuffType.UnlockPlant,
                    buffId = i,
                    plantType = thePlantType,
                    unlock = (TravelUnlocks)i
                });
            });*/
            AddGameAppInitAction(() =>
            {
                //var nullable = default(Il2CppSystem.Nullable<PlantType>);
                //if(theVariantType!=null) nullable = new Il2CppSystem.Nullable<PlantType>(theVariantType.Value);
                // Register mapping
                TravelDictionary.UnlockToPlant[(TravelUnlocks)i] = thePlantType;
                TravelDictionary.PlantToUnlock[thePlantType] = (TravelUnlocks)i;
                TravelDictionary.unlocksText[(TravelUnlocks)i] = desc;

                // Ensure allStrongUltimtePlant is large enough
                var list = TravelDictionary.allStrongUltimtePlant;
                while (list.Count <= i)
                    list.Add(PlantType.Nothing);

                // Assign plant at correct index
                list[i] = thePlantType;
            });
            CustomStrongUltiPlants[(int)thePlantType] = (i, default, desc);
            CustomTravelUnlocks.Add((TravelUnlocks)i);
        }
        /// <summary>
        /// Formats the strong-ultimate upgrade text for an unlocked plant
        /// and its variant, including their parent plant recipes.
        /// </summary>
        public static string FormatSPUpgradeBuff(
            string unlockedPlantName,
            string baseUltimateName,
            string parentPlant2Name,
            string unlockedVariantPlantName,
            string baseVariantUltimateName
        )
        {
            return "<color=red>【SP进化】</color>：\n" +
                $"<nobr>解锁<color=red>{unlockedPlantName}</color>\n" +
                $"{baseUltimateName} + {parentPlant2Name}</nobr>\n" +
                $"<nobr>解锁<color=red>{unlockedVariantPlantName}</color>\n" +
                $"{baseVariantUltimateName} + {parentPlant2Name}</nobr>\n" +
                "继承原转换配方";
        }
        /// <summary>
        /// Formats the unlock text for a strong-ultimate plant and its variant,
        /// including the variant transformation recipe.
        /// </summary>
        public static string FormatStrongUltimateUnlockBuff
        (
            string unlockedPlantName,
            string basePlant1,
            string baseplant2,
            string unlockedVariantPlantName,
            string variantToBase,
            string baseToVariant
        )
        {
            return $"<nobr>解锁<color=red>{unlockedPlantName}</color>\n" +
                    $"{basePlant1}+{baseplant2}</nobr>\n" +
                    $"<nobr>解锁<color=red>{unlockedVariantPlantName}</color></nobr>\n" +
                    $"<nobr>{variantToBase}←→{baseToVariant}</nobr>";

        }
        /// <summary>
        /// Formats the unlock text for a strong-ultimate plant
        /// and its two parent plants.
        /// </summary>
        public static string FormatStrongUltimateUnlockBuff
        (
            string unlockedPlantName,
            string basePlant1,
            string baseplant2
        )
        {
            return $"<nobr>解锁<color=red>{unlockedPlantName}</color>\n" +
                    $"{basePlant1}+{baseplant2}</nobr>";

        }
        /// <summary>
        /// Registers a custom level 4 zombie based on a level 3 type
        /// and assigns its spawn level and weight.
        /// </summary>
        public static ID AddCustomLevel4Zombie(ID theZombieType, ZombieType BaseLevel3)
        {
            Level4Zombies.TryAdd(BaseLevel3, theZombieType);
            AddCustomZombieSpawnRatio(theZombieType, 9, 0);
            return theZombieType;
        }
        /// <summary>
        /// Registers a custom weak ultimate plant and its optional variant
        /// and marks the involved types as ultimate when necessary.
        /// </summary>
        public static void AddCustomWeakUltimatePlant(ID thePlantType, bool isVariant = true, PlantType theVariantType = PlantType.Nothing, Func<bool> canUnlock = null!)
        {
            if (!CustomCore.CustomUltimatePlants.Contains(thePlantType)) AddCustomUltiPlant(thePlantType);
            CustomWeakUltiPlants.Add(thePlantType, false);
            if (isVariant && theVariantType != PlantType.Nothing && !CustomCore.CustomBanMix.ContainsKey(theVariantType))
            {
                CustomWeakUltiPlants.Add((int)theVariantType, true);
                if (canUnlock == null) canUnlock = () => true;
                if (!CustomCore.CustomUltimatePlants.Contains(theVariantType)) AddCustomUltiPlant(theVariantType);
                CustomCore.RegisterCustomBanMix(theVariantType, canUnlock, null, () => InGameText.Instance.ShowText("该配方需要抽取", 3f));
            }
        }
        /// <summary>
        /// Registers a custom weak ultimate plant
        /// and marks its type as ultimate when necessary.
        /// </summary>
        public static void AddCustomWeakUltimatePlant(ID thePlantType, bool isVariant = false)
        {
            if (!CustomCore.CustomUltimatePlants.Contains(thePlantType)) AddCustomUltiPlant(thePlantType);
            CustomWeakUltiPlants.Add(thePlantType, isVariant);
        }
        /*
        public static BuffID AddCustomWeakUltimateBuff(ID thePlantType, string desc, BuffID buff1, BuffID buff2, BuffBgType bg = default)
        {
            BuffID id=Compatibility.CustomCore_Old.RegisterCustomBuff(desc,BuffType.UnlockPlant,()=>Lawnf.TravelAdvanced(buff1) && Lawnf.TravelAdvanced(buff2),2000,thePlantType,1,bg);
            CustomCore.RegisterCustomBanMix(thePlantType, ()=>Utils.EnableTravelPlant() || Lawnf.TravelAdvanced(id), null, )
        }
        */
        /// <summary>
        /// Adds a zombie type to the custom spawn list
        /// for the specified level.
        /// </summary>
        public static void AddZombieToLevel(LevelType theLevelType, int theLevelID, ZombieType theZombieType)
        {
            var key = (theLevelType, theLevelID);
            if (AddedZombiesInLevel.ContainsKey(key))
            {
                AddedZombiesInLevel[key].Add(theZombieType);
            }
            else
            {
                AddedZombiesInLevel[key] = new([theZombieType]);
            }
        }
        #region Grid Items

        /// <summary>
        /// Registers a custom grid item with a specified base type
        /// and custom component class.
        /// </summary>
        public static GridItemType RegisterCustomGridItem<TBase, TClass>(BaseCustomGridItemData data) where TBase : GridItem where TClass : MonoBehaviour
        {
            EnsureGameNotStarted();
            data.Prefab.AddComponent<TClass>();
            return RegisterCustomGridItem<TBase>(data);
        }

        /// <summary>
        /// Registers a custom grid item with a custom component class
        /// and the default grid item base type.
        /// </summary>
        public static GridItemType RegisterCustomGridItemWithType<TClass>(BaseCustomGridItemData data) where TClass : MonoBehaviour
        {
            EnsureGameNotStarted();
            data.Prefab.AddComponent<TClass>();
            return RegisterCustomGridItem(data);
        }

        /// <summary>
        /// Registers a custom grid item from its data
        /// using the default grid item base type.
        /// </summary>
        public static GridItemType RegisterCustomGridItem(BaseCustomGridItemData data)
        {
            EnsureGameNotStarted();
            return RegisterCustomGridItem<GridItem>(data);
        }

        /// <summary>
        /// Registers a custom grid item from its data
        /// using the specified grid item base type.
        /// </summary>
        public static GridItemType RegisterCustomGridItem<TBase>(BaseCustomGridItemData data) where TBase : GridItem
        {
            EnsureGameNotStarted();
            GameObject gameObject = data.Prefab;
            if (!gameObject.TryGetComponent<GridItem>(out _)) gameObject.AddComponent<TBase>();
            GridItemType id = data.type.ToGridItemType();
            AddGameStartAction(() => GameAPP.resourcesManager.gridItemPrefabs[id] = gameObject);
            CustomGridItemTypes.Add(data.type.id);
            return id;
        }
        #endregion
        #region Levels
        /// <summary>
        /// Registers a custom level through the legacy generic API.
        /// Use <see cref="CustomLevelMgr.RegisterCustomLevel{T}(BaseCustomLevelData)"/> instead.
        /// </summary>
        [Obsolete("RegisterCustomLevel is deprecated. Use the new one in CustomPlantClass.Level.CustomLevelMgr.", false)]
        public static int RegisterCustomLevel<T>(BaseCustomLevelData data) where T : CustomLevelComponent => CustomLevelMgr.RegisterCustomLevel<T>(data);
        /// <summary>
        /// Registers a custom level through the legacy API.
        /// Use <see cref="CustomLevelMgr.RegisterCustomLevel(BaseCustomLevelData)"/> instead.
        /// </summary>
        [Obsolete("RegisterCustomLevel is deprecated. Use the new one in CustomPlantClass.Level.CustomLevelMgr.", false)]
        public static int RegisterCustomLevel(BaseCustomLevelData data) => CustomLevelMgr.RegisterCustomLevel(data);
        /// <summary>
        /// Allocates a custom level ID through the legacy API.
        /// Use the level ID allocator in <see cref="CustomLevelMgr"/> instead.
        /// </summary>
        [Obsolete("AllocateLevelID is deprecated. Use the new one in CustomPlantClass.Level.CustomLevelMgr.", false)]
        public static int AllocateLevelID()
        {
            CustomLevelMgr.LevelIDAllocator.LoadFreezeTable();

            // 1. Resolve mod identity
            Assembly? asm = CustomLevelMgr.LevelIDAllocator.ResolveModAssembly();

            string guidBase;

            if (asm != null)
            {
                var pluginAttr = asm.GetCustomAttribute<BepInPlugin>();
                guidBase = pluginAttr?.GUID ?? asm.FullName!;
            }
            else
            {
                var calling = Assembly.GetCallingAssembly();
                guidBase = calling?.FullName ?? "__LEVELIDFREEZE_UNKNOWN__";
            }

            // 2. Multi‑ID‑per‑mod: assign per‑mod call index
            if (!CustomLevelMgr.LevelIDAllocator.GuidCallIndex.TryGetValue(guidBase, out int index))
                index = 0;

            string freezeKey = $"{guidBase}::{index}";
            CustomLevelMgr.LevelIDAllocator.GuidCallIndex[guidBase] = index + 1;

            // 3. If frozen, return it
            if (CustomLevelMgr.LevelIDAllocator.FreezeTable.TryGetValue(freezeKey, out int frozen))
                return frozen;

            // 4. Build used set (custom levels only)
            HashSet<int> used = new();

            foreach (var lvl in LoadedCustomLevels)
                used.Add(lvl.LevelID);

            foreach (var id in UsedLevelIDs)
                used.Add(id);

            // 5. Deterministic base ID from freezeKey
            int baseId = Math.Abs(freezeKey.GetHashCode()) % 50000 + 10000;
            int candidate = baseId;

            while (used.Contains(candidate))
                candidate++;

            // 6. Freeze and return
            CustomLevelMgr.LevelIDAllocator.FreezeTable[freezeKey] = candidate;
            CustomLevelMgr.LevelIDAllocator.SaveFreezeTable();

            //UsedLevelIDs.Add(candidate);
            return candidate;
        }
        #endregion
        /// <summary>
        /// Verifies that registration is still allowed
        /// and throws if the game has already started.
        /// </summary>
        public static void EnsureGameNotStarted()
        {
            if (IsGameStarted)
                throw new InvalidOperationException("Can't do this after game start!");
        }
        /// <summary>
        /// Registers three ultimate zombie tiers without descriptions.
        /// This legacy overload is obsolete.
        /// </summary>
        [Obsolete]
        public static void AddLevelZombie(ZombieType A, ZombieType B, ZombieType C, ZombieType B2 = ZombieType.Nothing, ZombieType C2 = ZombieType.Nothing)
        {
            EnsureGameNotStarted();
            AddGameAppInitAction
            (
                () =>
                {
                    TypeMgr.UltiZombie_level_a.Add(A);
                    TypeMgr.UltiZombie_level_b.Add(B);
                    TypeMgr.UltiZombie_level_c.Add(C);
                    CustomCore.TypeMgrExtra.UltimateZombie.Add(A);
                    CustomCore.TypeMgrExtra.UltimateZombie.Add(B);
                    CustomCore.TypeMgrExtra.UltimateZombie.Add(C);
                    if (B2 != ZombieType.Nothing)
                    {
                        TypeMgr.UltiZombie_level_b.Add(B2);
                        CustomCore.TypeMgrExtra.UltimateZombie.Add(B2);
                    }
                    if (C2 != ZombieType.Nothing)
                    {
                        TypeMgr.UltiZombie_level_c.Add(C2);
                        CustomCore.TypeMgrExtra.UltimateZombie.Add(C2);
                    }
                }
            );
        }
        /// <summary>
        /// Registers ultimate zombie tiers, their descriptions,
        /// and whether they should be included in water levels.
        /// </summary>
        public static void AddLevelZombie(
            ZombieType A,
            string A_Desc,
            ZombieType B,
            string B_Desc,
            ZombieType C,
            string C_Desc,
            bool IsWater,
            ZombieType B2 = ZombieType.Nothing,
            string B2_Desc = "",
            ZombieType C2 = ZombieType.Nothing,
            string C2_Desc = ""
        )
        {
            EnsureGameNotStarted();
            AddGameAppInitAction
            (
                () =>
                {
                    TypeMgr.UltiZombie_level_a.Add(A);
                    TypeMgr.UltiZombie_level_b.Add(B);
                    TypeMgr.UltiZombie_level_c.Add(C);
                    CustomCore.TypeMgrExtra.UltimateZombie.Add(A);
                    CustomCore.TypeMgrExtra.UltimateZombie.Add(B);
                    CustomCore.TypeMgrExtra.UltimateZombie.Add(C);
                    CrisisZombieWindow.ZombieDescriptions.TryAdd(A, A_Desc);
                    CrisisZombieWindow.ZombieDescriptions.TryAdd(B, B_Desc);
                    CrisisZombieWindow.ZombieDescriptions.TryAdd(C, C_Desc);
                    if (IsWater)
                    {
                        TypeMgr.UltieZombie_level_water.Add(A);
                        TypeMgr.UltieZombie_level_water.Add(B);
                        TypeMgr.UltieZombie_level_water.Add(C);
                    }
                    if (B2 != ZombieType.Nothing)
                    {
                        TypeMgr.UltiZombie_level_b.Add(B2);
                        CustomCore.TypeMgrExtra.UltimateZombie.Add(B2);
                        if (IsWater)
                        {
                            TypeMgr.UltieZombie_level_water.Add(B2);
                        }
                        CrisisZombieWindow.ZombieDescriptions.TryAdd(B2, B2_Desc);
                    }
                    if (C2 != ZombieType.Nothing)
                    {
                        TypeMgr.UltiZombie_level_c.Add(C2);
                        CustomCore.TypeMgrExtra.UltimateZombie.Add(C2);
                        if (IsWater)
                        {
                            TypeMgr.UltieZombie_level_water.Add(C2);
                        }
                        CrisisZombieWindow.ZombieDescriptions.TryAdd(C2, C2_Desc);
                    }
                }
            );
        }

        /// <summary>
        /// Formats a rogue zombie description using health and attack
        /// classifications from the corresponding enums.
        /// </summary>
        public static string RogueZombieTextFormatter(string name, int level, RogueZombieHealth healthDesc, RogueZombieAttack attackDesc, string special)
        => RogueZombieTextFormatter
        (
            name,
            level,
            healthDesc switch
            {
                RogueZombieHealth.VeryLow => "很低",
                RogueZombieHealth.Low => "低",
                RogueZombieHealth.Mid => "中",
                RogueZombieHealth.High => "高",
                RogueZombieHealth.VeryHigh => "很高",
                _ => "未知"
            },
            attackDesc switch
            {
                RogueZombieAttack.VeryLow => "很低",
                RogueZombieAttack.Low => "低",
                RogueZombieAttack.Mid => "中",
                RogueZombieAttack.High => "高",
                RogueZombieAttack.VeryHigh => "很高",
                RogueZombieAttack.Crashing => "碾压",
                _ => "未知"
            },
            special
        );

        /// <summary>
        /// Formats a rogue zombie description from explicit health
        /// and attack description strings.
        /// </summary>
        public static string RogueZombieTextFormatter(string name, int level, string healthDesc, string attackDesc, string special)
        => $"{name?.Trim()}\n僵尸等级：{level}\n韧性：{healthDesc?.Trim()}\n攻击力：{attackDesc?.Trim()}\n特点：{special?.Trim()}\n";

        /// <summary>
        /// Registers a custom zombie's spawn level
        /// and its relative selection weight.
        /// </summary>
        public static void AddCustomZombieSpawnRatio(ID theZombieType, int level, int weight)
        {
            if (!CustomZombieSpawns.TryAdd(theZombieType, (level, weight)))
            {
                ModLogger.LogError("Duplicate zombie type in spawn ratio: " + (int)theZombieType);
            }
        }
    }

    /// <summary>
    /// Provides recommended baseline health values
    /// for custom plants.
    /// </summary>
    public class DefaultPlantStats
    {
        #region Fields
        #pragma warning disable CS1591 // Public constants are intentionally excluded from XML documentation
        public const int BaseHealth = 300;
        public const int NutPlantHealth = 4000;
        public const int TallNutHealth = 8000;
        public const int UltimateNutHealth = 16000;
        public const int UltimateTallNutHealth = 32000;
        public const int UltimateObsidianJalaHealth = 64000;
        #pragma warning restore CS1591
        #endregion
    }
    /// <summary>
    /// Provides baseline health and damage values
    /// for standard and ultimate zombies.
    /// </summary>
    public class DefaultZombieStats
    {
        #region Fields
        #pragma warning disable CS1591 // Public constants are intentionally excluded from XML documentation
        public const int NormalZombieDamage = 50;
        public const int NormalZombieHealth = 270;
        public const int ConeHealth = 370;
        public const int BucketHealth = 1100;
        public const int DoorHealth = 1100;
        public const int PaperHealth = 200;
        public const int HelmetHealth = 1400;
        public const int BrickHeadHealth = 2190; //2390 - 270 + 70 = this
        public const int GiantHealth = 3000;
        public const int UltiZombieA_Health_Light = 3000;
        public const int UltiZombieA_Health_Armored = 6000;
        public const int UltiZombieB_Health_Light = 6000;
        public const int UltiZombieB_Health_Armored = 12000;
        public const int UltiZombieC_Health_Light = 12000;
        public const int UltiZombieC_Health_Armored = 24000;
        public const int UltiZombieD_Health_Light = 24000;
        public const int UltiZombieD_Health_Armored = 48000;
        #pragma warning restore CS1591
        #endregion
    }
    /// <summary>
    /// Represents the relative health categories
    /// used to describe rogue zombies.
    /// </summary>
    public enum RogueZombieHealth
    {
#pragma warning disable CS1591 // Enum members are intentionally excluded from XML documentation
        VeryLow = -2,
        Low = -1,
        Mid = 0,
        High = 1,
        VeryHigh = 2
#pragma warning restore CS1591
    }
    /// <summary>
    /// Represents the relative attack categories
    /// used to describe rogue zombies.
    /// </summary>
    public enum RogueZombieAttack
    {
#pragma warning disable CS1591 // Enum members are intentionally excluded from XML documentation
        VeryLow = -2,
        Low = -1,
        Mid = 0,
        High = 1,
        VeryHigh = 2,
        Crashing = 100
#pragma warning restore CS1591
    }
}
