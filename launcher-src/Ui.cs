using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    public static class Theme
    {
        // ─── Основная палитра (Modrinth Slate + Steampunk Brass) ──────
        public static readonly Color Bg          = Color.FromArgb(16, 19, 24);
        public static readonly Color Background  = Color.FromArgb(16, 19, 24);
        public static readonly Color Sidebar     = Color.FromArgb(21, 25, 32);
        public static readonly Color Panel       = Color.FromArgb(24, 29, 38);
        public static readonly Color PanelLight  = Color.FromArgb(32, 38, 50);
        public static readonly Color Card        = Color.FromArgb(22, 27, 36);
        public static readonly Color CardAlt     = Color.FromArgb(18, 22, 30);
        public static readonly Color Field       = Color.FromArgb(15, 18, 24);

        public static readonly Color Text        = Color.FromArgb(235, 238, 245);
        public static readonly Color TextDim     = Color.FromArgb(160, 170, 185);
        public static readonly Color Muted       = Color.FromArgb(115, 128, 148);
        public static readonly Color Faint       = Color.FromArgb(75, 85, 102);

        public static readonly Color Border      = Color.FromArgb(46, 54, 70);
        public static readonly Color BorderSoft  = Color.FromArgb(34, 40, 52);

        // Burnished Brass & Gold
        public static readonly Color Gold        = Color.FromArgb(229, 184, 68);
        public static readonly Color GoldHover   = Color.FromArgb(245, 204, 90);
        public static readonly Color GoldDark    = Color.FromArgb(186, 142, 36);
        public static readonly Color GoldSoft    = Color.FromArgb(175, 140, 52);
        public static readonly Color GoldText    = Color.FromArgb(18, 14, 8); // Высококонтрастный текст на латуни

        public static readonly Color Green       = Color.FromArgb(35, 185, 95);
        public static readonly Color GreenHover  = Color.FromArgb(45, 210, 110);
        public static readonly Color GreenDark   = Color.FromArgb(25, 145, 75);
        public static readonly Color Red         = Color.FromArgb(215, 75, 75);
        public static readonly Color RedHover    = Color.FromArgb(235, 90, 90);

        public static Font Font(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font("Segoe UI", size, style, GraphicsUnit.Point);
        }

        public static Font Display(float size, FontStyle style = FontStyle.Bold)
        {
            return new Font("Segoe UI", size, style, GraphicsUnit.Point);
        }

        public static void PaintControlBackground(Graphics g, Control ctrl)
        {
            g.Clear(ctrl.BackColor);
        }

        public static void PaintBackdrop(Graphics g, Rectangle rect, int xOffset = 0, int yOffset = 0)
        {
            using (var brush = new SolidBrush(Bg))
            {
                g.FillRectangle(brush, rect);
            }
        }

        public static void PaintText(Graphics g, string text, Font font, Color color, RectangleF rect,
            StringAlignment align = StringAlignment.Near, StringAlignment lineAlign = StringAlignment.Center)
        {
            using (var brush = new SolidBrush(color))
            using (var format = new StringFormat
            {
                Alignment = align,
                LineAlignment = lineAlign,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                g.DrawString(text, font, brush, rect, format);
            }
        }

        public static void PaintText(Graphics g, string text, Font font, Color color, RectangleF rect, ContentAlignment align)
        {
            StringAlignment sa = StringAlignment.Near;
            StringAlignment lsa = StringAlignment.Center;
            switch (align)
            {
                case ContentAlignment.TopLeft: sa = StringAlignment.Near; lsa = StringAlignment.Near; break;
                case ContentAlignment.TopCenter: sa = StringAlignment.Center; lsa = StringAlignment.Near; break;
                case ContentAlignment.TopRight: sa = StringAlignment.Far; lsa = StringAlignment.Near; break;
                case ContentAlignment.MiddleLeft: sa = StringAlignment.Near; lsa = StringAlignment.Center; break;
                case ContentAlignment.MiddleCenter: sa = StringAlignment.Center; lsa = StringAlignment.Center; break;
                case ContentAlignment.MiddleRight: sa = StringAlignment.Far; lsa = StringAlignment.Center; break;
                case ContentAlignment.BottomLeft: sa = StringAlignment.Near; lsa = StringAlignment.Far; break;
                case ContentAlignment.BottomCenter: sa = StringAlignment.Center; lsa = StringAlignment.Far; break;
                case ContentAlignment.BottomRight: sa = StringAlignment.Far; lsa = StringAlignment.Far; break;
            }
            PaintText(g, text, font, color, rect, sa, lsa);
        }

        public static GraphicsPath RoundRect(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 2f || rect.Height <= 2f || radius <= 1f)
            {
                if (rect.Width > 0f && rect.Height > 0f)
                    path.AddRectangle(rect);
                return path;
            }

            float d = radius * 2f;
            if (d > rect.Width) d = rect.Width;
            if (d > rect.Height) d = rect.Height;

            if (d <= 1f)
            {
                if (rect.Width > 0f && rect.Height > 0f)
                    path.AddRectangle(rect);
                return path;
            }

            try
            {
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
            }
            catch
            {
                path.Reset();
                path.AddRectangle(rect);
            }
            return path;
        }

        public static Button FlatButton(string text, int width, int height, Color back, Color hover)
        {
            var btn = new Button
            {
                Text = text,
                Width = width,
                Height = height,
                FlatStyle = FlatStyle.Flat,
                BackColor = back,
                ForeColor = Color.White,
                Font = Font(10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = hover;
            btn.FlatAppearance.MouseDownBackColor = back;
            return btn;
        }
    }

    /// <summary>Скругленная карточка-контейнер</summary>
    public class Card : Panel
    {
        public float Radius { get; set; } = 12f;
        public Color FillColor { get; set; } = Theme.Card;
        public Color BorderColor { get; set; } = Theme.BorderSoft;
        public bool DrawBorder { get; set; } = true;

        public Card()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 2 || Height <= 2) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using (var path = Theme.RoundRect(rect, Radius))
            {
                using (var brush = new SolidBrush(FillColor))
                    g.FillPath(brush, path);

                if (DrawBorder)
                {
                    using (var pen = new Pen(BorderColor, 1f))
                        g.DrawPath(pen, path);
                }
            }

            base.OnPaint(e);
        }
    }

    /// <summary>
    /// Стилизованная интерактивная кнопка.
    /// Поддерживает тактильный латунный финиш без Web 2.0 бликов.
    /// </summary>
    public class AccentButton : Button
    {
        public float Radius { get; set; } = 10f;
        public Color NormalColor { get; set; } = Theme.PanelLight;
        public Color HoverColor { get; set; } = Theme.Border;
        public Color PressedColor { get; set; } = Theme.Panel;
        public Color OutlineColor { get; set; } = Color.Transparent;
        public bool IsPrimary { get; set; }
        public bool IsGoldGlow { get; set; }

        private bool _isHovered;
        private bool _isPressed;

        public AccentButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            ForeColor = Theme.Text;
            Font = Theme.Font(9.5f, FontStyle.Bold);
            Cursor = Cursors.Hand;
            Height = 36;
            Width = 120;
        }

        public void UpdateRegion()
        {
            // Без HWND Region: устраняет лестничные черные углы и мерцание
            this.Region = null;
        }

        public void SetGreenStyle()
        {
            NormalColor = Theme.Green;
            HoverColor = Theme.GreenHover;
            PressedColor = Theme.GreenDark;
            ForeColor = Color.White;
            IsPrimary = true;
            IsGoldGlow = false;
            Invalidate();
        }

        public void SetRedStyle()
        {
            NormalColor = Theme.Red;
            HoverColor = Theme.RedHover;
            PressedColor = Color.FromArgb(180, 50, 50);
            ForeColor = Color.White;
            IsGoldGlow = false;
            Invalidate();
        }

        public void SetDarkStyle()
        {
            NormalColor = Theme.Card;
            HoverColor = Theme.PanelLight;
            PressedColor = Theme.CardAlt;
            OutlineColor = Theme.BorderSoft;
            ForeColor = Theme.Text;
            IsGoldGlow = false;
            Invalidate();
        }

        public void SetGoldGlowStyle()
        {
            IsGoldGlow = true;
            OutlineColor = Theme.Gold;
            ForeColor = Theme.GoldText;
            Radius = 10f;
            IsPrimary = true;
            Invalidate();
        }

        private bool IsInsidePill(Point pt)
        {
            float r = Radius > 0 ? Radius : 10f;
            using (var path = Theme.RoundRect(new RectangleF(0, 0, Width, Height), r))
            {
                return path.IsVisible(pt);
            }
        }

        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            bool inside = IsInsidePill(mevent.Location);
            if (inside != _isHovered)
            {
                _isHovered = inside;
                Invalidate();
            }
            base.OnMouseMove(mevent);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left && IsInsidePill(mevent.Location))
            {
                _isPressed = true;
                Invalidate();
                base.OnMouseDown(mevent);
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(mevent);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (Width <= 2 || Height <= 2) return;

            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float r = Radius > 0 ? Radius : 10f;
            var rect = new RectangleF(1f, 1f, Width - 2f, Height - 2f);

            if (IsGoldGlow)
            {
                // Тактильный латунный финиш: чистый градиент шлифованного металла
                Color topC = _isPressed ? Color.FromArgb(195, 150, 42) : (_isHovered ? Color.FromArgb(248, 208, 92) : Color.FromArgb(232, 186, 68));
                Color botC = _isPressed ? Color.FromArgb(155, 114, 24) : (_isHovered ? Color.FromArgb(218, 164, 46) : Color.FromArgb(198, 148, 40));

                using (var path = Theme.RoundRect(rect, r))
                {
                    using (var brush = new LinearGradientBrush(rect, topC, botC, LinearGradientMode.Vertical))
                        g.FillPath(brush, path);

                    Color borderC = _isPressed ? Color.FromArgb(140, 100, 20) : (_isHovered ? Color.FromArgb(255, 230, 130) : Color.FromArgb(245, 200, 85));
                    using (var pen = new Pen(borderC, 1.2f))
                        g.DrawPath(pen, path);

                    // Тонкая линия фаски 1px сверху (эффект металлической грани)
                    if (!_isPressed)
                    {
                        using (var penHl = new Pen(Color.FromArgb(90, 255, 255, 255), 1f))
                        {
                            g.DrawLine(penHl, rect.X + r, rect.Y + 1f, rect.Right - r, rect.Y + 1f);
                        }
                    }
                }
            }
            else
            {
                Color fill = _isPressed ? PressedColor : (_isHovered ? HoverColor : NormalColor);
                using (var path = Theme.RoundRect(rect, r))
                {
                    using (var brush = new SolidBrush(fill))
                        g.FillPath(brush, path);

                    if (OutlineColor != Color.Transparent)
                    {
                        using (var pen = new Pen(OutlineColor, 1.2f))
                            g.DrawPath(pen, path);
                    }
                    else if (IsPrimary && !_isPressed)
                    {
                        using (var pen = new Pen(Color.FromArgb(60, 255, 255, 255), 1f))
                            g.DrawPath(pen, path);
                    }
                }
            }

            // Текст кнопки (на золотой кнопке — контрастный обсидиан GoldText)
            Color txtColor = IsGoldGlow ? Theme.GoldText : ForeColor;
            Theme.PaintText(g, Text, Font, txtColor, new RectangleF(0, 0, Width, Height),
                StringAlignment.Center, StringAlignment.Center);
        }
    }

    /// <summary>Стилизованное поле ввода</summary>
    public class FieldBox : TextBox
    {
        public FieldBox()
        {
            BorderStyle = BorderStyle.FixedSingle;
            BackColor = Theme.Field;
            ForeColor = Theme.Text;
            Font = Theme.Font(10.5f);
        }
    }

    /// <summary>Градиентный прогресс-бар</summary>
    public class FlatProgressBar : Control
    {
        private int _value;

        public FlatProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            Height = 6;
            BackColor = Color.Transparent;
        }

        public int Value
        {
            get { return _value; }
            set
            {
                int clamped = value < 0 ? 0 : (value > 100 ? 100 : value);
                if (clamped == _value) return;
                _value = clamped;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var track = new RectangleF(0, 0, Width, Height);
            using (var back = new SolidBrush(Color.FromArgb(20, 24, 32)))
            using (var path = Theme.RoundRect(track, Height / 2f))
                g.FillPath(back, path);

            if (_value > 0)
            {
                float fillWidth = Math.Max(Height, Width * (_value / 100.0f));
                var fill = new RectangleF(0, 0, fillWidth, Height);
                using (var brush = new LinearGradientBrush(fill, Theme.Gold, Theme.Green, LinearGradientMode.Horizontal))
                using (var path = Theme.RoundRect(fill, Height / 2f))
                    g.FillPath(brush, path);
            }
        }
    }

    /// <summary>
    /// Капсульная кнопка вкладки для верхней шапки.
    /// Полностью устраняет мерцание, дёргание мыши и лестничные края (без Win32 Region).
    /// </summary>
    public class NavPillButton : Control
    {
        private bool _active;
        private bool _hover;

        public bool Active
        {
            get { return _active; }
            set { if (_active != value) { _active = value; Invalidate(); } }
        }

        public NavPillButton(string text)
        {
            Text = text;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = Theme.Font(9.5f, FontStyle.Bold);
            Cursor = Cursors.Hand;
            Size = new Size(120, 38);
        }

        public void UpdateRegion()
        {
            // Без Region: WinForms не будет пересоздавать клиппинг окна при ховере
            this.Region = null;
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 2 || Height <= 2) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var rect = new RectangleF(1f, 1f, Width - 2f, Height - 2f);
            float radius = Height / 2f;

            if (_active)
            {
                using (var path = Theme.RoundRect(rect, radius))
                {
                    using (var brush = new LinearGradientBrush(rect, Color.FromArgb(62, 74, 96), Color.FromArgb(46, 56, 74), LinearGradientMode.Vertical))
                        g.FillPath(brush, path);
                    using (var pen = new Pen(Color.FromArgb(95, 115, 148), 1.2f))
                        g.DrawPath(pen, path);
                }
            }
            else if (_hover)
            {
                using (var path = Theme.RoundRect(rect, radius))
                using (var brush = new SolidBrush(Color.FromArgb(30, 255, 255, 255)))
                    g.FillPath(brush, path);
            }

            Color textColor = _active ? Color.White : (_hover ? Color.FromArgb(240, 245, 255) : Color.FromArgb(160, 172, 192));
            Theme.PaintText(g, Text, Font, textColor, new RectangleF(0, 0, Width, Height),
                StringAlignment.Center, StringAlignment.Center);
        }
    }

    /// <summary>
    /// Трёхпозиционный сегментированный переключатель (Segmented Control).
    /// Заменяет разрозненные кнопки с эмодзи на профессиональный переключатель пресетов.
    /// </summary>
    public class PresetSwitch : Control
    {
        public string[] Items { get; } = new string[] { "Быстрая", "Баланс", "Ультра" };
        private int _selectedIndex = 1;
        private int _hoverIndex = -1;

        public event EventHandler SelectedIndexChanged;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                int clamped = Math.Max(0, Math.Min(Items.Length - 1, value));
                if (_selectedIndex != clamped)
                {
                    _selectedIndex = clamped;
                    Invalidate();
                    SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public PresetSwitch()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = Theme.Font(8.8f, FontStyle.Bold);
            Cursor = Cursors.Hand;
            Size = new Size(310, 30);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            float itemW = (Width - 4f) / Items.Length;
            int idx = (int)((e.X - 2f) / itemW);
            if (idx < 0) idx = 0;
            if (idx >= Items.Length) idx = Items.Length - 1;

            if (_hoverIndex != idx)
            {
                _hoverIndex = idx;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverIndex = -1;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                float itemW = (Width - 4f) / Items.Length;
                int idx = (int)((e.X - 2f) / itemW);
                if (idx >= 0 && idx < Items.Length)
                {
                    SelectedIndex = idx;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 4 || Height <= 4) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Контейнер трека
            var trackRect = new RectangleF(1f, 1f, Width - 2f, Height - 2f);
            using (var pathTrack = Theme.RoundRect(trackRect, 6f))
            {
                using (var b = new SolidBrush(Color.FromArgb(18, 22, 30)))
                    g.FillPath(b, pathTrack);
                using (var p = new Pen(Color.FromArgb(40, 48, 64), 1f))
                    g.DrawPath(p, pathTrack);
            }

            float itemW = (Width - 6f) / Items.Length;

            for (int i = 0; i < Items.Length; i++)
            {
                var itemRect = new RectangleF(3f + (i * itemW), 3f, itemW, Height - 6f);
                bool isSelected = (i == _selectedIndex);
                bool isHovered = (i == _hoverIndex && !isSelected);

                if (isSelected)
                {
                    using (var pathSel = Theme.RoundRect(itemRect, 5f))
                    {
                        using (var brushSel = new LinearGradientBrush(itemRect, Color.FromArgb(48, 58, 78), Color.FromArgb(36, 44, 60), LinearGradientMode.Vertical))
                            g.FillPath(brushSel, pathSel);
                        using (var penSel = new Pen(Color.FromArgb(80, 96, 126), 1f))
                            g.DrawPath(penSel, pathSel);
                    }
                }
                else if (isHovered)
                {
                    using (var pathHov = Theme.RoundRect(itemRect, 5f))
                    using (var brushHov = new SolidBrush(Color.FromArgb(20, 255, 255, 255)))
                        g.FillPath(brushHov, pathHov);
                }

                Color textColor = isSelected ? Color.White : (isHovered ? Color.FromArgb(220, 228, 240) : Color.FromArgb(140, 150, 168));
                Theme.PaintText(g, Items[i], Font, textColor, itemRect, StringAlignment.Center, StringAlignment.Center);
            }
        }
    }

    /// <summary>
    /// Кнопка с векторной иконкой в шапке (Патчноут, Discord, Wiki).
    /// Заменяет эмодзи на чистые, выверенные векторные пиктограммы.
    /// </summary>
    public enum HeaderIconKind
    {
        Changelog,
        Discord,
        Wiki
    }

    public class HeaderIconButton : Control
    {
        private readonly HeaderIconKind _kind;
        private readonly ToolTip _tip;
        private bool _isHovered;
        private bool _isPressed;

        public HeaderIconButton(HeaderIconKind kind, string tooltipText)
        {
            _kind = kind;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
            BackColor = Color.Transparent;
            Size = new Size(36, 36);
            Cursor = Cursors.Hand;

            _tip = new ToolTip();
            if (!string.IsNullOrEmpty(tooltipText))
                _tip.SetToolTip(this, tooltipText);
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; _isPressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { _isPressed = true; Invalidate(); } base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new RectangleF(1f, 1f, Width - 2f, Height - 2f);
            float r = 10f;

            Color bg = _isPressed ? Color.FromArgb(20, 24, 34) : (_isHovered ? Color.FromArgb(42, 50, 68) : Color.FromArgb(24, 28, 38));
            Color border = _isHovered ? Theme.Gold : Theme.BorderSoft;

            using (var path = Theme.RoundRect(rect, r))
            {
                using (var b = new SolidBrush(bg)) g.FillPath(b, path);
                using (var p = new Pen(border, 1f)) g.DrawPath(p, path);
            }

            Color iconColor = _isHovered ? Theme.GoldHover : Theme.Gold;
            using (var pen = new Pen(iconColor, 1.6f))
            using (var brush = new SolidBrush(iconColor))
            {
                float cx = Width / 2f;
                float cy = Height / 2f;

                if (_kind == HeaderIconKind.Changelog)
                {
                    // Векторный свиток/документ с аккуратными строками
                    float x = cx - 7f, y = cy - 9f, w = 14f, h = 18f;
                    g.DrawRectangle(pen, x, y, w, h);
                    g.DrawLine(pen, x + 3f, y + 4.5f, x + w - 3f, y + 4.5f);
                    g.DrawLine(pen, x + 3f, y + 8.5f, x + w - 3f, y + 8.5f);
                    g.DrawLine(pen, x + 3f, y + 12.5f, x + w - 5f, y + 12.5f);
                }
                else if (_kind == HeaderIconKind.Discord)
                {
                    // Векторное облачко чата
                    var bubble = new RectangleF(cx - 9f, cy - 8f, 18f, 14f);
                    using (var pBubble = Theme.RoundRect(bubble, 4f))
                        g.DrawPath(pen, pBubble);
                    g.FillEllipse(brush, cx - 4.5f, cy - 2.5f, 2.5f, 2.5f);
                    g.FillEllipse(brush, cx + 2f, cy - 2.5f, 2.5f, 2.5f);
                    g.DrawLine(pen, cx - 5f, cy + 6f, cx - 8f, cy + 9.5f);
                }
                else if (_kind == HeaderIconKind.Wiki)
                {
                    // Векторная сфера/глобус знаний
                    g.DrawEllipse(pen, cx - 8f, cy - 8f, 16f, 16f);
                    g.DrawLine(pen, cx - 8f, cy, cx + 8f, cy);
                    g.DrawEllipse(pen, cx - 4f, cy - 8f, 8f, 16f);
                }
            }
        }
    }

    /// <summary>
    /// Единая нижняя панель управления (Command Deck).
    /// Объединяет блок RAM, выбор графики и кнопку пуска в один монолитный док.
    /// </summary>
    public class CommandDeck : Panel
    {
        public float Radius { get; set; } = 14f;
        public Color FillColor { get; set; } = Color.FromArgb(235, 17, 21, 29);
        public Color BorderColor { get; set; } = Color.FromArgb(46, 56, 74);

        public int Divider1X { get; set; } = -1;
        public int Divider2X { get; set; } = -1;

        public CommandDeck()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 2 || Height <= 2) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new RectangleF(1f, 1f, Width - 2f, Height - 2f);
            using (var path = Theme.RoundRect(rect, Radius))
            {
                using (var brush = new SolidBrush(FillColor))
                    g.FillPath(brush, path);

                using (var pen = new Pen(BorderColor, 1.2f))
                    g.DrawPath(pen, path);

                // Верхний тонкий блик грани
                using (var penHl = new Pen(Color.FromArgb(45, 255, 255, 255), 1f))
                {
                    g.DrawLine(penHl, rect.X + Radius, rect.Y + 1f, rect.Right - Radius, rect.Y + 1f);
                }
            }

            // Тонкие вертикальные разделители между функциональными зонами
            using (var penDiv = new Pen(Color.FromArgb(38, 46, 62), 1f))
            {
                if (Divider1X > 0 && Divider1X < Width)
                    g.DrawLine(penDiv, Divider1X, 16f, Divider1X, Height - 16f);
                if (Divider2X > 0 && Divider2X < Width)
                    g.DrawLine(penDiv, Divider2X, 16f, Divider2X, Height - 16f);
            }

            base.OnPaint(e);
        }
    }

    /// <summary>
    /// Блок заголовка и технических бейджей (STEAMPUNK & SWORDS, 1.21.1, NeoForge, Iris).
    /// Заменяет аляповатый рекламный слоган на благородный бренд-блок.
    /// </summary>
    public class HeroPanel : Panel
    {
        public HeroPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 2 || Height <= 2) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // 1. Заголовок STEAMPUNK & SWORDS
            using (var fTitle = Theme.Display(23f, FontStyle.Bold))
            using (var bWhite = new SolidBrush(Color.White))
            using (var bGold = new SolidBrush(Theme.Gold))
            {
                g.DrawString("STEAMPUNK", fTitle, bWhite, 0, 0);
                var size = g.MeasureString("STEAMPUNK ", fTitle);
                g.DrawString("& SWORDS", fTitle, bGold, size.Width - 4, 0);
            }

            // 2. Технические бейджи
            string[] badges = new string[] { "1.21.1", "NEOFORGE 21.1", "119 МОДОВ", "IRIS SHADERS" };
            float badgeX = 0f;
            float badgeY = 46f;
            float badgeH = 22f;

            using (var fBadge = Theme.Font(8.2f, FontStyle.Bold))
            using (var penBadge = new Pen(Color.FromArgb(48, 58, 76), 1f))
            using (var brushBadgeBg = new SolidBrush(Color.FromArgb(200, 16, 20, 28)))
            using (var brushBadgeText = new SolidBrush(Color.FromArgb(215, 222, 235)))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                foreach (var b in badges)
                {
                    var txtSize = g.MeasureString(b, fBadge);
                    float bw = txtSize.Width + 14f;
                    var bRect = new RectangleF(badgeX, badgeY, bw, badgeH);
                    using (var path = Theme.RoundRect(bRect, 4f))
                    {
                        g.FillPath(brushBadgeBg, path);
                        g.DrawPath(penBadge, path);
                    }
                    g.DrawString(b, fBadge, brushBadgeText, bRect, sf);
                    badgeX += bw + 8f;
                }
            }

            // 3. Лаконичный технический подзаголовок
            using (var fSub = Theme.Font(8.8f))
            using (var bSub = new SolidBrush(Color.FromArgb(150, 162, 182)))
            {
                g.DrawString("Официальная сборка • Создано для стабильной игры с друзьями", fSub, bSub, 0, 78);
            }
        }
    }

    /// <summary>
    /// Double-buffered background canvas with smooth wallpaper scaling, seamless vignette,
    /// and correct OnPaintBackground dispatch for 100% transparent child controls.
    /// </summary>
    public class GameCanvas : Panel
    {
        private Image _bgImage;

        public GameCanvas()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();
            BackColor = Color.FromArgb(14, 16, 22);
        }

        public void SetWallpaper(Image img)
        {
            _bgImage = img;
            Invalidate();
        }

        public void DrawBackdrop(Graphics g, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            if (_bgImage != null)
            {
                float screenRatio = (float)Width / Math.Max(1, Height);
                float imgRatio = (float)_bgImage.Width / Math.Max(1, _bgImage.Height);

                float drawW, drawH, drawX = 0, drawY = 0;
                if (screenRatio > imgRatio)
                {
                    drawW = Width;
                    drawH = Width / imgRatio;
                    drawY = (Height - drawH) / 2f;
                }
                else
                {
                    drawH = Height;
                    drawW = Height * imgRatio;
                    drawX = (Width - drawW) / 2f;
                }

                g.DrawImage(_bgImage, drawX, drawY, drawW, drawH);
            }
            else
            {
                using (var brush = new SolidBrush(Theme.Bg))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            // Мягкая виньетка слева вверху для читаемости названия и бейджей
            int heroVignetteW = Math.Min(Width, 680);
            int heroVignetteH = Math.Min(Height, 280);
            if (heroVignetteW > 20 && heroVignetteH > 20)
            {
                var rectHero = new Rectangle(0, 0, heroVignetteW, heroVignetteH);
                using (var lgbHero = new LinearGradientBrush(rectHero,
                    Color.FromArgb(160, 14, 16, 22),
                    Color.FromArgb(0, 14, 16, 22),
                    LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(lgbHero, rectHero);
                }
            }

            // Бесшовная плавная виньетка снизу под Command Deck
            int gradH = (int)(Height * 0.48f);
            if (gradH > 10)
            {
                int startY = Height - gradH;
                var brushRect = new Rectangle(0, startY, Width, gradH);
                using (var lgb = new LinearGradientBrush(brushRect,
                    Color.FromArgb(0, 14, 16, 22),
                    Color.FromArgb(240, 14, 16, 22),
                    LinearGradientMode.Vertical))
                {
                    var blend = new Blend(4)
                    {
                        Positions = new float[] { 0.0f, 0.25f, 0.70f, 1.0f },
                        Factors   = new float[] { 0.0f, 0.15f, 0.75f, 1.0f }
                    };
                    lgb.Blend = blend;
                    g.FillRectangle(lgb, brushRect);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            DrawBackdrop(pevent.Graphics, ClientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }

    /// <summary>
    /// Double-buffered vector logo control with animated spinning gear and crisp typography.
    /// </summary>
    public class LogoBox : Control
    {
        public float GearAngle { get; set; }
        public bool IsHovered { get; private set; }

        public LogoBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();
            BackColor = Color.FromArgb(16, 20, 26);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            IsHovered = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsHovered = false;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Отрисовка происходит напрямую в OnPaint на вторичном буфере
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var b = new SolidBrush(BackColor))
            {
                g.FillRectangle(b, ClientRectangle);
            }

            // Шестеренка по центру по вертикали
            Gear.Draw(g, new RectangleF(6, 15, 38, 38), Theme.Gold, GearAngle);

            using (var sf = new StringFormat
            {
                HotkeyPrefix = HotkeyPrefix.None,
                FormatFlags = StringFormatFlags.NoWrap
            })
            {
                // Заголовок STEAMPUNK
                using (var fTitle = Theme.Display(13.5f, FontStyle.Bold))
                using (var bTitle = new SolidBrush(Theme.Gold))
                {
                    g.DrawString("STEAMPUNK", fTitle, bTitle, new PointF(52, 14), sf);
                }

                // Подзаголовок & SWORDS
                using (var fSub = Theme.Font(9f, FontStyle.Bold))
                using (var bSub = new SolidBrush(Theme.TextDim))
                {
                    g.DrawString("& SWORDS", fSub, bSub, new PointF(54, 36), sf);
                }
            }
        }
    }
}
