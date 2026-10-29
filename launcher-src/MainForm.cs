using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    public class MainForm : Form
    {
        // ─── Состояние и конфигурация ──────────────────────────
        private readonly LauncherConfig _config;
        private readonly PackUpdater _updater = new PackUpdater();
        private ServerInfo _server = new ServerInfo();
        private CancellationTokenSource _cts;
        private bool _busy;
        private Image _bgImage;

        // ─── Навигация и контейнеры ───────────────────────────
        private Panel _headerBar;
        private Panel _logoPanel;
        private Panel _navContainer;
        private Panel _rightWrapper;
        private Panel _contentArea;
        private string _activeTab = "game";
        private readonly Dictionary<string, NavPillButton> _navButtons = new Dictionary<string, NavPillButton>();

        // ─── Вкладки ──────────────────────────────────────────
        private Panel _tabGame;
        private Panel _tabSkins;
        private Panel _tabScreenshots;
        private Panel _tabSettings;

        // ─── Профиль в верхней шапке ──────────────────────────
        private PictureBox _picProfileAvatar;
        private TextBox _txtHeaderNick;

        // ─── Элементы вкладки «Игра» (LoliLand Style) ───────────
        private Panel _gameCanvas;
        private AccentButton _btnLaunch;
        private AccentButton _btnCancel;
        private FlatProgressBar _progressBar;
        private Label _lblPhase;
        private Label _lblDetail;
        private TextBox _txtGameNick;
        private TrackBar _ramSlider;
        private TextBox _txtRamNumber;
        private Label _lblRamStatus;

        // ─── Элементы вкладки «Скины» (Modrinth Style) ─────────
        private SkinPreview _skinPreview;
        private TextBox _txtSkinPlayerNick;
        private Label _lblSkinModelType;
        private TileGrid _skinGrid;
        private TextBox _skinSearchBox;
        private Label _lblSkinStatus;
        private PictureBox _picActiveSkinCard;

        // ─── Элементы вкладки «Скриншоты» ──────────────────────
        private FlowLayoutPanel _screenshotFlow;
        private Label _lblScreenshotCount;
        private Button _btnFilterAll;
        private Button _btnFilterFav;
        private bool _showOnlyFavorites;

        // ─── Элементы вкладки «Настройки» ──────────────────────
        private TextBox _txtGameDir;
        private TextBox _txtJavaPath;
        private CheckBox _chkJoinServer;
        private TextBox _txtSettingsRam;
        private TrackBar _sliderSettingsRam;
        private RichTextBox _logBox;

        // ─── Дополнительные компоненты (Анимация, RPC, Шейдеры) ──
        private float _gearAngle = 0f;
        private bool _isLogoHovered = false;
        private System.Windows.Forms.Timer _gearAnimTimer;
        private ComboBox _cmbShaders;
        private CheckBox _chkDiscordRpc;

        public MainForm()
        {
            _config = LauncherConfig.Load();
            if (!string.IsNullOrEmpty(_config.JavaPath))
            {
                string fixJavaw = JavaInstaller.EnsureJavaw(_config.JavaPath);
                if (fixJavaw != _config.JavaPath)
                {
                    _config.JavaPath = fixJavaw;
                    _config.Save();
                }
            }

            _gearAnimTimer = new System.Windows.Forms.Timer { Interval = 33 };
            _gearAnimTimer.Tick += (s, e) =>
            {
                if (_busy || _isLogoHovered)
                {
                    _gearAngle = (_gearAngle + 3.5f) % 360f;
                    _logoPanel?.Invalidate();
                }
            };
            _gearAnimTimer.Start();

            // Свойства окна: по умолчанию развернуто во весь экран (Maximized)
            Text = "Steampunk & Swords Launcher";
            ClientSize = new Size(1200, 780);
            MinimumSize = new Size(1024, 660);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Bg;
            ForeColor = Theme.Text;
            Font = Theme.Font(9.5f);
            DoubleBuffered = true;

            // Иконка окна и процесса
            try
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }
            if (Icon == null)
            {
                try
                {
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("app.ico"))
                    {
                        if (stream != null) Icon = new Icon(stream);
                    }
                }
                catch { }
            }

            LoadBackgroundImage();
            BuildLayout();
            InitTabs();
            SwitchTab("game");

            SelfUpdater.CleanupLeftovers();
            AppendLog("Лаунчер запущен (v" + SelfUpdater.CurrentVersion + ")");
            if (!string.IsNullOrEmpty(_config.GameDir))
                AppendLog("Папка игры: " + _config.GameDir);

            LoadStoredSkin();
            UpdateProfileWidget();

            _ = InitBackgroundTasksAsync();
        }

        private void LoadBackgroundImage()
        {
            try
            {
                // 1. Попытка загрузить из папки assets
                string assetsBg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "background.png");
                if (File.Exists(assetsBg))
                {
                    _bgImage = Image.FromFile(assetsBg);
                    return;
                }

                // 2. Попытка загрузить из встроенных ресурсов сборки
                var asm = Assembly.GetExecutingAssembly();
                using (var stream = asm.GetManifestResourceStream("background.png"))
                {
                    if (stream != null)
                    {
                        _bgImage = Image.FromStream(stream);
                        return;
                    }
                }
            }
            catch { }
        }

        // ════════════════════════════════════════════════════════════════════
        //  БАЗОВЫЙ МАКЕТ (АККУРАТНАЯ ВЕРХНЯЯ ШАПКА БЕЗ НАЛОЖЕНИЙ)
        // ════════════════════════════════════════════════════════════════════
        private void BuildLayout()
        {
            // ─── ВЕРХНЯЯ ШАПКА (HEADER) ─────────────────────────
            _headerBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = Color.FromArgb(16, 20, 26)
            };

            // 1. Логотип слева (при клике переходит на главный экран)
            _logoPanel = new Panel
            {
                Location = new Point(20, 0),
                Size = new Size(190, 68),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            Action goHome = () => SwitchTab("game");
            _logoPanel.Click += (s, e) => goHome();
            _logoPanel.MouseEnter += (s, e) => { _isLogoHovered = true; };
            _logoPanel.MouseLeave += (s, e) => { _isLogoHovered = false; };

            _logoPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Четкая векторная шестеренка с плавной стимпанк-анимацией вращения
                Gear.Draw(g, new RectangleF(2, 14, 38, 38), Theme.Gold, _gearAngle);

                // Заголовок STEAMPUNK
                using (var fTitle = Theme.Display(13.5f, FontStyle.Bold))
                using (var bTitle = new SolidBrush(Theme.Gold))
                {
                    g.DrawString("STEAMPUNK", fTitle, bTitle, 46, 14);
                }

                // Буквальный подзаголовок & SWORDS (без мнемоники и обрезки)
                using (var fSub = Theme.Font(9f, FontStyle.Bold))
                using (var bSub = new SolidBrush(Theme.TextDim))
                {
                    g.DrawString("& SWORDS", fSub, bSub, 48, 37);
                }
            };

            _headerBar.Controls.Add(_logoPanel);

            // 2. Капсульные вкладки по центру (компактные пилюли без наложений)
            _navContainer = new Panel
            {
                Size = new Size(450, 44),
                BackColor = Color.Transparent
            };

            int btnX = 0;
            AddNavPill(_navContainer, "game",        "ГЛАВНАЯ",   ref btnX, 96,  () => SwitchTab("game"));
            AddNavPill(_navContainer, "skins",       "СКИНЫ",     ref btnX, 88,  () => SwitchTab("skins"));
            AddNavPill(_navContainer, "screenshots", "СКРИНШОТЫ", ref btnX, 122, () => SwitchTab("screenshots"));
            AddNavPill(_navContainer, "settings",    "НАСТРОЙКИ", ref btnX, 118, () => SwitchTab("settings"));

            _headerBar.Controls.Add(_navContainer);

            // 3. Блок быстрых кнопок и профиля справа (скругленная капсула)
            _rightWrapper = new Panel
            {
                Size = new Size(385, 46),
                BackColor = Color.Transparent
            };

            var btnChangelog = CreateHeaderIconBtn("📜", 0, "Что нового в сборке (Патчноут)", () =>
            {
                using (var dlg = new ChangelogDialog()) dlg.ShowDialog(this);
            });

            var btnDiscord = CreateHeaderIconBtn("💬", 42, "Наш Discord сервер", () =>
            {
                try { Process.Start(new ProcessStartInfo("https://discord.gg/") { UseShellExecute = true }); } catch { }
            });

            var btnWiki = CreateHeaderIconBtn("📖", 84, "База знаний и гайды", () =>
            {
                try { Process.Start(new ProcessStartInfo("https://github.com/Trubik1/minecraft") { UseShellExecute = true }); } catch { }
            });

            var profileBox = new Card
            {
                Location = new Point(130, 0),
                Size = new Size(250, 46),
                FillColor = Color.FromArgb(24, 28, 38),
                BorderColor = Theme.BorderSoft,
                Radius = 23f,
                Padding = new Padding(6)
            };

            _picProfileAvatar = new PictureBox
            {
                Location = new Point(8, 8),
                Size = new Size(30, 30),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Theme.Field,
                Cursor = Cursors.Hand
            };

            _txtHeaderNick = new TextBox
            {
                Text = string.IsNullOrEmpty(_config.Nickname) ? "Trubik" : _config.Nickname,
                Location = new Point(44, 11),
                Width = 160,
                Height = 24,
                Font = Theme.Font(10.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Theme.Field,
                BorderStyle = BorderStyle.FixedSingle
            };

            _txtHeaderNick.TextChanged += (s, e) =>
            {
                string val = _txtHeaderNick.Text.Trim();
                if (val.Length >= 2 && val.Length <= 16)
                {
                    _config.Nickname = val;
                    _config.Save();
                    if (_txtGameNick != null && _txtGameNick.Text != val) _txtGameNick.Text = val;
                    if (_txtSkinPlayerNick != null && _txtSkinPlayerNick.Text != val) _txtSkinPlayerNick.Text = val;
                    SkinStore.ApplyToGame(_config.GameDir, val);
                }
            };
            _txtHeaderNick.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    _contentArea.Focus();
                }
            };

            var btnEditProfile = new Label
            {
                Text = "✏",
                Font = Theme.Font(11f),
                ForeColor = Theme.Gold,
                Location = new Point(220, 11),
                Size = new Size(24, 24),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnEditProfile.Click += (s, e) => OpenProfileModal();
            _picProfileAvatar.Click += (s, e) => OpenProfileModal();

            profileBox.Controls.AddRange(new Control[] { _picProfileAvatar, _txtHeaderNick, btnEditProfile });
            _rightWrapper.Controls.AddRange(new Control[] { btnChangelog, btnDiscord, btnWiki, profileBox });
            _headerBar.Controls.Add(_rightWrapper);

            // Динамическое позиционирование шапки при ресайзе с защитой от наложений
            Action layoutHeader = () =>
            {
                _rightWrapper.Location = new Point(_headerBar.Width - _rightWrapper.Width - 20, 11);
                int leftLimit = _logoPanel.Right + 12;
                int rightLimit = _rightWrapper.Left - 12;
                int idealNavX = (_headerBar.Width - _navContainer.Width) / 2;
                int actualNavX = Math.Max(leftLimit, Math.Min(idealNavX, rightLimit - _navContainer.Width));
                _navContainer.Location = new Point(actualNavX, 12);
            };
            _headerBar.Resize += (s, e) => layoutHeader();
            layoutHeader();

            Controls.Add(_headerBar);

            // ─── Рабочая область (Content Area) ───────────────────
            _contentArea = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Bg
            };
            Controls.Add(_contentArea);
            _contentArea.BringToFront();
        }

        private void AddNavPill(Panel parent, string id, string title, ref int currentX, int width, Action onClick)
        {
            var btn = new NavPillButton(title)
            {
                Location = new Point(currentX, 2),
                Size = new Size(width, 40)
            };
            btn.Click += (s, e) => onClick();
            _navButtons[id] = btn;
            parent.Controls.Add(btn);
            currentX += width + 8;
        }

        private static Button CreateHeaderIconBtn(string text, int x, string tooltip, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, 5),
                Size = new Size(36, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(24, 28, 38),
                ForeColor = Theme.Gold,
                Font = Theme.Font(11f),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Theme.BorderSoft;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 46, 62);
            var tt = new ToolTip();
            tt.SetToolTip(btn, tooltip);
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private void OpenProfileModal()
        {
            using (var dlg = new ProfileDialog(_config))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK && dlg.NicknameChanged)
                {
                    _txtHeaderNick.Text = _config.Nickname;
                    UpdateProfileWidget();
                    UpdateSkinPlayerTitle();
                    LoadStoredSkin();
                    AppendLog("Никнейм изменен на «" + _config.Nickname + "»");
                }
            }
        }

        private void SwitchTab(string tabId)
        {
            _activeTab = tabId;

            foreach (var pair in _navButtons)
                pair.Value.Active = (pair.Key == tabId);

            _contentArea.SuspendLayout();
            _contentArea.Controls.Clear();

            switch (tabId)
            {
                case "game":
                    _contentArea.Controls.Add(_tabGame);
                    break;
                case "skins":
                    _contentArea.Controls.Add(_tabSkins);
                    UpdateSkinPlayerTitle();
                    break;
                case "screenshots":
                    _contentArea.Controls.Add(_tabScreenshots);
                    ReloadScreenshots();
                    break;
                case "settings":
                    _contentArea.Controls.Add(_tabSettings);
                    SyncSettingsFields();
                    break;
            }

            _contentArea.ResumeLayout(true);
        }

        private void InitTabs()
        {
            InitGameTab();
            InitSkinsTab();
            InitScreenshotsTab();
            InitSettingsTab();
        }

        private void UpdateProfileWidget()
        {
            try
            {
                var skin = SkinStore.Load();
                if (skin != null)
                {
                    var head = new Bitmap(30, 30);
                    using (var g = Graphics.FromImage(head))
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.PixelOffsetMode = PixelOffsetMode.Half;
                        g.DrawImage(skin, new Rectangle(0, 0, 30, 30), 8, 8, 8, 8, GraphicsUnit.Pixel);
                        g.DrawImage(skin, new Rectangle(0, 0, 30, 30), 40, 8, 8, 8, GraphicsUnit.Pixel);
                    }
                    _picProfileAvatar.Image = head;
                }
                else
                {
                    _picProfileAvatar.Image = null;
                }
            }
            catch { }
        }

        // ════════════════════════════════════════════════════════════════════
        //  ВКЛАДКА 1: ГЛАВНАЯ (LOLILAND STYLE: ФОН, ЧИСЛОВОЙ ВВОД RAM, ЗОЛОТАЯ КНОПКА)
        // ════════════════════════════════════════════════════════════════════
        private void InitGameTab()
        {
            _tabGame = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Bg };

            // Полноэкранный холст с отрисовкой кинематографичного арта и градиента
            _gameCanvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Bg
            };
            _gameCanvas.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(_gameCanvas, true, null);

            _gameCanvas.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.HighQuality;

                // 1. Отрисовка фонового арта с сохранением пропорций (Cover)
                if (_bgImage != null)
                {
                    float imgRatio = (float)_bgImage.Width / _bgImage.Height;
                    float screenRatio = (float)_gameCanvas.Width / _gameCanvas.Height;
                    float drawW, drawH, drawX, drawY;

                    if (screenRatio > imgRatio)
                    {
                        drawW = _gameCanvas.Width;
                        drawH = _gameCanvas.Width / imgRatio;
                        drawX = 0;
                        drawY = (_gameCanvas.Height - drawH) / 2f;
                    }
                    else
                    {
                        drawH = _gameCanvas.Height;
                        drawW = _gameCanvas.Height * imgRatio;
                        drawX = (_gameCanvas.Width - drawW) / 2f;
                        drawY = 0;
                    }

                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(_bgImage, drawX, drawY, drawW, drawH);
                }
                else
                {
                    using (var brush = new SolidBrush(Theme.Bg))
                        g.FillRectangle(brush, _gameCanvas.ClientRectangle);
                }

                // 2. Мягкое бесшовное градиентное затемнение снизу для идеальной читаемости (без швов)
                int gradH = (int)(_gameCanvas.Height * 0.55f);
                if (gradH > 10)
                {
                    int startY = _gameCanvas.Height - gradH;
                    var brushRect = new Rectangle(0, startY - 2, _gameCanvas.Width, gradH + 4);
                    using (var lgb = new LinearGradientBrush(brushRect,
                        Color.FromArgb(0, 14, 16, 22),
                        Color.FromArgb(245, 14, 16, 22),
                        LinearGradientMode.Vertical))
                    {
                        lgb.WrapMode = WrapMode.TileFlipY;
                        g.FillRectangle(lgb, 0, startY, _gameCanvas.Width, gradH);
                    }
                }
            };

            // ─── Снизу слева: Название, описание и ЧИСЛОВОЙ ВВОД RAM ───
            var bottomInfoPanel = new Panel
            {
                Location = new Point(32, _tabGame.Height - 330),
                Size = new Size(500, 315),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            var lblSubtitleTop = new Label
            {
                Text = "Увлекательное",
                Font = Theme.Display(23f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 0),
                AutoSize = true
            };

            var lblSubtitleGold = new Label
            {
                Text = "кубическое приключение",
                Font = Theme.Display(23f, FontStyle.Bold),
                ForeColor = Theme.Gold,
                Location = new Point(0, 40),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = "Steampunk & Swords — сборка с уникальными механизмами, магией и приключениями!\nMinecraft 1.21.1  •  NeoForge 21.1.264  •  Быстрое подключение и автосинхронизация",
                Font = Theme.Font(9.5f),
                ForeColor = Color.FromArgb(210, 220, 235),
                Location = new Point(0, 88),
                Size = new Size(500, 44)
            };

            // ─── Блок ввода никнейма игрока ───
            var nickCard = new Card
            {
                Location = new Point(0, 140),
                Size = new Size(490, 48),
                FillColor = Color.FromArgb(170, 20, 24, 32),
                BorderColor = Theme.BorderSoft,
                Radius = 12f,
                Padding = new Padding(12, 8, 12, 8)
            };

            var lblNickTitle = new Label
            {
                Text = "Никнейм игрока:",
                Font = Theme.Font(10f, FontStyle.Bold),
                ForeColor = Theme.Text,
                Location = new Point(14, 14),
                AutoSize = true
            };

            _txtGameNick = new TextBox
            {
                Text = string.IsNullOrEmpty(_config.Nickname) ? "Trubik" : _config.Nickname,
                Location = new Point(165, 10),
                Width = 220,
                Height = 26,
                Font = Theme.Font(11f, FontStyle.Bold),
                BackColor = Theme.Field,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            _txtGameNick.TextChanged += (s, e) =>
            {
                string val = _txtGameNick.Text.Trim();
                if (val.Length >= 2 && val.Length <= 16)
                {
                    _config.Nickname = val;
                    _config.Save();
                    if (_txtHeaderNick != null && _txtHeaderNick.Text != val) _txtHeaderNick.Text = val;
                    if (_txtSkinPlayerNick != null && _txtSkinPlayerNick.Text != val) _txtSkinPlayerNick.Text = val;
                    SkinStore.ApplyToGame(_config.GameDir, val);
                }
            };

            nickCard.Controls.AddRange(new Control[] { lblNickTitle, _txtGameNick });
            bottomInfoPanel.Controls.Add(nickCard);

            // ─── Блок оперативной памяти с ЧИСЛОВЫМ ВВОДОМ ───
            int curRam = _config.ResolveRamMb();
            long totalSystemRam = LauncherConfig.TotalRamMb();
            int maxSlider = Math.Max(8192, (int)totalSystemRam);

            var ramBoxCard = new Card
            {
                Location = new Point(0, 196),
                Size = new Size(490, 96),
                FillColor = Color.FromArgb(170, 20, 24, 32),
                BorderColor = Theme.BorderSoft,
                Radius = 12f,
                Padding = new Padding(12)
            };

            var lblRamTitle = new Label
            {
                Text = "Выделение памяти (RAM):",
                Font = Theme.Font(9.5f, FontStyle.Bold),
                ForeColor = Theme.Text,
                Location = new Point(14, 12),
                AutoSize = true
            };

            // Числовое поле ввода RAM (ГБ)
            _txtRamNumber = new TextBox
            {
                Text = string.Format("{0:0.#}", curRam / 1024.0),
                Location = new Point(205, 8),
                Size = new Size(52, 24),
                Font = Theme.Font(10.5f, FontStyle.Bold),
                BackColor = Theme.Field,
                ForeColor = Theme.Gold,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };

            var lblGbSuffix = new Label
            {
                Text = "ГБ",
                Font = Theme.Font(9.5f, FontStyle.Bold),
                ForeColor = Theme.Gold,
                Location = new Point(262, 11),
                AutoSize = true
            };

            _lblRamStatus = new Label
            {
                Text = string.Format("({0} МБ из {1} МБ)", curRam, totalSystemRam),
                Font = Theme.Font(8.5f),
                ForeColor = Theme.Muted,
                Location = new Point(292, 12),
                AutoSize = true
            };

            _ramSlider = new TrackBar
            {
                Location = new Point(10, 44),
                Size = new Size(300, 40),
                Minimum = 1024,
                Maximum = maxSlider,
                TickFrequency = 1024,
                SmallChange = 512,
                LargeChange = 1024,
                Value = Math.Min(maxSlider, Math.Max(1024, curRam)),
                BackColor = Color.FromArgb(20, 24, 32)
            };

            // Синхронизация слайдера с числовым полем
            _ramSlider.ValueChanged += (s, e) =>
            {
                int snapped = (_ramSlider.Value / 512) * 512;
                if (snapped < 1024) snapped = 1024;
                _config.RamMb = snapped;
                _config.Save();
                _txtRamNumber.Text = string.Format("{0:0.#}", snapped / 1024.0);
                _lblRamStatus.Text = string.Format("({0} МБ из {1} МБ)", snapped, totalSystemRam);
            };

            // Синхронизация числового поля со слайдером
            Action applyTypedRam = () =>
            {
                string raw = _txtRamNumber.Text.Trim().Replace(',', '.');
                double val;
                if (double.TryParse(raw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out val))
                {
                    int mb = (int)Math.Round(val * 1024);
                    if (mb < 1024) mb = 1024;
                    if (mb > maxSlider) mb = maxSlider;
                    _config.RamMb = mb;
                    _config.Save();
                    _ramSlider.Value = mb;
                    _lblRamStatus.Text = string.Format("({0} МБ из {1} МБ)", mb, totalSystemRam);
                }
            };
            _txtRamNumber.Leave += (s, e) => applyTypedRam();
            _txtRamNumber.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    applyTypedRam();
                    e.SuppressKeyPress = true;
                }
            };

            // Быстрые пресеты 4, 8, 12 ГБ
            Action<int> setPreset = (gb) =>
            {
                int mb = gb * 1024;
                if (mb > maxSlider) mb = maxSlider;
                _config.RamMb = mb;
                _config.Save();
                _ramSlider.Value = mb;
                _txtRamNumber.Text = gb.ToString();
                _lblRamStatus.Text = string.Format("({0} МБ из {1} МБ)", mb, totalSystemRam);
            };

            var btn4Gb = CreatePresetBtn("4G", 320, 46, () => setPreset(4));
            var btn8Gb = CreatePresetBtn("8G", 374, 46, () => setPreset(8));
            var btn12Gb = CreatePresetBtn("12G", 428, 46, () => setPreset(12));

            ramBoxCard.Controls.AddRange(new Control[] {
                lblRamTitle, _txtRamNumber, lblGbSuffix, _lblRamStatus, _ramSlider, btn4Gb, btn8Gb, btn12Gb
            });

            // Карточка пресетов графики («Слабый ПК», «Баланс», «Ультра»)
            var presetCard = new Card
            {
                Location = new Point(0, 276),
                Size = new Size(490, 42),
                FillColor = Color.FromArgb(20, 24, 32),
                BorderColor = Theme.BorderSoft,
                Radius = 12f
            };

            var lblPresetTitle = new Label
            {
                Text = "Графика:",
                Font = Theme.Font(9f, FontStyle.Bold),
                ForeColor = Theme.Text,
                Location = new Point(14, 12),
                AutoSize = true
            };

            var btnPotato = new Button();
            var btnBalanced = new Button();
            var btnUltra = new Button();

            Action updatePresetButtons = () =>
            {
                string cur = _config.GraphicsPreset ?? GraphicsPresets.Balanced;
                ConfigurePresetBtn(btnPotato, "🥔 Слабый ПК (Ноут)", cur == GraphicsPresets.Potato);
                ConfigurePresetBtn(btnBalanced, "⚙️ Баланс", cur == GraphicsPresets.Balanced);
                ConfigurePresetBtn(btnUltra, "✨ Ультра", cur == GraphicsPresets.Ultra);
            };

            Action<string> selectPreset = (p) =>
            {
                _config.GraphicsPreset = p;
                _config.Save();
                updatePresetButtons();
            };

            SetupPresetButton(btnPotato, 88, 6, 146, () => selectPreset(GraphicsPresets.Potato));
            SetupPresetButton(btnBalanced, 240, 6, 104, () => selectPreset(GraphicsPresets.Balanced));
            SetupPresetButton(btnUltra, 350, 6, 126, () => selectPreset(GraphicsPresets.Ultra));
            updatePresetButtons();

            presetCard.Controls.AddRange(new Control[] { lblPresetTitle, btnPotato, btnBalanced, btnUltra });

            bottomInfoPanel.Controls.AddRange(new Control[] {
                lblSubtitleTop, lblSubtitleGold, lblDesc, ramBoxCard, presetCard
            });
            _gameCanvas.Controls.Add(bottomInfoPanel);

            // ─── Снизу по центру: Большая золотая кнопка «НАЧАТЬ ИГРУ» ───
            _btnLaunch = new AccentButton
            {
                Text = "НАЧАТЬ ИГРУ",
                Size = new Size(280, 60),
                Font = Theme.Display(15f, FontStyle.Bold),
                Anchor = AnchorStyles.None
            };
            _btnLaunch.SetGoldGlowStyle();
            _btnLaunch.Click += (s, e) => StartWork(launchAfter: true);
            _gameCanvas.Controls.Add(_btnLaunch);

            _btnCancel = new AccentButton
            {
                Text = "Отмена",
                Size = new Size(120, 44),
                Radius = 22f,
                Font = Theme.Font(10.5f, FontStyle.Bold),
                Anchor = AnchorStyles.None,
                Visible = false
            };
            _btnCancel.SetRedStyle();
            _btnCancel.Radius = 22f;
            _btnCancel.UpdateRegion();
            _btnCancel.Click += (s, e) => { if (_cts != null) _cts.Cancel(); };
            _gameCanvas.Controls.Add(_btnCancel);

            // Прогресс-бар и статусы над кнопкой
            _progressBar = new FlatProgressBar
            {
                Size = new Size(460, 8),
                Anchor = AnchorStyles.None,
                Visible = false
            };
            _gameCanvas.Controls.Add(_progressBar);

            _lblPhase = new Label
            {
                Text = "Готов к запуску",
                Font = Theme.Font(10.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(460, 22),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.None,
                BackColor = Color.Transparent,
                Visible = false
            };
            _gameCanvas.Controls.Add(_lblPhase);

            _lblDetail = new Label
            {
                Text = "",
                Font = Theme.Font(9f),
                ForeColor = Theme.Gold,
                Size = new Size(460, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.None,
                BackColor = Color.Transparent,
                Visible = false
            };
            _gameCanvas.Controls.Add(_lblDetail);

            // Динамическое позиционирование элементов запуска с защитой от наложений на инфо-панель
            Action repositionLaunch = () =>
            {
                int bottomY = _gameCanvas.Height - 90;
                int infoRight = bottomInfoPanel.Right + 20; // Правый край карточек информации + зазор
                int idealX = (_gameCanvas.Width - _btnLaunch.Width) / 2;

                // В широком окне / на весь экран — центрируем по экрану.
                // В оконном режиме (если центр наезжает на левую панель) — смещаем в свободную правую зону!
                int launchX = Math.Max(infoRight, idealX);

                // Ограничиваем справа, чтобы не вылезало за границы
                int maxRight = _gameCanvas.Width - _btnLaunch.Width - 30;
                if (_btnCancel.Visible) maxRight -= (_btnCancel.Width + 14);
                if (launchX > maxRight) launchX = Math.Max(infoRight, maxRight);

                _btnLaunch.Location = new Point(launchX, bottomY);
                _btnCancel.Location = new Point(_btnLaunch.Right + 14, bottomY + 8);

                // Прогресс-бар центрируется строго относительно кнопки запуска
                int pbWidth = Math.Min(460, _gameCanvas.Width - launchX - 20);
                if (pbWidth < 280) pbWidth = 280;
                _progressBar.Size = new Size(pbWidth, 8);
                _progressBar.Location = new Point(launchX + (_btnLaunch.Width - pbWidth) / 2, bottomY - 18);
                _lblDetail.Location = new Point(launchX + (_btnLaunch.Width - _lblDetail.Width) / 2, bottomY - 40);
                _lblPhase.Location = new Point(launchX + (_btnLaunch.Width - _lblPhase.Width) / 2, bottomY - 62);
            };

            _gameCanvas.Resize += (s, e) =>
            {
                bottomInfoPanel.Location = new Point(32, Math.Max(70, _gameCanvas.Height - 340));
                repositionLaunch();
                _gameCanvas.Invalidate();
            };
            repositionLaunch();

            _tabGame.Controls.Add(_gameCanvas);
        }

        private static Button CreatePresetBtn(string text, int x, int y, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(46, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(32, 38, 50),
                ForeColor = Theme.TextDim,
                Font = Theme.Font(8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private static void SetupPresetButton(Button btn, int x, int y, int w, Action onClick)
        {
            btn.Location = new Point(x, y);
            btn.Size = new Size(w, 28);
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
            btn.Font = Theme.Font(8.5f, FontStyle.Bold);
            btn.FlatAppearance.BorderSize = 1;
            btn.Click += (s, e) => onClick();
        }

        private static void ConfigurePresetBtn(Button btn, string text, bool active)
        {
            btn.Text = text;
            btn.BackColor = active ? Color.FromArgb(45, 55, 75) : Color.FromArgb(24, 28, 36);
            btn.ForeColor = active ? Theme.Gold : Theme.TextDim;
            btn.FlatAppearance.BorderColor = active ? Theme.Gold : Theme.BorderSoft;
        }

        // ════════════════════════════════════════════════════════════════════
        //  ВКЛАДКА 2: СКИНИ (MODRINTH STYLE: ЧЕТКАЯ СТРУКТУРА БЕЗ НАЛОЖЕНИЙ)
        // ════════════════════════════════════════════════════════════════════
        private void InitSkinsTab()
        {
            _tabSkins = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Bg, Padding = new Padding(24) };

            // ─── Левая колонка: 3D персонаж (четкая панельная компоновка) ────
            var previewCard = new Card
            {
                Dock = DockStyle.Left,
                Width = 360,
                FillColor = Theme.Card,
                BorderColor = Theme.BorderSoft,
                Radius = 14f,
                Padding = new Padding(16)
            };

            // Верхняя часть: Имя и модель
            var topInfo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = Color.Transparent
            };
            var lblNickTag = new Label
            {
                Text = "Никнейм игрока (кликните для изменения):",
                Font = Theme.Font(7.5f),
                ForeColor = Theme.Gold,
                Dock = DockStyle.Top,
                Height = 16,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _txtSkinPlayerNick = new TextBox
            {
                Text = _config.Nickname ?? "Trubik",
                Font = Theme.Display(12f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Theme.Field,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Top,
                Height = 26,
                TextAlign = HorizontalAlignment.Center
            };
            _txtSkinPlayerNick.TextChanged += (s, e) =>
            {
                string val = _txtSkinPlayerNick.Text.Trim();
                if (val.Length >= 2 && val.Length <= 16)
                {
                    _config.Nickname = val;
                    _config.Save();
                    if (_txtHeaderNick != null && _txtHeaderNick.Text != val) _txtHeaderNick.Text = val;
                    if (_txtGameNick != null && _txtGameNick.Text != val) _txtGameNick.Text = val;
                    SkinStore.ApplyToGame(_config.GameDir, val);
                }
            };
            _lblSkinModelType = new Label
            {
                Text = "Модель: Стив (Classic 4px)",
                Font = Theme.Font(8.5f),
                ForeColor = Theme.Muted,
                Dock = DockStyle.Top,
                Height = 18,
                TextAlign = ContentAlignment.MiddleCenter
            };
            topInfo.Controls.AddRange(new Control[] { _lblSkinModelType, _txtSkinPlayerNick, lblNickTag });
            previewCard.Controls.Add(topInfo);

            // Нижняя часть: Кнопки управления (ВСЕГДА ВИДНЫ)
            var bottomActions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 185,
                BackColor = Color.Transparent
            };

            var lblHint = new Label
            {
                Text = "⟳ Вращение: зажать ЛКМ  •  Зум: колесико мыши",
                Font = Theme.Font(8f),
                ForeColor = Theme.Faint,
                Dock = DockStyle.Top,
                Height = 22,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var btnUpload = new AccentButton
            {
                Text = "📁  Загрузить PNG скин",
                Dock = DockStyle.Top,
                Height = 44,
                Font = Theme.Font(10f, FontStyle.Bold)
            };
            btnUpload.SetGreenStyle();
            btnUpload.Click += (s, e) => UploadSkinFromFile();

            var spacer2 = new Panel { Dock = DockStyle.Top, Height = 6 };

            var btnToggleCape = new AccentButton
            {
                Text = _config.ShowCape ? "✨ Стимпанк-плащ: Включен" : "✨ Стимпанк-плащ: Выключен",
                Dock = DockStyle.Top,
                Height = 34,
                Font = Theme.Font(9f)
            };
            btnToggleCape.SetDarkStyle();
            btnToggleCape.ForeColor = _config.ShowCape ? Theme.Gold : Theme.TextDim;
            btnToggleCape.Click += (s, e) =>
            {
                _config.ShowCape = !_config.ShowCape;
                _config.Save();
                _skinPreview.ShowCape = _config.ShowCape;
                btnToggleCape.Text = _config.ShowCape ? "✨ Стимпанк-плащ: Включен" : "✨ Стимпанк-плащ: Выключен";
                btnToggleCape.ForeColor = _config.ShowCape ? Theme.Gold : Theme.TextDim;
            };

            var spacer = new Panel { Dock = DockStyle.Top, Height = 6 };

            var btnReset = new AccentButton
            {
                Text = "Сбросить на стандартный",
                Dock = DockStyle.Top,
                Height = 34,
                Font = Theme.Font(9f)
            };
            btnReset.SetDarkStyle();
            btnReset.Click += (s, e) =>
            {
                SkinStore.Reset(_config.GameDir, _config.Nickname);
                _skinPreview.SetSkin(null);
                UpdateProfileWidget();
                UpdateActiveSkinCardImage();
                AppendLog("Скин сброшен к стандартному");
                SetSkinStatus("Скин сброшен", false);
            };

            _lblSkinStatus = new Label
            {
                Text = "",
                Font = Theme.Font(8.5f),
                ForeColor = Theme.Gold,
                Dock = DockStyle.Bottom,
                Height = 24,
                TextAlign = ContentAlignment.MiddleCenter
            };

            bottomActions.Controls.AddRange(new Control[] {
                _lblSkinStatus, btnReset, spacer, btnToggleCape, spacer2, btnUpload, lblHint
            });
            previewCard.Controls.Add(bottomActions);

            // Центральная часть: 3D просмотрщик (заполняет всё свободное пространство)
            _skinPreview = new SkinPreview
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Field,
                ShowCape = _config.ShowCape
            };
            _skinPreview.SkinChanged += (s, e) =>
            {
                _lblSkinModelType.Text = _skinPreview.IsSlim ? "Модель: Алекс (Slim 3px)" : "Модель: Стив (Classic 4px)";
                UpdateActiveSkinCardImage();
            };
            previewCard.Controls.Add(_skinPreview);
            _skinPreview.BringToFront();

            _tabSkins.Controls.Add(previewCard);

            // ─── Правая часть: Верхние карточки + Каталог ───────
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 0, 0, 0)
            };

            // 1. Верхний блок карточек в стиле Modrinth ("Ваш скин", "+ Добавить")
            var topCardsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.Transparent
            };

            var activeCard = new Card
            {
                Location = new Point(0, 0),
                Size = new Size(230, 96),
                FillColor = Theme.Card,
                BorderColor = Theme.GreenDark,
                Radius = 12f,
                Cursor = Cursors.Hand
            };
            _picActiveSkinCard = new PictureBox
            {
                Location = new Point(14, 14),
                Size = new Size(54, 68),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Theme.Field
            };
            var lblActiveTitle = new Label
            {
                Text = "Ваш скин",
                Font = Theme.Font(10.5f, FontStyle.Bold),
                ForeColor = Theme.Text,
                Location = new Point(78, 22),
                AutoSize = true
            };
            var badgeActive = new Label
            {
                Text = "✓ Активен",
                Font = Theme.Font(9f, FontStyle.Bold),
                ForeColor = Theme.Green,
                Location = new Point(78, 48),
                AutoSize = true
            };
            activeCard.Controls.AddRange(new Control[] { _picActiveSkinCard, lblActiveTitle, badgeActive });

            var addSkinCard = new Card
            {
                Location = new Point(246, 0),
                Size = new Size(240, 96),
                FillColor = Theme.CardAlt,
                BorderColor = Theme.BorderSoft,
                Radius = 12f,
                Cursor = Cursors.Hand
            };
            var lblAddIcon = new Label
            {
                Text = "＋",
                Font = Theme.Font(24f, FontStyle.Bold),
                ForeColor = Theme.Green,
                Location = new Point(16, 24),
                Size = new Size(36, 46),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var lblAddText = new Label
            {
                Text = "Добавить скин\nиз файла PNG",
                Font = Theme.Font(9.5f, FontStyle.Bold),
                ForeColor = Theme.TextDim,
                Location = new Point(58, 28),
                AutoSize = true
            };
            addSkinCard.Controls.AddRange(new Control[] { lblAddIcon, lblAddText });
            addSkinCard.Click += (s, e) => UploadSkinFromFile();
            lblAddIcon.Click += (s, e) => UploadSkinFromFile();
            lblAddText.Click += (s, e) => UploadSkinFromFile();

            topCardsPanel.Controls.AddRange(new Control[] { activeCard, addSkinCard });
            rightPanel.Controls.Add(topCardsPanel);

            // 2. Блок каталога скинов
            var catalogCard = new Card
            {
                Dock = DockStyle.Fill,
                FillColor = Theme.Card,
                BorderColor = Theme.BorderSoft,
                Radius = 14f,
                Padding = new Padding(20),
                Margin = new Padding(0, 14, 0, 0)
            };

            var searchHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.Transparent
            };
            var lblCatalogTitle = new Label
            {
                Text = "Популярные скины",
                Font = Theme.Display(13.5f, FontStyle.Bold),
                ForeColor = Theme.Text,
                Location = new Point(0, 10),
                AutoSize = true
            };

            var lblSearchHint = new Label
            {
                Text = "Поиск по нику:",
                Font = Theme.Font(9.5f),
                ForeColor = Theme.TextDim,
                AutoSize = true
            };

            _skinSearchBox = new TextBox
            {
                Text = "",
                Width = 170,
                Height = 28,
                Font = Theme.Font(10f),
                BackColor = Theme.Field,
                ForeColor = Theme.Text,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnSearch = new AccentButton
            {
                Text = "Найти по нику",
                Width = 120,
                Height = 32,
                Font = Theme.Font(9f, FontStyle.Bold)
            };
            btnSearch.SetGreenStyle();

            Action layoutSearch = () =>
            {
                btnSearch.Location = new Point(searchHeader.Width - btnSearch.Width - 4, 6);
                _skinSearchBox.Location = new Point(btnSearch.Left - _skinSearchBox.Width - 8, 8);
                lblSearchHint.Location = new Point(_skinSearchBox.Left - lblSearchHint.Width - 8, 12);
                lblSearchHint.Visible = (lblSearchHint.Left > lblCatalogTitle.Right + 12);
            };
            searchHeader.Resize += (s, e) => layoutSearch();

            btnSearch.Click += async (s, e) =>
            {
                string targetNick = _skinSearchBox.Text.Trim();
                if (string.IsNullOrEmpty(targetNick)) return;
                SetSkinStatus("Скачивание скина «" + targetNick + "»...", false);
                string err = await SkinCatalog.ApplyByNameAsync(targetNick, CancellationToken.None);
                if (err != null)
                {
                    SetSkinStatus("Ошибка: " + err, true);
                }
                else
                {
                    LoadStoredSkin();
                    UpdateProfileWidget();
                    UpdateActiveSkinCardImage();
                    SetSkinStatus("Скин «" + targetNick + "» успешно применен!", false);
                    AppendLog("Установлен скин «" + targetNick + "»");
                }
            };

            searchHeader.Controls.AddRange(new Control[] { lblCatalogTitle, lblSearchHint, _skinSearchBox, btnSearch });
            layoutSearch();
            catalogCard.Controls.Add(searchHeader);

            _skinGrid = new TileGrid
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Card,
                TileWidth = 115,
                TileHeight = 175,
                Gap = 14
            };
            _skinGrid.TileActivated += async (s, tile) =>
            {
                if (tile == null || string.IsNullOrEmpty(tile.Title)) return;
                SetSkinStatus("Применение скина «" + tile.Title + "»...", false);
                string err = await SkinCatalog.ApplyByNameAsync(tile.Title, CancellationToken.None);
                if (err != null)
                {
                    SetSkinStatus("Ошибка: " + err, true);
                }
                else
                {
                    LoadStoredSkin();
                    UpdateProfileWidget();
                    UpdateActiveSkinCardImage();
                    SetSkinStatus("Скин «" + tile.Title + "» установлен!", false);
                    AppendLog("Выбран скин «" + tile.Title + "»");
                }
            };

            catalogCard.Controls.Add(_skinGrid);
            _skinGrid.BringToFront();

            rightPanel.Controls.Add(catalogCard);
            catalogCard.BringToFront();

            _tabSkins.Controls.Add(rightPanel);
            rightPanel.BringToFront();

            // Drag & Drop файла скина
            _tabSkins.AllowDrop = true;
            _tabSkins.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
            };
            _tabSkins.DragDrop += (s, e) =>
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                    ApplySkinPath(files[0]);
            };

            PopulateSkinCatalog();
        }

        private void UpdateActiveSkinCardImage()
        {
            try
            {
                var skin = SkinStore.Load();
                if (skin != null)
                {
                    var head = new Bitmap(54, 54);
                    using (var g = Graphics.FromImage(head))
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.PixelOffsetMode = PixelOffsetMode.Half;
                        g.DrawImage(skin, new Rectangle(0, 0, 54, 54), 8, 8, 8, 8, GraphicsUnit.Pixel);
                        g.DrawImage(skin, new Rectangle(0, 0, 54, 54), 40, 8, 8, 8, GraphicsUnit.Pixel);
                    }
                    _picActiveSkinCard.Image = head;
                }
            }
            catch { }
        }

        private void PopulateSkinCatalog()
        {
            _skinGrid.Tiles.Clear();
            foreach (var entry in SkinCatalog.Popular)
            {
                var tile = new TileGrid.Tile
                {
                    Title = entry.Title,
                    Tag = entry.Name,
                    Busy = false
                };

                // Загружаем готовый скин с диска МГНОВЕННО (0 секунд ожидания)
                var cached = SkinCatalog.LoadRender(entry.Name);
                if (cached != null)
                {
                    tile.Image = cached;
                    tile.Busy = false;
                }
                else
                {
                    tile.Busy = true;
                    string name = entry.Name;
                    Task.Run(async () =>
                    {
                        try
                        {
                            var bmp = await SkinCatalog.FetchRenderAsync(name, 140, CancellationToken.None);
                            if (bmp != null)
                            {
                                if (IsHandleCreated)
                                {
                                    BeginInvoke((Action)(() =>
                                    {
                                        tile.Image = bmp;
                                        tile.Busy = false;
                                        _skinGrid.Invalidate();
                                    }));
                                }
                                else
                                {
                                    tile.Image = bmp;
                                    tile.Busy = false;
                                }
                            }
                        }
                        catch { }
                    });
                }

                _skinGrid.Tiles.Add(tile);
            }
            _skinGrid.Invalidate();
        }

        private void UpdateSkinPlayerTitle()
        {
            string nick = string.IsNullOrEmpty(_config.Nickname) ? "Trubik" : _config.Nickname;
            if (_txtSkinPlayerNick != null && _txtSkinPlayerNick.Text != nick) _txtSkinPlayerNick.Text = nick;
            if (_txtGameNick != null && _txtGameNick.Text != nick) _txtGameNick.Text = nick;
            if (_txtHeaderNick != null && _txtHeaderNick.Text != nick) _txtHeaderNick.Text = nick;
        }

        private void SetSkinStatus(string text, bool isError)
        {
            if (_lblSkinStatus != null)
            {
                _lblSkinStatus.Text = text;
                _lblSkinStatus.ForeColor = isError ? Theme.Red : Theme.Gold;
            }
        }

        private void LoadStoredSkin()
        {
            try
            {
                var skin = SkinStore.Load();
                if (_skinPreview != null)
                    _skinPreview.SetSkin(skin);
                UpdateActiveSkinCardImage();
            }
            catch { }
        }

        private void UploadSkinFromFile()
        {
            using (var dlg = new OpenFileDialog
            {
                Title = "Выберите файл скина Minecraft (64x64 или 64x32)",
                Filter = "Скины Minecraft (*.png)|*.png"
            })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    ApplySkinPath(dlg.FileName);
            }
        }

        private void ApplySkinPath(string path)
        {
            string error = SkinStore.Import(path);
            if (error != null)
            {
                MessageBox.Show(this, error, "Ошибка скина", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetSkinStatus(error, true);
                return;
            }

            LoadStoredSkin();
            UpdateProfileWidget();
            SkinStore.ApplyToGame(_config.GameDir, _config.Nickname);
            SetSkinStatus("Скин успешно обновлен!", false);
            AppendLog("Пользовательский скин загружен из " + Path.GetFileName(path));
        }

        // ════════════════════════════════════════════════════════════════════
        //  ВКЛАДКА 3: СКРИНШОТЫ (ИЗБРАННОЕ И УДАЛЕНИЕ)
        // ════════════════════════════════════════════════════════════════════
        private void InitScreenshotsTab()
        {
            _tabScreenshots = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Bg, Padding = new Padding(24) };

            var topCard = new Card
            {
                Dock = DockStyle.Top,
                Height = 64,
                FillColor = Theme.Panel,
                BorderColor = Theme.BorderSoft,
                Radius = 12f,
                Padding = new Padding(16, 12, 16, 12)
            };

            var title = new Label
            {
                Text = "Галерея скриншотов",
                Font = Theme.Display(13f, FontStyle.Bold),
                ForeColor = Theme.Text,
                Location = new Point(16, 16),
                AutoSize = true
            };

            _lblScreenshotCount = new Label
            {
                Text = "",
                Font = Theme.Font(9f),
                ForeColor = Theme.Muted,
                Location = new Point(200, 18),
                AutoSize = true
            };

            _btnFilterAll = new Button
            {
                Text = "Все снимки",
                Location = new Point(340, 12),
                Size = new Size(110, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Card,
                ForeColor = Theme.Green,
                Font = Theme.Font(9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnFilterAll.FlatAppearance.BorderSize = 0;
            _btnFilterAll.Click += (s, e) =>
            {
                _showOnlyFavorites = false;
                _btnFilterAll.ForeColor = Theme.Green;
                _btnFilterFav.ForeColor = Theme.TextDim;
                ReloadScreenshots();
            };

            _btnFilterFav = new Button
            {
                Text = "⭐ Избранные",
                Location = new Point(456, 12),
                Size = new Size(120, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Card,
                ForeColor = Theme.TextDim,
                Font = Theme.Font(9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnFilterFav.FlatAppearance.BorderSize = 0;
            _btnFilterFav.Click += (s, e) =>
            {
                _showOnlyFavorites = true;
                _btnFilterFav.ForeColor = Theme.Gold;
                _btnFilterAll.ForeColor = Theme.TextDim;
                ReloadScreenshots();
            };

            var btnOpenFolder = new AccentButton
            {
                Text = "📁 Папка скриншотов",
                Location = new Point(600, 12),
                Width = 190,
                Height = 36,
                Font = Theme.Font(9f, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnOpenFolder.SetDarkStyle();
            btnOpenFolder.Click += (s, e) =>
            {
                string shotsDir = Path.Combine(_config.GameDir ?? "", "screenshots");
                Directory.CreateDirectory(shotsDir);
                Process.Start("explorer.exe", "\"" + shotsDir + "\"");
            };

            var btnRefresh = new AccentButton
            {
                Text = "🔄 Обновить",
                Location = new Point(800, 12),
                Width = 110,
                Height = 36,
                Font = Theme.Font(9f),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnRefresh.SetDarkStyle();
            btnRefresh.Click += (s, e) => ReloadScreenshots();

            topCard.Controls.AddRange(new Control[] {
                title, _lblScreenshotCount, _btnFilterAll, _btnFilterFav, btnOpenFolder, btnRefresh
            });
            _tabScreenshots.Controls.Add(topCard);

            _screenshotFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Bg,
                AutoScroll = true,
                Padding = new Padding(0, 16, 0, 0)
            };
            _tabScreenshots.Controls.Add(_screenshotFlow);
            _screenshotFlow.BringToFront();
        }

        private void ReloadScreenshots()
        {
            if (_screenshotFlow == null) return;
            _screenshotFlow.Controls.Clear();

            string shotsDir = Path.Combine(_config.GameDir ?? "", "screenshots");
            if (!Directory.Exists(shotsDir))
            {
                _lblScreenshotCount.Text = "Папка пуста";
                return;
            }

            var allFiles = Directory.GetFiles(shotsDir, "*.png");
            Array.Sort(allFiles, (a, b) => File.GetLastWriteTime(b).CompareTo(File.GetLastWriteTime(a)));

            var files = new List<string>();
            foreach (var f in allFiles)
            {
                if (!_showOnlyFavorites || ScreenshotFavorites.IsFavorite(f))
                    files.Add(f);
            }

            _lblScreenshotCount.Text = string.Format("Показано: {0} из {1}", files.Count, allFiles.Length);

            if (files.Count == 0)
            {
                var lblEmpty = new Label
                {
                    Text = _showOnlyFavorites
                        ? "⭐ В избранном пока ничего нет.\nНажмите на звёздочку на карточке скриншота, чтобы добавить его сюда!"
                        : "Скриншотов пока нет.\nНажмите клавишу F2 в игре, чтобы сделать снимок экрана!",
                    Font = Theme.Font(11f),
                    ForeColor = Theme.Muted,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(540, 120),
                    Margin = new Padding(40)
                };
                _screenshotFlow.Controls.Add(lblEmpty);
                return;
            }

            foreach (var file in files)
            {
                string filePath = file;
                bool isFav = ScreenshotFavorites.IsFavorite(filePath);

                var card = new Card
                {
                    Width = 260,
                    Height = 226,
                    Margin = new Padding(12),
                    FillColor = Theme.Card,
                    BorderColor = isFav ? Theme.Gold : Theme.BorderSoft,
                    Cursor = Cursors.Hand
                };

                var pic = new PictureBox
                {
                    Location = new Point(10, 10),
                    Size = new Size(240, 135),
                    SizeMode = PictureBoxSizeMode.CenterImage,
                    BackColor = Theme.Field,
                    Cursor = Cursors.Hand
                };

                var lblDate = new Label
                {
                    Text = File.GetLastWriteTime(filePath).ToString("dd.MM.yyyy  HH:mm"),
                    Location = new Point(12, 150),
                    Width = 236,
                    Height = 20,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = Theme.TextDim,
                    Font = Theme.Font(9f)
                };

                // Кнопка ⭐ Избранное
                var btnFav = new Button
                {
                    Text = isFav ? "★" : "☆",
                    Location = new Point(12, 178),
                    Size = new Size(38, 34),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Theme.Field,
                    ForeColor = isFav ? Theme.Gold : Theme.Muted,
                    Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnFav.FlatAppearance.BorderSize = 0;
                btnFav.Click += (s, e) =>
                {
                    ScreenshotFavorites.ToggleFavorite(filePath);
                    ReloadScreenshots();
                };

                // Кнопка 🗑 Удалить
                var btnDel = new Button
                {
                    Text = "🗑 Удалить",
                    Location = new Point(138, 178),
                    Size = new Size(110, 34),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Theme.Field,
                    ForeColor = Theme.TextDim,
                    Font = Theme.Font(9f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnDel.FlatAppearance.BorderSize = 0;
                btnDel.MouseEnter += (s, e) => { btnDel.ForeColor = Theme.Red; btnDel.BackColor = Color.FromArgb(40, 215, 75, 75); };
                btnDel.MouseLeave += (s, e) => { btnDel.ForeColor = Theme.TextDim; btnDel.BackColor = Theme.Field; };
                btnDel.Click += (s, e) =>
                {
                    var ask = MessageBox.Show(this, "Удалить скриншот «" + Path.GetFileName(filePath) + "» безвозвратно?",
                        "Удаление скриншота", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (ask == DialogResult.Yes)
                    {
                        try
                        {
                            File.Delete(filePath);
                            ReloadScreenshots();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, "Не удалось удалить: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                };

                card.Controls.AddRange(new Control[] { pic, lblDate, btnFav, btnDel });
                _screenshotFlow.Controls.Add(card);

                pic.Click += (s, e) => OpenScreenshot(filePath);
                lblDate.Click += (s, e) => OpenScreenshot(filePath);

                // Загрузка миниатюры асинхронно с сохранением пропорций (без растягивания)
                Task.Run(() =>
                {
                    try
                    {
                        using (var src = Image.FromFile(filePath))
                        {
                            float scale = Math.Min(240f / src.Width, 135f / src.Height);
                            int tw = Math.Max(1, (int)(src.Width * scale));
                            int th = Math.Max(1, (int)(src.Height * scale));
                            var thumb = new Bitmap(240, 135);
                            using (var g = Graphics.FromImage(thumb))
                            {
                                g.Clear(Theme.Field);
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.DrawImage(src, (240 - tw) / 2, (135 - th) / 2, tw, th);
                            }
                            if (IsHandleCreated)
                                BeginInvoke((Action)(() => { pic.Image = thumb; }));
                            else
                                pic.Image = thumb;
                        }
                    }
                    catch { }
                });
            }
        }

        private void OpenScreenshot(string path)
        {
            try
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Не удалось открыть файл: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  ВКЛАДКА 4: НАСТРОЙКИ (ПОЛНОРАЗМЕРНЫЙ СОВРЕМЕННЫЙ ДИЗАЙН)
        // ════════════════════════════════════════════════════════════════════
        private void InitSettingsTab()
        {
            _tabSettings = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Bg, Padding = new Padding(28), AutoScroll = true };

            // Карточка 1: Пути и директории
            var cardPaths = new Card
            {
                Dock = DockStyle.Top,
                Height = 180,
                FillColor = Theme.Card,
                BorderColor = Theme.BorderSoft,
                Radius = 14f,
                Padding = new Padding(24)
            };

            var titlePaths = new Label
            {
                Text = "Папки и директории",
                Font = Theme.Display(13f, FontStyle.Bold),
                ForeColor = Theme.Text,
                Location = new Point(24, 16),
                AutoSize = true
            };

            var lblDir = new Label
            {
                Text = "Папка установки игры (.minecraft):",
                Location = new Point(24, 46),
                Font = Theme.Font(9f),
                ForeColor = Theme.TextDim,
                AutoSize = true
            };

            _txtGameDir = new TextBox
            {
                Text = _config.GameDir ?? "",
                Location = new Point(24, 68),
                Size = new Size(650, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = Theme.Font(10.5f),
                BackColor = Theme.Field,
                ForeColor = Theme.Text,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnBrowseDir = new AccentButton
            {
                Text = "Обзор...",
                Location = new Point(686, 66),
                Width = 110,
                Height = 32,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = Theme.Font(9f)
            };
            btnBrowseDir.SetDarkStyle();
            btnBrowseDir.Click += (s, e) =>
            {
                using (var dlg = new FolderBrowserDialog { SelectedPath = _config.GameDir ?? "" })
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        _config.GameDir = dlg.SelectedPath;
                        _config.Save();
                        _txtGameDir.Text = dlg.SelectedPath;
                        AppendLog("Новая папка игры: " + dlg.SelectedPath);
                    }
                }
            };

            var lblJava = new Label
            {
                Text = "Путь к Java 21 (javaw.exe, оставьте пустым для автопоиска):",
                Location = new Point(24, 108),
                Font = Theme.Font(9f),
                ForeColor = Theme.TextDim,
                AutoSize = true
            };

            _txtJavaPath = new TextBox
            {
                Text = _config.JavaPath ?? "",
                Location = new Point(24, 130),
                Size = new Size(650, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = Theme.Font(10.5f),
                BackColor = Theme.Field,
                ForeColor = Theme.Text,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnBrowseJava = new AccentButton
            {
                Text = "Обзор...",
                Location = new Point(686, 128),
                Width = 110,
                Height = 32,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = Theme.Font(9f)
            };
            btnBrowseJava.SetDarkStyle();
            btnBrowseJava.Click += (s, e) =>
            {
                using (var dlg = new OpenFileDialog { Filter = "Java Executable (javaw.exe;java.exe)|javaw.exe;java.exe" })
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        string chosen = JavaInstaller.EnsureJavaw(dlg.FileName);
                        _config.JavaPath = chosen;
                        _config.Save();
                        _txtJavaPath.Text = chosen;
                        AppendLog("Java: " + chosen);
                    }
                }
            };

            cardPaths.Controls.AddRange(new Control[] {
                titlePaths, lblDir, _txtGameDir, btnBrowseDir, lblJava, _txtJavaPath, btnBrowseJava
            });
            _tabSettings.Controls.Add(cardPaths);

            var spacer1 = new Panel { Dock = DockStyle.Top, Height = 14 };
            _tabSettings.Controls.Add(spacer1);

            // Карточка 2: Параметры запуска, графика и оптимизация
            var cardOptions = new Card
            {
                Dock = DockStyle.Top,
                Height = 225,
                FillColor = Theme.Card,
                BorderColor = Theme.BorderSoft,
                Radius = 14f,
                Padding = new Padding(24)
            };

            var titleOpts = new Label
            {
                Text = "Параметры запуска, графика и оптимизация",
                Font = Theme.Display(13f, FontStyle.Bold),
                ForeColor = Theme.Text,
                Location = new Point(24, 16),
                AutoSize = true
            };

            var lblSettingsRam = new Label
            {
                Text = "Выделение оперативной памяти:",
                Location = new Point(24, 48),
                Font = Theme.Font(9.5f),
                ForeColor = Theme.Text,
                AutoSize = true
            };

            int curRam = _config.ResolveRamMb();
            long totalSystemRam = LauncherConfig.TotalRamMb();
            int maxSlider = Math.Max(8192, (int)totalSystemRam);

            _txtSettingsRam = new TextBox
            {
                Text = string.Format("{0:0.#}", curRam / 1024.0),
                Location = new Point(240, 46),
                Size = new Size(60, 24),
                Font = Theme.Font(10f, FontStyle.Bold),
                BackColor = Theme.Field,
                ForeColor = Theme.Gold,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };

            var lblSettingsRamUnit = new Label
            {
                Text = "ГБ",
                Location = new Point(306, 48),
                Font = Theme.Font(9.5f, FontStyle.Bold),
                ForeColor = Theme.Gold,
                AutoSize = true
            };

            _sliderSettingsRam = new TrackBar
            {
                Location = new Point(340, 42),
                Size = new Size(340, 40),
                Minimum = 1024,
                Maximum = maxSlider,
                TickFrequency = 1024,
                SmallChange = 512,
                LargeChange = 1024,
                Value = Math.Min(maxSlider, Math.Max(1024, curRam)),
                BackColor = Theme.Card
            };

            _sliderSettingsRam.ValueChanged += (s, e) =>
            {
                int snapped = (_sliderSettingsRam.Value / 512) * 512;
                if (snapped < 1024) snapped = 1024;
                _config.RamMb = snapped;
                _config.Save();
                _txtSettingsRam.Text = string.Format("{0:0.#}", snapped / 1024.0);
                if (_ramSlider != null) _ramSlider.Value = snapped;
                if (_txtRamNumber != null) _txtRamNumber.Text = string.Format("{0:0.#}", snapped / 1024.0);
            };

            _txtSettingsRam.Leave += (s, e) =>
            {
                string raw = _txtSettingsRam.Text.Trim().Replace(',', '.');
                double val;
                if (double.TryParse(raw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out val))
                {
                    int mb = (int)Math.Round(val * 1024);
                    if (mb < 1024) mb = 1024;
                    if (mb > maxSlider) mb = maxSlider;
                    _config.RamMb = mb;
                    _config.Save();
                    _sliderSettingsRam.Value = mb;
                }
            };

            _chkJoinServer = new CheckBox
            {
                Text = "Подключаться к серверу автоматически при старте",
                Location = new Point(24, 88),
                Width = 380,
                AutoSize = true,
                ForeColor = Theme.TextDim,
                BackColor = Color.Transparent,
                Checked = _config.JoinServer,
                Cursor = Cursors.Hand
            };
            _chkJoinServer.CheckedChanged += (s, e) =>
            {
                _config.JoinServer = _chkJoinServer.Checked;
                _config.Save();
            };

            _chkDiscordRpc = new CheckBox
            {
                Text = "Отображать статус в Discord (RPC)",
                Location = new Point(410, 88),
                Width = 270,
                AutoSize = true,
                ForeColor = Theme.TextDim,
                BackColor = Color.Transparent,
                Checked = _config.DiscordRpcEnabled,
                Cursor = Cursors.Hand
            };
            _chkDiscordRpc.CheckedChanged += (s, e) =>
            {
                _config.DiscordRpcEnabled = _chkDiscordRpc.Checked;
                _config.Save();
                if (_config.DiscordRpcEnabled) DiscordRpc.Instance.Start();
                else DiscordRpc.Instance.SetPresence("", "");
            };

            // Выбор активного шейдера
            var lblShader = new Label
            {
                Text = "Шейдерпак (Iris):",
                Location = new Point(24, 126),
                Font = Theme.Font(9.5f),
                ForeColor = Theme.Text,
                AutoSize = true
            };

            _cmbShaders = new ComboBox
            {
                Location = new Point(160, 122),
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Theme.Field,
                ForeColor = Theme.Gold,
                Font = Theme.Font(9.5f)
            };
            _cmbShaders.SelectedIndexChanged += (s, e) =>
            {
                if (_cmbShaders.SelectedItem != null)
                {
                    string sel = _cmbShaders.SelectedItem.ToString();
                    ShaderManager.SetActiveShader(_config.GameDir, sel);
                    _config.ActiveShader = sel;
                    _config.Save();
                }
            };

            var btnRepair = new AccentButton
            {
                Text = "🛠️ Починить сборку",
                Location = new Point(24, 166),
                Width = 200,
                Height = 36,
                Font = Theme.Font(9f, FontStyle.Bold)
            };
            btnRepair.SetGoldGlowStyle();
            btnRepair.Click += async (s, e) =>
            {
                if (_busy) return;
                var ans = MessageBox.Show(this,
                    "Лаунчер сбросит временные кэши, перепроверит все моды и восстановит поврежденные файлы.\n\nПродолжить?",
                    "Починка сборки", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ans != DialogResult.Yes) return;

                SetBusy(true);
                _progressBar.Value = 0;
                try
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        var prog = new Progress<ProgressInfo>(p =>
                        {
                            if (p != null)
                            {
                                _progressBar.Value = p.Percent;
                                SetPhase(p.Phase, p.Detail);
                            }
                        });

                        PackUpdater.ResetVerificationCache(_config.GameDir);
                        GameInstaller.ResetInstallMarker(_config.GameDir);
                        SetPhase("Проверка файлов...", "");
                        PackManifest manifest = await _updater.FetchManifestAsync(cts.Token);
                        SyncResult res = await _updater.SyncAsync(manifest, _config.GameDir, prog, cts.Token);
                        AppendLog(string.Format("Починка завершена: проверено {0} файлов, обновлено {1}", res.Total, res.Downloaded));
                        SetPhase("Сборка в порядке!", "");
                        MessageBox.Show(this, "Все файлы успешно проверены и восстановлены!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    AppendLog("Ошибка починки: " + ex.Message);
                    MessageBox.Show(this, "Не удалось выполнить починку: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SetBusy(false);
                }
            };

            var btnOpenGameDir = new AccentButton
            {
                Text = "📂 Папка игры",
                Location = new Point(234, 166),
                Width = 140,
                Height = 36,
                Font = Theme.Font(9f)
            };
            btnOpenGameDir.SetDarkStyle();
            btnOpenGameDir.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_config.GameDir) && Directory.Exists(_config.GameDir))
                    Process.Start("explorer.exe", _config.GameDir);
            };

            var btnBackupWp = new AccentButton
            {
                Text = "💾 Бэкап меток карты",
                Location = new Point(384, 166),
                Width = 190,
                Height = 36,
                Font = Theme.Font(9f)
            };
            btnBackupWp.SetDarkStyle();
            btnBackupWp.Click += (s, e) =>
            {
                WaypointsBackup.CreateBackup(_config.GameDir);
                MessageBox.Show(this, "Резервная копия меток карты Xaero сохранена в backups/waypoints!",
                    "Бэкап меток", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            cardOptions.Controls.AddRange(new Control[] {
                titleOpts, lblSettingsRam, _txtSettingsRam, lblSettingsRamUnit, _sliderSettingsRam,
                _chkJoinServer, _chkDiscordRpc, lblShader, _cmbShaders, btnRepair, btnOpenGameDir, btnBackupWp
            });
            _tabSettings.Controls.Add(cardOptions);

            var spacer2 = new Panel { Dock = DockStyle.Top, Height = 14 };
            _tabSettings.Controls.Add(spacer2);

            // Карточка 3: Системный журнал
            var logCard = new Card
            {
                Dock = DockStyle.Fill,
                FillColor = Theme.Card,
                BorderColor = Theme.BorderSoft,
                Radius = 14f,
                Padding = new Padding(24)
            };

            var headerLog = new Panel { Dock = DockStyle.Top, Height = 30, BackColor = Color.Transparent };
            var lblLogTitle = new Label
            {
                Text = "Системный журнал лаунчера",
                Dock = DockStyle.Left,
                Font = Theme.Font(10.5f, FontStyle.Bold),
                ForeColor = Theme.Muted,
                AutoSize = true
            };
            var btnClearLog = new AccentButton
            {
                Text = "Очистить",
                Dock = DockStyle.Right,
                Width = 90,
                Height = 26,
                Font = Theme.Font(8.5f)
            };
            btnClearLog.SetDarkStyle();
            btnClearLog.Click += (s, e) => { if (_logBox != null) _logBox.Clear(); };
            headerLog.Controls.AddRange(new Control[] { lblLogTitle, btnClearLog });
            logCard.Controls.Add(headerLog);

            _logBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Field,
                ForeColor = Theme.TextDim,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Font = new Font("Consolas", 9f),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            logCard.Controls.Add(_logBox);
            _logBox.BringToFront();

            _tabSettings.Controls.Add(logCard);
            logCard.BringToFront();
        }

        private void SyncSettingsFields()
        {
            if (_txtGameDir != null) _txtGameDir.Text = _config.GameDir ?? "";
            if (_txtJavaPath != null) _txtJavaPath.Text = _config.JavaPath ?? "";
            int curRam = _config.ResolveRamMb();
            if (_txtSettingsRam != null) _txtSettingsRam.Text = string.Format("{0:0.#}", curRam / 1024.0);
            if (_sliderSettingsRam != null) _sliderSettingsRam.Value = curRam;
            if (_chkDiscordRpc != null) _chkDiscordRpc.Checked = _config.DiscordRpcEnabled;
            if (_cmbShaders != null && !string.IsNullOrEmpty(_config.GameDir) && Directory.Exists(_config.GameDir))
            {
                var shaders = ShaderManager.GetInstalledShaders(_config.GameDir);
                _cmbShaders.Items.Clear();
                _cmbShaders.Items.AddRange(shaders.ToArray());
                string curShader = ShaderManager.GetCurrentShader(_config.GameDir);
                int idx = _cmbShaders.Items.IndexOf(curShader);
                _cmbShaders.SelectedIndex = idx >= 0 ? idx : 0;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  ЗАПУСК ИГРЫ И ОБНОВЛЕНИЕ
        // ════════════════════════════════════════════════════════════════════
        private async void StartWork(bool launchAfter)
        {
            if (_busy) return;

            string nick = (_config.Nickname ?? "Trubik").Trim();
            if (nick.Length < 2 || nick.Length > 16)
            {
                OpenProfileModal();
                nick = _config.Nickname;
            }

            if (string.IsNullOrEmpty(_config.GameDir))
            {
                using (var dlg = new FolderBrowserDialog { Description = "Выберите папку для установки игры (.minecraft)" })
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    _config.GameDir = dlg.SelectedPath;
                    _config.Save();
                }
            }

            _cts = new CancellationTokenSource();
            SetBusy(true);
            _progressBar.Value = 0;
            SetPhase("Подготовка к запуску...", "");

            try
            {
                var ct = _cts.Token;
                IProgress<ProgressInfo> progress = new Progress<ProgressInfo>(info =>
                {
                    if (info == null) return;
                    _progressBar.Value = info.Percent;
                    SetPhase(info.Phase, info.Detail);
                });

                // 1. Проверка Java
                string java = await JavaInstaller.EnsureJavaAsync(_config.GameDir, _config.JavaPath, progress, ct);
                _config.JavaPath = java;
                _config.Save();

                // 2. Синхронизация файлов сборки с GitHub
                progress.Report(new ProgressInfo { Phase = "Проверка обновлений сборки...", Fraction = 0, Detail = "" });
                PackManifest manifest = await _updater.FetchManifestAsync(ct);
                AppendLog("Версия сборки: " + manifest.PackVersion + " (" + manifest.Files.Count + " файлов)");

                SyncResult sync = await _updater.SyncAsync(manifest, _config.GameDir, progress, ct);
                AppendLog(string.Format("Обновление: скачано {0} файлов ({1:N1} МБ), удалено {2}",
                    sync.Downloaded, sync.DownloadedBytes / 1024.0 / 1024.0, sync.Deleted));

                // 3. Проверка Minecraft 1.21.1 и NeoForge
                SetPhase("Проверка файлов игры...", "Minecraft " + GameInstaller.McVersion + " • NeoForge " + GameInstaller.NeoForgeVersion);
                string versionName = await GameInstaller.EnsureInstalledAsync(_config.GameDir, java, progress, ct);

                // 4.1 Применяем скин
                SkinStore.ApplyToGame(_config.GameDir, nick);

                // 4.2 Применяем графический пресет (Слабый ПК / Баланс / Ультра)
                GraphicsPresets.Apply(_config.GameDir, _config.GraphicsPreset);

                // 4.3 Резервный бэкап меток карты (Xaero's WorldMap & Waypoints)
                WaypointsBackup.CreateBackup(_config.GameDir);

                if (!launchAfter)
                {
                    SetPhase("Готово", "Все файлы проверены и готовы к игре");
                    _progressBar.Value = 100;
                    return;
                }

                // Discord RPC: В игре
                if (_config.DiscordRpcEnabled)
                {
                    DiscordRpc.Instance.SetInGamePresence(nick);
                }

                // 5. Запуск игры
                SetPhase("Запуск игры...", "");
                Process game = await GameInstaller.LaunchAsync(
                    _config.GameDir, versionName, java, nick,
                    _config.ResolveRamMb(), _server, _config.JoinServer, ct);

                AppendLog("Игра запущена!");
                Hide();
                await Task.Run(() => game.WaitForExit());
                Show();
                Activate();
                AppendLog("Игра закрыта (код " + game.ExitCode + ")");

                // Discord RPC: В лаунчере
                if (_config.DiscordRpcEnabled)
                {
                    DiscordRpc.Instance.SetLauncherPresence();
                }

                // Умный доктор крашей
                if (game.ExitCode != 0)
                {
                    CrashDoctor.CheckAndShowIfCrashed(this, _config.GameDir, game.ExitCode);
                }

                SetPhase("Готов к запуску", "");
                _progressBar.Value = 0;
            }
            catch (OperationCanceledException)
            {
                SetPhase("Отменено", "");
                AppendLog("Операция отменена пользователем");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                SetPhase("Ошибка", ex.Message);
                AppendLog("ОШИБКА: " + ex.Message);
                MessageBox.Show(this, ex.Message + "\n\nПодробности в " + Log.FilePath,
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

        private void SetBusy(bool busy)
        {
            _busy = busy;
            _btnLaunch.Enabled = !busy;
            _btnCancel.Visible = busy;
            _progressBar.Visible = busy;
            _lblPhase.Visible = busy;
            _lblDetail.Visible = busy;
            if (_ramSlider != null) _ramSlider.Enabled = !busy;
            if (_txtRamNumber != null) _txtRamNumber.Enabled = !busy;
        }

        private void SetPhase(string phase, string detail)
        {
            if (phase != null) _lblPhase.Text = phase;
            _lblDetail.Text = detail ?? "";
        }

        private void AppendLog(string message)
        {
            Log.Info(message);
            if (_logBox == null || _logBox.IsDisposed) return;
            try
            {
                _logBox.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message + Environment.NewLine);
                _logBox.SelectionStart = _logBox.TextLength;
                _logBox.ScrollToCaret();
            }
            catch { }
        }

        private async Task InitBackgroundTasksAsync()
        {
            // Инициализация Discord Rich Presence
            if (_config.DiscordRpcEnabled)
            {
                try
                {
                    DiscordRpc.Instance.Start();
                }
                catch { }
            }

            // Получаем данные о сервере в фоне
            try
            {
                string text = await HttpUtil.GetStringAsync(
                    "https://raw.githubusercontent.com/Trubik1/minecraft/main/server.txt",
                    CancellationToken.None, 2).ConfigureAwait(true);
                _server = ServerInfo.Parse(text);
            }
            catch { }

            // Проверка обновления лаунчера
            try
            {
                UpdateInfo update = await SelfUpdater.CheckAsync(CancellationToken.None).ConfigureAwait(true);
                if (update != null)
                {
                    var answer = MessageBox.Show(this,
                        "Доступна новая версия лаунчера (" + update.Version + ").\nОбновить сейчас?",
                        "Steampunk & Swords", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (answer == DialogResult.Yes)
                    {
                        SetBusy(true);
                        using (var cts = new CancellationTokenSource())
                        {
                            var prog = new Progress<ProgressInfo>(p => { if (p != null) SetPhase(p.Phase, p.Detail); });
                            await SelfUpdater.ApplyAsync(update, prog, cts.Token).ConfigureAwait(true);
                        }
                        Application.Exit();
                    }
                }
            }
            catch { }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try { DiscordRpc.Instance.Dispose(); } catch { }
            try { _gearAnimTimer?.Stop(); _gearAnimTimer?.Dispose(); } catch { }
            base.OnFormClosing(e);
        }
    }
}
