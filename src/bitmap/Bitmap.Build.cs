using System;
using static odl.SDL2.SDL;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DestRect">The available rectangle in the destination bitmap to draw the source bitmap in.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SX">The X position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SY">The Y position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SWidth">The width of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SHeight">The height of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(Rect DestRect, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(DestRect, SrcBitmap, new Rect(SX, SY, SWidth, SHeight), BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DestRect">The available rectangle in the destination bitmap to draw the source bitmap in.</param>
    /// <param name="SrcBitmap">The bitmap to be drawn.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(Rect DestRect, Bitmap SrcBitmap, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(DestRect, SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height), BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DP">The position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DS">The size available for the source bitmap to be drawn.</param>
    /// <param name="DestRect">The bitmap to be drawn on.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(Point DP, Size DS, Bitmap SrcBitmap, Rect SrcRect, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DP, DS), SrcBitmap, SrcRect, BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DP">The position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DS">The size available for the source bitmap to be drawn.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SX">The X position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SY">The Y position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SWidth">The width of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SHeight">The height of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(Point DP, Size DS, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DP, DS), SrcBitmap, new Rect(SX, SY, SWidth, SHeight), BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DP">The position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DWidth">The width available for the source bitmap to be drawn.</param>
    /// <param name="DHeight">The height available for the source bitmap to be drawn.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(Point DP, int DWidth, int DHeight, Bitmap SrcBitmap, Rect SrcRect, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DP, DWidth, DHeight), SrcBitmap, SrcRect, BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DP">The position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DWidth">The width available for the source bitmap to be drawn.</param>
    /// <param name="DHeight">The height available for the source bitmap to be drawn.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SX">The X position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SY">The Y position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SWidth">The width of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SHeight">The height of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(Point DP, int DWidth, int DHeight, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DP, DWidth, DHeight), SrcBitmap, new Rect(SX, SY, SWidth, SHeight), BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DX">The X position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DY">The Y position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DS">The size available for the source bitmap to be drawn.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(int DX, int DY, Size DS, Bitmap SrcBitmap, Rect SrcRect, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DX, DY, DS), SrcBitmap, SrcRect, BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DX">The X position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DY">The Y position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DS">The size available for the source bitmap to be drawn.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SX">The X position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SY">The Y position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SWidth">The width of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SHeight">The height of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(int DX, int DY, Size DS, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DX, DY, DS), SrcBitmap, new Rect(SX, SY, SWidth, SHeight), BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DX">The X position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DY">The Y position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DWidth">The width available for the source bitmap to be drawn.</param>
    /// <param name="DHeight">The height available for the source bitmap to be drawn.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SX">The X position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SY">The Y position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SWidth">The width of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SHeight">The height of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(int DX, int DY, int DWidth, int DHeight, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DX, DY, DWidth, DHeight), SrcBitmap, new Rect(SX, SY, SWidth, SHeight), BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DX">The X position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DY">The Y position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DWidth">The width available for the source bitmap to be drawn.</param>
    /// <param name="DHeight">The height available for the source bitmap to be drawn.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(int DX, int DY, int DWidth, int DHeight, Bitmap SrcBitmap, Rect SrcRect, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DX, DY, DWidth, DHeight), SrcBitmap, SrcRect, BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DP">The position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(Point DP, Bitmap SrcBitmap, Rect SrcRect, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DP, SrcRect.Width, SrcRect.Height), SrcBitmap, SrcRect, BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DP">The position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SX">The X position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SY">The Y position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SWidth">The width of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SHeight">The height of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(Point DP, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DP, SWidth, SHeight), SrcBitmap, new Rect(SX, SY, SWidth, SHeight), BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DX">The X position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DY">The Y position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(int DX, int DY, Bitmap SrcBitmap, Rect SrcRect, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DX, DY, SrcRect.Width, SrcRect.Height), SrcBitmap, SrcRect, BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DX">The X position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DY">The Y position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SX">The X position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SY">The Y position of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SWidth">The width of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="SHeight">The height of the rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(int DX, int DY, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DX, DY, SWidth, SHeight), SrcBitmap, new Rect(SX, SY, SWidth, SHeight), BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DP">The position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(Point DP, Bitmap SrcBitmap, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DP, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height), BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DX">The X position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="DY">The Y position in the destination bitmap to draw the source bitmap at.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(int DX, int DY, Bitmap SrcBitmap, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(DX, DY, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height), BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public void Build(Bitmap SrcBitmap, BlendMode BlendMode = BlendMode.Blend)
    {
        this.Build(new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height), BlendMode);
    }

    /// <summary>
    /// Blits the Source bitmap on top of the Destination bitmap.
    /// </summary>
    /// <param name="DestRect">The available rectangle in the destination bitmap to draw the source bitmap in.</param>
    /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
    /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
    /// <param name="BlendMode">The blend mode with which to blend the two bitmaps.</param>
    public unsafe virtual void Build(Rect DestRect, Bitmap SrcBitmap, Rect SrcRect, BlendMode BlendMode = BlendMode.Blend)
    {
        // Clip the destination rectangle
        if (DestRect.X < 0)
        {
            int amt = -DestRect.X;
            SrcRect.X += amt;
            SrcRect.Width -= amt;
            DestRect.X += amt;
            DestRect.Width -= amt;
            if (DestRect.Width <= 0 || SrcRect.Width <= 0) return;
        }
        if (DestRect.Y < 0)
        {
            int amt = -DestRect.Y;
            SrcRect.Y += amt;
            SrcRect.Height -= amt;
            DestRect.Y += amt;
            DestRect.Height -= amt;
            if (DestRect.Height <= 0 || SrcRect.Height <= 0) return;
        }
        if (DestRect.X + DestRect.Width > this.Width)
        {
            int amt = DestRect.X + DestRect.Width - this.Width;
            DestRect.Width -= amt;
            SrcRect.Width -= amt;
        }
        if (DestRect.Y + DestRect.Height > this.Height)
        {
            int amt = DestRect.Y + DestRect.Height - this.Height;
            DestRect.Height -= amt;
            SrcRect.Height -= amt;
        }
        if (DestRect.Width == 0 || DestRect.Height == 0)
            throw new Exception("Destination rectangle has a width or height of 0.");
        if (DestRect.X < 0 || DestRect.X + DestRect.Width > Width || DestRect.Y < 0 || DestRect.Y + DestRect.Height > Height)
            throw new Exception("Destination rectangle is out of bounds.");
        if (SrcRect.X < 0 || SrcRect.X + SrcRect.Width > SrcBitmap.Width || SrcRect.Y < 0 || SrcRect.Y + SrcRect.Height > SrcBitmap.Height)
            throw new Exception("Source rectangle is out of bounds.");
        if (Locked) throw new BitmapLockedException();
        if (IsChunky)
        {
            foreach (Bitmap bmp in this.InternalBitmaps)
            {
                Rect bmprect = new Rect(bmp.InternalX, bmp.InternalY, this.ChunkSize);
                if (DestRect.Overlaps(bmprect))
                {
                    int nx = Math.Max(DestRect.X, bmprect.X);
                    int ny = Math.Max(DestRect.Y, bmprect.Y);
                    int nw = Math.Min(DestRect.X + DestRect.Width, bmprect.X + bmprect.Width) - nx;
                    int nh = Math.Min(DestRect.Y + DestRect.Height, bmprect.Y + bmprect.Height) - ny;
                    int DX = nx - bmp.InternalX;
                    int DY = ny - bmp.InternalY;
                    int DW = nw;
                    int DH = nh;
                    int SX = SrcRect.X + nx - DestRect.X;
                    int SY = SrcRect.Y + ny - DestRect.Y;
                    int SW = DW;
                    int SH = DH;
                    if (bmp.Locked) bmp.Unlock();
                    bmp.Build(
                        DX, DY, DW, DH,
                        SrcBitmap,
                        SX, SY, SW, SH,
                        BlendMode
                    );
                }
            }
        }
        else
        {
            if (SrcBitmap.IsChunky)
            {
                foreach (Bitmap bmp in SrcBitmap.InternalBitmaps)
                {
                    Rect bmprect = new Rect(bmp.InternalX, bmp.InternalY, SrcBitmap.ChunkSize);
                    if (SrcRect.Overlaps(bmprect))
                    {
                        int nx = Math.Max(SrcRect.X, bmprect.X);
                        int ny = Math.Max(SrcRect.Y, bmprect.Y);
                        int nw = Math.Min(SrcRect.X + SrcRect.Width, bmprect.X + bmprect.Width) - nx;
                        int nh = Math.Min(SrcRect.Y + SrcRect.Height, bmprect.Y + bmprect.Height) - ny;
                        int DX = DestRect.X + (nx - SrcRect.X);
                        int DY = DestRect.Y + (ny - SrcRect.Y);
                        int DW = nw;
                        int DH = nh;
                        this.Build(
                            DX, DY, DW, DH,
                            bmp,
                            nx - bmp.InternalX, ny - bmp.InternalY, nw, nh, // XY - InternalXY
                            BlendMode
                        );
                    }
                }
            }
            else
            {
                if (BlendMode == BlendMode.None && SrcRect.Width == DestRect.Width && SrcRect.Height == DestRect.Height)
                {
                    // Provides a faster way to build two bitmaps if no blending is involved than SDL's blit functions
                    nint SourcePtr = (nint)(SrcBitmap.PixelPointer + SrcRect.Y * SrcBitmap.Width * 4 + SrcRect.X * 4);
                    nint DestPtr = (nint)(this.PixelPointer + DestRect.Y * this.Width * 4 + DestRect.X * 4);
                    int BufferSize = SrcRect.Width * 4;
                    Span<byte> SourceSpan = new Span<byte>((void*)SourcePtr, BufferSize);
                    Span<byte> DestSpan = new Span<byte>((void*)DestPtr, BufferSize);
                    for (int i = 0; i < SrcRect.Height; i++)
                    {
                        SourceSpan.CopyTo(DestSpan);
                        SourcePtr += SrcBitmap.Width * 4;
                        DestPtr += this.Width * 4;
                        SourceSpan = new Span<byte>((void*)SourcePtr, BufferSize);
                        DestSpan = new Span<byte>((void*)DestPtr, BufferSize);
                    }
                }
                else
                {
                    SDL_Rect Src = SrcRect.SDL_Rect;
                    SDL_Rect Dest = DestRect.SDL_Rect;
                    if (Dest.w != Src.w || Dest.h != Src.h) SDL_BlitScaled(SrcBitmap.Surface, ref Src, this.Surface, ref Dest);
                    else SDL_BlitSurface(SrcBitmap.Surface, ref Src, this.Surface, ref Dest);
                }
            }
        }
        if (this.Renderer != null) this.Renderer.Update();
    }
}

