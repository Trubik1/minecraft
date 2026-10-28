using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installers;
using CmlLib.Core.Installer.NeoForge;
using CmlLib.Core.Installer.NeoForge.Installers;
using CmlLib.Core.ProcessBuilder;

namespace SteampunkLauncher
{
    /// <summary>Installs Minecraft + NeoForge through CmlLib and launches the game.</summary>
    public static class GameInstaller
    {
        public const string McVersion = "1.21.1";
        public const string NeoForgeVersion = "21.1.264";

        public static async Task<string> EnsureInstalledAsync(string gameDir, string javaExe,
            IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            var path = new MinecraftPath(gameDir);
            var launcher = new MinecraftLauncher(path);

            var fileProgress = new Progress<InstallerProgressChangedEventArgs>(e =>
            {
                double fraction = e.TotalTasks > 0 ? (double)e.ProgressedTasks / e.TotalTasks : 0;
                progress.Report(new ProgressInfo
                {
                    Phase = "Файлы игры",
                    Fraction = fraction,
                    Detail = string.Format("{0} / {1} • {2}", e.ProgressedTasks, e.TotalTasks, e.Name)
                });
            });

            var byteProgress = new ThrottledByteProgress(progress);

            var neoForge = new NeoForgeInstaller(launcher);
            var options = new NeoForgeInstallOptions
            {
                JavaPath = javaExe,
                SkipIfAlreadyInstalled = true,
                FileProgress = fileProgress,
                ByteProgress = byteProgress,
                InstallerOutput = new Progress<string>(line =>
                {
                    if (!string.IsNullOrWhiteSpace(line)) Log.Info("[neoforge] " + line.Trim());
                }),
                CancellationToken = ct
            };

            Log.Info("Проверка NeoForge " + NeoForgeVersion + " для MC " + McVersion);
            string versionName = await neoForge.Install(McVersion, NeoForgeVersion, options).ConfigureAwait(false);

            progress.Report(new ProgressInfo { Phase = "Файлы игры", Fraction = 0, Detail = "проверка библиотек и ассетов" });
            await launcher.InstallAsync(versionName, fileProgress, byteProgress, ct).ConfigureAwait(false);

            return versionName;
        }

        public static async Task<Process> LaunchAsync(string gameDir, string versionName, string javaExe,
            string nickname, int ramMb, ServerInfo server, bool joinServer, CancellationToken ct)
        {
            var path = new MinecraftPath(gameDir);
            var launcher = new MinecraftLauncher(path);

            var option = new MLaunchOption
            {
                Path = path,
                Session = MSession.CreateOfflineSession(nickname),
                JavaPath = javaExe,
                MaximumRamMb = ramMb,
                MinimumRamMb = Math.Min(1024, ramMb),
                VersionType = "release",
                GameLauncherName = "SteampunkLauncher",
                GameLauncherVersion = "1.0.0"
            };

            if (joinServer && server != null && server.IsConfigured)
            {
                option.ServerIp = server.Ip;
                option.ServerPort = server.Port;
                Log.Info("Заход на сервер " + server.Endpoint);
            }

            var process = await launcher.BuildProcessAsync(versionName, option, ct).ConfigureAwait(false);
            process.StartInfo.WorkingDirectory = gameDir;
            Log.Info("Запуск игры: " + process.StartInfo.FileName + " " + process.StartInfo.Arguments);
            process.Start();
            return process;
        }

        private class ThrottledByteProgress : IProgress<ByteProgress>
        {
            private readonly IProgress<ProgressInfo> _target;
            private long _lastTicks;

            public ThrottledByteProgress(IProgress<ProgressInfo> target)
            {
                _target = target;
            }

            public void Report(ByteProgress value)
            {
                long now = DateTime.UtcNow.Ticks;
                if (now - _lastTicks < TimeSpan.FromMilliseconds(250).Ticks) return;
                _lastTicks = now;

                double ratio = value.TotalBytes > 0 ? (double)value.ProgressedBytes / value.TotalBytes : 0;
                _target.Report(new ProgressInfo
                {
                    Phase = "Файлы игры",
                    Fraction = Math.Max(0, Math.Min(1, ratio)),
                    Detail = string.Format("{0:N0} / {1:N0} МБ",
                        value.ProgressedBytes / 1024.0 / 1024.0,
                        value.TotalBytes / 1024.0 / 1024.0)
                });
            }
        }
    }
}
