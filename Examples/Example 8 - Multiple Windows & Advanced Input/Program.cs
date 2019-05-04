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

    // Creates a button that looks and works exactly like
    // a native Windows UI button, and demonstrates the
    // functionality of multiple windows.
    class CustomWindow : Window
    {
        Sprite button;
        bool button_hover = false;
        bool button_click = false;
        bool? clicked_button = null;

        public CustomWindow()
        {
            // Set this window's size to 200x80
            this.SetSize(200, 80);

            // Set the background color to the typical Windows UI color
            SetBackgroundColor(240, 240, 240);

            // Initializes the base form.
            base.Initialize();

            // Create a new button sprite and set its font.
            button = new Sprite(this.Viewport, 99, 21);
            button.Bitmap.Font = new Font("Arial", 12);
            // Creates the bitmap
            DrawButton();
            // Sets sprite position
            // To properly center a sprite, you don't need to set the OX and OY
            // values per se. You could also just modify the coordinates
            // to center it.
            button.X = this.Width / 2 - button.Bitmap.Width / 2;
            button.Y = this.Height / 2 - button.Bitmap.Height / 2;

            // Called whenever the mouse location changes
            this.OnMouseMoving += MouseMoving;
            // Called once whenever one of the mouse buttons is pressed
            this.OnMouseDown += MouseDown;
            // Called once whenever one of the mouse buttons is released
            this.OnMouseUp += MouseUp;

            // Finished initialization of this Form and invokes the OnLoaded event.
            base.Start();
        }

        private void MouseMoving(object sender, MouseEventArgs e)
        {
            // Stores the previous iteration's value of button_hover
            bool oldhover = button_hover;
            // e.Over() and e.InArea() are the same and determine whether or not
            // the mouse is over a sprite or rectangle.
            button_hover = e.Over(button);
            // If the mouse is now over the sprite or if it just left the sprite area
            if (oldhover != button_hover)
            {
                // Redraw the button graphic
                DrawButton();
            }
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            bool oldclick = button_click;
            // Whether the left mouse button was pressed
            if (e.LeftButton && !e.OldLeftButton)
            {
                button_click = true;
                // We know the left mouse button was pressed,
                // but not if it was pressed when over the button.
                // You don't need to do this per se, but for a proper
                // Windows UI button, this variable does influence
                // the button graphic.
                clicked_button = button_hover;
            }
            // If hovering over this button or the click state changed, redraw the button
            // We could also take this condition out and it'd work exactly the same.
            // By adding a condition to this DrawButton() call though, we make sure
            // that we don't redraw if not necessary.
            if (button_hover && button_click && !oldclick || !button_click && oldclick)
            {
                DrawButton();
            }
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            bool oldclick = button_click;
            // Left Mouse button is released
            if (!e.LeftButton && e.OldLeftButton)
            {
                button_click = false;
                // If we were started our mouse click on the button
                // and we end it there too, the button has been
                // pressed (this is how Windows UI buttons work)
                if (button_hover && clicked_button == true)
                {
                    // Create a new window
                    CustomWindow w = new CustomWindow();
                    // Generate random coordinates, but don't let the window
                    // go off-screen.
                    Random rand = new Random();
                    int rx = rand.Next(0, Graphics.GetWidth(w) - w.Width);
                    int ry = rand.Next(60, Graphics.GetHeight(w) - w.Height);
                    // Set the coordinates
                    w.SetPosition(rx, ry);
                    // Show the window
                    w.Show();
                    // By showing a new window, that new window automatically
                    // takes focus. To keep focus on the current window
                    // (if you want that; here it's purely for example purposes)
                    // call ForceFocus.
                    this.ForceFocus();
                }
                // Reset the clicked_button state
                clicked_button = null;
                // And redraw
                DrawButton();
            }
        }

        // Redraws the button sprite
        public void DrawButton()
        {
            // This clears the bitmap so we get rid of the old content
            // and can draw something new.
            // In this case that's not necessary per se, because we're
            // filling the entire bitmap with a solid color. There can't
            // be any overlap if you fill the entire bitmap, so you don't
            // need to clear it per se.
            button.Bitmap.Clear();
            Color outer = new Color(173, 173, 173);
            Color inner = new Color(225, 225, 225);
            // Conditions to draw the button as being clicked
            if (button_hover && button_click && clicked_button == true)
            {
                outer.Set(0, 84, 153);
                inner.Set(204, 228, 247);
            }
            // Conditions to draw the button as being hovered over
            else if (button_hover && clicked_button == null || clicked_button != null && clicked_button == true)
            {
                outer.Set(0, 120, 215);
                inner.Set(229, 241, 251);
            }
            // Draw a rectangle around the edges of the bitmap
            button.Bitmap.DrawRect(0, 0, button.Bitmap.Width, button.Bitmap.Height, outer);
            // Fill the inside of the bitmap
            button.Bitmap.FillRect(1, 1, button.Bitmap.Width - 2, button.Bitmap.Height - 2, inner);
            // Draw text on the bitmap
            button.Bitmap.DrawText("New window", 14, 4, Color.BLACK);
        }
    }
}
