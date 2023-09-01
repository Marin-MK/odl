using System;
using static odl.SDL2.SDL;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="r">The rectangle to draw.</param>
    /// <param name="c">The color to draw the rectangle with.</param>
    public void DrawRect(Rect r, Color c)
    {
        DrawRect(r.X, r.Y, r.Width, r.Height, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="rect">The rectangle to draw.</param>
    /// <param name="r">The Red component of the color to draw the rectangle with.</param>
    /// <param name="g">The Green component of the color to draw the rectangle with.</param>
    /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
    public void DrawRect(Rect rect, byte r, byte g, byte b, byte a = 255)
    {
        DrawRect(rect.X, rect.Y, rect.Width, rect.Height, r, g, b, a);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="Point">The position of the rectangle.</param>
    /// <param name="Size">The size of the rectangle.</param>
    /// <param name="c">The color to draw the rectangle with.</param>
    public void DrawRect(Point Point, Size Size, Color c)
    {
        DrawRect(Point.X, Point.Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="Point">The position of the rectangle.</param>
    /// <param name="Size">The size of the rectangle.</param>
    /// <param name="r">The Red component of the color to draw the rectangle with.</param>
    /// <param name="g">The Green component of the color to draw the rectangle with.</param>
    /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
    public void DrawRect(Point Point, Size Size, byte r, byte g, byte b, byte a = 255)
    {
        DrawRect(Point.X, Point.Y, Size.Width, Size.Height, r, g, b, a);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="Point">The position of the rectangle.</param>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="c">The color to draw the rectangle with.</param>
    public void DrawRect(Point Point, int Width, int Height, Color c)
    {
        DrawRect(Point.X, Point.Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="Point">The position of the rectangle.</param>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="r">The Red component of the color to draw the rectangle with.</param>
    /// <param name="g">The Green component of the color to draw the rectangle with.</param>
    /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
    public void DrawRect(Point Point, int Width, int Height, byte r, byte g, byte b, byte a = 255)
    {
        DrawRect(Point.X, Point.Y, Width, Height, r, g, b, a);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="r">The Red component of the color to draw the rectangle with.</param>
    /// <param name="g">The Green component of the color to draw the rectangle with.</param>
    /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
    public void DrawRect(Size size, byte r, byte g, byte b, byte a = 255)
    {
        this.DrawRect(0, 0, size, r, g, b, a);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="c">The color to draw the rectangle with.</param>
    public void DrawRect(Size size, Color c)
    {
        this.DrawRect(0, 0, size, c);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="r">The Red component of the color to draw the rectangle with.</param>
    /// <param name="g">The Green component of the color to draw the rectangle with.</param>
    /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
    public void DrawRect(int Width, int Height, byte r, byte g, byte b, byte a = 255)
    {
        this.DrawRect(0, 0, Width, Height, r, g, b, a);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="c">The color to draw the rectangle with.</param>
    public void DrawRect(int Width, int Height, Color c)
    {
        this.DrawRect(0, 0, Width, Height, c);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="X">The X position of the rectangle.</param>
    /// <param name="Y">The Y position of the rectangle.</param>
    /// <param name="Size">The size of the rectangle.</param>
    /// <param name="c">The color of the rectangle to draw.</param>
    public void DrawRect(int X, int Y, Size Size, Color c)
    {
        DrawRect(X, Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="X">The X position of the rectangle.</param>
    /// <param name="Y">The Y position of the rectangle.</param>
    /// <param name="Size">The size of the rectangle.</param>
    /// <param name="r">The Red component of the color to draw the rectangle with.</param>
    /// <param name="g">The Green component of the color to draw the rectangle with.</param>
    /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
    public void DrawRect(int X, int Y, Size Size, byte r, byte g, byte b, byte a = 255)
    {
        DrawRect(X, Y, Size.Width, Size.Height, r, g, b, a);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="X">The X position of the rectangle.</param>
    /// <param name="Y">The Y position of the rectangle.</param>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="c">The color to draw the rectangle with.</param>
    public void DrawRect(int X, int Y, int Width, int Height, Color c)
    {
        DrawRect(X, Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Draws a rectangle with a solid color.
    /// </summary>
    /// <param name="X">The X position of the rectangle.</param>
    /// <param name="Y">The Y position of the rectangle.</param>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="r">The Red component of the color to draw the rectangle with.</param>
    /// <param name="g">The Green component of the color to draw the rectangle with.</param>
    /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
    public virtual void DrawRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255)
    {
        if (Locked) throw new BitmapLockedException();
        if (X < 0 || Y < 0)
        {
            throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- minimum is (0,0)");
        }
        if (X + Width > this.Width || Y + Height > this.Height)
        {
            throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
        }
        DrawLine(X, Y, X + Width - 1, Y, r, g, b, a);
        DrawLine(X, Y, X, Y + Height - 1, r, g, b, a);
        DrawLine(X, Y + Height - 1, X + Width - 1, Y + Height - 1, r, g, b, a);
        DrawLine(X + Width - 1, Y, X + Width - 1, Y + Height - 1, r, g, b, a);
        if (this.Renderer != null) this.Renderer.Update();
    }
}

