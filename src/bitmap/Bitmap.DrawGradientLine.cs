using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Draws a line with its color linearly interpolated between two colors.
    /// </summary>
    /// <param name="p1">The starting position of the line.</param>
    /// <param name="p2">The ending position of the line.</param>
    /// <param name="c1">The starting color of the line.</param>
    /// <param name="c2">The ending color of the line.</param>
    public void DrawGradientLine(Point p1, Point p2, Color c1, Color c2)
    {
        DrawGradientLine(p1.X, p1.Y, p2.X, p2.Y, c1, c2);
    }

    /// <summary>
    /// Draws a line with its color linearly interpolated between two colors.
    /// </summary>
    /// <param name="x1">The starting X position of the line.</param>
    /// <param name="y1">The starting Y position of the line.</param>
    /// <param name="p2">The ending position of the line.</param>
    /// <param name="c1">The starting color of the line.</param>
    /// <param name="c2">The ending color of the line.</param>
    public void DrawGradientLine(int x1, int y1, Point p2, Color c1, Color c2)
    {
        DrawGradientLine(x1, y1, p2.X, p2.Y, c1, c2);
    }

    /// <summary>
    /// Draws a line with its color linearly interpolated between two colors.
    /// </summary>
    /// <param name="p1">The starting position of the line.</param>
    /// <param name="x2">The ending X position of the line.</param>
    /// <param name="y2">The ending Y position of the line.</param>
    /// <param name="c1">The starting color of the line.</param>
    /// <param name="c2">The ending color of the line.</param>
    public void DrawGradientLine(Point p1, int x2, int y2, Color c1, Color c2)
    {
        DrawGradientLine(p1.X, p1.Y, x2, y2, c1, c2);
    }

    /// <summary>
    /// Draws a line with its color linearly interpolated between two colors.
    /// </summary>
    /// <param name="x1">The starting X position of the line.</param>
    /// <param name="y1">The starting Y position of the line.</param>
    /// <param name="x2">The ending X position of the line.</param>
    /// <param name="y2">The ending Y position of the line.</param>
    /// <param name="c1">The starting color of the line.</param>
    /// <param name="c2">The ending color of the line.</param>
    public virtual void DrawGradientLine(int x1, int y1, int x2, int y2, Color c1, Color c2)
    {
        if (Locked) throw new BitmapLockedException();
        if (x1 < 0 || x2 < 0 || x1 >= Width || x2 >= Width ||
            y1 < 0 || y2 < 0 || y1 >= Height || y2 >= Height) throw new Exception($"Line out of bounds.");
        if (x1 != x2)
        {
            for (int x = x1 > x2 ? x2 : x1; (x1 > x2) ? (x <= x1) : (x <= x2); x++)
            {
                double fact = ((double)x - x1) / (x2 - x1);
                int y = (int)Math.Round(y1 + ((y2 - y1) * fact));
                if (y >= 0)
                {
                    long d1 = (long)Math.Sqrt(Math.Pow(x - x1, 2) + Math.Pow(y - y1, 2));
                    long d2 = (long)Math.Sqrt(Math.Pow(x - x2, 2) + Math.Pow(y - y2, 2));
                    double f1 = d2 / (double)(d1 + d2);
                    double f2 = d1 / (double)(d1 + d2);
                    byte r = (byte)Math.Round(f1 * c1.Red + f2 * c2.Red);
                    byte g = (byte)Math.Round(f1 * c1.Green + f2 * c2.Green);
                    byte b = (byte)Math.Round(f1 * c1.Blue + f2 * c2.Blue);
                    byte a = (byte)Math.Round(f1 * c1.Alpha + f2 * c2.Alpha);
                    SetPixel(x, y, r, g, b, a);
                }
            }
        }
        int sy = y1 > y2 ? y2 : y1;
        if (y1 != y2)
        {
            for (int y = y1 > y2 ? y2 : y1; (y1 > y2) ? (y <= y1) : (y <= y2); y++)
            {
                double fact = ((double)y - y1) / (y2 - y1);
                int x = (int)Math.Round(x1 + ((x2 - x1) * fact));
                if (x >= 0)
                {
                    long d1 = (long)(Math.Pow(x - x1, 2) + Math.Pow(y - y1, 2));
                    long d2 = (long)(Math.Pow(x - x2, 2) + Math.Pow(y - y2, 2));
                    double f1 = d2 / (double)(d1 + d2);
                    double f2 = d1 / (double)(d1 + d2);
                    byte r = (byte)Math.Round(f1 * c1.Red + f2 * c2.Red);
                    byte g = (byte)Math.Round(f1 * c1.Green + f2 * c2.Green);
                    byte b = (byte)Math.Round(f1 * c1.Blue + f2 * c2.Blue);
                    byte a = (byte)Math.Round(f1 * c1.Alpha + f2 * c2.Alpha);
                    SetPixel(x, y, r, g, b, a);
                }
            }
        }
        if (this.Renderer != null) this.Renderer.Update();
    }
}

