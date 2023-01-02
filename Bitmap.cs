using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static odl.SDL2.SDL;
using static odl.SDL2.SDL_ttf;
using static odl.SDL2.SDL_image;
using System.Text;

namespace odl;

public class Bitmap : IDisposable
{
    /// <summary>
    /// The default blend mode all bitmaps will be given.
    /// </summary>
    public static BlendMode DefaultBlendMode = BlendMode.Blend;

    /// <summary>
    /// Stores all bitmaps ever created so that they can be recreated if the VSync state of the program is changed.
    /// </summary>
    public static List<Bitmap> BitmapList = new List<Bitmap>();

    /// <summary>
    /// The pointer to the SDL_Surface.
    /// </summary>
    internal IntPtr Surface = IntPtr.Zero;
    /// <summary>
    /// The SDL_Surface object.
    /// </summary>
    internal SDL_Surface SurfaceObject;
    /// <summary>
    /// The pointer to the SDL_Texture.
    /// </summary>
    public IntPtr Texture { get; set; } = IntPtr.Zero;
    protected int _width;
    /// <summary>
    /// The width of the bitmap.
    /// </summary>
    public int Width { get { return _width; } protected set { _width = value; } }
    protected int _height;
    /// <summary>
    /// The height of the bitmap.
    /// </summary>
    public int Height { get { return _height; } protected set { _height = value; } }
    /// <summary>
    /// Whether or not the bitmap has been disposed.
    /// </summary>
    public bool Disposed { get; protected set; }
    private Renderer _renderer;
    /// <summary>
    /// The Renderer object associated with the bitmap.
    /// </summary>
    internal Renderer Renderer { get { return _renderer; } set { _renderer = value; if (IsChunky) foreach (Bitmap b in this.InternalBitmaps) b.Renderer = value; } }
    /// <summary>
    /// The Font object associated with the bitmap.
    /// </summary>
    public virtual Font Font { get; set; }
    /// <summary>
    /// The pointer to the raw pixel data.
    /// </summary>
    public virtual unsafe byte* PixelPointer
    {
        get
        {
            return (byte*)SurfaceObject.pixels;
        }
    }
    /// <summary>
    /// Whether the bitmap can be written on.
    /// </summary>
    public bool Locked { get; protected set; }
    /// <summary>
    /// A list of internal bitmaps, used when a bitmap is split up into several bitmap chunks.
    /// </summary>
    public List<Bitmap> InternalBitmaps = new List<Bitmap>();
    /// <summary>
    /// Whether or not the bitmap is split up into multiple chunks.
    /// </summary>
    public bool IsChunky { get { return InternalBitmaps.Count > 0; } }
    /// <summary>
    /// The size of the internal bitmap chunks.
    /// </summary>
    public Size ChunkSize { get; protected set; }
    /// <summary>
    /// The way this bitmap is blended with other bitmaps during rendering.
    /// </summary>
    public BlendMode BlendMode { get; set; } = DefaultBlendMode;

    /// <summary>
    /// The X position of this bitmap as part of a chunky bitmap.
    /// </summary>
    public int InternalX = 0;
    /// <summary>
    /// The Y position of this bitmap as part of a chunky bitmap.
    /// </summary>
    public int InternalY = 0;

    /// <summary>
    /// Whether this bitmap is in RGBA8 format (non-standard).
    /// </summary>
    public bool RGBA8 { get; protected set; } = false;
    /// <summary>
    /// Whether this bitmap is in ABGR8 format (standard).
    /// </summary>
    public bool ABGR8 { get; protected set; } = true;

    /// <summary>
    /// Used only for bitmaps created with a byte array.
    /// </summary>
    IntPtr PixelHandle = IntPtr.Zero;

    /// <summary>
    /// Creates a new bitmap with the given size.
    /// </summary>
    /// <param name="Size">The size of the new bitmap.</param>
    public Bitmap(Size Size)
        : this(Size.Width, Size.Height) { }

    /// <summary>
    /// Loads the specified file into a bitmap.
    /// </summary>
    /// <param name="Filename">The file to load into a bitmap.</param>
    public unsafe Bitmap(string Filename)
    {
        string RFilename = FindRealFilename(Filename);
        if (RFilename == null) throw new FileNotFoundException($"File could not be found -- {Filename}");
        (Size ImageSize, bool IsPNG) = ValidateIMG(RFilename);
        if (IsPNG && ImageSize.Width > Graphics.MaxTextureSize.Width && ImageSize.Height > Graphics.MaxTextureSize.Height)
        {
            (byte[] Bytes, int Width, int Height) data = decodl.PNGDecoder.Decode(RFilename);
            byte[] bytes = data.Bytes;
            int width = data.Width;
            int height = data.Height;
            this.ChunkSize = new Size(Math.Min(width, Graphics.MaxTextureSize.Width), Math.Min(height, Graphics.MaxTextureSize.Height));
            int xbmps = (int)Math.Ceiling((double)width / Graphics.MaxTextureSize.Width);
            int ybmps = (int)Math.Ceiling((double)height / Graphics.MaxTextureSize.Height);
            for (int ybmp = 0; ybmp < ybmps; ybmp++)
            {
                for (int xbmp = 0; xbmp < xbmps; xbmp++)
                {
                    int wbmp = xbmp == xbmps - 1 ? width - (xbmps - 1) * Graphics.MaxTextureSize.Width : Graphics.MaxTextureSize.Width;
                    int hbmp = ybmp == ybmps - 1 ? height - (ybmps - 1) * Graphics.MaxTextureSize.Height : Graphics.MaxTextureSize.Height;
                    IntPtr curbmp = Marshal.AllocHGlobal(wbmp * hbmp * 4);
                    for (int y = 0; y < hbmp; y++)
                    {
                        Marshal.Copy(bytes, xbmp * Graphics.MaxTextureSize.Width * 4 + ybmp * width * 4, curbmp, wbmp * hbmp * 4);
                    }
                    Bitmap bmp = new Bitmap(curbmp, wbmp, hbmp);
                    bmp.InternalX = xbmp * Graphics.MaxTextureSize.Width;
                    bmp.InternalY = ybmp * Graphics.MaxTextureSize.Height;
                    InternalBitmaps.Add(bmp);
                }
            }
            this.Width = width;
            this.Height = height;
            RGBA8 = InternalBitmaps[0].RGBA8;
            ABGR8 = InternalBitmaps[0].ABGR8;
        }
        else if (IsPNG && ImageSize.Height > Graphics.MaxTextureSize.Height)
        {
            (byte[] Bytes, int Width, int Height) data = decodl.PNGDecoder.Decode(RFilename);
            byte[] bytes = data.Bytes;
            int width = data.Width;
            int height = data.Height;
            this.ChunkSize = new Size(width, Math.Min(height, Graphics.MaxTextureSize.Height));
            int ybmps = (int)Math.Ceiling((double)height / Graphics.MaxTextureSize.Height);
            for (int ybmp = 0; ybmp < ybmps; ybmp++)
            {
                int hbmp = ybmp == ybmps - 1 ? height - (ybmps - 1) * Graphics.MaxTextureSize.Height : Graphics.MaxTextureSize.Height;
                IntPtr curbmp = Marshal.AllocHGlobal(width * hbmp * 4);
                int pos = ybmp * Graphics.MaxTextureSize.Height * width * 4;
                int len = width * hbmp * 4;
                Marshal.Copy(bytes, pos, curbmp, len);
                Bitmap bmp = new Bitmap(curbmp, width, hbmp);
                bmp.InternalX = 0;
                bmp.InternalY = ybmp * Graphics.MaxTextureSize.Height;
                InternalBitmaps.Add(bmp);
            }
            this.Width = width;
            this.Height = height;
            RGBA8 = InternalBitmaps[0].RGBA8;
            ABGR8 = InternalBitmaps[0].ABGR8;
        }
        else
        {
            this.Surface = IMG_Load(RFilename);
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
            this.Width = SurfaceObject.w;
            this.Height = SurfaceObject.h;
            SDL_PixelFormat format = Marshal.PtrToStructure<SDL_PixelFormat>(this.SurfaceObject.format);
            RGBA8 = format.format == SDL_PixelFormatEnum.SDL_PIXELFORMAT_RGBA8888;
            ABGR8 = format.format == SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888;
        }
        this.Lock();
        BitmapList.Add(this);
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
        this.Surface = SDL_CreateRGBSurfaceWithFormat(0, Width, Height, 32, SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888);
        //this.Surface = SDL_CreateRGBSurface(0, Width, Height, 32, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);
        this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
        this.Width = SurfaceObject.w;
        this.Height = SurfaceObject.h;
        if (!(this is SolidBitmap)) this.Lock();
        BitmapList.Add(this);
    }

    public Bitmap(int Width, int Height, Size ChunkSize) : this(Width, Height, ChunkSize.Width, ChunkSize.Height) { }
    public Bitmap(int Width, int Height, int ChunkWidth, int ChunkHeight)
    {
        if (Width < 1 || Height < 1)
        {
            throw new Exception($"Invalid Bitmap size ({Width},{Height}) -- must be at least (1,1)");
        }
        this.Width = Width;
        this.Height = Height;
        this.ChunkSize = ChunkSize;
        int ChunkCountHor = (int)Math.Ceiling((double)Width / ChunkWidth);
        int ChunkCountVer = (int)Math.Ceiling((double)Height / ChunkHeight);
        for (int x = 0; x < ChunkCountHor; x++)
        {
            for (int y = 0; y < ChunkCountVer; y++)
            {
                int w = Math.Min(ChunkWidth, Width - x * ChunkWidth);
                int h = Math.Min(ChunkHeight, Height - y * ChunkHeight);
                Bitmap b = new Bitmap(w, h);
                b.InternalX = x * ChunkWidth;
                b.InternalY = y * ChunkHeight;
                InternalBitmaps.Add(b);
            }
        }
        this.ChunkSize = new Size(ChunkWidth, ChunkHeight);
        this.Lock();
        BitmapList.Add(this);
    }

    /// <summary>
    /// Creates a bitmap object to wrap around an existing SDL_Surface.
    /// </summary>
    /// <param name="Surface"></param>
    public Bitmap(IntPtr Surface)
    {
        this.Surface = Surface;
        this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
        this.Width = SurfaceObject.w;
        this.Height = SurfaceObject.h;
        this.Lock();
        BitmapList.Add(this);
    }

    /// <summary>
    /// Creates a Bitmap from an RGBA byte array in memory.
    /// </summary>
    /// <param name="PixelPtr">Pointer to RGBA byte array of pixels.</param>
    /// <param name="Width">The width of the bitmap.</param>
    /// <param name="Height">The height of the bitmap.</param>
    public Bitmap(IntPtr PixelPtr, int Width, int Height)
    {
        this.Surface = SDL_CreateRGBSurfaceWithFormatFrom(PixelPtr, Width, Height, 32, Width * 4, SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888);
        this.PixelHandle = PixelPtr;
        if (this.Surface == IntPtr.Zero)
            throw new Exception($"odl failed to create a Bitmap from memory.\n\n" + SDL2.SDL.SDL_GetError());
        this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
        this.Width = this.SurfaceObject.w;
        this.Height = this.SurfaceObject.h;
        this.Lock();
        BitmapList.Add(this);
    }

    /// <summary>
    /// Creates a Bitmap from a Color list in memory.
    /// </summary>
    /// <param name="Pixels">The list of colors representing pixels.</param>
    /// <param name="Width">The width of the bitmap.</param>
    /// <param name="Height">The height of the bitmap.</param>
    public Bitmap(List<Color> Pixels, int Width, int Height)
    {
        this.PixelHandle = Marshal.AllocHGlobal(Pixels.Count * 4);
        for (int i = 0; i < Pixels.Count; i++)
        {
            Marshal.WriteByte(PixelHandle + i * 4, Pixels[i].Red);
            Marshal.WriteByte(PixelHandle + i * 4 + 1, Pixels[i].Green);
            Marshal.WriteByte(PixelHandle + i * 4 + 2, Pixels[i].Blue);
            Marshal.WriteByte(PixelHandle + i * 4 + 3, Pixels[i].Alpha);
        }
        this.Surface = SDL_CreateRGBSurfaceWithFormatFrom(PixelHandle, Width, Height, 32, Width * 4, SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888);
        if (this.Surface == IntPtr.Zero)
            throw new Exception($"odl failed to create a Bitmap from memory.\n\n" + SDL2.SDL.SDL_GetError());
        this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
        this.Width = SurfaceObject.w;
        this.Height = SurfaceObject.h;
        this.Lock();
        BitmapList.Add(this);
    }

    /// <summary>
    /// Creates a Bitmap from a Color list in memory.
    /// </summary>
    /// <param name="Pixels">The array of bytes representing color values.</param>
    /// <param name="Width">The width of the bitmap.</param>
    /// <param name="Height">The height of the bitmap.</param>
    public Bitmap(byte[] Pixels, int Width, int Height)
    {
        this.PixelHandle = Marshal.AllocHGlobal(Pixels.Length);
        Marshal.Copy(Pixels, 0, this.PixelHandle, Pixels.Length);
        this.Surface = SDL_CreateRGBSurfaceWithFormatFrom(PixelHandle, Width, Height, 32, Width * 4, SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888);
        if (this.Surface == IntPtr.Zero)
            throw new Exception($"odl failed to create a Bitmap from memory.\n\n" + SDL2.SDL.SDL_GetError());
        this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
        this.Width = SurfaceObject.w;
        this.Height = SurfaceObject.h;
        this.Lock();
        BitmapList.Add(this);
    }

    protected (Size Size, bool IsPNG) ValidateIMG(string Filename)
    {
        BinaryReader br = new BinaryReader(File.OpenRead(Filename));
        byte[] pngsignature = new byte[8] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        bool validpng = true;
        for (int i = 0; i < 8; i++)
        {
            if (br.ReadByte() != pngsignature[i])
            {
                validpng = false;
                break;
            }
        }
        if (validpng)
        {
            br.BaseStream.Position = 16;
            byte[] widthbytes = new byte[sizeof(int)];
            for (int i = 0; i < sizeof(int); i++) widthbytes[sizeof(int) - 1 - i] = br.ReadByte();
            int width = BitConverter.ToInt32(widthbytes, 0);
            byte[] heightbytes = new byte[sizeof(int)];
            for (int i = 0; i < sizeof(int); i++) heightbytes[sizeof(int) - 1 - i] = br.ReadByte();
            int height = BitConverter.ToInt32(heightbytes, 0);
            return (new Size(width, height), true);
        }
        else if (Graphics.LoadedJPEG)
        {
            br.BaseStream.Position = 0;
            byte[] jpegsignature = new byte[3] { 0xFF, 0xD8, 0xFF };
            bool validjpeg = true;
            for (int i = 0; i < 3; i++)
            {
                byte bt = br.ReadByte();
                if (bt != jpegsignature[i])
                {
                    validjpeg = false;
                    break;
                }
            }
            if (!validjpeg)
            {
                throw new Exception($"The given file is not a valid PNG or JPG/JPEG file: '{Filename}'");
            }
            return (null, false);
        }
        else
        {
            throw new Exception($"The given file is not a valid PNG file: '{Filename}'");
        }
    }

    ~Bitmap()
    {
        if (!Disposed || this.Surface != IntPtr.Zero || this.Texture != IntPtr.Zero)
        {
            Console.WriteLine($"An undisposed bitmap is being collected by the GC! This is a memory leak!\n    Bitmap info: Size ({Width},{Height})");
        }
        if (BitmapList.Contains(this)) BitmapList.Remove(this);
    }

    public static string FindRealFilename(string Filename)
    {
        while (Filename.Contains('\\')) Filename = Filename.Replace('\\', '/');
        if (!FileExistsCaseSensitive(Filename))
        {
            if (FileExistsCaseSensitive(Filename + ".png")) Filename += ".png";
            else if (FileExistsCaseSensitive(Filename + ".PNG")) Filename += ".PNG";
            else if (Filename.EndsWith(".png") && FileExistsCaseSensitive(Filename.Substring(0, Filename.Length - 3) + "PNG"))
                Filename = Filename.Substring(0, Filename.Length - 3) + "PNG";
            else if (FileExistsCaseSensitive(Filename + ".jpg")) Filename += ".jpg";
            else if (FileExistsCaseSensitive(Filename + ".jpeg")) Filename += ".jpeg";
            else if (FileExistsCaseSensitive(Filename + ".JPG")) Filename += ".JPG";
            else if (FileExistsCaseSensitive(Filename + ".JPEG")) Filename += ".JPEG";
            else if (Filename.EndsWith(".jpg") && FileExistsCaseSensitive(Filename.Substring(0, Filename.Length - 3) + "JPG"))
                Filename = Filename.Substring(0, Filename.Length - 3) + "JPG";
            else if (Filename.EndsWith(".jpeg") && FileExistsCaseSensitive(Filename.Substring(0, Filename.Length - 4) + "JPEG"))
                Filename = Filename.Substring(0, Filename.Length - 4) + "JPEG";
            else return null;
        }
        return Filename;
    }

    /// <summary>
    /// Case-sensitive version of File.Exists, but slower.
    /// </summary>
    /// <param name="Filename">The file to look for.</param>
    /// <returns>Whether the file exists.</returns>
    public static bool FileExistsCaseSensitive(string Filename)
    {
        if (!File.Exists(Filename)) return false;
        string fullfilepath = Path.GetFullPath(Filename);
        while (fullfilepath.Contains('\\')) fullfilepath = fullfilepath.Replace('\\', '/');
        string dirname = Path.GetDirectoryName(Filename);
        string[] files = Directory.GetFiles(dirname);
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = Path.GetFullPath(files[i]);
            while (files[i].Contains('\\')) files[i] = files[i].Replace('\\', '/');
        }
        return dirname != null && Array.Exists(files, e => e == fullfilepath);
    }

    /// <summary>
    /// Creates a bitmap with a mask.
    /// </summary>
    /// <param name="Source">The bitmap to sample.</param>
    /// <param name="Mask">The bitmap whose alpha values will be checked for masking.</param>
    /// <param name="XOffset">An x offset for sampling.</param>
    /// <param name="YOffset">An y ofset for sampling.</param>
    public static Bitmap Mask(Bitmap Mask, Bitmap Source, int XOffset = 0, int YOffset = 0)
    {
        return Bitmap.Mask(Mask, Source, new Rect(0, 0, Source.Width, Source.Height), XOffset, YOffset);
    }
    /// <summary>
    /// Creates a bitmap with a mask.
    /// </summary>
    /// <param name="Source">The bitmap to sample.</param>
    /// <param name="Mask">The bitmap whose alpha values will be checked for masking.</param>
    /// <param name="SourceRect">The rectangle inside the Source bitmap to sample from.</param>
    /// <param name="XOffset">An x offset for sampling, within the source rectangle.</param>
    /// <param name="YOffset">An y ofset for sampling, within the source rectangle.</param>
    public static unsafe Bitmap Mask(Bitmap Mask, Bitmap Source, Rect SourceRect, int XOffset = 0, int YOffset = 0)
    {
        byte* sourcepixels = Source.PixelPointer;
        byte* maskpixels = Mask.PixelPointer;
        Bitmap Result = new Bitmap(Mask.Width, Mask.Height);
        Result.Unlock();
        byte* resultpixels = Result.PixelPointer;
        for (int x = 0; x < Mask.Width; x++)
        {
            for (int y = 0; y < Mask.Height; y++)
            {
                byte maskalpha = maskpixels[y * Mask.Width * 4 + x * 4 + 3];
                if (maskalpha == 0) continue;
                int srcx = SourceRect.X + (x + XOffset) % SourceRect.Width;
                int srcy = SourceRect.Y + (y + YOffset) % SourceRect.Height;
                resultpixels[y * Result.Width * 4 + x * 4] = sourcepixels[srcy * Source.Width * 4 + srcx * 4];
                resultpixels[y * Result.Width * 4 + x * 4 + 1] = sourcepixels[srcy * Source.Width * 4 + srcx * 4 + 1];
                resultpixels[y * Result.Width * 4 + x * 4 + 2] = sourcepixels[srcy * Source.Width * 4 + srcx * 4 + 2];
                resultpixels[y * Result.Width * 4 + x * 4 + 3] = sourcepixels[srcy * Source.Width * 4 + srcx * 4 + 3];
            }
        }
        Result.Lock();
        return Result;
    }

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

    public override string ToString()
    {
        return $"(Bitmap: {this.Width},{this.Height})";
    }

    /// <summary>
    /// Clears the bitmap content.
    /// </summary>
    public virtual void Clear()
    {
        if (IsChunky)
        {
            foreach (Bitmap b in this.InternalBitmaps) b.Clear();
        }
        else
        {
            // Filling an alpha rectangle is faster than recreating the bitmap.
            FillRect(0, 0, this.Width, this.Height, Color.ALPHA);
        }
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
    /// Creates a clone of the bitmap.
    /// </summary>
    public virtual Bitmap Clone()
    {
        if (IsChunky)
        {
            throw new Exception("Cannot clone chunky Bitmap yet!");
        }
        else
        {
            Bitmap bmp = new Bitmap(Width, Height);
            bmp.Unlock();
            bmp.Build(this);
            bmp.Lock();
            bmp.Font = this.Font;
            bmp.Renderer = this.Renderer;
            return bmp;
        }
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
    public virtual unsafe void SetPixel(int X, int Y, byte r, byte g, byte b, byte a = 255)
    {
        if (Locked) throw new BitmapLockedException();
        if (X < 0 || Y < 0) throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- minimum is (0,0)");
        if (X >= this.Width || Y >= this.Height) throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
        if (IsChunky)
        {
            Bitmap bmp = GetBitmapFromCoordinate(X, Y);
            if (bmp.Locked) bmp.Unlock();
            bmp.SetPixel(X - bmp.InternalX, Y - bmp.InternalY, r, g, b, a);
        }
        else
        {
            if (!RGBA8 && !ABGR8) ConvertToABGR8();
            int num = 0;
            if (RGBA8)
            {
                num = (r << 24) + (g << 16) + (b << 8) + a;
            }
            else
            {
                num = (a << 24) + (b << 16) + (g << 8) + r;
            }
            Marshal.WriteInt32((nint) PixelPointer + Width * Y * 4 + X * 4, num);
        }
        if (this.Renderer != null) this.Renderer.Update();
    }

    #region SetPixelFast Overloads
    /// <summary>
    /// Performs purely byte assignment, and no safety or validity checks. Faster when used in bulk, but more dangerous. This overload does not change the alpha value of the pixel.
    /// </summary>
    /// <param name="X">The X position in the bitmap.</param>
    /// <param name="Y">The Y position in the bitmap.</param>
    /// <param name="r">The Red component of the color to set the pixel to.</param>
    /// <param name="g">The Green component of the color to set the pixel to.</param>
    /// <param name="b">The Blue component of the color to set the pixel to.</param>
    public virtual unsafe void SetPixelFast(int X, int Y, byte r, byte g, byte b)
    {
        SetPixelFast(X, Y, r, g, b, 255);
    }
    #endregion
    /// <summary>
    /// Performs purely byte assignment, and no safety or validity checks. Faster when used in bulk, but more dangerous.
    /// </summary>
    /// <param name="X">The X position in the bitmap.</param>
    /// <param name="Y">The Y position in the bitmap.</param>
    /// <param name="r">The Red component of the color to set the pixel to.</param>
    /// <param name="g">The Green component of the color to set the pixel to.</param>
    /// <param name="b">The Blue component of the color to set the pixel to.</param>
    /// <param name="a">The Alpha component of the color to set the pixel to.</param>
    public virtual unsafe void SetPixelFast(int X, int Y, byte r, byte g, byte b, byte a)
    {
        int num = (a << 24) + (b << 16) + (g << 8) + r;
        Marshal.WriteInt32((nint) PixelPointer + Width * Y * 4 + X * 4, num);
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
    public virtual unsafe Color GetPixel(int X, int Y)
    {
        if (X < 0 || Y < 0) throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- minimum is (0,0)");
        if (X >= this.Width || Y >= this.Height) throw new Exception($"Invalid Bitmap coordinate ({X},{Y}) -- exceeds Bitmap size of ({this.Width},{this.Height})");
        if (IsChunky)
        {
            Bitmap bmp = GetBitmapFromCoordinate(X, Y);
            return bmp.GetPixel(X - bmp.InternalX, Y - bmp.InternalY);
        }
        else
        {
            if (!RGBA8 && !ABGR8) ConvertToABGR8();
            int num = Marshal.ReadInt32((nint) PixelPointer + Width * Y * 4 + X * 4);
            if (RGBA8)
            {
                return new Color(
                    (byte) ((num >> 24) & 0xFF),
                    (byte) ((num >> 16) & 0xFF),
                    (byte) ((num >> 8) & 0xFF),
                    (byte) (num & 0xFF)
                );
            }
            else
            {
                return new Color(
                    (byte) (num & 0xFF),
                    (byte) ((num >> 8) & 0xFF),
                    (byte) ((num >> 16) & 0xFF),
                    (byte) ((num >> 24) & 0xFF)
                );
            }
        }
    }

    /// <summary>
    /// Performs purely byte reading, and no safety or validity checks. Faster when used in bulk, but more dangerous.
    /// </summary>
    /// <param name="X">The X position in the bitmap.</param>
    /// <param name="Y">The Y position in the bitmap.</param>
    public virtual unsafe Color GetPixelFast(int X, int Y)
    {
        int num = Marshal.ReadInt32((nint) PixelPointer, Width * Y * 4 + X * 4);
        return new Color(
            (byte) (num & 0xFF),
            (byte) ((num >> 8) & 0xFF),
            (byte) ((num >> 16) & 0xFF),
            (byte) ((num >> 24) & 0xFF)
        );
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
        if (x1 < 0 || x2 < 0 || x1 >= Width || x2 >= Width ||
            y1 < 0 || y2 < 0 || y1 >= Height || y2 >= Height) throw new Exception($"Line out of bounds.");
        for (int x = x1 > x2 ? x2 : x1; (x1 > x2) ? (x <= x1) : (x <= x2); x++)
        {
            double fact = ((double)x - x1) / (x2 - x1);
            int y = (int)Math.Round(y1 + ((y2 - y1) * fact));
            if (y >= 0) SetPixel(x, y, r, g, b, a);
        }
        int sy = y1 > y2 ? y2 : y1;
        for (int y = y1 > y2 ? y2 : y1; (y1 > y2) ? (y <= y1) : (y <= y2); y++)
        {
            double fact = ((double)y - y1) / (y2 - y1);
            int x = (int)Math.Round(x1 + ((x2 - x1) * fact));
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
            for (int i = ox - y; i <= ox + y; i++)
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
        Rect DestRect = new Rect(X, Y, Width, Height);
        if (IsChunky)
        {
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
        }
        else
        {
            SDL_Rect Rect = DestRect.SDL_Rect;
            SDL_FillRect(this.Surface, ref Rect, SDL_MapRGBA(this.SurfaceObject.format, r, g, b, a));
        }
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
        if (IsChunky)
        {
            foreach (Bitmap bmp in this.InternalBitmaps)
            {
                Rect bmprect = new Rect(bmp.InternalX, bmp.InternalY, this.ChunkSize);
                if (DestRect.Overlaps(bmprect))
                {
                    int nx = Math.Max(DestRect.X, bmprect.X);
                    int ny = Math.Max(DestRect.Y, bmprect.Y);
                    int nw = Math.Min(DestRect.X + DestRect.Width, bmprect.X + bmprect.Width) - nx;
                    int nh = Math.Min(DestRect.Y + DestRect.Height, bmprect.Y + bmprect.Height) - ny;
                    int DX = nx - bmp.InternalX;
                    int DY = ny - bmp.InternalY;
                    int DW = nw;
                    int DH = nh;
                    int SX = SrcRect.X + nx - DestRect.X;
                    int SY = SrcRect.Y + ny - DestRect.Y;
                    int SW = DW;
                    int SH = DH;
                    if (bmp.Locked) bmp.Unlock();
                    bmp.Build(
                        DX, DY, DW, DH,
                        SrcBitmap,
                        SX, SY, SW, SH
                    );
                }
            }
        }
        else
        {
            if (SrcBitmap.IsChunky)
            {
                foreach (Bitmap bmp in SrcBitmap.InternalBitmaps)
                {
                    Rect bmprect = new Rect(bmp.InternalX, bmp.InternalY, SrcBitmap.ChunkSize);
                    if (SrcRect.Overlaps(bmprect))
                    {
                        int nx = Math.Max(SrcRect.X, bmprect.X);
                        int ny = Math.Max(SrcRect.Y, bmprect.Y);
                        int nw = Math.Min(SrcRect.X + SrcRect.Width, bmprect.X + bmprect.Width) - nx;
                        int nh = Math.Min(SrcRect.Y + SrcRect.Height, bmprect.Y + bmprect.Height) - ny;
                        int DX = DestRect.X + (nx - SrcRect.X);
                        int DY = DestRect.Y + (ny - SrcRect.Y);
                        int DW = nw;
                        int DH = nh;
                        this.Build(
                            DX, DY, DW, DH,
                            bmp,
                            nx - bmp.InternalX, ny - bmp.InternalY, nw, nh // XY - InternalXY
                        );
                    }
                }
            }
            else
            {
                SDL_Rect Src = SrcRect.SDL_Rect;
                SDL_Rect Dest = DestRect.SDL_Rect;
                if (Dest.w != Src.w || Dest.h != Src.h) SDL_BlitScaled(SrcBitmap.Surface, ref Src, this.Surface, ref Dest);
                else SDL_BlitSurface(SrcBitmap.Surface, ref Src, this.Surface, ref Dest);
            }
        }
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
    /// Draws a string of text at (0, 0).
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="c">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawText(Text, 0, 0, c, DrawOptions);
    }
    /// <summary>
    /// Draws a string of text at (0, 0).
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
        if (Text.Length == 1)
        {
            DrawGlyph(Convert.ToChar(Text), X, Y, c, DrawOptions);
            return;
        }
        if (Locked) throw new BitmapLockedException();
        if (this.Font == null)
        {
            throw new Exception("No Font specified for this Bitmap.");
        }
        Text = Text.Replace("\r", "").Replace("\n", "");
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
        Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(Text));
        if (aliased) TextBitmap = new Bitmap(TTF_RenderUTF8_Solid(SDL_Font, Text, c.SDL_Color));
        else TextBitmap = new Bitmap(TTF_RenderUTF8_Blended(SDL_Font, Text, c.SDL_Color));
        if (centeralign) X -= TextBitmap.Width / 2;
        if (rightalign) X -= TextBitmap.Width;
        this.Build(new Rect(X, Y, TextBitmap.Width, TextBitmap.Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
        TextBitmap.Dispose();
        this.BlendMode = BlendMode.Addition;
        if (this.Renderer != null) this.Renderer.Update();
    }

    #region DrawText Overloads
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
        if (Text.Length == 1)
        {
            DrawGlyph(Convert.ToChar(Text), X, Y, Width, Height, c, DrawOptions);
            return;
        }
        if (Locked) throw new BitmapLockedException();
        if (this.Font == null)
        {
            throw new Exception("No Font specified for this Bitmap.");
        }
        Text = Text.Replace("\r", "").Replace("\n", "");
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
        this.BlendMode = BlendMode.Addition;
        if (this.Renderer != null) this.Renderer.Update();
    }

    #region DrawGlyph Overloads
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, int X, int Y, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, X, Y, new Color(R, G, B, A), DrawOptions);
    }
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, color, DrawOptions);
    }
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, new Color(R, G, B, A), DrawOptions);
    }
    /// <summary>
    /// Draws a character at (0, 0).
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, 0, 0, color, DrawOptions);
    }
    /// <summary>
    /// Draws a character at (0, 0).
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, 0, 0, new Color(R, G, B, A), DrawOptions);
    }
    #endregion
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
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
        else TextBitmap = new Bitmap(TTF_RenderGlyph_Blended(SDL_Font, c, color.SDL_Color));
        if (centeralign) X -= TextBitmap.Width / 2;
        if (rightalign) X -= TextBitmap.Width;
        this.Build(new Rect(X, Y, TextBitmap.Width, TextBitmap.Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
        TextBitmap.Dispose();
        this.BlendMode = BlendMode.Addition;
        if (this.Renderer != null) this.Renderer.Update();
    }

    #region DrawGlyph Overloads
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="rect">The rectangle within which to draw the text.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Rect rect, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, rect.X, rect.Y, rect.Width, rect.Height, new Color(R, G, B, A), DrawOptions);
    }
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="s">The size of the rectangle within which to draw the text.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, Size s, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, s.Width, s.Height, new Color(R, G, B, A), DrawOptions);
    }
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="Width">The width within which to draw the text.</param>
    /// <param name="Height">The height within which to draw the text.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, int Width, int Height, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, Width, Height, new Color(R, G, B, A), DrawOptions);
    }
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="s">The size of the rectangle within which to draw the text.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, int X, int Y, Size s, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, X, Y, s.Width, s.Height, new Color(R, G, B, A), DrawOptions);
    }
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="Width">The width within which to draw the text.</param>
    /// <param name="Height">The height within which to draw the text.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, int X, int Y, int Width, int Height, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, X, Y, Width, Height, new Color(R, G, B, A), DrawOptions);
    }
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="rect">The rectangle within which to draw the text.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Rect rect, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, rect.X, rect.Y, rect.Width, rect.Height, color, DrawOptions);
    }
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="s">The size of the rectangle within which to draw the text.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, Size s, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, s.Width, s.Height, color, DrawOptions);
    }
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="Width">The width within which to draw the text.</param>
    /// <param name="Height">The height within which to draw the text.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, int Width, int Height, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, Width, Height, color, DrawOptions);
    }
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="s">The size of the rectangle within which to draw the text.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, int X, int Y, Size s, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, X, Y, s.Width, s.Height, color, DrawOptions);
    }
    #endregion
    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="Width">The width within which to draw the text.</param>
    /// <param name="Height">The height within which to draw the text.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public virtual void DrawGlyph(char c, int X, int Y, int Width, int Height, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
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
        this.Build(new Rect(X, Y, Width, Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
        TextBitmap.Dispose();
        this.BlendMode = BlendMode.Addition;
        if (this.Renderer != null) this.Renderer.Update();
    }

    #region DrawGradientLine Overloads
    /// <summary>
    /// Draws a line with its color linearly interpolated between two colors.
    /// </summary>
    /// <param name="p1">The starting position of the line.</param>
    /// <param name="p2">The ending position of the line.</param>
    /// <param name="c1">The starting color of the line.</param>
    /// <param name="c2">The ending color of the line.</param>
    public void DrawGradientLine(Point p1, Point p2, Color c1, Color c2)
    {
        DrawGradientLine(p1.X, p1.Y, p2.X, p2.Y, c1, c2);
    }
    /// <summary>
    /// Draws a line with its color linearly interpolated between two colors.
    /// </summary>
    /// <param name="x1">The starting X position of the line.</param>
    /// <param name="y1">The starting Y position of the line.</param>
    /// <param name="p2">The ending position of the line.</param>
    /// <param name="c1">The starting color of the line.</param>
    /// <param name="c2">The ending color of the line.</param>
    public void DrawGradientLine(int x1, int y1, Point p2, Color c1, Color c2)
    {
        DrawGradientLine(x1, y1, p2.X, p2.Y, c1, c2);
    }
    /// <summary>
    /// Draws a line with its color linearly interpolated between two colors.
    /// </summary>
    /// <param name="p1">The starting position of the line.</param>
    /// <param name="x2">The ending X position of the line.</param>
    /// <param name="y2">The ending Y position of the line.</param>
    /// <param name="c1">The starting color of the line.</param>
    /// <param name="c2">The ending color of the line.</param>
    public void DrawGradientLine(Point p1, int x2, int y2, Color c1, Color c2)
    {
        DrawGradientLine(p1.X, p1.Y, x2, y2, c1, c2);
    }
    #endregion
    /// <summary>
    /// Draws a line with its color linearly interpolated between two colors.
    /// </summary>
    /// <param name="x1">The starting X position of the line.</param>
    /// <param name="y1">The starting Y position of the line.</param>
    /// <param name="x2">The ending X position of the line.</param>
    /// <param name="y2">The ending Y position of the line.</param>
    /// <param name="c1">The starting color of the line.</param>
    /// <param name="c2">The ending color of the line.</param>
    public virtual void DrawGradientLine(int x1, int y1, int x2, int y2, Color c1, Color c2)
    {
        if (Locked) throw new BitmapLockedException();
        if (x1 < 0 || x2 < 0 || x1 >= Width || x2 >= Width ||
            y1 < 0 || y2 < 0 || y1 >= Height || y2 >= Height) throw new Exception($"Line out of bounds.");
        if (x1 != x2)
        {
            for (int x = x1 > x2 ? x2 : x1; (x1 > x2) ? (x <= x1) : (x <= x2); x++)
            {
                double fact = ((double) x - x1) / (x2 - x1);
                int y = (int) Math.Round(y1 + ((y2 - y1) * fact));
                if (y >= 0)
                {
                    long d1 = (long) Math.Sqrt(Math.Pow(x - x1, 2) + Math.Pow(y - y1, 2));
                    long d2 = (long) Math.Sqrt(Math.Pow(x - x2, 2) + Math.Pow(y - y2, 2));
                    double f1 = d2 / (double) (d1 + d2);
                    double f2 = d1 / (double) (d1 + d2);
                    byte r = (byte) Math.Round(f1 * c1.Red + f2 * c2.Red);
                    byte g = (byte) Math.Round(f1 * c1.Green + f2 * c2.Green);
                    byte b = (byte) Math.Round(f1 * c1.Blue + f2 * c2.Blue);
                    byte a = (byte) Math.Round(f1 * c1.Alpha + f2 * c2.Alpha);
                    SetPixel(x, y, r, g, b, a);
                }
            }
        }
        int sy = y1 > y2 ? y2 : y1;
        if (y1 != y2)
        {
            for (int y = y1 > y2 ? y2 : y1; (y1 > y2) ? (y <= y1) : (y <= y2); y++)
            {
                double fact = ((double) y - y1) / (y2 - y1);
                int x = (int) Math.Round(x1 + ((x2 - x1) * fact));
                if (x >= 0)
                {
                    long d1 = (long) (Math.Pow(x - x1, 2) + Math.Pow(y - y1, 2));
                    long d2 = (long) (Math.Pow(x - x2, 2) + Math.Pow(y - y2, 2));
                    double f1 = d2 / (double) (d1 + d2);
                    double f2 = d1 / (double) (d1 + d2);
                    byte r = (byte) Math.Round(f1 * c1.Red + f2 * c2.Red);
                    byte g = (byte) Math.Round(f1 * c1.Green + f2 * c2.Green);
                    byte b = (byte) Math.Round(f1 * c1.Blue + f2 * c2.Blue);
                    byte a = (byte) Math.Round(f1 * c1.Alpha + f2 * c2.Alpha);
                    SetPixel(x, y, r, g, b, a);
                }
            }
        }
        if (this.Renderer != null) this.Renderer.Update();
    }

    #region FillGradientRect Overloads
    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Rect">The rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="c3">The color in the bottom-left corner of the rectangle.</param>
    /// <param name="c4">The color in the bottom-right corner of the rectangle.</param>
    public void FillGradientRect(Rect Rect, Color c1, Color c2, Color c3, Color c4)
    {
        FillGradientRect(Rect.X, Rect.Y, Rect.Width, Rect.Height, c1, c2, c3, c4);
    }
    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Point">The position of the rectangle to fill.</param>
    /// <param name="Size">The size of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="c3">The color in the bottom-left corner of the rectangle.</param>
    /// <param name="c4">The color in the bottom-right corner of the rectangle.</param>
    public void FillGradientRect(Point Point, Size Size, Color c1, Color c2, Color c3, Color c4)
    {
        FillGradientRect(Point.X, Point.Y, Size.Width, Size.Height, c1, c2, c3, c4);
    }
    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="X">The X position of the rectangle to fill.</param>
    /// <param name="Y">The Y position of the rectangle to fill.</param>
    /// <param name="Size">The size of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="c3">The color in the bottom-left corner of the rectangle.</param>
    /// <param name="c4">The color in the bottom-right corner of the rectangle.</param>
    public void FillGradientRect(int X, int Y, Size Size, Color c1, Color c2, Color c3, Color c4)
    {
        FillGradientRect(X, Y, Size.Width, Size.Height, c1, c2, c3, c4);
    }
    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Point">The position of the rectangle to fill.</param>
    /// <param name="Width">The width of the rectangle to fill.</param>
    /// <param name="Height">The height of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="c3">The color in the bottom-left corner of the rectangle.</param>
    /// <param name="c4">The color in the bottom-right corner of the rectangle.</param>
    public void FillGradientRect(Point Point, int Width, int Height, Color c1, Color c2, Color c3, Color c4)
    {
        FillGradientRect(Point.X, Point.Y, Width, Height, c1, c2, c3, c4);
    }
    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Rect">The rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="Flipped">Whether to flip the direction of the gradient on the horizontal axis.</param>
    /// <param name="UseTriangles">Whether to draw the rectangle using two triangles for a smooth gradient, or to use a simple rectangular function.</param>
    public void FillGradientRect(Rect Rect, Color c1, Color c2, bool Flipped = false, bool UseTriangles = true)
    {
        FillGradientRect(Rect.X, Rect.Y, Rect.Width, Rect.Height, c1, c2, Flipped, UseTriangles);
    }
    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Point">The position of the rectangle to fill.</param>
    /// <param name="Size">The size of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="Flipped">Whether to flip the direction of the gradient on the horizontal axis.</param>
    /// <param name="UseTriangles">Whether to draw the rectangle using two triangles for a smooth gradient, or to use a simple rectangular function.</param>
    public void FillGradientRect(Point Point, Size Size, Color c1, Color c2, bool Flipped = false, bool UseTriangles = true)
    {
        FillGradientRect(Point.X, Point.Y, Size.Width, Size.Height, c1, c2, Flipped, UseTriangles);
    }
    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="X">The X position of the rectangle to fill.</param>
    /// <param name="Y">The Y position of the rectangle to fill.</param>
    /// <param name="Size">The size of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="Flipped">Whether to flip the direction of the gradient on the horizontal axis.</param>
    /// <param name="UseTriangles">Whether to draw the rectangle using two triangles for a smooth gradient, or to use a simple rectangular function.</param>
    public void FillGradientRect(int X, int Y, Size Size, Color c1, Color c2, bool Flipped = false, bool UseTriangles = true)
    {
        FillGradientRect(X, Y, Size.Width, Size.Height, c1, c2, Flipped, UseTriangles);
    }
    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="Point">The position of the rectangle to fill.</param>
    /// <param name="Width">The width of the rectangle to fill.</param>
    /// <param name="Height">The height of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="Flipped">Whether to flip the direction of the gradient on the horizontal axis.</param>
    /// <param name="UseTriangles">Whether to draw the rectangle using two triangles for a smooth gradient, or to use a simple rectangular function.</param>
    public void FillGradientRect(Point Point, int Width, int Height, Color c1, Color c2, bool Flipped = false, bool UseTriangles = true)
    {
        FillGradientRect(Point.X, Point.Y, Width, Height, c1, c2, Flipped, UseTriangles);
    }
    #endregion
    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="X">The X position of the rectangle to fill.</param>
    /// <param name="Y">The Y position of the rectangle to fill.</param>
    /// <param name="Width">The width of the rectangle to fill.</param>
    /// <param name="Height">The height of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="c3">The color in the bottom-left corner of the rectangle.</param>
    /// <param name="c4">The color in the bottom-right corner of the rectangle.</param>
    public virtual void FillGradientRect(int X, int Y, int Width, int Height, Color c1, Color c2, Color c3, Color c4)
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
        for (int dy = Y; dy < Y + Height; dy++)
        {
            for (int dx = X; dx < X + Width; dx++)
            {
                double xl = dx - X;
                double xr = X + Width - 1 - dx;
                double yt = dy - Y;
                double yb = Y + Height - 1 - dy;
                double fxr = (xl / (xl + xr));
                double fxl = 1 - fxr;
                double fyb = (yt / (yt + yb));
                double fyt = 1 - fyb;
                double f1 = fxl * fyt;
                double f2 = fxr * fyt;
                double f3 = fxl * fyb;
                double f4 = fxr * fyb;
                byte r = (byte) Math.Round(f1 * c1.Red + f2 * c2.Red + f3 * c3.Red + f4 * c4.Red);
                byte g = (byte) Math.Round(f1 * c1.Green + f2 * c2.Green + f3 * c3.Green + f4 * c4.Green);
                byte b = (byte) Math.Round(f1 * c1.Blue + f2 * c2.Blue + f3 * c3.Blue + f4 * c4.Blue);
                byte a = (byte) Math.Round(f1 * c1.Alpha + f2 * c2.Alpha + f3 * c3.Alpha + f4 * c4.Alpha);
                SetPixel(dx, dy, r, g, b, a);
            }
        }
        if (this.Renderer != null) this.Renderer.Update();
    }

    /// <summary>
    /// Fills a rectangle with colors bilinearly interpolated from 4 color values, anchored at the corners.
    /// </summary>
    /// <param name="X">The X position of the rectangle to fill.</param>
    /// <param name="Y">The Y position of the rectangle to fill.</param>
    /// <param name="Width">The width of the rectangle to fill.</param>
    /// <param name="Height">The height of the rectangle to fill.</param>
    /// <param name="c1">The color in the top-left corner of the rectangle.</param>
    /// <param name="c2">The color in the top-right corner of the rectangle.</param>
    /// <param name="Flipped">Whether to flip the direction of the gradient on the horizontal axis.</param>
    /// <param name="UseTriangles">Whether to draw the rectangle using two triangles for a smooth gradient, or to use a simple rectangular function.</param>
    public virtual void FillGradientRect(int X, int Y, int Width, int Height, Color c1, Color c2, bool Flipped = false, bool UseTriangles = true)
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
        if (UseTriangles)
        {
            Color half = Interpolate2D(c1, c2, 0.5);
            FillGradientTriangle(
                new Vertex(X, Y + Height - 1, Flipped ? c2 : half),
                new Vertex(X, Y, Flipped ? half : c1),
                new Vertex(X + Width - 1, Y, Flipped ? c1 : half)
            );
            FillGradientTriangle(
                new Vertex(X, Y + Height - 1, Flipped ? c2 : half),
                new Vertex(X + Width - 1, Y, Flipped ? c1 : half),
                new Vertex(X + Width - 1, Y + Height - 1, Flipped ? half : c2)
            );
        }
        else
        {
            for (int dy = Y; dy < Y + Height; dy++)
            {
                for (int dx = X; dx < X + Width; dx++)
                {
                    long d1 = (long) (Math.Pow(dx - X, 2) + Math.Pow(dy - (Flipped ? Y + Height : Y), 2));
                    long d2 = (long) (Math.Pow(dx - (X + Width), 2) + Math.Pow(dy - (Flipped ? Y : Y + Height), 2));
                    double f1 = d2 / (double) (d1 + d2);
                    double f2 = d1 / (double) (d1 + d2);
                    byte r = (byte) Math.Round(f1 * c1.Red + f2 * c2.Red);
                    byte g = (byte) Math.Round(f1 * c1.Green + f2 * c2.Green);
                    byte b = (byte) Math.Round(f1 * c1.Blue + f2 * c2.Blue);
                    byte a = (byte) Math.Round(f1 * c1.Alpha + f2 * c2.Alpha);
                    SetPixel(dx, dy, r, g, b, a);
                }
            }
        }
        if (this.Renderer != null) this.Renderer.Update();
    }

    /// <summary>
    /// Fills a triangle with a solid color. The points must be given in clockwise order.
    /// </summary>
    /// <param name="v0">The left-most point of the triangle.</param>
    /// <param name="v1">The upper-most or right-most point of the triangle.</param>
    /// <param name="v2">The remaining point of the triangle.</param>
    /// <param name="c">The color to fill the triangle with.</param>
    public virtual void FillTriangle(Point v0, Point v1, Point v2, Color c)
    {
        if (Locked) throw new BitmapLockedException();
        if (v0.X < 0 || v0.Y < 0 || v0.X >= Width || v0.Y >= Height) throw new Exception("First triangle vector is out of bounds.");
        if (v1.X < 0 || v1.Y < 0 || v1.X >= Width || v1.Y >= Height) throw new Exception("Second triangle vector is out of bounds.");
        if (v2.X < 0 || v2.Y < 0 || v2.X >= Width || v2.Y >= Height) throw new Exception("Third triangle vector is out of bounds.");
        int minx = Math.Min(v0.X, Math.Min(v1.X, v2.X));
        int maxx = Math.Max(v0.X, Math.Max(v1.X, v2.X));
        int miny = Math.Min(v0.Y, Math.Min(v1.Y, v2.Y));
        int maxy = Math.Max(v0.Y, Math.Max(v1.Y, v2.Y));
        for (int x = minx; x <= maxx; x++)
        {
            for (int y = miny; y <= maxy; y++)
            {
                Point p = new Point(x, y);
                double w0 = EdgeFunction(v1, v2, p);
                double w1 = EdgeFunction(v2, v0, p);
                double w2 = EdgeFunction(v0, v1, p);
                if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                {
                    SetPixel(p, c);
                }
            }
        }
        this.Renderer?.Update();
    }

    #region FillGradientTriangle Overloads
    /// <summary>
    /// Fills a triangle with colors interpolated from the triangle vertices. The vertices must be given in clockwise order.
    /// </summary>
    /// <param name="v0">The left-most point of the triangle.</param>
    /// <param name="v1">The upper-most of right-most point of the triangle.</param>
    /// <param name="v2">The remaining point of the triangle.</param>
    /// <param name="c0">The color corresponding to <paramref name="v0"/>.</param>
    /// <param name="c1">The color corresponding to <paramref name="v1"/>.</param>
    /// <param name="c2">The color corresponding to <paramref name="v2"/>.</param>
    public virtual void FillGradientTriangle(Point v0, Point v1, Point v2, Color c0, Color c1, Color c2)
    {
        FillGradientTriangle(new Vertex(v0, c0), new Vertex(v1, c1), new Vertex(v2, c2));
    }
    #endregion
    /// <summary>
    /// Fills a triangle with colors interpolated from the triangle vertices. The vertices must be given in clockwise order.
    /// </summary>
    /// <param name="v0">The left-most vertex of the triangle.</param>
    /// <param name="v1">The upper-most of right-most vertex of the triangle.</param>
    /// <param name="v2">The remaining vertex of the triangle.</param>
    public virtual void FillGradientTriangle(Vertex v0, Vertex v1, Vertex v2)
    {
        if (Locked) throw new BitmapLockedException();
        if (v0.X < 0 || v0.Y < 0 || v0.X >= Width || v0.Y >= Height) throw new Exception("First triangle vector is out of bounds.");
        if (v1.X < 0 || v1.Y < 0 || v1.X >= Width || v1.Y >= Height) throw new Exception("Second triangle vector is out of bounds.");
        if (v2.X < 0 || v2.Y < 0 || v2.X >= Width || v2.Y >= Height) throw new Exception("Third triangle vector is out of bounds.");
        int minx = Math.Min(v0.X, Math.Min(v1.X, v2.X));
        int maxx = Math.Max(v0.X, Math.Max(v1.X, v2.X));
        int miny = Math.Min(v0.Y, Math.Min(v1.Y, v2.Y));
        int maxy = Math.Max(v0.Y, Math.Max(v1.Y, v2.Y));
        for (int x = minx; x <= maxx; x++)
        {
            for (int y = miny; y <= maxy; y++)
            {
                Point p = new Point(x, y);
                double w0 = EdgeFunction(v1, v2, p);
                double w1 = EdgeFunction(v2, v0, p);
                double w2 = EdgeFunction(v0, v1, p);
                if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                {
                    double area = EdgeFunction(v0, v1, v2);
                    double f0 = w0 / area;
                    double f1 = w1 / area;
                    double f2 = w2 / area;
                    byte r = (byte) Math.Round(f0 * v0.R + f1 * v1.R + f2 * v2.R);
                    byte g = (byte) Math.Round(f0 * v0.G + f1 * v1.G + f2 * v2.G);
                    byte b = (byte) Math.Round(f0 * v0.B + f1 * v1.B + f2 * v2.B);
                    byte a = (byte) Math.Round(f0 * v0.A + f1 * v1.A + f2 * v2.A);
                    SetPixel(p, r, g, b, a);
                }
            }
        }
        this.Renderer?.Update();
    }

    /// <summary>
    /// Draws a gradient between the <paramref name="inside"/> box and <paramref name="outside"/> box.
    /// </summary>
    /// <param name="outside">The outer box at which the gradient stops.</param>
    /// <param name="inside">The inner box from where to start the gradient.</param>
    /// <param name="c1">The inner-most color.</param>
    /// <param name="c2">The outer-most color.</param>
    /// <param name="FillInside">Whether to also fill the inner rectangle with <paramref name="c1"/>.</param>
    public virtual void FillGradientRectOutside(Rect outside, Rect inside, Color c1, Color c2, bool FillInside = true)
    {
        for (int y = outside.Y; y < outside.Y + outside.Height; y++)
        {
            for (int x = outside.X; x < outside.X + outside.Width; x++)
            {
                if (inside.Contains(x, y)) continue;
                double d = -1;
                if (x < inside.X && y >= inside.Y && y <= inside.Y + inside.Height)
                    d = (x - outside.X) / (double) (inside.X - outside.X - 1);
                else if (x >= inside.X + inside.Width && y >= inside.Y && y <= inside.Y + inside.Height)
                    d = 1 - (x - inside.X - inside.Width) / (double) (outside.X + outside.Width - inside.X - inside.Width - 1);
                else if (y < inside.Y && x >= inside.X && x <= inside.X + inside.Width)
                    d = (y - outside.Y) / (double) (inside.Y - outside.Y - 1);
                else if (y >= inside.Y + inside.Height && x >= inside.X && x <= inside.X + inside.Width)
                    d = 1 - (y - inside.Y - inside.Height) / (double) (outside.Y + outside.Height - inside.Y - inside.Height - 1);
                if (d == -1) continue;
                d = Math.Clamp(d, 0, 1);
                byte r = (byte) Math.Round(d * c1.Red + (1 - d) * c2.Red);
                byte g = (byte) Math.Round(d * c1.Green + (1 - d) * c2.Green);
                byte b = (byte) Math.Round(d * c1.Blue + (1 - d) * c2.Blue);
                byte a = (byte) Math.Round(d * c1.Alpha + (1 - d) * c2.Alpha);
                SetPixel(x, y, r, g, b, a);
            }
        }
        if (FillInside) FillRect(inside, c1);
        FillGradientRect(outside.X, outside.Y, inside.X - outside.X, inside.Y - outside.Y, c2, c2, c2, c1);
        FillGradientRect(inside.X + inside.Width, outside.Y, outside.X + outside.Width - (inside.X + inside.Width), inside.Y - outside.Y, c2, c2, c1, c2);
        FillGradientRect(outside.X, inside.Y + inside.Height, inside.X - outside.X, outside.Y + outside.Height - (inside.Y + inside.Height), c2, c1, c2, c2);
        FillGradientRect(inside.X + inside.Width, inside.Y + inside.Height, outside.X + outside.Width - (inside.X + inside.Width), outside.Y + outside.Height - inside.Y - inside.Height, c1, c2, c2, c2);
    }

    #region EdgeFunction Overloads
    double EdgeFunction(Vertex a, Vertex b, Vertex c)
    {
        return EdgeFunction(a.Point, b.Point, c.Point);
    }
    double EdgeFunction(Vertex a, Vertex b, Point c)
    {
        return EdgeFunction(a.Point, b.Point, c);
    }
    #endregion
    protected virtual double EdgeFunction(Point a, Point b, Point c)
    {
        return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
    }

    #region FillGradientCircle
    /// <summary>
    /// Fills a circle with a radial gradient.
    /// </summary>
    /// <param name="c">The origin position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="color1">The inner color of the circle.</param>
    /// <param name="color2">The outer color of the cirlce.</param>
    public void FillGradientCircle(Point c, int Radius, Color color1, Color color2)
    {
        FillGradientCircle(c.X, c.Y, Radius, color1, color2);
    }
    #endregion
    /// <summary>
    /// Fills a circle with a radial gradient.
    /// </summary>
    /// <param name="ox">The origin X position of the circle.</param>
    /// <param name="oy">The origin Y position of the circle.</param>
    /// <param name="Radius">The radius of the circle.</param>
    /// <param name="c1">The inner color of the circle.</param>
    /// <param name="c2">The outer color of the circle.</param>
    public virtual void FillGradientCircle(int ox, int oy, int Radius, Color c1, Color c2)
    {
        if (Locked) throw new BitmapLockedException();
        int x = Radius - 1;
        int y = 0;
        int dx = 1;
        int dy = 1;
        Point center = new Point(ox, oy);
        int err = dx - (Radius << 1);
        while (x >= y)
        {
            for (int i = ox - x; i <= ox + x; i++)
            {
                SetPixel(i, oy + y, Interpolate2D(c1, c2, 1 - center.Distance(new Point(i, oy + y)) / Radius));
                SetPixel(i, oy - y, Interpolate2D(c1, c2, 1 - center.Distance(new Point(i, oy - y)) / Radius));
            }
            for (int i = ox - y; i <= ox + y; i++)
            {
                SetPixel(i, oy + x, Interpolate2D(c1, c2, 1 - center.Distance(new Point(i, oy + x)) / Radius));
                SetPixel(i, oy - x, Interpolate2D(c1, c2, 1 - center.Distance(new Point(i, oy - x)) / Radius));
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

    public virtual void FillGradientQuadrant(int ox, int oy, int Radius, Location Location, Color c1, Color c2, double Multiplier = 1.0, bool FillOutsideQuadrant = true)
    {
        if (Locked) throw new BitmapLockedException();
        int x = Radius - 1;
        int y = 0;
        int dx = 1;
        int dy = 1;
        if (FillOutsideQuadrant) FillRect(ox, oy, Radius, Radius, c1);
        if (Location == Location.TopLeft)
        {
            ox += Radius - 1;
            oy += Radius - 1;
        }
        else if (Location == Location.TopRight) oy += Radius - 1;
        else if (Location == Location.BottomLeft) ox += Radius - 1;
        Point center = new Point(ox, oy);
        int err = dx - (Radius << 1);
        while (x >= y)
        {
            if (Location == Location.TopRight) // 0 - 90
            {
                for (int i = ox + y; i <= ox + x; i++)
                {
                    SetPixel(i, oy - y, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(i, oy - oy)) / Radius));
                }
                for (int i = oy - x; i <= oy - y; i++)
                {
                    SetPixel(ox + y, i, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(ox + y, i)) / Radius));
                }
            }
            else if (Location == Location.TopLeft) // 90 - 180
            {
                for (int i = ox - x; i <= ox - y; i++)
                {
                    SetPixel(i, oy - y, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(i, oy - y)) / Radius));
                }
                for (int i = oy - x; i <= oy - y; i++)
                {
                    SetPixel(ox - y, i, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(ox - y, i)) / Radius));
                }
            }
            else if (Location == Location.BottomLeft) // 180 - 270
            {
                for (int i = ox - x; i <= ox - y; i++)
                {
                    SetPixel(i, oy + y, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(i, oy + oy)) / Radius));
                }
                for (int i = oy + y; i <= oy + x; i++)
                {
                    SetPixel(ox - y, i, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(ox - y, i)) / Radius));
                }
            }
            else if (Location == Location.BottomRight) // 270 - 360
            {
                for (int i = ox + y; i <= ox + x; i++)
                {
                    SetPixel(i, oy + y, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(i, oy + oy)) / Radius));
                }
                for (int i = oy + y; i <= oy + x; i++)
                {
                    SetPixel(ox + y, i, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(ox + y, i)) / Radius));
                }
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

    public void FlipVertically()
    {
        FlipVertically(0, 0, Width, Height);
    }

    public void FlipVertically(int X, int Y, int Width, int Height)
    {
        if (X < 0 || Y < 0 || X + Width > this.Width || Y + Height > this.Height || this.Width < 0 || this.Height < 0)
            throw new Exception("Region out of bounds");
        if (Locked) throw new BitmapLockedException();
        if (!ABGR8) ConvertToABGR8();
        // TODO: Solve with memory/block copying for performance improvement vs. individual byte accesses
        Stack<Queue<Color>> Colors = new Stack<Queue<Color>>();
        for (int dy = Y; dy < Y + Height; dy++)
        {
            Queue<Color> Pixels = new Queue<Color>();
            for (int dx = X; dx < X + Width; dx++)
            {
                Pixels.Enqueue(GetPixelFast(dx, dy));
            }
            Colors.Push(Pixels);
        }
        for (int dy = Y; dy < Y + Height; dy++)
        {
            Queue<Color> Pixels = Colors.Pop();
            for (int dx = X; dx < X + Width; dx++)
            {
                Color c = Pixels.Dequeue();
                SetPixelFast(dx, dy, c.Red, c.Green, c.Blue, c.Alpha);
            }
        }
    }

    public void FlipHorizontally()
    {
        FlipHorizontally(0, 0, Width, Height);
    }

    public void FlipHorizontally(int X, int Y, int Width, int Height)
    {
        if (X < 0 || Y < 0 || X + Width > this.Width || Y + Height > this.Height || this.Width < 0 || this.Height < 0)
            throw new Exception("Region out of bounds");
        if (Locked) throw new BitmapLockedException();
        if (!ABGR8) ConvertToABGR8();
        // TODO: Solve with memory/block copying for performance improvement vs. individual byte accesses
        Stack<Queue<Color>> Colors = new Stack<Queue<Color>>();
        for (int dx = X; dx < X + Width; dx++)
        {
            Queue<Color> Pixels = new Queue<Color>();
            for (int dy = Y; dy < Y + Height; dy++)
            {
                Pixels.Enqueue(GetPixelFast(dx, dy));
            }
            Colors.Push(Pixels);
        }
        for (int dx = X; dx < X + Width; dx++)
        {
            Queue<Color> Pixels = Colors.Pop();
            for (int dy = Y; dy < Y + Height; dy++)
            {
                Color c = Pixels.Dequeue();
                SetPixelFast(dx, dy, c.Red, c.Green, c.Blue, c.Alpha);
            }
        }
    }
    
    /// <summary>
    /// Applies a box blur filter on the original image.
    /// </summary>
    /// <param name="Weight">The weight or size of the blur. Runtime increases exponentially with the weight. Must be at least 1.</param>
    /// <param name="TransparentEdges">If true, non-existent pixels for filtering near edges are seen as transparent. If false, a the filter weight is reduced locally.</param>
    /// <returns>The new blurred bitmap.</returns>
    public Bitmap Blur(int Weight = 1, float Scale = 1, bool TransparentEdges = true)
    {
        if (Weight < 1) throw new Exception("Blur weight must be at least 1.");
        Bitmap bmp = new Bitmap(Width, Height);
        bmp.Unlock();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float Red = 0,
                    Green = 0,
                    Blue = 0,
                    Alpha = 0;
                int count = 0;
                for (int dy = y - Weight; dy <= y + Weight; dy++)
                {
                    for (int dx = x - Weight; dx <= x + Weight; dx++)
                    {
                        if (dx >= 0 && dx < Width && dy >= 0 && dy < Height)
                        {
                            Color c = GetPixel(dx, dy);
                            Red += c.Red;
                            Green += c.Green;
                            Blue += c.Blue;
                            Alpha += c.Alpha;
                            count++;
                        }
                        else if (TransparentEdges) count++;
                    }
                }
                Red = Math.Clamp((float) Math.Round(Red * Scale / count), 0, 255);
                Green = Math.Clamp((float) Math.Round(Green * Scale / count), 0, 255);
                Blue = Math.Clamp((float) Math.Round(Blue * Scale / count), 0, 255);
                Alpha /= count;
                bmp.SetPixel(x, y, (byte) Red, (byte) Green, (byte) Blue, (byte) Alpha);
            }
        }
        bmp.Lock();
        return bmp;
    }

    /// <summary>
    /// Applies a box blur filter on the original image.
    /// </summary>
    /// <param name="Inside">Points within this region are not blurred, so as to save processing time. Useful especially when working with solid colors.</param>
    /// <param name="Weight">The weight or size of the blur. Runtime increases exponentially with the weight. Must be at least 1.</param>
    /// <param name="TransparentEdges">If true, non-existent pixels for filtering near edges are seen as transparent. If false, a the filter weight is reduced locally.</param>
    /// <returns>The new blurred bitmap.</returns>
    public Bitmap BlurExcludeRectangle(Rect Inside, int Weight = 1, bool TransparentEdges = true)
    {
        if (Weight < 1) throw new Exception("Blur weight must be at least 1.");
        Bitmap bmp = new Bitmap(Width, Height);
        bmp.Unlock();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Inside.Contains(x, y)) continue;
                int Red = 0,
                    Green = 0,
                    Blue = 0,
                    Alpha = 0;
                int count = 0;
                for (int dy = y - Weight; dy <= y + Weight; dy++)
                {
                    for (int dx = x - Weight; dx <= x + Weight; dx++)
                    {
                        if (dx >= 0 && dx < Width && dy >= 0 && dy < Height)
                        {
                            Color c = GetPixel(dx, dy);
                            Red += c.Red;
                            Green += c.Green;
                            Blue += c.Blue;
                            Alpha += c.Alpha;
                            count++;
                        }
                        else if (TransparentEdges) count++;
                    }
                }
                Red /= count;
                Green /= count;
                Blue /= count;
                Alpha /= count;
                bmp.SetPixel(x, y, (byte)Red, (byte)Green, (byte)Blue, (byte)Alpha);
            }
        }
        bmp.Lock();
        return bmp;
    }

    public Bitmap Bloom(float Scaling, float Threshold, int Weight)
    {
        Bitmap HighThreshold = WithThreshold(Threshold);
        Bitmap Filter = HighThreshold.Blur(Weight, Scaling);
        Bitmap Result = new Bitmap(Width, Height);
        Result.Unlock();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Color source = GetPixel(x, y);
                Color filter = Filter.GetPixel(x, y);
                Color ret = new Color(
                    (byte) Math.Clamp(Math.Round((double) (source.Red + filter.Red)), 0, 255),
                    (byte) Math.Clamp(Math.Round((double) (source.Green + filter.Green)), 0, 255),
                    (byte) Math.Clamp(Math.Round((double) (source.Blue + filter.Blue)), 0, 255),
                    (byte) Math.Clamp(Math.Round((double) (source.Alpha + filter.Alpha)), 0, 255)
                );
                Result.SetPixel(x, y, ret);
            }
        }
        Result.Lock();
        return Result;
    }

    public Bitmap WithThreshold(float Threshold)
    {
        Bitmap bmp = new Bitmap(Width, Height);
        bmp.Unlock();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Color c = GetPixel(x, y);
                if (c.Red / 255d >= Threshold || c.Green / 255d >= Threshold || c.Blue / 255d >= Threshold)
                    bmp.SetPixel(x, y, c);
            }
        }
        bmp.Lock();
        return bmp;
    }

    /// <summary>
    /// Interpolates between two colors.
    /// </summary>
    /// <param name="c1">The first color.</param>
    /// <param name="c2">The second color.</param>
    /// <param name="f1">The percentage of the first color the new color will contain.</param>
    /// <returns>The color interpolated between <paramref name="c1"/> and <paramref name="c2"/>.</returns>
    public static Color Interpolate2D(Color c1, Color c2, double f1)
    {
        f1 = Math.Clamp(f1, 0, 1);
        return new Color(
            (byte) Math.Round(f1 * c1.Red + (1 - f1) * c2.Red),
            (byte) Math.Round(f1 * c1.Green + (1 - f1) * c2.Green),
            (byte) Math.Round(f1 * c1.Blue + (1 - f1) * c2.Blue),
            (byte) Math.Round(f1 * c1.Alpha + (1 - f1) * c2.Alpha)
        );
    }

    /// <summary>
    /// Changes the size of the bitmap without scaling or interpolation.
    /// </summary>
    /// <param name="NewSize">The new size of the bitmap.</param>
    /// <returns>The resized bitmap.</returns>
    public Bitmap Resize(Size NewSize)
    {
        return Resize(NewSize.Width, NewSize.Height);
    }

    /// <summary>
    /// Changes the size of the bitmap without scaling or interpolation.
    /// </summary>
    /// <param name="NewWidth">The new width of the bitmap.</param>
    /// <param name="NewHeight">The new height of the bitmap.</param>
    /// <returns>The resized bitmap.</returns>
    public Bitmap Resize(int NewWidth, int NewHeight)
    {
        Bitmap NewBitmap = new Bitmap(NewWidth, NewHeight);
        NewBitmap.Unlock();
        NewBitmap.Build(this);
        NewBitmap.Lock();
        return NewBitmap;
    }

    /// <summary>
    /// Converts the bitmap to an ABGR8 format.
    /// </summary>
    protected virtual void ConvertToABGR8()
    {
        if (this.Surface == IntPtr.Zero) throw new Exception("Can not convert non-existing surface.");
        if (PixelHandle != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(PixelHandle);
            PixelHandle = IntPtr.Zero;
        }
        IntPtr oldsurface = this.Surface;
        this.Surface = SDL_ConvertSurfaceFormat(this.Surface, SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888, 0);
        this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
        RGBA8 = false;
        ABGR8 = true;
        SDL_FreeSurface(oldsurface);
        if (this.Renderer != null) this.Renderer.Update();
    }

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
    /// Saves the current bitmap to a file as a PNG.
    /// </summary>
    /// <param name="filename">The filename to save the bitmap as.</param>
    public virtual unsafe void SaveToPNG(string filename, bool Transparency = true, bool Indexed = false, int MaxPaletteSize = 0)
    {
        if (IsChunky)
        {
            throw new Exception("Cannot save chunky bitmap to PNG!");
        }
        else
        {
            decodl.PNGEncoder encoder = new decodl.PNGEncoder(PixelPointer, (uint) Width, (uint) Height);
            encoder.InvertData = RGBA8;
            encoder.ColorType = Transparency ? decodl.ColorTypes.RGBA : decodl.ColorTypes.RGB;
            if (Indexed)
            {
                encoder.ColorType = decodl.ColorTypes.Indexed;
                encoder.ReduceUnindexableImages = true;
                encoder.MaxPaletteSize = MaxPaletteSize;
                encoder.IncludeIndexedTransparency = Transparency;
            }
            encoder.Encode(filename);
        }
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
        SDL_SetTextureBlendMode(this.Texture, (SDL_BlendMode) this.BlendMode);
        this.Renderer.Update();
    }

    /// <summary>
    /// Applies a hue to the bitmap.
    /// </summary>
    /// <param name="Hue">The hue (0-360) to apply.</param>
    /// <returns>A new bitmap with the hue applied.</returns>
    public virtual Bitmap ApplyHue(int Hue)
    {
        if (!ABGR8) ConvertToABGR8();
        Bitmap bmp = new Bitmap(Width, Height);
        bmp.Unlock();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Color c = GetPixelFast(x, y);
                (float H, float S, float L) HSL = c.GetHSL();
                HSL.H = (HSL.H + Hue) % 360;
                c.SetHSL(HSL);
                bmp.SetPixelFast(x, y, c.Red, c.Green, c.Blue, c.Alpha);
            }
        }
        bmp.Lock();
        return bmp;
    }

    /// <summary>
    /// Applies a hue to a particular region of the bitmap.
    /// </summary>
    /// <param name="Hue">The hue (0-360) to apply.</param>
    /// <param name="OX">The X position of the rectangle to apply the hue to.</param>
    /// <param name="OY">The Y position of the rectangle to apply the hue to.</param>
    /// <param name="Width">The width of the rectangle to apply the hue to.</param>
    /// <param name="Height">The height of the rectangle to apply the hue to.</param>
    /// <returns>A new bitmap with the hue applied.</returns>
    public virtual Bitmap ApplyHue(int Hue, int OX, int OY, int Width, int Height)
    {
        if (!ABGR8) ConvertToABGR8();
        Bitmap bmp = new Bitmap(Width, Height);
        bmp.Unlock();
        for (int y = OY; y < Height; y++)
        {
            for (int x = OX; x < Width; x++)
            {
                Color c = GetPixelFast(x, y);
                (float H, float S, float L) HSL = c.GetHSL();
                HSL.H = (HSL.H + Hue) % 360;
                c.SetHSL(HSL);
                bmp.SetPixelFast(x - OX, y - OY, c.Red, c.Green, c.Blue, c.Alpha);
            }
        }
        bmp.Lock();
        return bmp;
    }
}

public enum BlendMode
{
    None = 0,
    Blend = 1,
    Addition = 2,
    Mod = 4
}

public enum DrawOptions
{
    None = 0,
    Bold = 1,
    Italic = 2,
    Underlined = 4,
    Strikethrough = 8,
    Aliased = 16,
    LeftAlign = 32,
    CenterAlign = 64,
    RightAlign = 128
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
