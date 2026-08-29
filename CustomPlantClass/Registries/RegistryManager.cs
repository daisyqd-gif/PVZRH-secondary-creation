
#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using UnityEngine;

namespace CustomPlantClass.Registry
{
    public static class RegistryManager
    {
        // Actual stored registry objects
        private static readonly Dictionary<string, object> Registry = new();

        // Freeze table for deterministic name allocation
        private static readonly string FreezePath =
            Path.Combine(Application.persistentDataPath, "RegistryFreeze.json");
        private static readonly string DataPath =
            Path.Combine(Application.persistentDataPath, "RegistryData.json");

        private static Dictionary<string, string> FreezeTable = new();
        private static bool FreezeLoaded = false;

        // Per-base-name call index (not saved)
        private static readonly Dictionary<string, int> NameCallIndex = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true
        };

        // ---------------------------------------------------------
        // Freeze table load/save
        // ---------------------------------------------------------

        private static void LoadFreeze()
        {
            if (FreezeLoaded) return;

            try
            {
                if (File.Exists(FreezePath))
                {
                    string json = File.ReadAllText(FreezePath);
                    FreezeTable = JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions)
                                 ?? new Dictionary<string, string>();
                }
            }
            catch
            {
                FreezeTable = new Dictionary<string, string>();
            }

            FreezeLoaded = true;
        }

        private static void SaveFreeze()
        {
            try
            {
                string json = JsonSerializer.Serialize(FreezeTable, JsonOptions);
                File.WriteAllText(FreezePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[RegistryFreeze] Failed to save: {e}");
            }
        }

        // ---------------------------------------------------------
        // Name allocator (LevelIDAllocator-style)
        // ---------------------------------------------------------

        private static string AllocateName(string baseName)
        {
            LoadFreeze();

            if (!NameCallIndex.TryGetValue(baseName, out int index))
                index = 0;

            string freezeKey = $"{baseName}::{index}";
            NameCallIndex[baseName] = index + 1;

            // If frozen, return it
            if (FreezeTable.TryGetValue(freezeKey, out string? frozen) && frozen != null)
                return frozen;

            // Deterministic hash
            int hash = Math.Abs(freezeKey.GetHashCode());
            string resolved = $"{baseName}_{hash:X8}";

            FreezeTable[freezeKey] = resolved;
            SaveFreeze();

            return resolved;
        }

        // ---------------------------------------------------------
        // Public API: Generic Add/Get
        // ---------------------------------------------------------

        /// <summary>
        /// Add a serializable object to the registry.
        /// Returns the resolved unique name.
        /// </summary>
        public static string Add<T>(string baseName, T data)
        {
            string resolved = AllocateName(baseName);
            Registry[resolved] = data!;
            SaveData();
            return resolved;
        }
        /// <summary>
        /// Try to get a registry object by resolved name.
        /// </summary>
        public static bool TryGet<T>(string resolvedName, out T? value)
        {
            if (Registry.TryGetValue(resolvedName, out var obj) && obj is T typed)
            {
                value = typed;
                return true;
            }

            value = default;
            return false;
        }
        /// <summary>
        /// Sets a registry object by resolved name.
        /// </summary>
        public static void Set<T>(string name, T value)
        {
            Registry[name] = value!;
            SaveData();
        }
        /// <summary>
        /// Get all resolved names.
        /// </summary>
        public static IEnumerable<string> GetNames()
        {
            return Registry.Keys;
        }
        private static void SaveData()
        {
            try
            {
                string json = JsonSerializer.Serialize(Registry, JsonOptions);
                File.WriteAllText(DataPath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[RegistryData] Failed to save: {e}");
            }
        }

        private static void LoadData()
        {
            try
            {
                if (File.Exists(DataPath))
                {
                    string json = File.ReadAllText(DataPath);
                    Registry.Clear();

                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json, JsonOptions);
                    if (dict != null)
                    {
                        foreach (var kv in dict)
                            Registry[kv.Key] = kv.Value!;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[RegistryData] Failed to load: {e}");
            }
        }
        static RegistryManager()
        {
            LoadFreeze();
            LoadData();
        }
    }
}
