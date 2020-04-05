using System;

namespace ODL
{
    public delegate void BaseEvent(BaseEventArgs Args);
    public delegate void BoolEvent(BoolEventArgs Args);
    public delegate void MouseEvent(MouseEventArgs Args);
    public delegate void TextEvent(TextEventArgs Args);
    public delegate void TimespanEvent(TimespanEventArgs Args);
    public delegate void StringEvent(StringEventArgs Args);
    public delegate void DirectionEvent(DirectionEventArgs Args);
    public delegate void ObjectEvent(ObjectEventArgs Args);

    public class BaseEventArgs
    {
        public bool Handled = false;
    }

    public class BoolEventArgs : BaseEventArgs
    {
        public bool Value = false;

        public BoolEventArgs(bool Value = false)
        {
            this.Value = Value;
        }
    }

    public class MouseEventArgs : BaseEventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool OldLeftButton { get; }
        public bool LeftButton { get; }
        public bool OldRightButton { get; }
        public bool RightButton { get; }
        public bool OldMiddleButton { get; }
        public bool MiddleButton { get; }
        public int WheelY { get; }

        public MouseEventArgs(int X, int Y,
                bool OldLeftButton, bool LeftButton,
                bool OldRightButton, bool RightButton,
                bool OldMiddleButton, bool MiddleButton,
                int WheelY = 0)
        {
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

        public MouseEventArgs(int X, int Y, bool Left, bool Right, bool Middle)
        {
            this.X = X;
            this.Y = Y;
            this.OldLeftButton = this.LeftButton = Left;
            this.OldRightButton = this.RightButton = Right;
            this.OldMiddleButton = this.MiddleButton = Middle;
            this.WheelY = 0;
        }

        public bool Over(ISprite s)
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

    public class TextEventArgs : BaseEventArgs
    {
        public string Text { get; }
        public bool Backspace { get; }
        public bool Delete { get; }

        public TextEventArgs(string Text, bool Backspace = false, bool Delete = false)
        {
            this.Text = Text;
            this.Backspace = Backspace;
            this.Delete = Delete;
        }
    }

    public class TimespanEventArgs : BaseEventArgs
    {
        public TimeSpan Timespan;

        public TimespanEventArgs(TimeSpan Timespan)
        {
            this.Timespan = Timespan;
        }
    }

    public class StringEventArgs : BaseEventArgs
    {
        public string String;

        public StringEventArgs(string String = null)
        {
            this.String = String;
        }
    }

    public class DirectionEventArgs : BaseEventArgs
    {
        public bool Up = false;
        public bool Down = false;

        public DirectionEventArgs(bool Up, bool Down)
        {
            this.Up = Up;
            this.Down = Down;
        }
    }

    public class ObjectEventArgs : BaseEventArgs
    {
        public object Object;

        public ObjectEventArgs(object Object)
        {
            this.Object = Object;
        }
    }
}
