using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Fills a triangle with colors interpolated from the triangle vertices. The vertices must be given in clockwise order.
    /// </summary>
    /// <param name="v0">The left-most point of the triangle.</param>
    /// <param name="v1">The upper-most of right-most point of the triangle.</param>
    /// <param name="v2">The remaining point of the triangle.</param>
    /// <param name="c0">The color corresponding to <paramref name="v0"/>.</param>
    /// <param name="c1">The color corresponding to <paramref name="v1"/>.</param>
    /// <param name="c2">The color corresponding to <paramref name="v2"/>.</param>
    public virtual void FillGradientTriangle(Point v0, Point v1, Point v2, Color c0, Color c1, Color c2)
    {
        FillGradientTriangle(new Vertex(v0, c0), new Vertex(v1, c1), new Vertex(v2, c2));
    }

    /// <summary>
    /// Fills a triangle with colors interpolated from the triangle vertices. The vertices must be given in clockwise order.
    /// </summary>
    /// <param name="v0">The left-most vertex of the triangle.</param>
    /// <param name="v1">The upper-most of right-most vertex of the triangle.</param>
    /// <param name="v2">The remaining vertex of the triangle.</param>
    public virtual void FillGradientTriangle(Vertex v0, Vertex v1, Vertex v2)
    {
        if (Locked) throw new BitmapLockedException();
        if (v0.X < 0 || v0.Y < 0 || v0.X >= Width || v0.Y >= Height) throw new Exception("First triangle vector is out of bounds.");
        if (v1.X < 0 || v1.Y < 0 || v1.X >= Width || v1.Y >= Height) throw new Exception("Second triangle vector is out of bounds.");
        if (v2.X < 0 || v2.Y < 0 || v2.X >= Width || v2.Y >= Height) throw new Exception("Third triangle vector is out of bounds.");
        int minx = Math.Min(v0.X, Math.Min(v1.X, v2.X));
        int maxx = Math.Max(v0.X, Math.Max(v1.X, v2.X));
        int miny = Math.Min(v0.Y, Math.Min(v1.Y, v2.Y));
        int maxy = Math.Max(v0.Y, Math.Max(v1.Y, v2.Y));
        for (int x = minx; x <= maxx; x++)
        {
            for (int y = miny; y <= maxy; y++)
            {
                Point p = new Point(x, y);
                double w0 = EdgeFunction(v1, v2, p);
                double w1 = EdgeFunction(v2, v0, p);
                double w2 = EdgeFunction(v0, v1, p);
                if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                {
                    double area = EdgeFunction(v0, v1, v2);
                    double f0 = w0 / area;
                    double f1 = w1 / area;
                    double f2 = w2 / area;
                    byte r = (byte)Math.Round(f0 * v0.R + f1 * v1.R + f2 * v2.R);
                    byte g = (byte)Math.Round(f0 * v0.G + f1 * v1.G + f2 * v2.G);
                    byte b = (byte)Math.Round(f0 * v0.B + f1 * v1.B + f2 * v2.B);
                    byte a = (byte)Math.Round(f0 * v0.A + f1 * v1.A + f2 * v2.A);
                    SetPixel(p, r, g, b, a);
                }
            }
        }
        this.Renderer?.Update();
    }

    protected double EdgeFunction(Vertex a, Vertex b, Vertex c)
    {
        return EdgeFunction(a.Point, b.Point, c.Point);
    }

    protected double EdgeFunction(Vertex a, Vertex b, Point c)
    {
        return EdgeFunction(a.Point, b.Point, c);
    }

    protected virtual double EdgeFunction(Point a, Point b, Point c)
    {
        return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
    }
}

