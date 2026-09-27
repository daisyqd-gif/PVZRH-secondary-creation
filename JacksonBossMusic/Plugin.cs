using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using System.IO;
using Il2CppInterop.Runtime.Injection;
using Unity.VisualScripting;
namespace JacksonBossMusic
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        private static AssetBundle GetAssetBundle(Assembly assembly, string name)
        {
            try
            {
                using Stream stream =
                    assembly.GetManifestResourceStream(assembly.FullName!.Split(",")[0] + "." + name) ??
                    assembly.GetManifestResourceStream(name)!;
                using MemoryStream stream1 = new();
                stream.CopyTo(stream1);
                var ab = AssetBundle.LoadFromMemory(stream1.ToArray());
                ArgumentNullException.ThrowIfNull(ab);
                return ab;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Failed to load {name} \n{e}");
            }
        }

        private static T GetAsset<T>(AssetBundle ab, string name) where T : UnityEngine.Object
        {
            foreach (var ase in ab.LoadAllAssetsAsync().allAssets)
            {
                if (ase.TryCast<T>()?.name == name)
                {
                    return ase.Cast<T>();
                }
            }
            throw new ArgumentException($"Could not find {name} from {ab.name}");
        }
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<Await_Component>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            var assetBundle = GetAssetBundle(Assembly.GetExecutingAssembly(), "ultimategoldjacksondrivermusic");
            BossMusicManager.Clip = GetAsset<AudioClip>(assetBundle, "UltimateGoldJacksonDriverMusic");
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "JacksonBossMusic.Bepinex";
        public const string PluginName = "JacksonBossMusic";
        public const string PluginVersion = "1.0.0";
    }
    public static class BossMusicManager
    {
        public static AudioClip Clip { get; set; }
    }
    [HarmonyPatch(typeof(CreateZombie), nameof(CreateZombie.SetZombie))]
    public static class Board_Update_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ZombieType theZombieType, ref Zombie __result)
        {
            if (!GameAPP.soundManager.musics.ContainsKey((MusicType)2141983) || GameAPP.soundManager.musics[(MusicType)2141983] != null)
            {
                GameAPP.soundManager.musics[(MusicType)2141983] = BossMusicManager.Clip;
            }
            if (theZombieType == ZombieType.JacksonDriverBoss)
            {
                __result.AddComponent<Await_Component>();
            }
        }
    }
    public class Await_Component : MonoBehaviour
    {
        float wait = 0.1f;
        public void FixedUpdate()
        {
            wait-=Time.deltaTime;
            if (wait <= 0)
            {
                GameAPP.Instance.PlayMusic((MusicType)2141983);
                Destroy(this);
            }
        }
    }
    [HarmonyPatch(typeof(JacksonDriverBoss), nameof(JacksonDriverBoss.DieEvent))]
    public static class JacksonDriverBoss_DieEvent_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(JacksonDriverBoss __instance)
        {
            if (__instance == null || __instance.board == null)
            {
                return;
            }
            Lawnf.SetMusic(__instance.board);
        }
    }
}
