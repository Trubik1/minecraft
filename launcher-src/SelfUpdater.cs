using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SteampunkLauncher
{
    public class UpdateInfo
    {
        public Version Version { get; set; }
        public string DownloadUrl { get; set; }
        public string Notes { get; set; }
    }

    /// <summary>Checks the copy of the launcher in the repo and swaps it in place.</summary>
    public static class SelfUpdater
    {
        private const string VersionUrl = "https://raw.githubusercontent.com/Trubik1/minecraft/main/launcher/version.txt";
        private const string ExeUrl = "https://raw.githubusercontent.com/Trubik1/minecraft/main/launcher/SteampunkLauncher.exe";

        public static Version CurrentVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public static async Task<UpdateInfo> CheckAsync(CancellationToken ct)
        {
            try
            {
                string text = await HttpUtil.GetStringAsync(
                    VersionUrl + "?t=" + DateTime.UtcNow.Ticks, ct, 2).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(text)) return null;

                var lines = text.Replace("\r", "").Split('\n');
                Version remote = ParseVersion(lines[0].Trim());
                if (remote == null || remote <= CurrentVersion) return null;

                return new UpdateInfo
                {
                    Version = remote,
                    DownloadUrl = ExeUrl,
                    Notes = string.Join("\n", lines).Trim()
                };
            }
            catch (Exception ex)
            {
                Log.Warn("Проверка обновлений лаунчера: " + ex.Message);
            }
            return null;
        }

        public static async Task ApplyAsync(UpdateInfo update, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string dir = Path.GetDirectoryName(exePath);
            string newExe = Path.Combine(dir, "SteampunkLauncher.new.exe");

            progress.Report(new ProgressInfo { Phase = "Обновление лаунчера", Fraction = 0, Detail = "" });
            await HttpUtil.DownloadAsync(update.DownloadUrl, newExe, bytes =>
            {
                progress.Report(new ProgressInfo
                {
                    Phase = "Обновление лаунчера",
                    Fraction = Math.Min(0.99, bytes / (12.0 * 1024 * 1024)),
                    Detail = string.Format("{0:N1} МБ", bytes / 1024.0 / 1024.0)
                });
            }, ct).ConfigureAwait(false);

            string script = Path.Combine(Path.GetTempPath(), "steampunk_launcher_update.cmd");
            var sb = new StringBuilder();
            sb.AppendLine("@echo off");
            sb.AppendLine("chcp 65001 >nul");
            sb.AppendLine("timeout /t 2 /nobreak >nul");
            sb.AppendLine("move /y \"" + exePath + "\" \"" + exePath + ".old\" >nul 2>&1");
            sb.AppendLine("move /y \"" + newExe + "\" \"" + exePath + "\" >nul 2>&1");
            sb.AppendLine("start \"\" \"" + exePath + "\"");
            sb.AppendLine("del \"" + exePath + ".old\" >nul 2>&1");
            sb.AppendLine("del \"%~f0\" >nul 2>&1");
            File.WriteAllText(script, sb.ToString(), Encoding.Default);

            Log.Info("Запуск скрипта обновления лаунчера");
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c \"" + script + "\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            });

            await Task.Delay(500).ConfigureAwait(false);
        }

        public static void CleanupLeftovers()
        {
            try
            {
                string exePath = Assembly.GetExecutingAssembly().Location;
                string old = exePath + ".old";
                if (File.Exists(old)) File.Delete(old);
                string pending = Path.Combine(Path.GetDirectoryName(exePath), "SteampunkLauncher.new.exe");
                if (File.Exists(pending)) File.Delete(pending);
            }
            catch
            {
                // will be retried next start
            }
        }

        private static Version ParseVersion(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return null;
            var digits = new string(tag.Where(c => char.IsDigit(c) || c == '.').ToArray()).Trim('.');
            var parts = digits.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return null;
            int major, minor, build = 0;
            if (!int.TryParse(parts[0], out major) || !int.TryParse(parts[1], out minor)) return null;
            if (parts.Length > 2) int.TryParse(parts[2], out build);
            return new Version(major, minor, build);
        }
    }
}
