using System;
using static odl.SDL2.SDL;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Locks the bitmap and converts the surface to a texture. The bitmap can no longer be modified until unlocked.
    /// </summary>
    public virtual void Lock()
    {
        if (Locked) throw new BitmapLockedException();
        this.Locked = true;
        if (IsChunky)
        {
            foreach (Bitmap b in this.InternalBitmaps)
            {
                if (!b.Locked && b.Renderer != null)
                {
                    b.Lock();
                }
            }
        }
        else
        {
            this.RecreateTexture();
        }
    }

    /// <summary>
    /// Unlocks the bitmap, allowing you to modify the bitmap until locked again.
    /// </summary>
    public virtual void Unlock()
    {
        if (!Locked) throw new Exception("Bitmap was already unlocked and cannot be unlocked again.");
        this.Locked = false;
    }

    /// <summary>
    /// Unlocks the bitmap and immediately locks it.
    /// </summary>
    public virtual void Relock()
    {
        this.Unlock();
        this.Lock();
    }

    /// <summary>
    /// Converts the SDL_Surface to an SDL_Texture used when rendering.
    /// </summary>
    public virtual void RecreateTexture(bool Full = true)
    {
        if (this.Renderer == null) return;
        if (this.Texture != IntPtr.Zero) SDL_DestroyTexture(this.Texture);
        this.Texture = SDL_CreateTextureFromSurface(this.Renderer.SDL_Renderer, this.Surface);
        if (this.Texture == IntPtr.Zero)
        {
            throw new Exception("Invalid texture");
        }
        SDL_SetTextureBlendMode(this.Texture, (SDL_BlendMode)this.BlendMode);
        this.Renderer.Update();
    }
}

