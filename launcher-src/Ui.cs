using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    public static class Theme
    {
        // ─── Основная палитра (Modrinth Dark + Steampunk) ──────
        public static readonly Color Bg          = Color.FromArgb(19, 21, 26);
        public static readonly Color Background  = Color.FromArgb(19, 21, 26);
        public static readonly Color Sidebar     = Color.FromArgb(24, 27, 34);
        public static readonly Color Panel       = Color.FromArgb(28, 32, 40);
        public static readonly Color PanelLight  = Color.FromArgb(36, 42, 53);
        public static readonly Color Card        = Color.FromArgb(30, 35, 45);
        public static readonly Color CardAlt     = Color.FromArgb(25, 29, 38);
        public static readonly Color Field       = Color.FromArgb(22, 25, 33);

        public static readonly Color Text        = Color.FromArgb(232, 235, 243);
        public static readonly Color TextDim     = Color.FromArgb(170, 178, 195);
        public static readonly Color Muted       = Color.FromArgb(130, 140, 160);
        public static readonly Color Faint       = Color.FromArgb(85, 95, 115);

        public static readonly Color Border      = Color.FromArgb(50, 58, 72);
        public static readonly Color BorderSoft  = Color.FromArgb(38, 44, 56);

        public static readonly Color Gold        = Color.FromArgb(224, 184, 74);
        public static readonly Color GoldSoft    = Color.FromArgb(180, 145, 55);
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

    /// <summary>Стилизованная интерактивная кнопка</summary>
    public class AccentButton : Button
    {
        public float Radius { get; set; } = 8f;
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
            if (Width <= 2 || Height <= 2) return;
            try
            {
                float r = IsGoldGlow ? (Height / 2f) : (Radius > 0 ? Radius : 8f);
                using (var path = Theme.RoundRect(new RectangleF(0, 0, Width, Height), r))
                {
                    this.Region = new Region(path);
                }
            }
            catch { }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRegion();
        }

        public void SetGreenStyle()
        {
            NormalColor = Theme.Green;
            HoverColor = Theme.GreenHover;
            PressedColor = Theme.GreenDark;
            ForeColor = Color.White;
            IsPrimary = true;
            IsGoldGlow = false;
            UpdateRegion();
            Invalidate();
        }

        public void SetRedStyle()
        {
            NormalColor = Theme.Red;
            HoverColor = Theme.RedHover;
            PressedColor = Color.FromArgb(180, 50, 50);
            ForeColor = Color.White;
            IsGoldGlow = false;
            UpdateRegion();
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
            UpdateRegion();
            Invalidate();
        }

        public void SetGoldGlowStyle()
        {
            IsGoldGlow = true;
            OutlineColor = Theme.Gold;
            ForeColor = Color.White;
            Radius = 28f;
            IsPrimary = true;
            UpdateRegion();
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; _isPressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs mevent) { _isPressed = true; Invalidate(); base.OnMouseDown(mevent); }
        protected override void OnMouseUp(MouseEventArgs mevent) { _isPressed = false; Invalidate(); base.OnMouseUp(mevent); }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (Width <= 2 || Height <= 2) return;
            if (this.Region == null) UpdateRegion();

            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float r = IsGoldGlow ? (Height / 2f) : (Radius > 0 ? Radius : 8f);
            var rect = new RectangleF(1f, 1f, Width - 2f, Height - 2f);

            if (IsGoldGlow)
            {
                Color topC = _isPressed ? Color.FromArgb(60, 36, 18) : (_isHovered ? Color.FromArgb(135, 82, 38) : Color.FromArgb(105, 62, 28));
                Color botC = _isPressed ? Color.FromArgb(85, 52, 24) : (_isHovered ? Color.FromArgb(180, 115, 48) : Color.FromArgb(145, 90, 38));

                using (var path = Theme.RoundRect(rect, r))
                {
                    using (var brush = new LinearGradientBrush(rect, topC, botC, LinearGradientMode.Vertical))
                        g.FillPath(brush, path);

                    Color borderC = _isHovered ? Color.FromArgb(255, 215, 105) : Color.FromArgb(224, 184, 74);
                    using (var pen = new Pen(borderC, 2f))
                        g.DrawPath(pen, path);

                    // Мягкий блик по верхней дуге
                    if (!_isPressed)
                    {
                        var highlightRect = new RectangleF(rect.X + 4, rect.Y + 1, rect.Width - 8, (rect.Height / 2f) - 2);
                        using (var hlPath = Theme.RoundRect(highlightRect, r / 2f))
                        using (var penHl = new Pen(Color.FromArgb(70, 255, 255, 255), 1f))
                            g.DrawPath(penHl, hlPath);
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

            // Текст кнопки
            Theme.PaintText(g, Text, Font, ForeColor, new RectangleF(0, 0, Width, Height),
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
            Height = 8;
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
            using (var back = new SolidBrush(Theme.Field))
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

    /// <summary>Капсульная кнопка вкладки для верхней шапки (LoliLand Style)</summary>
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
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = Theme.Font(9.5f, FontStyle.Bold);
            Cursor = Cursors.Hand;
            Size = new Size(120, 38);
        }

        public void UpdateRegion()
        {
            if (Width <= 2 || Height <= 2) return;
            try
            {
                using (var path = Theme.RoundRect(new RectangleF(0, 0, Width, Height), Height / 2f))
                {
                    this.Region = new Region(path);
                }
            }
            catch { }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRegion();
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 2 || Height <= 2) return;
            if (this.Region == null) UpdateRegion();
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_active)
            {
                var rect = new RectangleF(1.5f, 1.5f, Width - 3f, Height - 3f);
                using (var path = Theme.RoundRect(rect, Height / 2f))
                {
                    using (var brush = new SolidBrush(Color.FromArgb(90, 105, 130)))
                        g.FillPath(brush, path);
                    using (var pen = new Pen(Color.FromArgb(120, 140, 175), 1.2f))
                        g.DrawPath(pen, path);
                }
            }
            else if (_hover)
            {
                var rect = new RectangleF(1.5f, 1.5f, Width - 3f, Height - 3f);
                using (var path = Theme.RoundRect(rect, Height / 2f))
                using (var brush = new SolidBrush(Color.FromArgb(35, 255, 255, 255)))
                    g.FillPath(brush, path);
            }

            Color textColor = _active ? Color.White : (_hover ? Color.FromArgb(240, 245, 255) : Color.FromArgb(170, 180, 200));
            Theme.PaintText(g, Text, Font, textColor, new RectangleF(0, 0, Width, Height),
                StringAlignment.Center, StringAlignment.Center);
        }
    }
}
