using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ODL
{
    public class Sprite : ISprite, IDisposable
    {
        /// <summary>
        /// The debug name of the sprite.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The viewport associated with the sprite.
        /// </summary>
        public Viewport Viewport { get; protected set; }
        private Rect _SrcRect;
        /// <summary>
        /// The rectangle within the bitmap displayed when rendered.
        /// </summary>
        public Rect SrcRect { get { return _SrcRect; } set { this._SrcRect = value; Viewport.Update(); } }
        private Bitmap _Bitmap;
        /// <summary>
        /// The bitmap associated with the sprite.
        /// </summary>
        public Bitmap Bitmap
        {
            get { return _Bitmap; }
            set {
                this._Bitmap = value;
                value.Renderer = this.Viewport.Renderer;
                value.RecreateTexture();
                this.SrcRect = new Rect(value.Width, value.Height);
            }
        }
        private int _X = 0;
        /// <summary>
        /// The x position of the sprite.
        /// </summary>
        public int X { get { return _X; } set { this._X = value; Viewport.Update(); } }
        private int _Y = 0;
        /// <summary>
        /// The y position of the sprite.
        /// </summary>
        public int Y { get { return _Y; } set { this._Y = value; Viewport.Update(); } }
        private int _Z = 0;
        /// <summary>
        /// The z index of the sprite.
        /// </summary>
        public int Z { get { return _Z; } set { this._Z = value; Viewport.Update(); } }
        private double _ZoomX = 1;
        /// <summary>
        /// The horizontal zoom factor of the sprite.
        /// </summary>
        public double ZoomX { get { return _ZoomX; } set { this._ZoomX = value; Viewport.Update(); } }
        private double _ZoomY = 1;
        /// <summary>
        /// The vertical zoom factor of the sprite.
        /// </summary>
        public double ZoomY { get { return _ZoomY; } set { this._ZoomY = value; Viewport.Update(); } }
        /// <summary>
        /// Whether the sprite is disposed.
        /// </summary>
        public bool Disposed { get; protected set; } = false;
        private bool _Visible = true;
        /// <summary>
        /// Whether the sprite is visible.
        /// </summary>
        public bool Visible { get { return _Visible; } set { this._Visible = value; Viewport.Update(); } }
        private int _Angle = 0;
        /// <summary>
        /// The angle at which the sprite is rendered.
        /// </summary>
        public int Angle { get { return _Angle; } set { this._Angle = value; Viewport.Update(); } }
        private bool _MirrorX = false;
        /// <summary>
        /// Whether the sprite is mirrored horizontally.
        /// </summary>
        public bool MirrorX { get { return _MirrorX; } set { this._MirrorX = value; Viewport.Update(); } }
        private bool _MirrorY = false;
        /// <summary>
        /// Whether the sprite is mirrored vertically.
        /// </summary>
        public bool MirrorY { get { return _MirrorY; } set { this._MirrorY = value; Viewport.Update(); } }
        private int _OX = 0;
        /// <summary>
        /// The origin x position of the sprite.
        /// </summary>
        public int OX { get { return _OX; } set { this._OX = value; Viewport.Update(); } }
        private int _OY = 0;
        /// <summary>
        /// The origin y position of the sprite.
        /// </summary>
        public int OY { get { return _OY; } set { this._OY = value; Viewport.Update(); } }
        private Color _Color = new Color(255, 255, 255, 255);
        /// <summary>
        /// The color of the sprite.
        /// </summary>
        public Color Color { get { return _Color; } set { this._Color = value; Viewport.Update(); } }
        /// <summary>
        /// The timestamp at which the sprite was created.
        /// </summary>
        public long TimeCreated = ((10000L * Stopwatch.GetTimestamp()) / TimeSpan.TicksPerMillisecond) / 100L;
        private List<Point> _MultiplePositions = new List<Point>();
        /// <summary>
        /// The list of additional positions to render the sprite at.
        /// </summary>
        public List<Point> MultiplePositions { get { return _MultiplePositions; } set { this._MultiplePositions = value; Viewport.Update(); } }
        private byte _Opacity = 255;
        /// <summary>
        /// The opacity at which the sprite is rendered.
        /// </summary>
        public byte Opacity { get { return _Opacity; } set { this._Opacity = value; Viewport.Update(); } }

        public Sprite(Viewport Viewport, string Filename)
            : this(Viewport)
        {
            this.Bitmap = new Bitmap(Filename);
            this.SrcRect = new Rect(this.Bitmap.Width, this.Bitmap.Height);
        }

        public Sprite(Viewport Viewport, Size Size)
            : this(Viewport, Size.Width, Size.Height) { }
        public Sprite(Viewport Viewport, int Width, int Height)
            : this(Viewport)
        {
            this.Bitmap = new Bitmap(Width, Height);
            this.SrcRect = new Rect(this.Bitmap.Width, this.Bitmap.Height);
        }

        public Sprite(Viewport Viewport, Bitmap bmp)
            : this(Viewport)
        {
            this.Bitmap = bmp;
            this.SrcRect = new Rect(this.Bitmap.Width, this.Bitmap.Height);
        }

        public Sprite(Viewport Viewport)
        {
            this.Viewport = Viewport;
            this.Viewport.Sprites.Add(this);
            this.Viewport.Update();
        }

        /// <summary>
        /// Forces the Renderer to redraw.
        /// </summary>
        public void Update()
        {
            this.Viewport.Update();
        }

        /// <summary>
        /// Disposes the sprite and its bitmap.
        /// </summary>
        public void Dispose()
        {
            if (this.Bitmap != null) this.Bitmap.Dispose();
            this.Disposed = true;
            this.Viewport.Sprites.Remove(this);
            this.Viewport.Update();
        }
    }
}
