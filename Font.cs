using System;
using static SDL2.SDL_ttf;

namespace ODL
{
    public class Font
    {
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; UpdateFont(); } }
        private int _Size;
        public int Size { get { return _Size; } set { _Size = value; UpdateFont(); } }
        private IntPtr _font;
        public IntPtr SDL_Font { get { return _font; } }

        public Font(string Name, int Size = 12)
        {
            _Name = Name;
            _Size = Size;
            UpdateFont();
        }

        private void UpdateFont()
        {
            if (this.SDL_Font != null) TTF_CloseFont(this.SDL_Font);
            _font = TTF_OpenFont(this.Name + ".ttf", this.Size);
            if (this.SDL_Font == IntPtr.Zero)
            {
                throw new Exception("Invalid font: '" + this.Name + "'");
            }
        }

        public Font Clone()
        {
            return new Font(this.Name, this.Size);
        }
    }
}
