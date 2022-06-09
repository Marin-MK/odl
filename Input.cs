using System;
using System.Collections.Generic;
using static odl.SDL2.SDL;

namespace odl;

public static class Input
{
    public static IntPtr Cursor;
    public static CursorType? SystemCursor;

    public static List<long> OldKeysDown = new List<long>();
    public static List<long> KeysDown = new List<long>();

    internal static void Register(SDL_Keycode code, bool value)
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

    public static bool Trigger(Keycode code)
    {
        if (code == Keycode.CTRL) return Trigger(Convert.ToInt64(SDL_Keycode.SDLK_LCTRL)) || Trigger(Convert.ToInt64(SDL_Keycode.SDLK_RCTRL));
        if (code == Keycode.SHIFT) return Trigger(Convert.ToInt64(SDL_Keycode.SDLK_LSHIFT)) || Trigger(Convert.ToInt64(SDL_Keycode.SDLK_RSHIFT));
        if (code == Keycode.ALT) return Trigger(Convert.ToInt64(SDL_Keycode.SDLK_LALT)) || Trigger(Convert.ToInt64(SDL_Keycode.SDLK_RALT));
        return Trigger(Convert.ToInt64(code));
    }
    public static bool Trigger(long Code)
    {
        return !OldKeysDown.Contains(Code) && KeysDown.Contains(Code);
    }

    public static bool Press(Keycode code)
    {
        if (code == Keycode.CTRL) return Press(Convert.ToInt64(SDL_Keycode.SDLK_LCTRL)) || Press(Convert.ToInt64(SDL_Keycode.SDLK_RCTRL));
        if (code == Keycode.SHIFT) return Press(Convert.ToInt64(SDL_Keycode.SDLK_LSHIFT)) || Press(Convert.ToInt64(SDL_Keycode.SDLK_RSHIFT));
        if (code == Keycode.ALT) return Press(Convert.ToInt64(SDL_Keycode.SDLK_LALT)) || Press(Convert.ToInt64(SDL_Keycode.SDLK_RALT));
        return Press(Convert.ToInt64(code));
    }
    public static bool Press(long Code)
    {
        return KeysDown.Contains(Code);
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

    public static void SetCursor(CursorType Cursor)
    {
        if (SystemCursor != Cursor)
        {
            IntPtr surface = SDL_CreateSystemCursor((SDL_SystemCursor) Cursor);
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
    internal static void SetCursor(IntPtr Cursor)
    {
        SystemCursor = null;
        Graphics.LastCustomCursor = Cursor;
        SDL_SetCursor(Cursor);
    }

    /// <summary>
    /// Allows mouse events to fire when the mouse is outside of the window.
    /// </summary>
    public static void CaptureMouse()
    {
        SDL_CaptureMouse(SDL_bool.SDL_TRUE);
    }

    /// <summary>
    /// Disallows mouse events to fire when the mouse is outside of the window.
    /// </summary>
    public static void ReleaseMouse()
    {
        SDL_CaptureMouse(SDL_bool.SDL_FALSE);
    }

    /// <summary>
    /// Puts text on the clipboard.
    /// </summary>
    /// <param name="Data">The text to put on the clipboard.</param>
    public static void SetClipboard(string Data)
    {
        SDL_SetClipboardText(Data);
    }

    /// <summary>
    /// Gets text from the clipboard.
    /// </summary>
    /// <returns>The text on the clipboard.</returns>
    public static string GetClipboard()
    {
        return SDL_GetClipboardText();
    }
}

public enum CursorType
{
    Arrow     = SDL_SystemCursor.SDL_SYSTEM_CURSOR_ARROW,
    Crosshair = SDL_SystemCursor.SDL_SYSTEM_CURSOR_CROSSHAIR,
    Hand      = SDL_SystemCursor.SDL_SYSTEM_CURSOR_HAND,
    IBeam     = SDL_SystemCursor.SDL_SYSTEM_CURSOR_IBEAM,
    No        = SDL_SystemCursor.SDL_SYSTEM_CURSOR_NO,
    SizeAll   = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL,
    SizeNESW  = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENESW,
    SizeNS    = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENS,
    SizeNWSE  = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENWSE,
    SizeWE    = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE,
    Wait      = SDL_SystemCursor.SDL_SYSTEM_CURSOR_WAIT,
    WaitArrow = SDL_SystemCursor.SDL_SYSTEM_CURSOR_WAITARROW
}