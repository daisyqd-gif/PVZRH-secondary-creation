namespace FrameWorkLoader.Loader
{
    /// <summary>Discovers, orders, and initializes mods registered with the loader.</summary>
    public class ModLoader
    {
        /// <summary>Gets the registered mod assemblies and their metadata.</summary>
        public static List<(Assembly asm, ModInfo info)> Mods = new();
        /// <summary>Maps each registered assembly to its mod metadata.</summary>
        public static Dictionary<Assembly, ModInfo> ModByAssembly = new();

        /// <summary>Discovers mods, resolves their load order, and invokes their entry points.</summary>
        public static void OnGameStart()
        {
            Debug.Log("=== Discovering Mods ===");

            // 1. Discover mods
            foreach (var plugin in IL2CPPChainloader.Instance.Plugins)
            {
                var asm = plugin.Value.GetType().Assembly;
                var modAttr = asm.GetCustomAttribute<ModAttribute>();

                if (modAttr != null)
                {
                    Debug.Log($"Found Custom mod: {modAttr.Info.Name}");
                    RegisterAssembly(asm, modAttr.Info);
                }
            }

            // 2. Build name lookup
            var modsByName = Mods.ToDictionary(m => m.info.Name, m => m);

            // 3. Build dependency edges
            List<(string from, string to)> edges = new();
            foreach (var (asm, info) in Mods)
            {
                foreach (var dep in info.Dependencies)
                    edges.Add((info.Name, dep));
            }

            // 4. Validate missing dependencies
            foreach (var (asm, info) in Mods)
            {
                foreach (var dep in info.Dependencies)
                {
                    if (!modsByName.ContainsKey(dep))
                        Debug.LogError($"Mod '{info.Name}' depends on missing mod '{dep}'");
                }
            }

            // 5. Topological sort (dependency resolution)
            var sortedByDeps = TopologicalSort(modsByName, edges);

            // 6. Apply category → priority → danger ordering
            Mods = sortedByDeps
                .OrderBy(m => m.info.Category)
                .ThenBy(m => m.info.Priority)
                .ThenByDescending(m => m.info.Danger)
                .ToList();

            Debug.Log("=== Final Mod Load Order ===");
            foreach (var (asm, info) in Mods)
                Debug.Log($"[{info.Category} | P{info.Priority} | D{info.Danger}] {info.Name}");

            // 7. Load attributes
            Debug.Log("=== Loading Attributes ===");
            foreach (var (asm, info) in Mods)
            {
                if (info.AutoLoadAttributes)
                    AttributeMgr.LoadAllAttributes(asm);
            }

            // 8. Call mod lifecycle
            Debug.Log("=== Calling Mod Load ===");
            foreach (var plugin in IL2CPPChainloader.Instance.Plugins)
            {
                if (plugin.Value is IModEntry entry)
                    entry.OnModLoad();
            }

            Debug.Log("=== Mod Loading Complete ===");
        }

        /// <summary>Adds a mod assembly and its metadata to the loader's registry.</summary>
        /// <param name="asm">The assembly containing the mod.</param>
        /// <param name="info">Metadata describing the mod.</param>
        public static void RegisterAssembly(Assembly asm, ModInfo info)
        {
            Mods.Add((asm, info));
            ModByAssembly[asm] = info;
        }

        // ------------------------------
        // Dependency Resolver
        // ------------------------------
        private static List<(Assembly asm, ModInfo info)> TopologicalSort(
            Dictionary<string, (Assembly asm, ModInfo info)> modsByName,
            List<(string from, string to)> edges)
        {
            // Build adjacency + indegree
            Dictionary<string, List<string>> graph = new();
            Dictionary<string, int> indegree = new();

            foreach (var name in modsByName.Keys)
            {
                graph[name] = new List<string>();
                indegree[name] = 0;
            }

            foreach (var (from, to) in edges)
            {
                if (!modsByName.ContainsKey(to))
                    continue; // missing deps already logged

                graph[to].Add(from);
                indegree[from]++;
            }

            // Kahn's algorithm
            Queue<string> q = new();
            foreach (var kv in indegree)
                if (kv.Value == 0)
                    q.Enqueue(kv.Key);

            List<(Assembly asm, ModInfo info)> result = new();

            while (q.Count > 0)
            {
                string name = q.Dequeue();
                result.Add(modsByName[name]);

                foreach (var next in graph[name])
                {
                    indegree[next]--;
                    if (indegree[next] == 0)
                        q.Enqueue(next);
                }
            }

            // Detect cycles
            if (result.Count != modsByName.Count)
            {
                Debug.LogError("Dependency cycle detected between mods!");
            }

            return result;
        }
    }
}
