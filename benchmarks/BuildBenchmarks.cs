using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using BasicBenchmarks;
using System.Diagnostics;

namespace odl.benchmarks;

internal class BuildBenchmarks
{
    int X = 0;
    int Y = 0;
    int Width = 256;
    int Height = 256;
    Bitmap _final;
    Bitmap _mask;

    [BeforeAll]
    public void Initialize()
    {
        _final = new Bitmap(Width, Height);
        _final.Unlock();
        _final.FillGradientRect(
            new Rect(0, 0, _final.Width, _final.Height),
            Color.RED,
            Color.GREEN,
            Color.BLUE,
            Color.WHITE
        );
        _mask = new Bitmap(Width, Height);
        _mask.Unlock();
        _mask.FillRect(0, 0, Width, Height, new Color(255, 255, 0));
        _mask.Lock();
    }

    [Benchmark]
    public void BuildBlend()
    {
        _final.Build(_mask, BlendMode.Blend);
    }

    [Benchmark]
    public void BuildNone()
    {
        _final.Build(_mask, BlendMode.None);
    }

    [AfterAll]
    public void Cleanup()
    {
        _final.Dispose();
    }
}
