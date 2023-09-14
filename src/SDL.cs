using System;
using NativeLibraryLoader;

// Some structs and enums taken from https://github.com/flibitijibibo/SDL2-CS.
// Credit to Ethan "flibitijibibo" Lee for the aforementioned code.

namespace odl.SDL2;

public class SDL : NativeLibrary
{
    private static SDL Main;

    public static SDL_Version Version = new SDL_Version();

    public new static SDL Load(string Library)
    {
        Main = new SDL(Library);
        return Main;
    }

    protected SDL(string Library) : base(Library)
    {
        SDL_Init = GetFunction<SDL_IntUInt>("SDL_Init");
        SDL_GetVersion = GetFunction<SDL_VoidVersion>("SDL_GetVersion");
        FUNC_SDL_GetError = GetFunction<SDL_Ptr>("SDL_GetError");
        SDL_ClearError = GetFunction<Action>("SDL_ClearError");
        SDL_Quit = GetFunction<Action>("SDL_Quit");
        SDL_RenderClear = GetFunction<SDL_IntPtr>("SDL_RenderClear");
        SDL_RenderGetViewport = GetFunction<SDL_VoidPtrOutRect>("SDL_RenderGetViewport");
        FUNC_SDL_CreateWindow = GetFunction<SDL_PtrStrIntIntIntIntWindowFlags>("SDL_CreateWindow");
        SDL_CreateRenderer = GetFunction<SDL_PtrPtrIntRendererFlags>("SDL_CreateRenderer");
        SDL_GetWindowFlags = GetFunction<SDL_UIntPtr>("SDL_GetWindowFlags");
        FUNC_SDL_SetHint = GetFunction<SDL_SDLBoolPtrPtr>("SDL_SetHint");
        FUNC_SDL_GetHint = GetFunction<SDL_PtrPtr>("SDL_GetHint");
        FUNC_SDL_SetHintWithPriority = GetFunction<SDL_SDLBoolPtrPtrInt>("SDL_SetHintWithPriority");
        SDL_GetWindowPosition = GetFunction<SDL_VoidPtrOutIntOutInt>("SDL_GetWindowPosition");
        SDL_SetWindowPosition = GetFunction<SDL_VoidPtrIntInt>("SDL_SetWindowPosition");
        SDL_SetWindowResizable = GetFunction<SDL_VoidPtrSDLBool>("SDL_SetWindowResizable");
        SDL_SetWindowIcon = GetFunction<SDL_VoidPtrPtr>("SDL_SetWindowIcon");
        SDL_GetRendererInfo = GetFunction<SDL_IntIntPtrOutRendererInfo>("SDL_GetRendererInfo");
        SDL_GetRenderDriverInfo = GetFunction<SDL_IntIntOutRendererInfo>("SDL_GetRenderDriverInfo");
        SDL_SetWindowMinimumSize = GetFunction<SDL_VoidPtrIntInt>("SDL_SetWindowMinimumSize");
        SDL_SetWindowSize = GetFunction<SDL_VoidPtrIntInt>("SDL_SetWindowSize");
        SDL_GetWindowSize = GetFunction<SDL_VoidPtrOutIntOutInt>("SDL_GetWindowSize");
        FUNC_SDL_SetWindowTitle = GetFunction<SDL_VoidPtrPtr>("SDL_SetWindowTitle");
        SDL_GetWindowDisplayIndex = GetFunction<SDL_IntPtr>("SDL_GetWindowDisplayIndex");
        SDL_GetNumVideoDisplays = GetFunction<SDL_Int>("SDL_GetNumVideoDisplays");
        SDL_ShowWindow = GetFunction<SDL_VoidPtr>("SDL_ShowWindow");
        SDL_HideWindow = GetFunction<SDL_VoidPtr>("SDL_HideWindow");
        SDL_MinimizeWindow = GetFunction<SDL_VoidPtr>("SDL_MinimizeWindow");
        SDL_MaximizeWindow = GetFunction<SDL_VoidPtr>("SDL_MaximizeWindow");
        SDL_RaiseWindow = GetFunction<SDL_VoidPtr>("SDL_RaiseWindow");
        SDL_DestroyRenderer = GetFunction<SDL_VoidPtr>("SDL_DestroyRenderer");
        SDL_DestroyWindow = GetFunction<SDL_VoidPtr>("SDL_DestroyWindow");
        SDL_CreateRGBSurfaceWithFormat = GetFunction<SDL_PtrUIntIntIntIntPixelFormat>("SDL_CreateRGBSurfaceWithFormat");
        SDL_RenderReadPixels = GetFunction<SDL_IntPtrRectPixelFormatPtrInt>("SDL_RenderReadPixels");
        SDL_RenderSetLogicalSize = GetFunction<SDL_IntPtrIntInt>("SDL_RenderSetLogicalSize");
        SDL_RenderSetScale = GetFunction<SDL_IntPtrFltFlt>("SDL_RenderSetScale");
        SDL_RenderSetViewport = GetFunction<SDL_IntPtrRefRect>("SDL_RenderSetViewport");
        SDL_RenderPresent = GetFunction<SDL_VoidPtr>("SDL_RenderPresent");
        SDL_SetTextureAlphaMod = GetFunction<SDL_IntPtrByte>("SDL_SetTextureAlphaMod");
        SDL_RenderCopy = GetFunction<SDL_IntPtrPtrRefRectRefRect>("SDL_RenderCopy");
        SDL_RenderCopyEx = GetFunction<SDL_IntPtrPtrRefRectRefRectDblRefPointRendererFlip>("SDL_RenderCopyEx");
        SDL_StartTextInput = GetFunction<Action>("SDL_StartTextInput");
        SDL_StopTextInput = GetFunction<Action>("SDL_StopTextInput");
        SDL_IsTextInputActive = GetFunction<SDL_Bool>("SDL_IsTextInputActive");
        SDL_CreateSystemCursor = GetFunction<SDL_PtrSystemCursor>("SDL_CreateSystemCursor");
        SDL_SetCursor = GetFunction<SDL_VoidPtr>("SDL_SetCursor");
        SDL_CreateColorCursor = GetFunction<SDL_CursorPtrIntInt>("SDL_CreateColorCursor");
        SDL_CaptureMouse = GetFunction<SDL_IntSDLBool>("SDL_CaptureMouse");
        SDL_FreeSurface = GetFunction<SDL_VoidPtr>("SDL_FreeSurface");
        SDL_DestroyTexture = GetFunction<SDL_VoidPtr>("SDL_DestroyTexture");
        SDL_CreateRGBSurface = GetFunction<SDL_PtrUIntIntIntIntUIntUIntUIntUInt>("SDL_CreateRGBSurface");
        SDL_CreateRGBSurfaceWithFormatFrom = GetFunction<SDL_PtrPtrIntIntIntIntPixelFormat>("SDL_CreateRGBSurfaceWithFormatFrom");
        SDL_FillRect = GetFunction<SDL_IntPtrRefRectUInt>("SDL_FillRect");
        SDL_MapRGBA = GetFunction<SDL_UIntPtrByteByteByteByte>("SDL_MapRGBA");
        SDL_BlitScaled = GetFunction<SDL_IntPtrRefRectPtrRefRect>("SDL_UpperBlitScaled");
        SDL_BlitSurface = GetFunction<SDL_IntPtrRefRectPtrRefRect>("SDL_UpperBlit");
        SDL_SetTextureBlendMode = GetFunction<SDL_IntPtrBlendMode>("SDL_SetTextureBlendMode");
        SDL_GetTextureBlendMode = GetFunction<SDL_IntPtrOutBlendMode>("SDL_GetTextureBlendMode");
        SDL_SetSurfaceBlendMode = GetFunction<SDL_IntPtrBlendMode>("SDL_SetSurfaceBlendMode");
        SDL_GetSurfaceBlendMode = GetFunction<SDL_IntPtrOutBlendMode>("SDL_GetSurfaceBlendMode");
        SDL_CreateTextureFromSurface = GetFunction<SDL_PtrPtrPtr>("SDL_CreateTextureFromSurface");
        SDL_GetDisplayBounds = GetFunction<SDL_IntIntOutRect>("SDL_GetDisplayBounds");
        SDL_GetDisplayUsableBounds = GetFunction<SDL_IntIntOutRect>("SDL_GetDisplayUsableBounds");
        SDL_Delay = GetFunction<SDL_VoidUInt>("SDL_Delay");
        SDL_PollEvent = GetFunction<SDL_IntOutEvent>("SDL_PollEvent");
        SDL_GetWindowFromID = GetFunction<SDL_PtrUInt>("SDL_GetWindowFromID");
        FUNC_SDL_GetClipboardText = GetFunction<SDL_Ptr>("SDL_GetClipboardText");
        FUNC_SDL_SetClipboardText = GetFunction<SDL_IntPtr>("SDL_SetClipboardText");
        SDL_GetWindowDisplayMode = GetFunction<SDL_IntPtrOutDisplayMode>("SDL_GetWindowDisplayMode");
        SDL_ShowMessageBox = GetFunction<SDL_IntRefMessageBoxDataOutInt>("SDL_ShowMessageBox");
        SDL_LockSurface = GetFunction<SDL_IntPtr>("SDL_LockSurface");
        SDL_UnlockSurface = GetFunction<SDL_IntPtr>("SDL_UnlockSurface");
        SDL_SetSurfaceRLE = GetFunction<SDL_IntPtrInt>("SDL_SetSurfaceRLE");
        SDL_FlashWindow = GetFunction<SDL_IntPtrFlash>("SDL_FlashWindow");
        SDL_OpenURL = GetFunction<SDL_IntStr>("SDL_OpenURL");
        SDL_ConvertSurfaceFormat = GetFunction<SDL_PtrPtrPixelFormatUInt>("SDL_ConvertSurfaceFormat");
        SDL_GetDisplayDPI = GetFunction<SDL_IntIntFloatFloatFloat>("SDL_GetDisplayDPI");
        SDL_GetNumRenderDrivers = GetFunction<SDL_Int>("SDL_GetNumRenderDrivers");
        SDL_SetRenderTarget = GetFunction<SDL_IntPtrPtr>("SDL_SetRenderTarget");
        SDL_CreateTexture = GetFunction<SDL_PtrPtrPixelFormatIntIntInt>("SDL_CreateTexture");
        SDL_SetTextureColorMod = GetFunction<SDL_IntPtrByteByteByte>("SDL_SetTextureColorMod");
        SDL_SetWindowBordered = GetFunction<SDL_VoidPtrSDLBool>("SDL_SetWindowBordered");
        SDL_GetRGBA = GetFunction<SDL_VoidUintPtrPtrPtrPtrPtr>("SDL_GetRGBA");
        SDL_GL_GetProcAddress = GetFunction<SDL_PtrStr>("SDL_GL_GetProcAddress");
        SDL_GL_BindTexture = GetFunction<SDL_IntPtrPtrPtr>("SDL_GL_BindTexture");
        SDL_GL_SwapWindow = GetFunction<SDL_VoidPtr>("SDL_GL_SwapWindow");
        SDL_GetWindowWMInfo = GetFunction<SDL_BoolPtrWMinfo>("SDL_GetWindowWMInfo");
        SDL_GetVersion(ref Version);
    }

    public static TDelegate GetGLFunction<TDelegate>(string FunctionName)
    {
        IntPtr funcaddr = SDL_GL_GetProcAddress(FunctionName);
        if (funcaddr == IntPtr.Zero) throw new InvalidEntryPointException(Main.LibraryName, FunctionName);
        return System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<TDelegate>(funcaddr);
    }

    #region Function Delegates
    public delegate int SDL_IntUInt(uint UInt);
    public delegate void SDL_VoidVersion(ref SDL_Version Version);
    public delegate IntPtr SDL_Ptr();
    public delegate int SDL_IntPtr(IntPtr IntPtr);
    public delegate int SDL_VoidPtrOutRect(IntPtr IntPtr, out SDL_Rect Rect);
    public delegate IntPtr SDL_PtrStrIntIntIntIntWindowFlags(IntPtr String, int Int1, int Int2, int Int3, int Int4, SDL_WindowFlags WindowFlags);
    public delegate IntPtr SDL_PtrPtrIntRendererFlags(IntPtr IntPtr, int Int, SDL_RendererFlags UInt);
    public delegate IntPtr SDL_UIntPtr(IntPtr IntPtr);
    public delegate SDL_bool SDL_SDLBoolPtrPtr(IntPtr IntPtr1, IntPtr IntPtr2);
    public delegate SDL_bool SDL_SDLBoolPtrPtrInt(IntPtr IntPtr1, IntPtr IntPtr2, int Int1);
    public delegate void SDL_VoidPtrOutIntOutInt(IntPtr IntPtr, out int Int1, out int Int2);
    public delegate void SDL_VoidPtrSDLBool(IntPtr IntPtr, SDL_bool Bool);
    public delegate void SDL_VoidPtrPtr(IntPtr IntPtr1, IntPtr IntPtr2);
    public delegate int SDL_IntIntPtrOutRendererInfo(IntPtr IntPtr, out SDL_RendererInfo RendererInfo);
    public delegate int SDL_IntIntOutRendererInfo(int Int, out SDL_RendererInfo RendererInfo);
    public delegate void SDL_VoidPtrIntInt(IntPtr IntPtr, int Int1, int Int2);
    public delegate int SDL_Int();
    public delegate void SDL_VoidPtr(IntPtr IntPtr);
    public delegate IntPtr SDL_PtrUIntIntIntIntPixelFormat(uint UInt1, int Int1, int Int2, int Int3, SDL_PixelFormatEnum PixelFormat);
    public delegate int SDL_IntPtrRectPixelFormatPtrInt(IntPtr IntPtr1, SDL_Rect Rect, SDL_PixelFormatEnum PixelFormat, IntPtr IntPtr2, int Int);
    public delegate int SDL_IntPtrIntInt(IntPtr IntPtr, int Int1, int Int2);
    public delegate int SDL_IntPtrFltFlt(IntPtr IntPtr, float Flt1, float Flt2);
    public delegate int SDL_IntPtrRefRect(IntPtr IntPtr, ref SDL_Rect Rect);
    public delegate int SDL_IntPtrByte(IntPtr IntPtr, byte Byte);
    public delegate int SDL_IntPtrPtrRefRectRefRect(IntPtr IntPtr1, IntPtr IntPtr2, ref SDL_Rect Rect1, ref SDL_Rect Rect2);
    public delegate int SDL_IntPtrPtrRefRectRefRectDblRefPointRendererFlip(IntPtr IntPtr1, IntPtr IntPtr2, ref SDL_Rect Rect1, ref SDL_Rect Rect2, double Dbl, ref SDL_Point Point, SDL_RendererFlip RendererFlip);
    public delegate SDL_bool SDL_Bool();
    public delegate IntPtr SDL_PtrSystemCursor(SDL_SystemCursor SystemCursor);
    public delegate IntPtr SDL_CursorPtrIntInt(IntPtr IntPtr, int Int1, int Int2);
    public delegate int SDL_IntSDLBool(SDL_bool Bool);
    public delegate IntPtr SDL_PtrUIntIntIntIntUIntUIntUIntUInt(uint UInt1, int Int1, int Int2, int Int3, uint UInt2, uint UInt3, uint UInt4, uint UInt5);
    public delegate IntPtr SDL_PtrPtrIntIntIntIntPixelFormat(IntPtr IntPtr, int Int1, int Int2, int Int3, int Int4, SDL_PixelFormatEnum PixelFormat);
    public delegate int SDL_IntPtrRefRectUInt(IntPtr IntPtr, ref SDL_Rect Rect, uint UInt);
    public delegate uint SDL_UIntPtrByteByteByteByte(IntPtr IntPtr, byte Byte1, byte Byte2, byte Byte3, byte Byte4);
    public delegate int SDL_IntPtrRefRectPtrRefRect(IntPtr IntPtr1, ref SDL_Rect Rect1, IntPtr IntPtr2, ref SDL_Rect Rect2);
    public delegate int SDL_IntPtrBlendMode(IntPtr IntPtr, SDL_BlendMode BlendMode);
    public delegate int SDL_IntPtrOutBlendMode(IntPtr IntPtr, out SDL_BlendMode BlendMode);
    public delegate IntPtr SDL_PtrPtrPtr(IntPtr IntPtr1, IntPtr IntPtr2);
    public delegate int SDL_IntIntOutRect(int Int, out SDL_Rect Rect);
    public delegate void SDL_VoidUInt(uint UInt);
    public delegate int SDL_IntOutEvent(out SDL_Event Event);
    public delegate IntPtr SDL_PtrUInt(uint UInt);
    public delegate IntPtr SDL_PtrPtr(IntPtr IntPtr);
    public delegate int SDL_IntPtrOutDisplayMode(IntPtr IntPtr, out SDL_DisplayMode DisplayMode);
    public delegate int SDL_IntRefMessageBoxDataOutInt(ref SDL_MessageBoxData MessageBoxData, out int Int);
    public delegate int SDL_IntPtrInt(IntPtr Ptr, int Int);
    public delegate int SDL_IntPtrFlash(IntPtr Ptr, SDL_FlashOperation FlashOperation);
    public delegate int SDL_IntStr(string Str);
    public delegate IntPtr SDL_PtrPtrPixelFormatUInt(IntPtr Ptr, SDL_PixelFormatEnum PixelFormat, uint UInt);
    public delegate int SDL_IntIntFloatFloatFloat(int Int, out float Float1, out float Float2, out float Float3);
    public delegate int SDL_IntPtrPtr(IntPtr Ptr1, IntPtr Ptr2);
    public delegate IntPtr SDL_PtrPtrPixelFormatIntIntInt(IntPtr Ptr, SDL_PixelFormatEnum PixelFormat, int Int1, int Int2, int Int3);
    public delegate IntPtr SDL_PtrStr(string Str);
    public delegate int SDL_IntPtrPtrPtr(IntPtr Ptr1, IntPtr Ptr2, IntPtr Ptr3);
    public delegate SDL_bool SDL_BoolPtrWMinfo(IntPtr Ptr1, ref SDL_SysWMinfo Ptr2);
    public delegate int SDL_IntPtrByteByteByte(IntPtr Ptr, byte Byte1, byte Byte2, byte Byte3);
    public delegate void SDL_VoidUintPtrPtrPtrPtrPtr(uint UInt, IntPtr Ptr, ref byte Byte1, ref byte Byte2, ref byte Byte3, ref byte Byte4);
    #endregion

    #region Utility
    public static unsafe string PtrToStr(IntPtr Pointer)
    {
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Pointer);
    }

    public static unsafe IntPtr StrToPtr(string String)
    {
        return System.Runtime.InteropServices.Marshal.StringToCoTaskMemAnsi(String);
    }

    public static unsafe string PtrToStrUTF8(IntPtr Pointer)
    {
        return System.Runtime.InteropServices.Marshal.PtrToStringUTF8(Pointer);
    }

    public static unsafe IntPtr StrUTF8ToPtr(string String)
    {
        return System.Runtime.InteropServices.Marshal.StringToCoTaskMemUTF8(String);
    }
    #endregion

    #region SDL Functions
    private static SDL_Ptr FUNC_SDL_GetError;
    private static SDL_PtrStrIntIntIntIntWindowFlags FUNC_SDL_CreateWindow;
    private static SDL_SDLBoolPtrPtr FUNC_SDL_SetHint;
    private static SDL_PtrPtr FUNC_SDL_GetHint;
    private static SDL_SDLBoolPtrPtrInt FUNC_SDL_SetHintWithPriority;
    private static SDL_VoidPtrPtr FUNC_SDL_SetWindowTitle;
    private static SDL_Ptr FUNC_SDL_GetClipboardText;
    private static SDL_IntPtr FUNC_SDL_SetClipboardText;

    public static SDL_IntUInt SDL_Init;
    public static SDL_VoidVersion SDL_GetVersion;
    public static string SDL_GetError()
    {
        string str = PtrToStrUTF8(FUNC_SDL_GetError());
        SDL_ClearError();
        return str;
    }
    public static Action SDL_ClearError;
    public static Action SDL_Quit;
    public static SDL_IntPtr SDL_RenderClear;
    public static SDL_VoidPtrOutRect SDL_RenderGetViewport;
    public unsafe static IntPtr SDL_CreateWindow(string Title, int X, int Y, int Width, int Height, SDL_WindowFlags Flags)
    {
        return FUNC_SDL_CreateWindow(StrUTF8ToPtr(Title), X, Y, Width, Height, Flags);
    }
    public static SDL_PtrPtrIntRendererFlags SDL_CreateRenderer;
    public static SDL_UIntPtr SDL_GetWindowFlags;
    public static SDL_bool SDL_SetHint(string Name, string Value)
    {
        return FUNC_SDL_SetHint(StrUTF8ToPtr(Name), StrUTF8ToPtr(Value));
    }
    public static string SDL_GetHint(string Name)
    {
        return PtrToStrUTF8(FUNC_SDL_GetHint(StrUTF8ToPtr(Name)));
    }
    public static SDL_bool SDL_SetHintWithPriority(string Name, string Value, SDL_HintPriority Priority)
    {
        return FUNC_SDL_SetHintWithPriority(StrUTF8ToPtr(Name), StrUTF8ToPtr(Value), (int)Priority);
    }
    public static SDL_VoidPtrOutIntOutInt SDL_GetWindowPosition;
    public static SDL_VoidPtrIntInt SDL_SetWindowPosition;
    public static SDL_VoidPtrSDLBool SDL_SetWindowResizable;
    public static SDL_VoidPtrPtr SDL_SetWindowIcon;
    public static SDL_IntIntPtrOutRendererInfo SDL_GetRendererInfo;
    public static SDL_IntIntOutRendererInfo SDL_GetRenderDriverInfo;
    public static SDL_VoidPtrIntInt SDL_SetWindowMinimumSize;
    public static SDL_VoidPtrIntInt SDL_SetWindowSize;
    public static SDL_VoidPtrOutIntOutInt SDL_GetWindowSize;
    public static void SDL_SetWindowTitle(IntPtr SDL_Window, string Title)
    {
        FUNC_SDL_SetWindowTitle(SDL_Window, StrUTF8ToPtr(Title));
    }
    public static SDL_IntPtr SDL_GetWindowDisplayIndex;
    public static SDL_Int SDL_GetNumVideoDisplays;
    public static SDL_VoidPtr SDL_ShowWindow;
    public static SDL_VoidPtr SDL_HideWindow;
    public static SDL_VoidPtr SDL_MinimizeWindow;
    public static SDL_VoidPtr SDL_MaximizeWindow;
    public static SDL_VoidPtr SDL_RaiseWindow;
    public static SDL_VoidPtr SDL_DestroyRenderer;
    public static SDL_VoidPtr SDL_DestroyWindow;
    public static SDL_PtrUIntIntIntIntPixelFormat SDL_CreateRGBSurfaceWithFormat;
    public static SDL_IntPtrRectPixelFormatPtrInt SDL_RenderReadPixels;
    public static SDL_IntPtrIntInt SDL_RenderSetLogicalSize;
    public static SDL_IntPtrFltFlt SDL_RenderSetScale;
    public static SDL_IntPtrRefRect SDL_RenderSetViewport;
    public static SDL_VoidPtr SDL_RenderPresent;
    public static SDL_IntPtrByte SDL_SetTextureAlphaMod;
    public static SDL_IntPtrPtrRefRectRefRect SDL_RenderCopy;
    public static SDL_IntPtrPtrRefRectRefRectDblRefPointRendererFlip SDL_RenderCopyEx;
    public static Action SDL_StartTextInput;
    public static Action SDL_StopTextInput;
    public static SDL_Bool SDL_IsTextInputActive;
    public static SDL_PtrSystemCursor SDL_CreateSystemCursor;
    public static SDL_VoidPtr SDL_SetCursor;
    public static SDL_CursorPtrIntInt SDL_CreateColorCursor;
    public static SDL_IntSDLBool SDL_CaptureMouse;
    public static SDL_VoidPtr SDL_FreeSurface;
    public static SDL_VoidPtr SDL_DestroyTexture;
    public static SDL_PtrUIntIntIntIntUIntUIntUIntUInt SDL_CreateRGBSurface;
    public static SDL_PtrPtrIntIntIntIntPixelFormat SDL_CreateRGBSurfaceWithFormatFrom;
    public static SDL_IntPtrRefRectUInt SDL_FillRect;
    public static SDL_UIntPtrByteByteByteByte SDL_MapRGBA;
    public static SDL_IntPtrRefRectPtrRefRect SDL_BlitScaled;
    public static SDL_IntPtrRefRectPtrRefRect SDL_BlitSurface;
    public static SDL_IntPtrBlendMode SDL_SetTextureBlendMode;
    public static SDL_IntPtrOutBlendMode SDL_GetTextureBlendMode;
    public static SDL_IntPtrBlendMode SDL_SetSurfaceBlendMode;
    public static SDL_IntPtrOutBlendMode SDL_GetSurfaceBlendMode;
    public static SDL_PtrPtrPtr SDL_CreateTextureFromSurface;
    public static SDL_IntIntOutRect SDL_GetDisplayBounds;
    public static SDL_IntIntOutRect SDL_GetDisplayUsableBounds;
    public static SDL_VoidUInt SDL_Delay;
    public static SDL_IntOutEvent SDL_PollEvent;
    public static SDL_PtrUInt SDL_GetWindowFromID;
    public static string SDL_GetClipboardText()
    {
        return PtrToStrUTF8(FUNC_SDL_GetClipboardText());
    }
    public static int SDL_SetClipboardText(string Text)
    {
        return FUNC_SDL_SetClipboardText(StrUTF8ToPtr(Text));
    }
    public static SDL_IntPtrOutDisplayMode SDL_GetWindowDisplayMode;
    public static SDL_IntRefMessageBoxDataOutInt SDL_ShowMessageBox;
    public static SDL_IntPtr SDL_LockSurface;
    public static SDL_IntPtr SDL_UnlockSurface;
    public static SDL_IntPtrInt SDL_SetSurfaceRLE;
    public static SDL_IntPtrFlash SDL_FlashWindow;
    public static SDL_IntStr SDL_OpenURL;
    public static SDL_PtrPtrPixelFormatUInt SDL_ConvertSurfaceFormat;
    public static SDL_IntIntFloatFloatFloat SDL_GetDisplayDPI;
    public static SDL_Int SDL_GetNumRenderDrivers;
    public static SDL_IntPtrPtr SDL_SetRenderTarget;
    public static SDL_PtrPtrPixelFormatIntIntInt SDL_CreateTexture;
    public static SDL_IntPtrByteByteByte SDL_SetTextureColorMod;
    public static SDL_VoidPtrSDLBool SDL_SetWindowBordered;
    public static SDL_VoidUintPtrPtrPtrPtrPtr SDL_GetRGBA;
    public static SDL_PtrStr SDL_GL_GetProcAddress;
    public static SDL_IntPtrPtrPtr SDL_GL_BindTexture;
    public static SDL_VoidPtr SDL_GL_SwapWindow;
    public static SDL_BoolPtrWMinfo SDL_GetWindowWMInfo;
    #endregion

    #region Structs
    public struct SDL_Version
    {
        public byte major;
        public byte minor;
        public byte patch;
    }

    public struct SDL_Color
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
    }

    public struct SDL_Surface
    {
        public uint flags;
        public IntPtr format;
        public int w;
        public int h;
        public int pitch;
        public IntPtr pixels;
        public IntPtr userdata;
        public int locked;
        public IntPtr lock_data;
        public SDL_Rect clip_rect;
        public IntPtr map;
        public int refcount;
    }

    public struct SDL_Texture
    {
        public uint format;
        public int access;
        public int w;
        public int h;
        public int modMode;
        public SDL_BlendMode blendMode;
        public byte r;
        public byte g;
        public byte b;
        public byte a;
        public IntPtr renderer;
        public IntPtr native;
        public IntPtr yuv;
        public IntPtr pixels;
        public int pitch;
        public SDL_Rect locked_rect;
        public IntPtr driverdata;
        public IntPtr prev;
        public IntPtr next;
    }

    public struct SDL_DisplayMode
    {
        public uint format;
        public int w;
        public int h;
        public int refresh_rate;
        public IntPtr driverdata;
    }

    public struct SDL_MessageBoxData
    {
        public SDL_MessageBoxFlags flags;
        public IntPtr window;
        public string title;
        public string message;
        public int numbuttons;
        public IntPtr buttons;
        public IntPtr colorScheme;
    }

    public struct SDL_MessageBoxButtonData
    {
        public SDL_MessageBoxButtonFlags flags;
        public int buttonid;
        public IntPtr text;
    }

    public unsafe struct SDL_MessageBoxColorScheme
    {
        public IntPtr colors;
    }

    public struct SDL_MessageBoxColor
    {
        public byte r;
        public byte g;
        public byte b;
    }

    public unsafe struct SDL_RendererInfo
    {
        public IntPtr name;
        public uint flags;
        public uint num_texture_formats;
        public fixed uint texture_formats[16];
        public int max_texture_width;
        public int max_texture_height;
    }

    public struct SDL_Point
    {
        public int x;
        public int y;
    }

    public struct SDL_Rect
    {
        public int x;
        public int y;
        public int w;
        public int h;
    }

    public enum SDL_EventType : uint
    {
        SDL_FIRSTEVENT = 0,
        SDL_QUIT = 0x100,
        SDL_APP_TERMINATING,
        SDL_APP_LOWMEMORY,
        SDL_APP_WILLENTERBACKGROUND,
        SDL_APP_DIDENTERBACKGROUND,
        SDL_APP_WILLENTERFOREGROUND,
        SDL_APP_DIDENTERFOREGROUND,
        SDL_DISPLAYEVENT = 0x150,
        SDL_WINDOWEVENT = 0x200,
        SDL_SYSWMEVENT,
        SDL_KEYDOWN = 0x300,
        SDL_KEYUP,
        SDL_TEXTEDITING,
        SDL_TEXTINPUT,
        SDL_KEYMAPCHANGED,
        SDL_MOUSEMOTION = 0x400,
        SDL_MOUSEBUTTONDOWN,
        SDL_MOUSEBUTTONUP,
        SDL_MOUSEWHEEL,
        SDL_JOYAXISMOTION = 0x600,
        SDL_JOYBALLMOTION,
        SDL_JOYHATMOTION,
        SDL_JOYBUTTONDOWN,
        SDL_JOYBUTTONUP,
        SDL_JOYDEVICEADDED,
        SDL_JOYDEVICEREMOVED,
        SDL_CONTROLLERAXISMOTION = 0x650,
        SDL_CONTROLLERBUTTONDOWN,
        SDL_CONTROLLERBUTTONUP,
        SDL_CONTROLLERDEVICEADDED,
        SDL_CONTROLLERDEVICEREMOVED,
        SDL_CONTROLLERDEVICEREMAPPED,
        SDL_FINGERDOWN = 0x700,
        SDL_FINGERUP,
        SDL_FINGERMOTION,
        SDL_DOLLARGESTURE = 0x800,
        SDL_DOLLARRECORD,
        SDL_MULTIGESTURE,
        SDL_CLIPBOARDUPDATE = 0x900,
        SDL_DROPFILE = 0x1000,
        SDL_DROPTEXT,
        SDL_DROPBEGIN,
        SDL_DROPCOMPLETE,
        SDL_AUDIODEVICEADDED = 0x1100,
        SDL_AUDIODEVICEREMOVED,
        SDL_SENSORUPDATE = 0x1200,
        SDL_RENDER_TARGETS_RESET = 0x2000,
        SDL_RENDER_DEVICE_RESET,
        SDL_USEREVENT = 0x8000,
        SDL_LASTEVENT = 0xFFFF
    }

    public enum SDL_MouseWheelDirection : uint
    {
        SDL_MOUSEWHEEL_NORMAL,
        SDL_MOUSEWHEEL_FLIPPED
    }

    public enum SDL_PixelFormatEnum : uint
    {
        SDL_PIXELFORMAT_UNKNOWN = 0,
        SDL_PIXELFORMAT_INDEX1LSB = 286261504,
        SDL_PIXELFORMAT_INDEX1MSB = 287310080,
        SDL_PIXELFORMAT_INDEX4LSB = 303039488,
        SDL_PIXELFORMAT_INDEX4MSB = 304088064,
        SDL_PIXELFORMAT_INDEX8 = 318769153,
        SDL_PIXELFORMAT_RGB332 = 336660481,
        SDL_PIXELFORMAT_RGB444 = 353504258,
        SDL_PIXELFORMAT_BGR444 = 357698562,
        SDL_PIXELFORMAT_RGB555 = 353570562,
        SDL_PIXELFORMAT_BGR555 = 286461698,
        SDL_PIXELFORMAT_ARGB4444 = 355602434,
        SDL_PIXELFORMAT_RGBA4444 = 356651010,
        SDL_PIXELFORMAT_ABGR4444 = 359796738,
        SDL_PIXELFORMAT_BGRA4444 = 360845314,
        SDL_PIXELFORMAT_ARGB1555 = 355667970,
        SDL_PIXELFORMAT_RGBA5551 = 356782082,
        SDL_PIXELFORMAT_ABGR1555 = 359862274,
        SDL_PIXELFORMAT_BGRA5551 = 360976386,
        SDL_PIXELFORMAT_RGB565 = 353701890,
        SDL_PIXELFORMAT_BGR565 = 353701890,
        SDL_PIXELFORMAT_RGB24 = 386930691,
        SDL_PIXELFORMAT_BGR24 = 390076419,
        SDL_PIXELFORMAT_RGB888 = 370546692,
        SDL_PIXELFORMAT_RGBX8888 = 371595268,
        SDL_PIXELFORMAT_BGR888 = 374740996,
        SDL_PIXELFORMAT_BGRX8888 = 375789572,
        SDL_PIXELFORMAT_ARGB8888 = 372645892,
        SDL_PIXELFORMAT_RGBA8888 = 373694468,
        SDL_PIXELFORMAT_ABGR8888 = 376840196,
        SDL_PIXELFORMAT_BGRA8888 = 377888772,
        SDL_PIXELFORMAT_ARGB2101010 = 372711428,
        SDL_PIXELFORMAT_YV12 = 842094169,
        SDL_PIXELFORMAT_IYUV = 1448433993,
        SDL_PIXELFORMAT_YUY2 = 844715353,
        SDL_PIXELFORMAT_UYVY = 1498831189,
        SDL_PIXELFORMAT_YVYU = 1431918169
    }

    public struct SDL_GenericEvent
    {
        public SDL_EventType type;
        public uint timestamp;
    }

    public struct SDL_DisplayEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public uint display;
        public SDL_DisplayEventID displayevent;
        public byte padding1;
        public byte padding2;
        public byte padding3;
        public Int32 data1;
    }

    public struct SDL_WindowEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public uint windowID;
        public SDL_WindowEventID windowevent;
        public byte padding1;
        public byte padding2;
        public byte padding3;
        public int data1;
        public int data2;
    }

    public struct SDL_KeyboardEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public uint windowID;
        public byte state;
        public byte repeat;
        public byte padding2;
        public byte padding3;
        public SDL_Keysym keysym;
    }

    public unsafe struct SDL_TextEditingEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public uint windowID;
        public fixed byte text[SDL_TEXTEDITINGEVENT_TEXT_SIZE];
        public uint start;
        public uint length;
    }

    public unsafe struct SDL_TextInputEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public uint windowID;
        public fixed byte text[SDL_TEXTINPUTEVENT_TEXT_SIZE];
    }

    public struct SDL_MouseMotionEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public uint windowID;
        public uint which;
        public byte state;
        public byte padding1;
        public byte padding2;
        public byte padding3;
        public int x;
        public int y;
        public int xrel;
        public int yrel;
    }

    public struct SDL_MouseButtonEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public uint windowID;
        public uint which;
        public byte button;
        public byte state;
        public byte clicks;
        public byte padding1;
        public int x;
        public int y;
    }

    public struct SDL_MouseWheelEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public uint windowID;
        public uint which;
        public int x;
        public int y;
        public uint direction;
    }

    public struct SDL_JoyAxisEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte axis;
        public byte padding1;
        public byte padding2;
        public byte padding3;
        public short axisValue;
        public ushort padding4;
    }

    public struct SDL_JoyBallEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte ball;
        public byte padding1;
        public byte padding2;
        public byte padding3;
        public short xrel;
        public short yrel;
    }

    public struct SDL_JoyHatEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte hat;
        public byte hatValue;
        public byte padding1;
        public byte padding2;
    }

    public struct SDL_JoyButtonEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte button;
        public byte state;
        public byte padding1;
        public byte padding2;
    }

    public struct SDL_JoyDeviceEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
    }

    public struct SDL_ControllerAxisEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte axis;
        public byte padding1;
        public byte padding2;
        public byte padding3;
        public short axisValue;
        public ushort padding4;
    }

    public struct SDL_ControllerButtonEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte button;
        public byte state;
        public byte padding1;
        public byte padding2;
    }

    public struct SDL_ControllerDeviceEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
    }

    public struct SDL_AudioDeviceEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public uint which;
        public byte iscapture;
        public byte padding1;
        public byte padding2;
        public byte padding3;
    }

    public struct SDL_TouchFingerEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public long touchId;
        public long fingerId;
        public float x;
        public float y;
        public float dx;
        public float dy;
        public float pressure;
        public uint windowID;
    }

    public struct SDL_MultiGestureEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public long touchId;
        public float dTheta;
        public float dDist;
        public float x;
        public float y;
        public ushort numFingers;
        public ushort padding;
    }

    public struct SDL_DollarGestureEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public long touchId;
        public long gestureId;
        public uint numFingers;
        public float error;
        public float x;
        public float y;
    }

    public struct SDL_DropEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public IntPtr file;
        public uint windowID;
    }

    public unsafe struct SDL_SensorEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public fixed float data[6];
    }

    public struct SDL_QuitEvent
    {
        public SDL_EventType type;
        public uint timestamp;
    }

    public struct SDL_UserEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public uint windowID;
        public int code;
        public IntPtr data1;
        public IntPtr data2;
    }

    public struct SDL_SysWMEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public IntPtr msg;
    }

    public struct SDL_Keysym
    {
        public SDL_Scancode scancode;
        public SDL_Keycode sym;
        public SDL_Keymod mod;
        public uint unicode;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public unsafe struct SDL_Event
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_EventType type;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_EventType typeFSharp;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_DisplayEvent display;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_WindowEvent window;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_KeyboardEvent key;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_TextEditingEvent edit;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_TextInputEvent text;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_MouseMotionEvent motion;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_MouseButtonEvent button;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_MouseWheelEvent wheel;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_JoyAxisEvent jaxis;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_JoyBallEvent jball;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_JoyHatEvent jhat;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_JoyButtonEvent jbutton;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_JoyDeviceEvent jdevice;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_ControllerAxisEvent caxis;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_ControllerButtonEvent cbutton;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_ControllerDeviceEvent cdevice;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_AudioDeviceEvent adevice;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_SensorEvent sensor;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_QuitEvent quit;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_UserEvent user;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_SysWMEvent syswm;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_TouchFingerEvent tfinger;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_MultiGestureEvent mgesture;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_DollarGestureEvent dgesture;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public SDL_DropEvent drop;
        [System.Runtime.InteropServices.FieldOffset(0)]
        private fixed byte padding[56];
    }

    public struct SDL_PixelFormat
    {
        public SDL_PixelFormatEnum format;
        public IntPtr palette;
        public byte bitsperpixel;
        public byte bytesperpixel;
        public uint rmask;
        public uint gmask;
        public uint bmask;
        public uint amask;
    }

    public struct SDL_SysWMinfo
    {
        public SDL_Version version;
        public uint subsystem;
        // Lazy: now follows a union with window data per platform, but we only ever need hWnd for OpenGL purposes.
        public winstruct win;
    }

    public struct winstruct
    {
        public int window;
        // etc.
    }
    #endregion

    #region Enums
    public enum SDL_bool
    {
        SDL_FALSE = 0,
        SDL_TRUE = 1
    }

    public enum SDL_BlendMode : uint
    {
        SDL_BLENDMODE_NONE = 0x00000000,
        SDL_BLENDMODE_BLEND = 0x00000001,
        SDL_BLENDMODE_ADD = 0x00000002,
        SDL_BLENDMODE_MOD = 0x00000004
    }

    public enum SDL_WindowFlags : uint
    {
        SDL_WINDOW_FULLSCREEN = 0x00000001,
        SDL_WINDOW_OPENGL = 0x00000002,
        SDL_WINDOW_SHOWN = 0x00000004,
        SDL_WINDOW_HIDDEN = 0x00000008,
        SDL_WINDOW_BORDERLESS = 0x00000010,
        SDL_WINDOW_RESIZABLE = 0x00000020,
        SDL_WINDOW_MINIMIZED = 0x00000040,
        SDL_WINDOW_MAXIMIZED = 0x00000080,
        SDL_WINDOW_INPUT_GRABBED = 0x00000100,
        SDL_WINDOW_INPUT_FOCUS = 0x00000200,
        SDL_WINDOW_MOUSE_FOCUS = 0x00000400,
        SDL_WINDOW_FULLSCREEN_DESKTOP = (SDL_WINDOW_FULLSCREEN | 0x00001000),
        SDL_WINDOW_FOREIGN = 0x00000800,
        SDL_WINDOW_ALLOW_HIGHDPI = 0x00002000
    }

    public enum SDL_RendererFlags : uint
    {
        SDL_RENDERER_SOFTWARE = 0x00000001,
        SDL_RENDERER_ACCELERATED = 0x00000002,
        SDL_RENDERER_PRESENTVSYNC = 0x00000004,
        SDL_RENDERER_TARGETTEXTURE = 0x00000008
    }

    public enum SDL_RendererFlip : uint
    {
        SDL_FLIP_NONE = 0x00000000,
        SDL_FLIP_HORIZONTAL = 0x00000001,
        SDL_FLIP_VERTICAL = 0x00000002
    }

    public enum SDL_MessageBoxFlags : uint
    {
        SDL_MESSAGEBOX_ERROR = 0x00000010,
        SDL_MESSAGEBOX_WARNING = 0x00000020,
        SDL_MESSAGEBOX_INFORMATION = 0x00000040
    }

    public enum SDL_MessageBoxButtonFlags : uint
    {
        SDL_MESSAGEBOX_BUTTON_RETURNKEY_DEFAULT = 0x00000001,
        SDL_MESSAGEBOX_BUTTON_ESCAPEKEY_DEFAULT = 0x00000002
    }

    public enum SDL_SystemCursor
    {
        SDL_SYSTEM_CURSOR_ARROW,
        SDL_SYSTEM_CURSOR_IBEAM,
        SDL_SYSTEM_CURSOR_WAIT,
        SDL_SYSTEM_CURSOR_CROSSHAIR,
        SDL_SYSTEM_CURSOR_WAITARROW,
        SDL_SYSTEM_CURSOR_SIZENWSE,
        SDL_SYSTEM_CURSOR_SIZENESW,
        SDL_SYSTEM_CURSOR_SIZEWE,
        SDL_SYSTEM_CURSOR_SIZENS,
        SDL_SYSTEM_CURSOR_SIZEALL,
        SDL_SYSTEM_CURSOR_NO,
        SDL_SYSTEM_CURSOR_HAND,
        SDL_NUM_SYSTEM_CURSORS
    }

    public enum SDL_PixelType
    {
        SDL_PIXELTYPE_UNKNOWN,
        SDL_PIXELTYPE_INDEX1,
        SDL_PIXELTYPE_INDEX4,
        SDL_PIXELTYPE_INDEX8,
        SDL_PIXELTYPE_PACKED8,
        SDL_PIXELTYPE_PACKED16,
        SDL_PIXELTYPE_PACKED32,
        SDL_PIXELTYPE_ARRAYU8,
        SDL_PIXELTYPE_ARRAYU16,
        SDL_PIXELTYPE_ARRAYU32,
        SDL_PIXELTYPE_ARRAYF16,
        SDL_PIXELTYPE_ARRAYF32
    }

    public enum SDL_BitmapOrder
    {
        SDL_BITMAPORDER_NONE,
        SDL_BITMAPORDER_4321,
        SDL_BITMAPORDER_1234
    }

    public enum SDL_PackedOrder
    {
        SDL_PACKEDORDER_NONE,
        SDL_PACKEDORDER_XRGB,
        SDL_PACKEDORDER_RGBX,
        SDL_PACKEDORDER_ARGB,
        SDL_PACKEDORDER_RGBA,
        SDL_PACKEDORDER_XBGR,
        SDL_PACKEDORDER_BGRX,
        SDL_PACKEDORDER_ABGR,
        SDL_PACKEDORDER_BGRA
    }

    public enum SDL_ArrayOrder
    {
        SDL_ARRAYORDER_NONE,
        SDL_ARRAYORDER_RGB,
        SDL_ARRAYORDER_RGBA,
        SDL_ARRAYORDER_ARGB,
        SDL_ARRAYORDER_BGR,
        SDL_ARRAYORDER_BGRA,
        SDL_ARRAYORDER_ABGR
    }

    public enum SDL_PackedLayout
    {
        SDL_PACKEDLAYOUT_NONE,
        SDL_PACKEDLAYOUT_332,
        SDL_PACKEDLAYOUT_4444,
        SDL_PACKEDLAYOUT_1555,
        SDL_PACKEDLAYOUT_5551,
        SDL_PACKEDLAYOUT_565,
        SDL_PACKEDLAYOUT_8888,
        SDL_PACKEDLAYOUT_2101010,
        SDL_PACKEDLAYOUT_1010102
    }

    public enum SDL_WindowEventID : byte
    {
        SDL_WINDOWEVENT_NONE,
        SDL_WINDOWEVENT_SHOWN,
        SDL_WINDOWEVENT_HIDDEN,
        SDL_WINDOWEVENT_EXPOSED,
        SDL_WINDOWEVENT_MOVED,
        SDL_WINDOWEVENT_RESIZED,
        SDL_WINDOWEVENT_SIZE_CHANGED,
        SDL_WINDOWEVENT_MINIMIZED,
        SDL_WINDOWEVENT_MAXIMIZED,
        SDL_WINDOWEVENT_RESTORED,
        SDL_WINDOWEVENT_ENTER,
        SDL_WINDOWEVENT_LEAVE,
        SDL_WINDOWEVENT_FOCUS_GAINED,
        SDL_WINDOWEVENT_FOCUS_LOST,
        SDL_WINDOWEVENT_CLOSE,
        SDL_WINDOWEVENT_TAKE_FOCUS,
        SDL_WINDOWEVENT_HIT_TEST
    }

    public enum SDL_DisplayEventID : byte
    {
        SDL_DISPLAYEVENT_NONE,
        SDL_DISPLAYEVENT_ORIENTATION
    }

    public enum SDL_Keymod : ushort
    {
        KMOD_NONE = 0x0000,
        KMOD_LSHIFT = 0x0001,
        KMOD_RSHIFT = 0x0002,
        KMOD_LCTRL = 0x0040,
        KMOD_RCTRL = 0x0080,
        KMOD_LALT = 0x0100,
        KMOD_RALT = 0x0200,
        KMOD_LGUI = 0x0400,
        KMOD_RGUI = 0x0800,
        KMOD_NUM = 0x1000,
        KMOD_CAPS = 0x2000,
        KMOD_MODE = 0x4000,
        KMOD_RESERVED = 0x8000,
        KMOD_CTRL = (KMOD_LCTRL | KMOD_RCTRL),
        KMOD_SHIFT = (KMOD_LSHIFT | KMOD_RSHIFT),
        KMOD_ALT = (KMOD_LALT | KMOD_RALT),
        KMOD_GUI = (KMOD_LGUI | KMOD_RGUI)
    }

    public enum SDL_Scancode
    {
        SDL_SCANCODE_UNKNOWN = 0,
        SDL_SCANCODE_A = 4,
        SDL_SCANCODE_B = 5,
        SDL_SCANCODE_C = 6,
        SDL_SCANCODE_D = 7,
        SDL_SCANCODE_E = 8,
        SDL_SCANCODE_F = 9,
        SDL_SCANCODE_G = 10,
        SDL_SCANCODE_H = 11,
        SDL_SCANCODE_I = 12,
        SDL_SCANCODE_J = 13,
        SDL_SCANCODE_K = 14,
        SDL_SCANCODE_L = 15,
        SDL_SCANCODE_M = 16,
        SDL_SCANCODE_N = 17,
        SDL_SCANCODE_O = 18,
        SDL_SCANCODE_P = 19,
        SDL_SCANCODE_Q = 20,
        SDL_SCANCODE_R = 21,
        SDL_SCANCODE_S = 22,
        SDL_SCANCODE_T = 23,
        SDL_SCANCODE_U = 24,
        SDL_SCANCODE_V = 25,
        SDL_SCANCODE_W = 26,
        SDL_SCANCODE_X = 27,
        SDL_SCANCODE_Y = 28,
        SDL_SCANCODE_Z = 29,
        SDL_SCANCODE_1 = 30,
        SDL_SCANCODE_2 = 31,
        SDL_SCANCODE_3 = 32,
        SDL_SCANCODE_4 = 33,
        SDL_SCANCODE_5 = 34,
        SDL_SCANCODE_6 = 35,
        SDL_SCANCODE_7 = 36,
        SDL_SCANCODE_8 = 37,
        SDL_SCANCODE_9 = 38,
        SDL_SCANCODE_0 = 39,
        SDL_SCANCODE_RETURN = 40,
        SDL_SCANCODE_ESCAPE = 41,
        SDL_SCANCODE_BACKSPACE = 42,
        SDL_SCANCODE_TAB = 43,
        SDL_SCANCODE_SPACE = 44,
        SDL_SCANCODE_MINUS = 45,
        SDL_SCANCODE_EQUALS = 46,
        SDL_SCANCODE_LEFTBRACKET = 47,
        SDL_SCANCODE_RIGHTBRACKET = 48,
        SDL_SCANCODE_BACKSLASH = 49,
        SDL_SCANCODE_NONUSHASH = 50,
        SDL_SCANCODE_SEMICOLON = 51,
        SDL_SCANCODE_APOSTROPHE = 52,
        SDL_SCANCODE_GRAVE = 53,
        SDL_SCANCODE_COMMA = 54,
        SDL_SCANCODE_PERIOD = 55,
        SDL_SCANCODE_SLASH = 56,
        SDL_SCANCODE_CAPSLOCK = 57,
        SDL_SCANCODE_F1 = 58,
        SDL_SCANCODE_F2 = 59,
        SDL_SCANCODE_F3 = 60,
        SDL_SCANCODE_F4 = 61,
        SDL_SCANCODE_F5 = 62,
        SDL_SCANCODE_F6 = 63,
        SDL_SCANCODE_F7 = 64,
        SDL_SCANCODE_F8 = 65,
        SDL_SCANCODE_F9 = 66,
        SDL_SCANCODE_F10 = 67,
        SDL_SCANCODE_F11 = 68,
        SDL_SCANCODE_F12 = 69,
        SDL_SCANCODE_PRINTSCREEN = 70,
        SDL_SCANCODE_SCROLLLOCK = 71,
        SDL_SCANCODE_PAUSE = 72,
        SDL_SCANCODE_INSERT = 73,
        SDL_SCANCODE_HOME = 74,
        SDL_SCANCODE_PAGEUP = 75,
        SDL_SCANCODE_DELETE = 76,
        SDL_SCANCODE_END = 77,
        SDL_SCANCODE_PAGEDOWN = 78,
        SDL_SCANCODE_RIGHT = 79,
        SDL_SCANCODE_LEFT = 80,
        SDL_SCANCODE_DOWN = 81,
        SDL_SCANCODE_UP = 82,
        SDL_SCANCODE_NUMLOCKCLEAR = 83,
        SDL_SCANCODE_KP_DIVIDE = 84,
        SDL_SCANCODE_KP_MULTIPLY = 85,
        SDL_SCANCODE_KP_MINUS = 86,
        SDL_SCANCODE_KP_PLUS = 87,
        SDL_SCANCODE_KP_ENTER = 88,
        SDL_SCANCODE_KP_1 = 89,
        SDL_SCANCODE_KP_2 = 90,
        SDL_SCANCODE_KP_3 = 91,
        SDL_SCANCODE_KP_4 = 92,
        SDL_SCANCODE_KP_5 = 93,
        SDL_SCANCODE_KP_6 = 94,
        SDL_SCANCODE_KP_7 = 95,
        SDL_SCANCODE_KP_8 = 96,
        SDL_SCANCODE_KP_9 = 97,
        SDL_SCANCODE_KP_0 = 98,
        SDL_SCANCODE_KP_PERIOD = 99,
        SDL_SCANCODE_NONUSBACKSLASH = 100,
        SDL_SCANCODE_APPLICATION = 101,
        SDL_SCANCODE_POWER = 102,
        SDL_SCANCODE_KP_EQUALS = 103,
        SDL_SCANCODE_F13 = 104,
        SDL_SCANCODE_F14 = 105,
        SDL_SCANCODE_F15 = 106,
        SDL_SCANCODE_F16 = 107,
        SDL_SCANCODE_F17 = 108,
        SDL_SCANCODE_F18 = 109,
        SDL_SCANCODE_F19 = 110,
        SDL_SCANCODE_F20 = 111,
        SDL_SCANCODE_F21 = 112,
        SDL_SCANCODE_F22 = 113,
        SDL_SCANCODE_F23 = 114,
        SDL_SCANCODE_F24 = 115,
        SDL_SCANCODE_EXECUTE = 116,
        SDL_SCANCODE_HELP = 117,
        SDL_SCANCODE_MENU = 118,
        SDL_SCANCODE_SELECT = 119,
        SDL_SCANCODE_STOP = 120,
        SDL_SCANCODE_AGAIN = 121,
        SDL_SCANCODE_UNDO = 122,
        SDL_SCANCODE_CUT = 123,
        SDL_SCANCODE_COPY = 124,
        SDL_SCANCODE_PASTE = 125,
        SDL_SCANCODE_FIND = 126,
        SDL_SCANCODE_MUTE = 127,
        SDL_SCANCODE_VOLUMEUP = 128,
        SDL_SCANCODE_VOLUMEDOWN = 129,
        SDL_SCANCODE_LOCKINGCAPSLOCK = 130,
        SDL_SCANCODE_LOCKINGNUMLOCK = 131,
        SDL_SCANCODE_LOCKINGSCROLLLOCK = 132,
        SDL_SCANCODE_KP_COMMA = 133,
        SDL_SCANCODE_KP_EQUALSAS400 = 134,
        SDL_SCANCODE_INTERNATIONAL1 = 135,
        SDL_SCANCODE_INTERNATIONAL2 = 136,
        SDL_SCANCODE_INTERNATIONAL3 = 137,
        SDL_SCANCODE_INTERNATIONAL4 = 138,
        SDL_SCANCODE_INTERNATIONAL5 = 139,
        SDL_SCANCODE_INTERNATIONAL6 = 140,
        SDL_SCANCODE_INTERNATIONAL7 = 141,
        SDL_SCANCODE_INTERNATIONAL8 = 142,
        SDL_SCANCODE_INTERNATIONAL9 = 143,
        SDL_SCANCODE_LANG1 = 144,
        SDL_SCANCODE_LANG2 = 145,
        SDL_SCANCODE_LANG3 = 146,
        SDL_SCANCODE_LANG4 = 147,
        SDL_SCANCODE_LANG5 = 148,
        SDL_SCANCODE_LANG6 = 149,
        SDL_SCANCODE_LANG7 = 150,
        SDL_SCANCODE_LANG8 = 151,
        SDL_SCANCODE_LANG9 = 152,
        SDL_SCANCODE_ALTERASE = 153,
        SDL_SCANCODE_SYSREQ = 154,
        SDL_SCANCODE_CANCEL = 155,
        SDL_SCANCODE_CLEAR = 156,
        SDL_SCANCODE_PRIOR = 157,
        SDL_SCANCODE_RETURN2 = 158,
        SDL_SCANCODE_SEPARATOR = 159,
        SDL_SCANCODE_OUT = 160,
        SDL_SCANCODE_OPER = 161,
        SDL_SCANCODE_CLEARAGAIN = 162,
        SDL_SCANCODE_CRSEL = 163,
        SDL_SCANCODE_EXSEL = 164,
        SDL_SCANCODE_KP_00 = 176,
        SDL_SCANCODE_KP_000 = 177,
        SDL_SCANCODE_THOUSANDSSEPARATOR = 178,
        SDL_SCANCODE_DECIMALSEPARATOR = 179,
        SDL_SCANCODE_CURRENCYUNIT = 180,
        SDL_SCANCODE_CURRENCYSUBUNIT = 181,
        SDL_SCANCODE_KP_LEFTPAREN = 182,
        SDL_SCANCODE_KP_RIGHTPAREN = 183,
        SDL_SCANCODE_KP_LEFTBRACE = 184,
        SDL_SCANCODE_KP_RIGHTBRACE = 185,
        SDL_SCANCODE_KP_TAB = 186,
        SDL_SCANCODE_KP_BACKSPACE = 187,
        SDL_SCANCODE_KP_A = 188,
        SDL_SCANCODE_KP_B = 189,
        SDL_SCANCODE_KP_C = 190,
        SDL_SCANCODE_KP_D = 191,
        SDL_SCANCODE_KP_E = 192,
        SDL_SCANCODE_KP_F = 193,
        SDL_SCANCODE_KP_XOR = 194,
        SDL_SCANCODE_KP_POWER = 195,
        SDL_SCANCODE_KP_PERCENT = 196,
        SDL_SCANCODE_KP_LESS = 197,
        SDL_SCANCODE_KP_GREATER = 198,
        SDL_SCANCODE_KP_AMPERSAND = 199,
        SDL_SCANCODE_KP_DBLAMPERSAND = 200,
        SDL_SCANCODE_KP_VERTICALBAR = 201,
        SDL_SCANCODE_KP_DBLVERTICALBAR = 202,
        SDL_SCANCODE_KP_COLON = 203,
        SDL_SCANCODE_KP_HASH = 204,
        SDL_SCANCODE_KP_SPACE = 205,
        SDL_SCANCODE_KP_AT = 206,
        SDL_SCANCODE_KP_EXCLAM = 207,
        SDL_SCANCODE_KP_MEMSTORE = 208,
        SDL_SCANCODE_KP_MEMRECALL = 209,
        SDL_SCANCODE_KP_MEMCLEAR = 210,
        SDL_SCANCODE_KP_MEMADD = 211,
        SDL_SCANCODE_KP_MEMSUBTRACT = 212,
        SDL_SCANCODE_KP_MEMMULTIPLY = 213,
        SDL_SCANCODE_KP_MEMDIVIDE = 214,
        SDL_SCANCODE_KP_PLUSMINUS = 215,
        SDL_SCANCODE_KP_CLEAR = 216,
        SDL_SCANCODE_KP_CLEARENTRY = 217,
        SDL_SCANCODE_KP_BINARY = 218,
        SDL_SCANCODE_KP_OCTAL = 219,
        SDL_SCANCODE_KP_DECIMAL = 220,
        SDL_SCANCODE_KP_HEXADECIMAL = 221,
        SDL_SCANCODE_LCTRL = 224,
        SDL_SCANCODE_LSHIFT = 225,
        SDL_SCANCODE_LALT = 226,
        SDL_SCANCODE_LGUI = 227,
        SDL_SCANCODE_RCTRL = 228,
        SDL_SCANCODE_RSHIFT = 229,
        SDL_SCANCODE_RALT = 230,
        SDL_SCANCODE_RGUI = 231,
        SDL_SCANCODE_MODE = 257,
        SDL_SCANCODE_AUDIONEXT = 258,
        SDL_SCANCODE_AUDIOPREV = 259,
        SDL_SCANCODE_AUDIOSTOP = 260,
        SDL_SCANCODE_AUDIOPLAY = 261,
        SDL_SCANCODE_AUDIOMUTE = 262,
        SDL_SCANCODE_MEDIASELECT = 263,
        SDL_SCANCODE_WWW = 264,
        SDL_SCANCODE_MAIL = 265,
        SDL_SCANCODE_CALCULATOR = 266,
        SDL_SCANCODE_COMPUTER = 267,
        SDL_SCANCODE_AC_SEARCH = 268,
        SDL_SCANCODE_AC_HOME = 269,
        SDL_SCANCODE_AC_BACK = 270,
        SDL_SCANCODE_AC_FORWARD = 271,
        SDL_SCANCODE_AC_STOP = 272,
        SDL_SCANCODE_AC_REFRESH = 273,
        SDL_SCANCODE_AC_BOOKMARKS = 274,
        SDL_SCANCODE_BRIGHTNESSDOWN = 275,
        SDL_SCANCODE_BRIGHTNESSUP = 276,
        SDL_SCANCODE_DISPLAYSWITCH = 277,
        SDL_SCANCODE_KBDILLUMTOGGLE = 278,
        SDL_SCANCODE_KBDILLUMDOWN = 279,
        SDL_SCANCODE_KBDILLUMUP = 280,
        SDL_SCANCODE_EJECT = 281,
        SDL_SCANCODE_SLEEP = 282,
        SDL_SCANCODE_APP1 = 283,
        SDL_SCANCODE_APP2 = 284,
        SDL_SCANCODE_AUDIOREWIND = 285,
        SDL_SCANCODE_AUDIOFASTFORWARD = 286,
        SDL_NUM_SCANCODES = 512
    }

    public enum SDL_Keycode
    {
        SDLK_UNKNOWN = 0,
        SDLK_RETURN = '\r',
        SDLK_ESCAPE = 27,
        SDLK_BACKSPACE = '\b',
        SDLK_TAB = '\t',
        SDLK_SPACE = ' ',
        SDLK_EXCLAIM = '!',
        SDLK_QUOTEDBL = '"',
        SDLK_HASH = '#',
        SDLK_PERCENT = '%',
        SDLK_DOLLAR = '$',
        SDLK_AMPERSAND = '&',
        SDLK_QUOTE = '\'',
        SDLK_LEFTPAREN = '(',
        SDLK_RIGHTPAREN = ')',
        SDLK_ASTERISK = '*',
        SDLK_PLUS = '+',
        SDLK_COMMA = ',',
        SDLK_MINUS = '-',
        SDLK_PERIOD = '.',
        SDLK_SLASH = '/',
        SDLK_0 = '0',
        SDLK_1 = '1',
        SDLK_2 = '2',
        SDLK_3 = '3',
        SDLK_4 = '4',
        SDLK_5 = '5',
        SDLK_6 = '6',
        SDLK_7 = '7',
        SDLK_8 = '8',
        SDLK_9 = '9',
        SDLK_COLON = ':',
        SDLK_SEMICOLON = ';',
        SDLK_LESS = '<',
        SDLK_EQUALS = '=',
        SDLK_GREATER = '>',
        SDLK_QUESTION = '?',
        SDLK_AT = '@',
        SDLK_LEFTBRACKET = '[',
        SDLK_BACKSLASH = '\\',
        SDLK_RIGHTBRACKET = ']',
        SDLK_CARET = '^',
        SDLK_UNDERSCORE = '_',
        SDLK_BACKQUOTE = '`',
        SDLK_a = 'a',
        SDLK_b = 'b',
        SDLK_c = 'c',
        SDLK_d = 'd',
        SDLK_e = 'e',
        SDLK_f = 'f',
        SDLK_g = 'g',
        SDLK_h = 'h',
        SDLK_i = 'i',
        SDLK_j = 'j',
        SDLK_k = 'k',
        SDLK_l = 'l',
        SDLK_m = 'm',
        SDLK_n = 'n',
        SDLK_o = 'o',
        SDLK_p = 'p',
        SDLK_q = 'q',
        SDLK_r = 'r',
        SDLK_s = 's',
        SDLK_t = 't',
        SDLK_u = 'u',
        SDLK_v = 'v',
        SDLK_w = 'w',
        SDLK_x = 'x',
        SDLK_y = 'y',
        SDLK_z = 'z',
        SDLK_CAPSLOCK = (int)SDL_Scancode.SDL_SCANCODE_CAPSLOCK | SDLK_SCANCODE_MASK,
        SDLK_F1 = (int)SDL_Scancode.SDL_SCANCODE_F1 | SDLK_SCANCODE_MASK,
        SDLK_F2 = (int)SDL_Scancode.SDL_SCANCODE_F2 | SDLK_SCANCODE_MASK,
        SDLK_F3 = (int)SDL_Scancode.SDL_SCANCODE_F3 | SDLK_SCANCODE_MASK,
        SDLK_F4 = (int)SDL_Scancode.SDL_SCANCODE_F4 | SDLK_SCANCODE_MASK,
        SDLK_F5 = (int)SDL_Scancode.SDL_SCANCODE_F5 | SDLK_SCANCODE_MASK,
        SDLK_F6 = (int)SDL_Scancode.SDL_SCANCODE_F6 | SDLK_SCANCODE_MASK,
        SDLK_F7 = (int)SDL_Scancode.SDL_SCANCODE_F7 | SDLK_SCANCODE_MASK,
        SDLK_F8 = (int)SDL_Scancode.SDL_SCANCODE_F8 | SDLK_SCANCODE_MASK,
        SDLK_F9 = (int)SDL_Scancode.SDL_SCANCODE_F9 | SDLK_SCANCODE_MASK,
        SDLK_F10 = (int)SDL_Scancode.SDL_SCANCODE_F10 | SDLK_SCANCODE_MASK,
        SDLK_F11 = (int)SDL_Scancode.SDL_SCANCODE_F11 | SDLK_SCANCODE_MASK,
        SDLK_F12 = (int)SDL_Scancode.SDL_SCANCODE_F12 | SDLK_SCANCODE_MASK,
        SDLK_PRINTSCREEN = (int)SDL_Scancode.SDL_SCANCODE_PRINTSCREEN | SDLK_SCANCODE_MASK,
        SDLK_SCROLLLOCK = (int)SDL_Scancode.SDL_SCANCODE_SCROLLLOCK | SDLK_SCANCODE_MASK,
        SDLK_PAUSE = (int)SDL_Scancode.SDL_SCANCODE_PAUSE | SDLK_SCANCODE_MASK,
        SDLK_INSERT = (int)SDL_Scancode.SDL_SCANCODE_INSERT | SDLK_SCANCODE_MASK,
        SDLK_HOME = (int)SDL_Scancode.SDL_SCANCODE_HOME | SDLK_SCANCODE_MASK,
        SDLK_PAGEUP = (int)SDL_Scancode.SDL_SCANCODE_PAGEUP | SDLK_SCANCODE_MASK,
        SDLK_DELETE = 127,
        SDLK_END = (int)SDL_Scancode.SDL_SCANCODE_END | SDLK_SCANCODE_MASK,
        SDLK_PAGEDOWN = (int)SDL_Scancode.SDL_SCANCODE_PAGEDOWN | SDLK_SCANCODE_MASK,
        SDLK_RIGHT = (int)SDL_Scancode.SDL_SCANCODE_RIGHT | SDLK_SCANCODE_MASK,
        SDLK_LEFT = (int)SDL_Scancode.SDL_SCANCODE_LEFT | SDLK_SCANCODE_MASK,
        SDLK_DOWN = (int)SDL_Scancode.SDL_SCANCODE_DOWN | SDLK_SCANCODE_MASK,
        SDLK_UP = (int)SDL_Scancode.SDL_SCANCODE_UP | SDLK_SCANCODE_MASK,
        SDLK_NUMLOCKCLEAR = (int)SDL_Scancode.SDL_SCANCODE_NUMLOCKCLEAR | SDLK_SCANCODE_MASK,
        SDLK_KP_DIVIDE = (int)SDL_Scancode.SDL_SCANCODE_KP_DIVIDE | SDLK_SCANCODE_MASK,
        SDLK_KP_MULTIPLY = (int)SDL_Scancode.SDL_SCANCODE_KP_MULTIPLY | SDLK_SCANCODE_MASK,
        SDLK_KP_MINUS = (int)SDL_Scancode.SDL_SCANCODE_KP_MINUS | SDLK_SCANCODE_MASK,
        SDLK_KP_PLUS = (int)SDL_Scancode.SDL_SCANCODE_KP_PLUS | SDLK_SCANCODE_MASK,
        SDLK_KP_ENTER = (int)SDL_Scancode.SDL_SCANCODE_KP_ENTER | SDLK_SCANCODE_MASK,
        SDLK_KP_1 = (int)SDL_Scancode.SDL_SCANCODE_KP_1 | SDLK_SCANCODE_MASK,
        SDLK_KP_2 = (int)SDL_Scancode.SDL_SCANCODE_KP_2 | SDLK_SCANCODE_MASK,
        SDLK_KP_3 = (int)SDL_Scancode.SDL_SCANCODE_KP_3 | SDLK_SCANCODE_MASK,
        SDLK_KP_4 = (int)SDL_Scancode.SDL_SCANCODE_KP_4 | SDLK_SCANCODE_MASK,
        SDLK_KP_5 = (int)SDL_Scancode.SDL_SCANCODE_KP_5 | SDLK_SCANCODE_MASK,
        SDLK_KP_6 = (int)SDL_Scancode.SDL_SCANCODE_KP_6 | SDLK_SCANCODE_MASK,
        SDLK_KP_7 = (int)SDL_Scancode.SDL_SCANCODE_KP_7 | SDLK_SCANCODE_MASK,
        SDLK_KP_8 = (int)SDL_Scancode.SDL_SCANCODE_KP_8 | SDLK_SCANCODE_MASK,
        SDLK_KP_9 = (int)SDL_Scancode.SDL_SCANCODE_KP_9 | SDLK_SCANCODE_MASK,
        SDLK_KP_0 = (int)SDL_Scancode.SDL_SCANCODE_KP_0 | SDLK_SCANCODE_MASK,
        SDLK_KP_PERIOD = (int)SDL_Scancode.SDL_SCANCODE_KP_PERIOD | SDLK_SCANCODE_MASK,
        SDLK_APPLICATION = (int)SDL_Scancode.SDL_SCANCODE_APPLICATION | SDLK_SCANCODE_MASK,
        SDLK_POWER = (int)SDL_Scancode.SDL_SCANCODE_POWER | SDLK_SCANCODE_MASK,
        SDLK_KP_EQUALS = (int)SDL_Scancode.SDL_SCANCODE_KP_EQUALS | SDLK_SCANCODE_MASK,
        SDLK_F13 = (int)SDL_Scancode.SDL_SCANCODE_F13 | SDLK_SCANCODE_MASK,
        SDLK_F14 = (int)SDL_Scancode.SDL_SCANCODE_F14 | SDLK_SCANCODE_MASK,
        SDLK_F15 = (int)SDL_Scancode.SDL_SCANCODE_F15 | SDLK_SCANCODE_MASK,
        SDLK_F16 = (int)SDL_Scancode.SDL_SCANCODE_F16 | SDLK_SCANCODE_MASK,
        SDLK_F17 = (int)SDL_Scancode.SDL_SCANCODE_F17 | SDLK_SCANCODE_MASK,
        SDLK_F18 = (int)SDL_Scancode.SDL_SCANCODE_F18 | SDLK_SCANCODE_MASK,
        SDLK_F19 = (int)SDL_Scancode.SDL_SCANCODE_F19 | SDLK_SCANCODE_MASK,
        SDLK_F20 = (int)SDL_Scancode.SDL_SCANCODE_F20 | SDLK_SCANCODE_MASK,
        SDLK_F21 = (int)SDL_Scancode.SDL_SCANCODE_F21 | SDLK_SCANCODE_MASK,
        SDLK_F22 = (int)SDL_Scancode.SDL_SCANCODE_F22 | SDLK_SCANCODE_MASK,
        SDLK_F23 = (int)SDL_Scancode.SDL_SCANCODE_F23 | SDLK_SCANCODE_MASK,
        SDLK_F24 = (int)SDL_Scancode.SDL_SCANCODE_F24 | SDLK_SCANCODE_MASK,
        SDLK_EXECUTE = (int)SDL_Scancode.SDL_SCANCODE_EXECUTE | SDLK_SCANCODE_MASK,
        SDLK_HELP = (int)SDL_Scancode.SDL_SCANCODE_HELP | SDLK_SCANCODE_MASK,
        SDLK_MENU = (int)SDL_Scancode.SDL_SCANCODE_MENU | SDLK_SCANCODE_MASK,
        SDLK_SELECT = (int)SDL_Scancode.SDL_SCANCODE_SELECT | SDLK_SCANCODE_MASK,
        SDLK_STOP = (int)SDL_Scancode.SDL_SCANCODE_STOP | SDLK_SCANCODE_MASK,
        SDLK_AGAIN = (int)SDL_Scancode.SDL_SCANCODE_AGAIN | SDLK_SCANCODE_MASK,
        SDLK_UNDO = (int)SDL_Scancode.SDL_SCANCODE_UNDO | SDLK_SCANCODE_MASK,
        SDLK_CUT = (int)SDL_Scancode.SDL_SCANCODE_CUT | SDLK_SCANCODE_MASK,
        SDLK_COPY = (int)SDL_Scancode.SDL_SCANCODE_COPY | SDLK_SCANCODE_MASK,
        SDLK_PASTE = (int)SDL_Scancode.SDL_SCANCODE_PASTE | SDLK_SCANCODE_MASK,
        SDLK_FIND = (int)SDL_Scancode.SDL_SCANCODE_FIND | SDLK_SCANCODE_MASK,
        SDLK_MUTE = (int)SDL_Scancode.SDL_SCANCODE_MUTE | SDLK_SCANCODE_MASK,
        SDLK_VOLUMEUP = (int)SDL_Scancode.SDL_SCANCODE_VOLUMEUP | SDLK_SCANCODE_MASK,
        SDLK_VOLUMEDOWN = (int)SDL_Scancode.SDL_SCANCODE_VOLUMEDOWN | SDLK_SCANCODE_MASK,
        SDLK_KP_COMMA = (int)SDL_Scancode.SDL_SCANCODE_KP_COMMA | SDLK_SCANCODE_MASK,
        SDLK_KP_EQUALSAS400 = (int)SDL_Scancode.SDL_SCANCODE_KP_EQUALSAS400 | SDLK_SCANCODE_MASK,
        SDLK_ALTERASE = (int)SDL_Scancode.SDL_SCANCODE_ALTERASE | SDLK_SCANCODE_MASK,
        SDLK_SYSREQ = (int)SDL_Scancode.SDL_SCANCODE_SYSREQ | SDLK_SCANCODE_MASK,
        SDLK_CANCEL = (int)SDL_Scancode.SDL_SCANCODE_CANCEL | SDLK_SCANCODE_MASK,
        SDLK_CLEAR = (int)SDL_Scancode.SDL_SCANCODE_CLEAR | SDLK_SCANCODE_MASK,
        SDLK_PRIOR = (int)SDL_Scancode.SDL_SCANCODE_PRIOR | SDLK_SCANCODE_MASK,
        SDLK_RETURN2 = (int)SDL_Scancode.SDL_SCANCODE_RETURN2 | SDLK_SCANCODE_MASK,
        SDLK_SEPARATOR = (int)SDL_Scancode.SDL_SCANCODE_SEPARATOR | SDLK_SCANCODE_MASK,
        SDLK_OUT = (int)SDL_Scancode.SDL_SCANCODE_OUT | SDLK_SCANCODE_MASK,
        SDLK_OPER = (int)SDL_Scancode.SDL_SCANCODE_OPER | SDLK_SCANCODE_MASK,
        SDLK_CLEARAGAIN = (int)SDL_Scancode.SDL_SCANCODE_CLEARAGAIN | SDLK_SCANCODE_MASK,
        SDLK_CRSEL = (int)SDL_Scancode.SDL_SCANCODE_CRSEL | SDLK_SCANCODE_MASK,
        SDLK_EXSEL = (int)SDL_Scancode.SDL_SCANCODE_EXSEL | SDLK_SCANCODE_MASK,
        SDLK_KP_00 = (int)SDL_Scancode.SDL_SCANCODE_KP_00 | SDLK_SCANCODE_MASK,
        SDLK_KP_000 = (int)SDL_Scancode.SDL_SCANCODE_KP_000 | SDLK_SCANCODE_MASK,
        SDLK_THOUSANDSSEPARATOR = (int)SDL_Scancode.SDL_SCANCODE_THOUSANDSSEPARATOR | SDLK_SCANCODE_MASK,
        SDLK_DECIMALSEPARATOR = (int)SDL_Scancode.SDL_SCANCODE_DECIMALSEPARATOR | SDLK_SCANCODE_MASK,
        SDLK_CURRENCYUNIT = (int)SDL_Scancode.SDL_SCANCODE_CURRENCYUNIT | SDLK_SCANCODE_MASK,
        SDLK_CURRENCYSUBUNIT = (int)SDL_Scancode.SDL_SCANCODE_CURRENCYSUBUNIT | SDLK_SCANCODE_MASK,
        SDLK_KP_LEFTPAREN = (int)SDL_Scancode.SDL_SCANCODE_KP_LEFTPAREN | SDLK_SCANCODE_MASK,
        SDLK_KP_RIGHTPAREN = (int)SDL_Scancode.SDL_SCANCODE_KP_RIGHTPAREN | SDLK_SCANCODE_MASK,
        SDLK_KP_LEFTBRACE = (int)SDL_Scancode.SDL_SCANCODE_KP_LEFTBRACE | SDLK_SCANCODE_MASK,
        SDLK_KP_RIGHTBRACE = (int)SDL_Scancode.SDL_SCANCODE_KP_RIGHTBRACE | SDLK_SCANCODE_MASK,
        SDLK_KP_TAB = (int)SDL_Scancode.SDL_SCANCODE_KP_TAB | SDLK_SCANCODE_MASK,
        SDLK_KP_BACKSPACE = (int)SDL_Scancode.SDL_SCANCODE_KP_BACKSPACE | SDLK_SCANCODE_MASK,
        SDLK_KP_A = (int)SDL_Scancode.SDL_SCANCODE_KP_A | SDLK_SCANCODE_MASK,
        SDLK_KP_B = (int)SDL_Scancode.SDL_SCANCODE_KP_B | SDLK_SCANCODE_MASK,
        SDLK_KP_C = (int)SDL_Scancode.SDL_SCANCODE_KP_C | SDLK_SCANCODE_MASK,
        SDLK_KP_D = (int)SDL_Scancode.SDL_SCANCODE_KP_D | SDLK_SCANCODE_MASK,
        SDLK_KP_E = (int)SDL_Scancode.SDL_SCANCODE_KP_E | SDLK_SCANCODE_MASK,
        SDLK_KP_F = (int)SDL_Scancode.SDL_SCANCODE_KP_F | SDLK_SCANCODE_MASK,
        SDLK_KP_XOR = (int)SDL_Scancode.SDL_SCANCODE_KP_XOR | SDLK_SCANCODE_MASK,
        SDLK_KP_POWER = (int)SDL_Scancode.SDL_SCANCODE_KP_POWER | SDLK_SCANCODE_MASK,
        SDLK_KP_PERCENT = (int)SDL_Scancode.SDL_SCANCODE_KP_PERCENT | SDLK_SCANCODE_MASK,
        SDLK_KP_LESS = (int)SDL_Scancode.SDL_SCANCODE_KP_LESS | SDLK_SCANCODE_MASK,
        SDLK_KP_GREATER = (int)SDL_Scancode.SDL_SCANCODE_KP_GREATER | SDLK_SCANCODE_MASK,
        SDLK_KP_AMPERSAND = (int)SDL_Scancode.SDL_SCANCODE_KP_AMPERSAND | SDLK_SCANCODE_MASK,
        SDLK_KP_DBLAMPERSAND = (int)SDL_Scancode.SDL_SCANCODE_KP_DBLAMPERSAND | SDLK_SCANCODE_MASK,
        SDLK_KP_VERTICALBAR = (int)SDL_Scancode.SDL_SCANCODE_KP_VERTICALBAR | SDLK_SCANCODE_MASK,
        SDLK_KP_DBLVERTICALBAR = (int)SDL_Scancode.SDL_SCANCODE_KP_DBLVERTICALBAR | SDLK_SCANCODE_MASK,
        SDLK_KP_COLON = (int)SDL_Scancode.SDL_SCANCODE_KP_COLON | SDLK_SCANCODE_MASK,
        SDLK_KP_HASH = (int)SDL_Scancode.SDL_SCANCODE_KP_HASH | SDLK_SCANCODE_MASK,
        SDLK_KP_SPACE = (int)SDL_Scancode.SDL_SCANCODE_KP_SPACE | SDLK_SCANCODE_MASK,
        SDLK_KP_AT = (int)SDL_Scancode.SDL_SCANCODE_KP_AT | SDLK_SCANCODE_MASK,
        SDLK_KP_EXCLAM = (int)SDL_Scancode.SDL_SCANCODE_KP_EXCLAM | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMSTORE = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMSTORE | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMRECALL = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMRECALL | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMCLEAR = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMCLEAR | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMADD = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMADD | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMSUBTRACT = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMSUBTRACT | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMMULTIPLY = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMMULTIPLY | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMDIVIDE = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMDIVIDE | SDLK_SCANCODE_MASK,
        SDLK_KP_PLUSMINUS = (int)SDL_Scancode.SDL_SCANCODE_KP_PLUSMINUS | SDLK_SCANCODE_MASK,
        SDLK_KP_CLEAR = (int)SDL_Scancode.SDL_SCANCODE_KP_CLEAR | SDLK_SCANCODE_MASK,
        SDLK_KP_CLEARENTRY = (int)SDL_Scancode.SDL_SCANCODE_KP_CLEARENTRY | SDLK_SCANCODE_MASK,
        SDLK_KP_BINARY = (int)SDL_Scancode.SDL_SCANCODE_KP_BINARY | SDLK_SCANCODE_MASK,
        SDLK_KP_OCTAL = (int)SDL_Scancode.SDL_SCANCODE_KP_OCTAL | SDLK_SCANCODE_MASK,
        SDLK_KP_DECIMAL = (int)SDL_Scancode.SDL_SCANCODE_KP_DECIMAL | SDLK_SCANCODE_MASK,
        SDLK_KP_HEXADECIMAL = (int)SDL_Scancode.SDL_SCANCODE_KP_HEXADECIMAL | SDLK_SCANCODE_MASK,
        SDLK_LCTRL = (int)SDL_Scancode.SDL_SCANCODE_LCTRL | SDLK_SCANCODE_MASK,
        SDLK_LSHIFT = (int)SDL_Scancode.SDL_SCANCODE_LSHIFT | SDLK_SCANCODE_MASK,
        SDLK_LALT = (int)SDL_Scancode.SDL_SCANCODE_LALT | SDLK_SCANCODE_MASK,
        SDLK_LGUI = (int)SDL_Scancode.SDL_SCANCODE_LGUI | SDLK_SCANCODE_MASK,
        SDLK_RCTRL = (int)SDL_Scancode.SDL_SCANCODE_RCTRL | SDLK_SCANCODE_MASK,
        SDLK_RSHIFT = (int)SDL_Scancode.SDL_SCANCODE_RSHIFT | SDLK_SCANCODE_MASK,
        SDLK_RALT = (int)SDL_Scancode.SDL_SCANCODE_RALT | SDLK_SCANCODE_MASK,
        SDLK_RGUI = (int)SDL_Scancode.SDL_SCANCODE_RGUI | SDLK_SCANCODE_MASK,
        SDLK_MODE = (int)SDL_Scancode.SDL_SCANCODE_MODE | SDLK_SCANCODE_MASK,
        SDLK_AUDIONEXT = (int)SDL_Scancode.SDL_SCANCODE_AUDIONEXT | SDLK_SCANCODE_MASK,
        SDLK_AUDIOPREV = (int)SDL_Scancode.SDL_SCANCODE_AUDIOPREV | SDLK_SCANCODE_MASK,
        SDLK_AUDIOSTOP = (int)SDL_Scancode.SDL_SCANCODE_AUDIOSTOP | SDLK_SCANCODE_MASK,
        SDLK_AUDIOPLAY = (int)SDL_Scancode.SDL_SCANCODE_AUDIOPLAY | SDLK_SCANCODE_MASK,
        SDLK_AUDIOMUTE = (int)SDL_Scancode.SDL_SCANCODE_AUDIOMUTE | SDLK_SCANCODE_MASK,
        SDLK_MEDIASELECT = (int)SDL_Scancode.SDL_SCANCODE_MEDIASELECT | SDLK_SCANCODE_MASK,
        SDLK_WWW = (int)SDL_Scancode.SDL_SCANCODE_WWW | SDLK_SCANCODE_MASK,
        SDLK_MAIL = (int)SDL_Scancode.SDL_SCANCODE_MAIL | SDLK_SCANCODE_MASK,
        SDLK_CALCULATOR = (int)SDL_Scancode.SDL_SCANCODE_CALCULATOR | SDLK_SCANCODE_MASK,
        SDLK_COMPUTER = (int)SDL_Scancode.SDL_SCANCODE_COMPUTER | SDLK_SCANCODE_MASK,
        SDLK_AC_SEARCH = (int)SDL_Scancode.SDL_SCANCODE_AC_SEARCH | SDLK_SCANCODE_MASK,
        SDLK_AC_HOME = (int)SDL_Scancode.SDL_SCANCODE_AC_HOME | SDLK_SCANCODE_MASK,
        SDLK_AC_BACK = (int)SDL_Scancode.SDL_SCANCODE_AC_BACK | SDLK_SCANCODE_MASK,
        SDLK_AC_FORWARD = (int)SDL_Scancode.SDL_SCANCODE_AC_FORWARD | SDLK_SCANCODE_MASK,
        SDLK_AC_STOP = (int)SDL_Scancode.SDL_SCANCODE_AC_STOP | SDLK_SCANCODE_MASK,
        SDLK_AC_REFRESH = (int)SDL_Scancode.SDL_SCANCODE_AC_REFRESH | SDLK_SCANCODE_MASK,
        SDLK_AC_BOOKMARKS = (int)SDL_Scancode.SDL_SCANCODE_AC_BOOKMARKS | SDLK_SCANCODE_MASK,
        SDLK_BRIGHTNESSDOWN = (int)SDL_Scancode.SDL_SCANCODE_BRIGHTNESSDOWN | SDLK_SCANCODE_MASK,
        SDLK_BRIGHTNESSUP = (int)SDL_Scancode.SDL_SCANCODE_BRIGHTNESSUP | SDLK_SCANCODE_MASK,
        SDLK_DISPLAYSWITCH = (int)SDL_Scancode.SDL_SCANCODE_DISPLAYSWITCH | SDLK_SCANCODE_MASK,
        SDLK_KBDILLUMTOGGLE = (int)SDL_Scancode.SDL_SCANCODE_KBDILLUMTOGGLE | SDLK_SCANCODE_MASK,
        SDLK_KBDILLUMDOWN = (int)SDL_Scancode.SDL_SCANCODE_KBDILLUMDOWN | SDLK_SCANCODE_MASK,
        SDLK_KBDILLUMUP = (int)SDL_Scancode.SDL_SCANCODE_KBDILLUMUP | SDLK_SCANCODE_MASK,
        SDLK_EJECT = (int)SDL_Scancode.SDL_SCANCODE_EJECT | SDLK_SCANCODE_MASK,
        SDLK_SLEEP = (int)SDL_Scancode.SDL_SCANCODE_SLEEP | SDLK_SCANCODE_MASK,
        SDLK_APP1 = (int)SDL_Scancode.SDL_SCANCODE_APP1 | SDLK_SCANCODE_MASK,
        SDLK_APP2 = (int)SDL_Scancode.SDL_SCANCODE_APP2 | SDLK_SCANCODE_MASK,
        SDLK_AUDIOREWIND = (int)SDL_Scancode.SDL_SCANCODE_AUDIOREWIND | SDLK_SCANCODE_MASK,
        SDLK_AUDIOFASTFORWARD = (int)SDL_Scancode.SDL_SCANCODE_AUDIOFASTFORWARD | SDLK_SCANCODE_MASK
    }

    public enum SDL_FlashOperation
    {
        SDL_FLASH_CANCEl,
        SDL_FLASH_BRIEFLY,
        SDL_FLASH_UNTIL_FOCUSED
    }

    public enum SDL_HintPriority
    {
        SDL_HINT_DEFAULT,
        SDL_HINT_NORMAL,
        SDL_HINT_OVERRIDE
    }
    #endregion

    #region Constants
    public const uint SDL_INIT_TIMER = 0x00000001;
    public const uint SDL_INIT_AUDIO = 0x00000010;
    public const uint SDL_INIT_VIDEO = 0x00000020;
    public const uint SDL_INIT_CDROM = 0x00000100;
    public const uint SDL_INIT_JOYSTICK = 0x00000200;
    public const uint SDL_INIT_NOPARACHUTE = 0x00100000;
    public const uint SDL_INIT_EVENTTHREAD = 0x01000000;
    public const uint SDL_INIT_EVERYTHING = 0x0000FFFF;

    public const int SDL_WINDOWPOS_UNDEFINED_MASK = 0x1FFF0000;
    public const int SDL_WINDOWPOS_CENTERED_MASK = 0x2FFF0000;
    public const int SDL_WINDOWPOS_UNDEFINED = 0x1FFF0000;
    public const int SDL_WINDOWPOS_CENTERED = 0x2FFF0000;

    public const int SDL_TEXTEDITINGEVENT_TEXT_SIZE = 32;
    public const int SDL_TEXTINPUTEVENT_TEXT_SIZE = 32;

    public const string SDL_HINT_FRAMEBUFFER_ACCELERATION = "SDL_FRAMEBUFFER_ACCELERATION";
    public const string SDL_HINT_RENDER_DRIVER = "SDL_RENDER_DRIVER";
    public const string SDL_HINT_RENDER_OPENGL_SHADERS = "SDL_RENDER_OPENGL_SHADERS";
    public const string SDL_HINT_RENDER_DIRECT3D_THREADSAFE = "SDL_RENDER_DIRECT3D_THREADSAFE";
    public const string SDL_HINT_RENDER_VSYNC = "SDL_RENDER_VSYNC";
    public const string SDL_HINT_VIDEO_X11_XVIDMODE = "SDL_VIDEO_X11_XVIDMODE";
    public const string SDL_HINT_VIDEO_X11_XINERAMA = "SDL_VIDEO_X11_XINERAMA";
    public const string SDL_HINT_VIDEO_X11_XRANDR = "SDL_VIDEO_X11_XRANDR";
    public const string SDL_HINT_GRAB_KEYBOARD = "SDL_GRAB_KEYBOARD";
    public const string SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS = "SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS";
    public const string SDL_HINT_IDLE_TIMER_DISABLED = "SDL_IOS_IDLE_TIMER_DISABLED";
    public const string SDL_HINT_ORIENTATIONS = "SDL_IOS_ORIENTATIONS";
    public const string SDL_HINT_XINPUT_ENABLED = "SDL_XINPUT_ENABLED";
    public const string SDL_HINT_GAMECONTROLLERCONFIG = "SDL_GAMECONTROLLERCONFIG";
    public const string SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS = "SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS";
    public const string SDL_HINT_ALLOW_TOPMOST = "SDL_ALLOW_TOPMOST";
    public const string SDL_HINT_TIMER_RESOLUTION = "SDL_TIMER_RESOLUTION";
    public const string SDL_HINT_RENDER_SCALE_QUALITY = "SDL_RENDER_SCALE_QUALITY";
    public const string SDL_HINT_VIDEO_HIGHDPI_DISABLED = "SDL_VIDEO_HIGHDPI_DISABLED";
    public const string SDL_HINT_CTRL_CLICK_EMULATE_RIGHT_CLICK = "SDL_CTRL_CLICK_EMULATE_RIGHT_CLICK";
    public const string SDL_HINT_VIDEO_WIN_D3DCOMPILER = "SDL_VIDEO_WIN_D3DCOMPILER";
    public const string SDL_HINT_MOUSE_RELATIVE_MODE_WARP = "SDL_MOUSE_RELATIVE_MODE_WARP";
    public const string SDL_HINT_VIDEO_WINDOW_SHARE_PIXEL_FORMAT = "SDL_VIDEO_WINDOW_SHARE_PIXEL_FORMAT";
    public const string SDL_HINT_VIDEO_ALLOW_SCREENSAVER = "SDL_VIDEO_ALLOW_SCREENSAVER";
    public const string SDL_HINT_ACCELEROMETER_AS_JOYSTICK = "SDL_ACCELEROMETER_AS_JOYSTICK";
    public const string SDL_HINT_VIDEO_MAC_FULLSCREEN_SPACES = "SDL_VIDEO_MAC_FULLSCREEN_SPACES";
    public const string SDL_HINT_WINRT_PRIVACY_POLICY_URL = "SDL_WINRT_PRIVACY_POLICY_URL";
    public const string SDL_HINT_WINRT_PRIVACY_POLICY_LABEL = "SDL_WINRT_PRIVACY_POLICY_LABEL";
    public const string SDL_HINT_WINRT_HANDLE_BACK_BUTTON = "SDL_WINRT_HANDLE_BACK_BUTTON";
    public const string SDL_HINT_NO_SIGNAL_HANDLERS = "SDL_NO_SIGNAL_HANDLERS";
    public const string SDL_HINT_IME_INTERNAL_EDITING = "SDL_IME_INTERNAL_EDITING";
    public const string SDL_HINT_ANDROID_SEPARATE_MOUSE_AND_TOUCH = "SDL_ANDROID_SEPARATE_MOUSE_AND_TOUCH";
    public const string SDL_HINT_EMSCRIPTEN_KEYBOARD_ELEMENT = "SDL_EMSCRIPTEN_KEYBOARD_ELEMENT";
    public const string SDL_HINT_THREAD_STACK_SIZE = "SDL_THREAD_STACK_SIZE";
    public const string SDL_HINT_WINDOW_FRAME_USABLE_WHILE_CURSOR_HIDDEN = "SDL_WINDOW_FRAME_USABLE_WHILE_CURSOR_HIDDEN";
    public const string SDL_HINT_WINDOWS_ENABLE_MESSAGELOOP = "SDL_WINDOWS_ENABLE_MESSAGELOOP";
    public const string SDL_HINT_WINDOWS_NO_CLOSE_ON_ALT_F4 = "SDL_WINDOWS_NO_CLOSE_ON_ALT_F4";
    public const string SDL_HINT_XINPUT_USE_OLD_JOYSTICK_MAPPING = "SDL_XINPUT_USE_OLD_JOYSTICK_MAPPING";
    public const string SDL_HINT_MAC_BACKGROUND_APP = "SDL_MAC_BACKGROUND_APP";
    public const string SDL_HINT_VIDEO_X11_NET_WM_PING = "SDL_VIDEO_X11_NET_WM_PING";
    public const string SDL_HINT_ANDROID_APK_EXPANSION_MAIN_FILE_VERSION = "SDL_ANDROID_APK_EXPANSION_MAIN_FILE_VERSION";
    public const string SDL_HINT_ANDROID_APK_EXPANSION_PATCH_FILE_VERSION = "SDL_ANDROID_APK_EXPANSION_PATCH_FILE_VERSION";
    public const string SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH = "SDL_MOUSE_FOCUS_CLICKTHROUGH";
    public const string SDL_HINT_BMP_SAVE_LEGACY_FORMAT = "SDL_BMP_SAVE_LEGACY_FORMAT";
    public const string SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING = "SDL_WINDOWS_DISABLE_THREAD_NAMING";
    public const string SDL_HINT_APPLE_TV_REMOTE_ALLOW_ROTATION = "SDL_APPLE_TV_REMOTE_ALLOW_ROTATION";
    public const string SDL_HINT_AUDIO_RESAMPLING_MODE = "SDL_AUDIO_RESAMPLING_MODE";
    public const string SDL_HINT_RENDER_LOGICAL_SIZE_MODE = "SDL_RENDER_LOGICAL_SIZE_MODE";
    public const string SDL_HINT_MOUSE_NORMAL_SPEED_SCALE = "SDL_MOUSE_NORMAL_SPEED_SCALE";
    public const string SDL_HINT_MOUSE_RELATIVE_SPEED_SCALE = "SDL_MOUSE_RELATIVE_SPEED_SCALE";
    public const string SDL_HINT_TOUCH_MOUSE_EVENTS = "SDL_TOUCH_MOUSE_EVENTS";
    public const string SDL_HINT_WINDOWS_INTRESOURCE_ICON = "SDL_WINDOWS_INTRESOURCE_ICON";
    public const string SDL_HINT_WINDOWS_INTRESOURCE_ICON_SMALL = "SDL_WINDOWS_INTRESOURCE_ICON_SMALL";
    public const string SDL_HINT_IOS_HIDE_HOME_INDICATOR = "SDL_IOS_HIDE_HOME_INDICATOR";
    public const string SDL_HINT_TV_REMOTE_AS_JOYSTICK = "SDL_TV_REMOTE_AS_JOYSTICK";
    public const string SDL_HINT_MOUSE_DOUBLE_CLICK_TIME = "SDL_MOUSE_DOUBLE_CLICK_TIME";
    public const string SDL_HINT_MOUSE_DOUBLE_CLICK_RADIUS = "SDL_MOUSE_DOUBLE_CLICK_RADIUS";
    public const string SDL_HINT_JOYSTICK_HIDAPI = "SDL_JOYSTICK_HIDAPI";
    public const string SDL_HINT_JOYSTICK_HIDAPI_PS4 = "SDL_JOYSTICK_HIDAPI_PS4";
    public const string SDL_HINT_JOYSTICK_HIDAPI_PS4_RUMBLE = "SDL_JOYSTICK_HIDAPI_PS4_RUMBLE";
    public const string SDL_HINT_JOYSTICK_HIDAPI_STEAM = "SDL_JOYSTICK_HIDAPI_STEAM";
    public const string SDL_HINT_JOYSTICK_HIDAPI_SWITCH = "SDL_JOYSTICK_HIDAPI_SWITCH";
    public const string SDL_HINT_JOYSTICK_HIDAPI_XBOX = "SDL_JOYSTICK_HIDAPI_XBOX";
    public const string SDL_HINT_ENABLE_STEAM_CONTROLLERS = "SDL_ENABLE_STEAM_CONTROLLERS";
    public const string SDL_HINT_ANDROID_TRAP_BACK_BUTTON = "SDL_ANDROID_TRAP_BACK_BUTTON";
    public const string SDL_HINT_MOUSE_TOUCH_EVENTS = "SDL_MOUSE_TOUCH_EVENTS";
    public const string SDL_HINT_GAMECONTROLLERCONFIG_FILE = "SDL_GAMECONTROLLERCONFIG_FILE";
    public const string SDL_HINT_ANDROID_BLOCK_ON_PAUSE = "SDL_ANDROID_BLOCK_ON_PAUSE";
    public const string SDL_HINT_RENDER_BATCHING = "SDL_RENDER_BATCHING";
    public const string SDL_HINT_EVENT_LOGGING = "SDL_EVENT_LOGGING";
    public const string SDL_HINT_WAVE_RIFF_CHUNK_SIZE = "SDL_WAVE_RIFF_CHUNK_SIZE";
    public const string SDL_HINT_WAVE_TRUNCATION = "SDL_WAVE_TRUNCATION";
    public const string SDL_HINT_WAVE_FACT_CHUNK = "SDL_WAVE_FACT_CHUNK";
    public const string SDL_HINT_VIDO_X11_WINDOW_VISUALID = "SDL_VIDEO_X11_WINDOW_VISUALID";
    public const string SDL_HINT_GAMECONTROLLER_USE_BUTTON_LABELS = "SDL_GAMECONTROLLER_USE_BUTTON_LABELS";
    public const string SDL_HINT_VIDEO_EXTERNAL_CONTEXT = "SDL_VIDEO_EXTERNAL_CONTEXT";
    public const string SDL_HINT_JOYSTICK_HIDAPI_GAMECUBE = "SDL_JOYSTICK_HIDAPI_GAMECUBE";
    public const string SDL_HINT_DISPLAY_USABLE_BOUNDS = "SDL_DISPLAY_USABLE_BOUNDS";
    public const string SDL_HINT_VIDEO_X11_FORCE_EGL = "SDL_VIDEO_X11_FORCE_EGL";
    public const string SDL_HINT_GAMECONTROLLERTYPE = "SDL_GAMECONTROLLERTYPE";

    public const int SDLK_SCANCODE_MASK = (1 << 30);
    public static SDL_Keycode SDL_SCANCODE_TO_KEYCODE(SDL_Scancode X)
    {
        return (SDL_Keycode)((int)X | SDLK_SCANCODE_MASK);
    }
    #endregion
}
