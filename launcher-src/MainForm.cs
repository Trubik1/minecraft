using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    public class MainForm : Form
    {
        private readonly LauncherConfig _config;
        private readonly PackUpdater _updater = new PackUpdater();

        private TextBox _nick;
        private Button _play;
        private Button _cancel;
        private Button _settings;
        private Button _folder;
        private Button _verify;
        private Button _copyIp;
        private CheckBox _joinServer;
        private Label _serverLabel;
        private Label _phase;
        private Label _detail;
        private RichTextBox _log;
        private FlatProgressBar _progress;
        private Label _version;

        private ServerInfo _server = new ServerInfo();
        private CancellationTokenSource _cts;
        private bool _busy;
        private bool _firstRunHandled;

        public MainForm()
        {
            _config = LauncherConfig.Load();
            BuildUi();
            SelfUpdater.CleanupLeftovers();
            AppendLog("Лаунчер " + SelfUpdater.CurrentVersion + " запущен");
            if (!string.IsNullOrEmpty(_config.GameDir)) AppendLog("Папка игры: " + _config.GameDir);
        }

        private void BuildUi()
        {
            Text = "Steampunk & Swords";
            ClientSize = new Size(780, 560);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Background;
            ForeColor = Theme.Text;
            Font = Theme.Font(9.5f);
            DoubleBuffered = true;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 92,
                BackColor = Theme.Panel
            };
            Controls.Add(header);

            var accent = new Panel { Dock = DockStyle.Bottom, Height = 2, BackColor = Theme.Border };
            header.Controls.Add(accent);

            var title = new Label
            {
                Text = "STEAMPUNK & SWORDS",
                Left = 24,
                Top = 18,
                AutoSize = true,
                UseMnemonic = false,
                Font = Theme.Font(19f, FontStyle.Bold),
                ForeColor = Theme.Gold,
                BackColor = Color.Transparent
            };
            header.Controls.Add(title);

            var subtitle = new Label
            {
                Text = "Minecraft 1.21.1  •  NeoForge 21.1.264  •  " + "сборка с автообновлением",
                Left = 26,
                Top = 54,
                AutoSize = true,
                Font = Theme.Font(9f),
                ForeColor = Theme.Muted,
                BackColor = Color.Transparent
            };
            header.Controls.Add(subtitle);

            _version = new Label
            {
                Text = "v" + SelfUpdater.CurrentVersion,
                AutoSize = true,
                Font = Theme.Font(9f),
                ForeColor = Theme.Muted,
                BackColor = Color.Transparent,
                Left = 660,
                Top = 24
            };
            header.Controls.Add(_version);

            int x = 24;
            int y = 112;

            var nickLabel = new Label
            {
                Text = "Никнейм", Left = x, Top = y, AutoSize = true,
                ForeColor = Theme.Muted, BackColor = Color.Transparent
            };
            Controls.Add(nickLabel);

            _nick = new TextBox
            {
                Left = x, Top = y + 22, Width = 240, Height = 32,
                BorderStyle = BorderStyle.FixedSingle, AutoSize = false,
                BackColor = Theme.PanelLight, ForeColor = Theme.Text, Font = Theme.Font(12f)
            };
            Controls.Add(_nick);

            _play = Theme.FlatButton("ИГРАТЬ", 220, 54, Theme.Green, Theme.GreenHover);
            _play.Left = x;
            _play.Top = y + 70;
            _play.Font = Theme.Font(14f, FontStyle.Bold);
            _play.Click += (s, e) => StartWork(launchAfter: true);
            Controls.Add(_play);

            _joinServer = new CheckBox
            {
                Text = "Заходить на сервер сразу",
                Left = x + 240, Top = y + 74, AutoSize = true,
                ForeColor = Theme.Text, BackColor = Color.Transparent,
                Checked = _config.JoinServer, Visible = false
            };
            Controls.Add(_joinServer);

            _serverLabel = new Label
            {
                Text = "Сервер не настроен",
                Left = x + 240, Top = y + 98, Width = 470, Height = 20,
                ForeColor = Theme.Muted, BackColor = Color.Transparent,
                AutoEllipsis = true
            };
            Controls.Add(_serverLabel);

            _copyIp = Theme.FlatButton("IP", 54, 24, Theme.PanelLight, Theme.Border);
            _copyIp.Left = x + 248 + 400;
            _copyIp.Top = y + 96;
            _copyIp.Font = Theme.Font(8.5f);
            _copyIp.Visible = false;
            _copyIp.Click += (s, e) => CopyServer();
            Controls.Add(_copyIp);

            _progress = new FlatProgressBar
            {
                Left = x, Top = y + 140, Width = 732, Height = 10, Value = 0
            };
            Controls.Add(_progress);

            _phase = new Label
            {
                Left = x, Top = y + 158, Width = 732, Height = 22,
                Font = Theme.Font(10f, FontStyle.Bold), ForeColor = Theme.Text,
                BackColor = Color.Transparent, Text = "Готов к запуску"
            };
            Controls.Add(_phase);

            _detail = new Label
            {
                Left = x, Top = y + 180, Width = 732, Height = 20,
                Font = Theme.Font(8.5f), ForeColor = Theme.Muted,
                BackColor = Color.Transparent, AutoEllipsis = true
            };
            Controls.Add(_detail);

            _log = new RichTextBox
            {
                Left = x, Top = y + 210, Width = 732, Height = 118,
                BackColor = Theme.Panel, ForeColor = Theme.Muted,
                BorderStyle = BorderStyle.None, ReadOnly = true,
                Font = new Font("Consolas", 8.25f), DetectUrls = false,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            Controls.Add(_log);

            _folder = Theme.FlatButton("Папка игры", 128, 34, Theme.PanelLight, Theme.Border);
            _folder.Left = x; _folder.Top = 498; _folder.ForeColor = Theme.Text;
            _folder.Click += (s, e) => OpenFolder();
            Controls.Add(_folder);

            _verify = Theme.FlatButton("Проверить файлы", 168, 34, Theme.PanelLight, Theme.Border);
            _verify.Left = x + 140; _verify.Top = 498; _verify.ForeColor = Theme.Text;
            _verify.Click += (s, e) =>
            {
                PackUpdater.ResetVerificationCache(_config.GameDir);
                AppendLog("Кэш проверки сброшен");
                StartWork(launchAfter: false);
            };
            Controls.Add(_verify);

            _settings = Theme.FlatButton("Настройки", 128, 34, Theme.PanelLight, Theme.Border);
            _settings.Left = x + 320; _settings.Top = 498; _settings.ForeColor = Theme.Text;
            _settings.Click += (s, e) => OpenSettings();
            Controls.Add(_settings);

            _cancel = Theme.FlatButton("Отмена", 128, 34, Theme.Red, Theme.Red);
            _cancel.Left = 628; _cancel.Top = 498;
            _cancel.Visible = false;
            _cancel.Click += (s, e) =>
            {
                if (_cts != null) _cts.Cancel();
            };
            Controls.Add(_cancel);
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!_firstRunHandled)
            {
                _firstRunHandled = true;
                if (_config.IsFirstRun && !RunSettings(true))
                {
                    Close();
                    return;
                }
            }

            _nick.Text = _config.Nickname ?? "";
            await LoadServerAsync();
            await CheckLauncherUpdateAsync();
        }

        private bool RunSettings(bool firstRun)
        {
            using (var form = new SettingsForm(_config, firstRun))
            {
                return form.ShowDialog(this) == DialogResult.OK && form.Saved;
            }
        }

        private void OpenSettings()
        {
            if (_busy) return;
            if (RunSettings(false) && !_config.IsFirstRun)
            {
                _nick.Text = _config.Nickname;
                AppendLog("Настройки сохранены");
            }
        }

        private void OpenFolder()
        {
            try
            {
                if (!string.IsNullOrEmpty(_config.GameDir))
                {
                    Directory.CreateDirectory(_config.GameDir);
                    Process.Start("explorer.exe", "\"" + _config.GameDir + "\"");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task LoadServerAsync()
        {
            try
            {
                string text = await HttpUtil.GetStringAsync(
                    "https://raw.githubusercontent.com/Trubik1/minecraft/main/server.txt",
                    CancellationToken.None, 2).ConfigureAwait(true);
                _server = ServerInfo.Parse(text);
            }
            catch (Exception ex)
            {
                Log.Warn("server.txt: " + ex.Message);
            }

            if (_server.IsConfigured)
            {
                _serverLabel.Text = "Сервер: " + _server.Endpoint;
                _joinServer.Visible = true;
                _copyIp.Visible = true;
            }
            else
            {
                _serverLabel.Text = "Сервер не настроен (IP появится позже)";
            }
        }

        private void CopyServer()
        {
            if (!_server.IsConfigured) return;
            try
            {
                Clipboard.SetText(_server.Endpoint);
                AppendLog("IP скопирован: " + _server.Endpoint);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task CheckLauncherUpdateAsync()
        {
            try
            {
                UpdateInfo update = await SelfUpdater.CheckAsync(CancellationToken.None).ConfigureAwait(true);
                if (update == null) return;

                var answer = MessageBox.Show(this,
                    "Доступна новая версия лаунчера " + update.Version + ".\nОбновить сейчас?",
                    "Steampunk & Swords", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes) return;

                SetBusy(true);
                using (var cts = new CancellationTokenSource())
                {
                    _cts = cts;
                    await SelfUpdater.ApplyAsync(update, CreateProgress(), cts.Token).ConfigureAwait(true);
                }
                Application.Exit();
            }
            catch (Exception ex)
            {
                Log.Warn("Обновление лаунчера: " + ex.Message);
                SetBusy(false);
            }
        }

        private async void StartWork(bool launchAfter)
        {
            if (_busy) return;

            if (_config.IsFirstRun)
            {
                if (!RunSettings(true)) return;
            }

            string nick = (_nick.Text ?? "").Trim();
            if (nick.Length < 3 || nick.Length > 16)
            {
                MessageBox.Show(this, "Введите ник (3–16 символов).", "Steampunk & Swords",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _config.Nickname = nick;
            _config.JoinServer = _joinServer.Checked;
            _config.Save();

            _cts = new CancellationTokenSource();
            SetBusy(true);
            _progress.Value = 0;
            SetPhase("Подготовка", "");

            try
            {
                var ct = _cts.Token;
                var progress = CreateProgress();

                string java = await JavaInstaller.EnsureJavaAsync(_config.GameDir, _config.JavaPath, progress, ct);
                _config.JavaPath = java;
                _config.Save();

                progress.Report(new ProgressInfo { Phase = "Проверка обновлений сборки", Fraction = 0, Detail = "" });
                PackManifest manifest = await _updater.FetchManifestAsync(ct);
                AppendLog("Версия сборки: " + manifest.PackVersion + " (" + manifest.Files.Count + " файлов)");

                SyncResult sync = await _updater.SyncAsync(manifest, _config.GameDir, progress, ct);
                AppendLog(string.Format("Обновление: скачано {0} файлов ({1:N1} МБ), удалено {2}",
                    sync.Downloaded, sync.DownloadedBytes / 1024.0 / 1024.0, sync.Deleted));

                SetPhase("Проверка игры", "Minecraft " + GameInstaller.McVersion + " • NeoForge " + GameInstaller.NeoForgeVersion);
                string versionName = await GameInstaller.EnsureInstalledAsync(_config.GameDir, java, progress, ct);

                if (!launchAfter)
                {
                    SetPhase("Готово", "Файлы в порядке");
                    _progress.Value = 100;
                    return;
                }

                SetPhase("Запуск игры", "");
                Process game = await GameInstaller.LaunchAsync(_config.GameDir, versionName, java, nick,
                    _config.ResolveRamMb(), _server, _joinServer.Checked, ct);

                AppendLog("Игра запущена");
                Hide();
                await Task.Run(() => game.WaitForExit());
                Show();
                Activate();
                AppendLog("Игра закрыта (код " + game.ExitCode + ")");
                SetPhase("Готов к запуску", "");
                _progress.Value = 0;
            }
            catch (OperationCanceledException)
            {
                SetPhase("Отменено", "");
                AppendLog("Операция отменена");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                SetPhase("Ошибка", ex.Message);
                AppendLog("ОШИБКА: " + ex.Message);
                MessageBox.Show(this,
                    ex.Message + "\n\nПодробности: " + Log.FilePath,
                    "Steampunk & Swords", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
                if (_cts != null)
                {
                    _cts.Dispose();
                    _cts = null;
                }
            }
        }

        private IProgress<ProgressInfo> CreateProgress()
        {
            return new Progress<ProgressInfo>(info =>
            {
                if (info == null) return;
                _progress.Value = info.Percent;
                SetPhase(info.Phase, info.Detail);
            });
        }

        private void SetPhase(string phase, string detail)
        {
            if (phase != null) _phase.Text = phase;
            _detail.Text = detail ?? "";
        }

        private void SetBusy(bool busy)
        {
            _busy = busy;
            _play.Enabled = !busy;
            _settings.Enabled = !busy;
            _verify.Enabled = !busy;
            _folder.Enabled = !busy;
            _nick.Enabled = !busy;
            _joinServer.Enabled = !busy;
            _cancel.Visible = busy;
            _play.BackColor = busy ? Theme.PanelLight : Theme.Green;
            _play.FlatAppearance.MouseOverBackColor = busy ? Theme.PanelLight : Theme.GreenHover;
        }

        private void AppendLog(string message)
        {
            Log.Info(message);
            try
            {
                _log.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message + Environment.NewLine);
                _log.SelectionStart = _log.TextLength;
                _log.ScrollToCaret();
            }
            catch
            {
                // ignore UI logging failures
            }
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Steampunk & Swords", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
