using System;

namespace odl
{
    public class Point : IDisposable
    {
        public int X;
        public int Y;

        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public void Dispose()
        {
            
        }

        public override string ToString()
        {
            return $"(Point: {this.X},{this.Y})";
        }
    }
}
