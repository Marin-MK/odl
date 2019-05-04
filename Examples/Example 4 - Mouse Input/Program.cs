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

    // This example makes the circle sprite follow our cursor.
    // If the left button is held down, the cirle will be red.
    // If the right button is held down, the circle will be green.
    // If the middle button is held down, the circle will be blue.

    // Note: if you plan on making a custom cursor, you should not use this
    //       approach of setting a sprite's location equal to the mouse location.
    //       See instead (not C# and therefore needs to be ported first):
    //       https://www.freepascal-meets-sdl.net/a-custom-mouse-cursor/

    class CustomWindow : Window
    {
        Sprite circle;

        public CustomWindow()
        {
            // Initializes the base form.
            base.Initialize();

            // Create a new sprite with circle.png as bitmap and center it
            circle = new Sprite(this.Viewport, "circle.png");
            circle.OX = circle.Bitmap.Width / 2;
            circle.OY = circle.Bitmap.Height / 2;

            // Registers three events.
            // One of three mouse buttons is pressed
            this.OnMouseDown += MouseDown;
            // One of three mouse buttons is released
            this.OnMouseUp += MouseUp;
            // The locatin of the mouse on the screen has moved
            this.OnMouseMoving += MouseMoving;

            // Finished initialization of this Form and invokes the OnLoaded event.
            base.Start();
        }

        // Called whenever the position of the mouse has changed
        public void MouseMoving(object sender, MouseEventArgs e)
        {
            // e.X and e.Y are the position of the mouse relative to the window.
            circle.X = e.X;
            circle.Y = e.Y;
        }

        // Called once whenever the left, middle or right button is pressed.
        public void MouseDown(object sender, MouseEventArgs e)
        {
            UpdateMouseColor(e);
        }

        // Called once whenever the left, middle or right button is released.
        public void MouseUp(object sender, MouseEventArgs e)
        {
            UpdateMouseColor(e);
        }

        // This sets the color of our circle to RED if the LEFT button is down,
        // GREEN if the middle button is down, and BLUE is the right button is down.
        // It's coded so that LEFT is dominant over RIGHT and MIDDLE, RIGHT is dominant over MIDDLE
        // and MIDDLE is dominant over nothing.
        public void UpdateMouseColor(MouseEventArgs e)
        {
            if (e.LeftButton)
            {
                circle.Color = new Color(255, 0, 0);
            }
            else if (e.RightButton)
            {
                circle.Color = new Color(0, 255, 0);
            }
            else if (e.MiddleButton)
            {
                circle.Color = new Color(0, 0, 255);
            }
            else
            {
                circle.Color = Color.WHITE;
            }
        }
    }
}
