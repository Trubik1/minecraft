using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    public class ChangelogDialog : Form
    {
        private RichTextBox _txtContent;
        private Label _lblStatus;

        public ChangelogDialog()
        {
            BuildUi();
            LoadChangelogAsync();
        }

        private void BuildUi()
        {
            Text = "Что нового в Steampunk & Swords";
            Size = new Size(680, 540);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Theme.Bg;
            ForeColor = Theme.Text;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Theme.Panel
            };
            header.Paint += (s, e) =>
            {
                using (var pen = new Pen(Theme.Border, 1))
                    e.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };

            var lblTitle = new Label
            {
                Text = "📜 Список изменений и патчноуты сборки",
                Font = Theme.Display(12f, FontStyle.Bold),
                ForeColor = Theme.Gold,
                Location = new Point(20, 14),
                AutoSize = true
            };

            _lblStatus = new Label
            {
                Text = "Загрузка актуального списка изменений...",
                Font = Theme.Font(8.5f),
                ForeColor = Theme.TextDim,
                Location = new Point(22, 38),
                AutoSize = true
            };
            header.Controls.AddRange(new Control[] { lblTitle, _lblStatus });

            var bottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = Theme.Panel
            };
            var btnClose = new AccentButton
            {
                Text = "Отлично!",
                Size = new Size(130, 36),
                Location = new Point(Width - 160, 10),
                Font = Theme.Font(9.5f, FontStyle.Bold),
                IsPrimary = true
            };
            btnClose.SetGoldGlowStyle();
            btnClose.Click += (s, e) => Close();
            bottom.Controls.Add(btnClose);

            _txtContent = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Bg,
                ForeColor = Theme.Text,
                Font = Theme.Font(10f),
                BorderStyle = BorderStyle.None,
                ReadOnly = true
            };

            Controls.Add(_txtContent);
            Controls.Add(header);
            Controls.Add(bottom);
        }

        private async void LoadChangelogAsync()
        {
            string onlineChangelog = null;
            try
            {
                onlineChangelog = await HttpUtil.GetStringAsync(
                    "https://raw.githubusercontent.com/Trubik1/minecraft/main/changelog.txt",
                    CancellationToken.None, 1).ConfigureAwait(true);
            }
            catch { }

            if (!string.IsNullOrWhiteSpace(onlineChangelog))
            {
                _txtContent.Text = "\n" + onlineChangelog;
                _lblStatus.Text = "Загружено актуальное обновление с сервера";
                _lblStatus.ForeColor = Color.FromArgb(90, 210, 130);
            }
            else
            {
                _txtContent.Text = GetDefaultChangelog();
                _lblStatus.Text = "Официальная версия Steampunk & Swords 1.0.0 (NeoForge 1.21.1)";
                _lblStatus.ForeColor = Theme.TextDim;
            }
        }

        private static string GetDefaultChangelog()
        {
            return @"
═══════════════════════════════════════════════════════════════════════
        ⚙️ STEAMPUNK & SWORDS — Версия 1.0.0 (Minecraft 1.21.1)
═══════════════════════════════════════════════════════════════════════

🔥 ГЛАВНЫЕ НОВОВВЕДЕНИЯ:

  ⚙️ Механика и Автоматизация (Create & Addons):
    • Create 6.0 — кинетическая энергия, конвейеры, поезда и ветряки.
    • Create Big Cannons — артиллерия, пушки и баллистика.
    • Create New Age — электричество, генераторы, вращающиеся магниты.
    • Create Aeronautics — дирижабли и летающие корабли.
    • Refined Storage & Powah — беспроводные цифровые сети хранения и реакторы.

  ⚔️ Боевая система и RPG:
    • Epic Fight — боевые стойки, увороты, комбо и спец-удары оружием.
    • L_Ender's Cataclysm — уникальные боссы в древних подземельях.
    • Weapons of Miracles & Artifacts — легендарное оружие и артефакты.
    • Puffish Skills — прокачиваемое древо талантов персонажа.

  🌌 Исследования и Миры:
    • The Aether — парящие острова в небесах.
    • Twilight Forest — Сумеречный лес с боссами и лабиринтами.
    • Deeper and Darker — измерения древнего города скалка.
    • The Undergarden — темный подземный мир с грибами и реликвиями.
    • Структуры YUNG's — обновленные подземелья, шахты, крепости и храмы.

  📜 Квесты и Баланс:
    • FTB Quests — сотни подробных квестов на русском языке.
    • Плавная прогрессия от простых паровых машин до ядерной энергии.

  🚀 Лаунчер 2.0:
    • Стильный интерфейс LoliLand / Modrinth с плавной анимацией шестеренок.
    • Пресеты графики: «Слабый ПК», «Баланс», «Ультра» в 1 клик.
    • Интерактивная 3D-примерочная скинов и плащей с вращением мышью.
    • Встроенная галерея скриншотов с избранным.
    • «Умный доктор» — мгновенный анализ причин вылетов и помощь.
    • Discord Rich Presence — отображение статуса игры у друзей.
    • Автоматический бэкап меток карты и точек дома.
";
        }
    }
}
