﻿using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace ODL
{
    public class Renderer
    {
        public List<Viewport> Viewports { get; set; } = new List<Viewport>();
        public IntPtr SDL_Renderer { get; set; }
        public bool Disposed { get; set; } = false;

        private bool ForcedUpdate = false;

        public Renderer(IntPtr SDL_Renderer)
        {
            this.SDL_Renderer = SDL_Renderer;
        }

        public void Update(bool Force = true)
        {
            if (this.ForcedUpdate || Force)
            {
                SDL_RenderClear(this.SDL_Renderer);
                this.Viewports.Sort(delegate (Viewport vp1, Viewport vp2) { return vp1.Z.CompareTo(vp2.Z); });
                for (int i = 0; i < Viewports.Count; i++)
                {
                    Viewport vp = Viewports[i];
                    if (vp.Disposed)
                    {
                        this.Viewports.RemoveAt(i);
                    }
                    else if (vp.Visible && vp.Sprites.Count > 0)
                    {
                        SDL_Rect ViewportRect = new SDL_Rect();
                        SDL_RenderGetViewport(this.SDL_Renderer, out ViewportRect);
                        ViewportRect.x = vp.X;
                        ViewportRect.y = vp.Y;
                        if (vp.Width == -1) vp.Width = ViewportRect.w;
                        if (vp.Height == -1) vp.Height = ViewportRect.h;
                        ViewportRect.w = vp.Width;
                        ViewportRect.h = vp.Height;
                        SDL_RenderSetViewport(this.SDL_Renderer, ref ViewportRect);
                        vp.Sprites.Sort(delegate (Sprite s1, Sprite s2) { return s1.Z.CompareTo(s2.Z); });
                        for (int j = 0; j < vp.Sprites.Count; j++)
                        {
                            Sprite s = vp.Sprites[j];
                            if (s.Disposed)
                            {
                                vp.Sprites.RemoveAt(j);
                            }
                            else if (s.Visible && s.Bitmap != null)
                            {
                                IntPtr Texture = SDL_CreateTextureFromSurface(this.SDL_Renderer, s.Bitmap.Surface);
                                SDL_SetTextureColorMod(Texture, s.Color.Red, s.Color.Green, s.Color.Blue);
                                SDL_SetTextureAlphaMod(Texture, s.Color.Alpha);

                                SDL_Rect Src = s.SrcRect.SDL_Rect;
                                SDL_Rect Dest = new SDL_Rect();
                                Dest.x = s.X - s.OX;
                                Dest.y = s.Y - s.OY;
                                // Additional checks, since ZoomX/ZoomY are 1 99% of the time, this way it skips the extra calculation.
                                if (s.ZoomX == 1) Dest.w = s.SrcRect.Width;
                                else Dest.w = (int) Math.Round(s.SrcRect.Width * s.ZoomX);
                                if (s.ZoomY == 1) Dest.h = s.SrcRect.Height;
                                else Dest.h = (int) Math.Round(s.SrcRect.Height * s.ZoomY);

                                SDL_Point Center = new SDL_Point();
                                Center.x = s.OX;
                                Center.y = s.OY;

                                SDL_RendererFlip MirrorState;
                                if (s.MirrorX && !s.MirrorY) { MirrorState = SDL_RendererFlip.SDL_FLIP_HORIZONTAL; }
                                else if (s.MirrorY && !s.MirrorX) { MirrorState = SDL_RendererFlip.SDL_FLIP_VERTICAL; }
                                else if (s.MirrorX && s.MirrorY) { MirrorState = SDL_RendererFlip.SDL_FLIP_HORIZONTAL | SDL_RendererFlip.SDL_FLIP_VERTICAL; }
                                else { MirrorState = SDL_RendererFlip.SDL_FLIP_NONE; }
                                
                                SDL_RenderCopyEx(this.SDL_Renderer, Texture, ref Src, ref Dest, s.Angle, ref Center, MirrorState);
                                SDL_DestroyTexture(Texture);
                            }
                        }
                    }
                }
                SDL_RenderPresent(this.SDL_Renderer);
                this.ForcedUpdate = false;
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
                Viewports.RemoveAt(i);
            }
            SDL_DestroyRenderer(this.SDL_Renderer);
            this.Disposed = true;
        }
    }
}
