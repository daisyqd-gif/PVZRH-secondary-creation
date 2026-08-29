using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AssetbundleDestroyer
{
    [BepInPlugin("assetbundledestroyer.bepinexplugin", "AssetbundleDestroyer", "1.0.0")]
    public class AssetbundleDestroyerPlugin : BasePlugin
    {
        public static bool Dumped;

        public override void Load()
        {
            Log.LogInfo("AssetbundleDestroyer loaded. ConfusedByAttribute-filtered dumper active.");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(AssetBundle), "LoadFromMemory", new[] { typeof(byte[]) })]
    public static class LoadFromMemoryPatch
    {
        [HarmonyPrefix]
        public static void Prefix(byte[] binary)
        {
            if (AssetbundleDestroyerPlugin.Dumped || binary == null)
                return;

            // Walk the stack and look for any method with ConfusedByAttribute
            var trace = new StackTrace();
            bool fromConfusedMethod = trace
                .GetFrames()?
                .Select(f => f.GetMethod())
                .Where(m => m != null)
                .Any(m =>
                {
                    try
                    {
                        // Avoid hard reference: match by attribute type name
                        return m.GetCustomAttributes(false)
                                .Any(a => a.GetType().Name.Contains("ConfusedByAttribute"));
                    }
                    catch
                    {
                        return false;
                    }
                }) ?? false;

            if (!fromConfusedMethod)
                return;

            try
            {
                string path = Path.Combine(Paths.GameRootPath, "ConfusedBundle_decrypted.unity3d");
                File.WriteAllBytes(path, binary);
                AssetbundleDestroyerPlugin.Dumped = true;

                Console.WriteLine("[AssetbundleDestroyer] Dumped ConfuserEx-protected AssetBundle to: " + path);
            }
            catch (Exception e)
            {
                Console.WriteLine("[AssetbundleDestroyer] ERROR dumping bundle: " + e);
            }
        }
    }
}
