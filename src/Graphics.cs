using NativeLibraryLoader;
using odl.SDL2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static odl.SDL2.SDL;
using static odl.SDL2.SDL_image;
using static odl.SDL2.SDL_ttf;

namespace odl;

public static class Graphics
{
    /// <summary>
    /// The list of active windows.
    /// </summary>
    public static List<Window> Windows = new List<Window>();
    /// <summary>
    /// The list of screen rectangles.
    /// </summary>
    public static List<Rect> Screens = new List<Rect>();
    /// <summary>
    /// The list of active renderers.
    /// </summary>
    internal static List<Renderer> Renderers = new List<Renderer>();
    /// <summary>
    /// The maximum size of an SDL_Texture.
    /// </summary>
    public static Size MaxTextureSize;
    /// <summary>
    /// The last used MouseEventArgs object.
    /// </summary>
    public static MouseEventArgs LastMouseEvent = new MouseEventArgs(0, 0, false, false, false, false, false, false, 0, 0);
    /// <summary>
    /// The last custom cursor that was set.
    /// </summary>
    public static IntPtr LastCustomCursor;
    /// <summary>
    /// Whether SDL and its componenents have been initialized.
    /// </summary>
    public static bool Initialized = false;
    /// <summary>
    /// Whether or not the JPEG libary was loaded.
    /// </summary>
    public static bool LoadedJPEG = false;

    private static Platform? _platform;
    /// <summary>
    /// The current OS.
    /// </summary>
    public static Platform Platform
    {
        get
        {
            if (_platform != null) return (Platform)_platform;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) _platform = Platform.Windows;
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX)) _platform = Platform.MacOS;
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux)) _platform = Platform.Linux;
            else _platform = Platform.Unknown;
            return (Platform)_platform;
        }
    }

    /// <summary>
    /// Callbacks that were added from threads other than Main, required for execution on the Main thread.
    /// </summary>
    private static List<Action> ScheduledCallbacks = new List<Action>();

    private static ObjectCollection CurrentObjects;

    public static void MarkObjects()
    {
        CurrentObjects = new ObjectCollection(Windows[0]);
    }

    public static void ShowDifferences()
    {
        if (CurrentObjects == null) return;
        CurrentObjects.CompareWith(new ObjectCollection(Windows[0]));
    }

    delegate bool SetProcessDPIAwareDelegate();
    delegate bool SetProcessDpiAwarenessContextDelegate(long value);
    delegate int SetProcessDpiAwarenessDelegate(long value);

    /// <summary>
    /// Initializes SDL and its components.
    /// </summary>
    public static void Start(PathInfo PathInfo)
    {
        PathPlatformInfo Path = PathInfo.GetPlatform(NativeLibrary.Platform);

        Console.WriteLine("Loading graphical components...");

        SDL.Load(Path.Get("libsdl2"), Path.Get("libz"));
        Console.WriteLine($"Loaded SDL2 ({SDL.Version.major}.{SDL.Version.minor}.{SDL.Version.patch})");

        if (Path.Has("libjpeg"))
        {
            NativeLibrary.Load(Path.Get("libjpeg"));
            LoadedJPEG = true;
        }

        SDL_image.Load(Path.Get("libsdl2_image"), Path.Get("libpng"));
        Console.WriteLine($"Loaded SDL2_image ({SDL_image.Version.major}.{SDL_image.Version.minor}.{SDL_image.Version.patch})");

        SDL_ttf.Load(Path.Get("libsdl2_ttf"), Path.Get("libfreetype"));
        Console.WriteLine($"Loaded SDL2_ttf ({SDL_ttf.Version.major}.{SDL_ttf.Version.minor}.{SDL_ttf.Version.patch})");

        if (NativeLibrary.Platform == NativeLibraryLoader.Platform.Windows)
        {
            // DPI-aware per monitor
            // Windows 10.1607+
            NativeLibrary user32 = NativeLibrary.Load("user32");
            if (user32.HasFunction("SetProcessDpiAwarenessContext"))
            {
                // -1 unaware, -2 aware, -3 per monitor aware, -4 per monitor aware v2 (?), -5 unaware gdi scaled (?)
                bool ret = user32.GetFunction<SetProcessDpiAwarenessContextDelegate>("SetProcessDpiAwarenessContext").Invoke(-4);
                if (ret) Console.WriteLine("Set DPI awareness per monitor v2 (v10.1607+)");
                else Console.WriteLine("Failed to set DPI awareness per monitor v2 (v10.1607+)");
            }
            else
            {
                // DPI-aware per monitor
                // Windows 8.1+
                NativeLibrary shcore = NativeLibrary.Load("shcore");
                if (shcore.HasFunction("SetProcessDpiAwareness"))
                {
                    // 0 unaware, 1 aware, 2 per monitor aware
                    int ret = shcore.GetFunction<SetProcessDpiAwarenessDelegate>("SetProcessDpiAwareness").Invoke(2);
                    if (ret == 0) Console.WriteLine("Set DPI awareness per monitor (v8.1+)");
                    else Console.WriteLine("Failed to set DPI awareness per monitor (v8.1+)");
                }
                else
                {
                    // Global DPI awareness
                    // Windows Vista+
                    if (user32.HasFunction("SetProcessDPIAware"))
                    {
                        // Fully aware; not per monitor.
                        bool ret = user32.GetFunction<SetProcessDPIAwareDelegate>("SetProcessDPIAware").Invoke();
                        if (ret) Console.WriteLine("Set DPI awareness globally");
                        else Console.WriteLine("Failed to set DPI awareness globally");
                    }
                }
            }

        }
        else if (NativeLibrary.Platform != NativeLibraryLoader.Platform.Linux)
        {
            throw new NativeLibrary.UnsupportedPlatformException();
        }

        uint IMG_Flags = IMG_INIT_PNG;
        if (LoadedJPEG) IMG_Flags |= IMG_INIT_JPG;
        if (SDL_Init(SDL_INIT_EVERYTHING) < 0 ||
            IMG_Init(IMG_Flags) != (int)IMG_Flags ||
            TTF_Init() < 0)
        {
            throw new Exception(SDL_GetError());
        }

        int maj, min, pat;
        TTF_GetFreeTypeVersion(out maj, out min, out pat);
        Console.WriteLine($"FreeType ({maj}.{min}.{pat})");

        int screens = SDL_GetNumVideoDisplays();
        for (int i = 0; i < screens; i++)
        {
            SDL_Rect r;
            if (SDL_GetDisplayBounds(i, out r) != 0)
            {
                throw new Exception($"Could not retrieve screen size for screen {i}: {SDL_GetError()}");
            }
            Screens.Add(new Rect(r));
        }
        SDL_StopTextInput();
        Initialized = true;
    }

    /// <summary>
    /// This method schedules callbacks to be run on the main UI thread. This is to be used when inside an awaited method or a different thread,
    /// when you must be sure that the code is running on the UI thread.
    /// </summary>
    /// <param name="Callback">The callback that will be executed on the UI thread.</param>
    public static void Schedule(Action Callback)
    {
        lock (ScheduledCallbacks)
        {
            ScheduledCallbacks.Add(Callback);
        }
    }

    public static Rect GetUsableBounds(int Screen)
    {
        SDL_Rect rect;
        SDL_GetDisplayUsableBounds(Screen, out rect);
        return new Rect(rect);
    }

    public static Bitmap RenderToBitmap(List<Viewport> Viewports, int Width = 0, int Height = 0, int XOffset = 0, int YOffset = 0)
    {
        if (Viewports.Count == 0) return new Bitmap(Width, Height);
        Renderer r = Viewports[0].Renderer;
        return r.RenderViewports(Viewports, Width, Height, XOffset, YOffset);
    }

    public static Bitmap RenderToBitmap(int Width = 0, int Height = 0, int XOffset = 0, int YOffset = 0)
    {
        return RenderToBitmap(Windows[0].Renderer.Viewports, Width, Height, XOffset, YOffset);
    }

    /// <summary>
    /// Updates all renderers.
    /// </summary>
    private static void Render()
    {
        while (true)
        {
            for (int i = 0; i < Renderers.Count; i++)
            {
                Renderers[i].Redraw();
            }
        }
    }

    /// <summary>
    /// Registers a new Window object.
    /// </summary>
    /// <param name="w">The window to register.</param>
    public static void AddWindow(Window w)
    {
        Windows.Add(w);
    }

    /// <summary>
    /// Returns whether a screen exists at the given index.
    /// </summary>
    /// <param name="screen">The screen index to look for.</param>
    public static bool ScreenExists(int screen)
    {
        return screen < Screens.Count;
    }

    /// <summary>
    /// Returns the width of the screen the Window is on.
    /// </summary>
    /// <param name="w">The window to determine the screen of.</param>
    public static int GetWidth(Window w)
    {
        int screen = SDL_GetWindowDisplayIndex(w.SDL_Window);
        return GetWidth(screen);
    }
    /// <summary>
    /// Returns the width of the screen at the given index.
    /// </summary>
    /// <param name="screen">The screen index to find the width of.</param>
    public static int GetWidth(int screen = 0)
    {
        return Screens[screen].Width;
    }

    /// <summary>
    /// Returns the height of the screen the Window is on.
    /// </summary>
    /// <param name="w">The window to determine the height of.</param>
    public static int GetHeight(Window w)
    {
        int screen = SDL_GetWindowDisplayIndex(w.SDL_Window);
        return GetHeight(screen);
    }

    /// <summary>
    /// Opens a URL in the browser.
    /// </summary>
    /// <param name="url">The URL to open.</param>
    public static void OpenURL(string url)
    {
        SDL_OpenURL(url);
    }

    /// <summary>
    /// Returns the height of the screen at the given index.
    /// </summary>
    /// <param name="screen">The screen index to find the height of.</param>
    public static int GetHeight(int screen = 0)
    {
        return Screens[screen].Height;
    }

    private static int OldMouseX = -1;
    private static int OldMouseY = -1;
    private static bool LeftDown = false;
    private static bool MiddleDown = false;
    private static bool RightDown = false;

    /// <summary>
    /// Registers a new Renderer object.
    /// </summary>
    /// <param name="Renderer">The renderer to register.</param>
    internal static void RegisterRenderer(Renderer Renderer)
    {
        Renderers.Add(Renderer);
    }

    /// <summary>
    /// Returns whether there are windows to update.
    /// </summary>
    public static bool CanUpdate()
    {
        for (int i = 0; i < Windows.Count; i++)
        {
            if (Windows[i] != null && !Windows[i].Disposed) return true;
        }
        return false;
    }

    /// <summary>
    /// Delays the main thread for the given timespan.
    /// </summary>
    /// <param name="milliseconds">The number of milliseconds to delay the main thread for.</param>
    public static void Sleep(long milliseconds)
    {
        SDL_Delay(Convert.ToUInt32(milliseconds));
    }

    static bool oldleftdown;
    static bool oldrightdown;
    static bool oldmiddledown;

    internal static void UpdateInput(bool IgnoreErrors = false)
    {
        // Old mouse states
        oldleftdown = LeftDown;
        oldrightdown = RightDown;
        oldmiddledown = MiddleDown;

        // Update all the windows
        for (int i = 0; i < Windows.Count; i++)
        {
            Window w = Windows[i];
            if (w == null) continue;
            if (IgnoreErrors)
            {
                try
                {
                    w.OnTick(new BaseEventArgs());
                    if (w.Focus && (LeftDown || MiddleDown || RightDown))
                    {
                        w.OnMousePress(new MouseEventArgs(OldMouseX, OldMouseY,
                                                          oldleftdown, LeftDown,
                                                          oldrightdown, RightDown,
                                                          oldmiddledown, MiddleDown));
                    }
                }
                catch (Exception) { }
            }
            else
            {
                w.OnTick(new BaseEventArgs());
                if (w.Focus && (LeftDown || MiddleDown || RightDown))
                {
                    w.OnMousePress(new MouseEventArgs(OldMouseX, OldMouseY,
                                                      oldleftdown, LeftDown,
                                                      oldrightdown, RightDown,
                                                      oldmiddledown, MiddleDown));
                }
            }
        }

        // Update button key states
        Input.IterationEnd();
    }

    public static void UpdateGraphics(bool Force = false)
    {
        // Updates the renderers
        for (int i = 0; i < Renderers.Count; i++)
        {
            Renderers[i].Redraw(Force);
        }
    }

    internal static void UpdateWindows()
    {
        // Get events
        SDL_Event e;
        while (SDL_PollEvent(out e) > 0)
        {
            EvaluateEvent(e);
        }
    }

    internal static void EvaluateEvent(SDL_Event e)
    {
        if (e.window.windowID == 0) return;
        IntPtr sdlwindow = SDL_GetWindowFromID(e.window.windowID);
        Window w = Windows.Find(win => win != null && win.SDL_Window == sdlwindow);
        if (w == null) return;
        int idx = Windows.IndexOf(w);
        // After closing a window, there are still a few more events like losing focus;
        // We can skip these as the window was already destroyed.
        if (w == null) return;
        if (OldMouseX == -1) OldMouseX = e.motion.x;
        if (OldMouseY == -1) OldMouseY = e.motion.y;
        if (e.type == SDL_EventType.SDL_WINDOWEVENT)
        {
            switch (e.window.windowevent)
            {
                case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                    BoolEventArgs ClosingArgs = new BoolEventArgs();
                    w.OnClosing?.Invoke(ClosingArgs);
                    if (!ClosingArgs.Value)
                    {
                        SDL_DestroyWindow(w.SDL_Window);
                        w.OnClosed(new BaseEventArgs());
                        w.Dispose();
                        Windows[idx] = null;
                    }
                    break;
                case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
                    w.OnPositionChanged?.Invoke(new BaseEventArgs());
                    break;
                case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                    w.OnSizeChanged?.Invoke(new BaseEventArgs());
                    break;
                case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                    w.OnSizeChanged?.Invoke(new BaseEventArgs());
                    break;
                case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                    w.Focus = true;
                    w.OnFocusGained?.Invoke(new BaseEventArgs());
                    break;
                case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                    w.Focus = false;
                    w.OnFocusLost?.Invoke(new BaseEventArgs());
                    break;
            }
        }
        if (Windows[idx] == null) return; // Just disposed this window
        if (w.Focus)
        {
            switch (e.type)
            {
                case SDL_EventType.SDL_MOUSEMOTION:
                    if (e.motion.x != OldMouseX || e.motion.y != OldMouseY)
                    {
                        LeftDown = (e.button.button & 1) == 1;
                        MiddleDown = (e.button.button & 2) == 2;
                        RightDown = (e.button.button & 4) == 4;
                        w.OnMouseMoving?.Invoke(new MouseEventArgs(e.motion.x, e.motion.y,
                                                           oldleftdown, LeftDown,
                                                           oldrightdown, RightDown,
                                                           oldmiddledown, MiddleDown));
                    }
                    OldMouseX = e.motion.x;
                    OldMouseY = e.motion.y;
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    if (e.button.button == 1) LeftDown = true;
                    if (e.button.button == 2) MiddleDown = true;
                    if (e.button.button == 3) RightDown = true;
                    w.OnMouseDown?.Invoke(new MouseEventArgs(e.motion.x, e.motion.y,
                                                     oldleftdown, LeftDown,
                                                     oldrightdown, RightDown,
                                                     oldmiddledown, MiddleDown));
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    if (e.button.button == 1) LeftDown = false;
                    if (e.button.button == 2) MiddleDown = false;
                    if (e.button.button == 3) RightDown = false;
                    w.OnMouseUp?.Invoke(new MouseEventArgs(e.motion.x, e.motion.y,
                                                   oldleftdown, LeftDown,
                                                   oldrightdown, RightDown,
                                                   oldmiddledown, MiddleDown));
                    break;
                case SDL_EventType.SDL_MOUSEWHEEL:
                    w.OnMouseWheel?.Invoke(new MouseEventArgs(OldMouseX, OldMouseY,
                                                      oldleftdown, LeftDown,
                                                      oldrightdown, RightDown,
                                                      oldmiddledown, MiddleDown,
                                                      e.wheel.x, e.wheel.y));
                    break;
                case SDL_EventType.SDL_KEYDOWN:
                    SDL_Keycode sym1 = e.key.keysym.sym;
                    Input.Register(sym1, true);
                    string txt = "";
                    bool backspace = false;
                    bool delete = false;
                    bool tab = false;
                    if (sym1 == SDL_Keycode.SDLK_RETURN) txt = "\n";
                    if (sym1 == SDL_Keycode.SDLK_BACKSPACE) backspace = true;
                    if (sym1 == SDL_Keycode.SDLK_DELETE) delete = true;
                    if (sym1 == SDL_Keycode.SDLK_TAB) tab = true;
                    if (txt.Length > 0 || backspace || delete || tab)
                    {
                        w.OnTextInput?.Invoke(new TextEventArgs(txt, null, backspace, delete, tab));
                    }
                    break;
                case SDL_EventType.SDL_KEYUP:
                    SDL_Keycode sym2 = e.key.keysym.sym;
                    Input.Register(sym2, false);
                    break;
                case SDL_EventType.SDL_TEXTINPUT:
                    byte[] bytes = new byte[32];
                    unsafe
                    {
                        byte* ptr = e.text.text;
                        byte* data = ptr;
                        int i = 0;
                        while (*data != 0)
                        {
                            bytes[i] = *data;
                            data++;
                            i++;
                        }
                    }
                    string text = "";
                    foreach (char c in Encoding.UTF8.GetChars(bytes)) text += c;
                    w.OnTextInput?.Invoke(new TextEventArgs(text.TrimEnd('\x00'), null));
                    break;
            }
        }
    }

    static int PrevSecond;
    static int Frames;
    static bool _ShowFrames;
    public static bool ShowFrames
    {
        get
        {
            return _ShowFrames;
        }
        set
        {
            _ShowFrames = value;
            if (!_ShowFrames)
            {
                Window w = Windows[0];
                if (w == null || w.Disposed) return;
                string text = w.Text;
                Match m = Regex.Match(text, @"(^.*) - \d+ FPS");
                if (m.Success)
                {
                    text = m.Groups[1].Value;
                    w.SetText(text);
                }
            }
        }
    }

    /// <summary>
    /// Updates input, graphics and the window.
    /// </summary>
    public static void Update(bool IgnoreErrors = false, bool ForceRerender = false)
    {
        UpdateInput(IgnoreErrors);
        UpdateGraphics(ForceRerender);
        UpdateWindows();
        while (ScheduledCallbacks.Count > 0)
        {
            // Remove the callback before calling it, so that if Graphics.Update() happens in our callback, we
            // don't call the callback again in an infinite loop.
            lock (ScheduledCallbacks)
            {
                Action Callback = ScheduledCallbacks[0];
                ScheduledCallbacks.RemoveAt(0);
                Callback();
            }
        }
        if (ShowFrames)
        {
            int CurSecond = DateTime.Now.Second;
            Frames++;
            if (CurSecond != PrevSecond)
            {
                SetFrameCount(Frames);
                Frames = 0;
                PrevSecond = CurSecond;
            }
        }
    }

    static void SetFrameCount(int Frames)
    {
        Window w = Windows[0];
        if (w == null || w.Disposed) return;
        string text = w.Text;
        Match m = Regex.Match(text, @"(^.*) - \d+ FPS");
        if (m.Success)
        {
            text = m.Groups[1].Value;
        }
        w.SetText(text + " - " + Frames.ToString() + " FPS");
    }

    /// <summary>
    /// Disposes SDL and all components.
    /// </summary>
    public static void Stop()
    {
        for (int i = 0; i < Font.Cache.Count; i++)
        {
            Font.Cache[i].Dispose();
        }
        Font.Cache.Clear();
        IMG_Quit();
        SDL_Quit();
        TTF_Quit();
        Initialized = false;
    }
}

public enum Platform
{
    Unknown,
    Windows,
    Linux,
    MacOS,
    IOS,
    Android
}

public enum RenderDriver
{
    Default,
    OpenGL,
    OpenGLES,
    OpenGLES2,
    Direct3D,
    Direct3D11,
    Vulkan,
    Metal,
    Software
}