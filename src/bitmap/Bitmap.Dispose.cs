using System;
using System.Runtime.InteropServices;
using static odl.SDL2.SDL;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Disposes and destroys the bitmap.
    /// </summary>
    public virtual void Dispose()
    {
        if (Disposed) return;
        BitmapList.Remove(this);
        if (IsChunky)
        {
            foreach (Bitmap b in this.InternalBitmaps) b.Dispose();
            this.InternalBitmaps.Clear();
        }
        else
        {
            if (this.Surface != IntPtr.Zero) SDL_FreeSurface(this.Surface);
            if (this.Texture != IntPtr.Zero) SDL_DestroyTexture(this.Texture);
            if (PixelHandle != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(PixelHandle);
                PixelHandle = IntPtr.Zero;
            }
            this.Surface = IntPtr.Zero;
            this.Texture = IntPtr.Zero;
            this.SurfaceObject = new SDL_Surface();
        }
        this.Disposed = true;
        if (this.Renderer != null) this.Renderer.Update();
    }
}

