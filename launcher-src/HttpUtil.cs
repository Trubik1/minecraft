using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SteampunkLauncher
{
    public static class HttpUtil
    {
        public static readonly HttpClient Client = Create();

        private static HttpClient Create()
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                ServicePointManager.DefaultConnectionLimit = 16;
                ServicePointManager.Expect100Continue = false;
            }
            catch
            {
                // older runtimes: ignore
            }

            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(30);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("SteampunkLauncher/1.0 (+https://github.com/Trubik1/minecraft)");
            return client;
        }

        public static async Task<string> GetStringAsync(string url, CancellationToken ct, int attempts = 3)
        {
            Exception last = null;
            for (int i = 1; i <= attempts; i++)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    using (var response = await Client.GetAsync(url, HttpCompletionOption.ResponseContentRead, ct).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    last = ex;
                    if (i == attempts) break;
                    await Task.Delay(700 * i, ct).ConfigureAwait(false);
                }
            }
            throw new IOException("Не удалось скачать " + url + ": " + (last == null ? "?" : last.Message), last);
        }

        /// <summary>Copies the response body to a file, reporting downloaded bytes.</summary>
        public static async Task DownloadAsync(string url, string destination, Action<long> onBytes,
            CancellationToken ct, int attempts = 3)
        {
            Exception last = null;
            for (int attempt = 1; attempt <= attempts; attempt++)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    using (var response = await Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        using (var source = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        using (var target = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None, 131072, true))
                        {
                            var buffer = new byte[131072];
                            long total = 0;
                            int read;
                            while ((read = await source.ReadAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false)) > 0)
                            {
                                await target.WriteAsync(buffer, 0, read, ct).ConfigureAwait(false);
                                total += read;
                                if (onBytes != null) onBytes(total);
                            }
                        }
                    }
                    return;
                }
                catch (Exception ex)
                {
                    last = ex;
                    try { if (File.Exists(destination)) File.Delete(destination); } catch { }
                    if (attempt == attempts) break;
                    await Task.Delay(1000 * attempt, ct).ConfigureAwait(false);
                }
            }
            throw new IOException("Не удалось скачать " + url + ": " + (last == null ? "?" : last.Message), last);
        }

        public static string Combine(string baseUrl, string relativePath)
        {
            string url = (baseUrl ?? "").TrimEnd('/') + "/" + EscapePath(relativePath);
            return url;
        }

        public static string EscapePath(string path)
        {
            var parts = path.Replace('\\', '/').Split('/');
            for (int i = 0; i < parts.Length; i++)
                parts[i] = Uri.EscapeDataString(parts[i]);
            return string.Join("/", parts);
        }
    }
}
