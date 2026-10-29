using System;
using System.Collections.Generic;
using System.IO;

namespace SteampunkLauncher
{
    public static class GraphicsPresets
    {
        public const string Potato = "Potato";     // Слабый ПК / Ноутбук
        public const string Balanced = "Balanced"; // Баланс (Рекомендуемый)
        public const string Ultra = "Ultra";       // Ультра графика

        public static void Apply(string gameDir, string preset)
        {
            if (string.IsNullOrEmpty(gameDir) || !Directory.Exists(gameDir)) return;
            if (string.IsNullOrEmpty(preset)) preset = Balanced;

            try
            {
                ApplyOptionsTxt(gameDir, preset);
                ApplyIrisSettings(gameDir, preset);
                Log.Info("Применен графический пресет: " + preset);
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось применить графический пресет: " + ex.Message);
            }
        }

        private static void ApplyOptionsTxt(string gameDir, string preset)
        {
            string path = Path.Combine(gameDir, "options.txt");
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (File.Exists(path))
            {
                foreach (string line in File.ReadAllLines(path))
                {
                    int colon = line.IndexOf(':');
                    if (colon > 0)
                    {
                        string k = line.Substring(0, colon).Trim();
                        string v = line.Substring(colon + 1).Trim();
                        dict[k] = v;
                    }
                }
            }

            if (preset == Potato)
            {
                dict["renderDistance"] = "6";
                dict["simulationDistance"] = "4";
                dict["graphicsMode"] = "0"; // Fast
                dict["particles"] = "2";    // Minimal
                dict["entityDistanceScaling"] = "0.5";
                dict["biomeBlendRadius"] = "0";
                dict["ao"] = "0";           // Smooth lighting off
            }
            else if (preset == Ultra)
            {
                dict["renderDistance"] = "14";
                dict["simulationDistance"] = "10";
                dict["graphicsMode"] = "2"; // Fabulous
                dict["particles"] = "0";    // All
                dict["entityDistanceScaling"] = "1.25";
                dict["biomeBlendRadius"] = "3";
                dict["ao"] = "2";           // Smooth lighting maximum
            }
            else // Balanced
            {
                dict["renderDistance"] = "9";
                dict["simulationDistance"] = "6";
                dict["graphicsMode"] = "1"; // Fancy
                dict["particles"] = "1";    // Decreased
                dict["entityDistanceScaling"] = "1.0";
                dict["biomeBlendRadius"] = "2";
                dict["ao"] = "2";
            }

            var lines = new List<string>();
            foreach (var kv in dict)
            {
                lines.Add(kv.Key + ":" + kv.Value);
            }
            File.WriteAllLines(path, lines);
        }

        private static void ApplyIrisSettings(string gameDir, string preset)
        {
            string cfgDir = Path.Combine(gameDir, "config");
            string irisCfg = Path.Combine(cfgDir, "iris.properties");
            if (!Directory.Exists(cfgDir)) Directory.CreateDirectory(cfgDir);

            if (preset == Potato)
            {
                // На слабом ноутбуке отключаем шейдеры принудительно, чтобы не крашило и не лагало
                if (File.Exists(irisCfg))
                {
                    string text = File.ReadAllText(irisCfg);
                    text = System.Text.RegularExpressions.Regex.Replace(text, @"enableShaders\s*=\s*true", "enableShaders=false");
                    File.WriteAllText(irisCfg, text);
                }
            }
            else if (preset == Ultra)
            {
                if (File.Exists(irisCfg))
                {
                    string text = File.ReadAllText(irisCfg);
                    text = System.Text.RegularExpressions.Regex.Replace(text, @"enableShaders\s*=\s*false", "enableShaders=true");
                    File.WriteAllText(irisCfg, text);
                }
            }
        }
    }
}
