namespace CustomPlantClass.Runtime
{
    /// <summary>
    /// Allows adding lifetime events to board.
    /// </summary>
    public class BoardBehaviour : MonoBehaviour
    {
        internal Board board => GetComponent<Board>();
        internal static List<Action<Board>> StartEvents = new();
        internal static List<Action<Board>> UpdateEvents = new();
        internal static List<Action<Board>> FixedUpdateEvents = new();
        internal static List<Action<Board>> DestroyEvents = new();
        /// <summary>
        /// Runs when board starts.
        /// </summary>
        /// <param name="action">The action to run</param>
        public static void AddStartEvent(Action<Board> action)
        {
            StartEvents.Add(action);
        }
        /// <summary>
        /// Runs when board updates.
        /// </summary>
        /// <param name="action">The action to run</param>
        public static void AddUpdateEvent(Action<Board> action)
        {
            UpdateEvents.Add(action);
        }
        /// <summary>
        /// Runs when board updates during physics ticks.
        /// </summary>
        /// <param name="action">The action to run</param>
        public static void AddFixedUpdateEvent(Action<Board> action)
        {
            FixedUpdateEvents.Add(action);
        }
        /// <summary>
        /// Runs when board is destroyed.
        /// </summary>
        /// <param name="action">The action to run</param>
        public static void AddDestroyEvent(Action<Board> action)
        {
            DestroyEvents.Add(action);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void Start()
        {
            ResetScanner.EnsureScanned();
            foreach (var i in StartEvents)
            {
                try
                {
                    i(board);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }
        public void Update()
        {
            foreach (var i in UpdateEvents)
            {
                try
                {
                    i(board);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }
        public void FixedUpdate()
        {
            foreach (var i in FixedUpdateEvents)
            {
                try
                {
                    i(board);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }
        public void OnDestroy()
        {
            ResetRegistry.ResetAll();
            foreach (var i in DestroyEvents)
            {
                try
                {
                    i(board);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
    /// <summary>
    /// An attribute to reset a field to its default value when board is destroyed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ResetOnBoardDestroyAttribute : Attribute
    {
        /// <summary>
        /// The default value.
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// Use this constructor when the field's default value is enough.
        /// </summary>
        public ResetOnBoardDestroyAttribute() { }

        /// <summary>
        /// Use this constructor to reset a field to a specific value (Please check that the field type is correct!).
        /// </summary>
        public ResetOnBoardDestroyAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
    /// <summary>
    /// An attribute to run an attribute on a field when the board is destroyed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ActionOnBoardDestroyAttribute : Attribute
    {
        /// <summary>
        /// The action to run
        /// </summary>
        public Func<object> Action { get; }

        /// <summary>
        /// An attribute to run an attribute on a field when the board is destroyed.
        /// </summary>
        public ActionOnBoardDestroyAttribute(Func<object> action)
        {
            Action = action;
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class ResetRegistry
    {
        private static readonly List<(FieldInfo field, object defaultValue)> entries = new();
        private static readonly List<(FieldInfo field, Func<object> action)> entries2 = new();

        public static void Register(FieldInfo field, object defaultValue)
        {
            entries.Add((field, defaultValue));
        }
        public static void Register(FieldInfo field, Func<object> action)
        {
            entries.Add((field, action));
        }

        public static void ResetAll()
        {
            foreach (var (field, defaultValue) in entries)
            {
                Debug.Log($"Defaulted field {field.Name}.");
                field.SetValue(null, defaultValue); // static fields
            }
            foreach (var (field, action) in entries2)
            {
                Debug.Log($"Defaulted field {field.Name}.");
                field.SetValue(null, action()); // static fields
            }
        }
    }
    public static class ResetScanner
    {
        private static bool initialized = false;

        public static void EnsureScanned()
        {
            if (initialized) return;
            initialized = true;

            ScanAllManagedAssemblies();
        }

        public static void ScanType(Type type)
        {
            var fields = type.GetFields(
                BindingFlags.Static | BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<ResetOnBoardDestroyAttribute>();
                if (attr == null) continue;

                object defaultValue = attr.DefaultValue;

                if (defaultValue == null)
                    defaultValue = field.FieldType.IsValueType
                        ? Activator.CreateInstance(field.FieldType)
                        : null;

                ResetRegistry.Register(field, defaultValue);
                Debug.Log($"Found field defaulter for field {field.Name}.");
            }
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<ActionOnBoardDestroyAttribute>();
                if (attr == null) continue;

                Func<object> defaultValue = attr.Action;

                ResetRegistry.Register(field, defaultValue);
                Debug.Log($"Found field defaulter for field {field.Name}.");
            }
        }
        private static void ScanAllManagedAssemblies()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Skip Unity/IL2CPP engine assemblies
                if (asm.FullName.StartsWith("Unity") ||
                    asm.FullName.StartsWith("System") ||
                    asm.FullName.StartsWith("mscorlib"))
                    continue;

                ScanAssembly(asm);
            }
        }
        private static void ScanAssembly(Assembly asm)
        {
            try
            {
                foreach (var type in asm.GetTypes())
                    ScanType(type);
            }
            catch (Exception)
            {

            }
        }
    }

}
