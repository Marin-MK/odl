using System;
using System.Linq;
using static SDL2.SDL;

namespace ODL
{
    public class Window
    {
        public Viewport Viewport { get; protected set; }
        public IntPtr SDL_Window { get; protected set; }
        public Renderer Renderer { get; protected set; }

        public Window Parent { get; protected set; }
        public string Text { get; protected set; } = "New Window";
        public Bitmap Icon { get; protected set; }
        public int X { get; protected set; } = -1;
        public int Y { get; protected set; } = -1;
        public int Width { get; protected set; } = 640;
        public int Height { get; protected set; } = 480;
        public bool Resizable { get; protected set; } = true;
        public bool Focus;
        public bool Disposed { get; protected set; } = false;
        public bool Closed { get; protected set; } = false;
        public int Screen { get; protected set; } = 0;

        public Color BackgroundColor { get; protected set; } = new Color(0, 0, 0);
        protected Sprite _BackgroundSprite;

        public EventHandler<TimeEventArgs> OnLoaded;
        public EventHandler<ClosingEventArgs> OnClosing;
        public EventHandler<ClosedEventArgs> OnClosed;
        public EventHandler<MouseEventArgs> OnMouseMoving;
        public EventHandler<MouseEventArgs> OnMouseDown;
        public EventHandler<MouseEventArgs> OnMousePress;
        public EventHandler<MouseEventArgs> OnMouseUp;
        public EventHandler<MouseEventArgs> OnMouseWheel;
        public EventHandler<EventArgs> OnTick;
        public EventHandler<FocusEventArgs> OnFocusGained;
        public EventHandler<FocusEventArgs> OnFocusLost;
        public EventHandler<TextInputEventArgs> OnTextInput;

        private DateTime _StartTime;
        private bool Init = false;

        public Window(Window Parent = null)
        {
            this.Parent = Parent;
            this.OnLoaded = new EventHandler<TimeEventArgs>(Window_Loaded);
            this.OnClosing = new EventHandler<ClosingEventArgs>(Window_Closing);
            this.OnClosed = new EventHandler<ClosedEventArgs>(Window_Closed);
            this.OnMouseMoving = new EventHandler<MouseEventArgs>(Window_MouseMoving);
            this.OnMouseDown = new EventHandler<MouseEventArgs>(Window_MouseDown);
            this.OnMousePress = new EventHandler<MouseEventArgs>(Window_MousePress);
            this.OnMouseUp = new EventHandler<MouseEventArgs>(Window_MouseUp);
            this.OnMouseWheel = new EventHandler<MouseEventArgs>(Window_MouseWheel);
            this.OnTick = new EventHandler<EventArgs>(Window_Tick);
            this.OnFocusGained = new EventHandler<FocusEventArgs>(Window_FocusGained);
            this.OnFocusLost = new EventHandler<FocusEventArgs>(Window_FocusLost);
            this.OnTextInput = new EventHandler<TextInputEventArgs>(Window_TextInput);

            if (this.GetType() == typeof(Window)) { Initialize(); }
        }

        public void Initialize()
        {
            if (Graphics.Windows.Contains(this)) return;
            _StartTime = DateTime.Now;
            SDL_WindowFlags flags = SDL_WindowFlags.SDL_WINDOW_HIDDEN | SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI;
            this.SDL_Window = SDL_CreateWindow(this.Text, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
                this.Width, this.Height, flags);
            int WX;
            int WY;
            SDL_GetWindowPosition(this.SDL_Window, out WX, out WY);
            this.X = WX;
            this.Y = WY;
            SDL_SetWindowResizable(this.SDL_Window, SDL_bool.SDL_TRUE);
            if (this.Icon != null)
            {
                SDL_SetWindowIcon(this.SDL_Window, this.Icon.Surface);
            }
            this.Renderer = new Renderer(SDL_CreateRenderer(this.SDL_Window, -1, SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE));

            this.Viewport = new Viewport(this.Renderer, 0, 0, this.Width, this.Height);
            this.Viewport.Name = "Main Viewport";

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
            this.Width = width;
            this.Height = height;
            if (Initialized())
            {
                SDL_SetWindowSize(this.SDL_Window, width, height);
            }
        }

        public void SetText(string title)
        {
            this.Text = title;
            if (Initialized())
            {
                SDL_SetWindowTitle(this.SDL_Window, title);
            }
        }

        public void SetIcon(string filename)
        {
            Bitmap bmp = new Bitmap(filename);
            this.Icon = bmp;
            if (Initialized()) SetIcon(bmp);
        }
        public void SetIcon(Bitmap bmp)
        {
            if (this.Icon != null) this.Icon.Dispose();
            this.Icon = bmp;
            if (Initialized()) SDL_SetWindowIcon(this.SDL_Window, this.Icon.Surface);
        }

        public void SetPosition(int X, int Y)
        {
            this.X = X + Graphics.Screens[this.Screen].X;
            this.Y = Y + Graphics.Screens[this.Screen].Y;
            if (Initialized())
            {
                int oldscreen = this.Screen;
                SDL_SetWindowPosition(this.SDL_Window, this.X, this.Y);
                this.Screen = SDL_GetWindowDisplayIndex(this.SDL_Window);
                if (this.Screen != oldscreen)
                {
                    this.X -= Graphics.Screens[this.Screen].X;
                    this.Y -= Graphics.Screens[this.Screen].Y;
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
            this.X = Graphics.Screens[screen].X + this.X;
            this.Y = Graphics.Screens[screen].Y + this.Y;
            if (Initialized()) this.SetPosition(this.X, this.Y);
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
            this.Closed = true;
        }
        
        public void Window_Loaded(object sender, TimeEventArgs e) { }

        public void Window_MouseMoving(object sender, MouseEventArgs e) { }

        public void Window_MouseDown(object sender, MouseEventArgs e) { }

        public void Window_MousePress(object sender, MouseEventArgs e) { }

        public void Window_MouseUp(object sender, MouseEventArgs e) { }

        public void Window_MouseWheel(object sender, MouseEventArgs e) { }

        public void Window_Tick(object sender, EventArgs e)
        {
            this.Renderer.Update(false);
        }

        private void Window_FocusGained(object sender, FocusEventArgs e) { }

        private void Window_FocusLost(object sender, FocusEventArgs e) { }

        public void Window_TextInput(object sender, TextInputEventArgs e) { }

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

        public void SetBackgroundColor(byte r, byte g, byte b, byte a = 255)
        {
            SetBackgroundColor(new Color(r, g, b, a));
        }
        public void SetBackgroundColor(Color c)
        {
            this.BackgroundColor = c;
            if (Initialized())
            {
                Sprite bg = this.Renderer.Viewports[1].Sprites[0];
                bg.Bitmap.FillRect(0, 0, bg.Bitmap.Width, bg.Bitmap.Height, c);
            }
        }
    }
}
