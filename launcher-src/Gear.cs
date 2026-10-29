using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SteampunkLauncher
{
    /// <summary>Shared steampunk gear emblem used by the header, watermark and window icon.</summary>
    internal static class Gear
    {
        public static void Draw(Graphics g, RectangleF rect, Color color, float angleDegrees = 0f)
        {
            float cx = rect.X + rect.Width / 2f;
            float cy = rect.Y + rect.Height / 2f;
            float rOuter = rect.Width / 2f - 1f;
            float thickness = Math.Max(1.4f, rect.Width * 0.06f);
            float toothLen = rect.Width * 0.18f;

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var state = g.Save();
            try
            {
                g.TranslateTransform(cx, cy);
                if (angleDegrees != 0f)
                {
                    g.RotateTransform(angleDegrees);
                }

                using (var pen = new Pen(color, thickness))
                {
                    // 8 teeth radiating outwards
                    for (int i = 0; i < 8; i++)
                    {
                        float a = i * 45f * (float)(Math.PI / 180.0);
                        float cos = (float)Math.Cos(a);
                        float sin = (float)Math.Sin(a);
                        float x1 = (rOuter - toothLen) * cos;
                        float y1 = (rOuter - toothLen) * sin;
                        float x2 = rOuter * cos;
                        float y2 = rOuter * sin;
                        g.DrawLine(pen, x1, y1, x2, y2);
                    }

                    // Outer rim
                    float rRim = rOuter - toothLen * 0.35f;
                    g.DrawEllipse(pen, -rRim, -rRim, rRim * 2f, rRim * 2f);

                    // Inner circle
                    float rInner = rRim * 0.52f;
                    g.DrawEllipse(pen, -rInner, -rInner, rInner * 2f, rInner * 2f);

                    using (var brush = new SolidBrush(Color.FromArgb(Math.Max(15, color.A / 4), color)))
                    {
                        g.FillEllipse(brush, -rInner, -rInner, rInner * 2f, rInner * 2f);
                    }
                }

                // Center dot
                float dot = Math.Max(1.5f, rect.Width * 0.055f);
                using (var brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, -dot, -dot, dot * 2f, dot * 2f);
                }
            }
            finally
            {
                g.Restore(state);
                g.SmoothingMode = oldMode;
            }
        }
    }
}
