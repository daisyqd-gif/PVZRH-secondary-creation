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
            BossMusicManager.Clip_Jungle = GetAsset<AudioClip>(assetBundle, "LC_minigame");
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
        public static AudioClip Clip_Jungle { get; set; }
        public const MusicType JacksonBoss = (MusicType)2141983;
        public const MusicType HorseBoss = (MusicType)2141984;
        public const MusicType BigWaveBeach_DMG = (MusicType)2141985;
        public const MusicType AncientEgypt_FinalWave = (MusicType)2141986;
        public const MusicType LostCity_DMG = (MusicType)2141987;
    }
    [HarmonyPatch(typeof(CreateZombie), nameof(CreateZombie.SetZombie))]
    public static class CreateZombie_SetZombie_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ZombieType theZombieType, ref Zombie __result)
        {
            if (!GameAPP.soundManager.musics.ContainsKey(BossMusicManager.JacksonBoss) || GameAPP.soundManager.musics[BossMusicManager.JacksonBoss] != null)
            {
                GameAPP.soundManager.musics[BossMusicManager.JacksonBoss] = BossMusicManager.Clip;
            }
            if (!GameAPP.soundManager.musics.ContainsKey(BossMusicManager.HorseBoss) || GameAPP.soundManager.musics[BossMusicManager.HorseBoss] != null)
            {
                GameAPP.soundManager.musics[BossMusicManager.HorseBoss] = BossMusicManager.Clip_Horse;
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
                GameAPP.Instance.PlayMusic(BossMusicManager.JacksonBoss);
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
                GameAPP.Instance.PlayMusic(BossMusicManager.HorseBoss);
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
            if (!GameAPP.soundManager.musics.ContainsKey(BossMusicManager.BigWaveBeach_DMG) || GameAPP.soundManager.musics[BossMusicManager.BigWaveBeach_DMG] != null)
            {
                GameAPP.soundManager.musics[BossMusicManager.BigWaveBeach_DMG] = BossMusicManager.Clip_Beach;
            }
            if (!GameAPP.soundManager.musics.ContainsKey(BossMusicManager.LostCity_DMG) || GameAPP.soundManager.musics[BossMusicManager.LostCity_DMG] != null)
            {
                GameAPP.soundManager.musics[BossMusicManager.LostCity_DMG] = BossMusicManager.Clip_Jungle;
            }
            if(ShootingManager.Instance!=null && ShootingManager.Instance.scene1 == SceneType.NormalBeach)
            {
                GameAPP.Instance.PlayMusic(BossMusicManager.BigWaveBeach_DMG);
            }
            else if(ShootingManager.Instance!=null && ShootingManager.Instance.scene1 == SceneType.Swamp)
            {
                GameAPP.Instance.PlayMusic(BossMusicManager.LostCity_DMG);
            }
        }
    }
    [HarmonyPatch(typeof(GameAPP), nameof(GameAPP.PlayMusic))]
    public static class GameAP_PlayMusic_Patch
    {
        [HarmonyPostfix]
        public static void Prefix(ref MusicType id)
        {
            if (!GameAPP.soundManager.musics.ContainsKey(BossMusicManager.AncientEgypt_FinalWave) || GameAPP.soundManager.musics[BossMusicManager.AncientEgypt_FinalWave] != null)
            {
                GameAPP.soundManager.musics[BossMusicManager.AncientEgypt_FinalWave] = BossMusicManager.Clip_LiuKun;
            }
            if(ShootingManager.Instance != null && id == MusicType.Day && ShootingManager.Instance.scene3 == SceneType.Desert && ShootingManager.Instance.stage == 3)
            {
                id=BossMusicManager.AncientEgypt_FinalWave;
            }
        }
    }
}
