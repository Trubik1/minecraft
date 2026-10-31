using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    /// <summary>Settings dialog. Doubles as the first-run wizard.</summary>
    public class SettingsForm : Form
    {
        private readonly bool _firstRun;
        private readonly LauncherConfig _config;

        private TextBox _nick;
        private TextBox _folder;
        private TrackBar _ram;
        private Label _ramValue;
        private Label _folderHint;
        private Label _java;
        private Label _error;

        public bool Saved { get; private set; }

        public SettingsForm(LauncherConfig config, bool firstRun)
        {
            _config = config;
            _firstRun = firstRun;
            BuildUi();
            LoadValues();
        }

        private void BuildUi()
        {
            Text = _firstRun ? "Steampunk & Swords — первый запуск" : "Настройки";
            ClientSize = new Size(560, 500);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.Background;
            ForeColor = Theme.Text;
            Font = Theme.Font(9.5f);

            int x = 24, y = 20;

            var lblNick = MakeLabel("Никнейм в игре", x, y);
            y += 24;
            _nick = new TextBox
            {
                Left = x, Top = y, Width = 220, Height = 30, BorderStyle = BorderStyle.FixedSingle,
                BackColor = Theme.PanelLight, ForeColor = Theme.Text, Font = Theme.Font(11f),
                AutoSize = false
            };
            Controls.Add(_nick);
            y += 32;
            var nickHint = MakeLabel("Без пароля. Подойдёт любой ник (3–16 символов, латиница, цифры, _)", x, y, 8.5f, Theme.Muted);
            y += 34;

            MakeLabel("Папка игры", x, y);
            y += 24;
            _folder = new TextBox
            {
                Left = x, Top = y, Width = 380, Height = 30, BorderStyle = BorderStyle.FixedSingle,
                BackColor = Theme.PanelLight, ForeColor = Theme.Text, Font = Theme.Font(10f),
                AutoSize = false
            };
            Controls.Add(_folder);

            var browse = Theme.FlatButton("Обзор…", 110, 30, Theme.PanelLight, Theme.Border);
            browse.Left = x + 392;
            browse.Top = y;
            browse.ForeColor = Theme.Text;
            browse.Click += (s, e) => BrowseFolder();
            Controls.Add(browse);
            y += 32;

            _folderHint = MakeLabel("", x, y, 8.5f, Theme.Muted);
            y += 34;

            MakeLabel("Память для игры", x, y);
            y += 24;
            _ram = new TrackBar
            {
                Left = x - 6, Top = y, Width = 380, Height = 40,
                Minimum = 2048, Maximum = Math.Max(4096, (int)Math.Min(16384, LauncherConfig.TotalRamMb() - 2048)),
                TickFrequency = 1024, SmallChange = 512, LargeChange = 1024, Value = 4096
            };
            _ram.ValueChanged += (s, e) => UpdateRamLabel();
            Controls.Add(_ram);

            _ramValue = MakeLabel("", x + 386, y + 12, 10f, Theme.Gold, true);
            y += 52;

            MakeLabel("Java", x, y, 9f, Theme.Muted);
            y += 20;
            _java = MakeLabel("", x, y, 8.5f, Theme.Muted);
            y += 30;

            var javaReset = Theme.FlatButton("Скачать Java 21 заново", 220, 30, Theme.PanelLight, Theme.Border);
            javaReset.Left = x; javaReset.Top = y; javaReset.ForeColor = Theme.Text;
            javaReset.Click += (s, e) =>
            {
                _config.JavaPath = null;
                _java.Text = "Будет скачана при следующем запуске";
            };
            Controls.Add(javaReset);
            y += 44;

            _error = MakeLabel("", x, y, 8.5f, Theme.Red);

            var ok = Theme.FlatButton(_firstRun ? "Продолжить" : "Сохранить", 150, 38, Theme.Green, Theme.GreenHover);
            ok.Left = 386; ok.Top = 440;
            ok.DialogResult = DialogResult.None;
            ok.Click += (s, e) => SaveAndClose();
            Controls.Add(ok);

            var cancel = Theme.FlatButton(_firstRun ? "Выход" : "Отмена", 130, 38, Theme.PanelLight, Theme.Border);
            cancel.Left = 244; cancel.Top = 440; cancel.ForeColor = Theme.Text;
            cancel.DialogResult = DialogResult.Cancel;
            Controls.Add(cancel);

            AcceptButton = ok;
            CancelButton = cancel;
        }

        private Label MakeLabel(string text, int x, int y, float size = 10f, Color? color = null, bool bold = false)
        {
            var label = new Label
            {
                Text = text,
                Left = x,
                Top = y,
                AutoSize = true,
                Font = Theme.Font(size, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color ?? Theme.Text,
                BackColor = Color.Transparent
            };
            Controls.Add(label);
            return label;
        }

        private void LoadValues()
        {
            _nick.Text = _config.Nickname ?? "";
            _folder.Text = _config.GameDir ?? "";
            if (string.IsNullOrEmpty(_folder.Text)) _folder.Text = SuggestFolder();

            int ram = _config.ResolveRamMb();
            if (ram < _ram.Minimum) ram = _ram.Minimum;
            if (ram > _ram.Maximum) ram = _ram.Maximum;
            _ram.Value = ram / 512 * 512;
            UpdateRamLabel();

            _java.Text = JavaInstaller.IsUsable(_config.JavaPath)
                ? _config.JavaPath
                : "Java 21 ещё не установлена — скачается автоматически";
            UpdateFolderHint();
        }

        private void UpdateRamLabel()
        {
            _ramValue.Text = string.Format("{0:0.#} ГБ", _ram.Value / 1024.0);
        }

        private void UpdateFolderHint()
        {
            string path = _folder.Text;
            string hint;
            try
            {
                string root = Path.GetPathRoot(path);
                var drive = DriveInfo.GetDrives().FirstOrDefault(d =>
                    string.Equals(d.Name, root, StringComparison.OrdinalIgnoreCase));
                hint = drive != null && drive.IsReady
                    ? string.Format("Свободно на {0}: {1:N0} ГБ • нужно ~4 ГБ", drive.Name.TrimEnd('\\'), drive.AvailableFreeSpace / 1024.0 / 1024 / 1024)
                    : "Папка будет создана автоматически";
            }
            catch
            {
                hint = "Папка будет создана автоматически";
            }
            _folderHint.Text = hint;
        }

        private void BrowseFolder()
        {
            using (var dialog = new FolderBrowserDialog
            {
                Description = "Выберите папку для игры (нужно ~4 ГБ свободного места)",
                SelectedPath = !string.IsNullOrEmpty(_folder.Text) ? _folder.Text : LauncherConfig.DefaultGameDir(),
                ShowNewFolderButton = true,
                                RootFolder = Environment.SpecialFolder.MyComputer
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _folder.Text = LauncherConfig.SanitizeGameDir(dialog.SelectedPath);
                    UpdateFolderHint();
                }
            }
        }

        private void SaveAndClose()
        {
            string nick = (_nick.Text ?? "").Trim();
            if (!IsValidNick(nick))
            {
                _error.Text = "Ник: 3–16 символов, латиница, цифры и подчёркивание.";
                return;
            }

            string folder = LauncherConfig.SanitizeGameDir((_folder.Text ?? "").Trim());
            if (folder.Length < 3)
            {
                _error.Text = "Выберите папку для игры.";
                return;
            }

            try
            {
                Directory.CreateDirectory(folder);
                string probe = Path.Combine(folder, ".write-test");
                File.WriteAllText(probe, "ok");
                File.Delete(probe);
            }
            catch (Exception ex)
            {
                _error.Text = "Нет доступа к папке: " + ex.Message;
                return;
            }

            _config.Nickname = nick;
            _config.GameDir = folder;
            _config.RamMb = _ram.Value;
            _config.Save();

            Saved = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private static bool IsValidNick(string nick)
        {
            if (nick == null || nick.Length < 3 || nick.Length > 16) return false;
            return nick.All(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                                 (c >= '0' && c <= '9') || c == '_');
        }

        public static string SuggestFolder()
        {
            try
            {
                var drives = DriveInfo.GetDrives()
                    .Where(d => d.IsReady && d.DriveType == DriveType.Fixed &&
                                d.Name.IndexOf("C:", StringComparison.OrdinalIgnoreCase) != 0)
                    .OrderByDescending(d => d.AvailableFreeSpace)
                    .ToList();

                var best = drives.FirstOrDefault() ?? DriveInfo.GetDrives().First(d => d.IsReady);
                return Path.Combine(best.Name, "SteampunkLauncher");
            }
            catch
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SteampunkLauncher");
            }
        }
    }
}
