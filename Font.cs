using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL_ttf;

namespace VCS
{
    public class Font
    {
        public string Name { get; set; }
        public int Size { get; set; } = 12;
        public IntPtr SDL_Font { get { return TTF_OpenFont(this.Name + ".ttf", this.Size); } }

        public Font(string Name, int Size)
        {
            this.Name = Name;
            this.Size = Size;
        }

        public Font Clone()
        {
            return new Font(this.Name, this.Size);
        }
    }
}
