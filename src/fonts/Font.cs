using System;
using System.Collections.Generic;
using System.IO;
using static odl.SDL2.SDL_ttf;

namespace odl;

public class Font : IDisposable
{
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
    public nint SDL_Font;

    public Font(string Name, int Size = 12)
    {
        this.Name = Name.Replace('\\', '/');
        this.Size = Size;
        LoadFont();
    }

    /// <summary>
    /// Loads the font.
    /// </summary>
    private void LoadFont()
    {
        SDL_Font = nint.Zero;
        string? filename = ODL.FontResolver.ResolveFilename(this.Name);
        if (filename is null) throw new FileResolverException($"Could not resolve a font file for '{this.Name}'.");
        SDL_Font = TTF_OpenFont(filename, Size + 5);
        if (SDL_Font == nint.Zero) throw new Exception("Invalid font: '" + Name + "'");
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
        nint SDL_Font = this.SDL_Font;
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
        nint SDL_Font = this.SDL_Font;
        TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
        int w, h;
        TTF_SizeText(SDL_Font, Text, out w, out h);
        return new Size(w, h);
    }

    public override bool Equals(object obj)
    {
        if (this == obj) return true;
        if (obj is Font)
        {
            Font f = (Font)obj;
            return Name == f.Name && Size == f.Size;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (int) (SDL_Font.ToInt64() >> 32);
    }

    public void Dispose()
    {
        if (SDL_Font != nint.Zero) TTF_CloseFont(SDL_Font);
        SDL_Font = nint.Zero;
    }
}

public class FontException : Exception
{
    public FontException(string message) : base(message) { }
}