using System.Collections;

//using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnityEngine;

namespace CustomPlantClass.Main
{
    /// <summary>
    /// 
    /// </summary>
    public static class UnityExtensions
    {
        /// <summary>
        /// Tries to find a component implementing the specified interface on this component's GameObject.
        /// </summary>
        /// <typeparam name="T">The interface type to find.</typeparam>
        /// <param name="self">The component whose GameObject is searched.</param>
        /// <param name="outInterface">When this method returns, the matching interface instance, if found.</param>
        /// <returns><see langword="true"/> if a matching component is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetInterface<T>(this MonoBehaviour self, out T outInterface) where T : class
        {
            foreach (var comp in self.GetComponents<Component>())
                if (comp is T output)
                {
                    outInterface = output!;
                    return true;
                }
            outInterface = default!;
            return false;
        }
        /// <summary>
        /// Tries to find a component implementing the specified interface on this component's GameObject.
        /// </summary>
        /// <typeparam name="T">The interface type to find.</typeparam>
        /// <param name="self">The component whose GameObject is searched.</param>
        /// <param name="outInterface">When this method returns, the matching interface instance, if found.</param>
        /// <returns><see langword="true"/> if a matching component is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetInterface<T>(this GameObject self, out T outInterface) where T : class
        {
            foreach (var comp in self.GetComponents<Component>())
                if (comp is T output)
                {
                    outInterface = output!;
                    return true;
                }
            outInterface = default!;
            return false;
        }
        /// <summary>
        /// Tries to find a component implementing the specified interface on this component's GameObject.
        /// </summary>
        /// <typeparam name="T">The interface type to find.</typeparam>
        /// <param name="self">The component whose GameObject is searched.</param>
        /// <param name="outInterface">When this method returns, the matching interface instance, if found.</param>
        /// <returns><see langword="true"/> if a matching component is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetInterface<T>(this Component self, out T outInterface) where T : class
        {
            foreach (var comp in self.GetComponents<Component>())
                if (comp is T output)
                {
                    outInterface = output!;
                    return true;
                }
            outInterface = default!;
            return false;
        }
    }
    /// <summary>
    /// Represents a particle that can destroy its GameObject when it dies.
    /// </summary>
    public class CustomParticle : MonoBehaviour
    {
        /// <summary>
        /// Destroys this particle's GameObject.
        /// </summary>
        public void Die() => Destroy(gameObject);
    }
}