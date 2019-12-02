using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace ODL
{
    public static class Input
    {
        public static IntPtr Cursor;
        public static SDL_SystemCursor? SystemCursor;

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
                IntPtr surface = SDL_CreateSystemCursor(Cursor);
                SystemCursor = Cursor;
                SDL_SetCursor(surface);
            }
        }
        /// <summary>
        /// Replaces the mouse cursor with a custom bitmap.
        /// </summary>
        /// <param name="CursorBitmap">The bitmap to use for the mouse cursor.</param>
        /// <param name="OriginX">The origin x position of the bitmap.</param>
        /// <param name="OriginY">The origin y position of the bitmap.</param>
        public static void SetCursor(Bitmap CursorBitmap, int OriginX = 0, int OriginY = 0)
        {
            IntPtr cursor = SDL_CreateColorCursor(CursorBitmap.Surface, OriginX, OriginY);
            Graphics.LastCustomCursor = cursor;
            SystemCursor = null;
            SDL_SetCursor(cursor);
        }
        /// <summary>
        /// Replaces the mouse cursor with a different cursor.
        /// </summary>
        /// <param name="Cursor">Pointer to the SDL_Cursor object.</param>
        public static void SetCursor(IntPtr Cursor)
        {
            SystemCursor = null;
            Graphics.LastCustomCursor = Cursor;
            SDL_SetCursor(Cursor);
        }
    }
}
