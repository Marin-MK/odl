using System;

namespace ODL
{
    public class TimeEventArgs : EventArgs
    {
        public TimeSpan Duration { get; }

        public TimeEventArgs(TimeSpan Duration)
        {
            this.Duration = Duration;
        }
    }

    public class CancelEventArgs : EventArgs
    {
        public bool Cancel = false;

        public CancelEventArgs(bool Cancel = false)
        {
            this.Cancel = Cancel;
        }
    }

    public class ClosedEventArgs : EventArgs
    {
        public Exception Error { get; } = null;
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
        public int OldX { get; }
        public int OldY { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool OldLeftButton { get; }
        public bool LeftButton { get; }
        public bool OldRightButton { get; }
        public bool RightButton { get; }
        public bool OldMiddleButton { get; }
        public bool MiddleButton { get; }
        public int WheelY { get; }
        public bool Handled = false;

        public MouseEventArgs(int OldX, int OldY, int X, int Y,
                bool OldLeftButton, bool LeftButton,
                bool OldRightButton, bool RightButton,
                bool OldMiddleButton, bool MiddleButton,
                int WheelY = 0)
        {
            this.OldX = OldX;
            this.OldY = OldY;
            this.X = X;
            this.Y = Y;
            this.OldLeftButton = OldLeftButton;
            this.LeftButton = LeftButton;
            this.OldRightButton = OldRightButton;
            this.RightButton = RightButton;
            this.OldMiddleButton = OldMiddleButton;
            this.MiddleButton = MiddleButton;
            this.WheelY = WheelY;
            Graphics.LastMouseEvent = this;
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

    public class FocusEventArgs : EventArgs
    {
        public bool Focus { get; }

        public FocusEventArgs(bool Focus)
        {
            this.Focus = Focus;
        }
    }

    public class TextInputEventArgs : EventArgs
    {
        public string Text { get; }
        public bool Backspace { get; }
        public bool Delete { get; }

        public TextInputEventArgs(string Text, bool Backspace = false, bool Delete = false)
        {
            this.Text = Text;
            this.Backspace = Backspace;
            this.Delete = Delete;
        }
    }

    public class WindowEventArgs : EventArgs
    {
        public int Width { get; }
        public int Height { get; }

        public WindowEventArgs(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }
    }

    public class SizeEventArgs : EventArgs
    {
        public int Width { get; }
        public int Height { get; }
        public int OldWidth { get; }
        public int OldHeight { get; }

        public SizeEventArgs(Size size)
            : this(size, size) { }
        public SizeEventArgs(int Width, int Height)
            : this(Width, Width, Height, Height) { }
        public SizeEventArgs(Size newsize, Size oldsize)
            : this(newsize.Width, oldsize.Width, newsize.Height, oldsize.Height) { }
        public SizeEventArgs(Size newsize, int OldWidth, int OldHeight)
            : this(newsize.Width, OldWidth, newsize.Height, OldHeight) { }
        public SizeEventArgs(int NewWidth, int NewHeight, Size oldsize)
            : this(NewWidth, oldsize.Width, NewHeight, oldsize.Height) { }
        public SizeEventArgs(int NewWidth, int OldWidth, int NewHeight, int OldHeight)
        {
            this.Width = NewWidth;
            this.OldWidth = OldWidth;
            this.Height = NewHeight;
            this.OldHeight = OldHeight;
        }
    }

    public class ConditionEventArgs : EventArgs
    {
        public bool ConditionValue;

        public ConditionEventArgs(bool DefaultValue = true)
        {
            this.ConditionValue = DefaultValue;
        }
    }

    public class DirectionEventArgs : EventArgs
    {
        public bool Up;
        public bool Down;

        public DirectionEventArgs(bool Up, bool Down)
        {
            this.Up = Up;
            this.Down = Down;
        }
    }

    public class FetchEventArgs : EventArgs
    {
        public object Value;

        public FetchEventArgs() { }
        public FetchEventArgs(object Value)
        {
            this.Value = Value;
        }
    }

    public class PointEventArgs : EventArgs
    {
        public int X;
        public int Y;
        public bool LeftButton;
        public bool RightButton;
        public bool MiddleButton;

        public PointEventArgs(int X, int Y, bool LeftButton = false, bool RightButton = false, bool MiddleButton = false)
        {
            this.X = X;
            this.Y = Y;
            this.LeftButton = LeftButton;
            this.RightButton = RightButton;
            this.MiddleButton = MiddleButton;
        }

        public PointEventArgs(Point p, bool LeftButton = false, bool RightButton = false, bool MiddleButton = false)
        {
            this.X = p.X;
            this.Y = p.Y;
            this.LeftButton = LeftButton;
            this.RightButton = RightButton;
            this.MiddleButton = MiddleButton;
        }
    }

    public class ObjectEventArgs : EventArgs
    {
        public object Value;

        public ObjectEventArgs(object Value)
        {
            this.Value = Value;
        }
    }
}
