namespace FrameWorkLoader.API
{
    /// <summary>Defines the lifecycle callback implemented by a mod entry point.</summary>
    public interface IModEntry
    {
        /// <summary>Runs after the mod loader has discovered and ordered mods.</summary>
        public void OnModLoad();
    }
}
