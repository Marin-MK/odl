﻿using System;
using System.Collections.Generic;
using System.Linq;

using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace ODL
{
    public static class Graphics
    {
        public static List<Window> Windows = new List<Window>();
        public static List<Rect> Screens = new List<Rect>();

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
            });

            // Update button key states
            Input.IterationEnd();

            // Get events
            SDL_Event e;

            if (SDL_PollEvent(out e) >= 0)
            {
                if (e.window.windowID == 0) return;
                int idx = Convert.ToInt32(e.window.windowID) - 1;
                Window w = Windows[idx] as Window;
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
                            Console.WriteLine("resized");
                            break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                            Console.WriteLine("resizing");
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
                        case SDL_EventType.SDL_KEYDOWN:
                            SDL_Keycode sym1 = e.key.keysym.sym;
                            Input.Register(sym1, true);
                            break;
                        case SDL_EventType.SDL_KEYUP:
                            SDL_Keycode sym2 = e.key.keysym.sym;
                            Input.Register(sym2, false);
                            break;
                    }
                }
                if (w.Focus && (LeftDown || MiddleDown || RightDown))
                {
                    w.OnMousePress.Invoke(w, new MouseEventArgs(OldMouseX, OldMouseY,
                                            e.motion.x, e.motion.y,
                                            oldleftdown, LeftDown,
                                            oldrightdown, RightDown,
                                            oldmiddledown, MiddleDown));
                }
            }
        }

        public static void Stop()
        {
            IMG_Quit();
            SDL_Quit();
            TTF_Quit();
        }
    }
}
