using System;
using NativeLibraryLoader;

namespace odl.SDL2;

public class SDL_image : NativeLibrary
{
    private static SDL_image Main;

    public static SDL.SDL_Version Version;

    public new static SDL_image Load(string Library, params string[] PreloadLibraries)
    {
        Main = new SDL_image(Library, PreloadLibraries);
        return Main;
    }

    unsafe protected SDL_image(string Library, params string[] PreloadLibraries) : base(Library, PreloadLibraries)
    {
        IMG_Init = GetFunction<IMG_IntUInt>("IMG_Init");
        IMG_Linked_Version = GetFunction<IMG_Version>("IMG_Linked_Version");
        IMG_Quit = GetFunction<Action>("IMG_Quit");
        FUNC_IMG_Load = GetFunction<IMG_PtrPtr>("IMG_Load");
        FUNC_IMG_SavePNG = GetFunction<IMG_IntPtrPtr>("IMG_SavePNG");
        Version = *IMG_Linked_Version();
    }

    #region Function Delegates
    public delegate int IMG_IntUInt(uint UInt);
    public unsafe delegate SDL.SDL_Version* IMG_Version();
    public delegate IntPtr IMG_PtrPtr(IntPtr IntPtr);
    public delegate int IMG_IntPtrPtr(IntPtr IntPtr1, IntPtr IntPtr2);
    #endregion

    #region SDL_image Functions
    private static IMG_PtrPtr FUNC_IMG_Load;
    private static IMG_IntPtrPtr FUNC_IMG_SavePNG;

    public static IMG_IntUInt IMG_Init;
    public static IMG_Version IMG_Linked_Version;
    public static Action IMG_Quit;
    public static IntPtr IMG_Load(string Filename)
    {
        return FUNC_IMG_Load(SDL.StrUTF8ToPtr(Filename));
    }
    public static int IMG_SavePNG(IntPtr SDL_Surface, string Filename)
    {
        return FUNC_IMG_SavePNG(SDL_Surface, SDL.StrUTF8ToPtr(Filename));
    }
    #endregion

    #region Constants
    public const uint IMG_INIT_JPG = 0x00000001;
    public const uint IMG_INIT_PNG = 0x00000002;
    public const uint IMG_INIT_TIF = 0x00000004;
    public const uint IMG_INIT_WEBP = 0x00000008;
    #endregion
}
