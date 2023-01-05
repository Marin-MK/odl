using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using BasicBenchmarks;
using System.Diagnostics;

namespace odl.benchmarks;

internal class FillBitmapBenchmarks
{
    int X = 0;
    int Y = 0;
    int Width = 512;
    int Height = 512;
    Bitmap _bitmap;

    [BeforeAll]
    public void Initialize()
    {
        _bitmap = new Bitmap(Width, Height);
        _bitmap.Unlock();
        _bitmap.FillGradientRect(
            new Rect(0, 0, _bitmap.Width, _bitmap.Height),
            Color.RED,
            Color.GREEN,
            Color.BLUE,
            Color.WHITE
        );
    }

    [Benchmark]
    public void FillRect()
    {
        _bitmap.FillRect(X, Y, Width, Height, Color.GREEN);
    }

    [Benchmark]
    public void FillWithSetPixel()
    {
        for (int dy = Y; dy < Y + Height; dy++)
        {
            for (int dx = X; dx < X + Width; dx++)
            {
                _bitmap.SetPixel(dx, dy, Color.GREEN);
            }
        }
    }

    [Benchmark]
    public void FillWithSetPixelFast()
    {
        for (int dy = Y; dy < Y + Height; dy++)
        {
            for (int dx = X; dx < X + Width; dx++)
            {
                _bitmap.SetPixelFast(dx, dy, Color.GREEN);
            }
        }
    }

    [AfterAll]
    public void Cleanup()
    {
        _bitmap.Dispose();
    }
}
