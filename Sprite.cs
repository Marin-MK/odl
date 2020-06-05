using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace odl
{
    public class Sprite : ISprite, IDisposable
    {
        /// <summary>
        /// The default viewport assigned if no viewport is specified.
        /// </summary>
        public static Viewport DefaultViewport;

        /// <summary>
        /// The debug name of the sprite.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The viewport associated with the sprite.
        /// </summary>
        public Viewport Viewport { get; protected set; }
        private Rect _SrcRect = new Rect(0, 0, 0, 0);
        /// <summary>
        /// The rectangle within the bitmap displayed when rendered.
        /// </summary>
        public Rect SrcRect { get { return _SrcRect; } set { if (value != _SrcRect) Viewport.Update(); _SrcRect = value; } }
        private Bitmap _Bitmap;
        /// <summary>
        /// The bitmap associated with the sprite.
        /// </summary>
        public Bitmap Bitmap
        {
            get { return _Bitmap; }
            set {
                this._Bitmap = value;
                if (value != null)
                {
                    value.Renderer = this.Viewport.Renderer;
                    if (value is LargeBitmap)
                    {
                        if (value.Locked) value.Unlock();
                        value.Lock();
                    }
                    else value.RecreateTexture();
                    this.SrcRect = new Rect(value.Width, value.Height);
                }
            }
        }
        private int _X = 0;
        /// <summary>
        /// The x position of the sprite.
        /// </summary>
        public int X { get { return _X; } set { if (value != _X) Viewport.Update(); _X = value; } }
        private int _Y = 0;
        /// <summary>
        /// The y position of the sprite.
        /// </summary>
        public int Y { get { return _Y; } set { if (value != _Y) Viewport.Update(); _Y = value; } }
        private int _Z = 0;
        /// <summary>
        /// The z index of the sprite.
        /// </summary>
        public int Z { get { return _Z; } set { if (value != _Z) Viewport.Update(); _Z = value; } }
        private double _ZoomX = 1;
        /// <summary>
        /// The horizontal zoom factor of the sprite.
        /// </summary>
        public double ZoomX { get { return _ZoomX; } set { if (value != _ZoomX) Viewport.Update(); _ZoomX = value; } }
        private double _ZoomY = 1;
        /// <summary>
        /// The vertical zoom factor of the sprite.
        /// </summary>
        public double ZoomY { get { return _ZoomY; } set { if (value != _ZoomY) Viewport.Update(); _ZoomY = value; } }
        /// <summary>
        /// Whether the sprite is disposed.
        /// </summary>
        public bool Disposed { get; protected set; } = false;
        private bool _Visible = true;
        /// <summary>
        /// Whether the sprite is visible.
        /// </summary>
        public bool Visible { get { return _Visible; } set { if (value != _Visible) Viewport.Update(); _Visible = value; } }
        private int _Angle = 0;
        /// <summary>
        /// The angle at which the sprite is rendered.
        /// </summary>
        public int Angle { get { return _Angle; } set { if (value != _Angle) Viewport.Update(); _Angle = value; } }
        private bool _MirrorX = false;
        /// <summary>
        /// Whether the sprite is mirrored horizontally.
        /// </summary>
        public bool MirrorX { get { return _MirrorX; } set { if (value != _MirrorX) Viewport.Update(); _MirrorX = value; } }
        private bool _MirrorY = false;
        /// <summary>
        /// Whether the sprite is mirrored vertically.
        /// </summary>
        public bool MirrorY { get { return _MirrorY; } set { if (value != _MirrorY) Viewport.Update(); _MirrorY = value; } }
        private int _OX = 0;
        /// <summary>
        /// The origin x position of the sprite.
        /// </summary>
        public int OX { get { return _OX; } set { if (value != _OX) Viewport.Update(); _OX = value; } }
        private int _OY = 0;
        /// <summary>
        /// The origin y position of the sprite.
        /// </summary>
        public int OY { get { return _OY; } set { if (value != _OY) Viewport.Update(); _OY = value; } }
        private Color _Color = new Color(255, 255, 255, 0);
        /// <summary>
        /// The color of the sprite.
        /// </summary>
        public Color Color { get { return _Color; } set { if (value != _Color) Viewport.Update(); _Color = value; } }
        private Tone _Tone = new Tone(0, 0, 0, 0);
        /// <summary>
        /// The tone of the sprite.
        /// </summary>
        public Tone Tone { get { return _Tone; } set { if (value != _Tone) Viewport.Update(); _Tone = value; } }
        /// <summary>
        /// The timestamp at which the sprite was created.
        /// </summary>
        public long TimeCreated = ((10000L * Stopwatch.GetTimestamp()) / TimeSpan.TicksPerMillisecond) / 100L;
        private List<Point> _MultiplePositions = new List<Point>();
        /// <summary>
        /// The list of additional positions to render the sprite at.
        /// </summary>
        public List<Point> MultiplePositions { get { return _MultiplePositions; } set { if (value != _MultiplePositions) Viewport.Update(); _MultiplePositions = value; } }
        private byte _Opacity = 255;
        /// <summary>
        /// The opacity at which the sprite is rendered.
        /// </summary>
        public byte Opacity { get { return _Opacity; } set { if (value != _Opacity) Viewport.Update(); _Opacity = value; } }
        /// <summary>
        /// Whether to destroy the bitmap upon disposal alongside the sprite.
        /// </summary>
        public bool DestroyBitmap { get; set; } = true;

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

        public Sprite()
            : this(DefaultViewport) { }

        public Sprite(Sprite Copy)
        {
            this.Viewport = Copy.Viewport;
            this.Name = Copy.Name;
            this.SrcRect = new Rect(Copy.SrcRect.SDL_Rect);
            this.Bitmap = Copy.Bitmap;
            this.X = Copy.X;
            this.Y = Copy.Y;
            this.Z = Copy.Z;
            this.ZoomX = Copy.ZoomX;
            this.ZoomY = Copy.ZoomY;
            this.Disposed = Copy.Disposed;
            this.Visible = Copy.Visible;
            this.Angle = Copy.Angle;
            this.MirrorX = Copy.MirrorX;
            this.MirrorY = Copy.MirrorY;
            this.OX = Copy.OX;
            this.OY = Copy.OY;
            this.Color = Copy.Color;
            this.TimeCreated = Copy.TimeCreated;
            this.MultiplePositions = new List<Point>(Copy.MultiplePositions);
            this.Opacity = Copy.Opacity;
        }

        ~Sprite()
        {
            if (!Disposed)
            {
                Console.WriteLine($"An undisposed sprite is being collected by the GC! This is likely a memory leak!");
            }
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
            if (this.DestroyBitmap && this.Bitmap != null) this.Bitmap.Dispose();
            this.Disposed = true;
            if (this.Viewport.Sprites != null) this.Viewport.Sprites.Remove(this);
            this.Viewport.Update();
        }
    }
}
