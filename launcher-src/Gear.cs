using System.Drawing;
using System.Drawing.Drawing2D;

namespace SteampunkLauncher
{
    /// <summary>Shared steampunk gear emblem used by the header, watermark and window icon.</summary>
    internal static class Gear
    {
        public static void Draw(Graphics g, RectangleF rect, Color color)
        {
            var center = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
            float outer = rect.Width / 2f - 1f;
            float thickness = System.Math.Max(1.2f, rect.Width * 0.055f);
            float tooth = rect.Width * 0.17f;

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            try
            {
                using (var pen = new Pen(color, thickness))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        var state = g.Save();
                        g.TranslateTransform(center.X, center.Y);
                        g.RotateTransform(i * 45f);
                        g.DrawLine(pen, 0, -outer, 0, -outer + tooth);
                        g.Restore(state);
                    }

                    var body = new RectangleF(rect.X + rect.Width * 0.08f, rect.Y + rect.Width * 0.08f,
                        rect.Width * 0.84f, rect.Width * 0.84f);
                    g.DrawEllipse(pen, body);

                    var inner = RectangleF.Inflate(body, -body.Width * 0.27f, -body.Width * 0.27f);
                    g.DrawEllipse(pen, inner);

                    using (var brush = new SolidBrush(Color.FromArgb(System.Math.Max(10, color.A / 4), color)))
                        g.FillEllipse(brush, inner);
                }

                float dot = System.Math.Max(1.4f, rect.Width * 0.04f);
                using (var brush = new SolidBrush(color))
                    g.FillEllipse(brush, center.X - dot, center.Y - dot, dot * 2f, dot * 2f);
            }
            finally
            {
                g.SmoothingMode = oldMode;
            }
        }
    }
}
