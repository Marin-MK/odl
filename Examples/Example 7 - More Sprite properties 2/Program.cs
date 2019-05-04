using System;
using System.Collections.Generic;
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

    // Displays random red circles that shrink in size with random sizes
    // and at random positions.
    class CustomWindow : Window
    {
        Random rand = new Random();
        List<Sprite> sprites = new List<Sprite>();

        public CustomWindow()
        {
            // Initializes the base form.
            base.Initialize();

            // Called every frame
            this.OnTick += Tick;

            // Finished initialization of this Form and invokes the OnLoaded event.
            base.Start();
        }

        public void Tick(object sender, EventArgs e)
        {
            // Generate a random number between 0 and 10 and compare it to 2 (20% chance)
            if (rand.Next(0, 10) < 2)
            {
                // Create a new sprite
                Sprite s = new Sprite(this.Viewport);
                // Bitmap size is between 10x10 and 200x200
                int size = rand.Next(10, 200);
                s.Bitmap = new Bitmap(size, size);
                // Draw and fill a circle with a solid color
                s.Bitmap.FillCircle(size / 2, size / 2, size / 2, 255, 0, 0);
                // Center the sprite
                s.OX = s.Bitmap.Width / 2;
                s.OY = s.Bitmap.Height / 2;
                // Set the sprite position to some random location, but it can't go off-screen.
                s.X = rand.Next(0, this.Width - s.Bitmap.Width) + s.OX;
                s.Y = rand.Next(0, this.Height - s.Bitmap.Height) + s.OY;
                // Add it to our collection so we can modify it later
                sprites.Add(s);
            }
            // Iterate over all sprites registered in this collection
            for (int i = 0; i < sprites.Count; i++)
            {
                Sprite s = sprites[i];
                // Make it shrink slightly over time
                s.ZoomX -= 0.01;
                s.ZoomY -= 0.01;
                // If ZoomX or ZoomY go below 0, the sprite becomes invisible so we can get rid of it.
                if (s.ZoomX <= 0)
                {
                    // Especially when doing this many sprite operations, it's absolutely
                    // *crucial* that you dispose your sprites or bitmaps when you're done.
                    s.Dispose();
                    // We have no use for disposed sprites, so remove it from the collection
                    sprites.RemoveAt(i);
                    i--;
                }
            };
        }
    }
}
