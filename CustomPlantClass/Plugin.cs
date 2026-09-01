using CustomPlantClass.Registry;
using CustomPlantClass.Runtime.Tasks;
using CustomPlantClass.UI;

namespace CustomPlantClass
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Logger;
        public static AssetBundle assetBundle;
        //internal static Plugin plugin;
        public static Plugin Instance { get; private set; }
        public static bool Loaded = false;
        public static GameObject behaviourObject;
        public override void Load()
        {
            Instance = this;
            DataMgr.AddGameAppInitAction
            (() =>
            {
                behaviourObject = new GameObject("CustomPlantClass_Behaviour").AddComponent<PluginBehaviour>().gameObject;
                Object.DontDestroyOnLoad(behaviourObject);
            });
            Loaded = true;
            Loader.RunAllLoadMethods();
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
        public override bool Unload()
        {
            Loaded = false;
            Loader.RunAllUnloadMethods();
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} unloaded.");
            return base.Unload();
        }
        [OnLoad]
        public static void OnLoad()
        {
            Logger = Instance.Log;
            Tools.InitMod(Assembly.GetExecutingAssembly());
            assetBundle = AssetMgr.LoadBundleFromResource(Assembly.GetExecutingAssembly(), "datamgr", false);
            CustomCore.RegisterCustomCardToColorfulCards(PlantType.ElectricOnion, 1);
            KeyBindingRegistry.Add
            (
                () => $"科学计数法",
                (ActionButton btn) =>
                {
                    ScientificNumberMgr.IsEnglishNumber = !ScientificNumberMgr.IsEnglishNumber;
                    btn.Label = ScientificNumberMgr.IsEnglishNumber ? "允许" : "不允许";
                }
            );
        }
    }
    public static class ScientificNumberMgr
    {
        public static string name;
        [OnLoad]
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
    public class PluginBehaviour : MonoBehaviour
    {
        public static Queue<Action> queued = new();
        public static void QueueOrExecute(Action a)
        {
            if (Plugin.Loaded) a();
            else queued.Enqueue(a);
        }
        [OnLoad]
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
        public static async Task AddComponentToPlugin<T>() where T : Component
        {
            await WaitUntilTask.WaitUntil(() => IsActive == true);
            Plugin.behaviourObject.AddComponent<T>();
        }
        public static bool IsActive { get; private set; } = false;
        public virtual void Awake() => IsActive = true;
        public virtual void OnDestroy() => IsActive = false;
    }
    internal static class Loader
    {
        public static void RunAllLoadMethods()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods(
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (method.GetCustomAttribute<OnLoadAttribute>() != null)
                    {
                        object instance = null;

                        if (!method.IsStatic)
                            instance = Activator.CreateInstance(type);

                        method.Invoke(instance, null);
                    }
                }
            }
        }
        public static void RunAllUnloadMethods()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods(
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (method.GetCustomAttribute<OnUnloadAttribute>() != null)
                    {
                        object instance = null;

                        if (!method.IsStatic)
                            instance = Activator.CreateInstance(type);

                        method.Invoke(instance, null);
                    }
                }
            }
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class OnLoadAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class OnUnloadAttribute : Attribute { }
    public static class MyPluginInfo
    {
        public const string PluginGuid = "CustomPlantClass.Bepinex";
        public const string PluginName = "CustomPlantClass";
        public const string PluginVersion = "1.0.0";
        public const string TargetVersion = "3.9";
    }
}