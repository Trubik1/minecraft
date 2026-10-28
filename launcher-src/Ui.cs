using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    public static class Theme
    {
        public static readonly Color Background = Color.FromArgb(20, 23, 28);
        public static readonly Color Panel = Color.FromArgb(28, 33, 40);
        public static readonly Color PanelLight = Color.FromArgb(36, 42, 51);
        public static readonly Color Text = Color.FromArgb(230, 233, 239);
        public static readonly Color Muted = Color.FromArgb(152, 162, 179);
        public static readonly Color Gold = Color.FromArgb(224, 184, 74);
        public static readonly Color Green = Color.FromArgb(47, 158, 90);
        public static readonly Color GreenHover = Color.FromArgb(55, 178, 104);
        public static readonly Color Red = Color.FromArgb(214, 92, 92);
        public static readonly Color Border = Color.FromArgb(58, 66, 80);

        public static Font Font(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font("Segoe UI", size, style, GraphicsUnit.Point);
        }

        public static Button FlatButton(string text, int width, int height, Color back, Color hover)
        {
            var button = new Button
            {
                Text = text,
                Width = width,
                Height = height,
                FlatStyle = FlatStyle.Flat,
                BackColor = back,
                ForeColor = Color.White,
                Font = Font(9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = hover;
            button.FlatAppearance.MouseDownBackColor = hover;
            return button;
        }
    }

    public class FlatProgressBar : Control
    {
        private int _value;

        public FlatProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            Height = 10;
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

            var track = new Rectangle(0, 0, Width, Height);
            using (var back = new SolidBrush(Theme.PanelLight))
            using (var path = RoundRect(track, Height / 2))
                g.FillPath(back, path);

            if (_value > 0)
            {
                int fillWidth = Math.Max(Height, (int)(Width * (_value / 100.0)));
                var fill = new Rectangle(0, 0, fillWidth, Height);
                using (var brush = new LinearGradientBrush(fill, Theme.Gold, Theme.Green, LinearGradientMode.Horizontal))
                using (var path = RoundRect(fill, Height / 2))
                    g.FillPath(brush, path);
            }
        }

        private static GraphicsPath RoundRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 1)
            {
                path.AddRectangle(rect);
                return path;
            }

            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
