using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ODL
{
    public class Viewport : IDisposable
    {
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
        /// <summary>
        /// The x position of the viewport.
        /// </summary>
        public int X = 0;
        /// <summary>
        /// The y position of the viewport.
        /// </summary>
        public int Y = 0;
        /// <summary>
        /// The z index of the viewport.
        /// </summary>
        public int Z = 0;
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
        public bool Visible { get { return _Visible; } set { _Visible = value; Update(); } }
        private double _ZoomX = 1;
        /// <summary>
        /// The horizontal zoom factor of the viewport.
        /// </summary>
        public double ZoomX { get { return _ZoomX; } set { this._ZoomX = value; Update(); } }
        private double _ZoomY = 1;
        /// <summary>
        /// The vertical zoom factor of the viewport.
        /// </summary>
        public double ZoomY { get { return _ZoomY; } set { this._ZoomY = value; Update(); } }
        /// <summary>
        /// The timestamp at which the viewport was created.
        /// </summary>
        public long TimeCreated = ((10000L * Stopwatch.GetTimestamp()) / TimeSpan.TicksPerMillisecond) / 100L;

        #region Constructor Overloads
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
