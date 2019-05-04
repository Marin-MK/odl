using System;
using System.Collections;
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

    // Demonstrates how to work with windows and multiple screens.
    class CustomWindow : Window
    {
        public CustomWindow()
        {
            // Initializes the base form.
            base.Initialize();

            // Moves this window to (x,y) = (50, 50)
            // Positions are relative to the screen they're on.
            this.SetPosition(50, 50);
            if (Graphics.ScreenExists(1))
            {
                // Moves this window to the second screen
                // (x,y) = (50,50) will now apply to the second monitor instead of the first
                // Note: will not work if you have only 1 screen.
                //       Always use Graphics.ScreenExists(idx) beforehand to check if the
                //       screen you want to use actually exists.
                // Windows always start on the first screen, no matter how many
                // screens you have.
                this.SetScreen(1);
            }

            // Gets the width of the screen this window is currently on
            int screenwidth = Graphics.GetWidth(this);
            // Gets the height of the screen this window is currently on
            int screenheight = Graphics.GetHeight(this);

            // Sets this window position to the bottom right of the
            // second screen if present, otherwise the first screen.
            this.SetPosition(screenwidth - this.Width, screenheight - this.Height);

            // Pro tip:
            // Some people have their task bar at the top of their screen.
            // Setting Y to 0 makes it impossible for them to move the window
            // without either moving their task bar or using some other shortcuts
            // of maximizing or moving the window.

            // Finished initialization of this Form and invokes the OnLoaded event.
            base.Start();
        }
    }
}
