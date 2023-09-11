using System;
using System.Runtime.InteropServices;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Changes the size of the bitmap without scaling or interpolation.
    /// </summary>
    /// <param name="NewSize">The new size of the bitmap.</param>
    /// <returns>The resized bitmap.</returns>
    public Bitmap Resize(Size NewSize, Size? ChunkSize = null)
    {
        return Resize(NewSize.Width, NewSize.Height, ChunkSize ?? Graphics.MaxTextureSize);
    }

    /// <summary>
    /// Changes the size of the bitmap without scaling or interpolation.
    /// </summary>
    /// <param name="NewWidth">The new width of the bitmap.</param>
    /// <param name="NewHeight">The new height of the bitmap.</param>
    /// <returns>The resized bitmap.</returns>
    public Bitmap Resize(int NewWidth, int NewHeight, Size? ChunkSize = null)
    {
        Bitmap NewBitmap = null;
        if (ChunkSize == null) NewBitmap = new Bitmap(NewWidth, NewHeight, this.ChunkSize ?? Graphics.MaxTextureSize);
        else NewBitmap = new Bitmap(NewWidth, NewHeight, ChunkSize);
        NewBitmap.Unlock();
        NewBitmap.Build(this, BlendMode.None);
        NewBitmap.Lock();
        return NewBitmap;
    }

    /// <summary>
    /// Changes the size of the bitmap without scaling or interpolation.
    /// </summary>
    /// <param name="NewWidth">The new width of the bitmap.</param>
    /// <param name="NewHeight">The new height of the bitmap.</param>
    /// <returns>The resized bitmap.</returns>
    public unsafe Bitmap ResizeWithoutBuild(int NewWidth, int NewHeight)
    {
        if (IsChunky) throw new NotImplementedException();
        nint PixelHandle = Marshal.AllocHGlobal(NewWidth * NewHeight * 4);
        int copyWidth = Math.Min(NewWidth, this.Width) * 4;
        int minHeight = NewHeight;
        if (this.Height < minHeight) minHeight = this.Height;
        Span<byte> srcSpan = new Span<byte>((void*)this.PixelPointer, copyWidth);
        Span<byte> destSpan = new Span<byte>((void*)PixelHandle, copyWidth);
        for (int y = 0; y < minHeight; y++)
        {
            srcSpan.CopyTo(destSpan);
            srcSpan = new Span<byte>((void*)(this.PixelPointer + y * this.Width * 4), copyWidth);
            destSpan = new Span<byte>((void*)(PixelHandle + y * NewWidth * 4), copyWidth);
        }
        Bitmap bmp = new Bitmap(PixelHandle, NewWidth, NewHeight);
        return bmp;
    }
}

