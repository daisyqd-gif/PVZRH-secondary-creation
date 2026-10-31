#nullable enable
using System;
using System.Reflection;

namespace CustomPlantClass.Runtime.Core
{
    /// <summary>
    /// Provides helper methods for retrieving and invoking methods through reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gets a method with the specified name and binding flags from a type.
        /// </summary>
        /// <typeparam name="T">The type to search for the method.</typeparam>
        /// <param name="name">The name of the method to find.</param>
        /// <param name="flags">A bitmask specifying how the search is conducted.</param>
        /// <returns>The method matching the specified criteria, or <see langword="null"/> if no match is found.</returns>
        public static MethodInfo? GetMethod<T>(string name, BindingFlags flags = BindingFlags.Default)
        {
            return typeof(T).GetMethod(name,flags);
        }
        /// <summary>
        /// Invokes a method with the specified name on the specified type.
        /// </summary>
        /// <param name="type">The type to search for the method.</param>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="flags">A bitmask specifying how the search is conducted.</param>
        /// <param name="__instance">The object on which to invoke the method, or <see langword="null"/> for a static method.</param>
        /// <param name="parameters">The arguments to pass to the method.</param>
        /// <returns>The method's return value, or <see langword="null"/> if no matching method is found or if the method returns void.</returns>
        public static object? CallMethod(Type type, string name, BindingFlags flags = BindingFlags.Default, object? __instance = null, params object?[]? parameters)
        {
            var method = type.GetMethod(name,flags);
            if (method == null)
            {
                return null;
            }
            return method.Invoke(__instance,parameters);
        }
        /// <summary>
        /// Invokes a method with the specified name on the specified type.
        /// </summary>
        /// <typeparam name="T">The type to search for the method.</typeparam>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="flags">A bitmask specifying how the search is conducted.</param>
        /// <param name="__instance">The object on which to invoke the method, or <see langword="null"/> for a static method.</param>
        /// <param name="parameters">The arguments to pass to the method.</param>
        /// <returns>The method's return value, or <see langword="null"/> if no matching method is found or if the method returns void.</returns>
        public static object? CallMethod<T>(string name, BindingFlags flags = BindingFlags.Default, object? __instance = null, params object?[]? parameters)
        {
            var method = GetMethod<T>(name,flags);
            if (method == null)
            {
                return null;
            }
            return method.Invoke(__instance,parameters);
        }
    }
}