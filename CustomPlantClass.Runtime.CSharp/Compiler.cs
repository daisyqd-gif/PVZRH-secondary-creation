namespace CustomPlantClass.Runtime.CSharp
{
    public static class Compile
    {
        private static uint token = 0;
        public static List<MetadataReference> References { get; private set; } = null;
        public static List<ICompilableScript> CompilableScripts = new();
        #nullable enable
        private static List<string[]> DllPaths =
        [
            ["dotnet"],
            ["BepInEx", "core"],
            ["BepInEx", "interop"],
            ["BepInEx", "plugins"],
        ];
        public static Assembly? RunCSharpScript(string content)
        {
            Plugin.Instance.Logger.LogInfo("Compiling script...");
            Console.Write("[");
            var syntaxTree = CSharpSyntaxTree.ParseText(content);
            Console.Write("*");
            if(References == null)
            {
                References = new List<MetadataReference>
                {
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
                };
                foreach (var arr in DllPaths)
                {
                    var path = Path.Combine([Environment.CurrentDirectory, .. arr]);
                    foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    {
                        if (!file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) continue;
                        try
                        {
                            AssemblyName.GetAssemblyName(file);
                        }
                        catch (BadImageFormatException) { continue; }
                        catch (FileNotFoundException) { continue; }
                        catch (FileLoadException) { continue; }
                        References.Add(MetadataReference.CreateFromFile(file));
                    }
                }
                Console.Write("*");
            }

            var compilation = CSharpCompilation.Create(
                $"CustomizeLibDynamicScript{Interlocked.Increment(ref token)}",
                new[] { syntaxTree },
                References,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );
            Console.Write("*");

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            Console.Write("*");
            Console.Write("]");

            Plugin.Instance.Logger.LogWarning("Compile warnings:");
            foreach (var diagnostic in result.Diagnostics
                            .Where(d => d.Severity == DiagnosticSeverity.Warning))
            {
                Plugin.Instance.Logger.LogWarning($"  {diagnostic.Id}: {diagnostic.GetMessage()}");
            }
            if (!result.Success)
            {
                Plugin.Instance.Logger.LogError("Compile errors:");
                foreach (var diagnostic in result.Diagnostics
                             .Where(d => d.Severity == DiagnosticSeverity.Error))
                {
                    Plugin.Instance.Logger.LogError($"  {diagnostic.Id}: {diagnostic.GetMessage()}");
                }
                return null;
            }
            Plugin.Instance.Logger.LogInfo("Compile information:");
            foreach (var diagnostic in result.Diagnostics
                            .Where(d => d.Severity == DiagnosticSeverity.Info))
            {
                Plugin.Instance.Logger.LogInfo($"  {diagnostic.Id}: {diagnostic.GetMessage()}");
            }

            ms.Seek(0, SeekOrigin.Begin);
            return Assembly.Load(ms.ToArray());
        }

        public static void CallMethod(Assembly assembly)
        {
            try
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray()!;
                }

                bool found = false;
                foreach (var type in types)
                {
                    if  ( typeof(ICompilableScript).IsAssignableFrom(type) )
                    {
                        var instance = Activator.CreateInstance(type) as ICompilableScript;
                        CompilableScripts.Add(instance);
                        var method = type.GetMethod("Main",BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                        if (method != null)
                        {
                            object? target = method.IsStatic ? null : instance;
                            method.Invoke(target, null);
                            found = true;
                            Plugin.Instance.Logger.LogInfo($"Called Main() method in script: {instance!.Name}");
                            break;
                        }
                    }
                }
                if (!found) Plugin.Instance.Logger.LogWarning($"Could not find method in script! (The signature of method Main should be public void Main() and the class should implement ICompilableScript)");
            }
            catch (Exception ex)
            {
                Plugin.Instance.Logger.LogError($"Failed to call Main() method: {ex.Message}");
            }
        }
        public static List<Assembly> LoadAllScripts()
        {
            CompilableScripts.Clear();
            var assemblies = new List<Assembly>();

            foreach (var file in EnumerateScripts())
            {
                try
                {
                    string content = File.ReadAllText(file);
                    var asm = RunCSharpScript(content);

                    if (asm != null)
                    {
                        assemblies.Add(asm);
                        Plugin.Instance.Logger.LogInfo($"Loaded script: {Path.GetFileName(file)}");
                    }
                    else
                    {
                        Plugin.Instance.Logger.LogError($"Failed to compile script: {file}");
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Instance.Logger.LogError($"Exception while loading script {file}: {ex}");
                }
            }

            return assemblies;
        }
        public static IEnumerable<string> EnumerateScripts()
        {
            EnsureFolder();
            EnsureProject();
            return Directory.EnumerateFiles(Folder, "*.cs", SearchOption.AllDirectories)
                            .Where(f => !f.Contains("AssemblyInfo") &&
                                        !f.Contains("AssemblyAttributes") &&
                                        !f.Contains(".NET"));
        }
        public static void ExecuteAllScripts(List<Assembly> assemblies)
        {
            foreach (var asm in assemblies)
            {
                CallMethod(asm);
            }
        }
        public static readonly string Folder =
            Path.Combine(Paths.PluginPath, "ModScripts");
        public static void EnsureFolder()
        {
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
        }
        public static void EnsureProject()
        {
            if(!File.Exists(Path.Combine(Paths.PluginPath,"ModScripts","Project.csproj")))
            {
                Plugin.Instance.Logger.LogWarning("Project.csproj not found, creating a new one.");
                var asm = Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream("CustomPlantClass.Runtime.CSharp.Project.txt");
                using var reader = new StreamReader(stream!);
                string project = reader.ReadToEnd();
                File.WriteAllText("Project.csproj", project);
                Plugin.Instance.Logger.LogInfo("Project.csproj created successfully.");
            }
        }
        public static void ReloadAllScripts()
        {
            foreach( var i in CompilableScripts)
            {
                i.Dispose();
            }
            var assemblies = LoadAllScripts();
            ExecuteAllScripts(assemblies);
        }
    }
    public interface ICompilableScript : IDisposable
    {
        public string Name { get; }
        public void Main();
    }
}
