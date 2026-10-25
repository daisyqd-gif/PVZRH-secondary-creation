using System;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;

namespace CustomPlantClass.Runtime.Core
{
    /// <summary>
    /// Provides logging helpers that tag messages with a mod name,
    /// add them to the appropriate startup log, and write them to BepInEx.
    /// </summary>
    public static class ModLogger
    {
        private static Lazy<ManualLogSource> Log_Lazy = new( () => Logger.CreateLogSource("ModLogger") );

        /// <summary>
        /// Gets the BepInEx log source used by this logger.
        /// </summary>
        public static ManualLogSource Log => Log_Lazy.Value;
        private static void SafeInfo(string msg)
        {
            Log.LogInfo(msg);
        }

        private static void SafeWarn(string msg)
        {
            Log.LogWarning(msg);
        }

        private static void SafeError(string msg)
        {
            Log.LogError(msg);
        }

        // -------------------------
        //  STRING + MOD NAME
        // -------------------------

        /// <summary>
        /// Logs an informational message tagged with the specified mod name
        /// and records it in the startup messages.
        /// </summary>
        /// <param name="mod">The mod name to include in the log entry.</param>
        /// <param name="msg">The informational message to log.</param>
        public static void LogInfo(string mod, string msg)
        {
            string line = $"[{mod}] Info — {msg}";
            SafeInfo(line);
        }

        /// <summary>
        /// Logs a warning tagged with the specified mod name
        /// and records it in the startup warnings.
        /// </summary>
        /// <param name="mod">The mod name to include in the log entry.</param>
        /// <param name="msg">The warning message to log.</param>
        public static void LogWarn(string mod, string msg)
        {
            string line = $"[{mod}] Warning — {msg}";
            SafeWarn(line);
        }

        /// <summary>
        /// Logs a warning tagged with the specified mod name
        /// and records it in the startup warnings.
        /// </summary>
        /// <param name="mod">The mod name to include in the log entry.</param>
        /// <param name="msg">The exception to log.</param>
        public static void LogWarn(string mod, Exception msg)
        {
            string line = $"[{mod}] Warning — {msg}";
            SafeWarn(line);
        }

        /// <summary>
        /// Logs an error tagged with the specified mod name
        /// and records it in the startup errors.
        /// </summary>
        /// <param name="mod">The mod name to include in the log entry.</param>
        /// <param name="msg">The error message to log.</param>
        public static void LogError(string mod, string msg)
        {
            string line = $"[{mod}] Error — {msg}";
            SafeError(line);
        }

        /// <summary>
        /// Logs an error tagged with the specified mod name
        /// and records it in the startup errors.
        /// </summary>
        /// <param name="mod">The mod name to include in the log entry.</param>
        /// <param name="msg">The exception to log.</param>
        public static void LogError(string mod, Exception msg)
        {
            string line = $"[{mod}] Error — {msg}";
            SafeError(line);
        }

        // -------------------------
        //  STRING ONLY
        // -------------------------

        /// <summary>
        /// Logs an informational message tagged with the current plugin name
        /// and records it in the startup messages.
        /// </summary>
        /// <param name="msg">The informational message to log.</param>
        public static void LogInfo(string msg)
        {
            string line = $"[{MyPluginInfo.PluginName}] Info — {msg}";
            SafeInfo(line);
        }

        /// <summary>
        /// Logs a warning tagged with the current plugin name
        /// and records it in the startup warnings.
        /// </summary>
        /// <param name="msg">The warning message to log.</param>
        public static void LogWarn(string msg)
        {
            string line = $"[{MyPluginInfo.PluginName}] Warning — {msg}";
            SafeWarn(line);
        }

        /// <summary>
        /// Logs a warning tagged with the current plugin name
        /// and records it in the startup warnings.
        /// </summary>
        /// <param name="msg">The exception to log.</param>
        public static void LogWarn(Exception msg)
        {
            string line = $"[{MyPluginInfo.PluginName}] Warning — {msg}";
            SafeWarn(line);
        }

        /// <summary>
        /// Logs an error tagged with the current plugin name
        /// and records it in the startup errors.
        /// </summary>
        /// <param name="msg">The error message to log.</param>
        public static void LogError(string msg)
        {
            string line = $"[{MyPluginInfo.PluginName}] Error — {msg}";
            SafeError(line);
        }

        /// <summary>
        /// Logs an error tagged with the current plugin name
        /// and records it in the startup errors.
        /// </summary>
        /// <param name="msg">The exception to log.</param>
        public static void LogError(Exception msg)
        {
            string line = $"[{MyPluginInfo.PluginName}] Error — {msg}";
            SafeError(line);
        }

        // -------------------------
        //  ASSEMBLY
        // -------------------------

        /// <summary>
        /// Resolves the mod name from the assembly, logs an informational message
        /// with that name, and records it in the startup messages.
        /// </summary>
        /// <param name="asm">The assembly used to resolve the mod name.</param>
        /// <param name="msg">The informational message to log.</param>
        public static void LogInfo(Assembly asm, string msg)
        {
            string mod = GetModName(asm);
            string line = $"[{mod}] Info — {msg}";
            SafeInfo(line);
            
        }

        /// <summary>
        /// Resolves the mod name from the assembly, logs a warning
        /// with that name, and records it in the startup warnings.
        /// </summary>
        /// <param name="asm">The assembly used to resolve the mod name.</param>
        /// <param name="msg">The warning message to log.</param>
        public static void LogWarn(Assembly asm, string msg)
        {
            string mod = GetModName(asm);
            string line = $"[{mod}] Warning — {msg}";
            SafeWarn(line);
        }

        /// <summary>
        /// Resolves the mod name from the assembly, logs a warning
        /// with that name, and records it in the startup warnings.
        /// </summary>
        /// <param name="asm">The assembly used to resolve the mod name.</param>
        /// <param name="msg">The exception to log.</param>
        public static void LogWarn(Assembly asm, Exception msg)
        {
            string mod = GetModName(asm);
            string line = $"[{mod}] Warning — {msg}";
            SafeWarn(line);
        }

        /// <summary>
        /// Resolves the mod name from the assembly, logs an error
        /// with that name, and records it in the startup errors.
        /// </summary>
        /// <param name="asm">The assembly used to resolve the mod name.</param>
        /// <param name="msg">The error message to log.</param>
        public static void LogError(Assembly asm, string msg)
        {
            string mod = GetModName(asm);
            string line = $"[{mod}] Error — {msg}";
            SafeError(line);
        }

        /// <summary>
        /// Resolves the mod name from the assembly, logs an error
        /// with that name, and records it in the startup errors.
        /// </summary>
        /// <param name="asm">The assembly used to resolve the mod name.</param>
        /// <param name="msg">The exception to log.</param>
        public static void LogError(Assembly asm, Exception msg)
        {
            string mod = GetModName(asm);
            string line = $"[{mod}] Error — {msg}";
            SafeError(line);
        }

        /// <summary>Resolves a mod's display name from its metadata or assembly information.</summary>
        /// <param name="asm">The assembly whose mod name should be resolved.</param>
        /// <returns>The resolved mod name, or <c>Unknown</c> if no name can be found.</returns>
        static string GetModName(Assembly asm)
        {
            // 1. CustomModAttribute (assembly-level)
            var modAttr = asm.GetCustomAttribute<CustomModAttribute>();
            if (modAttr != null && !string.IsNullOrWhiteSpace(modAttr.Name))
                return modAttr.Name;

            // 3. MyPluginInfo locator
            foreach (var type in asm.GetTypes())
            {
                // Look for a public static field named PluginName
                var field = type.GetField("PluginName",
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                if (field != null && field.FieldType == typeof(string))
                {
                    // Must be const or static readonly
                    string name = field.GetValue(null) as string;
                    if (!string.IsNullOrWhiteSpace(name))
                        return name;
                }
            }


            // 4. BepInPlugin fallback
            var bep = asm.GetCustomAttribute<BepInPlugin>();
            if (bep != null && !string.IsNullOrWhiteSpace(bep.Name))
                return bep.Name;

            if (!asm.FullName.IsNullOrWhiteSpace()) return asm.FullName;

            // 5. Final fallback (no file name, per your request)
            return "Unknown";
        }
    }
    /// <summary>Specifies the display name for a mod at the assembly level.</summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CustomModAttribute : Attribute
    {
#pragma warning disable CS1591 // Public field intentionally does not require XML documentation
        public string Name;
#pragma warning restore CS1591

        /// <summary>Creates assembly-level mod metadata with the specified name.</summary>
        /// <param name="name">The mod's display name.</param>
        public CustomModAttribute(string name)
        {
            Name = name;
        }
    }

}