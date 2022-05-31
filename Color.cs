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
        return this.Red << 24 + this.Green << 16 + this.Blue << 8 + this.Alpha;
    }

    /// <summary>
    /// Returns the HSL representation of this color.
    /// </summary>
    /// <returns>A tuple containing the HSL values.</returns>
    public (float H, float S, float L) GetHSL()
    {
        float r = this.Red / 255f;
        float g = this.Green / 255f;
        float b = this.Blue / 255f;
        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));
        float d = max - min;
        float H = 0;
        float S = 0;
        float L = (max + min) / 2;
        if (d == 0) { }
        else if (r >= g && r >= b)
        {
            float e = ((g - b) / d) % 6;
            if (e < 0) e += 6;
            H = 60 * e;
        }
        else if (g >= b)
        {
            H = 60 * ((b - r) / d + 2);
        }
        else
        {
            H = 60 * ((r - g) / d + 4);
        }

        if (d != 0)
        {
            if (L == 0) S = 1;
            else S = d / (1 - Math.Abs(2 * L - 1));
        }
        return (H, S, L);
    }

    /// <summary>
    /// Sets this color to value of an HSL color.
    /// </summary>
    /// <param name="Color">A tuple containing the HSL values.</param>
    public void SetHSL((float H, float S, float L) Color)
    {
        float H = Color.H;
        float S = Color.S;
        float L = Color.L;
        float C = (1 - Math.Abs(2 * L - 1)) * S;
        float X = C * (1 - Math.Abs((H / 60) % 2 - 1));
        float m = L - C / 2;
        (float R, float G, float B) Converted = H switch
        {
            >= 0 and < 60 => (C, X, 0),
            >= 60 and < 120 => (X, C, 0),
            >= 120 and < 180 => (0, C, X),
            >= 180 and < 240 => (0, X, C),
            >= 240 and < 320 => (X, 0, C),
            >= 320 and < 360 => (C, 0, X),
            _ => throw new Exception("Invalid color")
        };
        this.Red = (byte) Math.Round(255 * (Converted.R + m));
        this.Green = (byte) Math.Round(255 * (Converted.G + m));
        this.Blue = (byte) Math.Round(255 * (Converted.B + m));
    }
}
