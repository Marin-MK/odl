using System;

namespace ODL
{
    public class Point
    {
        public int X;
        public int Y;

        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public override string ToString()
        {
            return $"(Point: {this.X},{this.Y})";
        }
    }
}
