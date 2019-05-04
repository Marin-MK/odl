using System;
using ODL;

namespace Examples
{
    class Program
    {
        static void Main(string[] Args)
        {
            // Initializes SDL2
            Graphics.Start();
            // Creates the main form
            CustomWindow w = new CustomWindow();
            w.Show();
            // So long as there are unclosed windows, keep updating
            while (Graphics.CanUpdate())
            {
                // Updates SDL2 and all sprites when necessary
                Graphics.Update();
            }
            // Disposes SDL2
            Graphics.Stop();
        }
    }


    // Demonstrates the Sprite#Angle, Sprite#MirrorX
    // and Sprite#MirrorY properties.
    // Also explains all the other methods I haven't covered yet.
    class CustomWindow : Window
    {
        Sprite line;

        public CustomWindow()
        {
            // Initializes the base form.
            base.Initialize();

            // Create a new sprite
            line = new Sprite(this.Viewport);
            // With a 4x100 bitmap
            line.Bitmap = new Bitmap(4, 100);
            // Make it red
            line.Bitmap.FillRect(0, 0, 4, 100, Color.RED);
            // Center the sprite
            line.OX = 2;
            line.OY = 50;
            // Move it to the center
            line.X = this.Width / 2;
            line.Y = this.Height / 2;
            // Z order determines which sprite is displayed on top.
            // this line sprite would appear above the text sprite
            // if they were to overlap, due to this Z value being
            // higher than that of text. The default Z value is 0.
            line.Z = 2;

            Sprite text = new Sprite(this.Viewport, this.Width, this.Height);
            text.Z = 1;
            // Create a new font
            text.Bitmap.Font = new Font("always forever", 40);
            // Draw text at (x,y) = (10,10)
            text.Bitmap.DrawText("This is what happens when you mirror a bitmap!", 10, 10, Color.WHITE);
            // Mirrors the sprite on the horizontal axis.
            text.MirrorX = true;
            // Mirror the sprite on the vertical axis.
            text.MirrorY = true;

            // We drew at (x,y) = (10,10), but since we mirror the sprite both
            // horizontally and vertically, the text shows up in the bottom right.

            // There are a few other methods I haven't covered in any of the examples yet:
            // - Bitmap#Build(Rect Destination, Bitmap Source, Rect SourceRect)
            //   This copies the Source Rectangle from the Source bitmap to this Bitmap's Destination Rectangle.
            // - Bitmap#GetPixel(int X, int Y)
            //   This returns the Color value that corresponds with the given coordinates.
            // - Bitmap#SetPixel(int X, int Y, Color c)
            //   This sets the Color value of the given pixel to the given color.
            // - Bitmap#Clear()
            //   Completely clears the bitmap and all pixels on it.
            // - Bitmap#Dispose()
            //   Disposes the bitmap. Always do this when you're done with a Bitmap.
            // - Sprite#Dispose()
            //   Dispose the sprite. Always do this when you're done with a Sprite.
            //   This method also disposes its Bitmap, so just calling Sprite#Dispose() is fine.

            // Call Tick every tick.
            this.OnTick += Tick;

            // Finished initialization of this Form and invokes the OnLoaded event.
            base.Start();
        }

        public void Tick(object sender, EventArgs e)
        {
            // Every tick, change the angle of the line.
            line.Angle += 1;
        }
    }
}
