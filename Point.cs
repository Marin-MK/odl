using System;

namespace odl;

public class Point : IDisposable
{
    public int X;
    public int Y;

    public Point(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public double Distance(Point p)
    {
        return Math.Sqrt(Math.Pow(this.X - p.X, 2) + Math.Pow(this.Y - p.Y, 2));
    }

    public void Dispose()
    {

    }

    public override string ToString()
    {
        return $"(Point: {this.X},{this.Y})";
    }
}
