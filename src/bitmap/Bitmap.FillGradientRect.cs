using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Rect">The rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="c3">The color in the bottom-left corner of the rectangle.</param>
    /// <param name="c4">The color in the bottom-right corner of the rectangle.</param>
    public void FillGradientRect(Rect Rect, Color c1, Color c2, Color c3, Color c4)
    {
        FillGradientRect(Rect.X, Rect.Y, Rect.Width, Rect.Height, c1, c2, c3, c4);
    }

    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Point">The position of the rectangle to fill.</param>
    /// <param name="Size">The size of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="c3">The color in the bottom-left corner of the rectangle.</param>
    /// <param name="c4">The color in the bottom-right corner of the rectangle.</param>
    public void FillGradientRect(Point Point, Size Size, Color c1, Color c2, Color c3, Color c4)
    {
        FillGradientRect(Point.X, Point.Y, Size.Width, Size.Height, c1, c2, c3, c4);
    }

    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="X">The X position of the rectangle to fill.</param>
    /// <param name="Y">The Y position of the rectangle to fill.</param>
    /// <param name="Size">The size of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="c3">The color in the bottom-left corner of the rectangle.</param>
    /// <param name="c4">The color in the bottom-right corner of the rectangle.</param>
    public void FillGradientRect(int X, int Y, Size Size, Color c1, Color c2, Color c3, Color c4)
    {
        FillGradientRect(X, Y, Size.Width, Size.Height, c1, c2, c3, c4);
    }

    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Point">The position of the rectangle to fill.</param>
    /// <param name="Width">The width of the rectangle to fill.</param>
    /// <param name="Height">The height of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="c3">The color in the bottom-left corner of the rectangle.</param>
    /// <param name="c4">The color in the bottom-right corner of the rectangle.</param>
    public void FillGradientRect(Point Point, int Width, int Height, Color c1, Color c2, Color c3, Color c4)
    {
        FillGradientRect(Point.X, Point.Y, Width, Height, c1, c2, c3, c4);
    }

    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Rect">The rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="Flipped">Whether to flip the direction of the gradient on the horizontal axis.</param>
    /// <param name="UseTriangles">Whether to draw the rectangle using two triangles for a smooth gradient, or to use a simple rectangular function.</param>
    public void FillGradientRect(Rect Rect, Color c1, Color c2, bool Flipped = false, bool UseTriangles = true)
    {
        FillGradientRect(Rect.X, Rect.Y, Rect.Width, Rect.Height, c1, c2, Flipped, UseTriangles);
    }

    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Point">The position of the rectangle to fill.</param>
    /// <param name="Size">The size of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="Flipped">Whether to flip the direction of the gradient on the horizontal axis.</param>
    /// <param name="UseTriangles">Whether to draw the rectangle using two triangles for a smooth gradient, or to use a simple rectangular function.</param>
    public void FillGradientRect(Point Point, Size Size, Color c1, Color c2, bool Flipped = false, bool UseTriangles = true)
    {
        FillGradientRect(Point.X, Point.Y, Size.Width, Size.Height, c1, c2, Flipped, UseTriangles);
    }

    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="X">The X position of the rectangle to fill.</param>
    /// <param name="Y">The Y position of the rectangle to fill.</param>
    /// <param name="Size">The size of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="Flipped">Whether to flip the direction of the gradient on the horizontal axis.</param>
    /// <param name="UseTriangles">Whether to draw the rectangle using two triangles for a smooth gradient, or to use a simple rectangular function.</param>
    public void FillGradientRect(int X, int Y, Size Size, Color c1, Color c2, bool Flipped = false, bool UseTriangles = true)
    {
        FillGradientRect(X, Y, Size.Width, Size.Height, c1, c2, Flipped, UseTriangles);
    }

    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Point">The position of the rectangle to fill.</param>
    /// <param name="Width">The width of the rectangle to fill.</param>
    /// <param name="Height">The height of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="Flipped">Whether to flip the direction of the gradient on the horizontal axis.</param>
    /// <param name="UseTriangles">Whether to draw the rectangle using two triangles for a smooth gradient, or to use a simple rectangular function.</param>
    public void FillGradientRect(Point Point, int Width, int Height, Color c1, Color c2, bool Flipped = false, bool UseTriangles = true)
    {
        FillGradientRect(Point.X, Point.Y, Width, Height, c1, c2, Flipped, UseTriangles);
    }

    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="X">The X position of the rectangle to fill.</param>
    /// <param name="Y">The Y position of the rectangle to fill.</param>
    /// <param name="Width">The width of the rectangle to fill.</param>
    /// <param name="Height">The height of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="c3">The color in the bottom-left corner of the rectangle.</param>
    /// <param name="c4">The color in the bottom-right corner of the rectangle.</param>
    public virtual void FillGradientRect(int X, int Y, int Width, int Height, Color c1, Color c2, Color c3, Color c4)
    {
        if (Locked) throw new BitmapLockedException();
        if (X < 0 || Y < 0)
        {
            throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- minimum is (0,0)");
        }
        if (X >= this.Width || Y >= this.Height)
        {
            throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
        }
        if (X + Width - 1 >= this.Width || Y + Height - 1 >= this.Height)
        {
            throw new Exception($"Invalid rectangle ({X},{Y},{Width},{Height}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
        }
        for (int dy = Y; dy < Y + Height; dy++)
        {
            for (int dx = X; dx < X + Width; dx++)
            {
                double xl = dx - X;
                double xr = X + Width - 1 - dx;
                double yt = dy - Y;
                double yb = Y + Height - 1 - dy;
                double fxr = (xl / (xl + xr));
                double fxl = 1 - fxr;
                double fyb = (yt / (yt + yb));
                double fyt = 1 - fyb;
                double f1 = fxl * fyt;
                double f2 = fxr * fyt;
                double f3 = fxl * fyb;
                double f4 = fxr * fyb;
                byte r = (byte)Math.Round(f1 * c1.Red + f2 * c2.Red + f3 * c3.Red + f4 * c4.Red);
                byte g = (byte)Math.Round(f1 * c1.Green + f2 * c2.Green + f3 * c3.Green + f4 * c4.Green);
                byte b = (byte)Math.Round(f1 * c1.Blue + f2 * c2.Blue + f3 * c3.Blue + f4 * c4.Blue);
                byte a = (byte)Math.Round(f1 * c1.Alpha + f2 * c2.Alpha + f3 * c3.Alpha + f4 * c4.Alpha);
                SetPixel(dx, dy, r, g, b, a);
            }
        }
        if (this.Renderer != null) this.Renderer.Update();
    }

    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="X">The X position of the rectangle to fill.</param>
    /// <param name="Y">The Y position of the rectangle to fill.</param>
    /// <param name="Width">The width of the rectangle to fill.</param>
    /// <param name="Height">The height of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="Flipped">Whether to flip the direction of the gradient on the horizontal axis.</param>
    /// <param name="UseTriangles">Whether to draw the rectangle using two triangles for a smooth gradient, or to use a simple rectangular function.</param>
    public virtual void FillGradientRect(int X, int Y, int Width, int Height, Color c1, Color c2, bool Flipped = false, bool UseTriangles = true)
    {
        if (Locked) throw new BitmapLockedException();
        if (X < 0 || Y < 0)
        {
            throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- minimum is (0,0)");
        }
        if (X >= this.Width || Y >= this.Height)
        {
            throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
        }
        if (X + Width - 1 >= this.Width || Y + Height - 1 >= this.Height)
        {
            throw new Exception($"Invalid rectangle ({X},{Y},{Width},{Height}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
        }
        if (UseTriangles)
        {
            Color half = Interpolate2D(c1, c2, 0.5);
            FillGradientTriangle(
                new Vertex(X, Y + Height - 1, Flipped ? c2 : half),
                new Vertex(X, Y, Flipped ? half : c1),
                new Vertex(X + Width - 1, Y, Flipped ? c1 : half)
            );
            FillGradientTriangle(
                new Vertex(X, Y + Height - 1, Flipped ? c2 : half),
                new Vertex(X + Width - 1, Y, Flipped ? c1 : half),
                new Vertex(X + Width - 1, Y + Height - 1, Flipped ? half : c2)
            );
        }
        else
        {
            for (int dy = Y; dy < Y + Height; dy++)
            {
                for (int dx = X; dx < X + Width; dx++)
                {
                    long d1 = (long)(Math.Pow(dx - X, 2) + Math.Pow(dy - (Flipped ? Y + Height : Y), 2));
                    long d2 = (long)(Math.Pow(dx - (X + Width), 2) + Math.Pow(dy - (Flipped ? Y : Y + Height), 2));
                    double f1 = d2 / (double)(d1 + d2);
                    double f2 = d1 / (double)(d1 + d2);
                    byte r = (byte)Math.Round(f1 * c1.Red + f2 * c2.Red);
                    byte g = (byte)Math.Round(f1 * c1.Green + f2 * c2.Green);
                    byte b = (byte)Math.Round(f1 * c1.Blue + f2 * c2.Blue);
                    byte a = (byte)Math.Round(f1 * c1.Alpha + f2 * c2.Alpha);
                    SetPixel(dx, dy, r, g, b, a);
                }
            }
        }
        if (this.Renderer != null) this.Renderer.Update();
    }

    /// <summary>
    /// Draws a gradient between the <paramref name="inside"/> box and <paramref name="outside"/> box.
    /// </summary>
    /// <param name="outside">The outer box at which the gradient stops.</param>
    /// <param name="inside">The inner box from where to start the gradient.</param>
    /// <param name="c1">The inner-most color.</param>
    /// <param name="c2">The outer-most color.</param>
    /// <param name="FillInside">Whether to also fill the inner rectangle with <paramref name="c1"/>.</param>
    public virtual void FillGradientRectOutside(Rect outside, Rect inside, Color c1, Color c2, bool FillInside = true)
    {
        for (int y = outside.Y; y < outside.Y + outside.Height; y++)
        {
            for (int x = outside.X; x < outside.X + outside.Width; x++)
            {
                if (inside.Contains(x, y)) continue;
                double d = -1;
                if (x < inside.X && y >= inside.Y && y <= inside.Y + inside.Height)
                    d = (x - outside.X) / (double)(inside.X - outside.X - 1);
                else if (x >= inside.X + inside.Width && y >= inside.Y && y <= inside.Y + inside.Height)
                    d = 1 - (x - inside.X - inside.Width) / (double)(outside.X + outside.Width - inside.X - inside.Width - 1);
                else if (y < inside.Y && x >= inside.X && x <= inside.X + inside.Width)
                    d = (y - outside.Y) / (double)(inside.Y - outside.Y - 1);
                else if (y >= inside.Y + inside.Height && x >= inside.X && x <= inside.X + inside.Width)
                    d = 1 - (y - inside.Y - inside.Height) / (double)(outside.Y + outside.Height - inside.Y - inside.Height - 1);
                if (d == -1) continue;
                d = Math.Clamp(d, 0, 1);
                byte r = (byte)Math.Round(d * c1.Red + (1 - d) * c2.Red);
                byte g = (byte)Math.Round(d * c1.Green + (1 - d) * c2.Green);
                byte b = (byte)Math.Round(d * c1.Blue + (1 - d) * c2.Blue);
                byte a = (byte)Math.Round(d * c1.Alpha + (1 - d) * c2.Alpha);
                SetPixel(x, y, r, g, b, a);
            }
        }
        if (FillInside) FillRect(inside, c1);
        FillGradientRect(outside.X, outside.Y, inside.X - outside.X, inside.Y - outside.Y, c2, c2, c2, c1);
        FillGradientRect(inside.X + inside.Width, outside.Y, outside.X + outside.Width - (inside.X + inside.Width), inside.Y - outside.Y, c2, c2, c1, c2);
        FillGradientRect(outside.X, inside.Y + inside.Height, inside.X - outside.X, outside.Y + outside.Height - (inside.Y + inside.Height), c2, c1, c2, c2);
        FillGradientRect(inside.X + inside.Width, inside.Y + inside.Height, outside.X + outside.Width - (inside.X + inside.Width), outside.Y + outside.Height - inside.Y - inside.Height, c1, c2, c2, c2);
    }
}

