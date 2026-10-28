using System;
using System.Threading;

namespace SteampunkLauncher
{
    /// <summary>Headless commands used for testing and diagnostics (no window).</summary>
    internal static class Cli
    {
        public static int RunUpdate(string gameDir)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(gameDir))
                {
                    Log.Error("--update: не указана папка игры");
                    return 2;
                }

                gameDir = System.IO.Path.GetFullPath(gameDir);
                var config = LauncherConfig.Load();
                config.GameDir = gameDir;
                if (string.IsNullOrEmpty(config.Nickname)) config.Nickname = "TestPlayer";
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
