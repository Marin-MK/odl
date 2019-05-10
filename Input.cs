using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace ODL
{
    public static class Input
    {
        public static IntPtr CursorSurface = IntPtr.Zero;
        public static SDL_SystemCursor SystemCursor;

        public static List<long> OldKeysDown = new List<long>();
        public static List<long> KeysDown = new List<long>();

        public static void Register(SDL_Keycode code, bool value)
        {
            long key = Convert.ToInt64(code);
            if (value)
            {
                if (!KeysDown.Contains(key)) KeysDown.Add(key);
            }
            else
            {
                KeysDown.Remove(key);
            }
        }

        public static void IterationEnd()
        {
            OldKeysDown = new List<long>(KeysDown);
        }

        public static bool Trigger(SDL_Keycode code)
        {
            long key = Convert.ToInt64(code);
            return !OldKeysDown.Contains(key) && KeysDown.Contains(key);
        }

        public static bool Press(SDL_Keycode code)
        {
            long key = Convert.ToInt64(code);
            return KeysDown.Contains(key);
        }

        public static void StartTextInput()
        {
            SDL_StartTextInput();
        }

        public static bool TextInputActive()
        {
            return SDL_IsTextInputActive() == SDL_bool.SDL_TRUE;
        }

        public static void StopTextInput()
        {
            SDL_StopTextInput();
        }

        public static void SetCursor(SDL_SystemCursor Cursor)
        {
            if (SystemCursor != Cursor)
            {
                if (CursorSurface != IntPtr.Zero)
                {
                    SDL_FreeCursor(CursorSurface);
                }
                CursorSurface = SDL_CreateSystemCursor(Cursor);
                SystemCursor = Cursor;
                SDL_SetCursor(CursorSurface);
            }
        }
    }
}
