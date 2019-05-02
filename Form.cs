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
        protected Viewport _Viewport;
        public Viewport Viewport { get { return _Viewport; } }
        protected IntPtr _SDL_Window;
        public IntPtr SDL_Window { get { return _SDL_Window; } }
        protected Renderer _Renderer;
        public Renderer Renderer { get { return _Renderer; } }

        protected string _Text = "New Form";
        public string Text { get { return _Text; } }
        protected Bitmap _Icon;
        public Bitmap Icon { get { return _Icon; } }
        protected int _X = -1;
        public int X { get { return _X; } }
        protected int _Y = -1;
        public int Y { get { return _Y; } }
        protected int _Width = 640;
        public int Width { get { return _Width; } }
        protected int _Height = 480;
        public int Height { get { return _Height; } }
        protected bool _Resizable = true;
        public bool Resizable { get { return _Resizable; } }
        protected bool _Disposed = false;
        public bool Disposed { get { return _Disposed; } }
        protected bool _Closed = false;
        public bool Closed { get { return _Closed; } }

        protected Color _BackgroundColor = new Color(0, 0, 0);
        protected Sprite _BackgroundSprite;

        public EventHandler<TimeEventArgs> OnLoaded;
        public EventHandler<ClosingEventArgs> OnClosing;
        public EventHandler<ClosedEventArgs> OnClosed;
        public EventHandler<MouseEventArgs> OnMouseMoving;
        public EventHandler<MouseEventArgs> OnMouseDown;
        public EventHandler<MouseEventArgs> OnMouseUp;
        public EventHandler<MouseEventArgs> OnMousePress;
        public EventHandler<TickEventArgs> OnTick;

        private DateTime _StartTime;
        private bool Init = false;

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
            _StartTime = DateTime.Now;
            _SDL_Window = SDL_CreateWindow(this.Text, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
                this.Width, this.Height, SDL_WindowFlags.SDL_WINDOW_HIDDEN | SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI);
            int WX;
            int WY;
            SDL_GetWindowPosition(this.SDL_Window, out WX, out WY);
            _X = WX;
            _Y = WY;
            SDL_SetWindowResizable(this.SDL_Window, SDL_bool.SDL_TRUE);
            if (_Icon != null)
            {
                SDL_SetWindowIcon(this.SDL_Window, _Icon.Surface);
            }
            _Renderer = new Renderer(SDL_CreateRenderer(this.SDL_Window, -1, SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE));

            _Viewport = new Viewport(this.Renderer, 0, 0, this.Width, this.Height);
            _Viewport.Name = "Main Viewport";

            Viewport bgvp = new Viewport(this.Renderer, 0, 0, this.Width, this.Height);
            bgvp.Name = "Background Viewport";
            bgvp.Z = -999999999;
            _BackgroundSprite = new Sprite(bgvp, this.Width, this.Height);
            _BackgroundSprite.Name = "Background";
            _BackgroundSprite.Z = -999999999;
            _BackgroundSprite.Bitmap.FillRect(0, 0, this.Width, this.Height, _BackgroundColor);

            Graphics.Forms.Add(this);
            Init = true;
            if (this.GetType() == typeof(Form)) Start();
        }

        public void Start()
        {
            this.OnLoaded.Invoke(this, new TimeEventArgs(DateTime.Now - _StartTime));
        }

        public bool Initialized()
        {
            return Init;
        }

        public void SetSize(Size s)
        {
            this.SetSize(s.Width, s.Height);
        }
        public void SetSize(int width, int height)
        {
            _Width = width;
            _Height = height;
            if (Initialized())
            {
                SDL_SetWindowSize(this.SDL_Window, width, height);
            }
        }

        public void SetText(string title)
        {
            _Text = title;
            if (Initialized())
            {
                SDL_SetWindowTitle(this.SDL_Window, title);
            }
        }

        public void SetIcon(string filename)
        {
            Bitmap bmp = new Bitmap(filename);
            _Icon = bmp;
            if (Initialized())
            {
                SetIcon(bmp);
            }
        }
        public void SetIcon(Bitmap bmp)
        {
            if (_Icon != null) _Icon.Dispose();
            _Icon = bmp;
            if (Initialized())
            {
                SDL_SetWindowIcon(this.SDL_Window, _Icon.Surface);
            }
        }

        public void Form_Closing(object sender, ClosingEventArgs e)
        {

        }

        public void Form_Closed(object sender, ClosedEventArgs e)
        {
            _Closed = true;
        }

        // Called by Initialize();
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
            _Disposed = true;
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

        public Color GetBackgroundColor()
        {
            return _BackgroundColor;
        }

        public void SetBackgroundColor(byte r, byte g, byte b, byte a = 255)
        {
            SetBackgroundColor(new Color(r, g, b, a));
        }
        public void SetBackgroundColor(Color c)
        {
            _BackgroundColor = c;
            Sprite bg = this.Renderer.Viewports[1].Sprites[0];
            bg.Bitmap.FillRect(0, 0, bg.Bitmap.Width, bg.Bitmap.Height, c);
        }
    }
}
