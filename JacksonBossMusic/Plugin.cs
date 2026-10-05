using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using System.IO;
using Il2CppInterop.Runtime.Injection;
using Unity.VisualScripting;
using GameLevel.RogueShooting;
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
            ClassInjector.RegisterTypeInIl2Cpp<Await_Component_2>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            var assetBundle = GetAssetBundle(Assembly.GetExecutingAssembly(), "rsmusic");
            BossMusicManager.Clip = GetAsset<AudioClip>(assetBundle, "BeatIt");
            BossMusicManager.Clip_Horse = GetAsset<AudioClip>(assetBundle, "HorseBoss");
            BossMusicManager.Clip_LiuKun = GetAsset<AudioClip>(assetBundle, "LiuKun_BGM");
            BossMusicManager.Clip_Beach = GetAsset<AudioClip>(assetBundle, "BWB_DMG");
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
        public static AudioClip Clip_Horse { get; set; }
        public static AudioClip Clip_LiuKun { get; set; }
        public static AudioClip Clip_Beach { get; set; }
    }
    [HarmonyPatch(typeof(CreateZombie), nameof(CreateZombie.SetZombie))]
    public static class CreateZombie_SetZombie_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ZombieType theZombieType, ref Zombie __result)
        {
            if (!GameAPP.soundManager.musics.ContainsKey((MusicType)2141983) || GameAPP.soundManager.musics[(MusicType)2141983] != null)
            {
                GameAPP.soundManager.musics[(MusicType)2141983] = BossMusicManager.Clip;
            }
            if (!GameAPP.soundManager.musics.ContainsKey((MusicType)2141984) || GameAPP.soundManager.musics[(MusicType)2141984] != null)
            {
                GameAPP.soundManager.musics[(MusicType)2141984] = BossMusicManager.Clip_Horse;
            }
            if (theZombieType == ZombieType.JacksonDriverBoss)
            {
                __result.AddComponent<Await_Component>();
            }
            if (theZombieType == ZombieType.HorseBoss)
            {
                __result.AddComponent<Await_Component_2>();
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
    public class Await_Component_2 : MonoBehaviour
    {
        float wait = 0.1f;
        public void FixedUpdate()
        {
            wait-=Time.deltaTime;
            if (wait <= 0)
            {
                GameAPP.Instance.PlayMusic((MusicType)2141984);
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
    [HarmonyPatch(typeof(HorseBoss), nameof(HorseBoss.AnimDestoryHorse))]
    public static class HorseBoss_AnimDestoryHorse_Patch
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
    [HarmonyPatch(typeof(AnimUIOver), nameof(AnimUIOver.Die))]
    public static class AnimUIOver_Die_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!GameAPP.soundManager.musics.ContainsKey((MusicType)2141985) || GameAPP.soundManager.musics[(MusicType)2141985] != null)
            {
                GameAPP.soundManager.musics[(MusicType)2141985] = BossMusicManager.Clip_Beach;
            }
            if(ShootingManager.Instance!=null && ShootingManager.Instance.scene1 == SceneType.NormalBeach)
            {
                GameAPP.Instance.PlayMusic((MusicType)2141985);
            }
        }
    }
    [HarmonyPatch(typeof(Lawnf), nameof(Lawnf.SetMusic))]
    public static class Lawnf_SetMusic_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Board board)
        {
            if (!GameAPP.soundManager.musics.ContainsKey((MusicType)2141986) || GameAPP.soundManager.musics[(MusicType)2141986] != null)
            {
                GameAPP.soundManager.musics[(MusicType)2141986] = BossMusicManager.Clip_LiuKun;
            }
            if(board.sceneType == SceneType.Desert)
            {
                GameAPP.Instance.PlayMusic((MusicType)2141986);
            }
        }
    }
}
