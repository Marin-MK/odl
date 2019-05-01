using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCS
{
    public class Point
    {
        public int X { get; set; } 
        public int Y { get; set; }

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
