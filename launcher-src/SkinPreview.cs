using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    /// <summary>
    /// Software renderer for a 64x64 Minecraft skin: boxes are projected
    /// orthographically, so every face becomes a parallelogram that an affine
    /// texture transform can fill exactly.
    /// </summary>
    public class SkinPreview : Control
    {
        private struct V3
        {
            public float X, Y, Z;
            public V3(float x, float y, float z) { X = x; Y = y; Z = z; }
        }

        private class Face
        {
            public PointF A, B, C, D;
            public RectangleF Uv;
            public float Depth;
            public int Shade;
        }

        private readonly List<Face> _faces = new List<Face>();
        private readonly Timer _timer;

        private Bitmap _skin;
        private Bitmap _placeholder;
        private int _armWidth = 4;

        private float _yaw = -28f;
        private float _pitch = -8f;
        private float _zoom = 1f;
        private bool _dragging;
        private Point _dragFrom;
        private bool _autoSpin = true;

        // projection state, rebuilt for every paint
        private float _s, _cx, _cy, _sinYaw, _cosYaw, _sinPitch, _cosPitch;

        public event EventHandler SkinChanged;

        public SkinPreview()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.Bg;
            Cursor = Cursors.Hand;

            _placeholder = CreatePlaceholder();

            _timer = new Timer { Interval = 33 };
            _timer.Tick += (s, e) =>
            {
                if (!Visible || _dragging || !_autoSpin) return;
                _yaw += 0.22f;
                if (_yaw > 360f) _yaw -= 360f;
                Invalidate();
            };
            _timer.Start();
        }

        public bool HasCustomSkin
        {
            get { return _skin != null; }
        }

        public bool IsSlim
        {
            get { return _armWidth == 3; }
        }

        public void SetSkin(Bitmap skin)
        {
            if (_skin != null) { _skin.Dispose(); _skin = null; }

            if (skin != null)
            {
                _skin = ToArgb(skin);
                _armWidth = IsSlimSkin(_skin) ? 3 : 4;
            }
            else
            {
                _armWidth = 4;
            }

            Invalidate();
            var handler = SkinChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public static bool IsSlimSkin(Bitmap skin)
        {
            if (skin == null || skin.Width < 64 || skin.Height < 64) return false;
            if (skin.PixelFormat == PixelFormat.Format32bppArgb || skin.PixelFormat == PixelFormat.Format32bppPArgb)
                return skin.GetPixel(54, 20).A == 0;
            return false;
        }

        private static Bitmap ToArgb(Bitmap src)
        {
            var bmp = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(src, 0, 0, src.Width, src.Height);
            }
            return bmp;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragFrom = e.Location;
                Cursor = Cursors.SizeAll;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_dragging)
            {
                _yaw += (e.X - _dragFrom.X) * 0.7f;
                _pitch += (e.Y - _dragFrom.Y) * 0.35f;
                if (_pitch < -30f) _pitch = -30f;
                if (_pitch > 30f) _pitch = 30f;
                _dragFrom = e.Location;
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _dragging = false;
            Cursor = Cursors.Hand;
            base.OnMouseUp(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            _yaw = -28f;
            _pitch = -8f;
            _zoom = 1f;
            Invalidate();
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            _zoom *= e.Delta > 0 ? 1.08f : 1 / 1.08f;
            if (_zoom < 0.7f) _zoom = 0.7f;
            if (_zoom > 1.8f) _zoom = 1.8f;
            Invalidate();
            base.OnMouseWheel(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            Theme.PaintControlBackground(g, this);

            // soft glow behind the character
            var glow = new RectangleF(Width * 0.5f - Width * 0.42f, Height * 0.18f, Width * 0.84f, Height * 0.7f);
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(glow);
                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(46, 217, 169, 74);
                    brush.SurroundColors = new[] { Color.FromArgb(0, 217, 169, 74) };
                    g.FillPath(brush, path);
                }
            }

            Prepare();
            BuildFaces();

            // ground shadow
            var shadowBrushRect = new RectangleF(_cx - 7.5f * _s, _cy + 14.2f * _s, 15f * _s, 4.4f * _s);
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(shadowBrushRect);
                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(120, 0, 0, 0);
                    brush.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                    g.FillPath(brush, path);
                }
            }

            _faces.Sort((a, b) => a.Depth.CompareTo(b.Depth));

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            foreach (var face in _faces) DrawFace(g, face);

            g.SmoothingMode = SmoothingMode.Default;
            g.PixelOffsetMode = PixelOffsetMode.Default;
        }

        private void Prepare()
        {
            float pad = 12f;
            float available = Math.Max(40f, Height - pad * 2);
            _s = Math.Min(Width / 20f, available / 35f) * 1.15f * _zoom;
            _cx = Width * 0.5f;
            _cy = Height * 0.5f + 1f * _s;

            double yaw = _yaw * Math.PI / 180.0;
            double pitch = _pitch * Math.PI / 180.0;
            _sinYaw = (float)Math.Sin(yaw);
            _cosYaw = (float)Math.Cos(yaw);
            _sinPitch = (float)Math.Sin(pitch);
            _cosPitch = (float)Math.Cos(pitch);
        }

        private PointF Project(V3 p)
        {
            float x = p.X * _cosYaw + p.Z * _sinYaw;
            float z = -p.X * _sinYaw + p.Z * _cosYaw;
            float y = p.Y * _cosPitch - z * _sinPitch;
            float depth = p.Y * _sinPitch + z * _cosPitch;
            LastDepth = depth;
            return new PointF(_cx + x * _s, _cy - (y - 16f) * _s);
        }

        private float LastDepth;

        private void AddFace(V3 p0, V3 p1, V3 p2, V3 p3, RectangleF uv, int shade)
        {
            if (uv.Width <= 0 || uv.Height <= 0) return;

            var a = Project(p0);
            float d0 = LastDepth;
            var b = Project(p1);
            float d1 = LastDepth;
            var c = Project(p2);
            float d2 = LastDepth;
            var d = Project(p3);
            float d3 = LastDepth;

            float cross = (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
            if (cross <= 0.0001f) return; // back face

            _faces.Add(new Face
            {
                A = a, B = b, C = c, D = d,
                Uv = uv,
                Depth = (d0 + d1 + d2 + d3) * 0.25f,
                Shade = shade
            });
        }

        private void AddBox(float x0, float y0, float z0, float x1, float y1, float z1,
            RectangleF front, RectangleF right, RectangleF left, RectangleF back,
            RectangleF top, RectangleF bottom)
        {
            // Front (+Z)
            AddFace(new V3(x0, y1, z1), new V3(x1, y1, z1), new V3(x1, y0, z1), new V3(x0, y0, z1), front, 0);
            // Back (-Z)
            AddFace(new V3(x1, y1, z0), new V3(x0, y1, z0), new V3(x0, y0, z0), new V3(x1, y0, z0), back, 60);
            // Left (+X) - character's left: UV goes from front (+Z) to back (-Z)
            AddFace(new V3(x1, y1, z1), new V3(x1, y1, z0), new V3(x1, y0, z0), new V3(x1, y0, z1), left, 30);
            // Right (-X) - character's right: UV goes from back (-Z) to front (+Z)
            AddFace(new V3(x0, y1, z0), new V3(x0, y1, z1), new V3(x0, y0, z1), new V3(x0, y0, z0), right, 45);
            // Top (+Y)
            AddFace(new V3(x0, y1, z0), new V3(x1, y1, z0), new V3(x1, y1, z1), new V3(x0, y1, z1), top, 15);
            // Bottom (-Y)
            AddFace(new V3(x0, y0, z1), new V3(x1, y0, z1), new V3(x1, y0, z0), new V3(x0, y0, z0), bottom, 95);
        }

        private static RectangleF Uv(float x, float y, float w, float h)
        {
            return new RectangleF(x, y, w, h);
        }

        private void BuildFaces()
        {
            _faces.Clear();
            int arm = _armWidth;

            // right leg / left leg (Layer 1)
            AddBox(-4, 0, -2, 0, 12, 2,
                Uv(4, 20, 4, 12), Uv(0, 20, 4, 12), Uv(8, 20, 4, 12), Uv(12, 20, 4, 12),
                Uv(4, 16, 4, 4), Uv(8, 16, 4, 4));
            AddBox(0, 0, -2, 4, 12, 2,
                Uv(20, 52, 4, 12), Uv(16, 52, 4, 12), Uv(24, 52, 4, 12), Uv(28, 52, 4, 12),
                Uv(20, 48, 4, 4), Uv(24, 48, 4, 4));

            // body (Layer 1)
            AddBox(-4, 12, -2, 4, 24, 2,
                Uv(20, 20, 8, 12), Uv(16, 20, 4, 12), Uv(28, 20, 4, 12), Uv(32, 20, 8, 12),
                Uv(20, 16, 8, 4), Uv(28, 16, 8, 4));

            // right arm and left arm (Layer 1)
            AddBox(-4 - arm, 12, -2, -4, 24, 2,
                Uv(44, 20, 4, 12), Uv(40, 20, 4, 12), Uv(48, 20, 4, 12), Uv(52, 20, 4, 12),
                Uv(44, 16, 4, 4), Uv(48, 16, 4, 4));
            AddBox(4, 12, -2, 4 + arm, 24, 2,
                Uv(36, 52, 4, 12), Uv(32, 52, 4, 12), Uv(40, 52, 4, 12), Uv(44, 52, 4, 12),
                Uv(36, 48, 4, 4), Uv(40, 48, 4, 4));

            // head (Layer 1)
            AddBox(-4, 24, -4, 4, 32, 4,
                Uv(8, 8, 8, 8), Uv(0, 8, 8, 8), Uv(16, 8, 8, 8), Uv(24, 8, 8, 8),
                Uv(8, 0, 8, 8), Uv(16, 0, 8, 8));

            // ─── Слой 2 (Внешний слой одежды и волос / Hat layer) ───
            // Голова (Шапка / Волосы)
            AddBox(-4.35f, 23.65f, -4.35f, 4.35f, 32.35f, 4.35f,
                Uv(40, 8, 8, 8), Uv(32, 8, 8, 8), Uv(48, 8, 8, 8), Uv(56, 8, 8, 8),
                Uv(40, 0, 8, 8), Uv(48, 0, 8, 8));

            // Куртка / Тело (Слой 2)
            AddBox(-4.2f, 11.8f, -2.2f, 4.2f, 24.2f, 2.2f,
                Uv(20, 36, 8, 12), Uv(16, 36, 4, 12), Uv(28, 36, 4, 12), Uv(32, 36, 8, 12),
                Uv(20, 32, 8, 4), Uv(28, 32, 8, 4));

            // Рукава (Слой 2)
            AddBox(-4.2f - arm, 11.8f, -2.2f, -3.8f, 24.2f, 2.2f,
                Uv(44, 36, 4, 12), Uv(40, 36, 4, 12), Uv(48, 36, 4, 12), Uv(52, 36, 4, 12),
                Uv(44, 32, 4, 4), Uv(48, 32, 4, 4));
            AddBox(3.8f, 11.8f, -2.2f, 4.2f + arm, 24.2f, 2.2f,
                Uv(52, 52, 4, 12), Uv(48, 52, 4, 12), Uv(56, 52, 4, 12), Uv(60, 52, 4, 12),
                Uv(52, 48, 4, 4), Uv(56, 48, 4, 4));

            // Штанины (Слой 2)
            AddBox(-4.2f, -0.2f, -2.2f, 0.2f, 12.2f, 2.2f,
                Uv(4, 36, 4, 12), Uv(0, 36, 4, 12), Uv(8, 36, 4, 12), Uv(12, 36, 4, 12),
                Uv(4, 32, 4, 4), Uv(8, 32, 4, 4));
            AddBox(-0.2f, -0.2f, -2.2f, 4.2f, 12.2f, 2.2f,
                Uv(4, 52, 4, 12), Uv(0, 52, 4, 12), Uv(8, 52, 4, 12), Uv(12, 52, 4, 12),
                Uv(4, 48, 4, 4), Uv(8, 48, 4, 4));
        }

        private void DrawFace(Graphics g, Face face)
        {
            var uv = face.Uv;
            if (uv.Width < 0.01f || uv.Height < 0.01f) return;

            var bitmap = _skin ?? _placeholder;
            if (bitmap == null) return;

            var state = g.Save();
            using (var clip = new GraphicsPath())
            {
                clip.AddPolygon(new[] { face.A, face.B, face.C, face.D });

                // parallelogram mapping: GDI+ aligns the source rectangle with AB (top edge) and AD (left edge)
                var dest = new[] { face.A, face.B, face.D };
                g.SetClip(clip, CombineMode.Replace);
                
                // Ensure no smoothing is applied to the texture itself
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                
                g.DrawImage(bitmap, dest, uv, GraphicsUnit.Pixel);

                if (face.Shade > 0)
                {
                    using (var shade = new SolidBrush(Color.FromArgb(face.Shade, 0, 0, 0)))
                        g.FillPath(shade, clip);
                }
            }
            g.Restore(state);
        }

        /// <summary>Simple built-in character shown until the player picks a skin.</summary>
        private static Bitmap CreatePlaceholder()
        {
            var bmp = new Bitmap(64, 64, PixelFormat.Format32bppArgb);
            Color skin = Color.FromArgb(226, 178, 140);
            Color skinDark = Color.FromArgb(198, 150, 116);
            Color hair = Color.FromArgb(92, 64, 45);
            Color shirt = Color.FromArgb(60, 92, 118);
            Color shirtDark = Color.FromArgb(48, 76, 100);
            Color pants = Color.FromArgb(78, 84, 106);
            Color shoes = Color.FromArgb(52, 54, 66);
            Color eye = Color.FromArgb(52, 46, 44);

            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                Action<float, float, float, float, Color> fill = (x, y, w, h, color) =>
                {
                    using (var brush = new SolidBrush(color))
                        g.FillRectangle(brush, x, y, w, h);
                };

                // head
                fill(8, 8, 8, 8, skin);
                fill(8, 8, 8, 3, hair);
                fill(9, 12, 2, 1, eye);
                fill(13, 12, 2, 1, eye);
                fill(0, 8, 8, 8, hair);
                fill(16, 8, 8, 8, hair);
                fill(24, 8, 8, 8, hair);
                fill(8, 0, 8, 8, hair);
                fill(16, 0, 8, 8, skinDark);

                // body
                fill(20, 20, 8, 12, shirt);
                fill(32, 20, 8, 12, shirt);
                fill(16, 20, 4, 12, shirtDark);
                fill(28, 20, 4, 12, shirtDark);
                fill(20, 16, 8, 4, shirtDark);
                fill(28, 16, 8, 4, shirtDark);

                // arms with short sleeves
                fill(40, 16, 16, 16, skin);
                fill(40, 16, 16, 4, shirt);
                fill(32, 48, 16, 16, skin);
                fill(32, 48, 16, 4, shirt);

                // legs
                fill(0, 16, 16, 16, pants);
                fill(16, 48, 16, 16, pants);
                fill(0, 28, 16, 4, shoes);
                fill(16, 60, 16, 4, shoes);
            }
            return bmp;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Stop();
                _timer.Dispose();
                if (_skin != null) _skin.Dispose();
                if (_placeholder != null) _placeholder.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
