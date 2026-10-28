using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SteampunkLauncher
{
    /// <summary>Downloads and locates a Java 21 runtime (Temurin JRE) for the game.</summary>
    public static class JavaInstaller
    {
        private const string TemurinUrl =
            "https://api.adoptium.net/v3/binary/latest/21/ga/windows/x64/jre/hotspot/normal/eclipse";

        private const long ExpectedSize = 50L * 1024 * 1024;

        public static string EnsureJavaw(string javaPath)
        {
            if (string.IsNullOrEmpty(javaPath)) return javaPath;
            if (javaPath.EndsWith("javaw.exe", StringComparison.OrdinalIgnoreCase)) return javaPath;
            try
            {
                string dir = Path.GetDirectoryName(javaPath);
                if (!string.IsNullOrEmpty(dir))
                {
                    string javaw = Path.Combine(dir, "javaw.exe");
                    if (File.Exists(javaw)) return javaw;
                }
            }
            catch { }
            if (javaPath.EndsWith("java.exe", StringComparison.OrdinalIgnoreCase))
            {
                return javaPath.Substring(0, javaPath.Length - 8) + "javaw.exe";
            }
            return javaPath;
        }

        public static async Task<string> EnsureJavaAsync(string gameDir, string configuredPath,
            IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            if (IsUsable(configuredPath)) return EnsureJavaw(configuredPath);

            string found = FindJava(gameDir);
            if (found != null)
            {
                Log.Info("Найдена Java 21: " + found);
                return EnsureJavaw(found);
            }

            progress.Report(new ProgressInfo { Phase = "Скачивание Java 21", Fraction = 0, Detail = "Temurin JRE" });
            Log.Info("Java 21 не найдена, скачиваем Temurin JRE");

            string runtimeDir = Path.Combine(gameDir, "runtime");
            Directory.CreateDirectory(runtimeDir);
            string zipPath = Path.Combine(runtimeDir, "temurin21.zip");
            string tempZip = zipPath + ".part";

            long lastReport = 0;
            await HttpUtil.DownloadAsync(TemurinUrl, tempZip, bytes =>
            {
                long now = DateTime.UtcNow.Ticks;
                if (now - lastReport < TimeSpan.FromMilliseconds(200).Ticks) return;
                lastReport = now;
                progress.Report(new ProgressInfo
                {
                    Phase = "Скачивание Java 21",
                    Fraction = Math.Min(0.98, (double)bytes / ExpectedSize),
                    Detail = string.Format("{0:N1} МБ", bytes / 1024.0 / 1024.0)
                });
            }, ct).ConfigureAwait(false);

            if (File.Exists(zipPath)) File.Delete(zipPath);
            File.Move(tempZip, zipPath);

            progress.Report(new ProgressInfo { Phase = "Распаковка Java 21", Fraction = 0.99, Detail = "" });
            string target = Path.Combine(runtimeDir, "temurin21");
            if (Directory.Exists(target)) Directory.Delete(target, true);
            Directory.CreateDirectory(target);
            ZipFile.ExtractToDirectory(zipPath, target);
            TryDelete(zipPath);

            string java = FindJavaExe(target);
            if (java == null || !IsJava21(java))
                throw new InvalidOperationException("Не удалось установить Java 21 (javaw.exe не найден после распаковки).");

            string finalJava = EnsureJavaw(java);
            Log.Info("Java 21 установлена: " + finalJava);
            return finalJava;
        }

        public static string FindJava(string gameDir)
        {
            string runtimeDir = Path.Combine(gameDir, "runtime");
            if (!Directory.Exists(runtimeDir)) return null;
            try
            {
                foreach (var exe in Directory.EnumerateFiles(runtimeDir, "java.exe", SearchOption.AllDirectories))
                {
                    if (IsJava21(exe)) return EnsureJavaw(exe);
                }
                foreach (var exe in Directory.EnumerateFiles(runtimeDir, "javaw.exe", SearchOption.AllDirectories))
                {
                    if (IsJava21(exe)) return exe;
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Поиск Java: " + ex.Message);
            }
            return null;
        }

        public static string FindJavaExe(string root)
        {
            if (!Directory.Exists(root)) return null;
            try
            {
                var javaw = Directory.EnumerateFiles(root, "javaw.exe", SearchOption.AllDirectories)
                    .OrderBy(p => p.Length)
                    .FirstOrDefault();
                if (javaw != null) return javaw;

                var java = Directory.EnumerateFiles(root, "java.exe", SearchOption.AllDirectories)
                    .OrderBy(p => p.Length)
                    .FirstOrDefault();
                return java != null ? EnsureJavaw(java) : null;
            }
            catch
            {
                return null;
            }
        }

        public static bool IsUsable(string javaPath)
        {
            return !string.IsNullOrEmpty(javaPath) && File.Exists(javaPath) && IsJava21(javaPath);
        }

        public static bool IsJava21(string javaPath)
        {
            if (string.IsNullOrEmpty(javaPath)) return false;
            try
            {
                string probePath = javaPath;
                if (probePath.EndsWith("javaw.exe", StringComparison.OrdinalIgnoreCase))
                {
                    string dir = Path.GetDirectoryName(probePath);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        string sibling = Path.Combine(dir, "java.exe");
                        if (File.Exists(sibling)) probePath = sibling;
                    }
                }

                var psi = new ProcessStartInfo(probePath, "-version")
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                using (var process = Process.Start(psi))
                {
                    if (process == null) return false;
                    string output = process.StandardError.ReadToEnd() + process.StandardOutput.ReadToEnd();
                    if (!process.WaitForExit(5000))
                    {
                        try { process.Kill(); } catch { }
                        return false;
                    }
                    var match = Regex.Match(output, "version \"(\\d+)");
                    return match.Success && match.Groups[1].Value == "21";
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Проверка Java " + javaPath + ": " + ex.Message);
                return false;
            }
        }

        private static void TryDelete(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); } catch { }
        }
    }
}
