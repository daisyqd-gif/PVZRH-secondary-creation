using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Unity.VisualScripting;

namespace CustomPlantClass.RogueShootingManager
{
    public class RegistryHelper : MonoBehaviour
    {
        internal static int classIndex=0;
        public static Dictionary<string,CustomRogueShootingConfig> Specs = new();
        public static Dictionary<string,CustomRogueShootingBuff> Buffs = new();
        internal static string GetClassName()
        {
            classIndex++;
            return $"CLASS_ROGUESHOOTING_{classIndex}";
        }
        //planning: use the dictionary lookup approach but modify it to use a compile time set string inside the class
        public static BaseConfig MakeConfigType(CustomRogueShootingConfig spec)
        {
            string className = Guid.NewGuid().ToString("N");

            Specs.Add(className,spec);

            string template = LoadEmbeddedText("config.txt");

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

            Type t = asm.GetType("RogueClass_"+className);

            ClassInjector.RegisterTypeInIl2Cpp(t);
            var cashe = Specs[className];
            Specs[className] = cashe;
            
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

            return (BaseBuff)instance;
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
        public static void AddCustomRogueShootingPlant(PlantType thePlantType, BaseConfig config)
        {
            roguePlants.TryAdd(thePlantType,config);
        }
        public static void AddCustomBaseRogueShootingPlant(PlantType thePlantType, BaseConfig config)
        {
            CustomBasePlants.Add(thePlantType);
            AddCustomRogueShootingPlant(thePlantType,config);
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
    public class CustomShootingCurseComponent : MonoBehaviour
    {
        BuffID reversed;
        BuffID curseBuff;
        Func<Plant,bool> canreverse;
        Action onreverse;
        PlantType thePlantType;
        bool done = false;
        public void Init(BuffID curseBuff, BuffID reversed,Func<Plant,bool> canreverse,Action onreverse,PlantType thePlantType)
        {
            this.curseBuff = curseBuff;
            this.reversed=reversed;
            this.canreverse = canreverse;
            this.thePlantType = thePlantType;
            this.onreverse = onreverse;
        }
        public void FixedUpdate()
        {
            if(ShootingManager.Instance.TryGetPlant(thePlantType,out Plant plant) && canreverse(plant) && !done)
            {
                done = true;
                TravelMgr.Instance.GetNormalBuff(reversed);
                if(onreverse!=null)onreverse();
                if (TravelMgr.Instance.data.advBuffs.Contains(curseBuff))
                {
                    TravelMgr.Instance.data.advBuffs.Remove(curseBuff);
                }
                Destroy(this);
            }
        }
    }
    public enum Roles
    {
        Attacker = 0,
        Supporter = 1,
        Defense = 2,
        Insta = 3,
        Producer = 4
    }
    public enum RSConfigType
    {
        Peashooter,
        CherryGatling,
        HelmetGatling,
        //terminal plant, it is not recommended to add entries here
        UltimateGatling,
        //terminal plant, it is not recommended to add entries here
        UltimateHelmetGatling,
        LanternSplit,
        //terminal plant, it is not recommended to add entries here
        UltimateLanternSplit,
        SniperPea,
        //terminal plant, it is not recommended to add entries here
        DoomSniper,
        //terminal plant, it is not recommended to add entries here
        FireSniper,
        SnowPeaShooter,
        //terminal plant, it is not recommended to add entries here
        MagicSnowPea2,
        WallNut,
        SuperChomper,
        //terminal plant, it is not recommended to add entries here
        UltimateChomper,
        TallNut,
        //terminal plant, it is not recommended to add entries here
        UltimateTallNut,
        CabbageNut,
        //terminal plant, it is not recommended to add entries here
        MelonNut,
        //terminal plant, it is not recommended to add entries here
        MagnetNut,
        PotatoMine,
        PeaMine,
        //terminal plant, it is not recommended to add entries here
        ThreeMine,
        Chomper,
        CherryChomper,
        //terminal plant, it is not recommended to add entries here
        DoomChomper,
        BigChomper,
        //terminal plant, it is not recommended to add entries here
        UltimateBigChomper,
        SmallPuff,
        IcePuff,
        //terminal plant, it is not recommended to add entries here
        SnowGatlingPuff,
        IronPuff,
        //terminal plant, it is not recommended to add entries here
        IFVIronPuff,
        FumeShroom,
        IceFumeShroom,
        //terminal plant, it is not recommended to add entries here
        UltimateFume,
        GarlicFume,
        //terminal plant, it is not recommended to add entries here
        UltimatePoisonFume,
        GloomShroom,
        //terminal plant, it is not recommended to add entries here
        UltimateGloom,
        HypnoShroom,
        HypnoNut,
        //terminal plant, it is not recommended to add entries here
        HypnoEmperor,
        ScaredyShroom,
        SuperHypno,
        //terminal plant, it is not recommended to add entries here
        UltimateHypno,
        ScaredyDoom,
        //terminal plant, it is not recommended to add entries here
        UltimateDoomScared,
        Squash,
        Squalour,
        //terminal plant, it is not recommended to add entries here
        CattailLour,
        CherrySquash,
        //terminal plant, it is not recommended to add entries here
        NuclearSquash,
        ThreePeater,
        ThreeSquash,
        //terminal plant, it is not recommended to add entries here
        SuperThreePeater,
        BigGatling,
        //terminal plant, it is not recommended to add entries here
        UltimateBigGatling,
        Caltrop,
        SpikeRock,
        //terminal plant, it is not recommended to add entries here
        ObsidianSpike,
        CaltropNut,
        //terminal plant, it is not recommended to add entries here
        ObsidianWallNut,
        Cactus,
        DoomCactus,
        //terminal plant, it is not recommended to add entries here
        UltimateCactus,
        StarFruit,
        SuperStar,
        //terminal plant, it is not recommended to add entries here
        UltimateStar,
        SwordStar,
        //terminal plant, it is not recommended to add entries here
        AbyssSwordStar,
        Cabbagepult,
        GoldCabbage,
        UltimateCabbage,
        CabbageCannon,
        //terminal plant, it is not recommended to add entries here
        UltimateCabbageCannon,
        Melonpult,
        SuperMelon,
        //terminal plant, it is not recommended to add entries here
        UltimateMelon,
        FireMelon,
        //terminal plant, it is not recommended to add entries here
        UltimateSpring,
        SilverMelon,
        GoldMelon,
        WinterMelon,
        //terminal plant, it is not recommended to add entries here
        UltimateWinterMelon,
        Cornpult,
        PortalCorn,
        //terminal plant, it is not recommended to add entries here
        UltimateCorn,
        Umbrellaleaf,
        LanternUmbrella,
        //terminal plant, it is not recommended to add entries here
        LaserUmbrella,
        Bamboo,
        LotusBamboo,
        //terminal plant, it is not recommended to add entries here
        UltimateBamboo,
        SpruceShooter,
        SuperSpruce,
        //terminal plant, it is not recommended to add entries here
        UltimateSpruce,
        //expert plant, it is not recommended to add entries here
        UltimateSniperGatling,
        //expert plant, it is not recommended to add entries here
        UltimateMinigun,
        //expert plant, it is not recommended to add entries here
        UltimateBlover,
        //expert plant, it is not recommended to add entries here
        EmeraleBlover,
        //expert plant, it is not recommended to add entries here
        UltimateStarTorch
    }
    public struct CustomRogueShootingConfig
    {
        public CustomRogueShootingConfig()
        {
        }

        public PlantType CustomPlantType { get; set; } = PlantType.Nothing;
        public Func<List<BaseBuff>> CustomBuffs { get; set; } = () => new();
        public Action<Plant> CustomReinforcePlant { get; set; } = null;
        public string CustomRole { get; set; } = "";
    }
    public struct CustomRogueShootingBuff
    {
        public CustomRogueShootingBuff()
        {
        }

        public PlantType CustomPlantType { get; set; } = PlantType.Nothing;
        public string CustomTitle { get; set; } = "";
        public string CustomDescription { get; set; } = "";
        public ShootingBuffType CustomBuffType { get; set; } = ShootingBuffType.UniqueUpgrade;
        public Action CustomOnGet { get; set; } = () => {};
    }
    [HarmonyPatch(typeof(ShootingManager))]
    public static class ShootingManagerPatch
    {
        [HarmonyPatch(nameof(ShootingManager.Awake))]
        [HarmonyPrefix]
        public static void PreShootingManager(ShootingManager __instance)
        {
            __instance.AllPlants.Merge(RegistryHelper.CustomBasePlants);
        }
        [HarmonyPatch(nameof(ShootingManager.Awake))]
        [HarmonyPostfix]
        public static void PostShootingManager(ShootingManager __instance)
        {
            __instance.ExpertPlants.Merge(RegistryHelper.experts);
        }

        [HarmonyPatch(nameof(ShootingManager.ShowBuff))]
        [HarmonyPrefix]
        public static void ShowBuff()
        {
            if (Config.configs != null)
            {
                foreach( var i in RegistryHelper.roguePlants )
                {
                    if (!Config.configs.ContainsKey(i.Key))
                    {
                        Config.configs.Add(i.Key, i.Value);
                    }
                }
            }
        }
    }
}