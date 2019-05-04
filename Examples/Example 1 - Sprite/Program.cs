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
        Sprite circle;

        public CustomWindow()
        {
            // Set properties of this form that are referenced in Form#initialize()
            // before calling base.Initialize(). This just saves a few extra SDL calls.
            SetSize(400, 400);
            SetText("Custom Form");
            SetIcon("circle.png");

            // Initializes the base form.
            base.Initialize();

            // this.Viewport is bound to this form, meaning local variable sprites
            // or bitmaps will never be disposed by the garbage collector.
            // That means we don't need to remember the local variable unless we
            // want to modify it later on.
            circle = new Sprite(this.Viewport, "circle.png");
            // On-screen location of the sprite
            circle.X = 200;
            circle.Y = 200;
            // Doubles the display width and height
            circle.ZoomX = 2;
            circle.ZoomY = 2;
            // Centers the sprite. The center of the sprite is half the display width.
            // Normally the display width is equal to Bitmap.width, but since we doubled
            // the display width, the center would be Bitmap.width.
            circle.OX = circle.Bitmap.Width;
            circle.OY = circle.Bitmap.Height;
            // Takes the actual bitmap's color and applies this filter over it.
            circle.Color = new Color(255, 0, 255, 100);

            // Add a new OnLoaded event.
            this.OnLoaded += Loaded;

            // Finished initialization of this Form and invokes the OnLoaded event.
            base.Start();
        }

        private void Loaded(object sender, TimeEventArgs e)
        {
            Console.WriteLine("Loaded in " + e.Duration.Milliseconds.ToString() + "ms");
        }
    }
}
