namespace CustomPlantClass.Main
{
    /// <summary>Associates a custom plant implementation with its base type and registration data.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomPlantAttribute : Attribute
    {
        /// <summary>Gets the base plant type used by the custom implementation.</summary>
        public Type BaseType { get; }
        /// <summary>Gets the data used to register the custom plant.</summary>
        public BaseCustomPlantData Data { get; }

        /// <summary>Creates metadata for a custom plant implementation.</summary>
        /// <param name="baseType">The plant base type to extend.</param>
        /// <param name="data">The data used to register the custom plant.</param>
        public CustomPlantAttribute(Type baseType, BaseCustomPlantData data)
        {
            BaseType = baseType;
            Data = data;
        }
    }
    /// <summary>Associates a custom bullet implementation with its base type and registration data.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomBulletAttribute : Attribute
    {
        /// <summary>Gets the base bullet type used by the custom implementation.</summary>
        public Type BaseType { get; }
        /// <summary>Gets the data used to register the custom bullet.</summary>
        public BaseCustomBulletData Data { get; }

        /// <summary>Creates metadata for a custom bullet implementation.</summary>
        /// <param name="baseType">The bullet base type to extend.</param>
        /// <param name="data">The data used to register the custom bullet.</param>
        public CustomBulletAttribute(Type baseType, BaseCustomBulletData data)
        {
            BaseType = baseType;
            Data = data;
        }
    }

    /// <summary>Associates a custom zombie implementation with its base type and registration data.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomZombieAttribute : Attribute
    {
        /// <summary>Gets the base zombie type used by the custom implementation.</summary>
        public Type BaseType { get; }
        /// <summary>Gets the data used to register the custom zombie.</summary>
        public BaseCustomZombieData Data { get; }

        /// <summary>Creates metadata for a custom zombie implementation.</summary>
        /// <param name="baseType">The zombie base type to extend.</param>
        /// <param name="data">The data used to register the custom zombie.</param>
        public CustomZombieAttribute(Type baseType, BaseCustomZombieData data)
        {
            BaseType = baseType;
            Data = data;
        }
    }

    /// <summary>Specifies the display name for a mod at the assembly level.</summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CustomModAttribute : Attribute
    {
#pragma warning disable CS1591 // Public field intentionally does not require XML documentation
        public string Name;
#pragma warning restore CS1591

        /// <summary>Creates assembly-level mod metadata with the specified name.</summary>
        /// <param name="name">The mod's display name.</param>
        public CustomModAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>Specifies the display name for a mod at the module level.</summary>
    [AttributeUsage(AttributeTargets.Module)]
    public class CustomModModuleAttribute : Attribute
    {
#pragma warning disable CS1591 // Public field intentionally does not require XML documentation
        public string Name;
#pragma warning restore CS1591

        /// <summary>Creates module-level mod metadata with the specified name.</summary>
        /// <param name="name">The mod's display name.</param>
        public CustomModModuleAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>Loads custom plant, bullet, and zombie attributes and resolves mod display names.</summary>
    public static class AttributeMgr
    {

        /// <summary>Finds custom implementation attributes in an assembly and registers their data.</summary>
        /// <param name="asm">The assembly to scan for custom implementation attributes.</param>
        public static void LoadAllAttributes(Assembly asm)
        {
            foreach (Type t in asm.GetTypes())
            {
                // --- Plants ---
                var plantAttr = t.GetCustomAttribute<CustomPlantAttribute>();
                if (plantAttr != null)
                {
                    Type tBase = plantAttr.BaseType;
                    Type tClass = t;

                    var method = typeof(DataMgr)
                        .GetMethod("RegisterCustomPlant")
                        .MakeGenericMethod(tBase, tClass);

                    method.Invoke(null, new object[] { plantAttr.Data });
                    continue;
                }

                // --- Bullets ---
                var bulletAttr = t.GetCustomAttribute<CustomBulletAttribute>();
                if (bulletAttr != null)
                {
                    Type tBase = bulletAttr.BaseType;
                    Type tClass = t;

                    var method = typeof(DataMgr)
                        .GetMethod("RegisterCustomBullet")
                        .MakeGenericMethod(tBase, tClass);

                    method.Invoke(null, new object[] { bulletAttr.Data });
                    continue;
                }

                // --- Zombies ---
                var zombieAttr = t.GetCustomAttribute<CustomZombieAttribute>();
                if (zombieAttr != null)
                {
                    Type tBase = zombieAttr.BaseType;
                    Type tClass = t;

                    var method = typeof(DataMgr)
                        .GetMethod("RegisterCustomZombie")
                        .MakeGenericMethod(tBase, tClass);

                    method.Invoke(null, new object[] { zombieAttr.Data });
                    continue;
                }
            }
        }

        /// <summary>Resolves a mod's display name from its metadata or assembly information.</summary>
        /// <param name="asm">The assembly whose mod name should be resolved.</param>
        /// <returns>The resolved mod name, or <c>Unknown</c> if no name can be found.</returns>
        public static string GetModName(Assembly asm)
        {
            // 1. CustomModAttribute (assembly-level)
            var modAttr = asm.GetCustomAttribute<CustomModAttribute>();
            if (modAttr != null && !string.IsNullOrWhiteSpace(modAttr.Name))
                return modAttr.Name;

            // 2. CustomModAttribute (module-level, ConfuserEx style)
            foreach (var module in asm.GetModules())
            {
                var moduleAttr = module.GetCustomAttribute<CustomModModuleAttribute>();
                if (moduleAttr != null && !string.IsNullOrWhiteSpace(moduleAttr.Name))
                    return moduleAttr.Name;
            }

            // 3. MyPluginInfo locator
            foreach (var type in asm.GetTypes())
            {
                // Look for a public static field named PluginName
                var field = type.GetField("PluginName",
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                if (field != null && field.FieldType == typeof(string))
                {
                    // Must be const or static readonly
                    string name = field.GetValue(null) as string;
                    if (!string.IsNullOrWhiteSpace(name))
                        return name;
                }
            }


            // 4. BepInPlugin fallback
            var bep = asm.GetCustomAttribute<BepInPlugin>();
            if (bep != null && !string.IsNullOrWhiteSpace(bep.Name))
                return bep.Name;

            if (!asm.FullName.IsNullOrWhiteSpace()) return asm.FullName;

            // 5. Final fallback (no file name, per your request)
            return "Unknown";
        }
    }
}
