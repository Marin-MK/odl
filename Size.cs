using System;

namespace odl;

public class Size
{
    public int Width;
    public int Height;

    public Size(Size size)
        : this(size.Width, size.Height) { }
    public Size(int Width, int Height)
    {
        this.Width = Width;
        this.Height = Height;
    }

    public override string ToString()
    {
        return $"(Size: {this.Width},{this.Height})";
    }

    public override bool Equals(object obj)
    {
        if (this == obj) return true;
        if (obj is Size)
        {
            Size s = (Size) obj;
            return this.Width == s.Width && this.Height == s.Height;
        }
        return false;
    }
}
