using System;
using static odl.SDL2.SDL;

namespace odl
{
    public class Tone : IDisposable
    {
        /// <summary>
        /// The Red component of the color.
        /// </summary>
        public short Red = 0;
        /// <summary>
        /// The Green component of the color.
        /// </summary>
        public short Green = 0;
        /// <summary>
        /// The Blue component of the color.
        /// </summary>
        public short Blue = 0;
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
        public Tone(short Red = 0, short Green = 0, short Blue = 0, byte Gray = 0)
        {
            this.Red = Math.Max((short) -255, Math.Min((short) 255, Red));
            this.Green = Math.Max((short) -255, Math.Min((short) 255, Green));
            this.Blue = Math.Max((short) -255, Math.Min((short) 255, Blue));
            this.Gray = Gray;
        }

        public Tone Clone()
        {
            return new Tone(this.Red, this.Green, this.Blue, this.Gray);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"(Tone: {this.Red},{this.Green},{this.Blue},{this.Gray})";
        }
    }
}
