using System;
using System.Collections.Generic;
using static SDL2.SDL_ttf;

namespace ODL
{
    public class Font
    {
        public static List<Font> Cache = new List<Font>();

        public string Name { get; protected set; }
        public int Size { get; protected set; }
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

        public Size TextSize(char Char, DrawOptions DrawOptions = 0)
        {
            return this.TextSize(Char.ToString(), DrawOptions);
        }
        public Size TextSize(string Text, DrawOptions DrawOptions = 0)
        {
            IntPtr SDL_Font = this.SDL_Font;
            TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
            int w, h;
            TTF_SizeText(SDL_Font, Text, out w, out h);
            return new Size(w, h);
        }

        public Font Clone()
        {
            return new Font(this.Name, this.Size);
        }

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
