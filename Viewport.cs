using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ODL
{
    public class Viewport
    {
        public string Name;
        public Renderer Renderer;
        public List<Sprite> Sprites = new List<Sprite>();
        public int X = 0;
        public int Y = 0;
        public int Z = 0;
        public int Width = -1;
        public int Height = -1;
        public Rect Rect { get { return new Rect(X, Y, Width, Height); } }
        public bool Disposed { get; protected set; } = false;
        private bool _Visible = true;
        public bool Visible { get { return _Visible; } set { _Visible = value; ForceUpdate(); } }
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

        public void Update()
        {
            this.Renderer.Update();
        }

        public void Dispose()
        {
            for (int i = 0; i < Sprites.Count; i++)
            {
                Sprites[i].Dispose();
            }
            this.Renderer.Viewports.Remove(this);
            this.Renderer.ForceUpdate();
            this.Disposed = true;
        }

        public void ForceUpdate()
        {
            this.Renderer.ForceUpdate();
        }

        public bool Contains(int X, int Y)
        {
            return X >= this.X && X < this.X + this.Width && Y >= this.Y && Y < this.Y + this.Height;
        }

        public override string ToString()
        {
            return $"(Viewport: {this.X},{this.Y},{this.Width},{this.Height})";
        }
    }
}
