using System;
using System.Runtime.InteropServices;
using BasicBenchmarks;

namespace odl.benchmarks;

internal class ResizeBitmapBenchmarks
{
    int X = 0;
    int Y = 0;
    int Width = 256;
    int Height = 256;
    int NewWidth = 512;
    int NewHeight = 512;
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
    public unsafe void ResizeReadByteWriteByte()
    {
        nint PixelHandle = Marshal.AllocHGlobal(NewWidth * NewHeight * 4);
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                if (x >= NewWidth || y >= NewHeight) continue;
                byte rbyte = _bitmap.PixelPointer[y * this.Width * 4 + x * 4];
                byte gbyte = _bitmap.PixelPointer[y * this.Width * 4 + x * 4 + 1];
                byte bbyte = _bitmap.PixelPointer[y * this.Width * 4 + x * 4 + 2];
                byte abyte = _bitmap.PixelPointer[y * this.Width * 4 + x * 4 + 3];
                int num = (abyte << 24) + (bbyte << 16) + (gbyte << 8) + rbyte;
                Marshal.WriteByte(PixelHandle + y * NewWidth * 4 + x * 4, rbyte);
                Marshal.WriteByte(PixelHandle + y * NewWidth * 4 + x * 4 + 1, gbyte);
                Marshal.WriteByte(PixelHandle + y * NewWidth * 4 + x * 4 + 2, bbyte);
                Marshal.WriteByte(PixelHandle + y * NewWidth * 4 + x * 4 + 3, abyte);
            }
        }
        Bitmap bmp = new Bitmap(PixelHandle, NewWidth, NewHeight);
        bmp.Dispose();
    }

    [Benchmark]
    public unsafe void ResizeReadByteWriteInt()
    {
        nint PixelHandle = Marshal.AllocHGlobal(NewWidth * NewHeight * 4);
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                if (x >= NewWidth || y >= NewHeight) continue;
                byte rbyte = _bitmap.PixelPointer[y * this.Width * 4 + x * 4];
                byte gbyte = _bitmap.PixelPointer[y * this.Width * 4 + x * 4 + 1];
                byte bbyte = _bitmap.PixelPointer[y * this.Width * 4 + x * 4 + 2];
                byte abyte = _bitmap.PixelPointer[y * this.Width * 4 + x * 4 + 3];
                int num = (abyte << 24) + (bbyte << 16) + (gbyte << 8) + rbyte;
                Marshal.WriteInt32(PixelHandle + y * NewWidth * 4 + x * 4, num);
            }
        }
        Bitmap bmp = new Bitmap(PixelHandle, NewWidth, NewHeight);
        bmp.Dispose();
    }

    [Benchmark]
    public unsafe void ResizeReadIntWriteByte()
    {
        nint PixelHandle = Marshal.AllocHGlobal(NewWidth * NewHeight * 4);
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                if (x >= NewWidth || y >= NewHeight) continue;
                int Pixel = Marshal.ReadInt32((nint) _bitmap.PixelPointer + y * this.Width * 4 + x * 4);
                byte rbyte = (byte) ((Pixel & 0xFF000000) >> 24);
                byte gbyte = (byte) ((Pixel & 0x00FF0000) >> 16);
                byte bbyte = (byte) ((Pixel & 0x0000FF00) >> 8);
                byte abyte = (byte) (Pixel & 0x000000FF);
                Marshal.WriteByte(PixelHandle + y * NewWidth * 4 + x * 4, rbyte);
                Marshal.WriteByte(PixelHandle + y * NewWidth * 4 + x * 4 + 1, gbyte);
                Marshal.WriteByte(PixelHandle + y * NewWidth * 4 + x * 4 + 2, bbyte);
                Marshal.WriteByte(PixelHandle + y * NewWidth * 4 + x * 4 + 3, abyte);
                Marshal.WriteInt32(PixelHandle + y * NewWidth * 4 + x * 4, Pixel);
            }
        }
        Bitmap bmp = new Bitmap(PixelHandle, NewWidth, NewHeight);
        bmp.Dispose();
    }

    [Benchmark]
    public unsafe void ResizeReadIntWriteInt()
    {
        nint PixelHandle = Marshal.AllocHGlobal(NewWidth * NewHeight * 4);
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                if (x >= NewWidth || y >= NewHeight) continue;
                int Pixel = Marshal.ReadInt32((nint) _bitmap.PixelPointer + y * this.Width * 4 + x * 4);
                Marshal.WriteInt32(PixelHandle + y * NewWidth * 4 + x * 4, Pixel);
            }
        }
        Bitmap bmp = new Bitmap(PixelHandle, NewWidth, NewHeight);
        bmp.Dispose();
    }

    [Benchmark]
    public unsafe void ResizeWithSpan()
    {
        nint PixelHandle = Marshal.AllocHGlobal(NewWidth * NewHeight * 4);
        int srcWidth = this.Width * 4;
        int destWidth = NewWidth * 4;
        if (destWidth < srcWidth) srcWidth = destWidth;
        if (srcWidth < destWidth) destWidth = srcWidth;
        int minHeight = NewHeight;
        if (this.Height < minHeight) minHeight = this.Height;
        Span<byte> srcSpan = new Span<byte>((void*) _bitmap.PixelPointer, srcWidth);
        Span<byte> destSpan = new Span<byte>((void*) PixelHandle, destWidth);
        for (int y = 0; y < minHeight; y++)
        {
            srcSpan.CopyTo(destSpan);
            srcSpan = new Span<byte>((void*) (_bitmap.PixelPointer + y * this.Width * 4), srcWidth);
            destSpan = new Span<byte>((void*) (PixelHandle + y * NewWidth * 4), destWidth);
        }
        Bitmap bmp = new Bitmap(PixelHandle, NewWidth, NewHeight);
        bmp.Dispose();
    }

    [Benchmark]
    public void ResizeBuild()
    {
        Bitmap bmp = _bitmap.Resize(NewWidth, NewHeight);
    }

    [AfterAll]
    public void Cleanup()
    {
        _bitmap.Dispose();
    }
}
