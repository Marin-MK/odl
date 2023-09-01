using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Draws a line between two points.
    /// </summary>
    /// <param name="p1">The position of the first point.</param>
    /// <param name="p2">The position of the second point.</param>
    /// <param name="c">The color to draw the line with.</param>
    public void DrawLine(Point p1, Point p2, Color c)
    {
        DrawLine(p1.X, p1.Y, p2.X, p2.Y, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Draws a line between two points.
    /// </summary>
    /// <param name="p1">The position of the first point.</param>
    /// <param name="p2">The position of the second point.</param>
    /// <param name="r">The Red component of the color to draw the line with.</param>
    /// <param name="g">The Green component of the color to draw the line with.</param>
    /// <param name="b">The Blue component of the color to draw the line with.</param>
    /// <param name="a">The Alpha component of the color to draw the line with.</param>
    public void DrawLine(Point p1, Point p2, byte r, byte g, byte b, byte a = 255)
    {
        DrawLine(p1.X, p1.Y, p2.X, p2.Y, r, g, b, a);
    }

    /// <summary>
    /// Draws a line between two points.
    /// </summary>
    /// <param name="p1">The position of the first point.</param>
    /// <param name="x2">The X position of the second point.</param>
    /// <param name="y2">The Y position of the second point.</param>
    /// <param name="c">The color to draw the line with.</param>
    public void DrawLine(Point p1, int x2, int y2, Color c)
    {
        DrawLine(p1.X, p1.Y, x2, y2, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Draws a line between two points.
    /// </summary>
    /// <param name="p1">The position of the first point.</param>
    /// <param name="x2">The X position of the second point.</param>
    /// <param name="y2">The Y position of the second point.</param>
    /// <param name="r">The Red component of the color to draw the line with.</param>
    /// <param name="g">The Green component of the color to draw the line with.</param>
    /// <param name="b">The Blue component of the color to draw the line with.</param>
    /// <param name="a">The Alpha component of the color to draw the line with.</param>
    public void DrawLine(Point p1, int x2, int y2, byte r, byte g, byte b, byte a = 255)
    {
        DrawLine(p1.X, p1.Y, x2, y2, r, g, b, a);
    }

    /// <summary>
    /// Draws a line between two points.
    /// </summary>
    /// <param name="x1">The X position of the first point.</param>
    /// <param name="y1">The Y position of the first point.</param>
    /// <param name="p2">The position of the second point.</param>
    /// <param name="c">The color to draw the line with.</param>
    public void DrawLine(int x1, int y1, Point p2, Color c)
    {
        DrawLine(x1, y1, p2.X, p2.Y, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Draws a line between two points.
    /// </summary>
    /// <param name="x1">The X position of the first point.</param>
    /// <param name="y1">The Y position of the first point.</param>
    /// <param name="p2">The position of the second point.</param>
    /// <param name="r">The Red component of the color to draw the line with.</param>
    /// <param name="g">The Green component of the color to draw the line with.</param>
    /// <param name="b">The Blue component of the color to draw the line with.</param>
    /// <param name="a">The Alpha component of the color to draw the line with.</param>
    public void DrawLine(int x1, int y1, Point p2, byte r, byte g, byte b, byte a = 255)
    {
        DrawLine(x1, y1, p2.X, p2.Y, r, g, b, a);
    }

    /// <summary>
    /// Draws a line between two points.
    /// </summary>
    /// <param name="x1">The X position of the first point.</param>
    /// <param name="y1">The Y position of the first point.</param>
    /// <param name="x2">The X position of the second point.</param>
    /// <param name="y2">The Y position of the second point.</param>
    /// <param name="c">The color to draw the line with.</param>
    public void DrawLine(int x1, int y1, int x2, int y2, Color c)
    {
        DrawLine(x1, y1, x2, y2, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Draws a line between two points.
    /// </summary>
    /// <param name="x1">The X position of the first point.</param>
    /// <param name="y1">The Y position of the first point.</param>
    /// <param name="x2">The X position of the second point.</param>
    /// <param name="y2">The Y position of the second point.</param>
    /// <param name="r">The Red component of the color to draw the line with.</param>
    /// <param name="g">The Green component of the color to draw the line with.</param>
    /// <param name="b">The Blue component of the color to draw the line with.</param>
    /// <param name="a">The Alpha component of the color to draw the line with.</param>
    public virtual void DrawLine(int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a = 255)
    {
        if (Locked) throw new BitmapLockedException();
        if (x1 < 0 || x2 < 0 || x1 >= Width || x2 >= Width ||
            y1 < 0 || y2 < 0 || y1 >= Height || y2 >= Height) throw new Exception($"Line out of bounds.");
        for (int x = x1 > x2 ? x2 : x1; (x1 > x2) ? (x <= x1) : (x <= x2); x++)
        {
            double fact = ((double)x - x1) / (x2 - x1);
            int y = (int)Math.Round(y1 + ((y2 - y1) * fact));
            if (y >= 0) SetPixel(x, y, r, g, b, a);
        }
        int sy = y1 > y2 ? y2 : y1;
        for (int y = y1 > y2 ? y2 : y1; (y1 > y2) ? (y <= y1) : (y <= y2); y++)
        {
            double fact = ((double)y - y1) / (y2 - y1);
            int x = (int)Math.Round(x1 + ((x2 - x1) * fact));
            if (x >= 0) SetPixel(x, y, r, g, b, a);
        }
        if (this.Renderer != null) this.Renderer.Update();
    }

    /// <summary>
    /// Draws lines between the given points.
    /// </summary>
    /// <param name="c">The color to draw the lines with.</param>
    /// <param name="points">The list of points to draw the lines between.</param>
    public void DrawLines(Color c, params Point[] points)
    {
        this.DrawLines(c.Red, c.Green, c.Blue, c.Alpha, points);
    }

    /// <summary>
    /// Draws lines between the given points.
    /// </summary>
    /// <param name="r">The Red component of the color to draw the lines with.</param>
    /// <param name="g">The Green component of the color to draw the lines with.</param>
    /// <param name="b">The Blue component of the color to draw the lines with.</param>
    /// <param name="points">The list of points to draw the lines between.</param>
    public void DrawLines(byte r, byte g, byte b, params Point[] points)
    {
        this.DrawLines(r, g, b, 255, points);
    }

    /// <summary>
    /// Draws lines between the given points.
    /// </summary>
    /// <param name="r">The Red component of the color to draw the lines with.</param>
    /// <param name="g">The Green component of the color to draw the lines with.</param>
    /// <param name="b">The Blue component of the color to draw the lines with.</param>
    /// <param name="a">The Alpha component of the color to draw the lines with.</param>
    /// <param name="points">The list of points to draw the lines between.</param>
    public virtual void DrawLines(byte r, byte g, byte b, byte a, params Point[] points)
    {
        if (Locked) throw new BitmapLockedException();
        for (int i = 0; i < points.Length - 1; i++)
        {
            this.DrawLine(points[i], points[i + 1], r, g, b, a);
        }
    }
}

