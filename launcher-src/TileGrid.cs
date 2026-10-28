using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    /// <summary>Scrollable grid of rounded tiles with an image and a caption.</summary>
    public class TileGrid : Control
    {
        public class Tile
        {
            public string Title;
            public string Subtitle;
            public Image Image;
            public object Tag;
            public bool Busy;
        }

        public readonly List<Tile> Tiles = new List<Tile>();
        public int TileWidth = 140;
        public int TileHeight = 236;
        public int Gap = 14;
        public string EmptyText = "Пока пусто";
        public event EventHandler<Tile> TileActivated;

        private int _hover = -1;
        private int _scroll;
        private readonly Font _titleFont = Theme.Font(8.75f, FontStyle.Bold);
        private readonly Font _subFont = Theme.Font(7.75f);

        public TileGrid()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.Bg;
        }

        public void Reset()
        {
            _scroll = 0;
            _hover = -1;
            Invalidate();
        }

        private int Columns
        {
            get { return Math.Max(1, (Width - Gap) / Math.Max(1, TileWidth + Gap)); }
        }

        private int ContentHeight
        {
            get
            {
                int rows = (Tiles.Count + Columns - 1) / Columns;
                return rows * (TileHeight + Gap) - Gap + Gap;
            }
        }

        private int MaxScroll
        {
            get { return Math.Max(0, ContentHeight - Height); }
        }

        private Rectangle TileRect(int index)
        {
            int cols = Columns;
            int col = index % cols;
            int row = index / cols;
            return new Rectangle(
                Gap / 2 + col * (TileWidth + Gap),
                Gap / 2 + row * (TileHeight + Gap) - _scroll,
                TileWidth, TileHeight);
        }

        private int IndexAt(Point point)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (TileRect(i).Contains(point)) return i;
            }
            return -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Theme.PaintControlBackground(g, this);

            if (Tiles.Count == 0)
            {
                Theme.PaintText(g, EmptyText, Theme.Font(10f), Theme.Faint,
                    new RectangleF(0, 0, Width, Height), ContentAlignment.MiddleCenter);
                return;
            }

            for (int i = 0; i < Tiles.Count; i++)
            {
                var rect = TileRect(i);
                if (rect.Bottom < -10) continue;
                if (rect.Top > Height + 10) break;

                var tile = Tiles[i];
                bool hover = i == _hover;

                var card = new RectangleF(rect.X + 0.5f, rect.Y + 0.5f, rect.Width - 1f, rect.Height - 1f);
                using (var path = Theme.RoundRect(card, 14f))
                using (var brush = new SolidBrush(hover ? Theme.CardAlt : Theme.Field))
                using (var pen = new Pen(hover ? Theme.GoldSoft : Theme.BorderSoft, hover ? 1.6f : 1f))
                {
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }

                int captionH = tile.Subtitle == null ? 26 : 36;
                var imageArea = new RectangleF(rect.X + 10, rect.Y + 10,
                    rect.Width - 20, rect.Height - captionH - 14);

                if (tile.Image != null)
                {
                    float scale = Math.Min(imageArea.Width / tile.Image.Width, imageArea.Height / tile.Image.Height);
                    float w = tile.Image.Width * scale;
                    float h = tile.Image.Height * scale;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(tile.Image, imageArea.X + (imageArea.Width - w) / 2f,
                        imageArea.Y + (imageArea.Height - h) / 2f, w, h);
                    g.InterpolationMode = InterpolationMode.Default;
                }
                else
                {
                    Theme.PaintText(g, tile.Busy ? "⏳ Загрузка..." : "👤 Скин", _subFont, Theme.Faint, imageArea,
                        ContentAlignment.MiddleCenter);
                }

                var titleRect = new RectangleF(rect.X + 8, rect.Bottom - captionH, rect.Width - 16,
                    tile.Subtitle == null ? 24 : 18);
                Theme.PaintText(g, tile.Title, _titleFont, hover ? Theme.Text : Theme.TextDim,
                    titleRect, ContentAlignment.MiddleCenter);

                if (tile.Subtitle != null)
                {
                    Theme.PaintText(g, tile.Subtitle, _subFont, Theme.Faint,
                        new RectangleF(rect.X + 8, rect.Bottom - 20, rect.Width - 16, 16),
                        ContentAlignment.MiddleCenter);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int index = IndexAt(e.Location);
            if (index != _hover)
            {
                _hover = index;
                Cursor = index >= 0 ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_hover != -1)
            {
                _hover = -1;
                Cursor = Cursors.Default;
                Invalidate();
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int index = IndexAt(e.Location);
                if (index >= 0 && index < Tiles.Count)
                {
                    var handler = TileActivated;
                    if (handler != null) handler(this, Tiles[index]);
                }
            }
            base.OnMouseClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (MaxScroll > 0)
            {
                _scroll -= (e.Delta / 120) * 90;
                if (_scroll < 0) _scroll = 0;
                if (_scroll > MaxScroll) _scroll = MaxScroll;
                Invalidate();
            }
            base.OnMouseWheel(e);
        }
    }
}
