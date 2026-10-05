using CustomPlantClass.Runtime.Tasks;


namespace CustomPlantClass
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// Behaviour to run when CustomPlantClass loads.
    /// </summary>
    public class PluginBehaviour : MonoBehaviour
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static Queue<Action> queued = new();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        /// <summary>
        /// Queues an action to run if called before CustomPlantClass loads and runs it if CustomPlantClass is loaded already.
        /// </summary>
        /// <param name="a">The action to run when the plugin loads</param>
        public static void QueueOrExecute(Action a)
        {
            if (Plugin.Loaded) a();
            else queued.Enqueue(a);
        }
        [OnLoad]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static void OnLoad()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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
        /// <summary>
        /// Waits for game load and adds a component to CustomPlantClass's plugin's behaviour.
        /// </summary>
        /// <typeparam name="T">The component to add to CustomPlantClass's plugin's behaviour object</typeparam>
        public static async void AddComponentToPlugin<T>() where T : Component
        {
            await WaitUntilTask.WaitUntil(() => IsActive == true);
            Plugin.behaviourObject.AddComponent<T>();
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static bool IsActive { get; private set; } = false;
        public virtual void Awake() => IsActive = true;
        public virtual void OnDestroy() => IsActive = false;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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
    /// <summary>
    /// CustomPlantClass's plugin info. TargetVersion can be used as mod plugin's version.
    /// </summary>
    public static class MyPluginInfo
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const string PluginGuid = "CustomPlantClass.Bepinex";
        public const string PluginName = "CustomPlantClass";
        public const string PluginVersion = "1.0.5";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        /// <summary>
        /// The mod framework's target game version
        /// </summary>
        public const string TargetVersion = "4.0";
    }
}