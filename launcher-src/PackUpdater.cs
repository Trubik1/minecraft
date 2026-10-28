using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SteampunkLauncher
{
    public class SyncResult
    {
        public int Total { get; set; }
        public int Downloaded { get; set; }
        public long DownloadedBytes { get; set; }
        public int Deleted { get; set; }
    }

    /// <summary>Keeps the game folder in sync with manifest.json (only changed files).</summary>
    public class PackUpdater
    {
        private const int ParallelDownloads = 6;

        public async Task<PackManifest> FetchManifestAsync(CancellationToken ct)
        {
            string json = await HttpUtil.GetStringAsync(PackManifest.RemoteUrl, ct).ConfigureAwait(false);
            var manifest = JsonConvert.DeserializeObject<PackManifest>(json);
            if (manifest == null || manifest.Files == null || manifest.Files.Count == 0)
                throw new InvalidDataException("Пустой или повреждённый manifest.json");
            if (string.IsNullOrEmpty(manifest.BaseUrl))
                manifest.BaseUrl = "https://raw.githubusercontent.com/Trubik1/minecraft/main/";
            return manifest;
        }

        public async Task<SyncResult> SyncAsync(PackManifest manifest, string gameDir,
            IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            var result = new SyncResult { Total = manifest.Files.Count };
            string stateDir = Path.Combine(gameDir, "_launcher");
            Directory.CreateDirectory(stateDir);
            string installedFile = Path.Combine(stateDir, "installed.json");
            string verifiedFile = Path.Combine(stateDir, "verified.json");

            var installed = LoadList(installedFile);
            var verified = LoadVerified(verifiedFile);
            var wanted = new HashSet<string>(manifest.Files.Select(f => Normalize(f.Path)), StringComparer.OrdinalIgnoreCase);

            var toDownload = new List<ManifestFile>();
            int index = 0;
            foreach (var file in manifest.Files)
            {
                ct.ThrowIfCancellationRequested();
                index++;
                string rel = Normalize(file.Path);
                string full = LocalPath(gameDir, rel);
                bool ok = File.Exists(full) && IsUpToDate(full, rel, file, verified);

                if (!ok) toDownload.Add(file);

                if (index % 16 == 0 || index == manifest.Files.Count)
                {
                    progress.Report(new ProgressInfo
                    {
                        Phase = "Проверка файлов",
                        Fraction = (double)index / manifest.Files.Count,
                        Detail = rel + (ok ? "" : " — нужно обновить")
                    });
                }
            }

            if (toDownload.Count > 0)
                await DownloadAllAsync(manifest, gameDir, toDownload, verified, progress, ct).ConfigureAwait(false);

            result.Downloaded = toDownload.Count;
            result.DownloadedBytes = toDownload.Sum(f => f.Size);

            foreach (var old in installed)
            {
                if (wanted.Contains(old)) continue;
                try
                {
                    string full = LocalPath(gameDir, old);
                    if (File.Exists(full))
                    {
                        File.Delete(full);
                        result.Deleted++;
                        Log.Info("Удалён устаревший файл " + old);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("Не удалось удалить " + old + ": " + ex.Message);
                }
            }

            // prune the verification cache to files we manage
            var pruned = new Dictionary<string, VerifiedEntry>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in verified)
                if (wanted.Contains(pair.Key)) pruned[pair.Key] = pair.Value;

            SaveList(installedFile, manifest.Files.Select(f => Normalize(f.Path)).ToList());
            SaveVerified(verifiedFile, pruned);
            SavePackInfo(stateDir, manifest);

            progress.Report(new ProgressInfo { Phase = "Готово", Fraction = 1, Detail = "" });
            return result;
        }

        private bool IsUpToDate(string full, string rel, ManifestFile file, Dictionary<string, VerifiedEntry> verified)
        {
            var info = new FileInfo(full);
            VerifiedEntry cached;
            if (verified.TryGetValue(rel, out cached) &&
                cached.Size == info.Length &&
                cached.MTime == info.LastWriteTimeUtc.Ticks &&
                string.Equals(cached.Hash, file.Hash, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string hash = NormalizedHash.Sha1File(full);
            verified[rel] = new VerifiedEntry
            {
                Size = info.Length,
                MTime = info.LastWriteTimeUtc.Ticks,
                Hash = hash
            };
            return string.Equals(hash, file.Hash, StringComparison.OrdinalIgnoreCase);
        }

        private async Task DownloadAllAsync(PackManifest manifest, string gameDir, List<ManifestFile> files,
            Dictionary<string, VerifiedEntry> verified, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            long totalBytes = files.Sum(f => Math.Max(f.Size, 1));
            long doneBytes = 0;
            int doneFiles = 0;
            object sync = new object();
            var gate = new SemaphoreSlim(ParallelDownloads);

            var tasks = new List<Task>();
            foreach (var file in files)
            {
                await gate.WaitAsync(ct).ConfigureAwait(false);
                var captured = file;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        string rel = Normalize(captured.Path);
                        await DownloadOneAsync(manifest, gameDir, captured, rel, ct).ConfigureAwait(false);
                        verified[rel] = new VerifiedEntry
                        {
                            Size = new FileInfo(LocalPath(gameDir, rel)).Length,
                            MTime = File.GetLastWriteTimeUtc(LocalPath(gameDir, rel)).Ticks,
                            Hash = captured.Hash
                        };

                        lock (sync)
                        {
                            doneFiles++;
                            doneBytes += Math.Max(captured.Size, 1);
                            progress.Report(new ProgressInfo
                            {
                                Phase = "Скачивание сборки",
                                Fraction = totalBytes == 0 ? 0 : Math.Min(1.0, (double)doneBytes / totalBytes),
                                Detail = string.Format("{0} / {1} • {2}", doneFiles, files.Count, rel)
                            });
                        }
                    }
                    finally
                    {
                        gate.Release();
                    }
                }, ct));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task DownloadOneAsync(PackManifest manifest, string gameDir, ManifestFile file, string rel,
            CancellationToken ct)
        {
            string url = HttpUtil.Combine(manifest.BaseUrl, file.Path);
            string full = LocalPath(gameDir, rel);
            string dir = Path.GetDirectoryName(full);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            string temp = full + ".part";
            for (int attempt = 1; attempt <= 2; attempt++)
            {
                try
                {
                    Log.Info("Скачивание " + rel);
                    await HttpUtil.DownloadAsync(url, temp, null, ct).ConfigureAwait(false);
                    string hash = NormalizedHash.Sha1File(temp);
                    if (!string.Equals(hash, file.Hash, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException("контрольная сумма не совпала: " + rel);

                    if (File.Exists(full)) File.Delete(full);
                    File.Move(temp, full);
                    return;
                }
                catch (Exception ex)
                {
                    try { if (File.Exists(temp)) File.Delete(temp); } catch { }
                    if (attempt == 2)
                        throw new IOException("Не удалось обновить " + rel + ": " + ex.Message, ex);
                    await Task.Delay(1000, ct).ConfigureAwait(false);
                }
            }
        }

        private static string Normalize(string path)
        {
            return path.Replace('\\', '/').TrimStart('/');
        }

        public static string LocalPath(string gameDir, string relative)
        {
            return Path.Combine(gameDir, relative.Replace('/', Path.DirectorySeparatorChar));
        }

        private static List<string> LoadList(string path)
        {
            try
            {
                if (File.Exists(path))
                    return JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(path)) ?? new List<string>();
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось прочитать " + path + ": " + ex.Message);
            }
            return new List<string>();
        }

        private static void SaveList(string path, List<string> list)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(list));
        }

        private static Dictionary<string, VerifiedEntry> LoadVerified(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<string, VerifiedEntry>>(File.ReadAllText(path));
                    if (data != null) return new Dictionary<string, VerifiedEntry>(data, StringComparer.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось прочитать кэш проверки: " + ex.Message);
            }
            return new Dictionary<string, VerifiedEntry>(StringComparer.OrdinalIgnoreCase);
        }

        private static void SaveVerified(string path, Dictionary<string, VerifiedEntry> data)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(data));
        }

        public static void ResetVerificationCache(string gameDir)
        {
            try
            {
                string path = Path.Combine(gameDir, "_launcher", "verified.json");
                if (File.Exists(path)) File.Delete(path);
            }
            catch (Exception ex)
            {
                Log.Warn("Не удалось сбросить кэш проверки: " + ex.Message);
            }
        }

        private static void SavePackInfo(string stateDir, PackManifest manifest)
        {
            try
            {
                File.WriteAllText(Path.Combine(stateDir, "pack.json"),
                    JsonConvert.SerializeObject(new { version = manifest.PackVersion, installedAt = DateTime.Now }, Formatting.Indented));
            }
            catch
            {
                // not critical
            }
        }
    }
}
