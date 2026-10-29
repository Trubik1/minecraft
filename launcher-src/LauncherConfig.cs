using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SteampunkLauncher
{
    /// <summary>Per-player settings, stored in %APPDATA%\SteampunkLauncher\config.json.</summary>
    public class LauncherConfig
    {
        public string Nickname { get; set; }
        public string GameDir { get; set; }
        public int RamMb { get; set; }
        public string JavaPath { get; set; }
        public bool JoinServer { get; set; }
        public bool ShowLog { get; set; }
        public string GraphicsPreset { get; set; } = "Balanced";
        public string ActiveShader { get; set; } = "None";
        public bool DiscordRpcEnabled { get; set; } = true;
        public bool ShowCape { get; set; } = true;

        [JsonIgnore]
        public bool IsFirstRun
        {
            get { return string.IsNullOrEmpty(GameDir) || string.IsNullOrEmpty(Nickname); }
        }

        public static string Dir
        {
            get
            {
                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SteampunkLauncher");
                Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string FilePath
        {
            get { return Path.Combine(Dir, "config.json"); }
        }

        public static LauncherConfig Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var cfg = JsonConvert.DeserializeObject<LauncherConfig>(File.ReadAllText(FilePath));
                    if (cfg != null)
                    {
                        if (cfg.RamMb <= 0) cfg.RamMb = AutoRamMb();
                        return cfg;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось прочитать конфиг: " + ex.Message);
            }

            return new LauncherConfig { RamMb = AutoRamMb(), JoinServer = true };
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось сохранить конфиг: " + ex.Message);
            }
        }

        public int ResolveRamMb()
        {
            if (RamMb > 0) return RamMb;
            return AutoRamMb();
        }

        /// <summary>Half of installed RAM, clamped to 2..8 GB.</summary>
        public static int AutoRamMb()
        {
            long totalMb = TotalRamMb();
            long half = totalMb / 2;
            if (half > 8192) half = 8192;
            if (half < 2048) half = 2048;
            return (int)(half / 512 * 512);
        }

        public static long TotalRamMb()
        {
            try
            {
                ulong total = 0;
                var status = new MEMORYSTATUSEX();
                status.dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(MEMORYSTATUSEX));
                if (GlobalMemoryStatusEx(ref status))
                {
                    total = status.ullTotalPhys;
                    return (long)(total / 1024 / 1024);
                }
            }
            catch
            {
                // ignore, fall back below
            }
            return 8192;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
    }

    public static class Log
    {
        private static readonly object Sync = new object();

        public static string FilePath
        {
            get { return Path.Combine(LauncherConfig.Dir, "launcher.log"); }
        }

        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Warn(string message)
        {
            Write("WARN", message);
        }

        public static void Error(string message)
        {
            Write("ERROR", message);
        }

        public static void Fatal(Exception ex)
        {
            Write("FATAL", ex == null ? "unknown" : ex.ToString());
            System.Windows.Forms.MessageBox.Show(ex == null ? "Неизвестная ошибка." : ex.Message,
                "Steampunk & Swords — ошибка", System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);
        }

        private static void Write(string level, string message)
        {
            string line = string.Format("[{0:HH:mm:ss}] {1} {2}", DateTime.Now, level, message);
            Debug.WriteLine(line);
            try
            {
                lock (Sync)
                {
                    Rotate();
                    File.AppendAllText(FilePath, line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // logging must never break the launcher
            }
        }

        private static void Rotate()
        {
            var info = new FileInfo(FilePath);
            if (info.Exists && info.Length > 4 * 1024 * 1024)
            {
                string old = FilePath + ".old";
                if (File.Exists(old)) File.Delete(old);
                File.Move(FilePath, old);
            }
        }
    }
}
