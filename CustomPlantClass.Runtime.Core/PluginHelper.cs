using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx.Unity.IL2CPP;
using CustomPlantClass.Runtime.Tasks;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomPlantClass.Runtime.Core
{
    /// <inheritdoc/>
    public class RegisteredPlugin : BasePlugin
    {
        /// <inheritdoc/>
        public sealed override void Load()
        {
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
    /// <summary>
    /// General tools for various purposes
    /// </summary>
    public static class GeneralTools
    {
        /// <summary>Runs an action after the specified delay.</summary>
        /// <param name="a">The action to run.</param>
        /// <param name="delaySeconds">The delay before running the action, in seconds.</param>
        public static async void WaitAndExecute(Action a, float delaySeconds)
        {
            await DelayTask.Delay(delaySeconds);
            a();
        }
        /// <summary>Runs an action with a parameter after the specified delay.</summary>
        /// <typeparam name="T">The type of the action parameter.</typeparam>
        /// <param name="a">The action to run.</param>
        /// <param name="parameter">The value passed to the action.</param>
        /// <param name="delaySeconds">The delay before running the action, in seconds.</param>
        public static async void WaitAndExecute<T>(Action<T> a, T parameter, float delaySeconds)
        {
            await DelayTask.Delay(delaySeconds);
            a(parameter);
        }
        /// <summary>Runs a function with a parameter after the specified delay and returns its result.</summary>
        /// <typeparam name="T">The type of the function parameter.</typeparam>
        /// <typeparam name="Tout">The type of the function result.</typeparam>
        /// <param name="a">The function to run.</param>
        /// <param name="parameter">The value passed to the function.</param>
        /// <param name="delaySeconds">The delay before running the function, in seconds.</param>
        /// <returns>A task that completes with the function's result.</returns>
        public static async Task<Tout> WaitAndExecute<T, Tout>(Func<T, Tout> a, T parameter, float delaySeconds)
        {
            await DelayTask.Delay(delaySeconds);
            return a(parameter);
        }
        /// <summary>
        /// Returns a list of colliders within a cone-shaped area defined by the origin, direction, maximum distance, and cone angle.
        /// </summary>
        /// <param name="origin">The origin point of the cone</param>
        /// <param name="direction">The direction the cone is facing</param>
        /// <param name="maxDistance">The maximum distance of the cone</param>
        /// <param name="coneAngle">The angle of the cone in degrees</param>
        /// <param name="layerMask">The layer mask to use for the raycasts</param>
        /// <param name="rayCount">The number of rays to cast within the cone</param>
        /// <returns>A null checked list of colliders within a cone-shaped area</returns>
        public static List<Collider2D> GetCollidersInCone(Vector2 origin, Vector2 direction, float maxDistance, float coneAngle, LayerMask layerMask, int rayCount = 10)
        {
            List<Collider2D> collidersInCone = new List<Collider2D>();

            // Normalize the direction vector
            direction.Normalize();

            // Calculate the half angle of the cone in radians
            float halfAngle = coneAngle * 0.5f * Mathf.Deg2Rad;

            // Calculate the angle step for raycasting
            float angleStep = coneAngle / (rayCount - 1);

            // Loop through each ray in the cone
            for (int i = 0; i < rayCount; i++)
            {
                // Calculate the current angle for this ray
                float currentAngle = -halfAngle + (i * angleStep * Mathf.Deg2Rad);

                // Rotate the direction vector by the current angle
                Vector2 rotatedDirection = new Vector2(
                    direction.x * Mathf.Cos(currentAngle) - direction.y * Mathf.Sin(currentAngle),
                    direction.x * Mathf.Sin(currentAngle) + direction.y * Mathf.Cos(currentAngle)
                );

                // Perform a raycast in the rotated direction
                RaycastHit2D hit = Physics2D.Raycast(origin, rotatedDirection, maxDistance, layerMask);
                if (hit.collider != null && !collidersInCone.Contains(hit.collider))
                {
                    collidersInCone.Add(hit.collider);
                }
            }

            return collidersInCone;
        }
        /// <param name="center">The center of the oval</param>
        /// <param name="radiusX">The X radius of the oval</param>
        /// <param name="radiusY">The Y radius of the oval</param>
        /// <param name="layerMask">The layer of the collider</param>
        /// <returns>All colliders in an oval</returns>
        public static List<Collider2D> GetCollider2DsInOval(Vector2 center, float radiusX, float radiusY, LayerMask layerMask)
        {
            List<Collider2D> collidersInOval = new List<Collider2D>();

            // Calculate the bounding box of the oval
            Vector2 min = new Vector2(center.x - radiusX, center.y - radiusY);
            Vector2 max = new Vector2(center.x + radiusX, center.y + radiusY);

            // Get all colliders in the bounding box
            Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max, layerMask);

            // Filter colliders that are actually within the oval
            foreach (var collider in colliders)
            {
                Vector2 closestPoint = collider.ClosestPoint(center);
                float normalizedX = (closestPoint.x - center.x) / radiusX;
                float normalizedY = (closestPoint.y - center.y) / radiusY;

                if ((normalizedX * normalizedX) + (normalizedY * normalizedY) <= 1f)
                {
                    collidersInOval.Add(collider);
                }
            }

            return collidersInOval;
        }
        /// <summary>
        /// Registers all eligible derived types from the calling assembly
        /// with the IL2CPP class injector.
        /// </summary>
        public static void AutoRegisterTypes() => AutoRegisterTypes(Assembly.GetCallingAssembly());
        /// <summary>
        /// Registers eligible derived types from an assembly
        /// with the IL2CPP class injector.
        /// </summary>
        public static void AutoRegisterTypes([NotNull] Assembly asm)
        {
            foreach (var type in GetAllDerivedTypes<Il2CppSystem.Object>([asm], containBase: false))
            {
                if (!type.IsAbstract && !ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                {
                    ClassInjector.RegisterTypeInIl2Cpp(type);
                }
            }
        }

        /// <summary>
        /// Ensures the specified custom component type
        /// is registered with the IL2CPP class injector.
        /// </summary>
        public static void EnsureTypeRegistered<TClass>() where TClass : Il2CppSystem.Object
        {
            var type = typeof(TClass);
            if (!ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                ClassInjector.RegisterTypeInIl2Cpp(type);
        }
        /// <summary>
        /// Gets the types in the specified assemblies that derive from the given base type.
        /// </summary>
        /// <param name="asms">The assemblies to search.</param>
        /// <param name="baseType">The base type to match.</param>
        /// <param name="containBase">Whether to include the base type itself when present.</param>
        /// <returns>The matching types.</returns>
        public static Type[] GetAllDerivedTypes(Assembly[] asms, Type baseType, bool containBase = true)
        {
            Type[] allTypes = GetAllTypes(asms);
            return allTypes.Where((Type t) => IsSubTypeOf(t, baseType, containBase)).ToArray();
        }
        /// <summary>
        /// Gets the types in the specified assemblies that derive from the given type.
        /// </summary>
        /// <typeparam name="T">The base type to match.</typeparam>
        /// <param name="asms">The assemblies to search.</param>
        /// <param name="containBase">Whether to include the base type itself when present.</param>
        /// <returns>The matching types.</returns>
        public static Type[] GetAllDerivedTypes<T>(Assembly[] asms, bool containBase = true)
        {
            Type[] allTypes = GetAllTypes(asms);
            return allTypes.Where((Type t) => IsSubTypeOf(t, typeof(T), containBase)).ToArray();
        }

        /// <summary>
        /// Gets all types from the assemblies loaded in the current application domain.
        /// </summary>
        /// <returns>All types from the loaded assemblies.</returns>
        public static Type[] GetAllTypes()
        {
            return GetAllTypes(AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// Gets all types from the specified assemblies.
        /// </summary>
        /// <param name="asms">The assemblies to search.</param>
        /// <returns>All types found in the specified assemblies.</returns>
        public static Type[] GetAllTypes(Assembly[] asms)
        {
            List<Type> list = new List<Type>();
            foreach (Assembly asm in asms)
            {
                list.AddRange(GetAllTypes(asm));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Gets all types from the specified assembly.
        /// </summary>
        /// <param name="asm">The assembly to search.</param>
        /// <returns>All types found in the specified assembly.</returns>
        public static Type[] GetAllTypes(Assembly asm)
        {
            try
            {
                return asm.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                if (ex.Types != null)
                {
                    return (from t in ex.Types
                            where t != null
                            select (t)).ToArray();
                }

                return Array.Empty<Type>();
            }
        }

        /// <summary>Determines whether a type derives from or implements the specified base type.</summary>
        /// <param name="type">The type to inspect.</param>
        /// <param name="baseType">The base type or interface to match.</param>
        /// <param name="containBase">Whether to include the type itself in the match.</param>
        /// <returns><see langword="true"/> if the type matches; otherwise, <see langword="false"/>.</returns>
        public static bool IsSubTypeOf(Type type, Type baseType, bool containBase = true)
        {
            if (type == null || baseType == null)
            {
                return false;
            }

            if (baseType.IsGenericTypeDefinition)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                {
                    return true;
                }

                if (baseType.IsInterface)
                {
                    return type.GetInterfaces().Any((Type i) => i.IsGenericType && i.GetGenericTypeDefinition() == baseType);
                }

                Type baseType2 = type.BaseType;
                while (baseType2 != null && baseType2 != typeof(object))
                {
                    if (baseType2.IsGenericType && baseType2.GetGenericTypeDefinition() == baseType)
                    {
                        return true;
                    }

                    baseType2 = baseType2.BaseType;
                }

                return false;
            }

            if (containBase)
            {
                return baseType.IsAssignableFrom(type);
            }

            return baseType.IsAssignableFrom(type) && type != baseType;
        }
    }
}