namespace CustomPlantClass.Main.BulletBehaviour
{
    public class BulletRegistryManager : MonoBehaviour
    {
        public static Dictionary<string,CustomBulletMovement> BulletMovements = new();
        public static Dictionary<BulletMoveWay,BulletMovement> GetMovement_Dic = new();
        internal static string LoadEmbeddedText(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            using Stream s = asm.GetManifestResourceStream("CustomPlantClass.Main.BulletBehaviour."+resourceName);
            using StreamReader reader = new StreamReader(s);
            return reader.ReadToEnd();
        }
        //planning: use the dictionary lookup approach but modify it to use a compile time set string inside the class
        public static BulletMovement CreateBulletmovement(BulletMoveWay theMovingWay,CustomBulletMovement spec)
        {
            var output = CreateBulletmovement(spec);
            GetMovement_Dic.Add(theMovingWay,output);
            return output;
        }
        public static BulletMovement CreateBulletmovement(CustomBulletMovement spec)
        {
            string className = Guid.NewGuid().ToString("N");

            BulletMovements.Add(className,spec);

            string template = LoadEmbeddedText("buff.txt");

            string source = template
                .Replace("{{GUID}}", className);

            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            var refs = new List<MetadataReference>();

            // Load your own assembly
            refs.Add(MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location));

            // Load all managed assemblies under GameRoot
            foreach (var dll in Directory.EnumerateFiles(Paths.GameRootPath, "*.dll", SearchOption.AllDirectories))
            {
                if (!IsManagedAssembly(dll))
                    continue;

                try
                {
                    refs.Add(MetadataReference.CreateFromFile(dll));
                }
                catch
                {
                    // skip anything Roslyn still doesn't like
                }
            }

            var compilation = CSharpCompilation.Create(
                $"DynamicConfig_{Guid.NewGuid()}",
                new[] { syntaxTree },
                refs,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            if (!result.Success)
            {
                foreach (var diag in result.Diagnostics)
                    Debug.LogError(diag.ToString());

                throw new Exception("Buff subclass compilation failed.");
            }

            ms.Seek(0, SeekOrigin.Begin);
            var asm = Assembly.Load(ms.ToArray());

            Type t = asm.GetType("MoveWay_"+className);

            ClassInjector.RegisterTypeInIl2Cpp(t);
            var cache = BulletMovements[className];
            BulletMovements[className] = cache;
            return (BulletMovement)Activator.CreateInstance(t);
        }
        private static bool IsManagedAssembly(string path)
        {
            try
            {
                // Try to read metadata; if it fails, it's native
                using var stream = File.OpenRead(path);
                using var peReader = new PEReader(stream);

                return peReader.HasMetadata;
            }
            catch
            {
                return false;
            }
        }
    }
    public struct CustomBulletMovement
    {
        public Action<Bullet> PositionUpdate = (b) => {};
        public Action<Bullet> ShadowUpdate = (b) => {};
        public Action<Bullet> UpdateHitFilter = (b) => {};

        public CustomBulletMovement()
        {
        }
    }
    [HarmonyPatch(typeof(BulletMovement))]
    public static class ShootingManagerPatch
    {
        [HarmonyPatch(nameof(BulletMovement.GetMovement))]
        [HarmonyPrefix]
        public static bool GetMovement_Prefix(BulletMoveWay moveWay, ref BulletMovement __result)
        {
            if(BulletRegistryManager.GetMovement_Dic.TryGetValue(moveWay,out __result))
            {
                return false;
            }
            return true;
        }
    }
}