using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static odl.SDL2.SDL;

namespace odl;

public class Renderer : IDisposable
{
    private static int CreationCounter = 0;

    internal static int GetCreationCount()
    {
        return CreationCounter++;
    }

    /// <summary>
    /// The window this renderer renders for.
    /// </summary>
    public Window Window;
    /// <summary>
    /// The list of viewports associated with the renderer.
    /// </summary>
    public List<Viewport> Viewports = new List<Viewport>();
    /// <summary>
    /// The pointer to the SDL_Renderer object.
    /// </summary>
    public IntPtr SDL_Renderer;
    /// <summary>
    /// Whether the renderer is disposed.
    /// </summary>
    public bool Disposed = false;
    /// <summary>
    /// An X offset for rendering viewports.
    /// </summary>
    public int RenderOffsetX = 0;
    /// <summary>
    /// A Y offset for rendering viewports.
    /// </summary>
    public int RenderOffsetY = 0;
    /// <summary>
    /// X Scale multiplier for rendering viewports.
    /// </summary>
    public float RenderScaleX = 1f;
    /// <summary>
    /// Y Scale multiplier for rendering viewports.
    /// </summary>
    public float RenderScaleY = 1f;
    /// <summary>
    /// Additional renderer opacity factor that applies to all sprites and viewports.
    /// </summary>
    public byte Opacity = 255;
    /// <summary>
    /// Whether the viewport list needs to be reordered.
    /// </summary>
    public bool ReorderViewports { get; set; } = true;

    /// <summary>
    /// Whether the renderer needs to re-render.
    /// </summary>
    bool NeedUpdate = false;

    public Renderer(Window Window, IntPtr SDL_Renderer)
    {
        this.Window = Window;
        this.SDL_Renderer = SDL_Renderer;
        Graphics.RegisterRenderer(this);
    }

    /// <summary>
    /// Forces a re-render in the next Graphics update.
    /// </summary>
    public void Update()
    {
        NeedUpdate = true;
    }

    internal unsafe Bitmap RenderViewports(List<Viewport> Viewports, int Width = 0, int Height = 0, int XOffset = 0, int YOffset = 0)
    {
        if (Width == 0) Width = Window.Width;
        if (Height == 0) Height = Window.Height;
        int OldOffsetX = RenderOffsetX;
        int OldOffsetY = RenderOffsetY;
        RenderOffsetX = XOffset;
        RenderOffsetY = YOffset;
        IntPtr Target = SDL_CreateTexture(SDL_Renderer, SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888, 2, Width, Height);
        RenderStart(Target);
        RenderViewports(Viewports, true, true);
        SDL_Rect rect = new SDL_Rect();
        rect.x = 0;
        rect.y = 0;
        rect.w = Width;
        rect.h = Height;
        IntPtr pixels = Marshal.AllocHGlobal(Width * Height * 4);
        Bitmap bmp = new Bitmap(Width, Height);
        bmp.Unlock();
        SDL_RenderReadPixels(SDL_Renderer, rect, SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888, (IntPtr) bmp.PixelPointer, Width * 4);
        RenderOffsetX = OldOffsetX;
        RenderOffsetY = OldOffsetY;
        bmp.Lock();
        SDL_DestroyTexture(Target);
        return bmp;
    }

    /// <summary>
    /// Re-renders the entire screen.
    /// </summary>
    public void Redraw(bool Force = false)
    {
        if (NeedUpdate || Force)
        {
            RenderStart();
            RenderViewports(this.Viewports, false, true);
            RenderPresent();
            NeedUpdate = false;
        }
    }

    private void RenderStart(IntPtr? TargetTexture = null)
    {
        SDL_SetRenderTarget(this.SDL_Renderer, TargetTexture ?? IntPtr.Zero);
        SDL_RenderClear(this.SDL_Renderer);
    }

    private void RenderPresent()
    {
        SDL_RenderPresent(this.SDL_Renderer);
    }

    private void RenderViewports(List<Viewport> Viewports, bool ForceReorder, bool ForceRescale)
    {
        if (ReorderViewports || ForceReorder)
        {
            // Only toggle the global reordering if we weren't manually forced to reorder
            if (ReorderViewports && !ForceReorder) ReorderViewports = false;
            Viewports.Sort(delegate (Viewport vp1, Viewport vp2)
            {
                if (vp1.Z != vp2.Z) return vp1.Z.CompareTo(vp2.Z);
                return vp1.CreationTime.CompareTo(vp2.CreationTime);
            });
        }
        for (int i = 0; i < Viewports.Count; i++)
        {
            if (!RenderViewport(Viewports[i], ForceRescale))
            {
                Viewports.RemoveAt(i);
                i--;
            }
        }
    }

    private bool RenderViewport(Viewport vp, bool ForceRescale)
    {
        if (vp.Disposed)
        {
            return false;
        }
        else if (vp.Visible && vp.Width > 0 && vp.Height > 0 && vp.Sprites.Count > 0)
        {
            SDL_Rect ViewportRect = new SDL_Rect();
            SDL_RenderGetViewport(this.SDL_Renderer, out ViewportRect);
            ViewportRect.x = vp.X - vp.OX + RenderOffsetX;
            ViewportRect.y = vp.Y - vp.OY + RenderOffsetY;
            if (vp.Width == -1) vp.Width = ViewportRect.w;
            if (vp.Height == -1) vp.Height = ViewportRect.h;
            ViewportRect.w = (int) Math.Round(vp.Width * vp.ZoomX);
            ViewportRect.h = (int) Math.Round(vp.Height * vp.ZoomY);
            int xoffset = 0;
            int yoffset = 0;
            if (ForceRescale)
            {

            }
            if (ViewportRect.x < 0)
            {
                xoffset = ViewportRect.x;
                ViewportRect.x = 0;
                ViewportRect.w -= -xoffset;
            }
            if (ViewportRect.y < 0)
            {
                yoffset = ViewportRect.y;
                ViewportRect.y = 0;
                ViewportRect.h -= -yoffset;
            }
            if (RenderScaleX != 1f || RenderScaleY != 1f)
            {
                SDL_RenderSetLogicalSize(SDL_Renderer, vp.Width, vp.Height);
                SDL_RenderSetScale(SDL_Renderer, RenderScaleX, RenderScaleY);
            }
            SDL_RenderSetViewport(SDL_Renderer, ref ViewportRect);
            RenderSprites(vp, vp.Sprites, xoffset, yoffset);
        }
        return true;
    }

    private void RenderSprites(Viewport vp, List<Sprite> Sprites, int xoffset, int yoffset)
    {
        if (vp.ReorderSprites)
        {
            vp.ReorderSprites = false;
            vp.Sprites.Sort(delegate (Sprite s1, Sprite s2)
            {
                if (s1.Z != s2.Z) return s1.Z.CompareTo(s2.Z);
                return s1.CreationTime.CompareTo(s2.CreationTime);
            });
        }
        for (int j = 0; j < vp.Sprites.Count; j++)
        {
            Sprite s = vp.Sprites[j];
            if (s.Disposed)
            {
                vp.Sprites.RemoveAt(j);
            }
            else if (s.Visible && s.Bitmap != null && !s.Bitmap.Disposed && s.Opacity > 0 && s.ZoomX != 0 && s.ZoomY != 0)
            {
                if (s.Bitmap.IsChunky)
                {
                    int SX = s.X;
                    int SY = s.Y;
                    foreach (Bitmap bmp in s.Bitmap.InternalBitmaps)
                    {
                        s.X = SX + (int) Math.Round(bmp.InternalX * s.ZoomX);
                        s.Y = SY + (int) Math.Round(bmp.InternalY * s.ZoomY);
                        s.SrcRect = new Rect(0, 0, bmp.Width, bmp.Height);
                        RenderSprite(vp, s, bmp, xoffset, yoffset);
                    }
                    s.X = SX;
                    s.Y = SY;
                }
                else
                {
                    RenderSprite(vp, s, s.Bitmap, xoffset, yoffset);
                }
            }
        }
    }

    /// <summary>
    /// Renders an individual sprite.
    /// </summary>
    /// <param name="s">The sprite to render.</param>
    private void RenderSprite(Viewport vp, Sprite s, Bitmap bmp, int XOffset, int YOffset)
    {
        IntPtr Texture = bmp.Texture;
        
        if (Texture == IntPtr.Zero) throw new Exception("Attempted to render a zero-pointer texture.");

        // Sprite Opacity + Viewport Opacity + Renderer Opacity
        byte Alpha = Convert.ToByte(255d * (s.Opacity / 255d) * (s.Viewport.Opacity / 255d) * (this.Opacity / 255d));

        SDL_SetTextureColorMod(Texture, s.Color.Red, s.Color.Green, s.Color.Blue);
        SDL_SetTextureAlphaMod(Texture, Alpha);

        List<Point> Points;
        if (s.MultiplePositions.Count == 0) // Normal X,Y positions
        {
            Points = new List<Point>() { new Point(s.X, s.Y) };
        }
        else // Multiple positions; used to prevent the need for hundreds of identical sprites that only differ in position (e.g. in a background grid)
        {
            Points = s.MultiplePositions;
        }

        SDL_Rect Src = new SDL_Rect();
        SDL_Rect Dest = new SDL_Rect();
        if (bmp is SolidBitmap)
        {
            Src.x = 0;
            Src.y = 0;
            Src.w = 1;
            Src.h = 1;
            Dest.w = (int) Math.Round(((SolidBitmap) bmp).BitmapWidth * s.ZoomX * vp.ZoomX);
            Dest.h = (int) Math.Round(((SolidBitmap) bmp).BitmapHeight * s.ZoomY * vp.ZoomY);
        }
        else
        {
            Src = s.SrcRect.SDL_Rect;
            Dest.w = (int) Math.Round(Src.w * s.ZoomX * vp.ZoomX);
            Dest.h = (int) Math.Round(Src.h * s.ZoomY * vp.ZoomY);
        }

        if (!bmp.Locked && !(bmp is SolidBitmap))
        {
            throw new BitmapLockedException();
        }
        foreach (Point p in Points)
        {
            int oxoffset = s.FactorZoomIntoOrigin ? (int) Math.Round(s.OX * s.ZoomX) : s.OX;
            int oyoffset = s.FactorZoomIntoOrigin ? (int) Math.Round(s.OY * s.ZoomY) : s.OY;
            int vpoxdiff = (int) Math.Round((1 - vp.ZoomX) * (p.X + oxoffset));
            int vpoydiff = (int) Math.Round((1 - vp.ZoomY) * (p.Y + oyoffset));
            Dest.x = p.X - oxoffset + XOffset - vpoxdiff;
            Dest.y = p.Y - oyoffset + YOffset - vpoydiff;

            if (s.Angle % 360 == 0 && s.OX == 0 && s.OY == 0 && !s.MirrorX && !s.MirrorY)
            {
                SDL_RenderCopy(this.SDL_Renderer, Texture, ref Src, ref Dest);
            }
            else
            {
                SDL_Point Center = new SDL_Point();
                Center.x = s.OX;
                Center.y = s.OY;

                SDL_RendererFlip MirrorState = SDL_RendererFlip.SDL_FLIP_NONE;
                if (s.MirrorX) MirrorState |= SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
                if (s.MirrorY) MirrorState |= SDL_RendererFlip.SDL_FLIP_VERTICAL;

                SDL_RenderCopyEx(this.SDL_Renderer, Texture, ref Src, ref Dest, s.Angle % 360, ref Center, MirrorState);
            }
            // Don't destroy the texture as it can be reused next render
        }
    }

    /// <summary>
    /// Disposes the renderer and the associated viewports.
    /// </summary>
    public void Dispose()
    {
        while (Viewports.Count > 0) Viewports[0].Dispose();
        SDL_DestroyRenderer(this.SDL_Renderer);
        this.Disposed = true;
    }
}
