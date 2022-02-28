using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace odl;

public class Popup
{
    public PopupType Type;
    public Window ParentWindow;
    public string Title;
    public string Message;
    public List<string> Buttons;

    public Popup(string Title, string Message, PopupType Type, List<string> Buttons)
        : this(null, Title, Message, Type, Buttons) { }

    public Popup(string Title, string Message, PopupType Type)
        : this(null, Title, Message, Type, new List<string>() { "OK" }) { }

    public Popup(string Title, string Message, List<string> Buttons)
        :this (null, Title, Message, PopupType.Information, Buttons) { }

    public Popup(string Title, string Message)
        : this(null, Title, Message, PopupType.Information, new List<string>() { "OK" }) { }

    public Popup(Window ParentWindow, string Title, string Message, PopupType Type)
        : this(ParentWindow, Title, Message, Type, new List<string>() { "OK" }) { }

    public Popup(Window ParentWindow, string Title, string Message, List<string> Buttons)
        : this(ParentWindow, Title, Message, PopupType.Information, Buttons) { }

    public Popup(Window ParentWindow, string Title, string Message)
        : this(ParentWindow, Title, Message, PopupType.Information, new List<string>() { "OK" }) { }

    public Popup(Window ParentWindow, string Title, string Message, PopupType Type, List<string> Buttons)
    {
        this.ParentWindow = ParentWindow;
        this.Title = Title;
        this.Message = Message;
        this.Type = Type;
        this.Buttons = Buttons;
    }

    public unsafe int Show()
    {
        SDL2.SDL.SDL_MessageBoxData data = new SDL2.SDL.SDL_MessageBoxData();
        data.flags = (SDL2.SDL.SDL_MessageBoxFlags) this.Type;
        data.window = ParentWindow == null ? IntPtr.Zero : ParentWindow.SDL_Window;
        data.title = this.Title;
        data.message = this.Message;
        data.numbuttons = this.Buttons.Count;
        int bsize = sizeof(SDL2.SDL.SDL_MessageBoxButtonData);
        data.buttons = Marshal.AllocHGlobal(bsize * data.numbuttons);
        for (int i = 0; i < this.Buttons.Count; i++)
        {
            SDL2.SDL.SDL_MessageBoxButtonData button = new SDL2.SDL.SDL_MessageBoxButtonData();
            if (i == this.Buttons.Count - 1) button.flags |= SDL2.SDL.SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_RETURNKEY_DEFAULT;
            if (i == 0) button.flags |= SDL2.SDL.SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_ESCAPEKEY_DEFAULT;
            button.buttonid = this.Buttons.Count - i - 1;
            button.text = Marshal.StringToCoTaskMemUTF8(this.Buttons[this.Buttons.Count - i - 1]);
            Marshal.StructureToPtr(button, data.buttons + bsize * i, true);
        }
        data.colorScheme = IntPtr.Zero;
        int result = -1;
        SDL2.SDL.SDL_ShowMessageBox(ref data, out result);
        return result;
    }
}

public enum PopupType : uint
{
    Error = 0x10,
    Warning = 0x20,
    Information = 0x40
}
