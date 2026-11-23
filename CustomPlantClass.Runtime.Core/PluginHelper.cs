using System;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace CustomPlantClass.Runtime.Core
{
    /// <inheritdoc/>
    public class RegisteredPlugin : BasePlugin
    {
        /// <summary/>
        public Harmony harmony { get; set; }
        /// <inheritdoc/>
        public sealed override void Load()
        {
            try
            {
                GeneralTools.AutoRegisterTypes(GetType().Assembly);
                harmony = Harmony.CreateAndPatchAll(GetType().Assembly);
            }
            catch(Exception e)
            {
                ModLogger.LogError(e);
            }
            try
            {
                RegisterData();
            }
            catch(Exception e)
            {
                ModLogger.LogError(e);
            }
            try
            {
                OnLoad();
            }
            catch(Exception e)
            {
                ModLogger.LogError(e);
            }
            Load_Async();
        }
        /// <summary>
        /// Used for registering data and setting variables, this method runs first.
        /// </summary>
        public virtual void RegisterData()
        {
            
        }
        /// <summary>
        /// Used for executing code when the plugin loads, this method runs second.
        /// </summary>
        public virtual void OnLoad()
        {
            
        }
        /// <summary>
        /// This method runs asynchronously and is ran third. Make sure to wrap the overriden method in a try catch otherwise an exception will cause the whole process to end.
        /// </summary>
        public virtual async void Load_Async()
        {
            
        }
        /// <summary>
        /// Runs when the game starts
        /// </summary>
        [Obsolete("Not done yet!")]
        public virtual void OnGameInit()
        {
            
        }
    }
}