using odl.SDL2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static odl.SDL2.SDL;
using static odl.SDL2.SDL_image;
using static odl.SDL2.SDL_ttf;

namespace odl
{
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
        public static List<Renderer> Renderers = new List<Renderer>();
        /// <summary>
        /// The maximum size of an SDL_Texture.
        /// </summary>
        public static Size MaxTextureSize;
        /// <summary>
        /// Whether the debug log should be printed to the console.
        /// </summary>
        public static bool DebugLog = false;
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
                if (_platform != null) return (Platform) _platform;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) _platform = Platform.Windows;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) _platform = Platform.MacOS;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) _platform = Platform.Linux;
                else _platform = Platform.Unknown;
                return (Platform) _platform;
            }
        }

        private static ObjectCollection CurrentObjects;

        public static void MarkObjects()
        {
            CurrentObjects = new ObjectCollection(Renderers[0]);
        }

        public static void ShowDifferences()
        {
            if (CurrentObjects == null) return;
            CurrentObjects.CompareWith(new ObjectCollection(Renderers[0]));
        }
        
        // Windows DPI awareness
        [DllImport("user32")]
        public static extern bool SetProcessDPIAware();

        /// <summary>
        /// Initializes SDL and its components.
        /// </summary>
        public static void Start()
        {
            Console.WriteLine("Start loading dependencies...");
            if (Platform == Platform.Windows)
            {
                SetProcessDPIAware();
                SDL.Bind("./lib/windows/SDL2.dll", "./lib/windows/zlib1.dll");
                if (File.Exists("./lib/windows/libjpeg-9.dll"))
                {
                    LoadedJPEG = true;
                    SDL_image.Bind("./lib/windows/SDL2_image.dll", "./lib/windows/libpng16-16.dll", "./lib/windows/libjpeg-9.dll");
                }
                else SDL_image.Bind("./lib/windows/SDL2_image.dll", "./lib/windows/libpng16-16.dll");
                SDL_ttf.Bind("./lib/windows/SDL2_ttf.dll", "./lib/windows/libfreetype-6.dll");
            }
            else if (Platform == Platform.Linux)
            {
                SDL.Bind("./lib/linux/SDL2.so", "./lib/linux/libz.so");
                if (File.Exists("./lib/linux/libjpeg-9.so"))
                {
                    LoadedJPEG = true;
                    SDL_image.Bind("./lib/linux/SDL2_image.so", "./lib/linux/libpng16-16.so", "./lib/linux/libjpeg-9.so");
                }
                else SDL_image.Bind("./lib/linux/SDL2_image.so", "./lib/linux/libpng16-16.so");
                SDL_ttf.Bind("./lib/linux/SDL2_ttf.so", "./lib/linux/libfreetype-6.so");
            }
            else if (Platform == Platform.MacOS)
            {
                throw new Exception("MacOS support has not yet been implemented.");
            }
            else
            {
                throw new Exception("No platform could be detected.");
            }

            uint IMG_Flags = IMG_INIT_PNG;
            if (LoadedJPEG) IMG_Flags |= IMG_INIT_JPG;
            if (SDL_Init(SDL_INIT_EVERYTHING) < 0 ||
                IMG_Init(IMG_Flags) != (int) IMG_Flags ||
                TTF_Init() < 0)
            {
                throw new Exception(SDL_GetError());
            }
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
        /// Logs a message to the debug log.
        /// </summary>
        public static void Log(string Msg)
        {
            if (DebugLog)
            {
                DateTime n = DateTime.Now;
                string str = $"[{n.Hour}:{n.Minute}:{n.Second}:{n.Millisecond}] [ODL:DEBUG] " + Msg;
                Console.WriteLine(str);
            }
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
                    lock (Renderers[i]) { Renderers[i].Redraw(); }
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
        public static void RegisterRenderer(Renderer Renderer)
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

        public static void UpdateInput(bool IgnoreErrors = false)
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

        public static void UpdateWindows()
        {
            // Get events
            SDL_Event e;
            while (SDL_PollEvent(out e) > 0)
            {
                EvaluateEvent(e);
            }
        }

        public static void EvaluateEvent(SDL_Event e)
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
                        w.OnClosing(ClosingArgs);
                        if (!ClosingArgs.Value)
                        {
                            SDL_DestroyWindow(w.SDL_Window);
                            w.OnClosed(new BaseEventArgs());
                            w.Dispose();
                            Windows[idx] = null;
                        }
                        break;
                    case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
                        break;
                    case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                        int width1;
                        int height1;
                        SDL_GetWindowSize(w.SDL_Window, out width1, out height1);
                        w.OnSizeChanged(new BaseEventArgs());
                        break;
                    case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                        int width2;
                        int height2;
                        SDL_GetWindowSize(w.SDL_Window, out width2, out height2);
                        w.OnSizeChanged(new BaseEventArgs());
                        break;
                    case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                        w.Focus = true;
                        w.OnFocusGained(new BaseEventArgs());
                        break;
                    case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                        w.Focus = false;
                        w.OnFocusLost(new BaseEventArgs());
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
                            w.OnMouseMoving(new MouseEventArgs(e.motion.x, e.motion.y,
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
                        w.OnMouseDown(new MouseEventArgs(e.motion.x, e.motion.y,
                                                         oldleftdown, LeftDown,
                                                         oldrightdown, RightDown,
                                                         oldmiddledown, MiddleDown));
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                        if (e.button.button == 1) LeftDown = false;
                        if (e.button.button == 2) MiddleDown = false;
                        if (e.button.button == 3) RightDown = false;
                        w.OnMouseUp(new MouseEventArgs(e.motion.x, e.motion.y,
                                                       oldleftdown, LeftDown,
                                                       oldrightdown, RightDown,
                                                       oldmiddledown, MiddleDown));
                        break;
                    case SDL_EventType.SDL_MOUSEWHEEL:
                        w.OnMouseWheel(new MouseEventArgs(OldMouseX, OldMouseY,
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
                        if (sym1 == SDL_Keycode.SDLK_RETURN) txt = "\n";
                        if (sym1 == SDL_Keycode.SDLK_BACKSPACE) backspace = true;
                        if (sym1 == SDL_Keycode.SDLK_DELETE) delete = true;
                        if (txt.Length > 0 || backspace || delete)
                        {
                            w.OnTextInput(new TextEventArgs(txt, backspace, delete));
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
                        w.OnTextInput(new TextEventArgs(text.TrimEnd('\x00')));
                        break;
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
        }

        /// <summary>
        /// Disposes SDL and all components.
        /// </summary>
        public static void Stop()
        {
            for (int i = 0; i < Font.Cache.Count; i++)
            {
                TTF_CloseFont(Font.Cache[i].SDL_Font);
                Font.Cache[i] = null;
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
}
