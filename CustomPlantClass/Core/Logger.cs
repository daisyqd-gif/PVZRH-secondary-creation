using Logger = BepInEx.Logging.Logger;
namespace CustomPlantClass
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
            DataMgr.StartUpMessages.Add(line);
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
            DataMgr.StartUpWarnings.Add(line);
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
            DataMgr.StartUpErrors.Add(line);
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
            DataMgr.StartUpMessages.Add(line);
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
            DataMgr.StartUpWarnings.Add(line);
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
            DataMgr.StartUpErrors.Add(line);
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
            string mod = AttributeMgr.GetModName(asm);
            string line = $"[{mod}] Info — {msg}";
            SafeInfo(line);
            DataMgr.StartUpMessages.Add(line);
        }

        /// <summary>
        /// Resolves the mod name from the assembly, logs a warning
        /// with that name, and records it in the startup warnings.
        /// </summary>
        /// <param name="asm">The assembly used to resolve the mod name.</param>
        /// <param name="msg">The warning message to log.</param>
        public static void LogWarn(Assembly asm, string msg)
        {
            string mod = AttributeMgr.GetModName(asm);
            string line = $"[{mod}] Warning — {msg}";
            SafeWarn(line);
            DataMgr.StartUpWarnings.Add(line);
        }

        /// <summary>
        /// Resolves the mod name from the assembly, logs an error
        /// with that name, and records it in the startup errors.
        /// </summary>
        /// <param name="asm">The assembly used to resolve the mod name.</param>
        /// <param name="msg">The error message to log.</param>
        public static void LogError(Assembly asm, string msg)
        {
            string mod = AttributeMgr.GetModName(asm);
            string line = $"[{mod}] Error — {msg}";
            SafeError(line);
            DataMgr.StartUpErrors.Add(line);
        }
    }
}