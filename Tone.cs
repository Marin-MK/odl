using System;
using static SDL2.SDL;

namespace ODL
{
    public class Tone
    {
        /// <summary>
        /// The Red component of the color.
        /// </summary>
        public sbyte Red = 0;
        /// <summary>
        /// The Green component of the color.
        /// </summary>
        public sbyte Green = 0;
        /// <summary>
        /// The Blue component of the color.
        /// </summary>
        public sbyte Blue = 0;
        /// <summary>
        /// The Alpha component of the color.
        /// </summary>
        public byte Gray = 0;

        /// <summary>
        /// Creates a new Color object.
        /// </summary>
        /// <param name="Red">The Red component of the color.</param>
        /// <param name="Green">The Green component of the color.</param>
        /// <param name="Blue">The Blue component of the color.</param>
        /// <param name="Gray">The Grayscale component of the color.</param>
        public Tone(sbyte Red = 0, sbyte Green = 0, sbyte Blue = 0, byte Gray = 0)
        {
            this.Red = Red;
            this.Green = Green;
            this.Blue = Blue;
            this.Gray = Gray;
        }

        public override string ToString()
        {
            return $"(Tone: {this.Red},{this.Green},{this.Blue},{this.Gray})";
        }
    }
}
