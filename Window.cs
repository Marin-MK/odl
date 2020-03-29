using System;
using System.Linq;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace ODL
{
    public class Window : IDisposable
    {
        /// <summary>
        /// The main viewport of the window.
        /// </summary>
        public Viewport Viewport { get; protected set; }
        /// <summary>
        /// The pointer to the SDL_Window object.
        /// </summary>
        public IntPtr SDL_Window { get; protected set; }
        /// <summary>
        /// The renderer of this window.
        /// </summary>
        public Renderer Renderer { get; protected set; }

        /// <summary>
        /// The Parent window of this window, or null if parentless.
        /// </summary>
        public Window Parent { get; protected set; }
        /// <summary>
        /// The title of the window.
        /// </summary>
        public string Text { get; protected set; } = "New Window";
        /// <summary>
        /// The icon of the window.
        /// </summary>
        public Bitmap Icon { get; protected set; }
        /// <summary>
        /// The absolute screen x position of the window.
        /// </summary>
        public int X { get; protected set; } = -1;
        /// <summary>
        /// The absolute screen y position of the window.
        /// </summary>
        public int Y { get; protected set; } = -1;
        /// <summary>
        /// The inner width of the window.
        /// </summary>
        public int Width { get; protected set; } = 640;
        /// <summary>
        /// The inner height of the window.
        /// </summary>
        public int Height { get; protected set; } = 480;
        /// <summary>
        /// Whether or not the window is resizable.
        /// </summary>
        public bool Resizable { get; protected set; } = true;
        /// <summary>
        /// Whether or not the window has focus.
        /// </summary>
        public bool Focus;
        /// <summary>
        /// Whether or not the window has been disposed.
        /// </summary>
        public bool Disposed { get; protected set; } = false;
        /// <summary>
        /// Whether or not the window has been closed.
        /// </summary>
        public bool Closed { get; protected set; } = false;
        /// <summary>
        /// The index of the screen the window is displayed on.
        /// </summary>
        public int Screen { get; protected set; } = 0;
        /// <summary>
        /// The minimum size of the window.
        /// </summary>
        public Size MinimumSize { get; protected set; }
        /// <summary>
        /// The inner background color of the window.
        /// </summary>
        public Color BackgroundColor { get; protected set; } = new Color(0, 0, 0);
        /// <summary>
        /// Whether or not the window is maximized.
        /// </summary>
        public bool Maximized
        {
            get
            {
                SDL_WindowFlags flags = (SDL_WindowFlags) SDL_GetWindowFlags(SDL_Window);
                return (flags & SDL_WindowFlags.SDL_WINDOW_MAXIMIZED) == SDL_WindowFlags.SDL_WINDOW_MAXIMIZED;
            }
        }

        protected Sprite BackgroundSprite;
        protected Viewport BackgroundViewport;
        protected Viewport TopViewport;
        protected Sprite TopSprite;

        /// <summary>
        /// The event called whenever the window has been fully loaded.
        /// </summary>
        public EventHandler<TimeEventArgs> OnLoaded;
        /// <summary>
        /// The event called whenever the window is about to close.
        /// This event can cancel the close event.
        /// </summary>
        public EventHandler<CancelEventArgs> OnClosing;
        /// <summary>
        /// The event called when the window has been closed.
        /// </summary>
        public EventHandler<ClosedEventArgs> OnClosed;
        /// <summary>
        /// The event called when the mouse is moving.
        /// </summary>
        public EventHandler<MouseEventArgs> OnMouseMoving;
        /// <summary>
        /// The event called once one of the mouse buttons is held down.
        /// </summary>
        public EventHandler<MouseEventArgs> OnMouseDown;
        /// <summary>
        /// The event called so long as one of the mouse buttons is held down.
        /// </summary>
        public EventHandler<MouseEventArgs> OnMousePress;
        /// <summary>
        /// The event called once one of the mouse buttons is released.
        /// </summary>
        public EventHandler<MouseEventArgs> OnMouseUp;
        /// <summary>
        ///  The event called so long as the mouse wheel is being scrolled.
        /// </summary>
        public EventHandler<MouseEventArgs> OnMouseWheel;
        /// <summary>
        /// The event called every frame.
        /// </summary>
        public EventHandler<EventArgs> OnTick;
        /// <summary>
        /// The event called when the window gains focus.
        /// </summary>
        public EventHandler<FocusEventArgs> OnFocusGained;
        /// <summary>
        /// The event called when the window loses focus.
        /// </summary>
        public EventHandler<FocusEventArgs> OnFocusLost;
        /// <summary>
        /// The event called when free text input has been enabled.
        /// </summary>
        public EventHandler<TextInputEventArgs> OnTextInput;
        /// <summary>
        /// The event called when the window has been resized.
        /// </summary>
        public EventHandler<WindowEventArgs> OnWindowResized;
        /// <summary>
        /// The event called when the window has changed size.
        /// </summary>
        public EventHandler<WindowEventArgs> OnWindowSizeChanged;

        private DateTime _StartTime;
        private bool Init = false;

        public Window(Window Parent = null)
        {
            this.Parent = Parent;
            this.OnLoaded = new EventHandler<TimeEventArgs>(Window_Loaded);
            this.OnClosing = new EventHandler<CancelEventArgs>(Window_Closing);
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
            this.OnWindowResized = new EventHandler<WindowEventArgs>(Window_Resized);
            this.OnWindowSizeChanged = new EventHandler<WindowEventArgs>(Window_SizeChanged);

            if (this.GetType() == typeof(Window)) { Initialize(); }
        }

        /// <summary>
        /// Called in the constructor to actually create the window and renderer.
        /// </summary>
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
            SDL_SetWindowResizable(this.SDL_Window, Resizable ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
            if (this.Icon != null)
            {
                SDL_SetWindowIcon(this.SDL_Window, this.Icon.Surface);
            }
            this.Renderer = new Renderer(SDL_CreateRenderer(this.SDL_Window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED));
            
            if (Graphics.MaxTextureSize == null)
            {
                SDL_RendererInfo info;
                SDL_GetRendererInfo(this.Renderer.SDL_Renderer, out info);
                Graphics.MaxTextureSize = new Size(info.max_texture_width, info.max_texture_height);
                Console.WriteLine($"Maximum Texture Size: {info.max_texture_width}x{info.max_texture_height}");
            }

            this.Viewport = new Viewport(this.Renderer, 0, 0, this.Width, this.Height);
            this.Viewport.Name = "Main Viewport";

            BackgroundViewport = new Viewport(this.Renderer, 0, 0, this.Width, this.Height);
            BackgroundViewport.Name = "Background Viewport";
            BackgroundViewport.Z = -999999999;
            BackgroundSprite = new Sprite(BackgroundViewport);
            BackgroundSprite.Name = "Background";
            BackgroundSprite.Z = -999999999;
            BackgroundSprite.Bitmap = new SolidBitmap(this.Width, this.Height, this.BackgroundColor);

            TopViewport = new Viewport(this.Renderer, 0, 0, this.Width, this.Height);
            TopViewport.Z = -1;
            TopViewport.Name = "Top Viewport";
            TopSprite = new Sprite(TopViewport, new SolidBitmap(this.Width, this.Height, Color.BLACK));
            TopSprite.Z = 999999999;
            TopSprite.Opacity = 0;

            Graphics.AddWindow(this);

            if (MinimumSize != null) SDL_SetWindowMinimumSize(SDL_Window, MinimumSize.Width, MinimumSize.Height);

            Init = true;
            if (this.GetType() == typeof(Window)) Start();
        }

        /// <summary>
        /// Marks the window as fully loaded and launched the OnLoaded event.
        /// </summary>
        public void Start()
        {
            this.OnLoaded.Invoke(this, new TimeEventArgs(DateTime.Now - _StartTime));
        }

        /// <summary>
        /// Returns whether or not the actual window and renderer have been created yet.
        /// </summary>
        /// <returns></returns>
        public bool Initialized()
        {
            return Init;
        }

        /// <summary>
        /// Resizes the window.
        /// </summary>
        /// <param name="s">The new window size</param>
        public void SetSize(Size s)
        {
            this.SetSize(s.Width, s.Height);
        }
        /// <summary>
        /// Resizes the window.
        /// </summary>
        /// <param name="s">The new window size</param>
        public void SetSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            if (Initialized())
            {
                SDL_SetWindowSize(this.SDL_Window, width, height);
            }
        }
        private void UpdateSize()
        {
            int w, h;
            SDL_GetWindowSize(this.SDL_Window, out w, out h);
            this.Width = w;
            this.Height = h;
        }

        /// <summary>
        /// Sets the minimum size the window can be resized to.
        /// </summary>
        public void SetMinimumSize(int width, int height)
        {
            SetMinimumSize(new Size(width, height));
        }
        /// <summary>
        /// Sets the minimum size the window can be resized to.
        /// </summary>
        public void SetMinimumSize(Size s)
        {
            this.MinimumSize = s;
            if (Initialized()) SDL_SetWindowMinimumSize(SDL_Window, s.Width, s.Height);
        }

        /// <summary>
        /// Specifies whether the window can be resized.
        /// </summary>
        public void SetResizable(bool Resizable)
        {
            this.Resizable = Resizable;
            if (Initialized()) SDL_SetWindowResizable(this.SDL_Window, Resizable ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
        }

        /// <summary>
        /// Sets the title of the window.
        /// </summary>
        public void SetText(string title)
        {
            this.Text = title;
            if (Initialized())
            {
                SDL_SetWindowTitle(this.SDL_Window, title);
            }
        }

        /// <summary>
        /// Sets the icon of the window.
        /// </summary>
        /// <param name="filename">The file to load as the icon.</param>
        public void SetIcon(string filename)
        {
            Bitmap bmp = new Bitmap(filename);
            this.Icon = bmp;
            if (Initialized()) SetIcon(bmp);
        }
        /// <summary>
        /// Sets the icon of the window.
        /// </summary>
        /// <param name="bmp">The bitmap that will be used for the icon.</param>
        public void SetIcon(Bitmap bmp)
        {
            if (this.Icon != null) this.Icon.Dispose();
            this.Icon = bmp;
            if (Initialized()) SDL_SetWindowIcon(this.SDL_Window, this.Icon.Surface);
        }

        /// <summary>
        /// The absolute window position on screen.
        /// </summary>
        public void SetPosition(int X, int Y)
        {
            if (Maximized) return;
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

        /// <summary>
        /// Shows the window on a different screen.
        /// </summary>
        /// <param name="screen">The index of the screen to display the window on.</param>
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

        /// <summary>
        /// Shows the window if it had been hidden.
        /// </summary>
        public void Show()
        {
            SDL_ShowWindow(this.SDL_Window);
        }

        /// <summary>
        /// Makes the window the active and focused window.
        /// </summary>
        public void ForceFocus()
        {
            Focus = true;
            SDL_RaiseWindow(this.SDL_Window);
        }

        public void Window_Closing(object sender, CancelEventArgs e) { }

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
            
        }

        private void Window_FocusGained(object sender, FocusEventArgs e) { }

        private void Window_FocusLost(object sender, FocusEventArgs e) { }

        public void Window_TextInput(object sender, TextInputEventArgs e) { }

        private void Window_SizeChanged(object sender, WindowEventArgs e)
        {
            this.Viewport.Width = e.Width;
            this.Viewport.Height = e.Height;
            this.BackgroundViewport.Width = e.Width;
            this.BackgroundViewport.Height = e.Height;
            this.TopViewport.Width = e.Width;
            this.TopViewport.Height = e.Height;
            (BackgroundSprite.Bitmap as SolidBitmap).SetSize(this.Width, this.Height);
            (TopSprite.Bitmap as SolidBitmap).SetSize(this.Width, this.Height);
            UpdateSize();
        }

        private void Window_Resized(object sender, WindowEventArgs e) { }

        /// <summary>
        /// Updates the window and renderer every frame.
        /// </summary>
        public void Update()
        {
            this.Renderer.Update();
        }

        /// <summary>
        /// Disposes the window.
        /// </summary>
        public void Dispose()
        {
            this.Renderer.Dispose();
            this.Disposed = true;
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void Close()
        {
            CancelEventArgs e = new CancelEventArgs();
            this.OnClosing.Invoke(this, e);
            if (!e.Cancel)
            {
                this.Dispose();
                SDL_DestroyWindow(this.SDL_Window);
                this.OnClosed.Invoke(this, new ClosedEventArgs());
            }
        }

        /// <summary>
        /// Sets the inner background color for the window.
        /// </summary>
        public void SetBackgroundColor(byte r, byte g, byte b, byte a = 255)
        {
            SetBackgroundColor(new Color(r, g, b, a));
        }
        /// <summary>
        /// Sets the inner background color for the window.
        /// </summary>
        public void SetBackgroundColor(Color c)
        {
            this.BackgroundColor = c;
            if (Initialized())
            {
                (BackgroundSprite.Bitmap as SolidBitmap).SetColor(c);
            }
        }
    }

    /// <summary>
    /// Exception for unsupported methods in a class.
    /// </summary>
    public class MethodNotSupportedException : Exception
    {
        public MethodNotSupportedException(object o)
            : base($"This method is not supported in an object of type {o.GetType()}.") { }
    }
}
