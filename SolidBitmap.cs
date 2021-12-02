using System;
using System.Runtime.InteropServices;
using static odl.SDL2.SDL;

namespace odl;

public class SolidBitmap : Bitmap
{
    public Color Color { get; private set; } = Color.ALPHA;
    public int BitmapWidth { get; protected set; } = 1;
    public int BitmapHeight { get; protected set; } = 1;
    public new int Width { get { throw new MethodNotSupportedException(this); } }
    public new int Height { get { throw new MethodNotSupportedException(this); } }

    public SolidBitmap(Size s, byte r, byte g, byte b, byte a = 255)
        : this(s.Width, s.Height, new Color(r, g, b, a)) { }
    public SolidBitmap(Size s, Color c)
        : this(s.Width, s.Height, c) { }
    public SolidBitmap(int Width, int Height, byte r, byte g, byte b, byte a = 255)
        : this(Width, Height, new Color(r, g, b, a)) { }
    public SolidBitmap(int Width, int Height)
        : this(Width, Height, Color.ALPHA) { }
    public SolidBitmap(Size s)
        : this(s.Width, s.Height, Color.ALPHA) { }
    public SolidBitmap(int Width, int Height, Color c)
        : base(1, 1)
    {
        this.SetColor(c);
        this.BitmapWidth = Width;
        this.BitmapHeight = Height;
    }

    /// <summary>
    /// Sets the color of the bitmap.
    /// </summary>
    /// <param name="r">The Red component of the bitmap color.</param>
    /// <param name="g">The Blue component of the bitmap color.</param>
    /// <param name="b">The Green component of the bitmap color.</param>
    /// <param name="a">The Alpha component of the bitmap color.</param>
    public void SetColor(byte r, byte g, byte b, byte a = 255)
    {
        this.SetColor(new Color(r, g, b, a));
    }
    /// <summary>
    /// Sets the color of the bitmap.
    /// </summary>
    public void SetColor(Color c)
    {
        this.Color = c;
        bool e = BitConverter.IsLittleEndian;
        Marshal.WriteByte(SurfaceObject.pixels, 0, e ? c.Red : c.Blue);
        Marshal.WriteByte(SurfaceObject.pixels, 1, c.Green);
        Marshal.WriteByte(SurfaceObject.pixels, 2, e ? c.Blue : c.Red);
        Marshal.WriteByte(SurfaceObject.pixels, 3, c.Alpha);
        this.RecreateTexture();
        this.Locked = true;
        if (this.Renderer != null) this.Renderer.Update();
    }

    /// <summary>
    /// Sets the size of the bitmap.
    /// </summary>
    public void SetSize(Size size)
    {
        this.SetSize(size.Width, size.Height);
    }
    /// <summary>
    /// Sets the size of the bitmap.
    /// </summary>
    /// <param name="Width">The width of the bitmap.</param>
    /// <param name="Height">The height of the bitmap.</param>
    public void SetSize(int Width, int Height)
    {
        this.BitmapWidth = Width;
        this.BitmapHeight = Height;
        if (this.Renderer != null) this.Renderer.Update();
    }

    public override string ToString()
    {
        return $"(SolidBitmap: {this.Width},{this.Height})";
    }

    public override void Clear()
    {
        if (Locked) throw new BitmapLockedException();
        if (this.Surface != IntPtr.Zero && this.Surface != null)
        {
            SDL_FreeSurface(this.Surface);
            SDL_DestroyTexture(this.Texture);
            this.Surface = SDL_CreateRGBSurface(0, this.Width, this.Height, 32, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);
            this.SurfaceObject = Marshal.PtrToStructure<SDL_Surface>(this.Surface);
            this.Color = Color.ALPHA;
            if (this.Renderer != null) this.Renderer.Update();
        }
    }

    #region Make these methods throw an error.
    public override void SetPixel(int X, int Y, byte r, byte g, byte b, byte a = 255)
    {
        throw new MethodNotSupportedException(this);
    }
    public override Color GetPixel(int X, int Y)
    {
        throw new MethodNotSupportedException(this);
    }
    public override void DrawLine(int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a = 255)
    {
        throw new MethodNotSupportedException(this);
    }
    public override void DrawLines(byte r, byte g, byte b, byte a, params Point[] points)
    {
        throw new MethodNotSupportedException(this);
    }
    public override void DrawCircle(int ox, int oy, int Radius, byte r, byte g, byte b, byte a = 255)
    {
        throw new MethodNotSupportedException(this);
    }
    public override void FillCircle(int ox, int oy, int Radius, byte r, byte g, byte b, byte a = 255)
    {
        throw new MethodNotSupportedException(this);
    }
    public override void DrawQuadrant(int ox, int oy, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
    {
        throw new MethodNotSupportedException(this);
    }
    public override void FillQuadrant(int ox, int oy, int Radius, Location l, byte r, byte g, byte b, byte a = 255)
    {
        throw new MethodNotSupportedException(this);
    }
    public override void DrawRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255)
    {
        throw new MethodNotSupportedException(this);
    }
    public override void FillRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255)
    {
        throw new MethodNotSupportedException(this);
    }
    public override void Build(Rect DestRect, Bitmap SrcBitmap, Rect SrcRect)
    {
        throw new MethodNotSupportedException(this);
    }
    public override Size TextSize(char Char, DrawOptions DrawOptions = 0)
    {
        throw new MethodNotSupportedException(this);
    }
    public override Size TextSize(string Text, DrawOptions DrawOptions = 0)
    {
        throw new MethodNotSupportedException(this);
    }
    public override void DrawText(string Text, int X, int Y, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        throw new MethodNotSupportedException(this);
    }
    public override void DrawGlyph(char c, int X, int Y, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        throw new MethodNotSupportedException(this);
    }
    /// <summary>
    /// Locks the bitmap and converts the surface to a texture. The bitmap can no longer be modified until unlocked.
    /// </summary>
    public override void Lock()
    {
        throw new MethodNotSupportedException(this);
    }
    /// <summary>
    /// Unlocks the bitmap, allowing you to modify the bitmap until locked again.
    /// </summary>
    public override void Unlock()
    {
        throw new MethodNotSupportedException(this);
    }
    #endregion
}
