using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ODL
{
    public class Viewport : IDisposable
    {
        /// <summary>
        /// The default renderer assigned if no renderer is specified.
        /// </summary>
        public static Renderer DefaultRenderer;

        /// <summary>
        /// The debug name of the viewport.
        /// </summary>
        public string Name;
        /// <summary>
        /// The Renderer associated with this viewport.
        /// </summary>
        public Renderer Renderer;
        /// <summary>
        /// The sprites inside the viewport.
        /// </summary>
        public List<Sprite> Sprites = new List<Sprite>();
        private int _X = 0;
        /// <summary>
        /// The x position of the viewport.
        /// </summary>
        public int X { get { return _X; } set { if (value != _X) Update(); _X = value; } }
        private int _Y = 0;
        /// <summary>
        /// The y position of the viewport.
        /// </summary>
        public int Y { get { return _Y; } set { if (value != _Y) Update(); _Y = value; } }
        private int _Z = 0;
        /// <summary>
        /// The z index of the viewport.
        /// </summary>
        public int Z { get { return _Z; } set { if (value != _Z) Update(); _Z = value; } }
        private int _OX = 0;
        /// <summary>
        /// The x offset of the viewport.
        /// </summary>
        public int OX { get { return _OX; } set { if (value != _OX) Update(); _OX = value; } }
        private int _OY = 0;
        /// <summary>
        /// The y offset of the viewport.
        /// </summary>
        public int OY { get { return _OY; } set { if (value != _OY) Update(); _OY = value; } }
        /// <summary>
        /// The width of the viewport.
        /// </summary>
        public int Width = -1;
        /// <summary>
        /// The height of the viewport.
        /// </summary>
        public int Height = -1;
        /// <summary>
        /// The rectangle created by the viewport.
        /// </summary>
        public Rect Rect { get { return new Rect(X, Y, Width, Height); } }
        /// <summary>
        /// Whether the viewport is disposed.
        /// </summary>
        public bool Disposed { get; protected set; } = false;
        private bool _Visible = true;
        /// <summary>
        /// Whether the viewport is visible.
        /// </summary>
        public bool Visible { get { return _Visible; } set { if (value != _Visible) Update(); _Visible = value; } }
        private float _ZoomX = 1;
        /// <summary>
        /// The horizontal zoom factor of the viewport.
        /// </summary>
        public float ZoomX { get { return _ZoomX; } set { if (value != _ZoomX) Update(); _ZoomX = value; } }
        private float _ZoomY = 1;
        /// <summary>
        /// The vertical zoom factor of the viewport.
        /// </summary>
        public float ZoomY { get { return _ZoomY; } set { if (value != _ZoomY) Update(); _ZoomY = value; } }
        private Color _Color = new Color(255, 255, 255);
        /// <summary>
        /// The color effect applied to this viewport.
        /// </summary>
        public Color Color { get { return _Color; } set { if (value != _Color) Update(); _Color = value; } }
        /// <summary>
        /// The timestamp at which the viewport was created.
        /// </summary>
        public long TimeCreated = ((10000L * Stopwatch.GetTimestamp()) / TimeSpan.TicksPerMillisecond) / 100L;

        #region Constructor Overloads
        public Viewport(int X, int Y, int Width, int Height)
            : this(DefaultRenderer, X, Y, Width, Height) { }
        public Viewport(Point p, Size s)
            : this(DefaultRenderer, p.X, p.Y, s.Width, s.Height) { }
        public Viewport(Point p, int Width, int Height)
            : this(DefaultRenderer, p.X, p.Y, Width, Height) { }
        public Viewport(int X, int Y, Size s)
            : this(DefaultRenderer, X, Y, s.Width, s.Height) { }
        public Viewport(Rect rect)
            : this(DefaultRenderer, rect.X, rect.Y, rect.Width, rect.Height) { }
        public Viewport(Renderer Renderer, Point p, Size s)
            : this(Renderer, p.X, p.Y, s.Width, s.Height) { }
        public Viewport(Renderer Renderer, Point p, int Width, int Height)
            : this(Renderer, p.X, p.Y, Width, Height) { }
        public Viewport(Renderer Renderer, int X, int Y, Size s)
            : this(Renderer, X, Y, s.Width, s.Height) { }
        public Viewport(Renderer Renderer, Rect rect)
            : this(Renderer, rect.X, rect.Y, rect.Width, rect.Height) { }
        #endregion
        public Viewport(Renderer Renderer, int X, int Y, int Width, int Height)
        {
            this.Renderer = Renderer;
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.Renderer.Viewports.Add(this);
        }

        /// <summary>
        /// Forces the Renderer to redraw.
        /// </summary>
        public void Update()
        {
            this.Renderer.Update();
        }

        /// <summary>
        /// Disposes the viewport and all associated sprites.
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < Sprites.Count; i++)
            {
                Sprites[i].Dispose();
            }
            Sprites.Clear();
            Sprites = null;
            this.Renderer.Viewports.Remove(this);
            this.Renderer.Update();
            this.Disposed = true;
        }

        /// <summary>
        /// Whether the given point is within the viewport boundaries.
        /// </summary>
        /// <param name="p">The position of the point.</param>
        public bool Contains(Point p)
        {
            return Rect.Contains(p);
        }
        /// <summary>
        /// Whether the given point is within the viewport boundaries.
        /// </summary>
        /// <param name="X">The x position of the point.</param>
        /// <param name="Y">The y position of the point.</param>
        public bool Contains(int X, int Y)
        {
            return Rect.Contains(X, Y);
        }

        public override string ToString()
        {
            return $"(Viewport: {this.X},{this.Y},{this.Width},{this.Height})";
        }
    }
}
