using System;
using System.Runtime.InteropServices;
using BasicBenchmarks;

namespace odl.benchmarks;

internal class GetSetPixelBenchmarks
{
    int X = 0;
    int Y = 0;
    Bitmap _bitmap;

    [BeforeAll]
    public void Initialize()
    {
        _bitmap = new Bitmap(32, 32);
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
    public unsafe void GetPixel()
    {
        Color color = _bitmap.GetPixel(X, Y);
    }

    [Benchmark]
    public unsafe void GetPixelWithoutSafety()
    {
        Color color = _bitmap.GetPixelFast(X, Y);
    }

    [Benchmark]
    public unsafe void GetPixelWithSpan()
    {
        Span<byte> span = new Span<byte>(_bitmap.PixelPointer, _bitmap.Width * _bitmap.Height * 4);
        int idx = _bitmap.Width * Y * 4 + X * 4;
        Color color = new Color(
            span[idx],
            span[idx + 1],
            span[idx + 2],
            span[idx + 3]
        );
    }

    [Benchmark]
    public unsafe void GetPixelWithSpanAndSlice()
    {
        Span<byte> span = new Span<byte>(_bitmap.PixelPointer, _bitmap.Width * _bitmap.Height * 4);
        int idx = _bitmap.Width * Y * 4 + X * 4;
        Span<byte> region = span.Slice(idx, 4);
        Color color = new Color(region[0], region[1], region[2], region[3]);
    }

    [Benchmark]
    public void SDL_GetRGBA()
    {
        byte r = 0, g = 0, b = 0, a = 0;
        uint pixel = (uint) Marshal.ReadInt32(_bitmap.SurfaceObject.pixels);
        SDL2.SDL.SDL_GetRGBA(pixel, _bitmap.SurfaceObject.format, ref r, ref g, ref b, ref a);
        Color color = new Color(r, g, b, a);
    }

    [Benchmark]
    public unsafe void IndividualMarshalReads()
    {
        nint idx = (nint) _bitmap.PixelPointer;
        byte r = Marshal.ReadByte(idx);
        byte g = Marshal.ReadByte(idx + 1);
        byte b = Marshal.ReadByte(idx + 2);
        byte a = Marshal.ReadByte(idx + 3);
        Color color = new Color(r, g, b, a);
    }

    [Benchmark]
    public unsafe void SetPixel()
    {
        _bitmap.SetPixel(X, Y, Color.GREEN);
    }

    [Benchmark]
    public unsafe void SetPixelWithoutSafety()
    {
        _bitmap.SetPixelFast(X, Y, Color.GREEN);
    }

    [Benchmark]
    public unsafe void SetPixelWithSpan()
    {
        Span<byte> span = new Span<byte>(_bitmap.PixelPointer + Y * _bitmap.Width * 4 + X * 4, 4);
        span[0] = 0;
        span[1] = 255;
        span[2] = 0;
        span[3] = 255;
    }

    [Benchmark]
    public unsafe void IndividualMarshalWrites()
    {
        nint idx = (nint) _bitmap.PixelPointer + Y * _bitmap.Width * 4 + X * 4;
        Marshal.WriteByte(idx, 0);
        Marshal.WriteByte(idx + 1, 255);
        Marshal.WriteByte(idx + 2, 255);
        Marshal.WriteByte(idx + 3, 255);
    }

    [AfterAll]
    public void Cleanup()
    {
        _bitmap.Dispose();
    }
}
