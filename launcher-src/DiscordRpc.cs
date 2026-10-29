using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SteampunkLauncher
{
    /// <summary>
    /// Pure C# Discord Rich Presence client communicating via local named pipes (discord-ipc-0).
    /// Zero external native DLL dependencies — 100% self-contained and crash-safe.
    /// </summary>
    public class DiscordRpc : IDisposable
    {
        private const string DefaultClientId = "1344685002447913000"; // Steampunk & Swords App ID
        private static DiscordRpc _instance;
        public static DiscordRpc Instance => _instance ?? (_instance = new DiscordRpc());

        private NamedPipeClientStream _pipe;
        private CancellationTokenSource _cts;
        private readonly object _sync = new object();
        private bool _connected;
        private string _clientId = DefaultClientId;
        private long _gameStartTime;

        public bool IsConnected => _connected;

        public void Start(string clientId = null)
        {
            if (!string.IsNullOrEmpty(clientId)) _clientId = clientId;
            lock (_sync)
            {
                if (_connected || _pipe != null) return;
                _cts = new CancellationTokenSource();
                Task.Run(() => ConnectLoop(_cts.Token));
            }
        }

        private async Task ConnectLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        string pipeName = "discord-ipc-" + i;
                        try
                        {
                            var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                            await pipe.ConnectAsync(1500).ConfigureAwait(false);
                            if (pipe.IsConnected)
                            {
                                lock (_sync)
                                {
                                    _pipe = pipe;
                                    _connected = true;
                                }
                                SendHandshake();
                                Log.Info("Discord RPC подключен через " + pipeName);
                                SetLauncherPresence();
                                return;
                            }
                        }
                        catch
                        {
                            // Try next pipe index
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("Discord RPC connect failed: " + ex.Message);
                }

                // If not connected, wait before trying again
                try { await Task.Delay(10000, ct).ConfigureAwait(false); } catch { break; }
            }
        }

        private void SendHandshake()
        {
            try
            {
                var payload = new JObject
                {
                    ["v"] = 1,
                    ["client_id"] = _clientId
                };
                WriteFrame(0, payload.ToString()); // OpCode 0: Handshake
            }
            catch (Exception ex)
            {
                Log.Warn("Discord RPC handshake error: " + ex.Message);
                Disconnect();
            }
        }

        public void SetLauncherPresence()
        {
            SetPresence("В лаунчере", "Готовится к приключениям", 0);
        }

        public void SetInGamePresence(string nickname)
        {
            _gameStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            SetPresence("В игре: " + nickname, "Исследует мир Steampunk & Swords", _gameStartTime);
        }

        public void SetPresence(string state, string details, long startTimestamp = 0)
        {
            if (!_connected || _pipe == null) return;

            Task.Run(() =>
            {
                lock (_sync)
                {
                    try
                    {
                        var activity = new JObject
                        {
                            ["state"] = state,
                            ["details"] = details,
                            ["assets"] = new JObject
                            {
                                ["large_image"] = "logo",
                                ["large_text"] = "Steampunk & Swords (1.21.1)",
                                ["small_image"] = "gear",
                                ["small_text"] = "NeoForge 21.1.264"
                            }
                        };

                        if (startTimestamp > 0)
                        {
                            activity["timestamps"] = new JObject
                            {
                                ["start"] = startTimestamp
                            };
                        }

                        var frame = new JObject
                        {
                            ["cmd"] = "SET_ACTIVITY",
                            ["args"] = new JObject
                            {
                                ["pid"] = Process.GetCurrentProcess().Id,
                                ["activity"] = activity
                            },
                            ["nonce"] = Guid.NewGuid().ToString("N")
                        };

                        WriteFrame(1, frame.ToString()); // OpCode 1: Frame
                    }
                    catch
                    {
                        Disconnect();
                    }
                }
            });
        }

        private void WriteFrame(int opCode, string json)
        {
            if (_pipe == null || !_pipe.IsConnected) return;

            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] header = new byte[8];
            BitConverter.GetBytes(opCode).CopyTo(header, 0);
            BitConverter.GetBytes(jsonBytes.Length).CopyTo(header, 4);

            _pipe.Write(header, 0, 8);
            _pipe.Write(jsonBytes, 0, jsonBytes.Length);
            _pipe.Flush();
        }

        private void Disconnect()
        {
            lock (_sync)
            {
                _connected = false;
                try { _pipe?.Dispose(); } catch { }
                _pipe = null;
            }
        }

        public void Dispose()
        {
            try { _cts?.Cancel(); } catch { }
            Disconnect();
        }
    }
}
