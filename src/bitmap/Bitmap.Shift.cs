using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Shifts a region in the bitmap up or down depending on the configuration. Shifts across the entire width of the bitmap.
    /// </summary>
    /// <param name="StartY">The Y position to start shifting at.</param>
    /// <param name="RowCount">The number of rows to include in the shift.</param>
    /// <param name="YShift">The difference with the <paramref name="StartY"/> variable for the new position of the region.</param>
    /// <param name="ClearOrigin">Whether to clear the original region before copying it.</param>
    public unsafe void ShiftVertically(int StartY, int RowCount, int YShift, bool ClearOrigin)
    {
        if (Locked) throw new BitmapLockedException();
        if (StartY < 0) throw new Exception($"Cannot shift image with a region with a y coord less than 0, but got {StartY}.");
        if (StartY + YShift < 0) throw new Exception($"Cannot shift image y coord to less than 0, but got {StartY + YShift}.");
        if (StartY + RowCount > Height) throw new Exception($"Image shift cannot exceed bitmap height of {Height}, but got {StartY + RowCount}.");
        if (StartY + RowCount + YShift > Height) throw new Exception($"Cannot shift image y coord to more than bitmap height of {Height}, but got {StartY + RowCount + YShift}.");
        if (IsChunky)
        {
            if (InternalBitmaps.Any(b => b.InternalX > 0)) throw new Exception("This algorithm does not work for chunky bitmaps that have different bitmaps on the X axis");
            nint _itempptr = Marshal.AllocHGlobal(RowCount * Width * 4);
            int offset = 0;
            int RowsRemaining = RowCount;
            Rect Area = new Rect(0, StartY, Width, RowCount);
            Rect ShiftedArea = new Rect(0, StartY + YShift, Width, RowCount);
            foreach (Bitmap ibmp in InternalBitmaps)
            {
                if (RowsRemaining <= 0) break;
                if (Area.Overlaps(new Rect(ibmp.InternalX, ibmp.InternalY, ibmp.Width, ibmp.Height)))
                {
                    int _bmpy = ibmp.InternalY > StartY ? 0 : StartY - ibmp.InternalY;
                    int _bmpstart = _bmpy * ibmp.Width * 4;
                    int _bmprowcount = ibmp.Height - _bmpy;
                    if (_bmprowcount > RowsRemaining) _bmprowcount = RowsRemaining;
                    int _bmplength = _bmprowcount * Width * 4;
                    RowsRemaining -= _bmprowcount;
                    Buffer.MemoryCopy((void*)(ibmp.PixelPointer + _bmpstart), (void*)(_itempptr + offset), _bmplength, _bmplength);
                    if (ibmp.Locked) ibmp.Unlock();
                    if (ClearOrigin)
                    {
                        Span<byte> span = new Span<byte>((void*)(ibmp.PixelPointer + _bmpstart), _bmplength);
                        span.Fill(0);
                    }
                    offset += _bmplength;
                }
            }
            // _itempptr now contains all the copied pixels and the source region has been cleared (if desired)
            // Now we write them back to the internal bitmaps with an offset (the y shift).
            RowsRemaining = RowCount;
            offset = 0;
            foreach (Bitmap ibmp in InternalBitmaps)
            {
                if (RowsRemaining <= 0) break;
                if (ShiftedArea.Overlaps(new Rect(ibmp.InternalX, ibmp.InternalY, ibmp.Width, ibmp.Height)))
                {
                    int _bmpy = ibmp.InternalY > StartY + YShift ? 0 : StartY + YShift - ibmp.InternalY;
                    int _bmpstart = _bmpy * ibmp.Width * 4;
                    int _bmprowcount = ibmp.Height - _bmpy;
                    if (_bmprowcount > RowsRemaining) _bmprowcount = RowsRemaining;
                    int _bmplength = _bmprowcount * Width * 4;
                    RowsRemaining -= _bmprowcount;
                    Buffer.MemoryCopy((void*)(_itempptr + offset), (void*)(ibmp.PixelPointer + _bmpstart), _bmplength, _bmplength);
                    if (ibmp.Locked) ibmp.Unlock();
                    offset += _bmplength;
                }
            }
            return;
        }
        int StartPos = StartY * Width * 4;
        int Length = RowCount * Width * 4;
        int Shift = YShift * Width * 4;
        nint _tempptr = Marshal.AllocHGlobal(Length);
        void* temp = (void*)_tempptr;
        Buffer.MemoryCopy((void*)(PixelPointer + StartPos), temp, Length, Length);
        if (ClearOrigin)
        {
            // Use a span to fill a certain region of the array with 0 to avoid allocating memory
            Span<byte> span = new Span<byte>((void*)(PixelPointer + StartPos), Length);
            span.Fill(0);
        }
        Buffer.MemoryCopy(temp, (void*)(PixelPointer + StartPos + Shift), Length, Length);
        Marshal.FreeHGlobal(_tempptr);
    }
}

