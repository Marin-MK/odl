using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using static SDL2.SDL;

namespace odl
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
        public bool IsClosed { get; protected set; } = false;
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
                SDL_WindowFlags flags = (SDL_WindowFlags)SDL_GetWindowFlags(SDL_Window);
                return (flags & SDL_WindowFlags.SDL_WINDOW_MAXIMIZED) == SDL_WindowFlags.SDL_WINDOW_MAXIMIZED;
            }
        }
        /// <summary>
        /// Whether or not the window is minimized.
        /// </summary>
        public bool Minimized
        {
            get
            {
                SDL_WindowFlags flags = (SDL_WindowFlags)SDL_GetWindowFlags(SDL_Window);
                return (flags & SDL_WindowFlags.SDL_WINDOW_MINIMIZED) == SDL_WindowFlags.SDL_WINDOW_MINIMIZED;
            }
        }

        protected Sprite BackgroundSprite;
        protected Viewport BackgroundViewport;
        protected Viewport TopViewport;
        protected Sprite TopSprite;

        /// <summary>
        /// The event called whenever the window has been fully loaded.
        /// </summary>
        public TimespanEvent OnLoaded;
        /// <summary>
        /// The event called whenever the window is about to close.
        /// This event can cancel the close event.
        /// </summary>
        public BoolEvent OnClosing;
        /// <summary>
        /// The event called when the window has been closed.
        /// </summary>
        public BaseEvent OnClosed;
        /// <summary>
        /// The event called when the mouse is moving.
        /// </summary>
        public MouseEvent OnMouseMoving;
        /// <summary>
        /// The event called once one of the mouse buttons is held down.
        /// </summary>
        public MouseEvent OnMouseDown;
        /// <summary>
        /// The event called so long as one of the mouse buttons is held down.
        /// </summary>
        public MouseEvent OnMousePress;
        /// <summary>
        /// The event called once one of the mouse buttons is released.
        /// </summary>
        public MouseEvent OnMouseUp;
        /// <summary>
        ///  The event called so long as the mouse wheel is being scrolled.
        /// </summary>
        public MouseEvent OnMouseWheel;
        /// <summary>
        /// The event called every frame.
        /// </summary>
        public BaseEvent OnTick;
        /// <summary>
        /// The event called when the window gains focus.
        /// </summary>
        public BaseEvent OnFocusGained;
        /// <summary>
        /// The event called when the window loses focus.
        /// </summary>
        public BaseEvent OnFocusLost;
        /// <summary>
        /// The event called when free text input has been enabled.
        /// </summary>
        public TextEvent OnTextInput;
        /// <summary>
        /// The event called when the window has changed size.
        /// </summary>
        public BaseEvent OnSizeChanged;

        private DateTime _StartTime;
        private bool Init = false;

        public Window(Window Parent = null)
        {
            this.Parent = Parent;
            this.OnLoaded = Loaded;
            this.OnClosing = Closing;
            this.OnClosed = Closed;
            this.OnMouseMoving = MouseMoving;
            this.OnMouseDown = MouseDown;
            this.OnMousePress = MousePress;
            this.OnMouseUp = MouseUp;
            this.OnMouseWheel = MouseWheel;
            this.OnTick = Tick;
            this.OnFocusGained = FocusGained;
            this.OnFocusLost = FocusLost;
            this.OnTextInput = TextInput;
            this.OnSizeChanged = SizeChanged;
        }

        /// <summary>
        /// Called to actually create the window and renderer.
        /// </summary>
        public virtual void Initialize(bool HardwareAcceleration = true, bool VSync = false, bool Borderless = false, bool ForceOpenGL = false)
        {
            if (Graphics.Windows.Contains(this)) return;
            _StartTime = DateTime.Now;
            if (ForceOpenGL) SDL_SetHint(SDL_HINT_RENDER_DRIVER, "opengl");
            
            SDL_WindowFlags flags = SDL_WindowFlags.SDL_WINDOW_HIDDEN | SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI;
            if (Borderless) flags |= SDL_WindowFlags.SDL_WINDOW_BORDERLESS;
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
            SDL_RendererFlags renderflags = 0;
            if (HardwareAcceleration) renderflags |= SDL_RendererFlags.SDL_RENDERER_ACCELERATED;
            if (VSync) renderflags |= SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
            this.Renderer = new Renderer(SDL_CreateRenderer(this.SDL_Window, -1, renderflags));
            
            if (Graphics.MaxTextureSize == null)
            {
                SDL_RendererInfo info;
                SDL_GetRendererInfo(this.Renderer.SDL_Renderer, out info);
                Graphics.MaxTextureSize = new Size(info.max_texture_width, info.max_texture_height);
                Console.WriteLine($"Maximum Texture Size: {info.max_texture_width}x{info.max_texture_height}");
            }

            this.Viewport = new Viewport(this, 0, 0, this.Width, this.Height);
            this.Viewport.Name = "Main Viewport";
            if (Viewport.DefaultWindow == null) Viewport.DefaultWindow = this;
            if (Sprite.DefaultViewport == null) Sprite.DefaultViewport = this.Viewport;

            BackgroundViewport = new Viewport(this, 0, 0, this.Width, this.Height);
            BackgroundViewport.Name = "Background Viewport";
            BackgroundViewport.Z = -999999999;
            BackgroundSprite = new Sprite(BackgroundViewport);
            BackgroundSprite.Name = "Background";
            BackgroundSprite.Z = -999999999;
            BackgroundSprite.Bitmap = new SolidBitmap(this.Width, this.Height, this.BackgroundColor);

            TopViewport = new Viewport(this, 0, 0, this.Width, this.Height);
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
        /// Marks the window as fully loaded and launches the OnLoaded event.
        /// </summary>
        public virtual void Start()
        {
            this.OnLoaded(new TimespanEventArgs(DateTime.Now - _StartTime));
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
        public virtual void SetSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Viewport.Width = width;
            this.Viewport.Height = height;
            BackgroundViewport.Width = width;
            BackgroundViewport.Height = height;
            ((SolidBitmap) BackgroundSprite.Bitmap).SetSize(width, height);
            TopViewport.Width = width;
            TopViewport.Height = height;
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
        public virtual void SetMinimumSize(int width, int height)
        {
            SetMinimumSize(new Size(width, height));
        }
        /// <summary>
        /// Sets the minimum size the window can be resized to.
        /// </summary>
        public virtual void SetMinimumSize(Size s)
        {
            this.MinimumSize = s;
            if (Initialized()) SDL_SetWindowMinimumSize(SDL_Window, s.Width, s.Height);
        }

        /// <summary>
        /// Specifies whether the window can be resized.
        /// </summary>
        public virtual void SetResizable(bool Resizable)
        {
            this.Resizable = Resizable;
            if (Initialized()) SDL_SetWindowResizable(this.SDL_Window, Resizable ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
        }

        /// <summary>
        /// Sets the title of the window.
        /// </summary>
        public virtual void SetText(string title)
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
            if (Initialized()) SetIcon(bmp);
        }
        /// <summary>
        /// Sets the icon of the window.
        /// </summary>
        /// <param name="bmp">The bitmap that will be used for the icon.</param>
        public virtual void SetIcon(Bitmap bmp)
        {
            if (this.Icon != null) this.Icon.Dispose();
            this.Icon = bmp;
            if (Initialized()) SDL_SetWindowIcon(this.SDL_Window, this.Icon.Surface);
        }

        /// <summary>
        /// The absolute window position on screen.
        /// </summary>
        public virtual void SetPosition(int X, int Y)
        {
            if (Maximized) return;
            this.X = X + Graphics.Screens[this.Screen].X;
            this.Y = Y + Graphics.Screens[this.Screen].Y;
            if (Initialized())
            {
                SDL_SetWindowPosition(this.SDL_Window, this.X, this.Y);
                this.Screen = SDL_GetWindowDisplayIndex(this.SDL_Window);
                this.X -= Graphics.Screens[this.Screen].X;
                this.Y -= Graphics.Screens[this.Screen].Y;
            }
        }

        /// <summary>
        /// Shows the window on a different screen.
        /// </summary>
        /// <param name="screen">The index of the screen to display the window on.</param>
        public virtual void SetScreen(int screen)
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
        public virtual void Show()
        {
            SDL_ShowWindow(this.SDL_Window);
        }

        /// <summary>
        /// Minimizes the window to the system trey.
        /// </summary>
        public virtual void Minimize()
        {
            if (Initialized()) SDL_MinimizeWindow(SDL_Window);
        }

        /// <summary>
        /// Maximizes the window.
        /// </summary>
        public virtual void Maximize()
        {
            SDL_MaximizeWindow(SDL_Window);
        }

        /// <summary>
        /// Makes the window the active and focused window.
        /// </summary>
        public virtual void ForceFocus()
        {
            Focus = true;
            SDL_RaiseWindow(this.SDL_Window);
        }

        public virtual bool GetVSync()
        {
            SDL_RendererInfo info;
            SDL_GetRendererInfo(this.Renderer.SDL_Renderer, out info);
            SDL_RendererFlags flags = (SDL_RendererFlags)info.flags;
            bool VSync = (flags & SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC) != 0;
            return VSync;
        }

        public virtual void SetVSync(bool VSync)
        {
            SDL_RendererInfo info;
            SDL_GetRendererInfo(this.Renderer.SDL_Renderer, out info);
            SDL_RendererFlags flags = (SDL_RendererFlags) info.flags;
            bool HardwareAcceleration = (flags & SDL_RendererFlags.SDL_RENDERER_ACCELERATED) != 0;
            bool SoftwareRendering = (flags & SDL_RendererFlags.SDL_RENDERER_SOFTWARE) != 0;
            bool TargetTexture = (flags & SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE) != 0;
            bool OldVSync = (flags & SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC) != 0;
            if (OldVSync == VSync) return;
            SDL_RendererFlags newflags = 0;
            if (HardwareAcceleration) newflags |= SDL_RendererFlags.SDL_RENDERER_ACCELERATED;
            if (SoftwareRendering) newflags |= SDL_RendererFlags.SDL_RENDERER_SOFTWARE;
            if (TargetTexture) newflags |= SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE;
            if (VSync) newflags |= SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
            SDL_DestroyRenderer(this.Renderer.SDL_Renderer);
            this.Renderer.SDL_Renderer = SDL_CreateRenderer(this.SDL_Window, -1, newflags);
            foreach (Viewport vp in this.Renderer.Viewports)
            {
                foreach (Sprite s in vp.Sprites)
                {
                    if (s.Bitmap == null) continue;
                    if (s.Bitmap.IsChunky)
                    {
                        foreach (Bitmap bmp in s.Bitmap.InternalBitmaps)
                        {
                            bmp.RecreateTexture();
                        }
                    }
                    else
                    {
                        s.Bitmap.RecreateTexture();
                    }
                }
            }
        }

        public virtual void Closing(BoolEventArgs e) { }
               
        public virtual void Closed(BaseEventArgs e) { this.IsClosed = true; }
               
        public virtual void Loaded(TimespanEventArgs e) { }
               
        public virtual void MouseMoving(MouseEventArgs e) { }
               
        public virtual void MouseDown(MouseEventArgs e) { }
               
        public virtual void MousePress(MouseEventArgs e) { }
               
        public virtual void MouseUp(MouseEventArgs e) { }
               
        public virtual void MouseWheel(MouseEventArgs e) { }
               
        public virtual void Tick(BaseEventArgs e) { }
               
        public virtual void FocusGained(BaseEventArgs e) { }
               
        public virtual void FocusLost(BaseEventArgs e) { }
               
        public virtual void TextInput(TextEventArgs e) { }
                
        public virtual void SizeChanged(BaseEventArgs e)
        {
            UpdateSize();
            this.Viewport.Width = this.Width;
            this.Viewport.Height = this.Height;
            this.BackgroundViewport.Width = this.Width;
            this.BackgroundViewport.Height = this.Height;
            this.TopViewport.Width = this.Width;
            this.TopViewport.Height = this.Height;
            (BackgroundSprite.Bitmap as SolidBitmap).SetSize(this.Width, this.Height);
            (TopSprite.Bitmap as SolidBitmap).SetSize(this.Width, this.Height);
        }

        /// <summary>
        /// Updates the window and renderer every frame.
        /// </summary>
        public virtual void Update()
        {
            this.Renderer.Update();
        }

        /// <summary>
        /// Disposes the window.
        /// </summary>
        public virtual void Dispose()
        {
            this.Renderer.Dispose();
            this.Disposed = true;
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public virtual void Close()
        {
            BoolEventArgs e = new BoolEventArgs();
            this.OnClosing(e);
            if (e.Value)
            {
                this.Dispose();
                SDL_DestroyWindow(this.SDL_Window);
                this.OnClosed(new BaseEventArgs());
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
        public virtual void SetBackgroundColor(Color c)
        {
            this.BackgroundColor = c;
            if (Initialized())
            {
                (BackgroundSprite.Bitmap as SolidBitmap).SetColor(c);
            }
        }

        public virtual Bitmap Screenshot()
        {
            Bitmap bmp = new Bitmap(SDL_CreateRGBSurfaceWithFormat(0, this.Width, this.Height, 32, SDL_PIXELFORMAT_RGBA8888));
            SDL_Rect rect = new SDL_Rect();
            rect.x = 0;
            rect.y = 0;
            rect.w = bmp.Width;
            rect.h = bmp.Height;
            SDL_RenderReadPixels(this.Renderer.SDL_Renderer, ref rect, SDL_PIXELFORMAT_RGBA8888, bmp.SurfaceObject.pixels, bmp.SurfaceObject.pitch);
            return bmp;
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
