using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace ODL
{
    public static class Graphics
    {
        public static List<Window> Windows = new List<Window>();
        public static List<Rect> Screens = new List<Rect>();
        public static List<Renderer> Renderers = new List<Renderer>();

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
        }

        static void Render()
        {
            while (true)
            {
                for (int i = 0; i < Renderers.Count; i++)
                {
                    lock (Renderers[i]) { Renderers[i].UpdateGraphics(); }
                }
            }
        }

        public static void AddWindow(Window w)
        {
            Windows.Add(w);
        }

        public static bool ScreenExists(int screen)
        {
            return screen < Screens.Count;
        }

        public static int GetWidth(Window w)
        {
            int screen = SDL_GetWindowDisplayIndex(w.SDL_Window);
            return GetWidth(screen);
        }
        public static int GetWidth(int screen = 0)
        {
            return Screens[screen].Width;
        }

        public static int GetHeight(Window w)
        {
            int screen = SDL_GetWindowDisplayIndex(w.SDL_Window);
            return GetHeight(screen);
        }
        public static int GetHeight(int screen = 0)
        {
            return Screens[screen].Height;
        }

        private static int OldMouseX = -1;
        private static int OldMouseY = -1;
        private static bool LeftDown = false;
        private static bool MiddleDown = false;
        private static bool RightDown = false;

        public static void RegisterRenderer(Renderer Renderer)
        {
            Renderers.Add(Renderer);
        }

        public static bool CanUpdate()
        {
            return Windows.Count(w => w != null) > 0;
        }

        public static void Update()
        {
            // Old mouse states
            bool oldleftdown = LeftDown;
            bool oldrightdown = RightDown;
            bool oldmiddledown = MiddleDown;

            // Update all the windows
            Windows.ForEach(w =>
            {
                if (w != null) w.OnTick.Invoke(w, new EventArgs());
                if (w.Focus && (LeftDown || MiddleDown || RightDown))
                {
                    w.OnMousePress.Invoke(w, new MouseEventArgs(OldMouseX, OldMouseY,
                                            OldMouseX, OldMouseY,
                                            oldleftdown, LeftDown,
                                            oldrightdown, RightDown,
                                            oldmiddledown, MiddleDown));
                }
            });

            // Update button key states
            Input.IterationEnd();

            // Get events
            SDL_Event e;

            bool quit = false;

            while (!quit)
            {
                if (SDL_PollEvent(out e) > 0)
                {
                    if (e.type == SDL_EventType.SDL_FIRSTEVENT) { quit = true; continue; }
                    if (e.window.windowID == 0) continue;
                    IntPtr sdlwindow = SDL_GetWindowFromID(e.window.windowID);
                    Window w = Windows.Find(win => win != null && win.SDL_Window == sdlwindow);
                    if (w == null) continue;
                    int idx = Windows.IndexOf(w);
                    // After closing a window, there are still a few more events like losing focus;
                    // We can skip these as the window was already destroyed.
                    if (w == null) continue;
                    if (OldMouseX == -1) OldMouseX = e.motion.x;
                    if (OldMouseY == -1) OldMouseY = e.motion.y;
                    if (e.type == SDL_EventType.SDL_WINDOWEVENT)
                    {
                        switch (e.window.windowEvent)
                        {
                            case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                                ClosingEventArgs ClosingArgs = new ClosingEventArgs();
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
                    if (Windows[idx] == null) continue; // Just disposed this window
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
                                if (sym1 == SDL_Keycode.SDLK_RETURN) txt = "\n";
                                if (sym1 == SDL_Keycode.SDLK_BACKSPACE) backspace = true;
                                if (txt.Length > 0 || backspace)
                                {
                                    w.OnTextInput.Invoke(w, new TextInputEventArgs(txt, backspace));
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
                else
                {
                    quit = true;
                }
            }

            for (int i = 0; i < Renderers.Count; i++)
            {
                Renderers[i].UpdateGraphics();
            }
        }

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
        }
    }
}
