using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    public class ProfileDialog : Form
    {
        private readonly LauncherConfig _config;
        private TextBox _txtNick;
        private PictureBox _picAvatar;
        private Label _lblStatus;

        public bool NicknameChanged { get; private set; }

        public ProfileDialog(LauncherConfig config)
        {
            _config = config;

            Text = "Профиль игрока";
            ClientSize = new Size(440, 360);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.Bg;
            ForeColor = Theme.Text;
            Font = Theme.Font(9.5f);

            BuildUi();
            LoadAvatar();
        }

        private void BuildUi()
        {
            var card = new Card
            {
                Location = new Point(20, 20),
                Size = new Size(400, 320),
                FillColor = Theme.Card,
                BorderColor = Theme.BorderSoft,
                Radius = 14f
            };

            var title = new Label
            {
                Text = "Настройки профиля",
                Font = Theme.Display(13f, FontStyle.Bold),
                ForeColor = Theme.Gold,
                Location = new Point(20, 16),
                AutoSize = true
            };

            // Аватарка головы
            _picAvatar = new PictureBox
            {
                Location = new Point(20, 50),
                Size = new Size(64, 64),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Theme.Field
            };

            var lblNickHint = new Label
            {
                Text = "Никнейм игрока (3–16 символов)",
                Location = new Point(100, 50),
                Font = Theme.Font(9f),
                ForeColor = Theme.TextDim,
                AutoSize = true
            };

            _txtNick = new TextBox
            {
                Text = _config.Nickname ?? "Trubik",
                Location = new Point(100, 74),
                Width = 270,
                Height = 32,
                Font = Theme.Font(11f, FontStyle.Bold),
                BackColor = Theme.Field,
                ForeColor = Theme.Text,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblDesc = new Label
            {
                Text = "Этот ник используется для входа на сервер и в игре.\nСкин привязывается к вашему никнейму.",
                Location = new Point(20, 130),
                Width = 360,
                Height = 40,
                Font = Theme.Font(8.5f),
                ForeColor = Theme.Muted
            };

            var btnUploadSkin = new AccentButton
            {
                Text = "📁 Загрузить скин (PNG)",
                Location = new Point(20, 180),
                Width = 175,
                Height = 36,
                Font = Theme.Font(8.5f)
            };
            btnUploadSkin.SetDarkStyle();
            btnUploadSkin.Click += (s, e) =>
            {
                using (var dlg = new OpenFileDialog { Filter = "Скины Minecraft (*.png)|*.png" })
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        string err = SkinStore.Import(dlg.FileName);
                        if (err == null)
                        {
                            SkinStore.ApplyToGame(_config.GameDir, _config.Nickname);
                            LoadAvatar();
                            SetStatus("Скин успешно обновлен!", false);
                        }
                        else
                        {
                            SetStatus(err, true);
                        }
                    }
                }
            };

            var btnResetSkin = new AccentButton
            {
                Text = "Сбросить скин",
                Location = new Point(205, 180),
                Width = 175,
                Height = 36,
                Font = Theme.Font(8.5f)
            };
            btnResetSkin.SetDarkStyle();
            btnResetSkin.Click += (s, e) =>
            {
                SkinStore.Reset(_config.GameDir, _config.Nickname);
                LoadAvatar();
                SetStatus("Скин сброшен к стандартному", false);
            };

            _lblStatus = new Label
            {
                Text = "",
                Location = new Point(20, 226),
                Width = 360,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = Theme.Font(8.5f)
            };

            var btnSave = new AccentButton
            {
                Text = "Сохранить",
                Location = new Point(20, 260),
                Width = 175,
                Height = 40,
                Font = Theme.Font(10f, FontStyle.Bold)
            };
            btnSave.SetGreenStyle();
            btnSave.Click += (s, e) =>
            {
                string nick = _txtNick.Text.Trim();
                if (nick.Length < 3 || nick.Length > 16 || !SkinStore.IsValidNick(nick))
                {
                    MessageBox.Show(this, "Никнейм должен состоять из 3–16 латинских букв, цифр или знака _",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _config.Nickname = nick;
                _config.Save();
                SkinStore.ApplyToGame(_config.GameDir, nick);
                NicknameChanged = true;
                DialogResult = DialogResult.OK;
                Close();
            };

            var btnCancel = new AccentButton
            {
                Text = "Отмена",
                Location = new Point(205, 260),
                Width = 175,
                Height = 40,
                Font = Theme.Font(10f)
            };
            btnCancel.SetDarkStyle();
            btnCancel.Click += (s, e) => Close();

            card.Controls.AddRange(new Control[] {
                title, _picAvatar, lblNickHint, _txtNick, lblDesc,
                btnUploadSkin, btnResetSkin, _lblStatus, btnSave, btnCancel
            });
            Controls.Add(card);
        }

        private void SetStatus(string msg, bool error)
        {
            _lblStatus.Text = msg;
            _lblStatus.ForeColor = error ? Theme.Red : Theme.Gold;
        }

        private void LoadAvatar()
        {
            try
            {
                var skin = SkinStore.Load();
                if (skin != null)
                {
                    // вырезаем голову из скина (координаты лица 8,8, 8x8)
                    var head = new Bitmap(64, 64);
                    using (var g = Graphics.FromImage(head))
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.PixelOffsetMode = PixelOffsetMode.Half;
                        // лицо
                        g.DrawImage(skin, new Rectangle(0, 0, 64, 64), 8, 8, 8, 8, GraphicsUnit.Pixel);
                        // второй слой (шлем 40,8, 8x8)
                        g.DrawImage(skin, new Rectangle(0, 0, 64, 64), 40, 8, 8, 8, GraphicsUnit.Pixel);
                    }
                    _picAvatar.Image = head;
                }
                else
                {
                    _picAvatar.Image = null;
                }
            }
            catch { }
        }
    }
}
