using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Saves the current bitmap to a file as a PNG.
    /// </summary>
    /// <param name="filename">The filename to save the bitmap as.</param>
    public virtual unsafe void SaveToPNG(string filename, bool Transparency = true, bool Indexed = false, int MaxPaletteSize = 0)
    {
        if (IsChunky)
        {
            throw new Exception("Cannot save chunky bitmap to PNG!");
        }
        else
        {
            decodl.PNGEncoder encoder = new decodl.PNGEncoder(PixelPointer, (uint)Width, (uint)Height);
            encoder.ColorType = Transparency ? decodl.ColorTypes.RGBA : decodl.ColorTypes.RGB;
            if (Indexed)
            {
                encoder.ColorType = decodl.ColorTypes.Indexed;
                encoder.ReduceUnindexableImages = true;
                encoder.MaxPaletteSize = MaxPaletteSize;
                encoder.IncludeIndexedTransparency = Transparency;
            }
            encoder.Encode(filename);
        }
    }
}

