using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ODL
{
    public class Sprite : ISprite, IDisposable
    {
        // Sprites have a Name property to keep track of which Sprite is which -- totally optional, of course
        public string Name { get; set; }
        public Viewport Viewport { get; protected set; }
        public string Filename { get; }
        private Rect _SrcRect;
        public Rect SrcRect { get { return _SrcRect; } set { this._SrcRect = value; this.Viewport.ForceUpdate(); } }
        private IBitmap _Bitmap;
        public IBitmap Bitmap
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
        public int X { get { return _X; } set { this._X = value; this.Viewport.ForceUpdate(); } }
        private int _Y = 0;
        public int Y { get { return _Y; } set { this._Y = value; this.Viewport.ForceUpdate(); } }
        private int _Z = 0;
        public int Z { get { return _Z; } set { this._Z = value; this.Viewport.ForceUpdate(); } }
        private double _ZoomX = 1;
        public double ZoomX { get { return _ZoomX; } set { this._ZoomX = value; this.Viewport.ForceUpdate(); } }
        private double _ZoomY = 1;
        public double ZoomY { get { return _ZoomY; } set { this._ZoomY = value; this.Viewport.ForceUpdate(); } }
        public bool Disposed { get; protected set; } = false;
        private bool _Visible = true;
        public bool Visible { get { return _Visible; } set { this._Visible = value; this.Viewport.ForceUpdate(); } }
        private int _Angle = 0;
        public int Angle { get { return _Angle; } set { this._Angle = value; this.Viewport.ForceUpdate(); } }
        private bool _MirrorX = false;
        public bool MirrorX { get { return _MirrorX; } set { this._MirrorX = value; this.Viewport.ForceUpdate(); } }
        private bool _MirrorY = false;
        public bool MirrorY { get { return _MirrorY; } set { this._MirrorY = value; this.Viewport.ForceUpdate(); } }
        private int _OX = 0;
        public int OX { get { return _OX; } set { this._OX = value; this.Viewport.ForceUpdate(); } }
        private int _OY = 0;
        public int OY { get { return _OY; } set { this._OY = value; this.Viewport.ForceUpdate(); } }
        private Color _Color = new Color(255, 255, 255, 255);
        public Color Color { get { return _Color; } set { this._Color = value; this.Viewport.ForceUpdate(); } }
        public long TimeCreated = ((10000L * Stopwatch.GetTimestamp()) / TimeSpan.TicksPerMillisecond) / 100L;
        private List<Point> _MultiplePositions = new List<Point>();
        public List<Point> MultiplePositions { get { return _MultiplePositions; } set { this._MultiplePositions = value; this.Viewport.ForceUpdate(); } }
        private byte _Opacity = 255;
        public byte Opacity { get { return _Opacity; } set { this._Opacity = value; this.Viewport.ForceUpdate(); } }

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

        public Sprite(Viewport Viewport, IBitmap bmp)
            : this(Viewport)
        {
            this.Bitmap = bmp;
            this.SrcRect = new Rect(this.Bitmap.Width, this.Bitmap.Height);
        }

        public Sprite(Viewport Viewport)
        {
            this.Viewport = Viewport;
            this.Viewport.Sprites.Add(this);
            this.Viewport.ForceUpdate();
        }

        public void Update()
        {
            this.Viewport.Update();
        }

        public void Dispose()
        {
            if (this.Bitmap != null) this.Bitmap.Dispose();
            this.Disposed = true;
            this.Viewport.Sprites.Remove(this);
            this.Viewport.ForceUpdate();
        }
    }
}
