using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_ttf;

namespace ODL
{
    public class Bitmap
    {
        public IntPtr Surface { get; protected set; }
        public SDL_Surface SurfaceObject { get; protected set; }
        public int Width { get { return this.SurfaceObject.w; } }
        public int Height { get { return this.SurfaceObject.h; } }
        public bool Disposed { get; protected set; }
        public Renderer Renderer;
        public Font Font;

        public Bitmap(string Filename)
        {
            this.Surface = SDL2.SDL_image.IMG_Load(Filename);
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
        }

        public Bitmap(Size Size)
            : this(Size.Width, Size.Height) { }
        public Bitmap(int Width, int Height)
        {
            this.Surface = SDL_CreateRGBSurface(0, Width, Height, 32, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
        }

        public Bitmap(IntPtr Surface)
        {
            this.Surface = Surface;
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
        }

        public void Dispose()
        {
            if (this.Font != null) this.Font.Dispose();
            SDL_FreeSurface(this.Surface);
            this.Disposed = true;
            if (this.Renderer != null) this.Renderer.ForceUpdate();
        }

        public override string ToString()
        {
            return $"(Bitmap: {this.Width},{this.Height})";
        }

        public void Clear()
        {
            if (this.Surface != null)
            {
                SDL_FreeSurface(this.Surface);
                this.Surface = SDL_CreateRGBSurface(0, this.Width, this.Height, 32, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);
                this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
                if (this.Renderer != null) this.Renderer.ForceUpdate();
            }
        }

        #region SetPixel Overloads
        public void SetPixel(Point p, Color c)
        {
            SetPixel(p.X, p.Y, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void SetPixel(int X, int Y, Color c)
        {
            SetPixel(X, Y, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void SetPixel(Point p, byte r, byte g, byte b, byte a = 255)
        {
            SetPixel(p.X, p.Y, r, g, b, a);
        }
        #endregion
        public void SetPixel(int X, int Y, byte r, byte g, byte b, byte a = 255)
        {
            int Offset = this.Width * 4 * Y + 4 * X;
            bool e = BitConverter.IsLittleEndian;
            Marshal.WriteByte(SurfaceObject.pixels, Offset,     e ? r : b);
            Marshal.WriteByte(SurfaceObject.pixels, Offset + 1,     g);
            Marshal.WriteByte(SurfaceObject.pixels, Offset + 2, e ? b : r);
            Marshal.WriteByte(SurfaceObject.pixels, Offset + 3,     a);
            if (this.Renderer != null) this.Renderer.ForceUpdate();
        }

        #region GetPixel Overloads
        public Color GetPixel(Point p)
        {
            return GetPixel(p.X, p.Y);
        }
        #endregion
        public Color GetPixel(int X, int Y)
        {
            int Offset = this.Width * 4 * Y + 4 * X;
            byte[] color = new byte[4];
            color[0] = Marshal.ReadByte(SurfaceObject.pixels, Offset);
            color[1] = Marshal.ReadByte(SurfaceObject.pixels, Offset + 1);
            color[2] = Marshal.ReadByte(SurfaceObject.pixels, Offset + 2);
            color[3] = Marshal.ReadByte(SurfaceObject.pixels, Offset + 3);
            if (BitConverter.IsLittleEndian) return new Color(color[0], color[1], color[2], color[3]);
            return new Color(color[2], color[1], color[0], color[3]);
        }

        #region DrawLine Overloads
        public void DrawLine(Point p1, Point p2, Color c)
        {
            DrawLine(p1.X, p1.Y, p2.X, p2.Y, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void DrawLine(Point p1, Point p2, byte r, byte g, byte b, byte a = 255)
        {
            DrawLine(p1.X, p1.Y, p2.X, p2.Y, r, g, b, a);
        }
        public void DrawLine(Point p1, int x2, int y2, Color c)
        {
            DrawLine(p1.X, p1.Y, x2, y2, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void DrawLine(Point p1, int x2, int y2, byte r, byte g, byte b, byte a = 255)
        {
            DrawLine(p1.X, p1.Y, x2, y2, r, g, b, a);
        }
        public void DrawLine(int x1, int y1, Point p2, Color c)
        {
            DrawLine(x1, y1, p2.X, p2.Y, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void DrawLine(int x1, int y1, Point p2, byte r, byte g, byte b, byte a = 255)
        {
            DrawLine(x1, y1, p2.X, p2.Y, r, g, b, a);
        }
        public void DrawLine(int x1, int y1, int x2, int y2, Color c)
        {
            DrawLine(x1, y1, x2, y2, c.Red, c.Green, c.Blue, c.Alpha);
        }
        #endregion
        public void DrawLine(int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a = 255)
        {
            for (int x = x1 > x2 ? x2 : x1; (x1 > x2) ? (x <= x1) : (x <= x2); x++)
            {
                double fact = ((double)x - x1) / (x2 - x1);
                int y = (int)Math.Round(y1 + ((y2 - y1) * fact));
                if (y >= 0) SetPixel(x, y, r, g, b, a);
            }
            int sy = y1 > y2 ? y2 : y1;
            for (int y = y1 > y2 ? y2 : y1; (y1 > y2) ? (y <= y1) : (y <= y2); y++)
            {
                double fact = ((double) y - y1) / (y2 - y1);
                int x = (int) Math.Round(x1 + ((x2 - x1) * fact));
                if (x >= 0) SetPixel(x, y, r, g, b, a);
            }
            if (this.Renderer != null) this.Renderer.ForceUpdate();
        }

        #region DrawLines Overloads
        public void DrawLines(Color c, params Point[] points)
        {
            this.DrawLines(c.Red, c.Green, c.Blue, c.Alpha, points);
        }
        public void DrawLines(byte r, byte g, byte b, params Point[] points)
        {
            this.DrawLines(r, g, b, 255, points);
        }
        #endregion
        public void DrawLines(byte r, byte g, byte b, byte a, params Point[] points)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                this.DrawLine(points[i], points[i + 1], r, g, b, a);
            }
        }

        #region DrawCircle
        public void DrawCircle(Point c, int Radius, Color color)
        {
            DrawCircle(c.X, c.Y, Radius, color.Red, color.Green, color.Blue, color.Alpha);
        }
        public void DrawCircle(int ox, int oy, int Radius, Color c)
        {
            DrawCircle(ox, oy, Radius, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void DrawCircle(Point c, int Radius, byte r, byte g, byte b, byte a = 255)
        {
            DrawCircle(c.X, c.Y, Radius, r, g, b, a);
        }
        #endregion
        public void DrawCircle(int ox, int oy, int Radius, byte r, byte g, byte b, byte a = 255)
        {
            int x = Radius - 1;
            int y = 0;
            int dx = 1;
            int dy = 1;
            int err = dx - (Radius << 1);
            while (x >= y)
            {
                SetPixel(ox - x, oy - y, r, g, b, a);
                SetPixel(ox - x, oy + y, r, g, b, a);
                SetPixel(ox + x, oy - y, r, g, b, a);
                SetPixel(ox + x, oy + y, r, g, b, a);
                SetPixel(ox - y, oy - x, r, g, b, a);
                SetPixel(ox - y, oy + x, r, g, b, a);
                SetPixel(ox + y, oy - x, r, g, b, a);
                SetPixel(ox + y, oy + x, r, g, b, a);
                if (err <= 0)
                {
                    y++;
                    err += dy;
                    dy += 2;
                }
                if (err > 0)
                {
                    x--;
                    dx += 2;
                    err += dx - (Radius << 1);
                }
            }
            if (this.Renderer != null) this.Renderer.ForceUpdate();
        }

        #region FillCircle
        public void FillCircle(Point c, int Radius, Color color)
        {
            FillCircle(c.X, c.Y, Radius, color.Red, color.Green, color.Blue, color.Alpha);
        }
        public void FillCircle(int ox, int oy, int Radius, Color c)
        {
            FillCircle(ox, oy, Radius, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void FillCircle(Point c, int Radius, byte r, byte g, byte b, byte a = 255)
        {
            FillCircle(c.X, c.Y, Radius, r, g, b, a);
        }
        #endregion
        public void FillCircle(int ox, int oy, int Radius, byte r, byte g, byte b, byte a = 255)
        {
            int x = Radius - 1;
            int y = 0;
            int dx = 1;
            int dy = 1;
            int err = dx - (Radius << 1);
            while (x >= y)
            {
                for (int i = ox - x; i <= ox + x; i++)
                {
                    SetPixel(i, oy + y, r, g, b, a);
                    SetPixel(i, oy - y, r, g, b, a);
                }
                for (int i = oy - y; i <= ox + y; i++)
                {
                    SetPixel(i, oy + x, r, g, b, a);
                    SetPixel(i, oy - x, r, g, b, a);
                }

                y++;
                err += dy;
                dy += 2;
                if (err > 0)
                {
                    x--;
                    dx += 2;
                    err += dx - (Radius << 1);
                }
            }
            if (this.Renderer != null) this.Renderer.ForceUpdate();
        }

        #region DrawRect Overloads
        public void DrawRect(Rect r, Color c)
        {
            DrawRect(r.X, r.Y, r.Width, r.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void DrawRect(Rect rect, byte r, byte g, byte b, byte a = 255)
        {
            DrawRect(rect.X, rect.Y, rect.Width, rect.Height, r, g, b, a);
        }
        public void DrawRect(Point Point, Size Size, Color c)
        {
            DrawRect(Point.X, Point.Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void DrawRect(Point Point, Size Size, byte r, byte g, byte b, byte a = 255)
        {
            DrawRect(Point.X, Point.Y, Size.Width, Size.Height, r, g, b, a);
        }
        public void DrawRect(Point Point, int Width, int Height, Color c)
        {
            DrawRect(Point.X, Point.Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void DrawRect(Point Point, int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            DrawRect(Point.X, Point.Y, Width, Height, r, g, b, a);
        }
        public void DrawRect(Size size, byte r, byte g, byte b, byte a = 255)
        {
            this.DrawRect(0, 0, size, r, g, b, a);
        }
        public void DrawRect(Size size, Color c)
        {
            this.DrawRect(0, 0, size, c);
        }
        public void DrawRect(int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            this.DrawRect(0, 0, Width, Height, r, g, b, a);
        }
        public void DrawRect(int Width, int Height, Color c)
        {
            this.DrawRect(0, 0, Width, Height, c);
        }
        public void DrawRect(int X, int Y, Size Size, Color c)
        {
            DrawRect(X, Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void DrawRect(int X, int Y, Size Size, byte r, byte g, byte b, byte a = 255)
        {
            DrawRect(X, Y, Size.Width, Size.Height, r, g, b, a);
        }
        public void DrawRect(int X, int Y, int Width, int Height, Color c)
        {
            DrawRect(X, Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        #endregion
        public void DrawRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            DrawLine(X, Y, X + Width - 1, Y, r, g, b, a);
            DrawLine(X, Y, X, Y + Height - 1, r, g, b, a);
            DrawLine(X, Y + Height - 1, X + Width - 1, Y + Height - 1, r, g, b, a);
            DrawLine(X + Width - 1, Y, X + Width - 1, Y + Height - 1, r, g, b, a);
            if (this.Renderer != null) this.Renderer.ForceUpdate();
        }

        #region FillRect Overloads
        public void FillRect(Rect r, Color c)
        {
            FillRect(r.X, r.Y, r.Width, r.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void FillRect(Rect rect, byte r, byte g, byte b, byte a = 255)
        {
            FillRect(rect.X, rect.Y, rect.Width, rect.Height, r, g, b, a);
        }
        public void FillRect(Point Point, Size Size, Color c)
        {
            FillRect(Point.X, Point.Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void FillRect(Point Point, Size Size, byte r, byte g, byte b, byte a = 255)
        {
            FillRect(Point.X, Point.Y, Size.Width, Size.Height, r, g, b, a);
        }
        public void FillRect(Point Point, int Width, int Height, Color c)
        {
            FillRect(Point.X, Point.Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void FillRect(Point Point, int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            FillRect(Point.X, Point.Y, Width, Height, r, g, b, a);
        }
        public void FillRect(Size size, byte r, byte g, byte b, byte a = 255)
        {
            this.FillRect(0, 0, size, r, g, b, a);
        }
        public void FillRect(Size size, Color c)
        {
            this.FillRect(0, 0, size, c);
        }
        public void FillRect(int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            this.FillRect(0, 0, Width, Height, r, g, b, a);
        }
        public void FillRect(int Width, int Height, Color c)
        {
            this.FillRect(0, 0, Width, Height, c);
        }
        public void FillRect(int X, int Y, Size Size, Color c)
        {
            FillRect(X, Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        public void FillRect(int X, int Y, Size Size, byte r, byte g, byte b, byte a = 255)
        {
            FillRect(X, Y, Size.Width, Size.Height, r, g, b, a);
        }
        public void FillRect(int X, int Y, int Width, int Height, Color c)
        {
            FillRect(X, Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        #endregion
        public void FillRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            SDL_Rect Rect = new Rect(X, Y, Width, Height).SDL_Rect;
            SDL_FillRect(this.Surface, ref Rect, SDL_MapRGBA(this.SurfaceObject.format, r, g, b, a));
            if (this.Renderer != null) this.Renderer.ForceUpdate();
        }

        #region Build Overloads
        public void Build(Rect DestRect, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(DestRect, SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        public void Build(Rect DestRect, Bitmap SrcBitmap)
        {
            this.Build(DestRect, SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height));
        }
        public void Build(Point DP, Size DS, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DP, DS), SrcBitmap, SrcRect);
        }
        public void Build(Point DP, Size DS, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DP, DS), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        public void Build(Point DP, int DWidth, int DHeight, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DP, DWidth, DHeight), SrcBitmap, SrcRect);
        }
        public void Build(Point DP, int DWidth, int DHeight, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DP, DWidth, DHeight), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        public void Build(int DX, int DY, Size DS, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DX, DY, DS), SrcBitmap, SrcRect);
        }
        public void Build(int DX, int DY, Size DS, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DX, DY, DS), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        public void Build(int DX, int DY, int DWidth, int DHeight, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DX, DY, DWidth, DHeight), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        public void Build(int DX, int DY, int DWidth, int DHeight, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DX, DY, DWidth, DHeight), SrcBitmap, SrcRect);
        }
        public void Build(Point DP, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DP, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, SrcRect);
        }
        public void Build(Point DP, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DP, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        public void Build(int DX, int DY, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DX, DY, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, SrcRect);
        }
        public void Build(int DX, int DY, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DX, DY, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        public void Build(Point DP, Bitmap SrcBitmap)
        {
            this.Build(new Rect(DP, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height));
        }
        public void Build(int DX, int DY, Bitmap SrcBitmap)
        {
            this.Build(new Rect(DX, DY, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height));
        }
        public void Build(Bitmap SrcBitmap)
        {
            this.Build(new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height));
        }
        #endregion
        public void Build(Rect DestRect, Bitmap SrcBitmap, Rect SrcRect)
        {
            SDL_Rect Src = SrcRect.SDL_Rect;
            SDL_Rect Dest = DestRect.SDL_Rect;
            SDL_BlitSurface(SrcBitmap.Surface, ref Src, this.Surface, ref Dest);
        }

        public Size TextSize(string Text, DrawOptions DrawOptions = 0)
        {
            IntPtr SDL_Font = this.Font.SDL_Font;
            TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
            int w, h;
            TTF_SizeText(SDL_Font, Text, out w, out h);
            return new Size(w, h);
        }

        #region DrawText Overloads
        public void DrawText(string Text, int X, int Y, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawText(Text, X, Y, new Color(R, G, B, A), DrawOptions);
        }
        public void DrawText(string Text, Point p, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawText(Text, p.X, p.Y, c, DrawOptions);
        }
        public void DrawText(string Text, Point p, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawText(Text, p.X, p.Y, new Color(R, G, B, A), DrawOptions);
        }
        #endregion
        public void DrawText(string Text, int X, int Y, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            if (this.Font == null)
            {
                throw new Exception("No Font specified for this Bitmap.");
            }
            if (Text == "") return;
            IntPtr SDL_Font = this.Font.SDL_Font;
            bool solid = (DrawOptions & DrawOptions.Solid) == DrawOptions.Solid;
            bool leftalign = (DrawOptions & DrawOptions.LeftAlign) == DrawOptions.LeftAlign;
            bool centeralign = (DrawOptions & DrawOptions.CenterAlign) == DrawOptions.CenterAlign;
            bool rightalign = (DrawOptions & DrawOptions.RightAlign) == DrawOptions.RightAlign;
            if (leftalign && centeralign || leftalign && rightalign || centeralign && rightalign)
            {
                throw new Exception("Multiple alignments specified in DrawText DrawOptions - can only contain one alignment setting");
            }
            if (!leftalign && !centeralign && !rightalign) leftalign = true;
            TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
            Bitmap TextBitmap;
            if (solid) TextBitmap = new Bitmap(TTF_RenderText_Solid(  SDL_Font, Text, c.SDL_Color));
            else       TextBitmap = new Bitmap(TTF_RenderText_Blended(SDL_Font, Text, c.SDL_Color));
            if (centeralign) X -= TextBitmap.Width / 2;
            if (rightalign)  X -= TextBitmap.Width;
            this.Build(new Rect(X, Y, TextBitmap.Width, TextBitmap.Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
            TextBitmap.Dispose();
        }
    }

    public enum DrawOptions
    {
        Bold          = 1,
        Italic        = 2,
        Underlined    = 4,
        Strikethrough = 8,
        Solid         = 16,
        LeftAlign     = 32,
        CenterAlign   = 64,
        RightAlign    = 128
    }
}
