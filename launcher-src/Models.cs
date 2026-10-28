using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SteampunkLauncher
{
    public class ManifestFile
    {
        public string Path { get; set; }
        public long Size { get; set; }
        public string Hash { get; set; }
    }

    public class PackManifest
    {
        public int Schema { get; set; }
        public string PackVersion { get; set; }
        public string BaseUrl { get; set; }
        public List<ManifestFile> Files { get; set; }

        public static readonly string RemoteUrl =
            "https://raw.githubusercontent.com/Trubik1/minecraft/main/manifest.json";
    }

    public class VerifiedEntry
    {
        public long Size { get; set; }
        public long MTime { get; set; }
        public string Hash { get; set; }
    }

    public class ServerInfo
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }

        public ServerInfo()
        {
            Name = "Steampunk & Swords";
            Port = 25565;
        }

        public bool IsConfigured
        {
            get { return !string.IsNullOrWhiteSpace(Ip); }
        }

        public string Endpoint
        {
            get { return Port == 25565 ? Ip : Ip + ":" + Port; }
        }

        public static ServerInfo Parse(string text)
        {
            var info = new ServerInfo();
            if (string.IsNullOrWhiteSpace(text)) return info;
            foreach (var raw in text.Split('\n'))
            {
                string line = raw.Trim();
                int eq = line.IndexOf('=');
                if (eq <= 0) continue;
                string key = line.Substring(0, eq).Trim().ToLowerInvariant();
                string value = line.Substring(eq + 1).Trim();
                if (key == "name" && value.Length > 0) info.Name = value;
                else if (key == "ip") info.Ip = value;
                else if (key == "port")
                {
                    int port;
                    if (int.TryParse(value, out port) && port > 0 && port <= 65535) info.Port = port;
                }
            }
            return info;
        }
    }

    /// <summary>SHA-1 of file content with CRLF normalised to LF.</summary>
    public static class NormalizedHash
    {
        public static string Sha1File(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1048576))
            using (var sha = SHA1.Create())
            {
                var input = new byte[1048576];
                var output = new byte[1048576 + 2]; // +2: a CR left over from the previous chunk
                bool pendingCr = false;
                int read;
                while ((read = stream.Read(input, 0, input.Length)) > 0)
                {
                    int written = 0;
                    for (int i = 0; i < read; i++)
                    {
                        byte value = input[i];
                        if (pendingCr)
                        {
                            pendingCr = false;
                            if (value == 0x0A)
                            {
                                output[written++] = 0x0A;
                                continue;
                            }
                            output[written++] = 0x0D;
                        }

                        if (value == 0x0D)
                        {
                            if (i + 1 < read)
                            {
                                if (input[i + 1] == 0x0A) continue;
                                output[written++] = 0x0D;
                            }
                            else
                            {
                                pendingCr = true;
                            }
                        }
                        else
                        {
                            output[written++] = value;
                        }
                    }

                    if (written > 0) sha.TransformBlock(output, 0, written, null, 0);
                }

                if (pendingCr)
                {
                    output[0] = 0x0D;
                    sha.TransformBlock(output, 0, 1, null, 0);
                }

                sha.TransformFinalBlock(new byte[0], 0, 0);
                return ToHex(sha.Hash);
            }
        }

        public static string Sha1Bytes(byte[] data)
        {
            using (var sha = SHA1.Create())
            {
                var normalized = Normalize(data);
                return ToHex(sha.ComputeHash(normalized));
            }
        }

        private static byte[] Normalize(byte[] data)
        {
            var result = new byte[data.Length];
            int written = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0x0D && i + 1 < data.Length && data[i + 1] == 0x0A) continue;
                result[written++] = data[i];
            }
            if (written == result.Length) return result;
            var trimmed = new byte[written];
            Array.Copy(result, trimmed, written);
            return trimmed;
        }

        private static string ToHex(byte[] hash)
        {
            var chars = new char[hash.Length * 2];
            const string digits = "0123456789abcdef";
            for (int i = 0; i < hash.Length; i++)
            {
                chars[i * 2] = digits[hash[i] >> 4];
                chars[i * 2 + 1] = digits[hash[i] & 0xF];
            }
            return new string(chars);
        }
    }

    public class ProgressInfo
    {
        public string Phase { get; set; }
        public double Fraction { get; set; }
        public string Detail { get; set; }

        public int Percent
        {
            get
            {
                int value = (int)Math.Round(Fraction * 100);
                if (value < 0) return 0;
                if (value > 100) return 100;
                return value;
            }
        }
    }
}
