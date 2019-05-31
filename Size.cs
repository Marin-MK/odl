using System;

namespace ODL
{
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

        public void Clamp(Size MinimumSize, Size MaximumSize)
        {
            this.Clamp(MinimumSize.Width, MaximumSize.Width, MinimumSize.Height, MaximumSize.Height);
        }
        public void Clamp(int MinWidth, int MaxWidth, int MinHeight, int MaxHeight)
        {
            if (this.Width < MinWidth) this.Width = MinWidth;
            if (this.Width > MaxWidth) this.Width = MaxWidth;
            if (this.Height < MinHeight) this.Height = MinHeight;
            if (this.Height > MaxHeight) this.Height = MaxHeight;
        }
    }
}
