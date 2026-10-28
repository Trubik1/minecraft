using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SteampunkLauncher
{
    /// <summary>
    /// Keeps one player skin in the launcher folder and publishes it to
    /// CustomSkinLoader (LocalSkin provider) inside the game folder.
    /// </summary>
    public static class SkinStore
    {
        public const string SourceName = "skin.png";

        public static string SourcePath
        {
            get { return Path.Combine(LauncherConfig.Dir, SourceName); }
        }

        public static bool HasSkin
        {
            get { return File.Exists(SourcePath); }
        }

        /// <summary>Loads the stored skin as a 32bpp bitmap, or null.</summary>
        public static Bitmap Load()
        {
            try
            {
                if (!File.Exists(SourcePath)) return null;
                using (var stream = new FileStream(SourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var image = Image.FromStream(stream))
                {
                    var bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Transparent);
                        g.DrawImage(image, 0, 0, image.Width, image.Height);
                    }
                    return bmp;
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось прочитать скин: " + ex.Message);
                return null;
            }
        }

        /// <summary>Validates a PNG and stores it. Returns an error message or null on success.</summary>
        public static string Import(string path)
        {
            try
            {
                if (!File.Exists(path)) return "Файл не найден.";
                if (!string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase))
                    return "Нужен файл PNG.";

                int width, height;
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var image = Image.FromStream(stream))
                {
                    width = image.Width;
                    height = image.Height;
                }

                // Accept the classic 64x64, the legacy 64x32 and HD skins (128x128, 128x64, 256x256 ...).
                if (width != height && width != height * 2)
                    return "Нужен скин Minecraft: 64×64 (или 64×32, 128×128 и т. п.).";
                if (width % 64 != 0 || height % 32 != 0 || width < 64 || height < 32)
                    return "Нужен скин Minecraft: 64×64 (или 64×32, 128×128 и т. п.).";

                using (var source = new Bitmap(path))
                using (var result = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                {
                    using (var g = Graphics.FromImage(result))
                    {
                        g.Clear(Color.Transparent);
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.PixelOffsetMode = PixelOffsetMode.Half;
                        g.CompositingQuality = CompositingQuality.HighSpeed;

                        g.DrawImage(source, 0, 0, width, height);

                        // If legacy, we need to make sure the mirrored parts are populated
                        if (width == height * 2)
                        {
                            // copy the right leg/arm textures into the 64x64-only areas
                            // This needs adjustment for HD skins, but for now we follow original logic
                            CopyMirrored(g, source, new Rectangle(0, 16, 16, 16), new Rectangle(16, 48, 16, 16), width, height);
                            CopyMirrored(g, source, new Rectangle(40, 16, 16, 16), new Rectangle(32, 48, 16, 16), width, height);
                        }
                    }

                    Directory.CreateDirectory(LauncherConfig.Dir);
                    result.Save(SourcePath, ImageFormat.Png);
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Warn("Скин не принят: " + ex.Message);
                return "Не удалось прочитать изображение: " + ex.Message;
            }
        }

        private static void CopyMirrored(Graphics g, Bitmap source, Rectangle from, Rectangle to,
            int sourceWidth, int sourceHeight)
        {
            float scaleX = sourceWidth / 64f;
            float scaleY = sourceHeight / 32f;

            var scaledFrom = new RectangleF(from.X * scaleX, from.Y * scaleY,
                from.Width * scaleX, from.Height * scaleY);

            var state = g.Save();
            try
            {
                g.TranslateTransform(to.Right, to.Y);
                g.ScaleTransform(-1, 1);
                g.DrawImage(source, new RectangleF(0, 0, to.Width, to.Height), scaledFrom, GraphicsUnit.Pixel);
            }
            finally
            {
                g.Restore(state);
            }
        }

        public static bool IsSlim(Bitmap bitmap)
        {
            return SkinPreview.IsSlimSkin(bitmap);
        }

        /// <summary>Copies the skin into the game folder and makes sure LocalSkin is consulted first.</summary>
        public static void ApplyToGame(string gameDir, string nickname)
        {
            try
            {
                if (string.IsNullOrEmpty(gameDir) || !IsValidNick(nickname) || !HasSkin) return;

                string dir = Path.Combine(gameDir, "CustomSkinLoader", "LocalSkin", "skins");
                Directory.CreateDirectory(dir);

                string destination = Path.Combine(dir, nickname + ".png");
                bool changed = !SameContent(SourcePath, destination);
                File.Copy(SourcePath, destination, true);

                EnsureLoadOrder(gameDir);
                if (changed) InvalidateCaches(gameDir);
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось применить скин: " + ex.Message);
            }
        }

        public static void Reset(string gameDir, string nickname)
        {
            try
            {
                if (File.Exists(SourcePath)) File.Delete(SourcePath);

                if (!string.IsNullOrEmpty(gameDir) && IsValidNick(nickname))
                {
                    string destination = Path.Combine(gameDir, "CustomSkinLoader", "LocalSkin", "skins",
                        nickname + ".png");
                    if (File.Exists(destination)) File.Delete(destination);
                }

                InvalidateCaches(gameDir);
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось сбросить скин: " + ex.Message);
            }
        }

        public static void InvalidateCaches(string gameDir)
        {
            if (string.IsNullOrEmpty(gameDir)) return;
            try
            {
                string caches = Path.Combine(gameDir, "CustomSkinLoader", "caches");
                if (!Directory.Exists(caches)) return;
                foreach (var file in Directory.GetFiles(caches))
                {
                    try { File.Delete(file); } catch { }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось очистить кэш скинов: " + ex.Message);
            }
        }

        /// <summary>CustomSkinLoader writes its config itself; move LocalSkin to the top so the chosen skin wins.</summary>
        private static void EnsureLoadOrder(string gameDir)
        {
            try
            {
                string path = Path.Combine(gameDir, "CustomSkinLoader", "CustomSkinLoader.json");
                if (!File.Exists(path)) return;

                var root = JObject.Parse(File.ReadAllText(path));
                var list = root["loadlist"] as JArray;
                if (list == null) return;

                int index = -1;
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i] as JObject;
                    if (item != null && string.Equals((string)item["name"], "LocalSkin", StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        break;
                    }
                }

                if (index == 0) return;

                if (index > 0)
                {
                    var entry = list[index];
                    list.RemoveAt(index);
                    list.Insert(0, entry);
                }
                else
                {
                    list.Insert(0, new JObject
                    {
                        { "name", "LocalSkin" },
                        { "type", "Legacy" },
                        { "checkPNG", false },
                        { "skin", "LocalSkin/skins/{USERNAME}.png" },
                        { "model", "auto" },
                        { "cape", "LocalSkin/capes/{USERNAME}.png" },
                        { "elytra", "LocalSkin/elytras/{USERNAME}.png" }
                    });
                }

                File.WriteAllText(path, root.ToString(Formatting.Indented));
                Log.Info("LocalSkin включён первым в CustomSkinLoader.json");
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось обновить CustomSkinLoader.json: " + ex.Message);
            }
        }

        private static bool SameContent(string left, string right)
        {
            try
            {
                if (!File.Exists(right)) return false;
                var a = new FileInfo(left);
                var b = new FileInfo(right);
                if (a.Length != b.Length) return false;
                return Convert.ToBase64String(File.ReadAllBytes(left)) == Convert.ToBase64String(File.ReadAllBytes(right));
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidNick(string nickname)
        {
            if (string.IsNullOrEmpty(nickname) || nickname.Length < 3 || nickname.Length > 16) return false;
            foreach (char c in nickname)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_')
                    continue;
                return false;
            }
            return true;
        }
    }
}
