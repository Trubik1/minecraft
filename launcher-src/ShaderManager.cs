using System;
using System.Collections.Generic;
using System.IO;

namespace SteampunkLauncher
{
    public static class ShaderManager
    {
        public const string None = "Без шейдеров (Выключено)";

        public static List<string> GetInstalledShaders(string gameDir)
        {
            var list = new List<string> { None };
            if (string.IsNullOrEmpty(gameDir) || !Directory.Exists(gameDir)) return list;

            string shaderDir = Path.Combine(gameDir, "shaderpacks");
            if (Directory.Exists(shaderDir))
            {
                foreach (string file in Directory.GetFiles(shaderDir, "*.zip"))
                {
                    list.Add(Path.GetFileName(file));
                }
                foreach (string dir in Directory.GetDirectories(shaderDir))
                {
                    list.Add(Path.GetFileName(dir));
                }
            }

            return list;
        }

        public static string GetCurrentShader(string gameDir)
        {
            if (string.IsNullOrEmpty(gameDir)) return None;
            string irisCfg = Path.Combine(gameDir, "config", "iris.properties");
            if (!File.Exists(irisCfg)) return None;

            try
            {
                bool enabled = true;
                string pack = "";
                foreach (string line in File.ReadAllLines(irisCfg))
                {
                    string t = line.Trim();
                    if (t.StartsWith("enableShaders", StringComparison.OrdinalIgnoreCase))
                    {
                        enabled = t.EndsWith("true", StringComparison.OrdinalIgnoreCase);
                    }
                    else if (t.StartsWith("shaderPack", StringComparison.OrdinalIgnoreCase))
                    {
                        int eq = t.IndexOf('=');
                        if (eq > 0) pack = t.Substring(eq + 1).Trim();
                    }
                }

                if (!enabled || string.IsNullOrEmpty(pack)) return None;
                return pack;
            }
            catch
            {
                return None;
            }
        }

        public static void SetActiveShader(string gameDir, string shaderName)
        {
            if (string.IsNullOrEmpty(gameDir)) return;
            string cfgDir = Path.Combine(gameDir, "config");
            string irisCfg = Path.Combine(cfgDir, "iris.properties");
            if (!Directory.Exists(cfgDir)) Directory.CreateDirectory(cfgDir);

            try
            {
                if (shaderName == None || string.IsNullOrEmpty(shaderName))
                {
                    File.WriteAllText(irisCfg, "enableShaders=false\nshaderPack=\n");
                }
                else
                {
                    File.WriteAllText(irisCfg, "enableShaders=true\nshaderPack=" + shaderName + "\n");
                }
                Log.Info("Активный шейдер установлен: " + shaderName);
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось сохранить шейдер: " + ex.Message);
            }
        }
    }
}
