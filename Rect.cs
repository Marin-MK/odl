using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace VCS
{
    public class Rect
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public SDL_Rect SDL_Rect
        {
            get
            {
                SDL_Rect Rect = new SDL_Rect();
                Rect.x = this.X;
                Rect.y = this.Y;
                Rect.w = this.Width;
                Rect.h = this.Height;
                return Rect;
            }
        }

        public Rect(int X, int Y, int Width, int Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
        }

        public Rect(Point p, Size s)
        {
            this.X = p.X;
            this.Y = p.Y;
            this.Width = s.Width;
            this.Height = s.Height;
        }

        public Rect(int X, int Y, Size s)
        {
            this.X = X;
            this.Y = Y;
            this.Width = s.Width;
            this.Height = s.Height;
        }

        public Rect(Point p, int Width, int Height)
        {
            this.X = p.X;
            this.Y = p.Y;
            this.Width = Width;
            this.Height = Height;
        }

        public Rect(Size s)
        {
            this.X = 0;
            this.Y = 0;
            this.Width = s.Width;
            this.Height = s.Height;
        }

        public Rect(int Width, int Height)
        {
            this.X = 0;
            this.Y = 0;
            this.Width = Width;
            this.Height = Height;
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
