using System.IO;
using System.Linq;
using Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using UI;
using Unity.VisualScripting;

namespace CustomPlantClass.RogueShootingManager
{
    public class RegistryHelper : MonoBehaviour
    {
        internal static int classIndex=0;
        public static Dictionary<string,CustomRogueShootingConfig> Specs = new();
        public static Dictionary<string,CustomRogueShootingBuff> Buffs = new();
        public static Dictionary<PlantType,HashSet<CustomRogueShootingBuff>> RogueBuffs = new();

        internal static Dictionary<PlantType, ShootingAlmanacPlant> plants = new();
	    internal static Dictionary<PlantType, List<PlantType>> routes = new();
	    internal static Dictionary<PlantType, string> plantRoles = new();
	    internal static Dictionary<PlantType, string> portraitPaths = new();
	    internal static Dictionary<PlantType, Sprite> portraitSprites = new();
	    internal static Dictionary<PlantType, GameObject> portraitPreviews = new();
	    internal static Dictionary<ShootingAlmanacCategory, List<ShootingAlmanacEntry>> categories = new()
        {
            [ShootingAlmanacCategory.Plants] = new(), 
            [ShootingAlmanacCategory.Other] = new(), 
            [ShootingAlmanacCategory.SuperUpgrade] = new(), 
            [ShootingAlmanacCategory.Trials] = new(), 
            [ShootingAlmanacCategory.Expert] = new(), 
            [ShootingAlmanacCategory.Tactics] = new()
        };
        
        public static BaseConfig MakeConfigType(CustomRogueShootingConfig spec)
        {
            string className = Guid.NewGuid().ToString("N");

            Specs.Add(className,spec);

            string template = LoadEmbeddedText("config.txt");

            string source = template
                .Replace("{{GUID}}", className);

            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            var refs = new List<MetadataReference>
            {
                // Load your own assembly
                MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location)
            };

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

            Type t = asm.GetType("RogueClass_"+className);

            ClassInjector.RegisterTypeInIl2Cpp(t);
            var cashe = Specs[className];
            Specs[className] = cashe;

            RogueConfigs[spec.CustomPlantType] = spec;
            
            return (BaseConfig)Activator.CreateInstance(t);
        }
        public static BaseBuff MakeBuffType(CustomRogueShootingBuff buff)
        {
            // 1. Generate stable GUID
            string guid = Guid.NewGuid().ToString("N");

            // 2. Register buff spec in managed registry
            Buffs[guid] = buff;

            // 3. Load template
            string template = LoadEmbeddedText("buff.txt");

            // 4. Replace placeholders
            string source = template.Replace("{{GUID}}", guid);

            // 5. Parse syntax tree
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


            // 7. Compile dynamic assembly
            var compilation = CSharpCompilation.Create(
                $"DynamicBuff_{Guid.NewGuid()}",
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

            // 8. Load assembly
            var asm = Assembly.Load(ms.ToArray());

            // 9. Resolve type name
            string typeName = "RogueClass_" + guid;
            Type t = asm.GetType(typeName);
            if (t == null)
                throw new Exception($"Generated buff type '{typeName}' not found.");

            // 10. Register IL2CPP type
            ClassInjector.RegisterTypeInIl2Cpp(t);

            // 11. Instantiate IL2CPP buff
            var instance = Activator.CreateInstance(t);
            var output = (BaseBuff)instance;
            if (RogueBuffs.TryGetValue(buff.CustomPlantType, out var p))
            {
                p.Add(buff);
            }
            else
            {
                RogueBuffs[buff.CustomPlantType] = new()
                {
                    buff
                };
            }
            return output;
        }
        public static void RegisterBuffsForPlant(PlantType thePlantType, bool damageBuff = true, bool speedBuff = true, bool starUp = true,params CustomRogueShootingBuff[] buffs)
        {
            if (!RogueBuffs.TryGetValue(thePlantType, out var p))
            {
                foreach(var i in buffs)
                {
                    p.Add(i);
                }
                if (damageBuff)
                {
                    p.Add(new()
                    {
                        CustomTitle = "强化：力量",
                        CustomDescription = ""
                    });
                }
                if (speedBuff)
                {
                    p.Add(new()
                    {
                        CustomTitle = "强化：速度",
                        CustomDescription = ""
                    });
                }
                if (starUp)
                {
                    p.Add(new()
                    {
                        CustomTitle = "超进化：星辉",
                        CustomDescription = ""
                    });
                }
            }
            else
            {
                RogueBuffs[thePlantType] = [.. buffs];
                p=RogueBuffs[thePlantType];
                if (damageBuff)
                {
                    p.Add(new()
                    {
                        CustomTitle = "强化：力量",
                        CustomDescription = ""
                    });
                }
                if (speedBuff)
                {
                    p.Add(new()
                    {
                        CustomTitle = "强化：速度",
                        CustomDescription = ""
                    });
                }
                if (starUp)
                {
                    p.Add(new()
                    {
                        CustomTitle = "超进化：星辉",
                        CustomDescription = ""
                    });
                }
            }
        }
        internal static string LoadEmbeddedText(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            using Stream s = asm.GetManifestResourceStream("CustomPlant.RogueShootingManager."+resourceName);
            using StreamReader reader = new StreamReader(s);
            return reader.ReadToEnd();
        }
        internal static Il2CppSystem.Collections.Generic.List<PlantType> experts = new();
        internal static Dictionary<PlantType,BaseConfig> roguePlants = new();
        internal static Il2CppSystem.Collections.Generic.HashSet<PlantType> CustomBasePlants = new();
        public static void AddCustomExpertPlant(PlantType thePlantType, BaseConfig config)
        {
            experts.Add(thePlantType);
            AddCustomRogueShootingPlant(thePlantType,config);
        }
        [Obsolete("This code IS NOT DONE YET, calling it will DO NOTHING. This is a future impl for the 4.0 update")]
        public static void AddCustomExpertPlant(PlantType thePlantType, BaseConfig config, int cost)
        {
            
        }
        public static Dictionary<PlantType,CustomRogueShootingConfig> RogueConfigs = new();
        public static void AddCustomRogueShootingPlant(PlantType thePlantType, BaseConfig config)
        {
            roguePlants.TryAdd(thePlantType,config);
        }
        [Obsolete("This code IS NOT DONE YET, calling it will DO NOTHING. This is a future impl for the 4.0 update")]
        public static void AddCustomRogueShootingPlant(PlantType thePlantType, BaseConfig config, int cost)
        {
            
        }
        public static void AddCustomBaseRogueShootingPlant(PlantType thePlantType, BaseConfig config)
        {
            CustomBasePlants.Add(thePlantType);
            AddCustomRogueShootingPlant(thePlantType,config);
        }
        [Obsolete("This code IS NOT DONE YET, calling it will DO NOTHING. This is a future impl for the 4.0 update")]
        public static void AddCustomBaseRogueShootingPlant(PlantType thePlantType, BaseConfig config, int cost)
        {
            
        }
        internal static Dictionary<PlantType,Il2CppSystem.Collections.Generic.List<PlantType>> evolutionPathways = new();
        /// <summary>
        /// Placeholder
        /// Please register the upgrade buffs separately.
        /// </summary>
        /// <param name="thePlantType">The plant type of the starting plant</param>
        /// <param name="order"> Ascending from base->intermediate->final</param>
        public static void AddCustomEvolutionPathway(PlantType thePlantType, params PlantType[] order)
        {
            // order must begin with the starting plant
            if (order.Length < 2)
            {
                ModLogger.LogError("[EVOLUTION] Pathway must contain at least start -> next");
                return;
            }

            if (order[0] != thePlantType)
            {
                ModLogger.LogError($"[EVOLUTION] First element of order[] must be the starting plant ({thePlantType})");
                return;
            }

            // Build chain: order[i] -> order[i+1]
            for (int i = 0; i < order.Length - 1; i++)
            {
                var parent = order[i];
                var child  = order[i + 1];

                if (!evolutionPathways.TryGetValue(parent, out var list))
                    list = evolutionPathways[parent] = new Il2CppSystem.Collections.Generic.List<PlantType>();

                list.Add(child);

                ModLogger.LogInfo($"[EVOLUTION] {parent} -> {child}");
            }
        }
        [Obsolete("This code IS NOT DONE YET, calling it will DO NOTHING. This is a future impl for the 4.0 update")]
        public static void AddCustomBattleBuff(string name, string desc, int cost, object unknown_type)
        {
            
        }
        internal static List<(string name, string desc, Func<ShootingManager,bool> canGet, Action<ShootingManager> onGet)> CustomMissionBuffs = new();
        public static AdvBuff AddCustomCurseMissionBuff(
            string name_formatted, 
            string desc, 
            Action<ShootingManager> onGet)
        {
            if (string.IsNullOrWhiteSpace(name_formatted))
                throw new ArgumentException("Buff name cannot be empty.", nameof(name));

            // Sanitize input (remove whitespace, weird characters, etc.)
            name_formatted = name_formatted.Trim();

            // Format according to Rogue Shooting’s new naming rules
            string displayTitle = $"试炼：{name_formatted}";
            string internalName = $"试炼 - {name_formatted}：{desc}";

            // Register buff
            BuffID buff = Compatibility.CustomCore_Old.RegisterCustomBuff(
                internalName,
                BuffType.AdvancedBuff,
                () => Board.Instance != null && Board.Instance.boardTag.rogueShooting,
                5000,
                PlantType.ZombieEndoFlame
            );

            // Add to your mission buff list
            CustomMissionBuffs.Add((displayTitle, desc, mgr => !Lawnf.TravelAdvanced(buff), onGet));

            // TODO: return actual AdvBuff once Rogue Shooting update drops
            return buff;
        }
        public static void InjectUpgradeBuff(RSConfigType BasePlant, PlantType resultPlant)
        {
            if(Enum.GetName(BasePlant)==null)
            throw new ArgumentException("Invalid Base plant!");
            // 1. Generate stable GUID
            string guid = Guid.NewGuid().ToString("N");

            // 3. Load template
            string template = LoadEmbeddedText("buffinjector.txt");

            // 4. Replace placeholders
            string source = template
                .Replace("{{GUID}}", guid)
                .Replace("{{configName}}", Enum.GetName(BasePlant))
                .Replace("{{PlantID}}", ((int)resultPlant).ToString());

            // 5. Parse syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var refs = new List<MetadataReference>
            {
                // Load your own assembly
                MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location)
            };

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


            // 7. Compile dynamic assembly
            var compilation = CSharpCompilation.Create(
                $"DynamicBuff_{Guid.NewGuid()}",
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

            // 8. Load assembly
            var asm = Assembly.Load(ms.ToArray());

            // 9. Resolve type name
            string typeName = $"{Enum.GetName(BasePlant)}_Patch_{guid}";
            Type t = asm.GetType(typeName);
            if (t == null)
                throw new Exception($"Generated buff type '{typeName}' not found.");

            Harmony.CreateAndPatchAll(t.Assembly);
        }
        public const string QualitativeChangeText = "质变";
        public const string CurseText = "诅咒";
        public const string ReversedCurseText = "祝福";
        internal static 
        Dictionary<BuffID,(BuffID reversed,Func<Plant,bool> canreverse,Action oncurse,Action onreverse)> CurseBuffInfo = new();
        public static (BuffID curseBuff, BuffID reverseBuff, BaseBuff buffConfig) RegisterCustomCurseBuff(string name, string curseDesc, string reversedDesc, PlantType thePlantType, Func<Plant,bool> CanReverse, Action OnCurse = null, Action OnReverseEvent = null)
        {
            var buffText_curse=$"诅咒-{name}：{curseDesc}";
            var buffName=$"诅咒：{name}";
            var buffText_reversed=$"祝福-{name}：{reversedDesc}";
            BuffID buff_Curse = Compatibility.CustomCore_Old.RegisterCustomBuff(buffText_curse,BuffType.AdvancedBuff,()=>Board.Instance!=null && Board.Instance.boardTag.rogueShooting,5000,thePlantType);
            BuffID buff_Reversed = Compatibility.CustomCore_Old.RegisterCustomBuff(buffText_reversed,BuffType.AdvancedBuff,()=>Board.Instance!=null && Board.Instance.boardTag.rogueShooting,5000,thePlantType);
            BaseBuff rogueBuff = MakeBuffType(new()
            {
                CustomPlantType = thePlantType,
                CustomTitle = buffName,
                CustomDescription = curseDesc+"\n反转效果："+reversedDesc,
                CustomBuffType = ShootingBuffType.CurseBuff,
                CustomOnGet = () => {
                    TravelMgr.Instance.GetNormalBuff(buff_Curse);
                    if(OnCurse!=null)OnCurse();
                    ShootingManager.Instance.AddComponent<CustomShootingCurseComponent>().Init(buff_Curse,buff_Reversed,CanReverse,OnReverseEvent,thePlantType);
                }
            });
            CurseBuffInfo[buff_Curse]=(buff_Reversed,CanReverse,OnCurse!=null ? OnCurse : ()=>{}, OnReverseEvent!=null ? OnReverseEvent : ()=>{});
            //categories[ShootingAlmanacCategory.Trials].Add(new())
            return (buff_Curse,buff_Reversed,rogueBuff);
        }
        public static string GetStringFromRole(Roles role)
        {
            switch (role)
            {
                case Roles.Attacker: return "输出";
                case Roles.Supporter: return "辅助";
                case Roles.Defense: return "防御";
                case Roles.Insta: return "灰烬";
                default:
                case Roles.Producer: return "未知";
            }
        }
        public static string FormatBuffTitle(ShootingBuffType buffType, string Title)
        {
            switch (buffType)
            {
                case ShootingBuffType.General:
                case ShootingBuffType.UniqueUpgrade:
                default:
                return $"强化：{Title}";
                case ShootingBuffType.QualitativeChange:
                return $"质变：{Title}";
                case ShootingBuffType.SuperUpgrade:
                return $"超进化：{Title}";
                case ShootingBuffType.CurseBuff:
                return $"诅咒：{Title}";
                case ShootingBuffType.MissionBuff:
                return $"试炼：{Title}";
            }
        }
        public static (BuffID AdvBuff,BaseBuff buffConfig) RegisterCustomQualitativeChangeBuff(string name, string desc, PlantType thePlantType, Action OnGetBuff = null)
        {
            string name_formatted = $"质变-{name}：";
            BuffID buff = Compatibility.CustomCore_Old.RegisterCustomBuff(name_formatted+desc,BuffType.AdvancedBuff,()=>Board.Instance!=null && Board.Instance.boardTag.rogueShooting,5000,thePlantType);
            BaseBuff rogueBuff = MakeBuffType(new()
            {
                CustomPlantType = thePlantType,
                CustomTitle = $"质变：{name}",
                CustomDescription =desc,
                CustomBuffType = ShootingBuffType.QualitativeChange,
                CustomOnGet = () => {
                    TravelMgr.Instance.GetNormalBuff(buff);
                    if(OnGetBuff.IsNotNull())OnGetBuff();
                }
            });
            return (buff,rogueBuff);
        }
        private static bool IsManagedAssembly(string path)
        {
            try
            {
                // Try to read metadata; if it fails, it's native
                using var stream = File.OpenRead(path);
                using var peReader = new System.Reflection.PortableExecutable.PEReader(stream);

                return peReader.HasMetadata;
            }
            catch
            {
                return false;
            }
        }
    }
}