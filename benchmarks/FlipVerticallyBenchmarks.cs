using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BasicBenchmarks;

namespace odl.benchmarks;

internal class FlipVerticallyBenchmarks
{
    int X = 0;
    int Y = 0;
    int Width = 256;
    int Height = 256;
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
    public void QueueStackPixelOps()
    {
        Stack<Queue<Color>> Colors = new Stack<Queue<Color>>();
        for (int dy = Y; dy < Y + Height; dy++)
        {
            Queue<Color> Pixels = new Queue<Color>();
            for (int dx = X; dx < X + Width; dx++)
            {
                Pixels.Enqueue(_bitmap.GetPixelFast(dx, dy));
            }
            Colors.Push(Pixels);
        }
        for (int dy = Y; dy < Y + Height; dy++)
        {
            Queue<Color> Pixels = Colors.Pop();
            for (int dx = X; dx < X + Width; dx++)
            {
                Color c = Pixels.Dequeue();
                _bitmap.SetPixelFast(dx, dy, c.Red, c.Green, c.Blue, c.Alpha);
            }
        }
    }

    [Benchmark]
    public unsafe void WithSpans()
    {
        int BufferSize = Width * 4;
        nint BufferPtr = Marshal.AllocHGlobal(BufferSize);
        nint Top = (nint) (_bitmap.PixelPointer + Y * _bitmap.Width * 4 + X * 4);
        nint Bottom = (nint) (_bitmap.PixelPointer + (Y + Height - 1) * _bitmap.Width * 4 + X * 4);
        Span<byte> TopSpan = new Span<byte>((void*) Top, BufferSize);
        Span<byte> BottomSpan = new Span<byte>((void*) Bottom, BufferSize);
        Span<byte> Buffer = new Span<byte>((void*) BufferPtr, BufferSize);
        for (int i = 0; i < Height / 2; i++)
        {
            TopSpan.CopyTo(Buffer);
            BottomSpan.CopyTo(TopSpan);
            Buffer.CopyTo(BottomSpan);
            Top += _bitmap.Width * 4;
            Bottom -= _bitmap.Width * 4;
            TopSpan = new Span<byte>((void*) Top, BufferSize);
            BottomSpan = new Span<byte>((void*) Bottom, BufferSize);
        }
        Marshal.FreeHGlobal(BufferPtr);
    }

    [AfterAll]
    public void Cleanup()
    {
        _bitmap.Dispose();
    }
}
