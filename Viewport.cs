using System;
using System.Collections.Generic;

namespace ODL
{
    public class Viewport
    {
        public string Name { get; set; }
        public Renderer Renderer { get; set; }
        public List<Sprite> Sprites { get; set; } = new List<Sprite>();
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Z { get; set; } = 0;
        public int Width { get; set; } = -1;
        public int Height { get; set; } = -1;
        public bool Disposed { get; set; } = false;
        public bool Visible { get; set; } = true;

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
                Sprites.RemoveAt(i);
            }
            this.Disposed = true;
        }

        public void ForceUpdate()
        {
            this.Renderer.ForceUpdate();
        }
    }
}
