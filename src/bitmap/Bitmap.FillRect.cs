using System;
using static odl.SDL2.SDL;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="r">The rectangle to fill.</param>
    /// <param name="c">The color to fill the rectangle with.</param>
    public void FillRect(Rect r, Color c)
    {
        FillRect(r.X, r.Y, r.Width, r.Height, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="rect">The rectangle to fill.</param>
    /// <param name="r">The Red component of the color to fill the rectangle with.</param>
    /// <param name="g">The Green component of the color to fill the rectangle with.</param>
    /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
    public void FillRect(Rect rect, byte r, byte g, byte b, byte a = 255)
    {
        FillRect(rect.X, rect.Y, rect.Width, rect.Height, r, g, b, a);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="Point">The position of the rectangle.</param>
    /// <param name="Size">The size of the rectangle.</param>
    /// <param name="c">The color to fill the rectangle with.</param>
    public void FillRect(Point Point, Size Size, Color c)
    {
        FillRect(Point.X, Point.Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="Point">The position of the rectangle.</param>
    /// <param name="Size">The size of the rectangle.</param>
    /// <param name="r">The Red component of the color to fill the rectangle with.</param>
    /// <param name="g">The Green component of the color to fill the rectangle with.</param>
    /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
    public void FillRect(Point Point, Size Size, byte r, byte g, byte b, byte a = 255)
    {
        FillRect(Point.X, Point.Y, Size.Width, Size.Height, r, g, b, a);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="Point">The position of the rectangle.</param>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="c">The color to fill the rectangle with.</param>
    public void FillRect(Point Point, int Width, int Height, Color c)
    {
        FillRect(Point.X, Point.Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="Point">The position of the rectangle.</param>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="r">The Red component of the color to fill the rectangle with.</param>
    /// <param name="g">The Green component of the color to fill the rectangle with.</param>
    /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
    public void FillRect(Point Point, int Width, int Height, byte r, byte g, byte b, byte a = 255)
    {
        FillRect(Point.X, Point.Y, Width, Height, r, g, b, a);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="r">The Red component of the color to fill the rectangle with.</param>
    /// <param name="g">The Green component of the color to fill the rectangle with.</param>
    /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
    public void FillRect(Size size, byte r, byte g, byte b, byte a = 255)
    {
        this.FillRect(0, 0, size, r, g, b, a);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="c">The color to fill the rectangle with.</param>
    public void FillRect(Size size, Color c)
    {
        this.FillRect(0, 0, size, c);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="r">The Red component of the color to fill the rectangle with.</param>
    /// <param name="g">The Green component of the color to fill the rectangle with.</param>
    /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
    public void FillRect(int Width, int Height, byte r, byte g, byte b, byte a = 255)
    {
        this.FillRect(0, 0, Width, Height, r, g, b, a);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="c">The color to fill the rectangle with.</param>
    public void FillRect(int Width, int Height, Color c)
    {
        this.FillRect(0, 0, Width, Height, c);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="X">The X position of the rectangle.</param>
    /// <param name="Y">The Y position of the rectangle.</param>
    /// <param name="Size">The size of the rectangle.</param>
    /// <param name="c">The color to fill the rectangle with.</param>
    public void FillRect(int X, int Y, Size Size, Color c)
    {
        FillRect(X, Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="X">The X position of the rectangle.</param>
    /// <param name="Y">The Y position of the rectangle.</param>
    /// <param name="Size">The size of the rectangle.</param>
    /// <param name="r">The Red component of the color to fill the rectangle with.</param>
    /// <param name="g">The Green component of the color to fill the rectangle with.</param>
    /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
    public void FillRect(int X, int Y, Size Size, byte r, byte g, byte b, byte a = 255)
    {
        FillRect(X, Y, Size.Width, Size.Height, r, g, b, a);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="X">The X position of the rectangle.</param>
    /// <param name="Y">The Y position of the rectangle.</param>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="c">The color to fill the rectangle with.</param>
    public void FillRect(int X, int Y, int Width, int Height, Color c)
    {
        FillRect(X, Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Fills a rectangle with a solid color.
    /// </summary>
    /// <param name="X">The X position of the rectangle.</param>
    /// <param name="Y">The Y position of the rectangle.</param>
    /// <param name="Width">The width of the rectangle.</param>
    /// <param name="Height">The height of the rectangle.</param>
    /// <param name="r">The Red component of the color to fill the rectangle with.</param>
    /// <param name="g">The Green component of the color to fill the rectangle with.</param>
    /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
    /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
    public virtual void FillRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255)
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
        Rect DestRect = new Rect(X, Y, Width, Height);
        if (IsChunky)
        {
            foreach (Bitmap bmp in this.InternalBitmaps)
            {
                Rect bmprect = new Rect(bmp.InternalX, bmp.InternalY, this.ChunkSize);
                if (DestRect.Overlaps(bmprect))
                {
                    int nx = Math.Max(DestRect.X, bmprect.X);
                    int ny = Math.Max(DestRect.Y, bmprect.Y);
                    int nw = Math.Min(DestRect.X + DestRect.Width, bmprect.X + bmprect.Width) - nx;
                    int nh = Math.Min(DestRect.Y + DestRect.Height, bmprect.Y + bmprect.Height) - ny;
                    if (bmp.Locked) bmp.Unlock();
                    int DX = nx - bmp.InternalX;
                    int DY = ny - bmp.InternalY;
                    int DW = nw;
                    int DH = nh;
                    bmp.FillRect(DX, DY, nw, nh, r, g, b, a);
                }
            }
        }
        else
        {
            SDL_Rect Rect = DestRect.SDL_Rect;
            SDL_FillRect(this.Surface, ref Rect, SDL_MapRGBA(this.SurfaceObject.format, r, g, b, a));
        }
        if (this.Renderer != null) this.Renderer.Update();
    }
}

