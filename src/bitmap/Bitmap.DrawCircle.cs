using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Draws a circle.
    /// </summary>
    /// <param name="c">The origin position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="color">The color to draw the circle with.</param>
    public void DrawCircle(Point c, int Radius, Color color)
    {
        DrawCircle(c.X, c.Y, Radius, color.Red, color.Green, color.Blue, color.Alpha);
    }

    /// <summary>
    /// Draws a circle.
    /// </summary>
    /// <param name="ox">The origin X position of the circle.</param>
    /// <param name="oy">The origin Y position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="c">The color to draw the circle with.</param>
    public void DrawCircle(int ox, int oy, int Radius, Color c)
    {
        DrawCircle(ox, oy, Radius, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Draws a circle.
    /// </summary>
    /// <param name="c">The origin position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="r">The Red component of the color to draw the circle with.</param>
    /// <param name="g">The Green component of the color to draw the circle with.</param>
    /// <param name="b">The Blue component of the color to draw the circle with.</param>
    /// <param name="a">The Alpha component of the color to draw the circle with.</param>
    public void DrawCircle(Point c, int Radius, byte r, byte g, byte b, byte a = 255)
    {
        DrawCircle(c.X, c.Y, Radius, r, g, b, a);
    }

    /// <summary>
    /// Draws a circle.
    /// </summary>
    /// <param name="ox">The origin X position of the circle.</param>
    /// <param name="oy">The origin Y position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="r">The Red component of the color to draw the circle with.</param>
    /// <param name="g">The Green component of the color to draw the circle with.</param>
    /// <param name="b">The Blue component of the color to draw the circle with.</param>
    /// <param name="a">The Alpha component of the color to draw the circle with.</param>
    public virtual void DrawCircle(int ox, int oy, int Radius, byte r, byte g, byte b, byte a = 255)
    {
        if (Locked) throw new BitmapLockedException();
        int x = Radius - 1;
        int y = 0;
        int dx = 1;
        int dy = 1;
        int err = dx - (Radius << 1);
        while (x >= y)
        {
            SetPixel(ox - x, oy - y, r, g, b, a);
            SetPixel(ox - x, oy + y, r, g, b, a);
            SetPixel(ox + x, oy - y, r, g, b, a);
            SetPixel(ox + x, oy + y, r, g, b, a);
            SetPixel(ox - y, oy - x, r, g, b, a);
            SetPixel(ox - y, oy + x, r, g, b, a);
            SetPixel(ox + y, oy - x, r, g, b, a);
            SetPixel(ox + y, oy + x, r, g, b, a);
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

