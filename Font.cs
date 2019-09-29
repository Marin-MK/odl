﻿using System;
using System.Collections.Generic;
using static SDL2.SDL_ttf;

namespace ODL
{
    public class Font
    {
        /// <summary>
        /// The internal font cache.
        /// </summary>
        public static List<Font> Cache = new List<Font>();

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
            this.SDL_Font = TTF_OpenFont(this.Name + ".ttf", this.Size);
            if (this.SDL_Font == IntPtr.Zero)
            {
                throw new Exception("Invalid font: '" + this.Name + "'");
            }
            Cache.Add(this);
        }

        /// <summary>
        /// Returns the size the given character would take up when rendered.
        /// </summary>
        /// <param name="Char">The character to find the size of.</param>
        /// <param name="DrawOptions">Additional options for drawing the character.</param>
        public Size TextSize(char Char, DrawOptions DrawOptions = 0)
        {
            IntPtr SDL_Font = this.SDL_Font;
            TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
            int minx, maxx, miny, maxy, adv;
            TTF_GlyphMetrics(SDL_Font, Char, out minx, out maxx, out miny, out maxy, out adv);
            return new Size(maxx - minx, TTF_FontHeight(SDL_Font));
        }
        /// <summary>
        /// Returns the size the given string would take up when rendered.
        /// </summary>
        /// <param name="Char">The string to find the size of.</param>
        /// <param name="DrawOptions">Additional options for drawing the string.</param>
        public Size TextSize(string Text, DrawOptions DrawOptions = 0)
        {
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
    }
}
