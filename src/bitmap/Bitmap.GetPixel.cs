using System;
using System.Runtime.InteropServices;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Returns the color at the given position.
    /// </summary>
    /// <param name="p">The position in the bitmap.</param>
    public Color GetPixel(Point p)
    {
        return GetPixel(p.X, p.Y);
    }

    /// <summary>
    /// Returns the color at the given position.
    /// </summary>
    /// <param name="X">The X position in the bitmap.</param>
    /// <param name="Y">The Y position in the bitmap.</param>
    public virtual unsafe Color GetPixel(int X, int Y)
    {
        if (X < 0 || Y < 0) throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- minimum is (0,0)");
        if (X >= this.Width || Y >= this.Height) throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
        if (IsChunky)
        {
            Bitmap bmp = GetBitmapFromCoordinate(X, Y);
            return bmp.GetPixel(X - bmp.InternalX, Y - bmp.InternalY);
        }
        else
        {
            int num = Marshal.ReadInt32((nint)PixelPointer + Width * Y * 4 + X * 4);
            return new Color(
                (byte)(num & 0xFF),
                (byte)((num >> 8) & 0xFF),
                (byte)((num >> 16) & 0xFF),
                (byte)((num >> 24) & 0xFF)
            );
        }
    }

    /// <summary>
    /// Performs purely byte reading, and no safety or validity checks. Faster when used in bulk, but more dangerous.
    /// </summary>
    /// <param name="X">The X position in the bitmap.</param>
    /// <param name="Y">The Y position in the bitmap.</param>
    public virtual unsafe Color GetPixelFast(int X, int Y)
    {
        int num = Marshal.ReadInt32((nint)PixelPointer, Width * Y * 4 + X * 4);
        return new Color(
            (byte)(num & 0xFF),
            (byte)((num >> 8) & 0xFF),
            (byte)((num >> 16) & 0xFF),
            (byte)((num >> 24) & 0xFF)
        );
    }
}

