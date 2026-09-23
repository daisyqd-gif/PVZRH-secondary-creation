using Logger = BepInEx.Logging.Logger;
namespace CustomPlantClass
{
    public static class ModLogger
    {
        private static Lazy<ManualLogSource> Log_Lazy = new( () => Logger.CreateLogSource("ModLogger") );
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

        public static void LogInfo(string mod, string msg)
        {
            string line = $"[{mod}] Info — {msg}";
            SafeInfo(line);
            DataMgr.StartUpMessages.Add(line);
        }

        public static void LogWarn(string mod, string msg)
        {
            string line = $"[{mod}] Warning — {msg}";
            SafeWarn(line);
            DataMgr.StartUpWarnings.Add(line);
        }

        public static void LogError(string mod, string msg)
        {
            string line = $"[{mod}] Error — {msg}";
            SafeError(line);
            DataMgr.StartUpErrors.Add(line);
        }

        // -------------------------
        //  STRING ONLY
        // -------------------------

        public static void LogInfo(string msg)
        {
            string line = $"[{MyPluginInfo.PluginName}] Info — {msg}";
            SafeInfo(line);
            DataMgr.StartUpMessages.Add(line);
        }

        public static void LogWarn(string msg)
        {
            string line = $"[{MyPluginInfo.PluginName}] Warning — {msg}";
            SafeWarn(line);
            DataMgr.StartUpWarnings.Add(line);
        }

        public static void LogError(string msg)
        {
            string line = $"[{MyPluginInfo.PluginName}] Error — {msg}";
            SafeError(line);
            DataMgr.StartUpErrors.Add(line);
        }

        // -------------------------
        //  ASSEMBLY
        // -------------------------

        public static void LogInfo(Assembly asm, string msg)
        {
            string mod = AttributeMgr.GetModName(asm);
            string line = $"[{mod}] Info — {msg}";
            SafeInfo(line);
            DataMgr.StartUpMessages.Add(line);
        }

        public static void LogWarn(Assembly asm, string msg)
        {
            string mod = AttributeMgr.GetModName(asm);
            string line = $"[{mod}] Warning — {msg}";
            SafeWarn(line);
            DataMgr.StartUpWarnings.Add(line);
        }

        public static void LogError(Assembly asm, string msg)
        {
            string mod = AttributeMgr.GetModName(asm);
            string line = $"[{mod}] Error — {msg}";
            SafeError(line);
            DataMgr.StartUpErrors.Add(line);
        }
    }
}