using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Fills a circle with a radial gradient.
    /// </summary>
    /// <param name="c">The origin position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="color1">The inner color of the circle.</param>
    /// <param name="color2">The outer color of the cirlce.</param>
    public void FillGradientCircle(Point c, int Radius, Color color1, Color color2)
    {
        FillGradientCircle(c.X, c.Y, Radius, color1, color2);
    }

    /// <summary>
    /// Fills a circle with a radial gradient.
    /// </summary>
    /// <param name="ox">The origin X position of the circle.</param>
    /// <param name="oy">The origin Y position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="c1">The inner color of the circle.</param>
    /// <param name="c2">The outer color of the circle.</param>
    public virtual void FillGradientCircle(int ox, int oy, int Radius, Color c1, Color c2)
    {
        if (Locked) throw new BitmapLockedException();
        int x = Radius - 1;
        int y = 0;
        int dx = 1;
        int dy = 1;
        Point center = new Point(ox, oy);
        int err = dx - (Radius << 1);
        while (x >= y)
        {
            for (int i = ox - x; i <= ox + x; i++)
            {
                SetPixel(i, oy + y, Interpolate2D(c1, c2, 1 - center.Distance(new Point(i, oy + y)) / Radius));
                SetPixel(i, oy - y, Interpolate2D(c1, c2, 1 - center.Distance(new Point(i, oy - y)) / Radius));
            }
            for (int i = ox - y; i <= ox + y; i++)
            {
                SetPixel(i, oy + x, Interpolate2D(c1, c2, 1 - center.Distance(new Point(i, oy + x)) / Radius));
                SetPixel(i, oy - x, Interpolate2D(c1, c2, 1 - center.Distance(new Point(i, oy - x)) / Radius));
            }

            y++;
            err += dy;
            dy += 2;
            if (err > 0)
            {
                x--;
                dx += 2;
                err += dx - (Radius << 1);
            }
        }
        if (this.Renderer != null) this.Renderer.Update();
    }
}

