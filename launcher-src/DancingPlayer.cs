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
        Kazotsky, // Казачок / Присядка
        Disco,    // Диско-лихорадка
        Floss     // Волна и флекс
    }

    /// <summary>
    /// Интерактивный анимированный 3D-персонаж Minecraft, танцующий прямо в главном меню на каменном постаменте.
    /// Поддерживает скины игрока (Classic/Slim), плащ, переключение танцев кликом мыши,
    /// интерактивное вращение и систему парящих музыкальных нот.
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
            public float Ox, Oy, Oz;       // Точка вращения (Joint Pivot)
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
            public Bitmap CustomTexture;
        }

        private class NoteParticle
        {
            public float X, Y;
            public float Vx, Vy;
            public float Life;
            public float MaxLife;
            public string Glyph;
            public Color Color;
        }

        private readonly List<Face> _faces = new List<Face>();
        private readonly List<NoteParticle> _notes = new List<NoteParticle>();
        private readonly Timer _animTimer;
        private readonly Random _rand = new Random();

        private Bitmap _skin;
        private Bitmap _placeholder;
        private Bitmap _cape;
        private int _armWidth = 4;

        // Ракурсы камеры
        private float _yaw = -26f;
        private float _pitch = -6f;
        private float _targetYaw = -26f;
        private float _zoom = 1f;

        // Интерактивность
        private bool _dragging;
        private Point _dragFrom;
        private float _dragCooldown = 0f;
        private bool _isHovered;
        private Point _mousePos;

        // Состояние анимации
        private float _animTime = 0f;
        private DanceStyle _currentDance = DanceStyle.Groove;
        private bool _isExcited;
        private float _noteSpawnTimer = 0f;

        // Всплывающий бейдж с названием танца
        private string _badgeText = "Стимпанк-грув 🕺";
        private float _badgeTimer = 0f;

        // Проекционные коэффициенты
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
            Size = new Size(260, 380);

            _placeholder = CreatePlaceholder();
            _cape = CreateSteampunkCape();
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

            // Плавное возвращение к герою после вращения мышкой
            if (!_dragging)
            {
                if (_dragCooldown > 0f)
                {
                    _dragCooldown -= dt;
                }
                else
                {
                    // Мягкий возврат к исходному ракурсу
                    _yaw += (_targetYaw - _yaw) * 0.05f;
                }
            }

            // Таймер бейджа
            if (_badgeTimer > 0f)
            {
                _badgeTimer -= dt;
            }

            // Спавн музыкальных нот
            _noteSpawnTimer += dt;
            float spawnInterval = _isExcited ? 0.35f : 0.65f;
            if (_noteSpawnTimer >= spawnInterval)
            {
                _noteSpawnTimer = 0f;
                SpawnMusicNote();
            }

            // Обновление партиклов
            for (int i = _notes.Count - 1; i >= 0; i--)
            {
                var n = _notes[i];
                n.Life -= dt;
                if (n.Life <= 0f)
                {
                    _notes.RemoveAt(i);
                }
                else
                {
                    n.X += n.Vx * dt;
                    n.Y += n.Vy * dt;
                    n.Vx += (float)Math.Sin(n.Life * 6.0) * 2.5f * dt;
                }
            }

            Invalidate();
        }

        private void SpawnMusicNote(bool burst = false)
        {
            string[] glyphs = new string[] { "♪", "♫", "✦", "★" };
            Color[] colors = new Color[] {
                Theme.Gold,
                Theme.GoldHover,
                Color.FromArgb(64, 224, 208), // Бирюзовое свечение портала
                Color.FromArgb(240, 245, 255)
            };

            float spawnX = Width * 0.5f + (_rand.Next(-24, 25));
            float spawnY = Height * 0.28f + (_rand.Next(-15, 15));

            _notes.Add(new NoteParticle
            {
                X = spawnX,
                Y = spawnY,
                Vx = (float)(_rand.NextDouble() * 30 - 15),
                Vy = (float)(_rand.NextDouble() * -35 - 25),
                Life = burst ? 1.4f : 1.8f,
                MaxLife = burst ? 1.4f : 1.8f,
                Glyph = glyphs[_rand.Next(glyphs.Length)],
                Color = colors[_rand.Next(colors.Length)]
            });
        }

        public void CycleDance()
        {
            int next = ((int)_currentDance + 1) % 4;
            _currentDance = (DanceStyle)next;

            switch (_currentDance)
            {
                case DanceStyle.Groove:
                    _badgeText = "Стимпанк-грув 🕺";
                    break;
                case DanceStyle.Kazotsky:
                    _badgeText = "Присядка (Казачок) 🪗";
                    break;
                case DanceStyle.Disco:
                    _badgeText = "Диско-лихорадка 🪩";
                    break;
                case DanceStyle.Floss:
                    _badgeText = "Флекс и рейв ⚡";
                    break;
            }

            _badgeTimer = 2.4f;

            // Взрыв нот при смене танца
            for (int i = 0; i < 4; i++)
            {
                SpawnMusicNote(burst: true);
            }
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
                if (_pitch < -25f) _pitch = -25f;
                if (_pitch > 20f) _pitch = 20f;
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
                // Если клик без заметного перемещения — переключаем танец!
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

            // 1. Мягкая бирюзовая подсветка от портала с левой стороны
            var glowRect = new RectangleF(Width * 0.15f, Height * 0.25f, Width * 0.70f, Height * 0.60f);
            using (var pathGlow = new GraphicsPath())
            {
                pathGlow.AddEllipse(glowRect);
                using (var brushGlow = new PathGradientBrush(pathGlow))
                {
                    brushGlow.CenterColor = Color.FromArgb(32, 45, 210, 185);
                    brushGlow.SurroundColors = new[] { Color.FromArgb(0, 45, 210, 185) };
                    g.FillPath(brushGlow, pathGlow);
                }
            }

            PrepareCamera();
            BuildDancingSkel();

            // 2. Реалистичная тень на каменном постаменте
            var shadowRect = new RectangleF(_cx - 8f * _s, _cy + 13.8f * _s, 16f * _s, 4.6f * _s);
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

            // 3. Сортировка полигонов по глубине (Painter's Algorithm)
            _faces.Sort((a, b) => a.Depth.CompareTo(b.Depth));

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            foreach (var face in _faces)
            {
                DrawFace(g, face);
            }

            // 4. Отрисовка парящих музыкальных нот
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using (var fNote = Theme.Font(11f, FontStyle.Bold))
            {
                foreach (var n in _notes)
                {
                    float alphaFrac = n.Life / n.MaxLife;
                    int alpha = (int)(alphaFrac * 230);
                    if (alpha > 255) alpha = 255;
                    if (alpha < 0) alpha = 0;

                    using (var bNote = new SolidBrush(Color.FromArgb(alpha, n.Color)))
                    {
                        g.DrawString(n.Glyph, fNote, bNote, n.X, n.Y);
                    }
                }
            }

            // 5. Всплывающий стильный бейдж при смене танца
            if (_badgeTimer > 0f)
            {
                float badgeAlphaFrac = Math.Min(1f, _badgeTimer / 0.5f);
                int bAlpha = (int)(badgeAlphaFrac * 240);
                if (bAlpha > 255) bAlpha = 255;

                using (var fBadge = Theme.Font(8.5f, FontStyle.Bold))
                {
                    var sz = g.MeasureString(_badgeText, fBadge);
                    float bw = sz.Width + 18f;
                    float bh = 24f;
                    float bx = (Width - bw) / 2f;
                    float by = Height * 0.08f;

                    var bRect = new RectangleF(bx, by, bw, bh);
                    using (var pathB = Theme.RoundRect(bRect, 6f))
                    {
                        using (var brushB = new SolidBrush(Color.FromArgb(bAlpha, 18, 22, 32)))
                            g.FillPath(brushB, pathB);
                        using (var penB = new Pen(Color.FromArgb(bAlpha, Theme.Gold), 1.2f))
                            g.DrawPath(penB, pathB);
                    }

                    using (var brushTxt = new SolidBrush(Color.FromArgb(bAlpha, Color.White)))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(_badgeText, fBadge, brushTxt, bRect, sf);
                    }
                }
            }
            else if (_isHovered && !_dragging)
            {
                // Подсказка при наведении
                using (var fHint = Theme.Font(7.5f, FontStyle.Bold))
                using (var bHint = new SolidBrush(Color.FromArgb(180, 200, 215, 235)))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center })
                {
                    g.DrawString("Нажмите, чтобы сменить танец 🎵", fHint, bHint, Width / 2f, Height * 0.08f, sf);
                }
            }
        }

        private void PrepareCamera()
        {
            float available = Math.Max(40f, Height - 36f);
            _s = Math.Min(Width / 20f, available / 35f) * 1.18f * _zoom;
            _cx = Width * 0.5f;
            _cy = Height * 0.52f + 1f * _s;

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

            // Pitch (вокруг X)
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

            // Yaw (вокруг Y)
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

            // Roll (вокруг Z)
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

        private void AddFace(V3 p0, V3 p1, V3 p2, V3 p3, RectangleF uv, int shade, Bitmap customTex = null)
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

            // Backface culling
            float cross = (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
            if (cross <= 0.0001f) return;

            _faces.Add(new Face
            {
                A = a, B = b, C = c, D = d,
                Uv = uv,
                Depth = (d0 + d1 + d2 + d3) * 0.25f,
                Shade = shade,
                CustomTexture = customTex
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
            float capePitch = -12f, capeRoll = 0;

            switch (_currentDance)
            {
                case DanceStyle.Groove:
                    // Стимпанк-грув: пружинящий такт, кач руками и бедрами
                    float bounce = (float)Math.Abs(Math.Sin(t * 6.2)) * 2.2f;
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

                    capePitch = -15.0f - bounce * 7.0f;
                    capeRoll = -(float)Math.Sin(t * 3.1) * 10.0f;
                    break;

                case DanceStyle.Kazotsky:
                    // Присядка / Казачок: глубокий присед и выбрасывание ног!
                    float kKick = (float)Math.Sin(t * 7.5);
                    float kBounce = (float)Math.Abs(Math.Sin(t * 7.5)) * 1.6f;
                    rootY = -5.5f + kBounce;

                    // Скрещенные руки на груди
                    rArmPitch = -78.0f;
                    rArmRoll = 42.0f;
                    rArmYaw = 38.0f;

                    lArmPitch = -78.0f;
                    lArmRoll = -42.0f;
                    lArmYaw = -38.0f;

                    bodyPitch = 6.0f;
                    headPitch = -12.0f + kBounce * 6.0f;
                    headRoll = (float)Math.Sin(t * 3.75) * 6.0f;

                    if (kKick > 0)
                    {
                        rLegPitch = -75.0f * kKick;
                        lLegPitch = 40.0f * kKick;
                    }
                    else
                    {
                        float inv = -kKick;
                        lLegPitch = -75.0f * inv;
                        rLegPitch = 40.0f * inv;
                    }

                    capePitch = -35.0f - kBounce * 15.0f;
                    break;

                case DanceStyle.Disco:
                    // Диско: рука на бедре, вторая стреляет в портал
                    lArmPitch = 22.0f;
                    lArmRoll = -38.0f;
                    lArmYaw = -12.0f;

                    float discoPoint = (float)Math.Cos(t * 4.2);
                    rArmPitch = -110.0f + discoPoint * 60.0f;
                    rArmRoll = 24.0f + (float)Math.Sin(t * 4.2) * 18.0f;

                    bodyRoll = (float)Math.Sin(t * 4.2) * 10.0f;
                    bodyPitch = -4.0f + discoPoint * 7.0f;

                    headPitch = -15.0f - discoPoint * 12.0f;
                    headYaw = 12.0f + (float)Math.Sin(t * 4.2) * 14.0f;

                    rLegPitch = (float)Math.Sin(t * 4.2) * 18.0f;
                    lLegPitch = -(float)Math.Sin(t * 4.2) * 8.0f;

                    capePitch = -18.0f;
                    capeRoll = (float)Math.Sin(t * 4.2) * 12.0f;
                    break;

                case DanceStyle.Floss:
                    // Волна и флекс
                    float flossPhase = (float)Math.Sin(t * 7.5);
                    rootX = flossPhase * 2.8f;
                    bodyRoll = -flossPhase * 8.0f;
                    bodyYaw = flossPhase * 12.0f;

                    float armSwing = -flossPhase * 55.0f;
                    rArmYaw = armSwing;
                    rArmPitch = 25.0f + (float)Math.Abs(Math.Cos(t * 7.5)) * 14.0f;

                    lArmYaw = armSwing;
                    lArmPitch = 25.0f + (float)Math.Abs(Math.Cos(t * 7.5)) * 14.0f;

                    headRoll = flossPhase * 4.0f;
                    headPitch = -4.0f + (float)Math.Abs(flossPhase) * 6.0f;

                    rLegRoll = -flossPhase * 6.0f;
                    lLegRoll = -flossPhase * 6.0f;

                    capePitch = -15.0f;
                    capeRoll = armSwing * 0.35f;
                    break;
            }

            // Интерактивное слежение взглядом за курсором мыши
            if (_isHovered && !_dragging)
            {
                float dx = (_mousePos.X - (Width * 0.5f)) / (Width * 0.5f);
                float dy = (_mousePos.Y - (Height * 0.35f)) / (Height * 0.5f);
                headYaw += dx * 16.0f;
                headPitch += dy * 12.0f;
            }

            // ─── 1. Торс (Body) ───
            var tBody = new Transform(0, 12, 0, bodyPitch, bodyYaw, bodyRoll, rootX, rootY, rootZ);
            AddTransformedBox(-4, 12, -2, 4, 24, 2, tBody,
                Uv(20, 20, 8, 12), Uv(16, 20, 4, 12), Uv(28, 20, 4, 12), Uv(32, 20, 8, 12),
                Uv(20, 16, 8, 4), Uv(28, 16, 8, 4));

            // Куртка (Layer 2)
            AddTransformedBox(-4.2f, 11.8f, -2.2f, 4.2f, 24.2f, 2.2f, tBody,
                Uv(20, 36, 8, 12), Uv(16, 36, 4, 12), Uv(28, 36, 4, 12), Uv(32, 36, 8, 12),
                Uv(20, 32, 8, 4), Uv(28, 32, 8, 4));

            // ─── 2. Голова (Head) ───
            var tHead = new Transform(0, 24, 0, headPitch + bodyPitch, headYaw + bodyYaw, headRoll + bodyRoll, rootX, rootY, rootZ);
            AddTransformedBox(-4, 24, -4, 4, 32, 4, tHead,
                Uv(8, 8, 8, 8), Uv(0, 8, 8, 8), Uv(16, 8, 8, 8), Uv(24, 8, 8, 8),
                Uv(8, 0, 8, 8), Uv(16, 0, 8, 8));

            // Шапка / Волосы (Layer 2)
            AddTransformedBox(-4.35f, 23.65f, -4.35f, 4.35f, 32.35f, 4.35f, tHead,
                Uv(40, 8, 8, 8), Uv(32, 8, 8, 8), Uv(48, 8, 8, 8), Uv(56, 8, 8, 8),
                Uv(40, 0, 8, 8), Uv(48, 0, 8, 8));

            // ─── 3. Правая рука (Right Arm) ───
            float rShoulderX = -4f - (arm / 2f);
            var tRArm = new Transform(rShoulderX, 22, 0, rArmPitch, rArmYaw, rArmRoll, rootX, rootY, rootZ);
            AddTransformedBox(-4 - arm, 12, -2, -4, 24, 2, tRArm,
                Uv(44, 20, 4, 12), Uv(40, 20, 4, 12), Uv(48, 20, 4, 12), Uv(52, 20, 4, 12),
                Uv(44, 16, 4, 4), Uv(48, 16, 4, 4));

            // Рукав правый (Layer 2)
            AddTransformedBox(-4.2f - arm, 11.8f, -2.2f, -3.8f, 24.2f, 2.2f, tRArm,
                Uv(44, 36, 4, 12), Uv(40, 36, 4, 12), Uv(48, 36, 4, 12), Uv(52, 36, 4, 12),
                Uv(44, 32, 4, 4), Uv(48, 32, 4, 4));

            // ─── 4. Левая рука (Left Arm) ───
            float lShoulderX = 4f + (arm / 2f);
            var tLArm = new Transform(lShoulderX, 22, 0, lArmPitch, lArmYaw, lArmRoll, rootX, rootY, rootZ);
            AddTransformedBox(4, 12, -2, 4 + arm, 24, 2, tLArm,
                Uv(36, 52, 4, 12), Uv(32, 52, 4, 12), Uv(40, 52, 4, 12), Uv(44, 52, 4, 12),
                Uv(36, 48, 4, 4), Uv(40, 48, 4, 4));

            // Рукав левый (Layer 2)
            AddTransformedBox(3.8f, 11.8f, -2.2f, 4.2f + arm, 24.2f, 2.2f, tLArm,
                Uv(52, 52, 4, 12), Uv(48, 52, 4, 12), Uv(56, 52, 4, 12), Uv(60, 52, 4, 12),
                Uv(52, 48, 4, 4), Uv(56, 48, 4, 4));

            // ─── 5. Правая нога (Right Leg) ───
            var tRLeg = new Transform(-2, 12, 0, rLegPitch, rLegYaw, rLegRoll, rootX * 0.3f, rootY, rootZ);
            AddTransformedBox(-4, 0, -2, 0, 12, 2, tRLeg,
                Uv(4, 20, 4, 12), Uv(0, 20, 4, 12), Uv(8, 20, 4, 12), Uv(12, 20, 4, 12),
                Uv(4, 16, 4, 4), Uv(8, 16, 4, 4));

            // Штанина правая (Layer 2)
            AddTransformedBox(-4.2f, -0.2f, -2.2f, 0.2f, 12.2f, 2.2f, tRLeg,
                Uv(4, 36, 4, 12), Uv(0, 36, 4, 12), Uv(8, 36, 4, 12), Uv(12, 36, 4, 12),
                Uv(4, 32, 4, 4), Uv(8, 32, 4, 4));

            // ─── 6. Левая нога (Left Leg) ───
            var tLLeg = new Transform(2, 12, 0, lLegPitch, lLegYaw, lLegRoll, rootX * 0.3f, rootY, rootZ);
            AddTransformedBox(0, 0, -2, 4, 12, 2, tLLeg,
                Uv(20, 52, 4, 12), Uv(16, 52, 4, 12), Uv(24, 52, 4, 12), Uv(28, 52, 4, 12),
                Uv(20, 48, 4, 4), Uv(24, 48, 4, 4));

            // Штанина левая (Layer 2)
            AddTransformedBox(-0.2f, -0.2f, -2.2f, 4.2f, 12.2f, 2.2f, tLLeg,
                Uv(4, 52, 4, 12), Uv(0, 52, 4, 12), Uv(8, 52, 4, 12), Uv(12, 52, 4, 12),
                Uv(4, 48, 4, 4), Uv(8, 48, 4, 4));

            // ─── 7. Плащ (Steampunk Cape) ───
            AddAnimatedCape(capePitch, capeRoll, rootX, rootY, rootZ);
        }

        private void AddAnimatedCape(float pitch, float roll, float rootX, float rootY, float rootZ)
        {
            if (_cape == null) return;

            var tCape = new Transform(0, 24, -2, pitch, 0, roll, rootX, rootY, rootZ);

            float x0 = -5f, x1 = 5f;
            float y0 = 8f, y1 = 24f;
            float z0_top = -2.8f, z1_top = -2.0f;
            float z0_bot = -4.4f, z1_bot = -3.6f;

            V3 p0 = ApplyTransform(new V3(x0, y1, z1_top), tCape);
            V3 p1 = ApplyTransform(new V3(x1, y1, z1_top), tCape);
            V3 p2 = ApplyTransform(new V3(x1, y0, z1_bot), tCape);
            V3 p3 = ApplyTransform(new V3(x0, y0, z1_bot), tCape);

            V3 p4 = ApplyTransform(new V3(x1, y1, z0_top), tCape);
            V3 p5 = ApplyTransform(new V3(x0, y1, z0_top), tCape);
            V3 p6 = ApplyTransform(new V3(x0, y0, z0_bot), tCape);
            V3 p7 = ApplyTransform(new V3(x1, y0, z0_bot), tCape);

            // Front
            AddFace(p0, p1, p2, p3, Uv(1, 1, 10, 16), 25, _cape);
            // Back
            AddFace(p4, p5, p6, p7, Uv(12, 1, 10, 16), 55, _cape);
            // Left
            AddFace(p1, p4, p7, p2, Uv(0, 1, 1, 16), 35, _cape);
            // Right
            AddFace(p5, p0, p3, p6, Uv(11, 1, 1, 16), 40, _cape);
            // Top
            AddFace(p5, p4, p1, p0, Uv(1, 0, 10, 1), 15, _cape);
            // Bottom
            AddFace(p3, p2, p7, p6, Uv(11, 0, 10, 1), 75, _cape);
        }

        private void DrawFace(Graphics g, Face face)
        {
            var uv = face.Uv;
            if (uv.Width < 0.01f || uv.Height < 0.01f) return;

            var bitmap = face.CustomTexture ?? _skin ?? _placeholder;
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

        private static Bitmap CreateSteampunkCape()
        {
            var bmp = new Bitmap(64, 32, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);

                using (var brush = new SolidBrush(Color.FromArgb(24, 26, 34)))
                {
                    g.FillRectangle(brush, 0, 0, 24, 18);
                }

                using (var brush = new SolidBrush(Color.FromArgb(58, 20, 28)))
                {
                    g.FillRectangle(brush, 1, 1, 10, 16);
                }

                using (var border = new Pen(Color.FromArgb(245, 166, 35), 1))
                {
                    g.DrawRectangle(border, 12, 1, 9, 15);
                }

                using (var gold = new SolidBrush(Color.FromArgb(245, 166, 35)))
                {
                    g.FillEllipse(gold, 15, 6, 4, 4);
                    g.FillRectangle(gold, 14, 7, 6, 2);
                    g.FillRectangle(gold, 16, 5, 2, 6);
                }
                using (var centerHole = new SolidBrush(Color.FromArgb(24, 26, 34)))
                {
                    g.FillEllipse(centerHole, 16, 7, 2, 2);
                }
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
                if (_cape != null) _cape.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
