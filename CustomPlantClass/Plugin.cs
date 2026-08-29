using CustomPlantClass.Examples;
using CustomPlantClass.Registry;
using CustomPlantClass.UI;

namespace CustomPlantClass
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Logger;
        public static AssetBundle assetBundle;
        internal static Plugin plugin;
        public static Plugin Instance { get => plugin; }
        public static bool Loaded = false;
        public override void Load()
        {
            Loaded = true;
            plugin = this;
            ScientificNumberMgr.OnLoad();
            OnLoad();
            DataMgr.OnLoad();
            CustomLevelMgr.OnLoad();
            PluginBehaviour.OnLoad();
            StaticExamples.OnLoad();
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
        public void OnLoad()
        {
            Logger = Log;
            Tools.InitMod(Assembly.GetExecutingAssembly());
            assetBundle = AssetMgr.LoadBundleFromResource(Assembly.GetExecutingAssembly(), "datamgr", false);
            CustomCore.RegisterCustomCardToColorfulCards(PlantType.ElectricOnion, 1);
            KeyBindingRegistry.Add
            (
                () => $"允许科学计数法",
                (ActionButton btn) => {
                    ScientificNumberMgr.IsEnglishNumber = !ScientificNumberMgr.IsEnglishNumber;
                    btn.Label = ScientificNumberMgr.IsEnglishNumber ? "允许" : "不允许";
                }
            );
        }
    }
    public static class ScientificNumberMgr
    {
        public static string name;

        public static void OnLoad()
        {
            // If the key does not exist, create it
            if (!RegistryManager.TryGet<bool>("Is English Number", out _))
            {
                name = RegistryManager.Add("Is English Number", false);
            }
            else
            {
                // If it exists, resolved name is just the base name
                name = "Is English Number";
            }
        }

        public static bool IsEnglishNumber
        {
            get
            {
                if (RegistryManager.TryGet<bool>(name, out var val))
                    return val;

                return false;
            }

            set
            {
                // ALWAYS write the new value
                RegistryManager.Set(name, value);
            }
        }
    }
    public static class PluginBehaviour
    {
        public static Queue<Action> queued = new();
        public static void QueueOrExecute(Action a)
        {
            if (Plugin.Loaded) a();
            else queued.Enqueue(a);
        }
        public static void OnLoad()
        {
            while (queued.Count > 0)
            {
                try
                {
                    queued.Dequeue()();
                }
                catch (Exception e)
                {
                    ModLogger.LogError(e.ToString());
                }
            }
        }
    }
    public static class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.Bepinex";
        public const string PluginName = "CustomPlantClass";
        public const string PluginVersion = "1.0.0";
        public const string TargetVersion = "3.9";
    }
}