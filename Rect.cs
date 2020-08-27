using System;
using static odl.SDL2.SDL;

namespace odl
{
    public class Rect : IDisposable
    {
        /// <summary>
        /// The x position of the rectangle.
        /// </summary>
        public int X { get { return SDL_Rect.x; } set { this._SDL_Rect.x = value; } }
        /// <summary>
        /// The y position of the rectangle.
        /// </summary>
        public int Y { get { return SDL_Rect.y; } set { this._SDL_Rect.y = value; } }
        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        public int Width { get { return SDL_Rect.w; } set { this._SDL_Rect.w = value; } }
        /// <summary>
        /// The height of the rectangle.
        /// </summary>
        public int Height { get { return SDL_Rect.h; } set { this._SDL_Rect.h = value; } }
        private SDL_Rect _SDL_Rect;
        /// <summary>
        /// The SDL_Rect object associated with the rectangle.
        /// </summary>
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

        /// <summary>
        /// Returns whether the given point is within the rectangle boundaries.
        /// </summary>
        /// <param name="p">The point to test.</param>
        public bool Contains(Point p)
        {
            return Contains(p.X, p.Y);
        }
        /// <summary>
        /// Returns whether the given point is within the rectangle boundaries.
        /// </summary>
        /// <param name="X">The x position to test.</param>
        /// <param name="Y">The y position to test.</param>
        public bool Contains(int X, int Y)
        {
            return X >= this.X && X < this.X + this.Width && Y >= this.Y && Y < this.Y + this.Height;
        }

        /// <summary>
        /// Whether this rectangle overlaps the given rectangle or vice-versa.
        /// </summary>
        public bool Overlaps(Rect r)
        {
            return this.X + this.Width > r.X && this.X < r.X + r.Width &&
                   this.Y + this.Height > r.Y && this.Y < r.Y + r.Height;
        }

        public override string ToString()
        {
            return $"(Rect: {this.X},{this.Y},{this.Width},{this.Height})";
        }

        public void Dispose()
        {
            
        }
    }
}
