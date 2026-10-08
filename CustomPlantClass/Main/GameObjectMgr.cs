#nullable enable

namespace CustomPlantClass.Main
{
    /// <summary>
    /// A tool to store GameObjects and instantiate them
    /// </summary>
    public class GameObjectMgr : MonoBehaviour
    {
        private static readonly Dictionary<CustomItemType, GameObject> ObjectDictionary = new();

        /// <summary>Adds the specified component to a game object and registers that object.</summary>
        /// <typeparam name="T">The component type to add.</typeparam>
        /// <param name="gameObject">The game object to configure and register.</param>
        /// <param name="id">The ID to assign, or -1 to allocate one automatically.</param>
        /// <returns>The registered object's custom item type.</returns>
        public static CustomItemType Register<T>(GameObject gameObject, int id = -1) where T : Component
        {
            gameObject.AddComponent<T>();
            return Register(gameObject, id);
        }
        /// <summary>Registers a game object under a custom item type.</summary>
        /// <param name="gameObject">The game object to register.</param>
        /// <param name="id">The ID to assign, or -1 to allocate one automatically.</param>
        /// <returns>The registered object's custom item type.</returns>
        /// <exception cref="ArgumentException">The specified ID is already registered.</exception>
        public static CustomItemType Register(GameObject gameObject, int id = -1)
        {
            // If user manually passed an ID, ensure it's not taken
            if (id != -1 && ObjectDictionary.ContainsKey((CustomItemType)id))
            {
                throw new ArgumentException(
                    $"Prefab ID {id} is already taken. Use -1 to auto-allocate."
                );
            }

            // Allocate or wrap the ID
            CustomItemType theItemType = id == -1
                ? AllocatePrefabID()
                : (CustomItemType)id;

            ObjectDictionary.Add(theItemType, gameObject);
            return theItemType;
        }
        /// <summary>Gets the game object registered for a custom item type.</summary>
        /// <param name="type">The custom item type to look up.</param>
        /// <returns>The registered game object, or the safe default if no object is registered.</returns>
        public static GameObject Get(CustomItemType type)
        {
            return ObjectDictionary.GetValueSafe(type);
        }
        /// <summary>Creates an instance of the registered object at a position and rotation.</summary>
        /// <param name="type">The custom item type to instantiate.</param>
        /// <param name="position">The instance's position.</param>
        /// <param name="rotation">The instance's rotation.</param>
        /// <returns>The instantiated game object.</returns>
        public static GameObject Instantiate(CustomItemType type, Vector3 position, Quaternion rotation)
        {
            return Instantiate(Get(type), position, rotation);
        }
        /// <summary>Creates an instance of the registered object at a position and rotation under a parent.</summary>
        /// <param name="type">The custom item type to instantiate.</param>
        /// <param name="position">The instance's position.</param>
        /// <param name="rotation">The instance's rotation.</param>
        /// <param name="parent">The transform to parent the instance to.</param>
        /// <returns>The instantiated game object.</returns>
        public static GameObject Instantiate(CustomItemType type, Vector3 position, Quaternion rotation, Transform parent)
        {
            return Instantiate(Get(type), position, rotation, parent);
        }
        /// <summary>Creates an instance of the registered object using the default instantiate settings.</summary>
        /// <param name="type">The custom item type to instantiate.</param>
        /// <returns>The instantiated game object.</returns>
        public static GameObject Instantiate(CustomItemType type)
        {
            return Instantiate(Get(type));
        }

        // ---------------------------------------------------------
        //  Allocate prefab ID
        // ---------------------------------------------------------

        /// <summary>Allocates a deterministic prefab ID for the calling mod and avoids registered ID collisions.</summary>
        /// <returns>The allocated prefab ID.</returns>
        public static int AllocatePrefabID()
        {
            PrefabIDAllocator.LoadFreezeTable();

            // 1. Resolve mod identity
            Assembly? asm = PrefabIDAllocator.ResolveModAssembly();

            string guidBase;

            if (asm != null)
            {
                var pluginAttr = asm.GetCustomAttribute<BepInPlugin>();
                guidBase = pluginAttr?.GUID ?? asm.FullName!;
            }
            else
            {
                var calling = Assembly.GetCallingAssembly();
                guidBase = calling?.FullName ?? "__PREFABIDFREEZE_UNKNOWN__";
            }

            // 2. Multi‑ID‑per‑mod
            if (!PrefabIDAllocator.GuidCallIndex.TryGetValue(guidBase, out int index))
                index = 0;

            string freezeKey = $"{guidBase}::{index}";
            PrefabIDAllocator.GuidCallIndex[guidBase] = index + 1;

            // 3. If frozen, return it
            if (PrefabIDAllocator.FreezeTable.TryGetValue(freezeKey, out int frozen))
                return frozen;

            // 4. Deterministic base ID
            int baseId = Math.Abs(freezeKey.GetHashCode()) % 50000 + 10000;
            int candidate = baseId;

            // 5. Avoid collisions with existing prefabs
            while (ObjectDictionary.ContainsKey(new CustomItemType(candidate)))
                candidate++;

            // 6. Freeze and return
            PrefabIDAllocator.FreezeTable[freezeKey] = candidate;
            PrefabIDAllocator.SaveFreezeTable();

            return candidate;
        }
        private static class PrefabIDAllocator
        {
            static readonly string FreezePath = Path.Combine(Paths.ConfigPath, "PrefabIDFreeze.json");
            public static Dictionary<string, int> FreezeTable = new();
            static bool FreezeLoaded = false;

            // Per‑mod call index (not saved)
            public static readonly Dictionary<string, int> GuidCallIndex = new();

            static readonly JsonSerializerOptions FreezeJsonOptions = new()
            {
                WriteIndented = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNameCaseInsensitive = true
            };

            // ---------------------------------------------------------
            //  Freeze table load/save
            // ---------------------------------------------------------

            public static void LoadFreezeTable()
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
                    ModLogger.LogError($"[PrefabIDFreeze] Failed to load freeze table: {e}");
                    FreezeTable = new Dictionary<string, int>();
                }

                FreezeLoaded = true;
            }

            public static void SaveFreezeTable()
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
                    ModLogger.LogError($"[PrefabIDFreeze] Failed to save freeze table: {e}");
                }
            }

            // ---------------------------------------------------------
            //  Resolve mod assembly (same logic as LevelIDAllocator)
            // ---------------------------------------------------------

            public static Assembly? ResolveModAssembly()
            {
                var calling = Assembly.GetCallingAssembly();
                if (calling != null &&
                    calling != typeof(PrefabIDAllocator).Assembly &&
                    calling.GetCustomAttribute<BepInPlugin>() != null)
                {
                    return calling;
                }

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

                return null;
            }
        }
    }
    /// <summary>
    /// :)
    /// </summary>
    public struct CustomItemType
    {
        private int id;
        /// <summary>Converts a custom item type to its underlying integer ID.</summary>
        /// <param name="type">The custom item type to convert.</param>
        /// <returns>The integer ID.</returns>
        public static implicit operator int(CustomItemType type)
        {
            return type.id;
        }
        /// <summary>Creates a custom item type from an integer ID.</summary>
        /// <param name="type">The integer ID to convert.</param>
        /// <returns>A custom item type containing the specified ID.</returns>
        public static implicit operator CustomItemType(int type)
        {
            return new CustomItemType(type);
        }
        /// <summary>Initializes a custom item type with an ID.</summary>
        /// <param name="id">The integer ID to store.</param>
        public CustomItemType(int id) => this.id = id;
    }
}