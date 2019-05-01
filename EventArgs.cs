using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCS
{
    public class TimeEventArgs : EventArgs
    {
        public TimeSpan Duration { get; set; }

        public TimeEventArgs(TimeSpan Duration)
        {
            this.Duration = Duration;
        }
    }

    public class ClosingEventArgs : EventArgs
    {
        public Exception Error { get; set; } = null;
        public bool Cancel { get; set; } = false;
        public bool CausedByException { get { return this.Error != null; } }

        public ClosingEventArgs()
        {
            
        }

        public ClosingEventArgs(Exception Error)
        {
            this.Error = Error;
        }
    }

    public class ClosedEventArgs : EventArgs
    {
        public Exception Error { get; set; } = null;
        public bool CausedByException { get { return this.Error != null; } }

        public ClosedEventArgs()
        {

        }

        public ClosedEventArgs(Exception Error)
        {
            this.Error = Error;
        }
    }

    public enum MouseButtons
    {
        Left = 1,
        Middle = 2,
        Right = 4
    }

    public class MouseEventArgs : EventArgs
    {
        public int OldX { get; set; }
        public int OldY { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool LeftButton { get; }
        public bool MiddleButton { get; }
        public bool RightButton { get; }

        public MouseEventArgs(int OldX, int OldY, int X, int Y, bool LeftButton, bool MiddleButton, bool RightButton)
        {
            this.OldX = OldX;
            this.OldY = OldY;
            this.X = X;
            this.Y = Y;
            this.LeftButton = LeftButton;
            this.MiddleButton = MiddleButton;
            this.RightButton = RightButton;
        }

        public bool Over(Sprite s)
        {
            if (s.Bitmap == null) return false;
            if (X >= s.X && X < s.X + s.Bitmap.Width * s.ZoomX &&
                Y >= s.Y && Y < s.Y + s.Bitmap.Height * s.ZoomY)
            {
                return true;
            }
            return false;
        }

        public bool InArea(Rect r)
        {
            return InArea(r.X, r.Y, r.Width, r.Height);
        }

        public bool InArea(Point p, Size s)
        {
            return InArea(p.X, p.Y, s.Width, s.Height);
        }

        public bool InArea(Point p, int Width, int Height)
        {
            return InArea(p.X, p.Y, Width, Height);
        }

        public bool InArea(int X, int Y, Size s)
        {
            return InArea(X, Y, s.Width, s.Height);
        }

        public bool InArea(int X, int Y, int Width, int Height)
        {
            return this.X >= X && this.X < X + Width && this.Y >= Y && this.Y < Y + Height;
        }
    }

    public class LocationEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }

        public LocationEventArgs(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public LocationEventArgs(Point p)
        {
            this.X = p.X;
            this.Y = p.Y;
        }
    }

    public class TickEventArgs : EventArgs
    {
        public SDL2.SDL.SDL_Event e { get; set; }

        public TickEventArgs(SDL2.SDL.SDL_Event e)
        {
            this.e = e;
        }
    }
}
