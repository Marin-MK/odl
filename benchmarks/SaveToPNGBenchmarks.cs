using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using BasicBenchmarks;
using System.Diagnostics;

namespace odl.benchmarks;

internal class SaveToPNGBenchmarks
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
        _bitmap.Lock();
    }

    [Benchmark]
    public void SaveWithDecodl()
    {
        _bitmap.SaveToPNG("benchmark1.png");
    }

    [Benchmark]
    public void SaveWithSDL()
    {
        SDL2.SDL_image.IMG_SavePNG(_bitmap.Surface, "benchmark2.png");
    }

    [AfterAll]
    public void Cleanup()
    {
        _bitmap.Dispose();
    }
}
