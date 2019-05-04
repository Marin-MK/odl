using System;
using System.Linq;
using static SDL2.SDL;

namespace ODL
{
    public class Window
    {
        protected Viewport _Viewport;
        public Viewport Viewport { get { return _Viewport; } }
        protected IntPtr _SDL_Window;
        public IntPtr SDL_Window { get { return _SDL_Window; } }
        protected Renderer _Renderer;
        public Renderer Renderer { get { return _Renderer; } }

        protected Window _Parent;
        public Window Parent { get { return _Parent; } }
        protected string _Text = "New Window";
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
        public bool Focus;
        protected bool _Disposed = false;
        public bool Disposed { get { return _Disposed; } }
        protected bool _Closed = false;
        public bool Closed { get { return _Closed; } }
        protected int _Screen = 0;
        public int Screen { get { return _Screen; } }

        protected Color _BackgroundColor = new Color(0, 0, 0);
        public Color BackgroundColor { get { return _BackgroundColor; } }
        protected Sprite _BackgroundSprite;

        public EventHandler<TimeEventArgs> OnLoaded;
        public EventHandler<ClosingEventArgs> OnClosing;
        public EventHandler<ClosedEventArgs> OnClosed;
        public EventHandler<MouseEventArgs> OnMouseMoving;
        public EventHandler<MouseEventArgs> OnMouseDown;
        public EventHandler<MouseEventArgs> OnMousePress;
        public EventHandler<MouseEventArgs> OnMouseUp;
        public EventHandler<EventArgs> OnTick;
        public EventHandler<FocusEventArgs> OnFocusGained;
        public EventHandler<FocusEventArgs> OnFocusLost;

        private DateTime _StartTime;
        private bool Init = false;

        public Window(Window Parent = null)
        {
            _Parent = Parent;
            this.OnLoaded = new EventHandler<TimeEventArgs>(Window_Loaded);
            this.OnClosing = new EventHandler<ClosingEventArgs>(Window_Closing);
            this.OnClosed = new EventHandler<ClosedEventArgs>(Window_Closed);
            this.OnMouseMoving = new EventHandler<MouseEventArgs>(Window_MouseMoving);
            this.OnMouseDown = new EventHandler<MouseEventArgs>(Window_MouseDown);
            this.OnMouseUp = new EventHandler<MouseEventArgs>(Window_MouseUp);
            this.OnMousePress = new EventHandler<MouseEventArgs>(Window_MousePress);
            this.OnTick = new EventHandler<EventArgs>(Window_Tick);
            this.OnFocusGained = new EventHandler<FocusEventArgs>(Window_FocusGained);
            this.OnFocusLost = new EventHandler<FocusEventArgs>(Window_FocusLost);

            if (this.GetType() == typeof(Window)) { Initialize(); }
        }

        public void Initialize()
        {
            if (Graphics.Windows.Contains(this)) return;
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
            _BackgroundSprite.Bitmap.FillRect(0, 0, this.Width, this.Height, this.BackgroundColor);

            Graphics.AddWindow(this);
            Init = true;
            if (this.GetType() == typeof(Window)) Start();
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
            if (Initialized()) SetIcon(bmp);
        }
        public void SetIcon(Bitmap bmp)
        {
            if (_Icon != null) _Icon.Dispose();
            _Icon = bmp;
            if (Initialized()) SDL_SetWindowIcon(this.SDL_Window, _Icon.Surface);
        }

        public void SetPosition(int X, int Y)
        {
            _X = X + Graphics.Screens[this.Screen].X;
            _Y = Y + Graphics.Screens[this.Screen].Y;
            if (Initialized())
            {
                int oldscreen = this.Screen;
                SDL_SetWindowPosition(this.SDL_Window, _X, _Y);
                _Screen = SDL_GetWindowDisplayIndex(this.SDL_Window);
                if (_Screen != oldscreen)
                {
                    _X -= Graphics.Screens[_Screen].X;
                    _Y -= Graphics.Screens[_Screen].Y;
                }
            }
        }

        public void SetScreen(int screen)
        {
            int displaycount = SDL_GetNumVideoDisplays();
            if (screen >= displaycount)
            {
                throw new Exception($"Cannot set window to screen {screen} as it exceeds the screen count.");
            }
            _X = Graphics.Screens[screen].X + this.X;
            _Y = Graphics.Screens[screen].Y + this.Y;
            if (Initialized()) this.SetPosition(_X, _Y);
        }

        public void Show()
        {
            SDL_ShowWindow(this.SDL_Window);
        }

        public void ForceFocus()
        {
            Focus = true;
            SDL_RaiseWindow(this.SDL_Window);
        }

        public void Window_Closing(object sender, ClosingEventArgs e) { }

        public void Window_Closed(object sender, ClosedEventArgs e)
        {
            _Closed = true;
        }
        
        public void Window_Loaded(object sender, TimeEventArgs e) { }

        public void Window_MouseMoving(object sender, MouseEventArgs e) { }

        public void Window_MouseDown(object sender, MouseEventArgs e) { }

        public void Window_MouseUp(object sender, MouseEventArgs e) { }

        public void Window_MousePress(object sender, MouseEventArgs e) { }

        public void Window_Tick(object sender, EventArgs e)
        {
            this.Renderer.Update(false);
        }

        private void Window_FocusGained(object sender, FocusEventArgs e) { }
        private void Window_FocusLost(object sender, FocusEventArgs e) { }

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

        public void SetBackgroundColor(byte r, byte g, byte b, byte a = 255)
        {
            SetBackgroundColor(new Color(r, g, b, a));
        }
        public void SetBackgroundColor(Color c)
        {
            _BackgroundColor = c;
            if (Initialized())
            {
                Sprite bg = this.Renderer.Viewports[1].Sprites[0];
                bg.Bitmap.FillRect(0, 0, bg.Bitmap.Width, bg.Bitmap.Height, c);
            }
        }
    }
}
