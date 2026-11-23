#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace CustomPlantClass.Main
{

    public static class ExtensionManager
    {
        [Obsolete("ID now supports auto casting")]
        public static GridItemType ToGridItemType(this ID self)
        {
            return (GridItemType)self.id;
        }
        [Obsolete("ID now supports auto casting")]
        public static ID ToID(this GridItemType self)
        {
            return (int)self;
        }
        public static int AddAndGetIndex<T>(this List<T> self, T item)
        {
            int counter = self.Count;
            self.Add(item);
            return counter;
        }
        public static int AddAndGetIndex<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item)
        {
            int counter = self.Count;
            self.Add(item);
            return counter;
        }
        public static Component AddComponent(this MonoBehaviour self, Il2CppSystem.Type type)
        {
            return self.gameObject.AddComponent(type);
        }
        public static Component GetOrAddComponent(this MonoBehaviour self, Il2CppSystem.Type type)
        {
            if (self.TryGetComponent(type, out var component)) return component;
            return self.gameObject.AddComponent(type);
        }
    }
    public static class Il2CppExtensions
    {
        public static HashSet<T> ToSystemHashSet<T>(this Il2CppSystem.Collections.Generic.HashSet<T> self)
        {
            return [.. self];
        }
        public static Il2CppSystem.Collections.Generic.HashSet<T> ToIl2CppHashSet<T>(this IEnumerable<T> self)
        {
            var output = new Il2CppSystem.Collections.Generic.HashSet<T>();
            self.Where((T element) =>
            {
                output.Add(element);
                return true;
            });
            return output;
        }
        /// <summary>
        /// Merges 2 sequences
        /// </summary>
        public static void Merge<T>(this Il2CppSystem.Collections.Generic.HashSet<T> self, Il2CppSystem.Collections.Generic.HashSet<T> value)
        {
            value.ToSystemHashSet().Where((T element) =>
            {
                self.Add(element);
                return true;
            });
        }
        /// <summary>
        /// Merges 2 sequences
        /// </summary>
        public static void Merge<T>(this Il2CppSystem.Collections.Generic.HashSet<T> self, HashSet<T> value)
        {
            value.Where((T element) =>
            {
                self.Add(element);
                return true;
            });
        }
        /// <summary>
        /// Merges 2 sequences
        /// </summary>
        public static void Merge<T>(this Il2CppSystem.Collections.Generic.HashSet<T> self, Il2CppSystem.Collections.Generic.List<T> value)
        {
            value.ToSystemList().Where((T element) =>
            {
                self.Add(element);
                return true;
            });
        }
        /// <summary>
        /// Merges 2 sequences
        /// </summary>
        public static void Merge<T>(this Il2CppSystem.Collections.Generic.HashSet<T> self, List<T> value)
        {
            value.Where((T element) =>
            {
                self.Add(element);
                return true;
            });
        }
        /// <summary>
        /// Merges 2 sequences
        /// </summary>
        public static void Merge<T>(this Il2CppSystem.Collections.Generic.List<T> self, Il2CppSystem.Collections.Generic.List<T> value)
        {
            value.ToSystemList().Where((T element) =>
            {
                self.Add(element);
                return true;
            });
        }
        /// <summary>
        /// Merges 2 sequences
        /// </summary>
        public static void Merge<T>(this Il2CppSystem.Collections.Generic.List<T> self, List<T> value)
        {
            value.Where((T element) =>
            {
                self.Add(element);
                return true;
            });
        }
        /// <summary>
        /// Merges 2 sequences
        /// </summary>
        public static void Merge<T>(this Il2CppSystem.Collections.Generic.List<T> self, Il2CppSystem.Collections.Generic.HashSet<T> value)
        {
            value.ToSystemHashSet().Where((T element) =>
            {
                self.Add(element);
                return true;
            });
        }
        /// <summary>
        /// Merges 2 sequences
        /// </summary>
        public static void Merge<T>(this Il2CppSystem.Collections.Generic.List<T> self, HashSet<T> value)
        {
            value.Where((T element) =>
            {
                self.Add(element);
                return true;
            });
        }
        public static T GetRandomItem<T>(this IEnumerable<T> self)
        {
            if (self is IList<T> list)
                return list[Random.Range(0, list.Count)];

            List<T> temp = [.. self];

            return temp[Random.Range(0, temp.Count)];
        }
    }
    public static class GameExtensions
    {
        public static bool IsActive(this TravelUnlocks self) => Lawnf.TravelUnlock(self);
        public static bool IsActive(this AdvBuff self) => Lawnf.TravelAdvanced(self);
        public static bool IsActive(this UltiBuff self) => Lawnf.TravelUltimate(self);
        public static bool IsActive(this TravelDebuff self) => Lawnf.TravelDebuff(self);
        public static bool IsActive(this InvestBuff self) => Lawnf.TravelInvest(self);
        public static void Activate(this TravelUnlocks self) => TravelMgr.Instance.UnlockPlant(self);
        public static void Activate(this AdvBuff self) => TravelMgr.Instance.GetNormalBuff(self);
        public static void Activate(this UltiBuff self) => TravelMgr.Instance.GetUltiBuff(self);
        public static void Activate(this UltiBuff self, bool allowUpgrade = false) => TravelMgr.Instance.GetUltiBuff(self, allowUpgrade);
        public static void Activate(this TravelDebuff self) => TravelMgr.Instance.GetDebuff(self);
        public static void Activate(this InvestBuff self) => TravelMgr.Instance.GetInvestBuff(self);
        public static int GetBuffLevel(this UltiBuff self) => Lawnf.TravelUltimateLevel(self);
        public static bool IsActiveUnlock(this BuffID self) => Lawnf.TravelUnlock(self);
        public static bool IsActiveAdv(this BuffID self) => Lawnf.TravelAdvanced(self);
        public static bool IsActiveUlti(this BuffID self) => Lawnf.TravelUltimate(self);
        public static bool IsActiveDebuff(this BuffID self) => Lawnf.TravelDebuff(self);
        public static int GetBuffLevelUlti(this BuffID self) => Lawnf.TravelUltimateLevel(self);
        public static void ActivateUnlock(this BuffID self) => TravelMgr.Instance.UnlockPlant(self);
        public static void ActivateAdv(this BuffID self) => TravelMgr.Instance.GetNormalBuff(self);
        public static void ActivateUlti(this BuffID self) => TravelMgr.Instance.GetUltiBuff(self);
        public static void ActivateUlti(this BuffID self, bool allowUpgrade = false) => TravelMgr.Instance.GetUltiBuff(self, allowUpgrade);
        public static void ActivateDebuff(this BuffID self) => TravelMgr.Instance.GetDebuff(self);
        /*
        public static Plant SetPlant( this CreatePlant self, int newColumn, int newRow, ID theSeedType, Plant targetPlant = null, Vector2 puffV = default, bool isFreeSet = false, bool withEffect = true, Plant hidplant = null, TravelRoguePlantData travelRogueData = null)
        {
            return self.SetPlant( newColumn, newRow, theSeedType, targetPlant, puffV, isFreeSet, withEffect, hidplant, travelRogueData );
        }

        public static Zombie SetZombie( this CreateZombie self, int theRow, ID theZombieType, float theX = 9.9f, bool isMindControlled = false, bool ignoreTerrain = false )
        {
            return self.SetZombie( theRow, theZombieType, theX, isMindControlled, ignoreTerrain );
        }
        */
    }
}
