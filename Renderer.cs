using System;
using System.Collections.Generic;
using System.Diagnostics;
using static SDL2.SDL;

namespace ODL
{
    public class Renderer : IDisposable
    {
        public List<Viewport> Viewports = new List<Viewport>();
        public IntPtr SDL_Renderer;
        public bool Disposed = false;

        private bool ForcedUpdate = false;

        public Renderer(IntPtr SDL_Renderer)
        {
            this.SDL_Renderer = SDL_Renderer;
            Graphics.RegisterRenderer(this);
        }

        public void Update(bool Force = true)
        {
            if (Force) this.ForcedUpdate = true;
        }

        public void UpdateGraphics()
        {
            if (this.ForcedUpdate)
            {
                Graphics.Log("\n=====================");
                Graphics.Log("START Rendering");
                long l1 = Stopwatch.GetTimestamp();
                SDL_RenderClear(this.SDL_Renderer);
                Graphics.Log("Viewports count " + this.Viewports.Count.ToString());
                this.Viewports.Sort(delegate (Viewport vp1, Viewport vp2) {
                    if (vp1.Z != vp2.Z) return vp1.Z.CompareTo(vp2.Z);
                    return vp1.TimeCreated.CompareTo(vp2.TimeCreated);
                });
                Graphics.Log("Viewports sorted");
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
                        Graphics.Log("Drawing Viewport " + i.ToString() + (!string.IsNullOrEmpty(vp.Name) ? ": " + vp.Name : ""));
                        SDL_Rect ViewportRect = new SDL_Rect();
                        SDL_RenderGetViewport(this.SDL_Renderer, out ViewportRect);
                        ViewportRect.x = vp.X;
                        ViewportRect.y = vp.Y;
                        if (vp.Width == -1) vp.Width = ViewportRect.w;
                        if (vp.Height == -1) vp.Height = ViewportRect.h;
                        ViewportRect.w = vp.Width;
                        ViewportRect.h = vp.Height;
                        SDL_RenderSetScale(SDL_Renderer, (float) vp.ZoomX, (float) vp.ZoomY);
                        SDL_RenderSetViewport(this.SDL_Renderer, ref ViewportRect);
                        Graphics.Log($"Viewport rect set to ({ViewportRect.x},{ViewportRect.y},{ViewportRect.w},{ViewportRect.h})");
                        Graphics.Log("Sprite count " + vp.Sprites.Count.ToString());
                        vp.Sprites.Sort(delegate (Sprite s1, Sprite s2) 
                        {
                            if (s1.Z != s2.Z) return s1.Z.CompareTo(s2.Z);
                            return s1.TimeCreated.CompareTo(s2.TimeCreated);
                        });
                        Graphics.Log("Sprites sorted");
                        for (int j = 0; j < vp.Sprites.Count; j++)
                        {
                            Sprite s = vp.Sprites[j];
                            if (s.Disposed)
                            {
                                vp.Sprites.RemoveAt(j);
                                Graphics.Log("Sprite " + j.ToString() + " removed as it was disposed");
                            }
                            else if (s.Visible)
                            {
                                if (s.Bitmap != null) RenderSprite(s);
                            }
                        }
                    }
                }
                Graphics.Log("Presenting renderer");
                SDL_RenderPresent(this.SDL_Renderer);
                this.ForcedUpdate = false;
                Graphics.Log("FINISHED rendering");
                Graphics.Log("=====================\n");
            }
        }

        public void RenderSprite(Sprite s)
        {
            Graphics.Log("Rendering sprite" + (!string.IsNullOrEmpty(s.Name) ? ": " + s.Name : ""));
            IntPtr Texture = s.Bitmap.Texture;
            SDL_SetTextureColorMod(Texture, s.Color.Red, s.Color.Green, s.Color.Blue);
            SDL_SetTextureAlphaMod(Texture, Convert.ToByte(255d * ((s.Color.Alpha / 255d) * (s.Opacity / 255d))));

            List<Point> Points;
            if (s.MultiplePositions.Count == 0) // Normal X,Y positions
            {
                Points = new List<Point>() { new Point(s.X, s.Y) };
            }
            else // Multiple positions; used to prevent the need for hundreds of identical sprites that only differ in position (e.g. in a background grid)
            {
                Graphics.Log("Has multiple positions");
                Points = s.MultiplePositions;
            }

            SDL_Rect Src = new SDL_Rect();
            SDL_Rect Dest = new SDL_Rect();
            if (!(s.Bitmap is SolidBitmap))
            {
                Src = s.SrcRect.SDL_Rect;

                // Make sure the Dest size is never bigger than the Bitmap size (otherwise it'll stretch the bitmap)
                if (Src.w > s.Bitmap.Width * s.ZoomX) Src.w = s.Bitmap.Width;
                if (Src.h > s.Bitmap.Height * s.ZoomY) Src.h = s.Bitmap.Height;

                // Additional checks, since ZoomX/ZoomY are 1 99% of the time, this way it skips the extra calculation.
                if (s.ZoomX == 1) Dest.w = Src.w;
                else Dest.w = (int) Math.Round(Src.w * s.ZoomX);
                if (s.ZoomY == 1) Dest.h = Src.h;
                else Dest.h = (int) Math.Round(Src.h * s.ZoomY);
                Graphics.Log("Bitmap");
            }
            else // If s.Bitmap is SolidBitmap
            {
                Src.x = 0;
                Src.y = 0;
                Src.w = 1;
                Src.h = 1;
                Dest.w = (s.Bitmap as SolidBitmap).BitmapWidth;
                Dest.h = (s.Bitmap as SolidBitmap).BitmapHeight;
                Graphics.Log("SolidBitmap");
            }

            if (!s.Bitmap.Locked && !(s.Bitmap is SolidBitmap))
            {
                Graphics.Log("ERR: Bitmap is locked");
                throw new Exception("Bitmap not locked for writing - can't render it");
            }
            foreach (Point p in Points)
            {
                Dest.x = p.X - s.OX;
                Dest.y = p.Y - s.OY;

                if (s.Angle % 360 == 0 && s.OX == 0 && s.OY == 0 && !s.MirrorX && !s.MirrorY)
                {
                    SDL_RenderCopy(this.SDL_Renderer, Texture, ref Src, ref Dest);
                    Graphics.Log("Rendered successfully");
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
                    Graphics.Log("Rendered special successfully");
                }
                // Don't destroy the texture as it can be reused next render
            }
        }

        public void ForceUpdate()
        {
            this.ForcedUpdate = true;
        }

        public void Dispose()
        {
            for (int i = 0; i < Viewports.Count; i++)
            {
                Viewports[i].Dispose();
            }
            SDL_DestroyRenderer(this.SDL_Renderer);
            this.Disposed = true;
        }
    }
}
