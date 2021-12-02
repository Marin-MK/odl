using System;
using static odl.SDL2.SDL;

namespace odl;

[Serializable]
public class Color : ICloneable
{
    /// <summary>
    /// A solid red color.
    /// </summary>
    public static Color RED = new Color(255, 0, 0);
    /// <summary>
    /// A solid green color.
    /// </summary>
    public static Color GREEN = new Color(0, 255, 0);
    /// <summary>
    /// A solid blue color.
    /// </summary>
    public static Color BLUE = new Color(0, 0, 255);
    /// <summary>
    /// A fully transparent color.
    /// </summary>
    public static Color ALPHA = new Color(0, 0, 0, 0);
    /// <summary>
    /// A solid black color.
    /// </summary>
    public static Color BLACK = new Color(0, 0, 0);
    /// <summary>
    /// A solid white color.
    /// </summary>
    public static Color WHITE = new Color(255, 255, 255);

    /// <summary>
    /// The SDL_Color object.
    /// </summary>
    public SDL_Color SDL_Color
    {
        get
        {
            SDL_Color c = new SDL_Color();
            c.r = this.Red;
            c.g = this.Green;
            c.b = this.Blue;
            c.a = this.Alpha;
            return c;
        }
    }
    /// <summary>
    /// The Red component of the color.
    /// </summary>
    public byte Red;
    /// <summary>
    /// The Green component of the color.
    /// </summary>
    public byte Green;
    /// <summary>
    /// The Blue component of the color.
    /// </summary>
    public byte Blue;
    /// <summary>
    /// The Alpha component of the color.
    /// </summary>
    public byte Alpha = 255;
    /// <summary>
    /// Converts the Alpha component of the color (0-255) to a factor (0-1)
    /// </summary>
    public double AlphaFactor { get { return Alpha / 255d; } }

    /// <summary>
    /// Creates a new Color object.
    /// </summary>
    /// <param name="Red">The Red component of the color.</param>
    /// <param name="Green">The Green component of the color.</param>
    /// <param name="Blue">The Blue component of the color.</param>
    /// <param name="Alpha">The Alpha component of the color.</param>
    public Color(byte Red, byte Green, byte Blue, byte Alpha = 255)
    {
        this.Red = Red;
        this.Green = Green;
        this.Blue = Blue;
        this.Alpha = Alpha;
    }

    public object Clone()
    {
        return new Color(this.Red, this.Green, this.Blue, this.Alpha);
    }

    public override string ToString()
    {
        return $"(Color: {this.Red},{this.Green},{this.Blue},{this.Alpha})";
    }

    public override bool Equals(object obj)
    {
        if (this == obj) return true;
        if (obj is Color)
        {
            Color c = (Color)obj;
            return this.Red == c.Red &&
                   this.Green == c.Green &&
                   this.Blue == c.Blue &&
                   this.Alpha == c.Alpha;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
