using System;

namespace ODL
{
    public class Size
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Size(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }

        public override string ToString()
        {
            return $"(Size: {this.Width},{this.Height})";
        }
    }
}
