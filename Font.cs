using System;
using System.Collections.Generic;
using System.IO;
using static odl.SDL2.SDL_ttf;

namespace odl;

public class Font : IDisposable
{
    /// <summary>
    /// The internal font cache.
    /// </summary>
    public static List<Font> Cache = new List<Font>();

    /// <summary>
    /// An optional path to the folder to look for fonts in.
    /// </summary>
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
        this.Name = Name;
        this.Size = Size;
        LoadFont();
        Cache.Add(this);
    }

    /// <summary>
    /// Loads the font.
    /// </summary>
    private void LoadFont()
    {
        SDL_Font = IntPtr.Zero;
        if (!Name.EndsWith(".ttf") || !File.Exists(Name))
        {
            if (File.Exists(Name + ".ttf")) Name += ".ttf";
            else if (!string.IsNullOrEmpty(FontPath) && Name.EndsWith(".ttf") && File.Exists(Path.Combine(FontPath, Name))) Name = Path.Combine(FontPath, Name);
            else if (!string.IsNullOrEmpty(FontPath) && File.Exists(Path.Combine(FontPath, Name + ".ttf"))) Name = Path.Combine(FontPath, Name + ".ttf");
        }
        SDL_Font = TTF_OpenFont(Name, Size);
        if (SDL_Font == IntPtr.Zero) throw new Exception("Invalid font: '" + Name + "'");
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
    /// Fetches or creates a font with the given parameters.
    /// </summary>
    /// <param name="Name">The name of the font.</param>
    /// <param name="Size">The size of the font.</param>
    public static Font Get(string Name, int Size)
    {
        for (int i = 0; i < Cache.Count; i++)
        {
            Font f = Cache[i];
            if (f.Size != Size) continue;
            if (f.Name == Name || f.Name == Name + ".ttf" ||
                !string.IsNullOrEmpty(FontPath) &&
                (f.Name == Path.Combine(FontPath, Name) || f.Name == Path.Combine(FontPath, Name + ".ttf")))
                return f;
        }
        return new Font(Name, Size);
    }

    /// <summary>
    /// Returns whether a font exists at the specified path.
    /// </summary>
    /// <param name="Name">The name of the font to look for.</param>
    /// <returns>Whether a font exists at that location.</returns>
    public static bool Exists(string Name)
    {
        if (Name.EndsWith(".ttf") && File.Exists(Name)) return true;
        else if (File.Exists(Name + ".ttf")) return true;
        else if (!string.IsNullOrEmpty(FontPath) && Name.EndsWith(".ttf") && File.Exists(Path.Combine(FontPath, Name))) return true;
        else if (!string.IsNullOrEmpty(FontPath) && File.Exists(Path.Combine(FontPath, Name + ".ttf"))) return true;
        return false;
    }

    public void Dispose()
    {
        if (SDL_Font != IntPtr.Zero) TTF_CloseFont(SDL_Font);
        SDL_Font = IntPtr.Zero;
    }
}
