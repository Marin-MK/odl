using System;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Applies a box blur filter on the original image.
    /// </summary>
    /// <param name="Weight">The weight or size of the blur. Runtime increases exponentially with the weight. Must be at least 1.</param>
    /// <param name="TransparentEdges">If true, non-existent pixels for filtering near edges are seen as transparent. If false, a the filter weight is reduced locally.</param>
    /// <returns>The new blurred bitmap.</returns>
    public Bitmap Blur(int Weight = 1, float Scale = 1, bool TransparentEdges = true)
    {
        if (Weight < 1) throw new Exception("Blur weight must be at least 1.");
        Bitmap bmp = new Bitmap(Width, Height);
        bmp.Unlock();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float Red = 0,
                    Green = 0,
                    Blue = 0,
                    Alpha = 0;
                int count = 0;
                for (int dy = y - Weight; dy <= y + Weight; dy++)
                {
                    for (int dx = x - Weight; dx <= x + Weight; dx++)
                    {
                        if (dx >= 0 && dx < Width && dy >= 0 && dy < Height)
                        {
                            Color c = GetPixel(dx, dy);
                            Red += c.Red;
                            Green += c.Green;
                            Blue += c.Blue;
                            Alpha += c.Alpha;
                            count++;
                        }
                        else if (TransparentEdges) count++;
                    }
                }
                Red = Math.Clamp((float)Math.Round(Red * Scale / count), 0, 255);
                Green = Math.Clamp((float)Math.Round(Green * Scale / count), 0, 255);
                Blue = Math.Clamp((float)Math.Round(Blue * Scale / count), 0, 255);
                Alpha /= count;
                bmp.SetPixel(x, y, (byte)Red, (byte)Green, (byte)Blue, (byte)Alpha);
            }
        }
        bmp.Lock();
        return bmp;
    }

    /// <summary>
    /// Applies a box blur filter on the original image.
    /// </summary>
    /// <param name="Inside">Points within this region are not blurred, so as to save processing time. Useful especially when working with solid colors.</param>
    /// <param name="Weight">The weight or size of the blur. Runtime increases exponentially with the weight. Must be at least 1.</param>
    /// <param name="TransparentEdges">If true, non-existent pixels for filtering near edges are seen as transparent. If false, a the filter weight is reduced locally.</param>
    /// <returns>The new blurred bitmap.</returns>
    public Bitmap BlurExcludeRectangle(Rect Inside, int Weight = 1, bool TransparentEdges = true)
    {
        if (Weight < 1) throw new Exception("Blur weight must be at least 1.");
        Bitmap bmp = new Bitmap(Width, Height);
        bmp.Unlock();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Inside.Contains(x, y)) continue;
                int Red = 0,
                    Green = 0,
                    Blue = 0,
                    Alpha = 0;
                int count = 0;
                for (int dy = y - Weight; dy <= y + Weight; dy++)
                {
                    for (int dx = x - Weight; dx <= x + Weight; dx++)
                    {
                        if (dx >= 0 && dx < Width && dy >= 0 && dy < Height)
                        {
                            Color c = GetPixel(dx, dy);
                            Red += c.Red;
                            Green += c.Green;
                            Blue += c.Blue;
                            Alpha += c.Alpha;
                            count++;
                        }
                        else if (TransparentEdges) count++;
                    }
                }
                Red /= count;
                Green /= count;
                Blue /= count;
                Alpha /= count;
                bmp.SetPixel(x, y, (byte)Red, (byte)Green, (byte)Blue, (byte)Alpha);
            }
        }
        bmp.Lock();
        return bmp;
    }
}

