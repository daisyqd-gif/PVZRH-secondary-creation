using Il2CppInterop.Runtime.InteropTypes;

namespace CustomPlantClass.Runtime.Core
{
    /// <summary>
    /// Provides tools for Il2cpp
    /// </summary>
    public static partial class Il2CPPTools
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool TryCast<T>(this Il2CppSystem.Object self, out T result) where T : Il2CppObjectBase
        {
            result = self.TryCast<T>();
            return result != null;
        }
    }
}