using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace odl;

public partial class Bitmap
{
    public void FlipVertically()
    {
        FlipVertically(0, 0, Width, Height);
    }

    public unsafe void FlipVertically(int X, int Y, int Width, int Height)
    {
        if (X < 0 || Y < 0 || X + Width > this.Width || Y + Height > this.Height || this.Width < 0 || this.Height < 0)
            throw new Exception("Region out of bounds");
        if (Locked) throw new BitmapLockedException();
        int BufferSize = Width * 4;
        nint BufferPtr = Marshal.AllocHGlobal(BufferSize);
        nint Top = (nint)(PixelPointer + Y * this.Width * 4 + X * 4);
        nint Bottom = (nint)(PixelPointer + (Y + Height - 1) * this.Width * 4 + X * 4);
        Span<byte> TopSpan = new Span<byte>((void*)Top, BufferSize);
        Span<byte> BottomSpan = new Span<byte>((void*)Bottom, BufferSize);
        Span<byte> Buffer = new Span<byte>((void*)BufferPtr, BufferSize);
        for (int i = 0; i < Height / 2; i++)
        {
            TopSpan.CopyTo(Buffer);
            BottomSpan.CopyTo(TopSpan);
            Buffer.CopyTo(BottomSpan);
            Top += this.Width * 4;
            Bottom -= this.Width * 4;
            TopSpan = new Span<byte>((void*)Top, BufferSize);
            BottomSpan = new Span<byte>((void*)Bottom, BufferSize);
        }
        Marshal.FreeHGlobal(BufferPtr);
    }

    public void FlipHorizontally()
    {
        FlipHorizontally(0, 0, Width, Height);
    }

    public void FlipHorizontally(int X, int Y, int Width, int Height)
    {
        if (X < 0 || Y < 0 || X + Width > this.Width || Y + Height > this.Height || this.Width < 0 || this.Height < 0)
            throw new Exception("Region out of bounds");
        if (Locked) throw new BitmapLockedException();
        Stack<Queue<Color>> Colors = new Stack<Queue<Color>>();
        for (int dx = X; dx < X + Width; dx++)
        {
            Queue<Color> Pixels = new Queue<Color>();
            for (int dy = Y; dy < Y + Height; dy++)
            {
                Pixels.Enqueue(GetPixelFast(dx, dy));
            }
            Colors.Push(Pixels);
        }
        for (int dx = X; dx < X + Width; dx++)
        {
            Queue<Color> Pixels = Colors.Pop();
            for (int dy = Y; dy < Y + Height; dy++)
            {
                Color c = Pixels.Dequeue();
                SetPixelFast(dx, dy, c.Red, c.Green, c.Blue, c.Alpha);
            }
        }
    }
}

