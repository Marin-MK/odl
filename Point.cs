using System;

namespace odl;

public class Point
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

    public override string ToString()
    {
        return $"(Point: {this.X},{this.Y})";
    }

    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static Point operator -(Point p1, Point p2)
    {
        return new Point(p1.X - p2.X, p1.Y - p2.Y);
    }

    public Point Abs()
    {
        return new Point(Math.Abs(X), Math.Abs(Y));
    }

    public Size ToSize()
    {
        return new Size(this.X, this.Y);
    }
}
