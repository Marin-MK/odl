using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;
using System.Runtime.InteropServices;

namespace VCS
{
    public static class Graphics
    {
        public static List<Form> Forms = new List<Form>();

        public static void Start()
        {
            if (SDL_Init(SDL_INIT_EVERYTHING) < 0)
            {
                throw new Exception(SDL_GetError());
            }
            IMG_Init(IMG_InitFlags.IMG_INIT_PNG);
            TTF_Init();
        }

        private static int OldMouseX = -1;
        private static int OldMouseY = -1;
        private static bool LeftDown = false;
        private static bool MiddleDown = false;
        private static bool RightDown = false;

        public static void Update()
        {
            for (int i = 0; i < Forms.Count; i++)
            {
                Form f = Forms[i];
                SDL_Event e;

                if (SDL_PollEvent(out e) >= 0)
                {
                    if (OldMouseX == -1) OldMouseX = e.motion.x;
                    if (OldMouseY == -1) OldMouseY = e.motion.y;
                    switch (e.window.windowEvent)
                    {
                        case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                            ClosingEventArgs ClosingArgs = new ClosingEventArgs();
                            f.OnClosing.Invoke(f, ClosingArgs);
                            if (!ClosingArgs.Cancel)
                            {
                                SDL_DestroyWindow(f.SDL_Window);
                                f.OnClosed.Invoke(f, new ClosedEventArgs());
                            }
                            break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                            break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                            break;
                    }
                    switch (e.type)
                    {
                        case SDL_EventType.SDL_MOUSEMOTION:
                            if (e.motion.x != OldMouseX || e.motion.y != OldMouseY)
                            {
                                LeftDown = (e.button.button & Convert.ToInt32(MouseButtons.Left)) == Convert.ToInt32(MouseButtons.Left);
                                MiddleDown = (e.button.button & Convert.ToInt32(MouseButtons.Middle)) == Convert.ToInt32(MouseButtons.Middle);
                                RightDown = (e.button.button & Convert.ToInt32(MouseButtons.Right)) == Convert.ToInt32(MouseButtons.Right);
                                f.OnMouseMoving.Invoke(f, new MouseEventArgs(OldMouseX, OldMouseY, e.motion.x, e.motion.y, LeftDown, MiddleDown, RightDown));
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
                                f.OnMouseDown.Invoke(f, new MouseEventArgs(OldMouseX, OldMouseY, e.motion.x, e.motion.y, LeftDown, MiddleDown, RightDown));
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
                                f.OnMouseUp.Invoke(f, new MouseEventArgs(OldMouseX, OldMouseY, e.motion.x, e.motion.y, LeftDown, MiddleDown, RightDown));
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
                if (LeftDown || MiddleDown || RightDown)
                {
                    f.OnMousePress.Invoke(f, new MouseEventArgs(OldMouseX, OldMouseY, e.motion.x, e.motion.y, LeftDown, MiddleDown, RightDown));
                }
                if (!f.Closed)
                {
                    f.OnTick.Invoke(f, new TickEventArgs(e));
                }
                if (f.Closed)
                {
                    Forms.RemoveAt(i);
                }
                Input.IterationEnd();
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
