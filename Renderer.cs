using System;
using System.Collections.Generic;
using static odl.SDL2.SDL;

namespace odl;

internal class Renderer : IDisposable
{
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

    public Renderer(IntPtr SDL_Renderer)
    {
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

    /// <summary>
    /// Re-renders the entire screen.
    /// </summary>
    public void Redraw(bool Force = false)
    {
        if (NeedUpdate || Force)
        {
            Graphics.Log("=====================");
            Graphics.Log("START render cycle");
            SDL_RenderClear(this.SDL_Renderer);
            Graphics.Log("Viewport count: " + this.Viewports.Count.ToString());
            if (ReorderViewports)
            {
                ReorderViewports = false;
                this.Viewports.Sort(delegate (Viewport vp1, Viewport vp2)
                {
                    if (vp1.Z != vp2.Z) return vp1.Z.CompareTo(vp2.Z);
                    return vp1.TimeCreated.CompareTo(vp2.TimeCreated);
                });
            }
            for (int i = 0; i < Viewports.Count; i++)
            {
                Viewport vp = Viewports[i];
                if (vp.Disposed)
                {
                    this.Viewports.RemoveAt(i);
                    Graphics.Log("Viewport " + i.ToString() + " removed as it was disposed");
                }
                else if (vp.Visible && vp.Width > 0 && vp.Height > 0 && vp.Sprites.Count > 0)
                {
                    Graphics.Log("Viewport " + i.ToString() + $" z={vp.Z} ({vp.Rect}), zoomx: {vp.ZoomX} zoomy: {vp.ZoomY} sprites: {vp.Sprites.Count}");
                    SDL_Rect ViewportRect = new SDL_Rect();
                    SDL_RenderGetViewport(this.SDL_Renderer, out ViewportRect);
                    ViewportRect.x = vp.X - vp.OX + RenderOffsetX;
                    ViewportRect.y = vp.Y - vp.OY + RenderOffsetY;
                    if (vp.Width == -1) vp.Width = ViewportRect.w;
                    if (vp.Height == -1) vp.Height = ViewportRect.h;
                    ViewportRect.w = vp.Width;
                    ViewportRect.h = vp.Height;
                    int xoffset = 0;
                    int yoffset = 0;
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
                    if (vp.ReorderSprites)
                    {
                        vp.ReorderSprites = false;
                        vp.Sprites.Sort(delegate (Sprite s1, Sprite s2)
                        {
                            if (s1.Z != s2.Z) return s1.Z.CompareTo(s2.Z);
                            return s1.TimeCreated.CompareTo(s2.TimeCreated);
                        });
                    }
                    for (int j = 0; j < vp.Sprites.Count; j++)
                    {
                        Sprite s = vp.Sprites[j];
                        if (s.Disposed)
                        {
                            vp.Sprites.RemoveAt(j);
                            Graphics.Log("Sprite " + j.ToString() + " removed as it was disposed");
                        }
                        else if (s.Visible && s.Bitmap != null && !s.Bitmap.Disposed && s.Opacity > 0 && s.ZoomX != 0 && s.ZoomY != 0)
                        {
                            if (s.Bitmap.IsChunky)
                            {
                                int SX = s.X;
                                int SY = s.Y;
                                foreach (Bitmap bmp in s.Bitmap.InternalBitmaps)
                                {
                                    s.X = SX + (int)Math.Round(bmp.InternalX * s.ZoomX);
                                    s.Y = SY + (int)Math.Round(bmp.InternalY * s.ZoomY);
                                    s.SrcRect = new Rect(0, 0, bmp.Width, bmp.Height);
                                    RenderSprite(s, bmp, xoffset, yoffset);
                                }
                                s.X = SX;
                                s.Y = SY;
                            }
                            else
                            {
                                RenderSprite(s, s.Bitmap, xoffset, yoffset);
                            }
                        }
                    }
                    Graphics.Log("");
                }
            }
            Graphics.Log("Presenting renderer");
            SDL_RenderPresent(this.SDL_Renderer);
            NeedUpdate = false;
            Graphics.Log("FINISHED render cycle");
            Graphics.Log("=====================\n\n");
        }
    }

    /// <summary>
    /// Renders an individual sprite.
    /// </summary>
    /// <param name="s">The sprite to render.</param>
    public void RenderSprite(Sprite s, Bitmap bmp, int XOffset, int YOffset)
    {
        Graphics.Log($"Rendering sprite {(!string.IsNullOrEmpty(s.Name) ? "'" + s.Name + "' " : "")}-- color: {s.Color} x: {s.X} y: {s.Y} bmp({bmp.Width},{bmp.Height}{(s.Bitmap is SolidBitmap ? ", " + ((SolidBitmap)s.Bitmap).Color.ToString() : "")}) ox: {s.OX} oy: {s.OY} srcrect: {s.SrcRect}");
        IntPtr Texture = IntPtr.Zero;
        if (s.Tone.Red == 0 && s.Tone.Green == 0 && s.Tone.Blue == 0 && s.Tone.Gray == 0 &&
            s.Color.Alpha == 0)
            Texture = bmp.Texture;
        else Texture = bmp.ColorToneTexture(s.Color, s.Tone);

        if (Texture == IntPtr.Zero) throw new Exception("Attempted to render a zero-pointer texture.");

        // Sprite Opacity + Renderer opacity
        byte Alpha = Convert.ToByte(255d * (s.Opacity / 255d) * (this.Opacity / 255d));

        SDL_SetTextureAlphaMod(Texture, Alpha);

        List<Point> Points;
        if (s.MultiplePositions.Count == 0) // Normal X,Y positions
        {
            Points = new List<Point>() { new Point(s.X, s.Y) };
        }
        else // Multiple positions; used to prevent the need for hundreds of identical sprites that only differ in position (e.g. in a background grid)
        {
            Graphics.Log("Multiple positions!");
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
            Dest.w = (bmp as SolidBitmap).BitmapWidth;
            Dest.h = (bmp as SolidBitmap).BitmapHeight;
        }
        else
        {
            Src = s.SrcRect.SDL_Rect;

            // Additional checks, since ZoomX/ZoomY are 1 99% of the time, this way it skips the extra calculation.
            if (s.ZoomX == 1) Dest.w = Src.w;
            else Dest.w = (int)Math.Round(Src.w * s.ZoomX);
            if (s.ZoomY == 1) Dest.h = Src.h;
            else Dest.h = (int)Math.Round(Src.h * s.ZoomY);
        }

        if (!bmp.Locked && !(bmp is SolidBitmap))
        {
            Graphics.Log("ERR: Bitmap is locked");
            throw new BitmapLockedException();
        }
        foreach (Point p in Points)
        {
            int oxoffset = s.FactorZoomIntoOrigin ? (int)Math.Round(s.OX * s.ZoomX) : (int)Math.Round(s.OX);
            int oyoffset = s.FactorZoomIntoOrigin ? (int)Math.Round(s.OY * s.ZoomY) : (int)Math.Round(s.OY);
            Dest.x = p.X - oxoffset + XOffset;
            Dest.y = p.Y - oyoffset + YOffset;

            if (s.Angle % 360 == 0 && s.OX == 0 && s.OY == 0 && !s.MirrorX && !s.MirrorY)
            {
                SDL_RenderCopy(this.SDL_Renderer, Texture, ref Src, ref Dest);
            }
            else
            {
                SDL_Point Center = new SDL_Point();
                Center.x = (int)Math.Round(s.OX);
                Center.y = (int)Math.Round(s.OY);

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
