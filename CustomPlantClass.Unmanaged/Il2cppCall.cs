#nullable enable
using System.Runtime.CompilerServices;
using Il2CppInterop.Runtime;
using static Il2CppInterop.Runtime.IL2CPP;

namespace CustomPlantClass.Unmanaged
{
    /// <summary>
    /// Helpers for il2cpp
    /// </summary>
    public static partial class Il2CppHelper
    {
        /*s
        public unsafe static Il2CppSystem.Object? CallStaticMethod(IntPtr methodPtr)
        {
            IntPtr* ptr = stackalloc IntPtr[1];
            *ptr = il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull(loop));
            Unsafe.SkipInit(out IntPtr exc);
            IntPtr intPtr = il2cpp_runtime_invoke(methodPtr, (IntPtr)0, (void**)ptr, ref exc);
            Il2CppException.RaiseExceptionIfNecessary(exc);
        }
        */
        /// <summary>
        /// Calls an instance method that does not return anything
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodPtr"></param>
        public unsafe static void CallInstanceMethod(Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase instance, IntPtr methodPtr)
        {
            Il2CppObjectBaseToPtrNotNull(instance);
            IntPtr* param = null;
            Unsafe.SkipInit(out IntPtr exc);
            IntPtr intPtr = il2cpp_runtime_invoke(il2cpp_object_get_virtual_method(Il2CppObjectBaseToPtr(instance), methodPtr), Il2CppObjectBaseToPtrNotNull(instance), (void**)param, ref exc);
            Il2CppException.RaiseExceptionIfNecessary(exc);
        }
    }
}