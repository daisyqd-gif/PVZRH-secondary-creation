using System.Text.Json.Serialization;
using CustomPlantClass.Runtime.Tasks;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine.Networking;

namespace CustomPlantClass.Main
{
    public static class AssetMgr
    {
        private static readonly HttpClient http = new HttpClient();
        // Unified cache for all bundles (file, resource, base64)
        private static readonly Dictionary<string, AssetBundle> _bundles = new();

        // -----------------------------
        //  Utility: Hash Base64 strings
        // -----------------------------
        private static string HashBase64(string base64)
        {
            using var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(base64));
            return Convert.ToHexString(hash); // readable, stable key
        }

        // -----------------------------
        //  Load AssetBundle from Base64
        // -----------------------------
        public static AssetBundle LoadBundleBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return null;

            string key = "base64:" + HashBase64(base64);

            if (_bundles.TryGetValue(key, out var cached))
                return cached;

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(base64);
            }
            catch
            {
                ModLogger.LogInfo(MyPluginInfo.PluginName, "AssetMgr: Invalid Base64 string.");
                return null;
            }

            var bundle = AssetBundle.LoadFromMemory(bytes);
            if (bundle == null)
            {
                ModLogger.LogInfo(MyPluginInfo.PluginName, "AssetMgr: Failed to load AssetBundle from Base64.");
                return null;
            }

            _bundles[key] = bundle;
            return bundle;
        }



        // -----------------------------
        //  Load AssetBundle from file
        // -----------------------------
        public static AssetBundle LoadBundleFromFile(string path, string name, bool deduplicate = true)
        {
            string key = "file:" + name;

            if (deduplicate && _bundles.TryGetValue(key, out var cached))
                return cached;

            if (!File.Exists(path))
            {
                ModLogger.LogInfo(MyPluginInfo.PluginName, $"AssetMgr: File not found: {path}");
                return null;
            }

            // Synchronous load — IL2CPP safe
            var bundle = AssetBundle.LoadFromFile(path);
            if (bundle == null)
            {
                ModLogger.LogInfo(MyPluginInfo.PluginName, $"AssetMgr: Failed to load AssetBundle from file: {path}");
                return null;
            }

            _bundles[key] = bundle;
            return bundle;
        }

        // -----------------------------------------
        //  Load AssetBundle from embedded resources
        // -----------------------------------------
        public static AssetBundle LoadBundleFromResource(Assembly asm, string resourceName, bool deduplicate = true)
        {
            string key = "res:" + resourceName;

            if (deduplicate && _bundles.TryGetValue(key, out var cached))
                return cached;

            Stream stream =
                asm.GetManifestResourceStream(asm.GetName().Name + "." + resourceName) ??
                asm.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                ModLogger.LogInfo(MyPluginInfo.PluginName, $"AssetMgr: Resource not found: {resourceName}");
                return null;
            }

            using var ms = new MemoryStream();
            stream.CopyTo(ms);

            var bundle = AssetBundle.LoadFromMemory(ms.ToArray());
            if (bundle == null)
            {
                ModLogger.LogInfo(MyPluginInfo.PluginName, $"AssetMgr: Failed to load AssetBundle from resource: {resourceName}");
                return null;
            }

            _bundles[key] = bundle;
            return bundle;
        }
        public static T LoadResource<T>(string partialName, Assembly asm, Func<Stream, T> loader)
        {
            var names = asm.GetManifestResourceNames();

            string match = names.FirstOrDefault(n =>
                n.IndexOf(partialName, StringComparison.OrdinalIgnoreCase) >= 0);

            if (match == null)
                throw new Exception($"Embedded resource not found: {partialName}");

            using var stream = asm.GetManifestResourceStream(match);
            return loader(stream);
        }
        public static string LoadStringFromResource(string partialName, Assembly asm)
        {
            return LoadResource(partialName, asm, stream =>
            {
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            });
        }
        public static byte[] LoadBytesFromResource(string partialName, Assembly asm)
        {
            return LoadResource(partialName, asm, stream =>
            {
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                return ms.ToArray();
            });
        }
        public static Texture2D LoadTextureFromResource(string partialName, Assembly asm)
        {
            return LoadResource(partialName, asm, stream =>
            {
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                var data = ms.ToArray();

                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                ImageConversion.LoadImage(tex, data);
                return tex;
            });
        }
        public static Sprite LoadSpriteFromResource(string partialName, Assembly asm)
        {
            var tex = LoadTextureFromResource(partialName, asm);
            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }
        public static T LoadJsonFromResource<T>(string partialName, Assembly asm)
        {
            return LoadResource(partialName, asm, stream =>
            {
                using var reader = new StreamReader(stream);
                string json = reader.ReadToEnd();
                return JsonSerializer.Deserialize<T>(json);
            });
        }
        #nullable enable
        public static async Task<AudioClip?> LoadAudioClipFromResource(string partialName, Assembly asm, AudioType type = AudioType.OGGVORBIS)
        {
            try
            {
                // Load raw bytes from your universal loader
                byte[] data = LoadBytesFromResource(partialName, asm);

                // Write to a temporary file Unity can decode
                string path = Path.Combine(Application.temporaryCachePath, partialName + ".tmp");
                File.WriteAllBytes(path, data);

                string url = "file://" + path;

                var req = UnityWebRequestMultimedia.GetAudioClip(url, type);
                var op = req.SendWebRequest();

                while (!op.isDone)
                    await DelayTask.WaitForFixedUpdate();

                if (req.result != UnityWebRequest.Result.Success)
                    return null;

                AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                req.Dispose(); // manual cleanup

                return clip;
            }
            catch
            {
                return null;
            }
        }
        #nullable disable
        public static async Task<string> DownloadAndConvertToBase64Async(string url)
        {
            try
            {
                var bytes = await http.GetByteArrayAsync(url);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                ModLogger.LogError($"Download failed (are you offline?) : \n{ex.Message}");
                return null;
            }
        }
        public static string GetBase64FromCache(string cacheFolder, string filename, string urlfallback)
        {
            try
            {
                Directory.CreateDirectory(cacheFolder);

                string fullPath = Path.Combine(cacheFolder, filename);

                // 1. Cache hit → return Base64
                if (File.Exists(fullPath))
                {
                    ModLogger.LogInfo($"Loaded from cache: {filename}");
                    byte[] cachedBytes = File.ReadAllBytes(fullPath);
                    return Convert.ToBase64String(cachedBytes);
                }

                // 2. No fallback URL → cannot download
                if (string.IsNullOrWhiteSpace(urlfallback))
                {
                    ModLogger.LogWarn($"Cache miss and no fallback URL for: {filename}");
                    return null;
                }

                // 3. Download Base64 string
                ModLogger.LogInfo($"Downloading and caching: {filename}");
                string base64 = DownloadAndConvertToBase64Async(urlfallback).GetAwaiter().GetResult();

                if (string.IsNullOrEmpty(base64))
                    return null;

                // 4. Convert Base64 → bytes
                byte[] bytes = Convert.FromBase64String(base64);

                // 5. Save to cache
                File.WriteAllBytes(fullPath, bytes);

                // 6. Return Base64
                return base64;
            }
            catch (Exception ex)
            {
                ModLogger.LogError($"Cache error for {filename}: {ex.Message}");
                return null;
            }
        }
        public static T CastAsset<T>(this Object obj) where T : Object => (T)obj;
    }
    public sealed class Asset<T> where T : Object
    {
        public T Obj { get; }
        public string Name { get; }

        public Asset(T obj)
        {
            Obj = obj;
            Name = obj.name;
        }

        public static Asset<T> FromObject(Object obj)
            => new((T)obj);
    }
    public sealed class AssetDispatcher
    {
        private readonly Dictionary<Type, List<(Predicate<Object> match, Action<Object> action)>> _map = new();

        public void Add<T>(Predicate<T> match, Action<T> action) where T : Object
        {
            if (!_map.TryGetValue(typeof(T), out var list))
                list = _map[typeof(T)] = new();

            list.Add((
                obj => match((T)obj),
                obj => action((T)obj)
            ));
        }

        public bool TryInvoke(Object obj)
        {
            var type = obj.GetType();

            if (_map.TryGetValue(type, out var list))
            {
                foreach (var (match, action) in list)
                {
                    if (match(obj))
                    {
                        action(obj);
                        return true;
                    }
                }
            }

            return false;
        }

        public void Switch(AssetBundle bundle)
        {
            foreach (var obj in bundle.LoadAllAssets())
            {
                try
                {
                    TryInvoke(obj);
                }
                catch (Exception e)
                {
                    ModLogger.LogError(e + "\nIs the asset name wrong?");
                }
            }
        }
    }
}
