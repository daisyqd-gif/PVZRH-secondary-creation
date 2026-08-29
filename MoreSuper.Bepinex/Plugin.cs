global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using Il2CppInterop.Runtime.Injection;
global using System.Reflection;
global using System.Collections;
global using UnityEngine;
global using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace MoreSuper{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "MoreSuper.Bepinex";
        public const string PluginName = "MoreSuper";
        public const string PluginVersion = "3.4.1";
        public static GameObject FireMeteor = null;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            ClassInjector.RegisterTypeInIl2Cpp<MoreSuper>();
            ClassInjector.RegisterTypeInIl2Cpp<PeaSuper>();
            ClassInjector.RegisterTypeInIl2Cpp<ThreePeaSuper>();
            ClassInjector.RegisterTypeInIl2Cpp<GatlingPeaSuper>();
            // Pea family
            CustomCore.RegisterSuperSkill((int)PlantType.Peashooter,        p => 1000, p => SuperPea(p),   1000);
            CustomCore.RegisterSuperSkill((int)PlantType.JalaPeashooter,    p => 1000, p => SuperPea(p),   1000);
            CustomCore.RegisterSuperSkill((int)PlantType.SnowPeaShooter,    p => 1000, p => SuperPea(p),   1000);
            CustomCore.RegisterSuperSkill((int)PlantType.Cherryshooter,     p => 1000, p => SuperPea(p),   1000);
            CustomCore.RegisterSuperSkill((int)PlantType.DoomPeashooter,    p => 1000, p => SuperPea(p),   1000);
            CustomCore.RegisterSuperSkill((int)PlantType.HypnoPeashooter,   p => 1000, p => SuperPea(p),   1000);

            CustomCore.RegisterSuperSkill((int)PlantType.DoubleShooter,     p => 1000, p => SuperPea(p),   1000);
            CustomCore.RegisterSuperSkill((int)PlantType.JalaDoubleshooter, p => 1000, p => SuperPea(p),   1000);
            CustomCore.RegisterSuperSkill((int)PlantType.DoubleSnow,        p => 1000, p => SuperPea(p),   1000);
            CustomCore.RegisterSuperSkill((int)PlantType.DoubleCherry,      p => 1000, p => SuperPea(p),   1000);
            CustomCore.RegisterSuperSkill((int)PlantType.HypnoRepeater,     p => 1000, p => SuperPea(p),   1000);

            // ThreePea family
            CustomCore.RegisterSuperSkill((int)PlantType.ThreePeater,        p => 1000, p => SuperThree(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.SuperThreePeater,   p => 1000, p => SuperThree(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.GarlicThreePeater,  p => 1000, p => SuperThree(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.CherryThreePeater,  p => 1000, p => SuperThree(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.DarkThreePeater,    p => 1000, p => SuperThree(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.ThreeSquash,        p => 1000, p => SuperThree(p), 1000);

            CustomCore.RegisterSuperSkill((int)PlantType.AllPeater,          p => 1000, p => SuperThree(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.SuperKelp,          p => 1000, p => SuperThree(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.UltimateKelp,       p => 1000, p => SuperThree(p), 1000);

            // Gatling Pea family
            CustomCore.RegisterSuperSkill((int)PlantType.GatlingPea,         p => 1000, p => SuperGatling(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.JalaGatling,        p => 1000, p => SuperGatling(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.SnowGatling,        p => 1000, p => SuperGatling(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.CherryGatling,      p => 1000, p => SuperGatling(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.HypnoGatling,       p => 1000, p => SuperGatling(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.DoomGatling,        p => 1000, p => SuperGatling(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.UltimateDoomGatling,p => 1000, p => SuperGatling(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.UltimateGatling,    p => 1000, p => SuperGatling(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.SnowGatlingPuff,    p => 1000, p => SuperGatling(p), 1000);
            CustomCore.RegisterSuperSkill((int)PlantType.SunGatlingPuff,     p => 1000, p => SuperGatling(p), 1000);



            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
        public void SuperPea(Plant p)
        {
            // Add or get the component using the runtime type
            var pf = p.gameObject.GetOrAddComponent<PeaSuper>();
            pf.StartPF();
        }
        public void SuperThree(Plant p)
        {
            // Add or get the component using the runtime type
            var pf = p.gameObject.GetOrAddComponent<ThreePeaSuper>();
            pf.StartPF();
        }
        public void SuperGatling(Plant p)
        {
            var pf = p.gameObject.GetOrAddComponent<GatlingPeaSuper>();
            pf.StartPF();
        }
    }
}
