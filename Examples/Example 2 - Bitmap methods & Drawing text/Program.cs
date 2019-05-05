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

    class CustomWindow : Window
    {
        public CustomWindow()
        {
            // Initializes the base form.
            base.Initialize();

            // Create a new sprite with a bitmap as big as the screen.
            Sprite canvas = new Sprite(this.Viewport, new Size(this.Width, this.Height));

            // Draw a circle at (x,y) = (32,32) with radius 32 (meaning a 64x64 circle)
            canvas.Bitmap.DrawCircle(32, 32, 32, Color.WHITE);

            // Draw a rectangle at (x,y) = (64,0) with (w,h) = (64,64)
            canvas.Bitmap.DrawRect(new Rect(64, 0, 64, 64), Color.WHITE);

            // Draw and fill a rectangle at (x,y) = (128,0) with (w,h) = (64,64)
            canvas.Bitmap.FillRect(new Rect(128, 0, 64, 64), Color.WHITE);

            // Draw a line from (x,y) = (192,0) to (x,y) = (256,64)
            canvas.Bitmap.DrawLine(new Point(192, 0), new Point(256, 64), Color.WHITE);

            // Draw a line from (x,y) = (192,64) to (x,y) = (256,0)
            canvas.Bitmap.DrawLine(new Point(192, 64), new Point(256, 0), Color.WHITE);

            // Load a new font (e.g. "always forever" by Brittney Murphy: https://www.1001freefonts.com/always-forever.font)
            canvas.Bitmap.Font = new Font("always forever", 40);
            // Draw "Hello world!" at (x,y) = (10,80) in white
            // Note: This particular font does not support any special or accented characters.
            //       If such characters don't show up with your font, make sure your font does actually
            //       support/implement those characters to begin with.
            canvas.Bitmap.DrawText("Hello world!", new Point(10, 80), Color.WHITE);

            // Increase the font size despite already having created the font (works with changing font name too)
            canvas.Bitmap.Font.Size = 60;
            canvas.Bitmap.DrawText("These are some draw options.", new Point(10, 160), Color.WHITE,
                DrawOptions.Bold | DrawOptions.Underlined | DrawOptions.Strikethrough | DrawOptions.Italic | DrawOptions.CenterAlign);

            // Finished initialization of this Form and invokes the OnLoaded event.
            base.Start();
        }
    }
}