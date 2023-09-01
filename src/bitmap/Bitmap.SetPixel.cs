using System;
using System.Runtime.InteropServices;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Sets a pixel in the bitmap to the specified color.
    /// </summary>
    /// <param name="p">The position in the bitmap.</param>
    /// <param name="c">The color to set the pixel to.</param>
    public void SetPixel(Point p, Color c)
    {
        SetPixel(p.X, p.Y, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Sets a pixel in the bitmap to the specified color.
    /// </summary>
    /// <param name="X">The X position in the bitmap.</param>
    /// <param name="Y">The Y position in the bitmap.</param>
    /// <param name="c">The color to set the pixel to.</param>
    public void SetPixel(int X, int Y, Color c)
    {
        SetPixel(X, Y, c.Red, c.Green, c.Blue, c.Alpha);
    }

    /// <summary>
    /// Sets a pixel in the bitmap to the specified color.
    /// </summary>
    /// <param name="p">The position in the bitmap.</param>
    /// <param name="r">The Red component of the color to set the pixel to.</param>
    /// <param name="g">The Green component of the color to set the pixel to.</param>
    /// <param name="b">The Blue component of the color to set the pixel to.</param>
    /// <param name="a">The Alpha component of the color to set the pixel to.</param>
    public void SetPixel(Point p, byte r, byte g, byte b, byte a = 255)
    {
        SetPixel(p.X, p.Y, r, g, b, a);
    }

    /// <summary>
    /// Sets a pixel in the bitmap to the specified color.
    /// </summary>
    /// <param name="X">The X position in the bitmap.</param>
    /// <param name="Y">The Y position in the bitmap.</param>
    /// <param name="r">The Red component of the color to set the pixel to.</param>
    /// <param name="g">The Green component of the color to set the pixel to.</param>
    /// <param name="b">The Blue component of the color to set the pixel to.</param>
    /// <param name="a">The Alpha component of the color to set the pixel to.</param>
    public virtual unsafe void SetPixel(int X, int Y, byte r, byte g, byte b, byte a = 255)
    {
        if (Locked) throw new BitmapLockedException();
        if (X < 0 || Y < 0) throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- minimum is (0,0)");
        if (X >= this.Width || Y >= this.Height) throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
        if (IsChunky)
        {
            Bitmap bmp = GetBitmapFromCoordinate(X, Y);
            if (bmp.Locked) bmp.Unlock();
            bmp.SetPixel(X - bmp.InternalX, Y - bmp.InternalY, r, g, b, a);
        }
        else
        {
            int num = (a << 24) + (b << 16) + (g << 8) + r;
            Marshal.WriteInt32((nint)PixelPointer + Width * Y * 4 + X * 4, num);
        }
        if (this.Renderer != null) this.Renderer.Update();
    }

    /// <summary>
    /// Performs purely byte assignment, and no safety or validity checks. Faster when used in bulk, but more dangerous. This overload does not change the alpha value of the pixel.
    /// </summary>
    /// <param name="X">The X position in the bitmap.</param>
    /// <param name="Y">The Y position in the bitmap.</param>
    /// <param name="r">The Red component of the color to set the pixel to.</param>
    /// <param name="g">The Green component of the color to set the pixel to.</param>
    /// <param name="b">The Blue component of the color to set the pixel to.</param>
    public virtual unsafe void SetPixelFast(int X, int Y, byte r, byte g, byte b)
    {
        SetPixelFast(X, Y, r, g, b, 255);
    }

    /// <summary>
    /// Performs purely byte assignment, and no safety or validity checks. Faster when used in bulk, but more dangerous. This overload does not change the alpha value of the pixel.
    /// </summary>
    /// <param name="X">The X position in the bitmap.</param>
    /// <param name="Y">The Y position in the bitmap.</param>
    /// <param name="Color">The color to set the pixel to.</param>
    public virtual unsafe void SetPixelFast(int X, int Y, Color Color)
    {
        SetPixelFast(X, Y, Color.Red, Color.Green, Color.Blue, Color.Alpha);
    }

    /// <summary>
    /// Performs purely byte assignment, and no safety or validity checks. Faster when used in bulk, but more dangerous.
    /// </summary>
    /// <param name="X">The X position in the bitmap.</param>
    /// <param name="Y">The Y position in the bitmap.</param>
    /// <param name="r">The Red component of the color to set the pixel to.</param>
    /// <param name="g">The Green component of the color to set the pixel to.</param>
    /// <param name="b">The Blue component of the color to set the pixel to.</param>
    /// <param name="a">The Alpha component of the color to set the pixel to.</param>
    public virtual unsafe void SetPixelFast(int X, int Y, byte r, byte g, byte b, byte a)
    {
        int num = (a << 24) + (b << 16) + (g << 8) + r;
        Marshal.WriteInt32((nint)PixelPointer + Width * Y * 4 + X * 4, num);
    }
}

