using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SteampunkLauncher
{
    public static class ScreenshotFavorites
    {
        private static readonly string FilePath = Path.Combine(LauncherConfig.Dir, "favorites.json");
        private static HashSet<string> _favs;

        public static bool IsFavorite(string filename)
        {
            EnsureLoaded();
            return _favs.Contains(Path.GetFileName(filename));
        }

        public static void ToggleFavorite(string filename)
        {
            EnsureLoaded();
            string name = Path.GetFileName(filename);
            if (_favs.Contains(name)) _favs.Remove(name);
            else _favs.Add(name);
            Save();
        }

        private static void EnsureLoaded()
        {
            if (_favs != null) return;
            try
            {
                if (File.Exists(FilePath))
                {
                    var list = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(FilePath));
                    if (list != null)
                    {
                        _favs = new HashSet<string>(list, StringComparer.OrdinalIgnoreCase);
                        return;
                    }
                }
            }
            catch { }
            _favs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        private static void Save()
        {
            try
            {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(new List<string>(_favs)));
            }
            catch { }
        }
    }
}
