using System;
using static SDL2.SDL_ttf;

namespace ODL
{
    public class Font : IDisposable
    {
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; UpdateFont(); } }
        private int _Size;
        public int Size { get { return _Size; } set { _Size = value; UpdateFont(); } }
        public IntPtr SDL_Font { get; private set; }

        public Font(string Name, int Size = 12)
        {
            _Name = Name;
            _Size = Size;
            UpdateFont();
        }

        private void UpdateFont()
        {
            if (this.SDL_Font != null) TTF_CloseFont(this.SDL_Font);
            this.SDL_Font = TTF_OpenFont(this.Name + ".ttf", this.Size);
            if (this.SDL_Font == IntPtr.Zero)
            {
                throw new Exception("Invalid font: '" + this.Name + "'");
            }
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

        public void Dispose()
        {
            TTF_CloseFont(this.SDL_Font);
        }
    }
}
