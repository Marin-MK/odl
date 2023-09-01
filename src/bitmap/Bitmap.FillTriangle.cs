using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Fills a triangle with a solid color. The points must be given in clockwise order.
    /// </summary>
    /// <param name="v0">The left-most point of the triangle.</param>
    /// <param name="v1">The upper-most or right-most point of the triangle.</param>
    /// <param name="v2">The remaining point of the triangle.</param>
    /// <param name="c">The color to fill the triangle with.</param>
    public virtual void FillTriangle(Point v0, Point v1, Point v2, Color c)
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
                    SetPixel(p, c);
                }
            }
        }
        this.Renderer?.Update();
    }
}

