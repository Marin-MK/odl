using System;
using VCS;

namespace Examples
{
    class Program
    {
        static void Main(string[] Args)
        {
            // Initializes SDL2
            Graphics.Start();
            // Creates the main form
            CustomForm f = new CustomForm();
            // So long as the main form hasn't been closed, keep updating
            while (!f.Closed)
            {
                // Updates SDL2 and all sprites when necessary
                Graphics.Update();
            }
            // Disposes SDL2
            Graphics.Stop();
        }
    }

    // This example explains viewports and shows how to assign
    // a sprite a bitmap after creation of the sprite.
    // This can also used to re-assign a sprite another bitmap,
    // even if it already had one. Just make sure to dispose the old
    // one before you do this.
    class CustomForm : Form
    {
        public CustomForm()
        {
            // Initializes the base form.
            base.Initialize();

            // Creates a new viewport with (x,y) = (0,0) and (w,h) = (100,100)
            Viewport subvp = new Viewport(this.Renderer, new Rect(0, 0, 100, 100));
            subvp.X = 100;
            subvp.Y = 100;
            // As opposed to setting X and Y immediately after creation, you could also create
            // a Rect(100, 100, 100, 100), meaning (x,y) = (100,100) and (w,h) = (100,100).

            // A sprite cannot display pixels that would go outside a viewport.
            // That means a sprite width a bitmap of 200x200 would only have 100x100 displayed,
            // or even less if its (x,y) aren't (0,0). That's because a sprite's coordinates
            // are relative to its viewport. If the viewport's (x,y) is (0,0) as well as the
            // sprite's, the sprite will be at (0,0).
            // If viewport(x,y) = (100,100) and sprite(x,y) = (0,0), then you'll see the
            // sprite at (x,y) = (100,100).
            Sprite s = new Sprite(subvp);

            // We haven't specified a bitmap or filename while initializing our sprite,
            // so we have to create and assign one now. This is possible with filenames
            // and surfaces too.
            // Creates a new Bitmap with (w,h) = (100, 100)
            Bitmap bmp = new Bitmap(100, 100);

            // Assigns the newly created bitmap to the sprite.
            s.Bitmap = bmp;

            // Fill the bitmap with red
            s.Bitmap.FillRect(0, 0, s.Bitmap.Width, s.Bitmap.Height, Color.RED);

            // So far the sprite displays perfectly fine at (x,y) = (100,100).
            // But now if we set the sprite to (x,y) = (50,50), the sprite will
            // exceed the viewport by 50 pixels in width and height. Those will not
            // be shown on screen.
            // If you comment these two lines, you'll see the full image.
            s.X = 50;
            s.Y = 50;

            // Aside from manipulating the sprite's coordinates, the same thing will happen
            // If you changed the size of the viewport. If we shrink its width by 50px,
            // you'll also see 50px less of the sprite.
            // We haven't touched the height, so the sprite's full height will still be visible.
            //subvp.Width = 50;

            // If you've uncommented both the sprite coordinate and viewport width manipulation,
            // you won't see the sprite at all.
            // Since we set the viewport's width to 50 and the sprite's x to 50, it completely
            // goes out of bounds of the viewport, and you won't see it at all.
            
            // Finished initialization of this Form and invokes the OnLoaded event.
            base.Start();
        }
    }
}
