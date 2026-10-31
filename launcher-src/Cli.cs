using System;
using System.Diagnostics;
using System.Threading;

namespace SteampunkLauncher
{
    /// <summary>Headless commands used for testing and diagnostics (no window).</summary>
    internal static class Cli
    {
        public static int RunUpdate(string gameDir)
        {
            return Run(gameDir, false, null);
        }

        public static int RunLaunch(string gameDir, string nickname)
        {
            return Run(gameDir, true, nickname);
        }

        private static int Run(string gameDir, bool launch, string nickname)
        {
            try
            {
                var config = LauncherConfig.Load();
                if (string.IsNullOrWhiteSpace(gameDir))
                {
                    gameDir = config.GameDir;
                }
                if (string.IsNullOrWhiteSpace(gameDir))
                {
                    gameDir = LauncherConfig.DefaultGameDir();
                }
                gameDir = LauncherConfig.SanitizeGameDir(gameDir);
                config.GameDir = gameDir;
                if (!string.IsNullOrEmpty(nickname)) config.Nickname = nickname;
                if (string.IsNullOrEmpty(config.Nickname)) config.Nickname = "Player2";
                config.Save();

                var progress = new DirectProgress();
                var updater = new PackUpdater();

                Log.Info("=== Обновление " + gameDir + " ===");
                string java = JavaInstaller.EnsureJavaAsync(gameDir, config.JavaPath, progress, CancellationToken.None)
                    .GetAwaiter().GetResult();
                config.JavaPath = java;
                config.Save();

                PackManifest manifest = updater.FetchManifestAsync(CancellationToken.None).GetAwaiter().GetResult();
                SyncResult sync = updater.SyncAsync(manifest, gameDir, progress, CancellationToken.None)
                    .GetAwaiter().GetResult();

                string version = GameInstaller.EnsureInstalledAsync(gameDir, java, progress, CancellationToken.None)
                    .GetAwaiter().GetResult();

                Log.Info(string.Format("=== OK: сборка {0}, скачано {1} файлов ({2:N1} МБ), версия {3} ===",
                    manifest.PackVersion, sync.Downloaded, sync.DownloadedBytes / 1024.0 / 1024.0, version));

                if (!launch) return 0;

                Log.Info("=== Запуск игры ===");
                ServerInfo srv = new ServerInfo();
                try
                {
                    string text = HttpUtil.GetStringAsync(
                        "https://raw.githubusercontent.com/Trubik1/minecraft/main/server.txt",
                        CancellationToken.None, 2).GetAwaiter().GetResult();
                    srv = ServerInfo.Parse(text);
                }
                catch { }

                Process game = GameInstaller.LaunchAsync(gameDir, version, java, config.Nickname,
                    config.ResolveRamMb(), srv, config.JoinServer, CancellationToken.None)
                    .GetAwaiter().GetResult();

                game.WaitForExit();
                Log.Info("=== Игра закрыта, код " + game.ExitCode + " ===");
                return 0;
            }
            catch (Exception ex)
            {
                Log.Error("=== FAILED: " + ex);
                return 1;
            }
        }

        private class DirectProgress : IProgress<ProgressInfo>
        {
            public void Report(ProgressInfo value)
            {
                if (value == null) return;
                Log.Info(string.Format("[{0} {1}%] {2}", value.Phase, value.Percent, value.Detail));
            }
        }
    }
}
