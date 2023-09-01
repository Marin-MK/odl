using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Clears the bitmap content.
    /// </summary>
    public virtual void Clear()
    {
        if (IsChunky)
        {
            foreach (Bitmap b in this.InternalBitmaps) b.Clear();
        }
        else
        {
            // Filling an alpha rectangle is faster than recreating the bitmap.
            FillRect(0, 0, this.Width, this.Height, Color.ALPHA);
        }
    }
}

