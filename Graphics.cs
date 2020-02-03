using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace ODL
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
        public static MouseEventArgs LastMouseEvent = new MouseEventArgs(0, 0, 0, 0, false, false, false, false, false, false, 0);
        /// <summary>
        /// The last custom cursor that was set.
        /// </summary>
        public static IntPtr LastCustomCursor;
        /// <summary>
        /// Whether SDL and its componenents have been initialized.
        /// </summary>
        public static bool Initialized = false;

        /// <summary>
        /// Initializes SDL and its components.
        /// </summary>
        public static void Start()
        {
            if (SDL_Init(SDL_INIT_EVERYTHING) < 0 ||
                IMG_Init(IMG_InitFlags.IMG_INIT_PNG) < 0 ||
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
                string str = $"{n.Minute}min {n.Second}s {n.Millisecond}ms ODL:DEBUG: " + Msg;
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
            return Windows.Count(w => w != null && !w.Disposed) > 0;
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
                        w.OnTick.Invoke(w, new EventArgs());
                        if (w.Focus && (LeftDown || MiddleDown || RightDown))
                        {
                            w.OnMousePress.Invoke(w, new MouseEventArgs(OldMouseX, OldMouseY,
                                                    OldMouseX, OldMouseY,
                                                    oldleftdown, LeftDown,
                                                    oldrightdown, RightDown,
                                                    oldmiddledown, MiddleDown));
                        }
                    }
                    catch (Exception) { }
                }
                else
                {
                    w.OnTick.Invoke(w, new EventArgs());
                    if (w.Focus && (LeftDown || MiddleDown || RightDown))
                    {
                        w.OnMousePress.Invoke(w, new MouseEventArgs(OldMouseX, OldMouseY,
                                                OldMouseX, OldMouseY,
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
            if (SDL_WaitEventTimeout(out e, 5) > 0)
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
                switch (e.window.windowEvent)
                {
                    case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                        CancelEventArgs ClosingArgs = new CancelEventArgs();
                        w.OnClosing.Invoke(w, ClosingArgs);
                        if (!ClosingArgs.Cancel)
                        {
                            SDL_DestroyWindow(w.SDL_Window);
                            w.OnClosed.Invoke(w, new ClosedEventArgs());
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
                        w.OnWindowSizeChanged.Invoke(w, new WindowEventArgs(width1, height1));
                        w.OnWindowResized.Invoke(w, new WindowEventArgs(width1, height1));
                        break;
                    case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                        int width2;
                        int height2;
                        SDL_GetWindowSize(w.SDL_Window, out width2, out height2);
                        w.OnWindowSizeChanged.Invoke(w, new WindowEventArgs(width2, height2));
                        break;
                    case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                        w.Focus = true;
                        w.OnFocusGained(w, new FocusEventArgs(true));
                        break;
                    case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                        w.Focus = false;
                        w.OnFocusLost(w, new FocusEventArgs(false));
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
                            LeftDown = (e.button.button & Convert.ToInt32(MouseButtons.Left)) == Convert.ToInt32(MouseButtons.Left);
                            RightDown = (e.button.button & Convert.ToInt32(MouseButtons.Right)) == Convert.ToInt32(MouseButtons.Right);
                            MiddleDown = (e.button.button & Convert.ToInt32(MouseButtons.Middle)) == Convert.ToInt32(MouseButtons.Middle);
                            w.OnMouseMoving.Invoke(w, new MouseEventArgs(OldMouseX, OldMouseY,
                                    e.motion.x, e.motion.y,
                                    oldleftdown, LeftDown,
                                    oldrightdown, RightDown,
                                    oldmiddledown, MiddleDown));
                        }
                        OldMouseX = e.motion.x;
                        OldMouseY = e.motion.y;
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        if (e.button.button == 1 && !LeftDown ||
                            e.button.button == 2 && !MiddleDown ||
                            e.button.button == 3 && !RightDown)
                        {
                            if (e.button.button == 1) LeftDown = true;
                            if (e.button.button == 2) MiddleDown = true;
                            if (e.button.button == 3) RightDown = true;
                            w.OnMouseDown.Invoke(w, new MouseEventArgs(OldMouseX, OldMouseY,
                                    e.motion.x, e.motion.y,
                                    oldleftdown, LeftDown,
                                    oldrightdown, RightDown,
                                    oldmiddledown, MiddleDown));
                        }
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                        if (e.button.button == 1 && LeftDown ||
                            e.button.button == 2 && MiddleDown ||
                            e.button.button == 3 && RightDown)
                        {
                            if (e.button.button == 1 && LeftDown) LeftDown = false;
                            if (e.button.button == 2 && MiddleDown) MiddleDown = false;
                            if (e.button.button == 3 && RightDown) RightDown = false;
                            w.OnMouseUp.Invoke(w, new MouseEventArgs(OldMouseX, OldMouseY,
                                    e.motion.x, e.motion.y,
                                    oldleftdown, LeftDown,
                                    oldrightdown, RightDown,
                                    oldmiddledown, MiddleDown));
                        }
                        break;
                    case SDL_EventType.SDL_MOUSEWHEEL:
                        w.OnMouseWheel.Invoke(w, new MouseEventArgs(OldMouseX, OldMouseY,
                                OldMouseX, OldMouseY,
                                oldleftdown, LeftDown,
                                oldrightdown, RightDown,
                                oldmiddledown, MiddleDown,
                                e.wheel.y));
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
                            w.OnTextInput.Invoke(w, new TextInputEventArgs(txt, backspace, delete));
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
                        w.OnTextInput.Invoke(w, new TextInputEventArgs(text.TrimEnd('\x00')));
                        break;
                }
            }
        }

        /// <summary>
        /// Updates input, graphics and the window.
        /// </summary>
        public static void Update(bool IgnoreErrors = false)
        {
            UpdateInput(IgnoreErrors);
            UpdateGraphics();
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
}
