using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Draws a quarter of a circle.
    /// </summary>
    /// <param name="c">The origin position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="l">The part of the circle to draw.</param>
    /// <param name="color">The color to draw the quadrant with.</param>
    public void DrawQuadrant(Point c, int Radius, Location l, Color color)
    {
        DrawQuadrant(c.X, c.Y, Radius, l, color.Red, color.Green, color.Blue, color.Alpha);
    }

    /// <summary>
    /// Draws a quarter of a circle.
    /// </summary>
    /// <param name="ox">The origin X position of the circle.</param>
    /// <param name="oy">The origin Y position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="l">The part of the circle to draw.</param>
    /// <param name="c">The color to draw the quadrant with.</param>
    public void DrawQuadrant(int ox, int oy, int Radius, Location l, Color c)
    {
        DrawQuadrant(ox, oy, Radius, l, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Draws a quarter of a circle.
    /// </summary>
    /// <param name="c">The origin position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="l">The part of the circle to draw.</param>
    /// <param name="r">The Red component of the color to draw the quadrant with.</param>
    /// <param name="g">The Green component of the color to draw the quadrant with.</param>
    /// <param name="b">The Blue component of the color to draw the quadrant with.</param>
    /// <param name="a">The Alpha component of the color to draw the quadrant with.</param>
    public void DrawQuadrant(Point c, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
    {
        DrawQuadrant(c.X, c.Y, Radius, l, r, g, b, a);
    }

    /// <summary>
    /// Draws a quarter of a circle.
    /// </summary>
    /// <param name="ox">The origin X position of the circle.</param>
    /// <param name="oy">The origin Y position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="l">The part of the circle to draw.</param>
    /// <param name="r">The Red component of the color to draw the quadrant with.</param>
    /// <param name="g">The Green component of the color to draw the quadrant with.</param>
    /// <param name="b">The Blue component of the color to draw the quadrant with.</param>
    /// <param name="a">The Alpha component of the color to draw the quadrant with.</param>
    public virtual void DrawQuadrant(int ox, int oy, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
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
                SetPixel(ox + y, oy - x, r, g, b, a);
                SetPixel(ox + x, oy - y, r, g, b, a);
            }
            else if (l == Location.TopLeft) // 90 - 180
            {
                SetPixel(ox - x, oy - y, r, g, b, a);
                SetPixel(ox - y, oy - x, r, g, b, a);
            }
            else if (l == Location.BottomLeft) // 180 - 270
            {
                SetPixel(ox - x, oy + y, r, g, b, a);
                SetPixel(ox - y, oy + x, r, g, b, a);
            }
            else if (l == Location.BottomRight) // 270 - 360
            {
                SetPixel(ox + x, oy + y, r, g, b, a);
                SetPixel(ox + y, oy + x, r, g, b, a);
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

