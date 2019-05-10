using System;
using static SDL2.SDL;

namespace ODL
{
    public class Rect
    {
        public int X { get { return SDL_Rect.x; } set { this._SDL_Rect.x = value; } }
        public int Y { get { return SDL_Rect.y; } set { this._SDL_Rect.y = value; } }
        public int Width { get { return SDL_Rect.w; } set { this._SDL_Rect.w = value; } }
        public int Height { get { return SDL_Rect.h; } set { this._SDL_Rect.h = value; } }
        private SDL_Rect _SDL_Rect;
        public SDL_Rect SDL_Rect { get { return _SDL_Rect; } }

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
            this._SDL_Rect= r;
        }

        public Rect(int X, int Y, int Width, int Height)
        {
            SDL_Rect r = new SDL_Rect();
            r.x = X;
            r.y = Y;
            r.w = Width;
            r.h = Height;
            this._SDL_Rect = r;
        }

        public bool Contains(int X, int Y)
        {
            return X >= this.X && X < this.X + this.Width && Y >= this.Y && Y < this.Y + this.Height;
        }

        public override string ToString()
        {
            return $"(Rect: {this.X},{this.Y},{this.Width},{this.Height})";
        }
    }
}
