using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Creates a bitmap with a mask.
    /// </summary>
    /// <param name="Source">The bitmap to sample.</param>
    /// <param name="Mask">The bitmap whose alpha values will be checked for masking.</param>
    /// <param name="XOffset">An x offset for sampling.</param>
    /// <param name="YOffset">An y ofset for sampling.</param>
    public static Bitmap Mask(Bitmap Mask, Bitmap Source, int XOffset = 0, int YOffset = 0)
    {
        return Bitmap.Mask(Mask, Source, new Rect(0, 0, Source.Width, Source.Height), XOffset, YOffset);
    }

    /// <summary>
    /// Creates a bitmap with a mask.
    /// </summary>
    /// <param name="Source">The bitmap to sample.</param>
    /// <param name="Mask">The bitmap whose alpha values will be checked for masking.</param>
    /// <param name="SourceRect">The rectangle inside the Source bitmap to sample from.</param>
    /// <param name="XOffset">An x offset for sampling, within the source rectangle.</param>
    /// <param name="YOffset">An y ofset for sampling, within the source rectangle.</param>
    public static unsafe Bitmap Mask(Bitmap Mask, Bitmap Source, Rect SourceRect, int XOffset = 0, int YOffset = 0)
    {
        byte* sourcepixels = Source.PixelPointer;
        byte* maskpixels = Mask.PixelPointer;
        Bitmap Result = new Bitmap(Mask.Width, Mask.Height);
        Result.Unlock();
        byte* resultpixels = Result.PixelPointer;
        for (int x = 0; x < Mask.Width; x++)
        {
            for (int y = 0; y < Mask.Height; y++)
            {
                byte maskalpha = maskpixels[y * Mask.Width * 4 + x * 4 + 3];
                if (maskalpha == 0) continue;
                int srcx = SourceRect.X + (x + XOffset) % SourceRect.Width;
                int srcy = SourceRect.Y + (y + YOffset) % SourceRect.Height;
                resultpixels[y * Result.Width * 4 + x * 4] = sourcepixels[srcy * Source.Width * 4 + srcx * 4];
                resultpixels[y * Result.Width * 4 + x * 4 + 1] = sourcepixels[srcy * Source.Width * 4 + srcx * 4 + 1];
                resultpixels[y * Result.Width * 4 + x * 4 + 2] = sourcepixels[srcy * Source.Width * 4 + srcx * 4 + 2];
                resultpixels[y * Result.Width * 4 + x * 4 + 3] = sourcepixels[srcy * Source.Width * 4 + srcx * 4 + 3];
            }
        }
        Result.Lock();
        return Result;
    }
}

