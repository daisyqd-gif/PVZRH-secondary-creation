using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using System.Linq;

namespace ModAppSilencer
{
    [BepInPlugin("ModApp_Error_Silencer.Bepinex", "ModApp_Error_Silencer", "3.5")]
    public class Core : BasePlugin
    {
        public static readonly string[] BlockTags = {
            "ModAPP"
        };

        public override void Load()
        {
            // Find the existing console listener
            var consoleListener = Logger.Listeners.FirstOrDefault(l => l is ConsoleLogListener);
            if (consoleListener != null)
            {
                Logger.Listeners.Remove(consoleListener);
                Logger.Listeners.Add(new FilteredConsoleLogListener((ConsoleLogListener)consoleListener));
            }

            Log.LogInfo("ModAPP Error Silencer active");
        }
    }

    public class FilteredConsoleLogListener : ILogListener
    {
        private readonly ConsoleLogListener _inner;

        public FilteredConsoleLogListener(ConsoleLogListener inner)
        {
            _inner = inner;
        }

        public LogLevel LogLevelFilter
        {
            get => _inner.LogLevelFilter;
        }

        public void Dispose() => _inner.Dispose();

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            string msg = eventArgs.Data?.ToString() ?? "";

            foreach (var tag in Core.BlockTags)
                if (msg.Contains(tag))
                    return; // DROP ModAPP spam

            // Forward everything else to the original console listener
            _inner.LogEvent(sender, eventArgs);
        }
    }
}
