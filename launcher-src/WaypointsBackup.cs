using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SteampunkLauncher
{
    public static class WaypointsBackup
    {
        public static void CreateBackup(string gameDir)
        {
            if (string.IsNullOrEmpty(gameDir) || !Directory.Exists(gameDir)) return;

            try
            {
                string wpDir = Path.Combine(gameDir, "xaerowaypoints");
                string mapDir = Path.Combine(gameDir, "xaeroworldmap");
                if (!Directory.Exists(wpDir) && !Directory.Exists(mapDir)) return;

                string backupRoot = Path.Combine(gameDir, "backups", "waypoints");
                Directory.CreateDirectory(backupRoot);

                string zipName = string.Format("waypoints_{0:yyyyMMdd_HHmmss}.zip", DateTime.Now);
                string zipPath = Path.Combine(backupRoot, zipName);

                using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    if (Directory.Exists(wpDir))
                    {
                        AddDirectoryToZip(zip, wpDir, "xaerowaypoints");
                    }
                    if (Directory.Exists(mapDir))
                    {
                        AddDirectoryToZip(zip, mapDir, "xaeroworldmap");
                    }
                }

                Log.Info("Создан резервный бэкап меток карты: " + zipName);

                // Ограничиваем количество бэкапов до 5 последних
                var di = new DirectoryInfo(backupRoot);
                var oldZips = di.GetFiles("waypoints_*.zip").OrderByDescending(f => f.CreationTime).Skip(5);
                foreach (var f in oldZips)
                {
                    try { f.Delete(); } catch { }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось создать бэкап меток: " + ex.Message);
            }
        }

        private static void AddDirectoryToZip(ZipArchive zip, string sourceDir, string entryPrefix)
        {
            var dir = new DirectoryInfo(sourceDir);
            foreach (var file in dir.GetFiles("*", SearchOption.AllDirectories))
            {
                string rel = file.FullName.Substring(dir.FullName.Length).TrimStart('\\', '/');
                string entryName = Path.Combine(entryPrefix, rel).Replace('\\', '/');
                zip.CreateEntryFromFile(file.FullName, entryName, CompressionLevel.Fastest);
            }
        }
    }
}
