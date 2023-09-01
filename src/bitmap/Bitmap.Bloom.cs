using System;

namespace odl;

public partial class Bitmap
{
    public Bitmap Bloom(float Scaling, float Threshold, int Weight)
    {
        Bitmap HighThreshold = WithThreshold(Threshold);
        Bitmap Filter = HighThreshold.Blur(Weight, Scaling);
        Bitmap Result = new Bitmap(Width, Height);
        Result.Unlock();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Color source = GetPixel(x, y);
                Color filter = Filter.GetPixel(x, y);
                Color ret = new Color(
                    (byte)Math.Clamp(Math.Round((double)(source.Red + filter.Red)), 0, 255),
                    (byte)Math.Clamp(Math.Round((double)(source.Green + filter.Green)), 0, 255),
                    (byte)Math.Clamp(Math.Round((double)(source.Blue + filter.Blue)), 0, 255),
                    (byte)Math.Clamp(Math.Round((double)(source.Alpha + filter.Alpha)), 0, 255)
                );
                Result.SetPixel(x, y, ret);
            }
        }
        Result.Lock();
        return Result;
    }

    public Bitmap WithThreshold(float Threshold)
    {
        Bitmap bmp = new Bitmap(Width, Height);
        bmp.Unlock();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Color c = GetPixel(x, y);
                if (c.Red / 255d >= Threshold || c.Green / 255d >= Threshold || c.Blue / 255d >= Threshold)
                    bmp.SetPixel(x, y, c);
            }
        }
        bmp.Lock();
        return bmp;
    }
}

