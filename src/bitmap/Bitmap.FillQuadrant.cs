using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Fills a quarter of a circle.
    /// </summary>
    /// <param name="c">The origin position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="l">The part of the circle to draw.</param>
    /// <param name="color">The color to fill the quadrant with.</param>
    public void FillQuadrant(Point c, int Radius, Location l, Color color)
    {
        FillQuadrant(c.X, c.Y, Radius, l, color.Red, color.Green, color.Blue, color.Alpha);
    }

    /// <summary>
    /// Fills a quarter of a circle.
    /// </summary>
    /// <param name="ox">The origin X position of the circle.</param>
    /// <param name="oy">The origin Y position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="l">The part of the circle to draw.</param>
    /// <param name="c">The color to fill the quadrant with.</param>
    public void FillQuadrant(int ox, int oy, int Radius, Location l, Color c)
    {
        FillQuadrant(ox, oy, Radius, l, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Fills a quarter of a circle.
    /// </summary>
    /// <param name="c">The origin position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="l">The part of the circle to draw.</param>
    /// <param name="r">The Red component of the color to fill the quadrant with.</param>
    /// <param name="g">The Green component of the color to fill the quadrant with.</param>
    /// <param name="b">The Blue component of the color to fill the quadrant with.</param>
    /// <param name="a">The Alpha component of the color to fill the quadrant with.</param>
    public void FillQuadrant(Point c, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
    {
        FillQuadrant(c.X, c.Y, Radius, l, r, g, b, a);
    }

    /// <summary>
    /// Fills a quarter of a circle.
    /// </summary>
    /// <param name="ox">The origin X position of the circle.</param>
    /// <param name="oy">The origin Y position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="l">The part of the circle to draw.</param>
    /// <param name="r">The Red component of the color to fill the quadrant with.</param>
    /// <param name="g">The Green component of the color to fill the quadrant with.</param>
    /// <param name="b">The Blue component of the color to fill the quadrant with.</param>
    /// <param name="a">The Alpha component of the color to fill the quadrant with.</param>
    public virtual void FillQuadrant(int ox, int oy, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
    {
        if (Locked) throw new BitmapLockedException();
        int x = Radius - 1;
        int y = 0;
        int dx = 1;
        int dy = 1;
        int err = dx - (Radius << 1);
        while (x >= y)
        {
            if (l == Location.TopRight) // 0 - 90
            {
                for (int i = ox + y; i <= ox + x; i++)
                {
                    SetPixel(i, oy - y, r, g, b, a);
                }
                for (int i = oy - x; i <= oy - y; i++)
                {
                    SetPixel(ox + y, i, r, g, b, a);
                }
            }
            else if (l == Location.TopLeft) // 90 - 180
            {
                for (int i = ox - x; i <= ox - y; i++)
                {
                    SetPixel(i, oy - y, r, g, b, a);
                }
                for (int i = oy - x; i <= oy - y; i++)
                {
                    SetPixel(ox - y, i, r, g, b, a);
                }
            }
            else if (l == Location.BottomLeft) // 180 - 270
            {
                for (int i = ox - x; i <= ox - y; i++)
                {
                    SetPixel(i, oy + y, r, g, b, a);
                }
                for (int i = oy + y; i <= oy + x; i++)
                {
                    SetPixel(ox - y, i, r, g, b, a);
                }
            }
            else if (l == Location.BottomRight) // 270 - 360
            {
                for (int i = ox + y; i <= ox + x; i++)
                {
                    SetPixel(i, oy + y, r, g, b, a);
                }
                for (int i = oy + y; i <= oy + x; i++)
                {
                    SetPixel(ox + y, i, r, g, b, a);
                }
            }
            if (err <= 0)
            {
                y++;
                err += dy;
                dy += 2;
            }
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

