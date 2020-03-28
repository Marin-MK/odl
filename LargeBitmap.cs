using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_ttf;

namespace ODL
{
    public class LargeBitmap : Bitmap
    {
        protected int _width;
        /// <summary>
        /// The width of the bitmap.
        /// </summary>
        public override int Width { get { return _width; } protected set { _width = value; } }
        protected int _height;
        /// <summary>
        /// The height of the bitmap.
        /// </summary>
        public override int Height { get { return _height; } protected set { _height = value; } }
        /// <summary>
        /// The Font object associated with the bitmap.
        /// </summary>
        public override Font Font { get { return base.Font; } set { base.Font = value; foreach (Bitmap b in this.InternalBitmaps) b.Font = value; } }
        /// <summary>
        /// The Renderer object associated with the bitmap.
        /// </summary>
        public override Renderer Renderer { get { return base.Renderer; } set { base.Renderer = value; foreach (Bitmap b in this.InternalBitmaps) b.Renderer = value; } }

        public List<Bitmap> InternalBitmaps = new List<Bitmap>();
        public Size ChunkSize { get; protected set; }

        /// <summary>
        /// Loads the specified file into a bitmap.
        /// </summary>
        /// <param name="Filename">The file to load into a bitmap.</param>
        public LargeBitmap(string Filename)
            : base()
        {
            throw new MethodNotSupportedException(this);
        }

        /// <summary>
        /// Creates a new bitmap with the given size.
        /// </summary>
        /// <param name="Size">The size of the new bitmap.</param>
        public LargeBitmap(int Width, int Height, int ChunkWidth, int ChunkHeight)
            : this(Width, Height, new Size(ChunkWidth, ChunkHeight)) { }
        /// Creates a new bitmap with the given size.
        /// </summary>
        /// <param name="Size">The size of the new bitmap.</param>
        public LargeBitmap(Size Size, int ChunkWidth, int ChunkHeight)
            : this(Size.Width, Size.Height, ChunkWidth, ChunkHeight) { }
        /// <summary>
        /// Creates a new bitmap with the given size.
        /// </summary>
        /// <param name="Size">The size of the new bitmap.</param>
        /// <summary>
        public LargeBitmap(Size Size, Size ChunkSize)
            : this(Size.Width, Size.Height, ChunkSize) { }
        /// Creates a new bitmap with the given size.
        /// </summary>
        /// <param name="Width">The width of the new bitmap.</param>
        /// <param name="Height">The height of the new bitmap.</param>
        public LargeBitmap(int Width, int Height, Size ChunkSize)
            : base()
        {
            if (Width < 1 || Height < 1)
            {
                throw new Exception($"Invalid LargeBitmap size ({Width},{Height}) -- must be at least (1,1)");
            }
            this.Width = Width;
            this.Height = Height;
            this.ChunkSize = ChunkSize;
            int ChunkCountHor = (int) Math.Ceiling((double) Width / ChunkSize.Width);
            int ChunkCountVer = (int) Math.Ceiling((double) Height / ChunkSize.Height);
            for (int x = 0; x < ChunkCountHor; x++)
            {
                for (int y = 0; y < ChunkCountVer; y++)
                {
                    int w = Math.Min(ChunkSize.Width, Width - x * ChunkSize.Width);
                    int h = Math.Min(ChunkSize.Height, Height - y * ChunkSize.Height);
                    Bitmap b = new Bitmap(w, h);
                    b.InternalX = x * ChunkSize.Width;
                    b.InternalY = y * ChunkSize.Height;
                    InternalBitmaps.Add(b);
                }
            }
            this.Lock();
        }
        /// <summary>
        /// Creates a bitmap object to wrap around an existing SDL_Surface.
        /// </summary>
        /// <param name="Surface"></param>
        public LargeBitmap(IntPtr Surface)
        {
            throw new MethodNotSupportedException(this);
        }

        ~LargeBitmap()
        {
            if (!Disposed && InternalBitmaps.Count > 0 && InternalBitmaps.Any(b => !b.Disposed && (b.Surface != IntPtr.Zero || b.Texture != IntPtr.Zero)))
            {
                Console.WriteLine($"An undisposed LargeBitmap is being collected by the GC! This is a memory leak!\n    LargeBitmap info: Size ({Width},{Height})");
            }
        }

        /// <summary>
        /// Disposes and destroys the bitmap.
        /// </summary>
        public override void Dispose()
        {
            if (Disposed) return;
            foreach (Bitmap b in this.InternalBitmaps) b.Dispose();
            this.InternalBitmaps.Clear();
            this.Disposed = true;
            if (this.Renderer != null) this.Renderer.Update();
        }

        public override string ToString()
        {
            return $"(LargeBitmap: {this.Width},{this.Height})";
        }

        /// <summary>
        /// Clears the bitmap content.
        /// </summary>
        public override void Clear()
        {
            if (Locked) throw new BitmapLockedException();
            foreach (Bitmap b in this.InternalBitmaps) b.Clear();
        }

        /// <summary>
        /// Creates a clone of the bitmap.
        /// </summary>
        public override Bitmap Clone()
        {
            throw new MethodNotSupportedException(this);
        }

        private Bitmap GetBitmapFromCoordinate(int X, int Y)
        {
            foreach (Bitmap b in this.InternalBitmaps)
            {
                if (X >= b.InternalX && X < b.InternalX + this.ChunkSize.Width &&
                    Y >= b.InternalY && Y < b.InternalY + this.ChunkSize.Height)
                    return b;
            }
            return null;
        }
        
        /// <summary>
        /// Sets a pixel in the bitmap to the specified color.
        /// </summary>
        /// <param name="X">The X position in the bitmap.</param>
        /// <param name="Y">The Y position in the bitmap.</param>
        /// <param name="r">The Red component of the color to set the pixel to.</param>
        /// <param name="g">The Green component of the color to set the pixel to.</param>
        /// <param name="b">The Blue component of the color to set the pixel to.</param>
        /// <param name="a">The Alpha component of the color to set the pixel to.</param>
        public override void SetPixel(int X, int Y, byte r, byte g, byte b, byte a = 255)
        {
            if (Locked) throw new BitmapLockedException();
            if (X < 0 || Y < 0)
            {
                throw new Exception($"Invalid LargeBitmap coordinate ({X},{Y}) -- minimum is (0,0)");
            }
            if (X >= this.Width || Y >= this.Height)
            {
                throw new Exception($"Invalid LargeBitmap coordinate ({X},{Y}) -- exceeds LargeBitmap size of ({this.Width},{this.Height})");
            }
            Bitmap bmp = GetBitmapFromCoordinate(X, Y);
            if (bmp.Locked) bmp.Unlock();
            bmp.SetPixel(X - bmp.InternalX, Y - bmp.InternalY, r, g, b, a);
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Returns the color at the given position.
        /// </summary>
        /// <param name="X">The X position in the bitmap.</param>
        /// <param name="Y">The Y position in the bitmap.</param>
        public override Color GetPixel(int X, int Y)
        {
            if (X < 0 || Y < 0)
            {
                throw new Exception($"Invalid LargeBitmap coordinate ({X},{Y}) -- minimum is (0,0)");
            }
            if (X >= this.Width || Y >= this.Height)
            {
                throw new Exception($"Invalid LargeBitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
            }
            Bitmap bmp = GetBitmapFromCoordinate(X, Y);
            return bmp.GetPixel(X - bmp.InternalX, Y - bmp.InternalY);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="x1">The X position of the first point.</param>
        /// <param name="y1">The Y position of the first point.</param>
        /// <param name="x2">The X position of the second point.</param>
        /// <param name="y2">The Y position of the second point.</param>
        /// <param name="r">The Red component of the color to draw the line with.</param>
        /// <param name="g">The Green component of the color to draw the line with.</param>
        /// <param name="b">The Blue component of the color to draw the line with.</param>
        /// <param name="a">The Alpha component of the color to draw the line with.</param>
        public override void DrawLine(int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a = 255)
        {
            if (Locked) throw new BitmapLockedException();
            for (int x = x1 > x2 ? x2 : x1; (x1 > x2) ? (x <= x1) : (x <= x2); x++)
            {
                double fact = ((double) x - x1) / (x2 - x1);
                int y = (int) Math.Round(y1 + ((y2 - y1) * fact));
                if (y >= 0) SetPixel(x, y, r, g, b, a);
            }
            int sy = y1 > y2 ? y2 : y1;
            for (int y = y1 > y2 ? y2 : y1; (y1 > y2) ? (y <= y1) : (y <= y2); y++)
            {
                double fact = ((double) y - y1) / (y2 - y1);
                int x = (int) Math.Round(x1 + ((x2 - x1) * fact));
                if (x >= 0) SetPixel(x, y, r, g, b, a);
            }
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Draws lines between the given points.
        /// </summary>
        /// <param name="r">The Red component of the color to draw the lines with.</param>
        /// <param name="g">The Green component of the color to draw the lines with.</param>
        /// <param name="b">The Blue component of the color to draw the lines with.</param>
        /// <param name="a">The Alpha component of the color to draw the lines with.</param>
        /// <param name="points">The list of points to draw the lines between.</param>
        public override void DrawLines(byte r, byte g, byte b, byte a, params Point[] points)
        {
            if (Locked) throw new BitmapLockedException();
            for (int i = 0; i < points.Length - 1; i++)
            {
                this.DrawLine(points[i], points[i + 1], r, g, b, a);
            }
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="ox">The origin X position of the circle.</param>
        /// <param name="oy">The origin Y position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="r">The Red component of the color to draw the circle with.</param>
        /// <param name="g">The Green component of the color to draw the circle with.</param>
        /// <param name="b">The Blue component of the color to draw the circle with.</param>
        /// <param name="a">The Alpha component of the color to draw the circle with.</param>
        public override void DrawCircle(int ox, int oy, int Radius, byte r, byte g, byte b, byte a = 255)
        {
            if (Locked) throw new BitmapLockedException();
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
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Fills a circle.
        /// </summary>
        /// <param name="ox">The origin X position of the circle.</param>
        /// <param name="oy">The origin Y position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="r">The Red component of the color to fill the circle with.</param>
        /// <param name="g">The Green component of the color to fill the circle with.</param>
        /// <param name="b">The Blue component of the color to fill the circle with.</param>
        /// <param name="a">The Alpha component of the color to fill the circle with.</param>
        public override void FillCircle(int ox, int oy, int Radius, byte r, byte g, byte b, byte a = 255)
        {
            if (Locked) throw new BitmapLockedException();
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
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Draws a quarter of a circle.
        /// </summary>
        /// <param name="ox">The origin X position of the circle.</param>
        /// <param name="oy">The origin Y position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="l">The part of the circle to draw.</param>
        /// <param name="r">The Red component of the color to draw the quadrant with.</param>
        /// <param name="g">The Green component of the color to draw the quadrant with.</param>
        /// <param name="b">The Blue component of the color to draw the quadrant with.</param>
        /// <param name="a">The Alpha component of the color to draw the quadrant with.</param>
        public override void DrawQuadrant(int ox, int oy, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
        {
            if (Locked) throw new BitmapLockedException();
            int x = Radius - 1;
            int y = 0;
            int dx = 1;
            int dy = 1;
            int err = dx - (Radius << 1);
            while (x >= y)
            {
                if (l == Location.TopRight) // 0 - 90
                {
                    SetPixel(ox + y, oy - x, r, g, b, a);
                    SetPixel(ox + x, oy - y, r, g, b, a);
                }
                else if (l == Location.TopLeft) // 90 - 180
                {
                    SetPixel(ox - x, oy - y, r, g, b, a);
                    SetPixel(ox - y, oy - x, r, g, b, a);
                }
                else if (l == Location.BottomLeft) // 180 - 270
                {
                    SetPixel(ox - x, oy + y, r, g, b, a);
                    SetPixel(ox - y, oy + x, r, g, b, a);
                }
                else if (l == Location.BottomRight) // 270 - 360
                {
                    SetPixel(ox + x, oy + y, r, g, b, a);
                    SetPixel(ox + y, oy + x, r, g, b, a);
                }
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
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Fills a quarter of a circle.
        /// </summary>
        /// <param name="ox">The origin X position of the circle.</param>
        /// <param name="oy">The origin Y position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="l">The part of the circle to draw.</param>
        /// <param name="r">The Red component of the color to fill the quadrant with.</param>
        /// <param name="g">The Green component of the color to fill the quadrant with.</param>
        /// <param name="b">The Blue component of the color to fill the quadrant with.</param>
        /// <param name="a">The Alpha component of the color to fill the quadrant with.</param>
        public override void FillQuadrant(int ox, int oy, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
        {
            if (Locked) throw new BitmapLockedException();
            int x = Radius - 1;
            int y = 0;
            int dx = 1;
            int dy = 1;
            int err = dx - (Radius << 1);
            while (x >= y)
            {
                if (l == Location.TopRight) // 0 - 90
                {
                    for (int i = ox + y; i <= ox + x; i++)
                    {
                        SetPixel(i, oy - y, r, g, b, a);
                    }
                    for (int i = oy - x; i <= oy - y; i++)
                    {
                        SetPixel(ox + y, i, r, g, b, a);
                    }
                }
                else if (l == Location.TopLeft) // 90 - 180
                {
                    for (int i = ox - x; i <= ox - y; i++)
                    {
                        SetPixel(i, oy - y, r, g, b, a);
                    }
                    for (int i = oy - x; i <= oy - y; i++)
                    {
                        SetPixel(ox - y, i, r, g, b, a);
                    }
                }
                else if (l == Location.BottomLeft) // 180 - 270
                {
                    for (int i = ox - x; i <= ox - y; i++)
                    {
                        SetPixel(i, oy + y, r, g, b, a);
                    }
                    for (int i = oy + y; i <= oy + x; i++)
                    {
                        SetPixel(ox - y, i, r, g, b, a);
                    }
                }
                else if (l == Location.BottomRight) // 270 - 360
                {
                    for (int i = ox + y; i <= ox + x; i++)
                    {
                        SetPixel(i, oy + y, r, g, b, a);
                    }
                    for (int i = oy + y; i <= oy + x; i++)
                    {
                        SetPixel(ox + y, i, r, g, b, a);
                    }
                }
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
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="X">The X position of the rectangle.</param>
        /// <param name="Y">The Y position of the rectangle.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="r">The Red component of the color to draw the rectangle with.</param>
        /// <param name="g">The Green component of the color to draw the rectangle with.</param>
        /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
        public override void DrawRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            if (Locked) throw new BitmapLockedException();
            if (X < 0 || Y < 0)
            {
                throw new Exception($"Invalid LargeBitmap coordinate ({X},{Y}) -- minimum is (0,0)");
            }
            if (X + Width > this.Width || Y + Height > this.Height)
            {
                throw new Exception($"Invalid LargeBitmap coordinate ({X},{Y}) -- exceeds LargeBitmap size of ({this.Width},{this.Height})");
            }
            DrawLine(X, Y, X + Width - 1, Y, r, g, b, a);
            DrawLine(X, Y, X, Y + Height - 1, r, g, b, a);
            DrawLine(X, Y + Height - 1, X + Width - 1, Y + Height - 1, r, g, b, a);
            DrawLine(X + Width - 1, Y, X + Width - 1, Y + Height - 1, r, g, b, a);
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="X">The X position of the rectangle.</param>
        /// <param name="Y">The Y position of the rectangle.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="r">The Red component of the color to fill the rectangle with.</param>
        /// <param name="g">The Green component of the color to fill the rectangle with.</param>
        /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
        public override void FillRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            if (Locked) throw new BitmapLockedException();
            if (X < 0 || Y < 0)
            {
                throw new Exception($"Invalid LargeBitmap coordinate ({X},{Y}) -- minimum is (0,0)");
            }
            if (X >= this.Width || Y >= this.Height)
            {
                throw new Exception($"Invalid LargeBitmap coordinate ({X},{Y}) -- exceeds LargeBitmap size of ({this.Width},{this.Height})");
            }
            if (X + Width - 1 >= this.Width || Y + Height - 1 >= this.Height)
            {
                throw new Exception($"Invalid rectangle ({X},{Y},{Width},{Height}) -- exceeds LargeBitmap size of ({this.Width},{this.Height})");
            }
            Rect DestRect = new Rect(X, Y, Width, Height);
            foreach (Bitmap bmp in this.InternalBitmaps)
            {
                Rect bmprect = new Rect(bmp.InternalX, bmp.InternalY, this.ChunkSize);
                if (DestRect.Overlaps(bmprect))
                {
                    int nx = Math.Max(DestRect.X, bmprect.X);
                    int ny = Math.Max(DestRect.Y, bmprect.Y);
                    int nw = Math.Min(DestRect.X + DestRect.Width, bmprect.X + bmprect.Width) - nx;
                    int nh = Math.Min(DestRect.Y + DestRect.Height, bmprect.Y + bmprect.Height) - ny;
                    if (bmp.Locked) bmp.Unlock();
                    int DX = nx - bmp.InternalX;
                    int DY = ny - bmp.InternalY;
                    int DW = nw;
                    int DH = nh;
                    bmp.FillRect(DX, DY, nw, nh, r, g, b, a);
                }
            }
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="DestRect">The available rectangle in the destination bitmap to draw the source bitmap in.</param>
        /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
        /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
        public override void Build(Rect DestRect, Bitmap SrcBitmap, Rect SrcRect)
        {
            if (Locked) throw new BitmapLockedException();
            foreach (Bitmap bmp in this.InternalBitmaps)
            {
                Rect bmprect = new Rect(bmp.InternalX, bmp.InternalY, this.ChunkSize);
                if (DestRect.Overlaps(bmprect))
                {
                    int nx = Math.Max(DestRect.X, bmprect.X);
                    int ny = Math.Max(DestRect.Y, bmprect.Y);
                    int nw = Math.Min(DestRect.X + DestRect.Width, bmprect.X + bmprect.Width) - nx;
                    int nh = Math.Min(DestRect.Y + DestRect.Height, bmprect.Y + bmprect.Height) - ny;
                    if (bmp.Locked) bmp.Unlock();
                    int DX = nx - bmp.InternalX;
                    int DY = ny - bmp.InternalY;
                    int DW = nw;
                    int DH = nh;
                    int SX = SrcRect.X + nx - DestRect.X;
                    int SY = SrcRect.Y + ny - DestRect.Y;
                    int SW = DW;
                    int SH = DH;
                    bmp.Build(
                        DX, DY, DW, DH,
                        SrcBitmap,
                        SX, SY, SW, SH
                    );
                }
            }
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Returns the size the given character would take up when rendered.
        /// </summary>
        /// <param name="Char">The character to find the size of.</param>
        /// <param name="DrawOptions">Additional options for drawing the character.</param>
        public override Size TextSize(char Char, DrawOptions DrawOptions = 0)
        {
            return Font.TextSize(Char, DrawOptions);
        }
        /// <summary>
        /// Returns the size the given string would take up when rendered.
        /// </summary>
        /// <param name="Text">The string to find the size of.</param>
        /// <param name="DrawOptions">Additional options for drawing the string.</param>
        public override Size TextSize(string Text, DrawOptions DrawOptions = 0)
        {
            return Font.TextSize(Text, DrawOptions);
        }

        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="X">The X position to draw the text at.</param>
        /// <param name="Y">The Y position to draw the text at.</param>
        /// <param name="c">The color of the text to draw.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public override void DrawText(string Text, int X, int Y, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            if (Locked) throw new BitmapLockedException();
            if (this.Font == null)
            {
                throw new Exception("No Font specified for this Bitmap.");
            }
            if (string.IsNullOrEmpty(Text)) return;
            IntPtr SDL_Font = this.Font.SDL_Font;
            bool aliased = (DrawOptions & DrawOptions.Aliased) == DrawOptions.Aliased;
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
            if (aliased) TextBitmap = new Bitmap(TTF_RenderText_Solid(SDL_Font, Text, c.SDL_Color));
            else TextBitmap = new Bitmap(TTF_RenderText_Blended(SDL_Font, Text, c.SDL_Color));
            if (centeralign) X -= TextBitmap.Width / 2;
            if (rightalign) X -= TextBitmap.Width;
            this.Build(new Rect(X, Y, TextBitmap.Width, TextBitmap.Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
            TextBitmap.Dispose();
            foreach (Bitmap b in this.InternalBitmaps) SDL_SetTextureBlendMode(b.Texture, SDL_BlendMode.SDL_BLENDMODE_ADD);
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="X">The X position to draw the text at.</param>
        /// <param name="Y">The Y position to draw the text at.</param>
        /// <param name="Width">The width within which to draw the text.</param>
        /// <param name="Height">The height within which to draw the text.</param>
        /// <param name="c">The color of the text to draw.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public override void DrawText(string Text, int X, int Y, int Width, int Height, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            if (Locked) throw new BitmapLockedException();
            if (this.Font == null)
            {
                throw new Exception("No Font specified for this Bitmap.");
            }
            if (string.IsNullOrEmpty(Text)) return;
            IntPtr SDL_Font = this.Font.SDL_Font;
            bool aliased = (DrawOptions & DrawOptions.Aliased) == DrawOptions.Aliased;
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
            if (aliased) TextBitmap = new Bitmap(TTF_RenderText_Solid(SDL_Font, Text, c.SDL_Color));
            else TextBitmap = new Bitmap(TTF_RenderText_Blended(SDL_Font, Text, c.SDL_Color));
            if (centeralign) X -= TextBitmap.Width / 2;
            if (rightalign) X -= TextBitmap.Width;
            this.Build(new Rect(X, Y, Width, Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
            TextBitmap.Dispose();
            foreach (Bitmap b in this.InternalBitmaps) SDL_SetTextureBlendMode(b.Texture, SDL_BlendMode.SDL_BLENDMODE_ADD);
            if (this.Renderer != null) this.Renderer.Update();
        }

        public override void DrawGlyph(char c, int X, int Y, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            if (Locked) throw new BitmapLockedException();
            if (this.Font == null)
            {
                throw new Exception("No Font specified for this Bitmap.");
            }
            if (c == '\x00') return;
            IntPtr SDL_Font = this.Font.SDL_Font;
            bool aliased = (DrawOptions & DrawOptions.Aliased) == DrawOptions.Aliased;
            bool leftalign = (DrawOptions & DrawOptions.LeftAlign) == DrawOptions.LeftAlign;
            bool centeralign = (DrawOptions & DrawOptions.CenterAlign) == DrawOptions.CenterAlign;
            bool rightalign = (DrawOptions & DrawOptions.RightAlign) == DrawOptions.RightAlign;
            if (leftalign && centeralign || leftalign && rightalign || centeralign && rightalign)
            {
                throw new Exception("Multiple alignments specified in DrawGlyph DrawOptions - can only contain one alignment setting");
            }
            if (!leftalign && !centeralign && !rightalign) leftalign = true;
            TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
            Bitmap TextBitmap;
            if (aliased) TextBitmap = new Bitmap(TTF_RenderGlyph_Solid(SDL_Font, c, color.SDL_Color));
            else TextBitmap = new Bitmap(TTF_RenderGlyph_Blended(SDL_Font, c, color.SDL_Color));
            if (centeralign) X -= TextBitmap.Width / 2;
            if (rightalign) X -= TextBitmap.Width;
            this.Build(new Rect(X, Y, TextBitmap.Width, TextBitmap.Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
            TextBitmap.Dispose();
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Locks the bitmap and converts the surface to a texture. The bitmap can no longer be modified until unlocked.
        /// </summary>
        public override void Lock()
        {
            if (Locked) throw new BitmapLockedException();
            this.Locked = true;
            foreach (Bitmap b in this.InternalBitmaps)
            {
                if (!b.Locked && b.Renderer != null)
                {
                    b.Lock();
                }
            }
        }

        /// <summary>
        /// Unlocks the bitmap, allowing you to modify the bitmap until locked again.
        /// </summary>
        public override void Unlock()
        {
            if (!Locked) throw new Exception("Bitmap was already unlocked and cannot be unlocked again.");
            this.Locked = false;
        }

        /// <summary>
        /// Saves the current bitmap to a file as a PNG.
        /// </summary>
        /// <param name="filename">The filename to save the bitmap as.</param>
        public override void SaveToPNG(string filename)
        {
            throw new MethodNotSupportedException(this);
        }

        /// <summary>
        /// Converts the SDL_Surface to an SDL_Texture used when rendering.
        /// </summary>
        public override void RecreateTexture()
        {
            throw new MethodNotSupportedException(this);
        }
    }
}

