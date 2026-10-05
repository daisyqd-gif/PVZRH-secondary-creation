using CustomPlantClass.Runtime.Tasks;
using UnityEngine.Networking;

namespace CustomPlantClass.Main
{
    /// <summary>Loads and caches asset bundles and assets embedded in assemblies or stored remotely.</summary>
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
        /// <summary>Loads an asset bundle from a Base64 string, reusing a cached bundle when available.</summary>
        /// <param name="base64">The Base64-encoded asset bundle.</param>
        /// <returns>The loaded bundle, or <see langword="null"/> if the input is invalid or loading fails.</returns>
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
        /// <summary>Loads an asset bundle from a file, optionally reusing a cached bundle.</summary>
        /// <param name="path">The path to the asset bundle file.</param>
        /// <param name="name">The cache name used to identify the bundle.</param>
        /// <param name="deduplicate">Whether to return an existing cached bundle with the same name.</param>
        /// <returns>The loaded bundle, or <see langword="null"/> if the file is unavailable or loading fails.</returns>
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
        /// <summary>Loads an asset bundle from an assembly resource, optionally reusing a cached bundle.</summary>
        /// <param name="asm">The assembly containing the embedded resource.</param>
        /// <param name="resourceName">The embedded resource name.</param>
        /// <param name="deduplicate">Whether to return an existing cached bundle with the same resource name.</param>
        /// <returns>The loaded bundle, or <see langword="null"/> if the resource is unavailable or loading fails.</returns>
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
        /// <summary>Finds an embedded resource by partial name and passes its stream to a loader.</summary>
        /// <typeparam name="T">The type returned by the loader.</typeparam>
        /// <param name="partialName">A case-insensitive substring of the embedded resource name.</param>
        /// <param name="asm">The assembly containing the resource.</param>
        /// <param name="loader">The function that reads the resource stream and creates a value.</param>
        /// <returns>The value produced by the loader.</returns>
        /// <exception cref="Exception">No embedded resource name contains <paramref name="partialName"/>.</exception>
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
        /// <summary>Reads an embedded resource as text.</summary>
        /// <param name="partialName">A case-insensitive substring of the embedded resource name.</param>
        /// <param name="asm">The assembly containing the resource.</param>
        /// <returns>The resource contents as a string.</returns>
        public static string LoadStringFromResource(string partialName, Assembly asm)
        {
            return LoadResource(partialName, asm, stream =>
            {
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            });
        }
        /// <summary>Reads an embedded resource into a byte array.</summary>
        /// <param name="partialName">A case-insensitive substring of the embedded resource name.</param>
        /// <param name="asm">The assembly containing the resource.</param>
        /// <returns>The resource contents as bytes.</returns>
        public static byte[] LoadBytesFromResource(string partialName, Assembly asm)
        {
            return LoadResource(partialName, asm, stream =>
            {
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                return ms.ToArray();
            });
        }
        /// <summary>Loads an embedded image resource into a texture.</summary>
        /// <param name="partialName">A case-insensitive substring of the embedded resource name.</param>
        /// <param name="asm">The assembly containing the resource.</param>
        /// <returns>The texture decoded from the resource.</returns>
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
        /// <summary>Loads an embedded image resource and creates a centered sprite from it.</summary>
        /// <param name="partialName">A case-insensitive substring of the embedded resource name.</param>
        /// <param name="asm">The assembly containing the resource.</param>
        /// <returns>A sprite created from the loaded texture.</returns>
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
        /// <summary>Deserializes an embedded JSON resource into the specified type.</summary>
        /// <typeparam name="T">The type to deserialize the JSON into.</typeparam>
        /// <param name="partialName">A case-insensitive substring of the embedded resource name.</param>
        /// <param name="asm">The assembly containing the resource.</param>
        /// <returns>The deserialized value, or the default value if deserialization yields no value.</returns>
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
        /// <summary>Loads an embedded audio resource and decodes it as an audio clip.</summary>
        /// <param name="partialName">A case-insensitive substring of the embedded resource name.</param>
        /// <param name="asm">The assembly containing the audio resource.</param>
        /// <param name="type">The audio encoding type used by Unity's decoder.</param>
        /// <returns>A task containing the decoded clip, or <see langword="null"/> if loading fails.</returns>
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
        /// <summary>Downloads content from a URL and converts the response bytes to Base64.</summary>
        /// <param name="url">The URL to download.</param>
        /// <returns>A task containing the Base64 content, or <see langword="null"/> if the download fails.</returns>
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
        /// <summary>Loads Base64 content from a local cache or downloads and caches it from a fallback URL.</summary>
        /// <param name="cacheFolder">The directory in which the cached file is stored.</param>
        /// <param name="filename">The cache filename.</param>
        /// <param name="urlfallback">The URL to use when the cache file is missing.</param>
        /// <returns>The content as Base64, or <see langword="null"/> if it cannot be loaded or downloaded.</returns>
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
        /// <summary>Casts a Unity object to the requested Unity object type.</summary>
        /// <typeparam name="T">The target Unity object type.</typeparam>
        /// <param name="obj">The object to cast.</param>
        /// <returns>The object cast to <typeparamref name="T"/>.</returns>
        public static T CastAsset<T>(this Object obj) where T : Object => (T)obj;
    }
    /// <summary>Holds a Unity asset together with its name.</summary>
    /// <typeparam name="T">The Unity object type of the asset.</typeparam>
    public sealed class Asset<T> where T : Object
    {
        /// <summary>Gets the wrapped Unity object.</summary>
        public T Obj { get; }
        /// <summary>Gets the name of the wrapped Unity object.</summary>
        public string Name { get; }

        /// <summary>Creates an asset wrapper for a Unity object.</summary>
        /// <param name="obj">The object to wrap.</param>
        public Asset(T obj)
        {
            Obj = obj;
            Name = obj.name;
        }

        /// <summary>Wraps a Unity object as an asset of the requested type.</summary>
        /// <param name="obj">The Unity object to wrap.</param>
        /// <returns>An asset wrapper containing the object and its name.</returns>
        public static Asset<T> FromObject(Object obj)
            => new((T)obj);
    }
    /// <summary>Dispatches loaded assets to registered handlers based on their runtime type and predicates.</summary>
    public sealed class AssetDispatcher
    {
        private readonly Dictionary<Type, List<(Predicate<Object> match, Action<Object> action)>> _map = new();

        /// <summary>Registers a predicate and action for assets of a specified Unity object type.</summary>
        /// <typeparam name="T">The Unity object type handled by the registration.</typeparam>
        /// <param name="match">Determines whether an asset should be handled by the action.</param>
        /// <param name="action">The action to invoke for matching assets.</param>
        public void Add<T>(Predicate<T> match, Action<T> action) where T : Object
        {
            if (!_map.TryGetValue(typeof(T), out var list))
                list = _map[typeof(T)] = new();

            list.Add((
                obj => match((T)obj),
                obj => action((T)obj)
            ));
        }

        /// <summary>Invokes the first registered handler matching an asset's exact runtime type and predicate.</summary>
        /// <param name="obj">The asset to dispatch.</param>
        /// <returns><see langword="true"/> if a handler matched and was invoked; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>Loads all assets in a bundle and dispatches each to its registered handler.</summary>
        /// <param name="bundle">The bundle whose assets should be dispatched.</param>
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
