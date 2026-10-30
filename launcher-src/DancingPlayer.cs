using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    public enum DanceStyle
    {
        Groove,   // Стимпанк-грув
        Kazotsky  // Присядка (Казачок)
    }

    /// <summary>
    /// Масштабный интерактивный 3D-персонаж Minecraft, танцующий в правой части главного меню.
    /// Поддерживает скины игрока (Classic 4px и Slim 3px) со всеми внешними слоями (Hat, Jacket, Sleeves, Pants).
    /// Полностью очищен от багов плаща, частиц и навязчивых надписей.
    /// Предоставляет увеличенную рамку без обрезки рук и ног во время размашистых танцев.
    /// </summary>
    public class DancingPlayer : Control
    {
        private struct V3
        {
            public float X, Y, Z;
            public V3(float x, float y, float z) { X = x; Y = y; Z = z; }
        }

        private struct Transform
        {
            public float Ox, Oy, Oz;       // Сустав вращения (Joint Pivot)
            public float Pitch, Yaw, Roll; // Углы поворота в градусах
            public float Tx, Ty, Tz;       // Смещение (Translation)

            public Transform(float ox, float oy, float oz, float pitch = 0, float yaw = 0, float roll = 0, float tx = 0, float ty = 0, float tz = 0)
            {
                Ox = ox; Oy = oy; Oz = oz;
                Pitch = pitch; Yaw = yaw; Roll = roll;
                Tx = tx; Ty = ty; Tz = tz;
            }
        }

        private class Face
        {
            public PointF A, B, C, D;
            public RectangleF Uv;
            public float Depth;
            public int Shade;
        }

        private readonly List<Face> _faces = new List<Face>();
        private readonly Timer _animTimer;

        private Bitmap _skin;
        private Bitmap _placeholder;
        private int _armWidth = 4;

        // Ракурсы камеры
        private float _yaw = -26f;
        private float _pitch = -5f;
        private float _targetYaw = -26f;
        private float _zoom = 1f;

        // Интерактивное вращение
        private bool _dragging;
        private Point _dragFrom;
        private float _dragCooldown = 0f;
        private bool _isHovered;
        private Point _mousePos;

        // Состояние анимации
        private float _animTime = 0f;
        private DanceStyle _currentDance = DanceStyle.Groove;
        private bool _isExcited;

        // Проекция
        private float _s, _cx, _cy, _sinYaw, _cosYaw, _sinPitch, _cosPitch;
        private float _lastDepth;

        public DanceStyle CurrentDance
        {
            get { return _currentDance; }
            set { _currentDance = value; Invalidate(); }
        }

        public bool IsExcited
        {
            get { return _isExcited; }
            set { _isExcited = value; }
        }

        public DancingPlayer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;

            // Увеличенная рамка: 420x520 гарантирует, что никакие махи руками и ногами не обрежутся
            Size = new Size(420, 520);

            _placeholder = CreatePlaceholder();
            ReloadSkin();

            _animTimer = new Timer { Interval = 30 };
            _animTimer.Tick += (s, e) => UpdateAnimation(0.030f);
            _animTimer.Start();
        }

        public void ReloadSkin()
        {
            try
            {
                var loaded = SkinStore.Load();
                if (loaded != null)
                {
                    if (_skin != null) _skin.Dispose();
                    _skin = loaded;
                    _armWidth = SkinPreview.IsSlimSkin(_skin) ? 3 : 4;
                }
                else
                {
                    _armWidth = 4;
                }
            }
            catch
            {
                _armWidth = 4;
            }
            Invalidate();
        }

        private void UpdateAnimation(float dt)
        {
            if (!Visible) return;

            float speedMultiplier = _isExcited ? 1.35f : 1.0f;
            _animTime += dt * speedMultiplier;

            if (!_dragging)
            {
                if (_dragCooldown > 0f)
                {
                    _dragCooldown -= dt;
                }
                else
                {
                    _yaw += (_targetYaw - _yaw) * 0.05f;
                }
            }

            Invalidate();
        }

        public void CycleDance()
        {
            // Переключаем только между двумя лучшими танцами: Стимпанк-грув и Присядка
            _currentDance = (_currentDance == DanceStyle.Groove) ? DanceStyle.Kazotsky : DanceStyle.Groove;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragFrom = e.Location;
                _dragCooldown = 3.5f;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _isHovered = true;
            _mousePos = e.Location;

            if (_dragging)
            {
                _yaw += (e.X - _dragFrom.X) * 0.8f;
                _pitch += (e.Y - _dragFrom.Y) * 0.35f;
                if (_pitch < -22f) _pitch = -22f;
                if (_pitch > 18f) _pitch = 18f;
                _dragFrom = e.Location;
                _dragCooldown = 3.5f;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_dragging)
            {
                _dragging = false;
                int distSq = (e.X - _dragFrom.X) * (e.X - _dragFrom.X) + (e.Y - _dragFrom.Y) * (e.Y - _dragFrom.Y);
                if (distSq < 25)
                {
                    CycleDance();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            // Мягкая бирюзовая подсветка от портала с левой стороны
            var glowRect = new RectangleF(Width * 0.12f, Height * 0.22f, Width * 0.72f, Height * 0.65f);
            using (var pathGlow = new GraphicsPath())
            {
                pathGlow.AddEllipse(glowRect);
                using (var brushGlow = new PathGradientBrush(pathGlow))
                {
                    brushGlow.CenterColor = Color.FromArgb(28, 45, 210, 185);
                    brushGlow.SurroundColors = new[] { Color.FromArgb(0, 45, 210, 185) };
                    g.FillPath(brushGlow, pathGlow);
                }
            }

            PrepareCamera();
            BuildDancingSkel();

            // Реалистичная тень на каменном постаменте прямо под ногами
            var shadowRect = new RectangleF(_cx - 10f * _s, _cy + 15.6f * _s, 20f * _s, 5.2f * _s);
            using (var pathShadow = new GraphicsPath())
            {
                pathShadow.AddEllipse(shadowRect);
                using (var brushShadow = new PathGradientBrush(pathShadow))
                {
                    brushShadow.CenterColor = Color.FromArgb(145, 0, 0, 0);
                    brushShadow.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                    g.FillPath(brushShadow, pathShadow);
                }
            }

            // Сортировка по глубине (Painter's Algorithm)
            _faces.Sort((a, b) => a.Depth.CompareTo(b.Depth));

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            foreach (var face in _faces)
            {
                DrawFace(g, face);
            }
        }

        private void PrepareCamera()
        {
            // Масштаб персонажа: героический размер с гарантированным запасом от обрезки сверху и снизу
            float availableH = Math.Max(100f, Height - 60f);
            _s = (availableH / 37.0f) * _zoom;
            _cx = Width * 0.5f;
            _cy = Height * 0.43f;

            double yawRad = _yaw * Math.PI / 180.0;
            double pitchRad = _pitch * Math.PI / 180.0;
            _sinYaw = (float)Math.Sin(yawRad);
            _cosYaw = (float)Math.Cos(yawRad);
            _sinPitch = (float)Math.Sin(pitchRad);
            _cosPitch = (float)Math.Cos(pitchRad);
        }

        private PointF Project(V3 p)
        {
            float x = p.X * _cosYaw + p.Z * _sinYaw;
            float z = -p.X * _sinYaw + p.Z * _cosYaw;
            float y = p.Y * _cosPitch - z * _sinPitch;
            float depth = p.Y * _sinPitch + z * _cosPitch;
            _lastDepth = depth;
            return new PointF(_cx + x * _s, _cy - (y - 16f) * _s);
        }

        private static V3 ApplyTransform(V3 v, Transform t)
        {
            float x = v.X - t.Ox;
            float y = v.Y - t.Oy;
            float z = v.Z - t.Oz;

            if (Math.Abs(t.Pitch) > 0.001f)
            {
                float rad = (float)(t.Pitch * Math.PI / 180.0);
                float s = (float)Math.Sin(rad);
                float c = (float)Math.Cos(rad);
                float y1 = y * c - z * s;
                float z1 = y * s + z * c;
                y = y1;
                z = z1;
            }

            if (Math.Abs(t.Yaw) > 0.001f)
            {
                float rad = (float)(t.Yaw * Math.PI / 180.0);
                float s = (float)Math.Sin(rad);
                float c = (float)Math.Cos(rad);
                float x1 = x * c + z * s;
                float z1 = -x * s + z * c;
                x = x1;
                z = z1;
            }

            if (Math.Abs(t.Roll) > 0.001f)
            {
                float rad = (float)(t.Roll * Math.PI / 180.0);
                float s = (float)Math.Sin(rad);
                float c = (float)Math.Cos(rad);
                float x1 = x * c - y * s;
                float y1 = x * s + y * c;
                x = x1;
                y = y1;
            }

            return new V3(x + t.Ox + t.Tx, y + t.Oy + t.Ty, z + t.Oz + t.Tz);
        }

        private void AddFace(V3 p0, V3 p1, V3 p2, V3 p3, RectangleF uv, int shade)
        {
            if (uv.Width <= 0 || uv.Height <= 0) return;

            var a = Project(p0);
            float d0 = _lastDepth;
            var b = Project(p1);
            float d1 = _lastDepth;
            var c = Project(p2);
            float d2 = _lastDepth;
            var d = Project(p3);
            float d3 = _lastDepth;

            float cross = (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
            if (cross <= 0.0001f) return;

            _faces.Add(new Face
            {
                A = a, B = b, C = c, D = d,
                Uv = uv,
                Depth = (d0 + d1 + d2 + d3) * 0.25f,
                Shade = shade
            });
        }

        private void AddTransformedBox(float x0, float y0, float z0, float x1, float y1, float z1,
            Transform t,
            RectangleF front, RectangleF right, RectangleF left, RectangleF back,
            RectangleF top, RectangleF bottom)
        {
            V3 v000 = ApplyTransform(new V3(x0, y0, z0), t);
            V3 v001 = ApplyTransform(new V3(x0, y0, z1), t);
            V3 v010 = ApplyTransform(new V3(x0, y1, z0), t);
            V3 v011 = ApplyTransform(new V3(x0, y1, z1), t);
            V3 v100 = ApplyTransform(new V3(x1, y0, z0), t);
            V3 v101 = ApplyTransform(new V3(x1, y0, z1), t);
            V3 v110 = ApplyTransform(new V3(x1, y1, z0), t);
            V3 v111 = ApplyTransform(new V3(x1, y1, z1), t);

            // Front (+Z)
            AddFace(v011, v111, v101, v001, front, 0);
            // Back (-Z)
            AddFace(v110, v010, v000, v100, back, 55);
            // Left (+X)
            AddFace(v111, v110, v100, v101, left, 25);
            // Right (-X)
            AddFace(v010, v011, v001, v000, right, 40);
            // Top (+Y)
            AddFace(v010, v110, v111, v011, top, 15);
            // Bottom (-Y)
            AddFace(v001, v101, v100, v000, bottom, 90);
        }

        private static RectangleF Uv(float x, float y, float w, float h)
        {
            return new RectangleF(x, y, w, h);
        }

        private void BuildDancingSkel()
        {
            _faces.Clear();
            int arm = _armWidth;
            float t = _animTime;

            float rootX = 0, rootY = 0, rootZ = 0;
            float bodyPitch = 0, bodyYaw = 0, bodyRoll = 0;
            float headPitch = 0, headYaw = 0, headRoll = 0;
            float rArmPitch = 0, rArmYaw = 0, rArmRoll = 0;
            float lArmPitch = 0, lArmYaw = 0, lArmRoll = 0;
            float rLegPitch = 0, rLegYaw = 0, rLegRoll = 0;
            float lLegPitch = 0, lLegYaw = 0, lLegRoll = 0;

            if (_currentDance == DanceStyle.Groove)
            {
                // Стимпанк-грув: упругий пружинящий бит и кач руками/бёдрами
                float bounce = (float)Math.Abs(Math.Sin(t * 6.2)) * 1.8f;
                rootY = bounce;
                rootX = (float)Math.Sin(t * 3.1) * 1.8f;

                bodyRoll = (float)Math.Sin(t * 3.1) * 5.0f;
                bodyYaw = (float)Math.Cos(t * 1.55) * 8.0f;

                headPitch = -4f + (float)Math.Abs(Math.Sin(t * 6.2)) * 12.0f;
                headRoll = -(float)Math.Sin(t * 3.1) * 6.0f;

                rArmPitch = (float)Math.Sin(t * 6.2) * 35.0f;
                rArmRoll = 14.0f + (float)Math.Abs(Math.Cos(t * 3.1)) * 18.0f;

                lArmPitch = -(float)Math.Sin(t * 6.2) * 35.0f;
                lArmRoll = -14.0f - (float)Math.Abs(Math.Cos(t * 3.1)) * 18.0f;

                rLegPitch = -(float)Math.Sin(t * 3.1) * 20.0f;
                lLegPitch = (float)Math.Sin(t * 3.1) * 20.0f;
            }
            else // DanceStyle.Kazotsky
            {
                // Казачок / Присядка: глубокий присед со скрещенными руками и поочередным выбросом ног
                float kKick = (float)Math.Sin(t * 7.5);
                float kBounce = (float)Math.Abs(Math.Sin(t * 7.5)) * 1.0f;
                rootY = -2.8f + kBounce;

                rArmPitch = -78.0f;
                rArmRoll = 42.0f;
                rArmYaw = 38.0f;

                lArmPitch = -78.0f;
                lArmRoll = -42.0f;
                lArmYaw = -38.0f;

                bodyPitch = 6.0f;
                headPitch = -10.0f + kBounce * 4.0f;
                headRoll = (float)Math.Sin(t * 3.75) * 5.0f;

                // Базовый угол сгиба опорной ноги, удерживающий ступни на каменном постаменте
                float baseBend = 38.0f;
                if (kKick > 0)
                {
                    rLegPitch = -72.0f * kKick;
                    lLegPitch = baseBend + (1f - kKick) * 6.0f;
                }
                else
                {
                    float inv = -kKick;
                    lLegPitch = -72.0f * inv;
                    rLegPitch = baseBend + (1f - inv) * 6.0f;
                }
            }

            // Интерактивное слежение взглядом за курсором мыши
            if (_isHovered && !_dragging)
            {
                float dx = (_mousePos.X - (Width * 0.5f)) / (Width * 0.5f);
                float dy = (_mousePos.Y - (Height * 0.35f)) / (Height * 0.5f);
                headYaw += dx * 16.0f;
                headPitch += dy * 12.0f;
            }

            // 1. Торс (Body)
            var tBody = new Transform(0, 12, 0, bodyPitch, bodyYaw, bodyRoll, rootX, rootY, rootZ);
            AddTransformedBox(-4, 12, -2, 4, 24, 2, tBody,
                Uv(20, 20, 8, 12), Uv(16, 20, 4, 12), Uv(28, 20, 4, 12), Uv(32, 20, 8, 12),
                Uv(20, 16, 8, 4), Uv(28, 16, 8, 4));

            // Куртка (Layer 2)
            AddTransformedBox(-4.2f, 11.8f, -2.2f, 4.2f, 24.2f, 2.2f, tBody,
                Uv(20, 36, 8, 12), Uv(16, 36, 4, 12), Uv(28, 36, 4, 12), Uv(32, 36, 8, 12),
                Uv(20, 32, 8, 4), Uv(28, 32, 8, 4));

            // 2. Голова (Head)
            var tHead = new Transform(0, 24, 0, headPitch + bodyPitch, headYaw + bodyYaw, headRoll + bodyRoll, rootX, rootY, rootZ);
            AddTransformedBox(-4, 24, -4, 4, 32, 4, tHead,
                Uv(8, 8, 8, 8), Uv(0, 8, 8, 8), Uv(16, 8, 8, 8), Uv(24, 8, 8, 8),
                Uv(8, 0, 8, 8), Uv(16, 0, 8, 8));

            // Шапка / Волосы (Layer 2)
            AddTransformedBox(-4.35f, 23.65f, -4.35f, 4.35f, 32.35f, 4.35f, tHead,
                Uv(40, 8, 8, 8), Uv(32, 8, 8, 8), Uv(48, 8, 8, 8), Uv(56, 8, 8, 8),
                Uv(40, 0, 8, 8), Uv(48, 0, 8, 8));

            // 3. Правая рука (Right Arm)
            float rShoulderX = -4f - (arm / 2f);
            var tRArm = new Transform(rShoulderX, 22, 0, rArmPitch, rArmYaw, rArmRoll, rootX, rootY, rootZ);
            AddTransformedBox(-4 - arm, 12, -2, -4, 24, 2, tRArm,
                Uv(44, 20, 4, 12), Uv(40, 20, 4, 12), Uv(48, 20, 4, 12), Uv(52, 20, 4, 12),
                Uv(44, 16, 4, 4), Uv(48, 16, 4, 4));

            // Рукав правый (Layer 2)
            AddTransformedBox(-4.2f - arm, 11.8f, -2.2f, -3.8f, 24.2f, 2.2f, tRArm,
                Uv(44, 36, 4, 12), Uv(40, 36, 4, 12), Uv(48, 36, 4, 12), Uv(52, 36, 4, 12),
                Uv(44, 32, 4, 4), Uv(48, 32, 4, 4));

            // 4. Левая рука (Left Arm)
            float lShoulderX = 4f + (arm / 2f);
            var tLArm = new Transform(lShoulderX, 22, 0, lArmPitch, lArmYaw, lArmRoll, rootX, rootY, rootZ);
            AddTransformedBox(4, 12, -2, 4 + arm, 24, 2, tLArm,
                Uv(36, 52, 4, 12), Uv(32, 52, 4, 12), Uv(40, 52, 4, 12), Uv(44, 52, 4, 12),
                Uv(36, 48, 4, 4), Uv(40, 48, 4, 4));

            // Рукав левый (Layer 2)
            AddTransformedBox(3.8f, 11.8f, -2.2f, 4.2f + arm, 24.2f, 2.2f, tLArm,
                Uv(52, 52, 4, 12), Uv(48, 52, 4, 12), Uv(56, 52, 4, 12), Uv(60, 52, 4, 12),
                Uv(52, 48, 4, 4), Uv(56, 48, 4, 4));

            // 5. Правая нога (Right Leg)
            var tRLeg = new Transform(-2, 12, 0, rLegPitch, rLegYaw, rLegRoll, rootX * 0.3f, rootY, rootZ);
            AddTransformedBox(-4, 0, -2, 0, 12, 2, tRLeg,
                Uv(4, 20, 4, 12), Uv(0, 20, 4, 12), Uv(8, 20, 4, 12), Uv(12, 20, 4, 12),
                Uv(4, 16, 4, 4), Uv(8, 16, 4, 4));

            // Штанина правая (Layer 2)
            AddTransformedBox(-4.2f, -0.2f, -2.2f, 0.2f, 12.2f, 2.2f, tRLeg,
                Uv(4, 36, 4, 12), Uv(0, 36, 4, 12), Uv(8, 36, 4, 12), Uv(12, 36, 4, 12),
                Uv(4, 32, 4, 4), Uv(8, 32, 4, 4));

            // 6. Левая нога (Left Leg)
            var tLLeg = new Transform(2, 12, 0, lLegPitch, lLegYaw, lLegRoll, rootX * 0.3f, rootY, rootZ);
            AddTransformedBox(0, 0, -2, 4, 12, 2, tLLeg,
                Uv(20, 52, 4, 12), Uv(16, 52, 4, 12), Uv(24, 52, 4, 12), Uv(28, 52, 4, 12),
                Uv(20, 48, 4, 4), Uv(24, 48, 4, 4));

            // Штанина левая (Layer 2)
            AddTransformedBox(-0.2f, -0.2f, -2.2f, 4.2f, 12.2f, 2.2f, tLLeg,
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

                var dest = new[] { face.A, face.B, face.D };
                g.SetClip(clip, CombineMode.Replace);
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

        private static Bitmap CreatePlaceholder()
        {
            var bmp = new Bitmap(64, 64, PixelFormat.Format32bppArgb);
            Color skin = Color.FromArgb(226, 178, 140);
            Color hair = Color.FromArgb(92, 64, 45);
            Color shirt = Color.FromArgb(60, 92, 118);
            Color pants = Color.FromArgb(78, 84, 106);
            Color shoes = Color.FromArgb(52, 54, 66);

            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                Action<float, float, float, float, Color> fill = (x, y, w, h, color) =>
                {
                    using (var brush = new SolidBrush(color))
                        g.FillRectangle(brush, x, y, w, h);
                };

                fill(8, 8, 8, 8, skin);
                fill(8, 8, 8, 3, hair);
                fill(20, 20, 8, 12, shirt);
                fill(44, 20, 4, 12, shirt);
                fill(36, 52, 4, 12, shirt);
                fill(4, 20, 4, 12, pants);
                fill(20, 52, 4, 12, pants);
                fill(4, 28, 4, 4, shoes);
                fill(20, 60, 4, 4, shoes);
            }
            return bmp;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animTimer.Stop();
                _animTimer.Dispose();
                if (_skin != null) _skin.Dispose();
                if (_placeholder != null) _placeholder.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
