using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Applies a hue to the bitmap.
    /// </summary>
    /// <param name="Hue">The hue (0-360) to apply.</param>
    /// <returns>A new bitmap with the hue applied.</returns>
    public virtual Bitmap ApplyHue(int Hue)
    {
        Bitmap bmp = new Bitmap(Width, Height);
        bmp.Unlock();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Color c = GetPixelFast(x, y);
                (float H, float S, float L) HSL = c.GetHSL();
                HSL.H = (HSL.H + Hue) % 360;
                c.SetHSL(HSL);
                bmp.SetPixelFast(x, y, c.Red, c.Green, c.Blue, c.Alpha);
            }
        }
        bmp.Lock();
        return bmp;
    }

    /// <summary>
    /// Applies a hue to a particular region of the bitmap.
    /// </summary>
    /// <param name="Hue">The hue (0-360) to apply.</param>
    /// <param name="OX">The X position of the rectangle to apply the hue to.</param>
    /// <param name="OY">The Y position of the rectangle to apply the hue to.</param>
    /// <param name="Width">The width of the rectangle to apply the hue to.</param>
    /// <param name="Height">The height of the rectangle to apply the hue to.</param>
    /// <returns>A new bitmap with the hue applied.</returns>
    public virtual Bitmap ApplyHue(int Hue, int OX, int OY, int Width, int Height)
    {
        Bitmap bmp = new Bitmap(Width, Height);
        bmp.Unlock();
        for (int y = OY; y < Height; y++)
        {
            for (int x = OX; x < Width; x++)
            {
                Color c = GetPixelFast(x, y);
                (float H, float S, float L) HSL = c.GetHSL();
                HSL.H = (HSL.H + Hue) % 360;
                c.SetHSL(HSL);
                bmp.SetPixelFast(x - OX, y - OY, c.Red, c.Green, c.Blue, c.Alpha);
            }
        }
        bmp.Lock();
        return bmp;
    }
}

