namespace CustomPlantClass.Runtime.Tasks
{
    /// <summary>
    /// Helper for registering events that run on player loop update
    /// </summary>
    public class PlayerLoopTask : Il2CppSystem.Object
    {
        private static readonly List<Action> Run = new();
        private static readonly List<Action> Run_internal = new();

        /// <summary>
        /// Registers an even that runs every 
        /// </summary>
        /// <param name="run"></param>
        public static void AddRunAction(Action run)
        {
            if (run != null) Run.Add(run);
        }
        internal static void AddRunAction_internal(Action run)
        {
            if (run != null) Run_internal.Add(run);
        }
        internal static void AddToPlayerLoop(Action run, Type type = null)
        {
            bool useUniqueType = false;
            if(type != null && type.IsAssignableFrom(typeof(Il2CppSystem.Object)))
            {
                if(ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                    ClassInjector.RegisterTypeInIl2Cpp(type);
                useUniqueType = true;
            }
            PlayerLoopSystem pl = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystem playerLoopSystem = new()
            {
                updateDelegate = run,
                type = Il2CppType.From(useUniqueType ? type : typeof(PlayerLoopTask))
            };
            PlayerLoop.SetPlayerLoop(InsertSystemAfter<Update>(in pl,playerLoopSystem));
            AddRunAction_internal(DelayScheduler.OnPlayerLoop);
            AddRunAction_internal(WaitUntilScheduler.OnPlayerLoop);
        }

        internal static void OnPlayerLoop()
        {
            var count = Run.Count;
            var count2 = Run_internal.Count;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    Run[i]?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
            }
            for (int i = 0; i < count2; i++)
            {
                try
                {
                    Run_internal[i]?.Invoke();
                }
                catch {}
            }
        }
        
        internal static PlayerLoopSystem InsertSystemAfter<T>(in PlayerLoopSystem loopSystem, PlayerLoopSystem newSystem) where T : struct
        {
            // Create a new root PlayerLoopSystem
            PlayerLoopSystem newPlayerLoop = new()
            {
                loopConditionFunction = loopSystem.loopConditionFunction,
                type = loopSystem.type,
                updateDelegate = loopSystem.updateDelegate,
                updateFunction = loopSystem.updateFunction
            };
            // Create a new list to populate with subsystems, including the custom system
            List<PlayerLoopSystem> newSubSystemList = new();

            //Iterate through the subsystems in the existing loop we passed in and add them to the new list
            if (loopSystem.subSystemList != null)
            {
                for (var i = 0; i < loopSystem.subSystemList.Length; i++)
                {
                    newSubSystemList.Add(loopSystem.subSystemList[i]);
                    // If the previously added subsystem is of the type to add after, add the custom system
                    if (loopSystem.subSystemList[i].type == Il2CppType.From(typeof(T)))
                    {
                        newSubSystemList.Add(newSystem);
                        Debug.Log("Added system to playerloop");
                    }
                }
            }

            newPlayerLoop.subSystemList = newSubSystemList.ToArray();
            return newPlayerLoop;
        }
    }
}