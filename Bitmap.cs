using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_ttf;

namespace odl
{
    public class Bitmap : IDisposable
    {
        public static BlendMode DefaultBlendMode = BlendMode.Blend;

        /// <summary>
        /// The pointer to the SDL_Surface.
        /// </summary>
        public IntPtr Surface { get; protected set; } = IntPtr.Zero;
        /// <summary>
        /// The SDL_Surface object.
        /// </summary>
        public SDL_Surface SurfaceObject { get; protected set; }
        /// <summary>
        /// The pointer to the SDL_Texture.
        /// </summary>
        public IntPtr Texture { get; protected set; } = IntPtr.Zero;
        /// <summary>
        /// The width of the bitmap.
        /// </summary>
        public virtual int Width { get { return this.SurfaceObject.w; } protected set { } }
        /// <summary>
        /// The height of the bitmap.
        /// </summary>
        public virtual int Height { get { return this.SurfaceObject.h; } protected set { } }
        /// <summary>
        /// Whether or not the bitmap has been disposed.
        /// </summary>
        public bool Disposed { get; protected set; }
        /// <summary>
        /// The Renderer object associated with the bitmap.
        /// </summary>
        public virtual Renderer Renderer { get; set; }
        /// <summary>
        /// The Font object associated with the bitmap.
        /// </summary>
        public virtual Font Font { get; set; }
        /// <summary>
        /// Whether the bitmap can be written on.
        /// </summary>
        public bool Locked { get; protected set; }

        public int InternalX = 0;
        public int InternalY = 0;

        /// <summary>
        /// Creates a new bitmap with the given size.
        /// </summary>
        /// <param name="Size">The size of the new bitmap.</param>
        public Bitmap(Size Size)
            : this(Size.Width, Size.Height) { }
        /// <summary>
        /// Creates a Bitmap from an RGBA byte list in memory.
        /// </summary>
        /// <param name="Pixels">List of RGBA bytes representing pixels.</param>
        /// <param name="Width">The width of the bitmap.</param>
        /// <param name="Height">The height of the bitmap.</param>
        public Bitmap(List<byte> Pixels, int Width, int Height)
            : this(Pixels.ToArray(), Width, Height) { }
        /// <summary>
        /// Creates a Bitmap from a Color array in memory.
        /// </summary>
        /// <param name="Pixels">The array of colors representing pixels.</param>
        /// <param name="Width">The width of the bitmap.</param>
        /// <param name="Height">The height of the bitmap.</param>
        public Bitmap(Color[] Pixels, int Width, int Height)
            : this(Pixels.ToList(), Width, Height) { }

        /// <summary>
        /// Loads the specified file into a bitmap.
        /// </summary>
        /// <param name="Filename">The file to load into a bitmap.</param>
        public Bitmap(string Filename)
        {
            while (Filename.Contains('\\')) Filename = Filename.Replace('\\', '/');
            if (!File.Exists(Filename))
            {
                if (File.Exists(Filename + ".png")) Filename += ".png";
                else throw new FileNotFoundException($"File could not be found -- {Filename}");
            }

            Size ImageSize = ValidatePNG(Filename);
            if (ImageSize.Width > Graphics.MaxTextureSize.Width || ImageSize.Height > Graphics.MaxTextureSize.Height)
                throw new Exception($"The given file exceeds the texture size limit of {Graphics.MaxTextureSize}: '{Filename}'");

            this.Surface = SDL2.SDL_image.IMG_Load(Filename);
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
            this.Lock();
        }

        protected Bitmap() { }

        /// <summary>
        /// Creates a new bitmap with the given size.
        /// </summary>
        /// <param name="Width">The width of the new bitmap.</param>
        /// <param name="Height">The height of the new bitmap.</param>
        public Bitmap(int Width, int Height)
        {
            if (Width < 1 || Height < 1)
            {
                throw new Exception($"Invalid Bitmap size ({Width},{Height}) -- must be at least (1,1)");
            }
            if (Width > Graphics.MaxTextureSize.Width || Height > Graphics.MaxTextureSize.Height)
            {
                throw new Exception($"Bitmap ({Width},{Height}) exceeded maximum possible texture size ({Graphics.MaxTextureSize.Width},{Graphics.MaxTextureSize.Height})");
            }
            this.Surface = SDL_CreateRGBSurface(0, Width, Height, 32, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
            if (!(this is SolidBitmap)) this.Lock();
        }

        /// <summary>
        /// Creates a bitmap object to wrap around an existing SDL_Surface.
        /// </summary>
        /// <param name="Surface"></param>
        public Bitmap(IntPtr Surface)
        {
            this.Surface = Surface;
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
            this.Lock();
        }

        /// <summary>
        /// Creates a Bitmap from an RGBA byte array in memory.
        /// </summary>
        /// <param name="Pixels">Array of RGBA bytes representing pixels.</param>
        /// <param name="Width">The width of the bitmap.</param>
        /// <param name="Height">The height of the bitmap.</param>
        public Bitmap(byte[] Pixels, int Width, int Height)
        {
            IntPtr pixelptr = Marshal.AllocHGlobal(Pixels.Length);
            Marshal.Copy(Pixels, 0, pixelptr, Pixels.Length);
            this.Surface = SDL_CreateRGBSurfaceFrom(pixelptr, Width, Height, 32, Width * 4, 0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000);
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
            this.Lock();
            Marshal.FreeHGlobal(pixelptr);
        }

        /// <summary>
        /// Creates a Bitmap from a Color list in memory.
        /// </summary>
        /// <param name="Pixels">The list of colors representing pixels.</param>
        /// <param name="Width">The width of the bitmap.</param>
        /// <param name="Height">The height of the bitmap.</param>
        public Bitmap(List<Color> Pixels, int Width, int Height)
        {
            byte[] BytePixels = new byte[Pixels.Count * 4];
            for (int i = 0; i < Pixels.Count; i++)
            {
                BytePixels[i * 4] = Pixels[i].Red;
                BytePixels[i * 4 + 1] = Pixels[i].Green;
                BytePixels[i * 4 + 2] = Pixels[i].Blue;
                BytePixels[i * 4 + 3] = Pixels[i].Alpha;
            }
            IntPtr pixelptr = Marshal.AllocHGlobal(BytePixels.Length);
            Marshal.Copy(BytePixels, 0, pixelptr, BytePixels.Length);
            this.Surface = SDL_CreateRGBSurfaceFrom(pixelptr, Width, Height, 32, Width * 4, 0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000);
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
            this.Lock();
            Marshal.FreeHGlobal(pixelptr);
        }

        public Size ValidatePNG(string Filename)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(Filename));
            byte[] pngsignature = new byte[8] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            for (int i = 0; i < 8; i++)
            {
                if (br.ReadByte() != pngsignature[i])
                    throw new Exception($"The given file is not a valid PNG file: '{Filename}'");
            }

            br.BaseStream.Position = 16;
            byte[] widthbytes = new byte[sizeof(int)];
            for (int i = 0; i < sizeof(int); i++) widthbytes[sizeof(int) - 1 - i] = br.ReadByte();
            int width = BitConverter.ToInt32(widthbytes, 0);
            byte[] heightbytes = new byte[sizeof(int)];
            for (int i = 0; i < sizeof(int); i++) heightbytes[sizeof(int) - 1 - i] = br.ReadByte();
            int height = BitConverter.ToInt32(heightbytes, 0);
            return new Size(width, height);
        }

        ~Bitmap()
        {
            if (!Disposed || this.Surface != IntPtr.Zero || this.Texture != IntPtr.Zero)
            {
                Console.WriteLine($"An undisposed bitmap is being collected by the GC! This is a memory leak!\n    Bitmap info: Size ({Width},{Height})");
            }
        }

        /// <summary>
        /// Disposes and destroys the bitmap.
        /// </summary>
        public virtual void Dispose()
        {
            if (Disposed) return;
            if (this.Surface != IntPtr.Zero && this.Surface != null)
            {
                SDL_FreeSurface(this.Surface);
                SDL_DestroyTexture(this.Texture);
            }
            ColorToneBmp?.Dispose();
            ColorToneBmp = null;
            this.Surface = IntPtr.Zero;
            this.Texture = IntPtr.Zero;
            this.SurfaceObject = new SDL_Surface();
            this.Disposed = true;
            if (this.Renderer != null) this.Renderer.Update();
        }

        public override string ToString()
        {
            return $"(Bitmap: {this.Width},{this.Height})";
        }

        /// <summary>
        /// Clears the bitmap content.
        /// </summary>
        public virtual void Clear()
        {
            // Filling an alpha rectangle is faster than recreating the bitmap.
            FillRect(0, 0, this.Width, this.Height, Color.ALPHA);
        }

        /// <summary>
        /// Creates a clone of the bitmap.
        /// </summary>
        public virtual Bitmap Clone()
        {
            Bitmap bmp = new Bitmap(Width, Height);
            bmp.Unlock();
            bmp.Build(this);
            bmp.Lock();
            bmp.Font = this.Font;
            bmp.Renderer = this.Renderer;
            return bmp;
        }

        #region SetPixel Overloads
        /// <summary>
        /// Sets a pixel in the bitmap to the specified color.
        /// </summary>
        /// <param name="p">The position in the bitmap.</param>
        /// <param name="c">The color to set the pixel to.</param>
        public void SetPixel(Point p, Color c)
        {
            SetPixel(p.X, p.Y, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Sets a pixel in the bitmap to the specified color.
        /// </summary>
        /// <param name="X">The X position in the bitmap.</param>
        /// <param name="Y">The Y position in the bitmap.</param>
        /// <param name="c">The color to set the pixel to.</param>
        public void SetPixel(int X, int Y, Color c)
        {
            SetPixel(X, Y, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Sets a pixel in the bitmap to the specified color.
        /// </summary>
        /// <param name="p">The position in the bitmap.</param>
        /// <param name="r">The Red component of the color to set the pixel to.</param>
        /// <param name="g">The Green component of the color to set the pixel to.</param>
        /// <param name="b">The Blue component of the color to set the pixel to.</param>
        /// <param name="a">The Alpha component of the color to set the pixel to.</param>
        public void SetPixel(Point p, byte r, byte g, byte b, byte a = 255)
        {
            SetPixel(p.X, p.Y, r, g, b, a);
        }
        #endregion
        /// <summary>
        /// Sets a pixel in the bitmap to the specified color.
        /// </summary>
        /// <param name="X">The X position in the bitmap.</param>
        /// <param name="Y">The Y position in the bitmap.</param>
        /// <param name="r">The Red component of the color to set the pixel to.</param>
        /// <param name="g">The Green component of the color to set the pixel to.</param>
        /// <param name="b">The Blue component of the color to set the pixel to.</param>
        /// <param name="a">The Alpha component of the color to set the pixel to.</param>
        public virtual void SetPixel(int X, int Y, byte r, byte g, byte b, byte a = 255)
        {
            if (Locked) throw new BitmapLockedException();
            if (X < 0 || Y < 0)
            {
                throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- minimum is (0,0)");
            }
            if (X >= this.Width || Y >= this.Height)
            {
                throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
            }
            int Offset = this.Width * 4 * Y + 4 * X;
            bool e = BitConverter.IsLittleEndian;
            Marshal.WriteByte(SurfaceObject.pixels, Offset, e ? r : b);
            Marshal.WriteByte(SurfaceObject.pixels, Offset + 1, g);
            Marshal.WriteByte(SurfaceObject.pixels, Offset + 2, e ? b : r);
            Marshal.WriteByte(SurfaceObject.pixels, Offset + 3, a);
            if (this.Renderer != null) this.Renderer.Update();
        }

        #region GetPixel Overloads
        /// <summary>
        /// Returns the color at the given position.
        /// </summary>
        /// <param name="p">The position in the bitmap.</param>
        public Color GetPixel(Point p)
        {
            return GetPixel(p.X, p.Y);
        }
        #endregion
        /// <summary>
        /// Returns the color at the given position.
        /// </summary>
        /// <param name="X">The X position in the bitmap.</param>
        /// <param name="Y">The Y position in the bitmap.</param>
        public virtual Color GetPixel(int X, int Y)
        {
            if (X < 0 || Y < 0)
            {
                throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- minimum is (0,0)");
            }
            if (X >= this.Width || Y >= this.Height)
            {
                throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
            }
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
        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="p1">The position of the first point.</param>
        /// <param name="p2">The position of the second point.</param>
        /// <param name="c">The color to draw the line with.</param>
        public void DrawLine(Point p1, Point p2, Color c)
        {
            DrawLine(p1.X, p1.Y, p2.X, p2.Y, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="p1">The position of the first point.</param>
        /// <param name="p2">The position of the second point.</param>
        /// <param name="r">The Red component of the color to draw the line with.</param>
        /// <param name="g">The Green component of the color to draw the line with.</param>
        /// <param name="b">The Blue component of the color to draw the line with.</param>
        /// <param name="a">The Alpha component of the color to draw the line with.</param>
        public void DrawLine(Point p1, Point p2, byte r, byte g, byte b, byte a = 255)
        {
            DrawLine(p1.X, p1.Y, p2.X, p2.Y, r, g, b, a);
        }
        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="p1">The position of the first point.</param>
        /// <param name="x2">The X position of the second point.</param>
        /// <param name="y2">The Y position of the second point.</param>
        /// <param name="c">The color to draw the line with.</param>
        public void DrawLine(Point p1, int x2, int y2, Color c)
        {
            DrawLine(p1.X, p1.Y, x2, y2, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="p1">The position of the first point.</param>
        /// <param name="x2">The X position of the second point.</param>
        /// <param name="y2">The Y position of the second point.</param>
        /// <param name="r">The Red component of the color to draw the line with.</param>
        /// <param name="g">The Green component of the color to draw the line with.</param>
        /// <param name="b">The Blue component of the color to draw the line with.</param>
        /// <param name="a">The Alpha component of the color to draw the line with.</param>
        public void DrawLine(Point p1, int x2, int y2, byte r, byte g, byte b, byte a = 255)
        {
            DrawLine(p1.X, p1.Y, x2, y2, r, g, b, a);
        }
        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="x1">The X position of the first point.</param>
        /// <param name="y1">The Y position of the first point.</param>
        /// <param name="p2">The position of the second point.</param>
        /// <param name="c">The color to draw the line with.</param>
        public void DrawLine(int x1, int y1, Point p2, Color c)
        {
            DrawLine(x1, y1, p2.X, p2.Y, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="x1">The X position of the first point.</param>
        /// <param name="y1">The Y position of the first point.</param>
        /// <param name="p2">The position of the second point.</param>
        /// <param name="r">The Red component of the color to draw the line with.</param>
        /// <param name="g">The Green component of the color to draw the line with.</param>
        /// <param name="b">The Blue component of the color to draw the line with.</param>
        /// <param name="a">The Alpha component of the color to draw the line with.</param>
        public void DrawLine(int x1, int y1, Point p2, byte r, byte g, byte b, byte a = 255)
        {
            DrawLine(x1, y1, p2.X, p2.Y, r, g, b, a);
        }
        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="x1">The X position of the first point.</param>
        /// <param name="y1">The Y position of the first point.</param>
        /// <param name="x2">The X position of the second point.</param>
        /// <param name="y2">The Y position of the second point.</param>
        /// <param name="c">The color to draw the line with.</param>
        public void DrawLine(int x1, int y1, int x2, int y2, Color c)
        {
            DrawLine(x1, y1, x2, y2, c.Red, c.Green, c.Blue, c.Alpha);
        }
        #endregion
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
        public virtual void DrawLine(int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a = 255)
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

        #region DrawLines Overloads
        /// <summary>
        /// Draws lines between the given points.
        /// </summary>
        /// <param name="c">The color to draw the lines with.</param>
        /// <param name="points">The list of points to draw the lines between.</param>
        public void DrawLines(Color c, params Point[] points)
        {
            this.DrawLines(c.Red, c.Green, c.Blue, c.Alpha, points);
        }
        /// <summary>
        /// Draws lines between the given points.
        /// </summary>
        /// <param name="r">The Red component of the color to draw the lines with.</param>
        /// <param name="g">The Green component of the color to draw the lines with.</param>
        /// <param name="b">The Blue component of the color to draw the lines with.</param>
        /// <param name="points">The list of points to draw the lines between.</param>
        public void DrawLines(byte r, byte g, byte b, params Point[] points)
        {
            this.DrawLines(r, g, b, 255, points);
        }
        #endregion
        /// <summary>
        /// Draws lines between the given points.
        /// </summary>
        /// <param name="r">The Red component of the color to draw the lines with.</param>
        /// <param name="g">The Green component of the color to draw the lines with.</param>
        /// <param name="b">The Blue component of the color to draw the lines with.</param>
        /// <param name="a">The Alpha component of the color to draw the lines with.</param>
        /// <param name="points">The list of points to draw the lines between.</param>
        public virtual void DrawLines(byte r, byte g, byte b, byte a, params Point[] points)
        {
            if (Locked) throw new BitmapLockedException();
            for (int i = 0; i < points.Length - 1; i++)
            {
                this.DrawLine(points[i], points[i + 1], r, g, b, a);
            }
        }

        #region DrawCircle
        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="c">The origin position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="color">The color to draw the circle with.</param>
        public void DrawCircle(Point c, int Radius, Color color)
        {
            DrawCircle(c.X, c.Y, Radius, color.Red, color.Green, color.Blue, color.Alpha);
        }
        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="ox">The origin X position of the circle.</param>
        /// <param name="oy">The origin Y position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="c">The color to draw the circle with.</param>
        public void DrawCircle(int ox, int oy, int Radius, Color c)
        {
            DrawCircle(ox, oy, Radius, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="c">The origin position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="r">The Red component of the color to draw the circle with.</param>
        /// <param name="g">The Green component of the color to draw the circle with.</param>
        /// <param name="b">The Blue component of the color to draw the circle with.</param>
        /// <param name="a">The Alpha component of the color to draw the circle with.</param>
        public void DrawCircle(Point c, int Radius, byte r, byte g, byte b, byte a = 255)
        {
            DrawCircle(c.X, c.Y, Radius, r, g, b, a);
        }
        #endregion
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
        public virtual void DrawCircle(int ox, int oy, int Radius, byte r, byte g, byte b, byte a = 255)
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

        #region FillCircle
        /// <summary>
        /// Fills a circle.
        /// </summary>
        /// <param name="c">The origin position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="color">The color to fill the circle with.</param>
        public void FillCircle(Point c, int Radius, Color color)
        {
            FillCircle(c.X, c.Y, Radius, color.Red, color.Green, color.Blue, color.Alpha);
        }
        /// <summary>
        /// Fills a circle.
        /// </summary>
        /// <param name="ox">The origin X position of the circle.</param>
        /// <param name="oy">The origin Y position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="c">The color to fill the circle with.</param>
        public void FillCircle(int ox, int oy, int Radius, Color c)
        {
            FillCircle(ox, oy, Radius, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Fills a circle.
        /// </summary>
        /// <param name="c">The origin position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="r">The Red component of the color to fill the circle with.</param>
        /// <param name="g">The Green component of the color to fill the circle with.</param>
        /// <param name="b">The Blue component of the color to fill the circle with.</param>
        /// <param name="a">The Alpha component of the color to fill the circle with.</param>
        public void FillCircle(Point c, int Radius, byte r, byte g, byte b, byte a = 255)
        {
            FillCircle(c.X, c.Y, Radius, r, g, b, a);
        }
        #endregion
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
        public virtual void FillCircle(int ox, int oy, int Radius, byte r, byte g, byte b, byte a = 255)
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

        #region DrawQuadrant Overloads
        /// <summary>
        /// Draws a quarter of a circle.
        /// </summary>
        /// <param name="c">The origin position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="l">The part of the circle to draw.</param>
        /// <param name="color">The color to draw the quadrant with.</param>
        public void DrawQuadrant(Point c, int Radius, Location l, Color color)
        {
            DrawQuadrant(c.X, c.Y, Radius, l, color.Red, color.Green, color.Blue, color.Alpha);
        }
        /// <summary>
        /// Draws a quarter of a circle.
        /// </summary>
        /// <param name="ox">The origin X position of the circle.</param>
        /// <param name="oy">The origin Y position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="l">The part of the circle to draw.</param>
        /// <param name="c">The color to draw the quadrant with.</param>
        public void DrawQuadrant(int ox, int oy, int Radius, Location l, Color c)
        {
            DrawQuadrant(ox, oy, Radius, l, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Draws a quarter of a circle.
        /// </summary>
        /// <param name="c">The origin position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="l">The part of the circle to draw.</param>
        /// <param name="r">The Red component of the color to draw the quadrant with.</param>
        /// <param name="g">The Green component of the color to draw the quadrant with.</param>
        /// <param name="b">The Blue component of the color to draw the quadrant with.</param>
        /// <param name="a">The Alpha component of the color to draw the quadrant with.</param>
        public void DrawQuadrant(Point c, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
        {
            DrawQuadrant(c.X, c.Y, Radius, l, r, g, b, a);
        }
        #endregion
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
        public virtual void DrawQuadrant(int ox, int oy, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
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

        #region FillQuadrant Overloads
        /// <summary>
        /// Fills a quarter of a circle.
        /// </summary>
        /// <param name="c">The origin position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="l">The part of the circle to draw.</param>
        /// <param name="color">The color to fill the quadrant with.</param>
        public void FillQuadrant(Point c, int Radius, Location l, Color color)
        {
            FillQuadrant(c.X, c.Y, Radius, l, color.Red, color.Green, color.Blue, color.Alpha);
        }
        /// <summary>
        /// Fills a quarter of a circle.
        /// </summary>
        /// <param name="ox">The origin X position of the circle.</param>
        /// <param name="oy">The origin Y position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="l">The part of the circle to draw.</param>
        /// <param name="c">The color to fill the quadrant with.</param>
        public void FillQuadrant(int ox, int oy, int Radius, Location l, Color c)
        {
            FillQuadrant(ox, oy, Radius, l, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Fills a quarter of a circle.
        /// </summary>
        /// <param name="c">The origin position of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        /// <param name="l">The part of the circle to draw.</param>
        /// <param name="r">The Red component of the color to fill the quadrant with.</param>
        /// <param name="g">The Green component of the color to fill the quadrant with.</param>
        /// <param name="b">The Blue component of the color to fill the quadrant with.</param>
        /// <param name="a">The Alpha component of the color to fill the quadrant with.</param>
        public void FillQuadrant(Point c, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
        {
            FillQuadrant(c.X, c.Y, Radius, l, r, g, b, a);
        }
        #endregion
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
        public virtual void FillQuadrant(int ox, int oy, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
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

        #region DrawRect Overloads
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="r">The rectangle to draw.</param>
        /// <param name="c">The color to draw the rectangle with.</param>
        public void DrawRect(Rect r, Color c)
        {
            DrawRect(r.X, r.Y, r.Width, r.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="rect">The rectangle to draw.</param>
        /// <param name="r">The Red component of the color to draw the rectangle with.</param>
        /// <param name="g">The Green component of the color to draw the rectangle with.</param>
        /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
        public void DrawRect(Rect rect, byte r, byte g, byte b, byte a = 255)
        {
            DrawRect(rect.X, rect.Y, rect.Width, rect.Height, r, g, b, a);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="Point">The position of the rectangle.</param>
        /// <param name="Size">The size of the rectangle.</param>
        /// <param name="c">The color to draw the rectangle with.</param>
        public void DrawRect(Point Point, Size Size, Color c)
        {
            DrawRect(Point.X, Point.Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="Point">The position of the rectangle.</param>
        /// <param name="Size">The size of the rectangle.</param>
        /// <param name="r">The Red component of the color to draw the rectangle with.</param>
        /// <param name="g">The Green component of the color to draw the rectangle with.</param>
        /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
        public void DrawRect(Point Point, Size Size, byte r, byte g, byte b, byte a = 255)
        {
            DrawRect(Point.X, Point.Y, Size.Width, Size.Height, r, g, b, a);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="Point">The position of the rectangle.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="c">The color to draw the rectangle with.</param>
        public void DrawRect(Point Point, int Width, int Height, Color c)
        {
            DrawRect(Point.X, Point.Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="Point">The position of the rectangle.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="r">The Red component of the color to draw the rectangle with.</param>
        /// <param name="g">The Green component of the color to draw the rectangle with.</param>
        /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
        public void DrawRect(Point Point, int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            DrawRect(Point.X, Point.Y, Width, Height, r, g, b, a);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="size">The size of the rectangle.</param>
        /// <param name="r">The Red component of the color to draw the rectangle with.</param>
        /// <param name="g">The Green component of the color to draw the rectangle with.</param>
        /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
        public void DrawRect(Size size, byte r, byte g, byte b, byte a = 255)
        {
            this.DrawRect(0, 0, size, r, g, b, a);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="size">The size of the rectangle.</param>
        /// <param name="c">The color to draw the rectangle with.</param>
        public void DrawRect(Size size, Color c)
        {
            this.DrawRect(0, 0, size, c);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="r">The Red component of the color to draw the rectangle with.</param>
        /// <param name="g">The Green component of the color to draw the rectangle with.</param>
        /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
        public void DrawRect(int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            this.DrawRect(0, 0, Width, Height, r, g, b, a);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="c">The color to draw the rectangle with.</param>
        public void DrawRect(int Width, int Height, Color c)
        {
            this.DrawRect(0, 0, Width, Height, c);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="X">The X position of the rectangle.</param>
        /// <param name="Y">The Y position of the rectangle.</param>
        /// <param name="Size">The size of the rectangle.</param>
        /// <param name="c">The color of the rectangle to draw.</param>
        public void DrawRect(int X, int Y, Size Size, Color c)
        {
            DrawRect(X, Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="X">The X position of the rectangle.</param>
        /// <param name="Y">The Y position of the rectangle.</param>
        /// <param name="Size">The size of the rectangle.</param>
        /// <param name="r">The Red component of the color to draw the rectangle with.</param>
        /// <param name="g">The Green component of the color to draw the rectangle with.</param>
        /// <param name="b">The Blue component of the color to draw the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to draw the rectangle with.</param>
        public void DrawRect(int X, int Y, Size Size, byte r, byte g, byte b, byte a = 255)
        {
            DrawRect(X, Y, Size.Width, Size.Height, r, g, b, a);
        }
        /// <summary>
        /// Draws a rectangle with a solid color.
        /// </summary>
        /// <param name="X">The X position of the rectangle.</param>
        /// <param name="Y">The Y position of the rectangle.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="c">The color to draw the rectangle with.</param>
        public void DrawRect(int X, int Y, int Width, int Height, Color c)
        {
            DrawRect(X, Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        #endregion
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
        public virtual void DrawRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            if (Locked) throw new BitmapLockedException();
            if (X < 0 || Y < 0)
            {
                throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- minimum is (0,0)");
            }
            if (X + Width > this.Width || Y + Height > this.Height)
            {
                throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
            }
            DrawLine(X, Y, X + Width - 1, Y, r, g, b, a);
            DrawLine(X, Y, X, Y + Height - 1, r, g, b, a);
            DrawLine(X, Y + Height - 1, X + Width - 1, Y + Height - 1, r, g, b, a);
            DrawLine(X + Width - 1, Y, X + Width - 1, Y + Height - 1, r, g, b, a);
            if (this.Renderer != null) this.Renderer.Update();
        }

        #region FillRect Overloads
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="r">The rectangle to fill.</param>
        /// <param name="c">The color to fill the rectangle with.</param>
        public void FillRect(Rect r, Color c)
        {
            FillRect(r.X, r.Y, r.Width, r.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="rect">The rectangle to fill.</param>
        /// <param name="r">The Red component of the color to fill the rectangle with.</param>
        /// <param name="g">The Green component of the color to fill the rectangle with.</param>
        /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
        public void FillRect(Rect rect, byte r, byte g, byte b, byte a = 255)
        {
            FillRect(rect.X, rect.Y, rect.Width, rect.Height, r, g, b, a);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="Point">The position of the rectangle.</param>
        /// <param name="Size">The size of the rectangle.</param>
        /// <param name="c">The color to fill the rectangle with.</param>
        public void FillRect(Point Point, Size Size, Color c)
        {
            FillRect(Point.X, Point.Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="Point">The position of the rectangle.</param>
        /// <param name="Size">The size of the rectangle.</param>
        /// <param name="r">The Red component of the color to fill the rectangle with.</param>
        /// <param name="g">The Green component of the color to fill the rectangle with.</param>
        /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
        public void FillRect(Point Point, Size Size, byte r, byte g, byte b, byte a = 255)
        {
            FillRect(Point.X, Point.Y, Size.Width, Size.Height, r, g, b, a);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="Point">The position of the rectangle.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="c">The color to fill the rectangle with.</param>
        public void FillRect(Point Point, int Width, int Height, Color c)
        {
            FillRect(Point.X, Point.Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="Point">The position of the rectangle.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="r">The Red component of the color to fill the rectangle with.</param>
        /// <param name="g">The Green component of the color to fill the rectangle with.</param>
        /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
        public void FillRect(Point Point, int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            FillRect(Point.X, Point.Y, Width, Height, r, g, b, a);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="size">The size of the rectangle.</param>
        /// <param name="r">The Red component of the color to fill the rectangle with.</param>
        /// <param name="g">The Green component of the color to fill the rectangle with.</param>
        /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
        public void FillRect(Size size, byte r, byte g, byte b, byte a = 255)
        {
            this.FillRect(0, 0, size, r, g, b, a);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="size">The size of the rectangle.</param>
        /// <param name="c">The color to fill the rectangle with.</param>
        public void FillRect(Size size, Color c)
        {
            this.FillRect(0, 0, size, c);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="r">The Red component of the color to fill the rectangle with.</param>
        /// <param name="g">The Green component of the color to fill the rectangle with.</param>
        /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
        public void FillRect(int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            this.FillRect(0, 0, Width, Height, r, g, b, a);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="c">The color to fill the rectangle with.</param>
        public void FillRect(int Width, int Height, Color c)
        {
            this.FillRect(0, 0, Width, Height, c);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="X">The X position of the rectangle.</param>
        /// <param name="Y">The Y position of the rectangle.</param>
        /// <param name="Size">The size of the rectangle.</param>
        /// <param name="c">The color to fill the rectangle with.</param>
        public void FillRect(int X, int Y, Size Size, Color c)
        {
            FillRect(X, Y, Size.Width, Size.Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="X">The X position of the rectangle.</param>
        /// <param name="Y">The Y position of the rectangle.</param>
        /// <param name="Size">The size of the rectangle.</param>
        /// <param name="r">The Red component of the color to fill the rectangle with.</param>
        /// <param name="g">The Green component of the color to fill the rectangle with.</param>
        /// <param name="b">The Blue component of the color to fill the rectangle with.</param>
        /// <param name="a">The Alpha component of the color to fill the rectangle with.</param>
        public void FillRect(int X, int Y, Size Size, byte r, byte g, byte b, byte a = 255)
        {
            FillRect(X, Y, Size.Width, Size.Height, r, g, b, a);
        }
        /// <summary>
        /// Fills a rectangle with a solid color.
        /// </summary>
        /// <param name="X">The X position of the rectangle.</param>
        /// <param name="Y">The Y position of the rectangle.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        /// <param name="c">The color to fill the rectangle with.</param>
        public void FillRect(int X, int Y, int Width, int Height, Color c)
        {
            FillRect(X, Y, Width, Height, c.Red, c.Green, c.Blue, c.Alpha);
        }
        #endregion
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
        public virtual void FillRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255)
        {
            if (Locked) throw new BitmapLockedException();
            if (X < 0 || Y < 0)
            {
                throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- minimum is (0,0)");
            }
            if (X >= this.Width || Y >= this.Height)
            {
                throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
            }
            if (X + Width - 1 >= this.Width || Y + Height - 1 >= this.Height)
            {
                throw new Exception($"Invalid rectangle ({X},{Y},{Width},{Height}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
            }
            SDL_Rect Rect = new Rect(X, Y, Width, Height).SDL_Rect;
            SDL_FillRect(this.Surface, ref Rect, SDL_MapRGBA(this.SurfaceObject.format, r, g, b, a));
            if (this.Renderer != null) this.Renderer.Update();
        }

        #region Build Overloads
        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="DestRect">The available rectangle in the destination bitmap to draw the source bitmap in.</param>
        /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
        /// <param name="SX">The X position of the rectangle of the source bitmap to use for drawing.</param>
        /// <param name="SY">The Y position of the rectangle of the source bitmap to use for drawing.</param>
        /// <param name="SWidth">The width of the rectangle of the source bitmap to use for drawing.</param>
        /// <param name="SHeight">The height of the rectangle of the source bitmap to use for drawing.</param>
        public void Build(Rect DestRect, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(DestRect, SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="DestRect">The available rectangle in the destination bitmap to draw the source bitmap in.</param>
        /// <param name="SrcBitmap">The bitmap to be drawn.</param>
        public void Build(Rect DestRect, Bitmap SrcBitmap)
        {
            this.Build(DestRect, SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height));
        }
        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="DP">The position in the destination bitmap to draw the source bitmap at.</param>
        /// <param name="DS">The size available for the source bitmap to be drawn.</param>
        /// <param name="DestRect">The bitmap to be drawn on.</param>
        /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
        /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
        public void Build(Point DP, Size DS, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DP, DS), SrcBitmap, SrcRect);
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
        public void Build(Point DP, Size DS, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DP, DS), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="DP">The position in the destination bitmap to draw the source bitmap at.</param>
        /// <param name="DWidth">The width available for the source bitmap to be drawn.</param>
        /// <param name="DHeight">The height available for the source bitmap to be drawn.</param>
        /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
        /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
        public void Build(Point DP, int DWidth, int DHeight, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DP, DWidth, DHeight), SrcBitmap, SrcRect);
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
        public void Build(Point DP, int DWidth, int DHeight, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DP, DWidth, DHeight), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="DX">The X position in the destination bitmap to draw the source bitmap at.</param>
        /// <param name="DY">The Y position in the destination bitmap to draw the source bitmap at.</param>
        /// <param name="DS">The size available for the source bitmap to be drawn.</param>
        /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
        /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
        public void Build(int DX, int DY, Size DS, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DX, DY, DS), SrcBitmap, SrcRect);
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
        public void Build(int DX, int DY, Size DS, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DX, DY, DS), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
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
        public void Build(int DX, int DY, int DWidth, int DHeight, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DX, DY, DWidth, DHeight), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
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
        public void Build(int DX, int DY, int DWidth, int DHeight, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DX, DY, DWidth, DHeight), SrcBitmap, SrcRect);
        }
        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="DP">The position in the destination bitmap to draw the source bitmap at.</param>
        /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
        /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
        public void Build(Point DP, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DP, SrcRect.Width, SrcRect.Height), SrcBitmap, SrcRect);
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
        public void Build(Point DP, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DP, SWidth, SHeight), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="DX">The X position in the destination bitmap to draw the source bitmap at.</param>
        /// <param name="DY">The Y position in the destination bitmap to draw the source bitmap at.</param>
        /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
        /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
        public void Build(int DX, int DY, Bitmap SrcBitmap, Rect SrcRect)
        {
            this.Build(new Rect(DX, DY, SrcRect.Width, SrcRect.Height), SrcBitmap, SrcRect);
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
        public void Build(int DX, int DY, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight)
        {
            this.Build(new Rect(DX, DY, SWidth, SHeight), SrcBitmap, new Rect(SX, SY, SWidth, SHeight));
        }
        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="DP">The position in the destination bitmap to draw the source bitmap at.</param>
        /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
        public void Build(Point DP, Bitmap SrcBitmap)
        {
            this.Build(new Rect(DP, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height));
        }
        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="DX">The X position in the destination bitmap to draw the source bitmap at.</param>
        /// <param name="DY">The Y position in the destination bitmap to draw the source bitmap at.</param>
        /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
        public void Build(int DX, int DY, Bitmap SrcBitmap)
        {
            this.Build(new Rect(DX, DY, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height));
        }
        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
        public void Build(Bitmap SrcBitmap)
        {
            this.Build(new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height), SrcBitmap, new Rect(0, 0, SrcBitmap.Width, SrcBitmap.Height));
        }
        #endregion
        /// <summary>
        /// Blits the Source bitmap on top of the Destination bitmap.
        /// </summary>
        /// <param name="DestRect">The available rectangle in the destination bitmap to draw the source bitmap in.</param>
        /// <param name="SrcBitmap">The bitmap to be overlayed.</param>
        /// <param name="SrcRect">The rectangle of the source bitmap to use for drawing.</param>
        public virtual void Build(Rect DestRect, Bitmap SrcBitmap, Rect SrcRect)
        {
            if (Locked) throw new BitmapLockedException();
            SDL_Rect Src = SrcRect.SDL_Rect;
            SDL_Rect Dest = DestRect.SDL_Rect;
            if (Dest.w != Src.w || Dest.h != Src.h)
                 SDL_BlitScaled (SrcBitmap.Surface, ref Src, this.Surface, ref Dest);
            else SDL_BlitSurface(SrcBitmap.Surface, ref Src, this.Surface, ref Dest);
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Returns the size the given character would take up when rendered.
        /// </summary>
        /// <param name="Char">The character to find the size of.</param>
        /// <param name="DrawOptions">Additional options for drawing the character.</param>
        public virtual Size TextSize(char Char, DrawOptions DrawOptions = 0)
        {
            return Font.TextSize(Char, DrawOptions);
        }
        /// <summary>
        /// Returns the size the given string would take up when rendered.
        /// </summary>
        /// <param name="Text">The string to find the size of.</param>
        /// <param name="DrawOptions">Additional options for drawing the string.</param>
        public virtual Size TextSize(string Text, DrawOptions DrawOptions = 0)
        {
            return Font.TextSize(Text, DrawOptions);
        }

        #region DrawText Overloads
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="X">The X position to draw the text at.</param>
        /// <param name="Y">The Y position to draw the text at.</param>
        /// <param name="R">The Red component of the color.</param>
        /// <param name="G">The Green component of the color.</param>
        /// <param name="B">The Blue component of the color.</param>
        /// <param name="A">The Alpha component of the color.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, int X, int Y, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawText(Text, X, Y, new Color(R, G, B, A), DrawOptions);
        }
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="p">The position to draw the text at.</param>
        /// <param name="c">The color of the text to draw.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, Point p, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawText(Text, p.X, p.Y, c, DrawOptions);
        }
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="p">The position to draw the text at.</param>
        /// <param name="R">The Red component of the color.</param>
        /// <param name="G">The Green component of the color.</param>
        /// <param name="B">The Blue component of the color.</param>
        /// <param name="A">The Alpha component of the color.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, Point p, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawText(Text, p.X, p.Y, new Color(R, G, B, A), DrawOptions);
        }
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="c">The color of the text to draw.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawText(Text, 0, 0, c, DrawOptions);
        }
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="R">The Red component of the color.</param>
        /// <param name="G">The Green component of the color.</param>
        /// <param name="B">The Blue component of the color.</param>
        /// <param name="A">The Alpha component of the color.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawText(Text, 0, 0, new Color(R, G, B, A), DrawOptions);
        }
        #endregion
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="X">The X position to draw the text at.</param>
        /// <param name="Y">The Y position to draw the text at.</param>
        /// <param name="c">The color of the text to draw.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public virtual void DrawText(string Text, int X, int Y, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
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
            if (aliased) TextBitmap = new Bitmap(TTF_RenderText_Solid  (SDL_Font, Text, c.SDL_Color));
            else         TextBitmap = new Bitmap(TTF_RenderText_Blended(SDL_Font, Text, c.SDL_Color));
            if (centeralign) X -= TextBitmap.Width / 2;
            if (rightalign)  X -= TextBitmap.Width;
            this.Build(new Rect(X, Y, TextBitmap.Width, TextBitmap.Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
            TextBitmap.Dispose();
            SDL_BlendMode blendmode = SDL_BlendMode.SDL_BLENDMODE_NONE;
            if (DefaultBlendMode == BlendMode.Addition) blendmode = SDL_BlendMode.SDL_BLENDMODE_ADD;
            else if (DefaultBlendMode == BlendMode.Blend) blendmode = SDL_BlendMode.SDL_BLENDMODE_BLEND;
            else if (DefaultBlendMode == BlendMode.Modulation) blendmode = SDL_BlendMode.SDL_BLENDMODE_MOD;
            SDL_SetTextureBlendMode(this.Texture, blendmode);
            if (this.Renderer != null) this.Renderer.Update();
        }

        #region DrawText + Size Overloads
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="r">The rectangle in which to draw the text.</param>
        /// <param name="c">The color of the text to draw.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, Rect r, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            DrawText(Text, r.X, r.Y, r.Width, r.Height, c, DrawOptions);
        }
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="r">The rectangle in which to draw the text.</param>
        /// <param name="R">The Red component of the color.</param>
        /// <param name="G">The Green component of the color.</param>
        /// <param name="B">The Blue component of the color.</param>
        /// <param name="A">The Alpha component of the color.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, Rect r, byte R, byte G, byte B, byte A, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            DrawText(Text, r.X, r.Y, r.Width, r.Height, new Color(R, G, B, A), DrawOptions);
        }
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="p">The position at which to draw the text.</param>
        /// <param name="s">The size within which to draw the text.</param>
        /// <param name="c">The color of the text to draw.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, Point p, Size s, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            DrawText(Text, p.X, p.Y, s.Width, s.Height, c, DrawOptions);
        }
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="p">The position at which to draw the text.</param>
        /// <param name="s">The size within which to draw the text.</param>
        /// <param name="R">The Red component of the color.</param>
        /// <param name="G">The Green component of the color.</param>
        /// <param name="B">The Blue component of the color.</param>
        /// <param name="A">The Alpha component of the color.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, Point p, Size s, byte R, byte G, byte B, byte A, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            DrawText(Text, p.X, p.Y, s.Width, s.Height, new Color(R, G, B, A), DrawOptions);
        }
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="X">The X position at which to draw the text.</param>
        /// <param name="Y">The Y position at which to draw the text.</param>
        /// <param name="s">The size within which to draw the text.</param>
        /// <param name="c">The color of the text to draw.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, int X, int Y, Size s, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            DrawText(Text, X, Y, s.Width, s.Height, c, DrawOptions);
        }
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="X">The X position at which to draw the text.</param>
        /// <param name="Y">The Y position at which to draw the text.</param>
        /// <param name="s">The size within which to draw the text.</param>
        /// <param name="R">The Red component of the color.</param>
        /// <param name="G">The Green component of the color.</param>
        /// <param name="B">The Blue component of the color.</param>
        /// <param name="A">The Alpha component of the color.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, int X, int Y, Size s, byte R, byte G, byte B, byte A, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            DrawText(Text, X, Y, s.Width, s.Height, new Color(R, G, B, A), DrawOptions);
        }
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="p">The position at which to draw the text.</param>
        /// <param name="Width">The width within which to draw the text.</param>
        /// <param name="Height">The height within which to draw the text.</param>
        /// <param name="c">The color of the text to draw.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, Point p, int Width, int Height, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            DrawText(Text, p.X, p.Y, Width, Height, c, DrawOptions);
        }
        /// <summary>
        /// Draws a string of text.
        /// </summary>
        /// <param name="Text">The text to draw.</param>
        /// <param name="p">The position at which to draw the text.</param>
        /// <param name="Width">The width within which to draw the text.</param>
        /// <param name="Height">The height within which to draw the text.</param>
        /// <param name="R">The Red component of the color.</param>
        /// <param name="G">The Green component of the color.</param>
        /// <param name="B">The Blue component of the color.</param>
        /// <param name="A">The Alpha component of the color.</param>
        /// <param name="DrawOptions">Additional options for drawing the text.</param>
        public void DrawText(string Text, Point p, int Width, int Height, byte R, byte G, byte B, byte A, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            DrawText(Text, p.X, p.Y, Width, Height, new Color(R, G, B, A), DrawOptions);
        }
        #endregion
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
        public virtual void DrawText(string Text, int X, int Y, int Width, int Height, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
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
            if (aliased) TextBitmap = new Bitmap(TTF_RenderText_Solid  (SDL_Font, Text, c.SDL_Color));
            else         TextBitmap = new Bitmap(TTF_RenderText_Blended(SDL_Font, Text, c.SDL_Color));
            if (centeralign) X -= TextBitmap.Width / 2;
            if (rightalign) X -= TextBitmap.Width;
            this.Build(new Rect(X, Y, Width, Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
            TextBitmap.Dispose();
            SDL_BlendMode blendmode = SDL_BlendMode.SDL_BLENDMODE_NONE;
            if (DefaultBlendMode == BlendMode.Addition) blendmode = SDL_BlendMode.SDL_BLENDMODE_ADD;
            else if (DefaultBlendMode == BlendMode.Blend) blendmode = SDL_BlendMode.SDL_BLENDMODE_BLEND;
            else if (DefaultBlendMode == BlendMode.Modulation) blendmode = SDL_BlendMode.SDL_BLENDMODE_MOD;
            SDL_SetTextureBlendMode(this.Texture, blendmode);
            if (this.Renderer != null) this.Renderer.Update();
        }

        #region DrawGlyph Overloads
        public void DrawGlyph(char c, int X, int Y, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawGlyph(c, X, Y, new Color(R, G, B, A), DrawOptions);
        }
        public void DrawGlyph(char c, Point p, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawGlyph(c, p.X, p.Y, color, DrawOptions);
        }
        public void DrawGlyph(char c, Point p, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawGlyph(c, p.X, p.Y, new Color(R, G, B, A), DrawOptions);
        }
        public void DrawGlyph(char c, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawGlyph(c, 0, 0, color, DrawOptions);
        }
        public void DrawGlyph(char c, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
        {
            this.DrawGlyph(c, 0, 0, new Color(R, G, B, A), DrawOptions);
        }
        #endregion
        public virtual void DrawGlyph(char c, int X, int Y, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
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
            else         TextBitmap = new Bitmap(TTF_RenderGlyph_Blended(SDL_Font, c, color.SDL_Color));
            if (centeralign) X -= TextBitmap.Width / 2;
            if (rightalign)  X -= TextBitmap.Width;
            this.Build(new Rect(X, Y, TextBitmap.Width, TextBitmap.Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
            TextBitmap.Dispose();
            if (this.Renderer != null) this.Renderer.Update();
        }

        /// <summary>
        /// Locks the bitmap and converts the surface to a texture. The bitmap can no longer be modified until unlocked.
        /// </summary>
        public virtual void Lock()
        {
            if (Locked) throw new BitmapLockedException();
            this.Locked = true;
            this.RecreateTexture();
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
        /// Saves the current bitmap to a file as a PNG.
        /// </summary>
        /// <param name="filename">The filename to save the bitmap as.</param>
        public virtual void SaveToPNG(string filename)
        {
            SDL2.SDL_image.IMG_SavePNG(Surface, filename);
        }

        /// <summary>
        /// Converts the SDL_Surface to an SDL_Texture used when rendering.
        /// </summary>
        public virtual void RecreateTexture()
        {
            if (this.Renderer == null) return;
            SDL_BlendMode blend;
            SDL_GetTextureBlendMode(this.Texture, out blend);
            if (this.Texture != IntPtr.Zero && this.Texture != null) SDL_DestroyTexture(this.Texture);
            this.Texture = SDL_CreateTextureFromSurface(this.Renderer.SDL_Renderer, this.Surface);
            if (blend != SDL_BlendMode.SDL_BLENDMODE_NONE) SDL_SetTextureBlendMode(this.Texture, blend);
            if (ColorToneBmp != null) ColorToneBmp.Dispose();
            ColorToneBmp = null;
            this.Renderer.Update();
        }

        private Bitmap ColorToneBmp;
        private Color ColorToneColor;
        private Tone ColorToneTone;
        // Applies a Sprite's Color and Tone. CPU-intensive.
        public virtual IntPtr ColorToneTexture(Color Color, Tone Tone)
        {
            if (ColorToneBmp != null &&
                ColorToneColor.Red == Color.Red && ColorToneColor.Green == Color.Green && ColorToneColor.Blue == Color.Blue && ColorToneColor.Alpha == Color.Alpha &&
                ColorToneTone.Red == Tone.Red && ColorToneTone.Green == Tone.Green && ColorToneTone.Blue == Tone.Blue && ColorToneTone.Gray == Tone.Gray)
            {
                return ColorToneBmp.Texture;
            }
            ColorToneBmp?.Dispose();
            ColorToneBmp = new Bitmap(Width, Height);
            ColorToneBmp.Renderer = this.Renderer;
            ColorToneBmp.Unlock();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Color c = GetPixel(x, y);
                    if (c.Alpha == 0) continue;
                    Color n = new Color(c.Red, c.Green, c.Blue, c.Alpha);
                    double Avg = (c.Red + c.Green + c.Blue) / 3d;
                    // Tone
                    n.Red = Convert.ToByte(Math.Round(c.Red - ((c.Red - Avg) * (Tone.Gray / 255d))) + Tone.Red);
                    n.Green = Convert.ToByte(Math.Round(c.Green - ((c.Green - Avg) * (Tone.Gray / 255d))) + Tone.Green);
                    n.Blue = Convert.ToByte(Math.Round(c.Blue - ((c.Blue - Avg) * (Tone.Gray / 255d))) + Tone.Blue);
                    // Color - Additive blending (with double RGB strength so it can range between 0 and 255)
                    n.Red = Convert.ToByte(Math.Min(255, Math.Max(0, (-255 + 2 * Color.Red) * Color.Alpha / 255d + n.Red)));
                    n.Green = Convert.ToByte(Math.Min(255, Math.Max(0, (-255 + 2 * Color.Green) * Color.Alpha / 255d + n.Green)));
                    n.Blue = Convert.ToByte(Math.Min(255, Math.Max(0, (-255 + 2 * Color.Blue) * Color.Alpha / 255d + n.Blue)));
                    ColorToneBmp.SetPixel(x, y, n);
                }
            }
            ColorToneBmp.Lock();
            ColorToneColor = Color.Clone();
            ColorToneTone = Tone.Clone();
            return ColorToneBmp.Texture;
        }
    }

    public enum BlendMode
    {
        Addition,
        Modulation,
        Blend,
        None
    }

    public enum DrawOptions
    {
        Bold          = 1,
        Italic        = 2,
        Underlined    = 4,
        Strikethrough = 8,
        Aliased       = 16,
        LeftAlign     = 32,
        CenterAlign   = 64,
        RightAlign    = 128
    }

    public enum Location
    {
        TopRight,
        TopLeft,
        BottomRight,
        BottomLeft
    }

    public class BitmapLockedException : Exception
    {
        public BitmapLockedException() : base("The bitmap was locked for writing.")
        {

        }
    }
}
