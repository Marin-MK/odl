using System;
using static SDL2.SDL;

namespace ODL
{
    public class Color
    {
        public static Color WHITE = new Color(255, 255, 255);
        public static Color RED = new Color(255, 0, 0);
        public static Color GREEN = new Color(0, 255, 0);
        public static Color BLUE = new Color(0, 0, 255);
        public static Color BLACK = new Color(0, 0, 0);
        public static Color ALPHA = new Color(255, 255, 255, 0);

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
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte Alpha { get; set; } = 255;

        public Color(byte Red, byte Green, byte Blue, byte Alpha = 255)
        {
            this.Set(Red, Green, Blue, Alpha);
        }

        public void Set(byte Red, byte Green, byte Blue, byte Alpha = 255)
        {
            this.Red = Red;
            this.Green = Green;
            this.Blue = Blue;
            this.Alpha = Alpha;
        }

        public override string ToString()
        {
            return $"(Color: {this.Red},{this.Green},{this.Blue},{this.Alpha})";
        }
    }
}
