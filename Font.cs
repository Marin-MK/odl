using System;
using System.Collections.Generic;
using static odl.SDL2.SDL_ttf;

namespace odl;

public class Font : IDisposable
{
    /// <summary>
    /// The internal font cache.
    /// </summary>
    public static List<Font> Cache = new List<Font>();

    public static string FontPath = null;

    /// <summary>
    /// The name of the font.
    /// </summary>
    public string Name { get; protected set; }
    /// <summary>
    /// The size of the font.
    /// </summary>
    public int Size { get; protected set; }
    /// <summary>
    /// The pointer to the SDL_Font object.
    /// </summary>
    public IntPtr SDL_Font { get; private set; }

    public Font(string Name, int Size = 12)
    {
        SetName(Name, false);
        SetSize(Size, false);
        ReloadFont();
        Cache.Add(this);
    }

    public void SetName(string Name, bool Reload = true)
    {
        if (this.Name != Name)
        {
            this.Name = Name;
            if (Reload) ReloadFont();
        }
    }

    public void SetSize(int Size, bool Reload = true)
    {
        if (this.Size != Size)
        {
            this.Size = Size;
            if (Reload) ReloadFont();
        }
    }

    public void ReloadFont()
    {
        if (SDL_Font != IntPtr.Zero) TTF_CloseFont(SDL_Font);
        SDL_Font = IntPtr.Zero;
        if (System.IO.File.Exists(this.Name + ".ttf")) this.SDL_Font = TTF_OpenFont(this.Name + ".ttf", this.Size);
        if (this.SDL_Font == IntPtr.Zero)
        {
            throw new Exception("Invalid font: '" + this.Name + "'");
        }
    }

    /// <summary>
    /// Returns the size the given character would take up when rendered.
    /// </summary>
    /// <param name="Char">The character to find the size of.</param>
    /// <param name="DrawOptions">Additional options for drawing the character.</param>
    public Size TextSize(char Char, DrawOptions DrawOptions = 0)
    {
        if (Char == ' ')
        {
            Size total = TextSize("a a", DrawOptions);
            Size nospace = TextSize("aa", DrawOptions);
            return new Size(total.Width - nospace.Width, total.Height - nospace.Height);
        }
        IntPtr SDL_Font = this.SDL_Font;
        TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
        int minx, maxx, miny, maxy, adv;
        TTF_GlyphMetrics(SDL_Font, Char, out minx, out maxx, out miny, out maxy, out adv);
        return new Size(adv, TTF_FontHeight(SDL_Font));
    }
    /// <summary>
    /// Returns the size the given string would take up when rendered.
    /// </summary>
    /// <param name="Char">The string to find the size of.</param>
    /// <param name="DrawOptions">Additional options for drawing the string.</param>
    public Size TextSize(string Text, DrawOptions DrawOptions = 0)
    {
        if (Text.Length == 1) return TextSize(Convert.ToChar(Text), DrawOptions);
        IntPtr SDL_Font = this.SDL_Font;
        TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
        int w, h;
        TTF_SizeText(SDL_Font, Text, out w, out h);
        return new Size(w, h);
    }

    /// <summary>
    /// Creates a copy of the Font object.
    /// </summary>
    public Font Clone()
    {
        return new Font(this.Name, this.Size);
    }

    /// <summary>
    /// Fetches or creates a font with the given parameters.
    /// </summary>
    /// <param name="Name">The name of the font.</param>
    /// <param name="Size">The size of the font.</param>
    public static Font Get(string Name, int Size)
    {
        for (int i = 0; i < Cache.Count; i++)
        {
            Font f = Cache[i];
            if (f.Name == Name && f.Size == Size) return f;
        }
        return new Font(Name, Size);
    }

    public static bool Exists(string Name)
    {
        if (System.IO.File.Exists(Name + ".ttf")) return true;
        else if (!string.IsNullOrEmpty(FontPath) && System.IO.File.Exists(FontPath + "/" + Name + ".ttf")) return true;
        return false;
    }

    public void Dispose()
    {

    }
}
