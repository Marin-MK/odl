using System;
using ODL;
using static SDL2.SDL;

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

    // This example creates a circle and allows you to move it with the arrow keys.
    // Pressing Enter (Return) moves the circle to a random location.
    class CustomWindow : Window
    {
        Random rand = new Random();

        Sprite circle;

        public CustomWindow()
        {
            // Initializes the base form.
            base.Initialize();

            // Create a new sprite with circle.png as bitmap and center it
            circle = new Sprite(this.Viewport, "circle.png");
            circle.OX = circle.Bitmap.Width / 2;
            circle.OY = circle.Bitmap.Height / 2;
            RepositionCircle();

            // Adds an event that happens every single tick/iteration
            this.OnTick += Tick;

            // Finished initialization of this Form and invokes the OnLoaded event.
            base.Start();
        }

        // Main update method that's called thousands of times per second.
        // Avoid heavy processing in here because it might severely impact performance.
        // Make sure that if something doesn't need to be run thousands of times per second, it isn't.
        public void Tick(object sender, EventArgs e)
        {
            // Fires once when the key is pushed down
            if (Input.Trigger(SDL_Keycode.SDLK_RETURN))
            {
                RepositionCircle();
            }
            else if (Input.Press(SDL_Keycode.SDLK_RETURN))
            {
                // Fires for as long as Enter is being held down
            }

            // Important to note here is that these are separate different statements.
            // You can be pressing more than one button at a time. What you want to happen
            // when such a thing occurs is up to you, but in this case, they cancel each other
            // out (x += 1; x -= 1)
            if (Input.Press(SDL_Keycode.SDLK_RIGHT))
            {
                circle.X += 2;
            }
            if (Input.Press(SDL_Keycode.SDLK_LEFT))
            {
                circle.X -= 2;
            }

            if (Input.Press(SDL_Keycode.SDLK_DOWN))
            {
                circle.Y += 2;
            }
            if (Input.Press(SDL_Keycode.SDLK_UP))
            {
                circle.Y -= 2;
            }
        }

        // Repositions the circle sprite based on a random value.
        public void RepositionCircle()
        {
            // Generates a random number between 0 and the width minus the circle's width/height.
            // Since OX = 32, x = 0 means half of the sprite is off-screen. In this case we don't
            // want our sprite to be off screen, so we have to limit the sprite's x at circle.OX (= 32).
            // The same is true for the right edge of the screen, so therefore we subtract half the
            // circle's width again in the random range.
            // With this calculation the sprite will always be fully on-screen no matter what value is generated.
            int x = rand.Next(0, this.Width - circle.Bitmap.Width) + circle.OX;
            int y = rand.Next(0, this.Height - circle.Bitmap.Height) + circle.OY;
            circle.X = x;
            circle.Y = y;
        }
    }
}
