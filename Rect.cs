using System;
using static SDL2.SDL;

namespace ODL
{
    public class Rect
    {
        public int X { get { return SDL_Rect.x; } }
        public int Y { get { return SDL_Rect.y; } }
        public int Width { get { return SDL_Rect.w; } }
        public int Height { get { return SDL_Rect.h; } }
        private SDL_Rect _rect;
        public SDL_Rect SDL_Rect { get { return _rect; } }

        public Rect(Point p, Size s)
            : this(p.X, p.Y, s.Width, s.Height) { }

        public Rect(int X, int Y, Size s)
            : this(X, Y, s.Width, s.Height) { }

        public Rect(Point p, int Width, int Height)
            : this(p.X, p.Y, Width, Height) { }

        public Rect(Size s)
            : this(0, 0, s.Width, s.Height) { }

        public Rect(int Width, int Height)
            : this(0, 0, Width, Height) { }

        public Rect(SDL_Rect r)
        {
            _rect = r;
        }

        public Rect(int X, int Y, int Width, int Height)
        {
            _rect = new SDL_Rect();
            _rect.x = X;
            _rect.y = Y;
            _rect.w = Width;
            _rect.h = Height;
        }

        public bool Contains(int X, int Y)
        {
            return X >= this.X && X < this.Width && Y >= this.Y && Y < this.Height;
        }

        public override string ToString()
        {
            return $"(Rect: {this.X},{this.Y},{this.Width},{this.Height})";
        }
    }
}
