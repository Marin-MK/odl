using System;

namespace odl
{
    public class Size : IDisposable
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

        public void Dispose()
        {
            
        }

        public override string ToString()
        {
            return $"(Size: {this.Width},{this.Height})";
        }
    }
}
