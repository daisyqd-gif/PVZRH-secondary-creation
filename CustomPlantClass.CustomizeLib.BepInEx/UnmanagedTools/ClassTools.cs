namespace CustomPlantClass.CustomizeLib.UnmanagedTools
{
    /// <summary>
    /// Compatibility shim for legacy code that still references the old
    /// UnmanagedTools namespace. The custom library only needs the Il2Cpp void
    /// type name for native hook generation.
    /// </summary>
    public static class ClassTools
    {
        public const string Void = "void";
    }
}
