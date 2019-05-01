using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCS
{
    public class Size
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Size(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }

        public override string ToString()
        {
            return $"(Size: {this.Width},{this.Height})";
        }
    }
}
