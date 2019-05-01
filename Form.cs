using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace VCS
{
    public class Form
    {
        public Viewport Viewport { get; set; }
        public IntPtr SDL_Window { get; set; }
        public Renderer Renderer { get; set; }


        public string Text { get; set; } = "New Form";
        public int X { get; set; } = -1;
        public int Y { get; set; } = -1;
        public int Width { get; set; } = 640;
        public int Height { get; set; } = 480;
        public bool Resizable { get; set; } = true;
        public bool Disposed { get; set; } = false;
        public bool Closed { get; set; } = false;

        public EventHandler<TimeEventArgs> OnLoaded;
        public EventHandler<ClosingEventArgs> OnClosing;
        public EventHandler<ClosedEventArgs> OnClosed;
        public EventHandler<MouseEventArgs> OnMouseMoving;
        public EventHandler<MouseEventArgs> OnMouseDown;
        public EventHandler<MouseEventArgs> OnMouseUp;
        public EventHandler<MouseEventArgs> OnMousePress;
        public EventHandler<TickEventArgs> OnTick;

        public Form()
        {
            this.OnLoaded = new EventHandler<TimeEventArgs>(Form_Loaded);
            this.OnClosing = new EventHandler<ClosingEventArgs>(Form_Closing);
            this.OnClosed = new EventHandler<ClosedEventArgs>(Form_Closed);
            this.OnMouseMoving = new EventHandler<MouseEventArgs>(Form_MouseMoving);
            this.OnMouseDown = new EventHandler<MouseEventArgs>(Form_MouseDown);
            this.OnMouseUp = new EventHandler<MouseEventArgs>(Form_MouseUp);
            this.OnMousePress = new EventHandler<MouseEventArgs>(Form_MousePress);
            this.OnTick = new EventHandler<TickEventArgs>(Form_Tick);

            if (this.GetType() == typeof(Form)) { Initialize(); }
        }

        public void Initialize()
        {
            if (Graphics.Forms.Contains(this)) return;
            DateTime Start = DateTime.Now;
            this.SDL_Window = SDL_CreateWindow(this.Text, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
                this.Width, this.Height, SDL_WindowFlags.SDL_WINDOW_HIDDEN | SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI);
            int WX;
            int WY;
            SDL_GetWindowPosition(this.SDL_Window, out WX, out WY);
            this.X = WX;
            this.Y = WY;
            SDL_SetWindowResizable(this.SDL_Window, SDL_bool.SDL_TRUE);
            this.Renderer = new Renderer(SDL_CreateRenderer(this.SDL_Window, -1, SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE));

            this.Viewport = new Viewport(this.Renderer, this.Width, this.Height);
            this.Viewport.Name = "Main Viewport";

            Viewport bgvp = new Viewport(this.Renderer, this.Width, this.Height);
            bgvp.Name = "Background Viewport";
            bgvp.Z = -999999999;
            Sprite bg = new Sprite(bgvp, this.Width, this.Height);
            bg.Name = "Background";
            bg.Z = -999999999;
            bg.Bitmap.FillRect(0, 0, this.Width, this.Height, 0, 0, 0);

            Graphics.Forms.Add(this);
            this.OnLoaded.Invoke(this, new TimeEventArgs(DateTime.Now - Start));
        }

        public void Form_Closing(object sender, ClosingEventArgs e)
        {
            
        }

        public void Form_Closed(object sender, ClosedEventArgs e)
        {
            this.Closed = true;
        }

        public void Form_Loaded(object sender, TimeEventArgs e)
        {
            SDL_ShowWindow(this.SDL_Window);
        }

        public void Form_MouseMoving(object sender, MouseEventArgs e)
        {
            
        }

        public void Form_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        public void Form_MouseUp(object sender, MouseEventArgs e)
        {
            
        }

        public void Form_MousePress(object sender, MouseEventArgs e)
        {

        }

        public void Form_Tick(object sender, TickEventArgs e)
        {
            this.Renderer.Update(false);
        }

        public void Update()
        {
            this.Renderer.Update();
        }

        public void Dispose()
        {
            this.Renderer.Dispose();
            this.Disposed = true;
        }

        public void Close()
        {
            ClosingEventArgs e = new ClosingEventArgs();
            this.OnClosing.Invoke(this, e);
            if (!e.Cancel)
            {
                SDL_DestroyWindow(this.SDL_Window);
                this.OnClosed.Invoke(this, new ClosedEventArgs());
            }
        }
    }
}
