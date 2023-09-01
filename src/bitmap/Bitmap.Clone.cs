using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Creates a clone of the bitmap.
    /// </summary>
    public virtual Bitmap Clone()
    {
        if (IsChunky)
        {
            throw new Exception("Cannot clone chunky Bitmap yet!");
        }
        else
        {
            Bitmap bmp = new Bitmap(Width, Height);
            bmp.Unlock();
            bmp.Build(this, BlendMode.None);
            bmp.Lock();
            bmp.Font = this.Font;
            bmp.Renderer = this.Renderer;
            return bmp;
        }
    }
}

