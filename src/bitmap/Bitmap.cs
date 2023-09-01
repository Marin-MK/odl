using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static odl.SDL2.SDL;
using static odl.SDL2.SDL_ttf;
using static odl.SDL2.SDL_image;
using System.Text;
using System.Linq;

namespace odl;

public partial class Bitmap : IDisposable
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
    public virtual unsafe byte* PixelPointer => (byte*) SurfaceObject.pixels;
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
        string? RFilename = ODL.ImageResolver.ResolveFilename(Filename);
        if (RFilename is null) throw new FileNotFoundException($"File could not be found -- '{Filename}'");
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
        }
        else
        {
            this.Surface = IMG_Load(RFilename);
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
            this.Width = SurfaceObject.w;
            this.Height = SurfaceObject.h;
            SDL_PixelFormat format = Marshal.PtrToStructure<SDL_PixelFormat>(this.SurfaceObject.format);
            if (format.format != SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888) ConvertToABGR8();
        }
        this.Lock();
        BitmapList.Add(this);
    }

    protected Bitmap() { }

    public Bitmap(int Width, int Height) : this(Width, Height, Graphics.MaxTextureSize) { }
    public Bitmap(int Width, int Height, Size ChunkSize) : this(Width, Height, ChunkSize.Width, ChunkSize.Height) { }
    public Bitmap(int Width, int Height, int ChunkWidth, int ChunkHeight)
    {
        if (Width < 1 || Height < 1)
        {
            throw new Exception($"Invalid Bitmap size ({Width},{Height}) -- must be at least (1,1)");
        }
        if (Width > ChunkWidth || Height > ChunkHeight)
        {
            ODL.Logger?.WriteLine("Creating chunky bitmap");
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
        else
        {
            this.Surface = SDL_CreateRGBSurfaceWithFormat(0, Width, Height, 32, SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888);
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
            this.Width = SurfaceObject.w;
            this.Height = SurfaceObject.h;
            if (!(this is SolidBitmap)) this.Lock();
            BitmapList.Add(this);
            SDL_PixelFormat format = Marshal.PtrToStructure<SDL_PixelFormat>(this.SurfaceObject.format);
            if (format.format != SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888) ConvertToABGR8();
        }
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
        SDL_PixelFormat format = Marshal.PtrToStructure<SDL_PixelFormat>(this.SurfaceObject.format);
        if (format.format != SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888) ConvertToABGR8();
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
        SDL_PixelFormat format = Marshal.PtrToStructure<SDL_PixelFormat>(this.SurfaceObject.format);
        if (format.format != SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888) ConvertToABGR8();
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
        SDL_PixelFormat format = Marshal.PtrToStructure<SDL_PixelFormat>(this.SurfaceObject.format);
        if (format.format != SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888) ConvertToABGR8();
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
        SDL_PixelFormat format = Marshal.PtrToStructure<SDL_PixelFormat>(this.SurfaceObject.format);
        if (format.format != SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888) ConvertToABGR8();
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
            ODL.Logger?.Error($"An undisposed bitmap is being collected by the GC! This is a memory leak!\n    Bitmap info: Size ({Width},{Height})");
        }
        if (BitmapList.Contains(this)) BitmapList.Remove(this);
    }

    public override string ToString()
    {
        return $"(Bitmap: {this.Width},{this.Height})";
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
        SDL_FreeSurface(oldsurface);
        if (this.Renderer != null) this.Renderer.Update();
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
    public BitmapLockedException() : base("The bitmap was locked for writing.") { }
}
