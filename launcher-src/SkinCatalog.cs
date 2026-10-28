using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SteampunkLauncher
{
    /// <summary>
    /// Popular skins fetched from mc-heads.net, which serves the exact skin texture
    /// used by Minecraft for any nickname (and a ready 3D render for the gallery).
    /// Everything is cached in the launcher folder.
    /// </summary>
    public static class SkinCatalog
    {
        public class Entry
        {
            public string Name;
            public string Title;

            public Entry(string name, string title)
            {
                Name = name;
                Title = title;
            }
        }

        public static readonly Entry[] Popular =
        {
            new Entry("Technoblade", "Technoblade"),
            new Entry("Dream", "Dream"),
            new Entry("Notch", "Notch"),
            new Entry("jeb_", "jeb_"),
            new Entry("Dinnerbone", "Dinnerbone"),
            new Entry("Herobrine", "Herobrine"),
            new Entry("Steve", "Steve"),
            new Entry("Alex", "Alex"),
            new Entry("CaptainSparklez", "CaptainSparklez"),
            new Entry("Grian", "Grian")
        };

        public static string CacheDir
        {
            get
            {
                string dir = Path.Combine(LauncherConfig.Dir, "skin-cache");
                Directory.CreateDirectory(dir);
                return dir;
            }
        }

        private static string RenderPath(string name)
        {
            return Path.Combine(CacheDir, "body-" + Safe(name) + ".png");
        }

        private static string SkinPath(string name)
        {
            return Path.Combine(CacheDir, "skin-" + Safe(name) + ".png");
        }

        private static string Safe(string name)
        {
            var chars = name.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                bool ok = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                          (c >= '0' && c <= '9') || c == '_' || c == '-';
                if (!ok) chars[i] = '_';
            }
            return new string(chars).ToLowerInvariant();
        }

        /// <summary>Cached gallery render, or null.</summary>
        public static Bitmap LoadRender(string name)
        {
            try
            {
                string path = RenderPath(name);
                if (!File.Exists(path)) return null;
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var image = Image.FromStream(stream))
                {
                    var copy = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
                    using (var g = Graphics.FromImage(copy)) g.DrawImage(image, 0, 0);
                    return copy;
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Bitmap> FetchRenderAsync(string name, int height, CancellationToken ct)
        {
            string path = RenderPath(name);
            if (!File.Exists(path) || new FileInfo(path).Length < 200)
            {
                bool ok = false;
                // Попытка 1: minotar.net (работает стабильно и быстро)
                try
                {
                    if (File.Exists(path)) File.Delete(path);
                    await HttpUtil.DownloadAsync(
                        "https://minotar.net/armor/body/" + Uri.EscapeDataString(name) + "/" + height + ".png",
                        path, null, ct).ConfigureAwait(false);
                    ok = File.Exists(path) && new FileInfo(path).Length > 200;
                }
                catch { }

                // Попытка 2: mc-heads.net
                if (!ok)
                {
                    try
                    {
                        if (File.Exists(path)) File.Delete(path);
                        await HttpUtil.DownloadAsync(
                            "https://mc-heads.net/body/" + Uri.EscapeDataString(name) + "/" + height,
                            path, null, ct).ConfigureAwait(false);
                        ok = File.Exists(path) && new FileInfo(path).Length > 200;
                    }
                    catch { }
                }

                // Попытка 3: crafatar.com
                if (!ok)
                {
                    try
                    {
                        if (File.Exists(path)) File.Delete(path);
                        await HttpUtil.DownloadAsync(
                            "https://crafatar.com/renders/body/" + Uri.EscapeDataString(name) + "?scale=4",
                            path, null, ct).ConfigureAwait(false);
                    }
                    catch { }
                }
            }
            return LoadRender(name);
        }

        /// <summary>
        /// Downloads the skin texture for a nickname into a temporary file and imports it.
        /// Returns null on success, otherwise an error message.
        /// </summary>
        public static async Task<string> ApplyByNameAsync(string name, CancellationToken ct)
        {
            string cache = SkinPath(name);
            try
            {
                if (File.Exists(cache)) File.Delete(cache);

                bool downloaded = false;
                // Попытка 1: minotar.net
                try
                {
                    await HttpUtil.DownloadAsync(
                        "https://minotar.net/skin/" + Uri.EscapeDataString(name),
                        cache, null, ct).ConfigureAwait(false);
                    downloaded = File.Exists(cache) && new FileInfo(cache).Length > 100;
                }
                catch { }

                // Попытка 2: mc-heads.net
                if (!downloaded)
                {
                    try
                    {
                        await HttpUtil.DownloadAsync(
                            "https://mc-heads.net/skin/" + Uri.EscapeDataString(name),
                            cache, null, ct).ConfigureAwait(false);
                        downloaded = File.Exists(cache) && new FileInfo(cache).Length > 100;
                    }
                    catch { }
                }

                if (!downloaded)
                    return "Не удалось скачать скин «" + name + "». Проверьте никнейм.";
            }
            catch (Exception ex)
            {
                Log.Warn("Скин " + name + ": " + ex.Message);
                return "Не удалось скачать скин «" + name + "»: " + ex.Message;
            }

            string error = SkinStore.Import(cache);
            if (error != null && File.Exists(cache))
            {
                try { File.Delete(cache); } catch { }
            }
            return error;
        }

        /// <summary>True when the nickname exists (the service answers with an image).</summary>
        public static async Task<bool> ExistsAsync(string name, CancellationToken ct)
        {
            try
            {
                using (var response = await HttpUtil.Client.GetAsync(
                           "https://mc-heads.net/avatar/" + Uri.EscapeDataString(name) + "/32",
                           HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
                {
                    return response.IsSuccessStatusCode &&
                           response.Content.Headers.ContentType != null &&
                           response.Content.Headers.ContentType.MediaType.StartsWith("image",
                               StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
