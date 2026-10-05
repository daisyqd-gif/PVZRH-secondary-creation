

namespace CustomPlantClass.Main
{
    /// <summary>
    /// A base plugin for mods
    /// </summary>
    public class ModPlugin : BasePlugin
    {
        /// <summary>
        /// The logger of the plugin. Use ModLogger as an alternative
        /// </summary>
        public ManualLogSource Logger;
        /// <inheritdoc/>
        public override void Load()
        {
            try
            {
                Logger = Log;
                try
                {
                    DataMgr.AutoRegisterTypes(GetType().Assembly);
                    Harmony.CreateAndPatchAll(GetType().Assembly);
                }
                catch (Exception e)
                {
                    ModLogger.LogError("Exception occured during initmod\n" + e.ToString());
                }

                try { InitializeMod(); }
                catch (Exception e)
                {
                    ModLogger.LogError("Exception occured during mod initialization\n" + e.ToString());
                }

#pragma warning disable CS0612 // Type or member is obsolete
                try { OnStart(); }
#pragma warning restore CS0612 // Type or member is obsolete
                catch (Exception e)
                {
                    ModLogger.LogError("Exception occured in OnStart\n" + e.ToString());
                }
                try { InitializePlants(); }
                catch (Exception e)
                {
                    ModLogger.LogError("Exception occured during plant initialization\n" + e.ToString());
                }
                try { InitializeZombies(); }
                catch (Exception e)
                {
                    ModLogger.LogError("Exception occured during zombie initialization\n" + e.ToString());
                }
                try { InitializeBuffs(); }
                catch (Exception e)
                {
                    ModLogger.LogError("Exception occured during buff initialization\n" + e.ToString());
                }

                DataMgr.AddGameStartAction(OnGameStart);
                DataMgr.AddGameStartAction(OnGameInit);
                DataMgr.AddGameStartAction(InitializeConditions);
                PluginBehaviour.QueueOrExecute(OnDataMgrLoad);
            }
            catch (Exception e)
            {
                ModLogger.LogError(e.ToString());
            }
        }
        /// <summary>
        /// Priority: 1
        /// </summary>
        public virtual void InitializeMod() { }
        [OnLoad]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static void CLoad()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            
        }
        [Obsolete]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public virtual void OnStart() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        /// <summary>
        /// Priority: 4
        /// </summary>
        public virtual void InitializeBuffs() { }
        /// <summary>
        /// Priority: 2
        /// </summary>
        public virtual void InitializePlants() { }
        /// <summary>
        /// Priority: 3
        /// </summary>
        public virtual void InitializeZombies() { }
        /// <summary>
        /// Priority: GameStart
        /// </summary>
        public virtual void InitializeConditions() { }
        /// <summary>
        /// Priority: GameStart
        /// </summary>
        public virtual void OnGameStart() { }
        /// <summary>
        /// Priority: GameStart
        /// </summary>
        public virtual void OnGameInit() { }
        /// <summary>
        /// Priority: GameStart
        /// </summary>
        public virtual void OnDataMgrLoad() { }
    }
}