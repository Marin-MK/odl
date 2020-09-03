using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace odl.SDL2
{
    public class SDL_image : NativeLibrary
    {
        static SDL_image Main;

        #region Function Delegates
        public delegate int IMG_IntUInt(uint UInt);
        public delegate IntPtr IMG_PtrPtr(IntPtr IntPtr);
        public delegate int IMG_IntPtrPtr(IntPtr IntPtr1, IntPtr IntPtr2);
        #endregion

        public static void Bind(string Library, params string[] PreloadLibraries)
        {
            Main = new SDL_image(Library, PreloadLibraries);
        }

        SDL_image(string Library, params string[] PreloadLibraries) : base(Library, PreloadLibraries)
        {
            IMG_Init = GetFunction<IMG_IntUInt>("IMG_Init");
            IMG_Quit = GetFunction<Action>("IMG_Quit");
            FUNC_IMG_Load = GetFunction<IMG_PtrPtr>("IMG_Load");
            FUNC_IMG_SavePNG = GetFunction<IMG_IntPtrPtr>("IMG_SavePNG");
        }

        #region SDL_image Functions
        static IMG_PtrPtr FUNC_IMG_Load;
        static IMG_IntPtrPtr FUNC_IMG_SavePNG;

        public static IMG_IntUInt IMG_Init;
        public static Action IMG_Quit;
        public static IntPtr IMG_Load(string Filename)
        {
            return FUNC_IMG_Load(SDL.StrToPtr(Filename));
        }
        public static int IMG_SavePNG(IntPtr SDL_Surface, string Filename)
        {
            return FUNC_IMG_SavePNG(SDL_Surface, SDL.StrToPtr(Filename));
        }
        #endregion

        #region Constants
        public const int SDL_IMAGE_MAJOR_VERSION = 2;
        public const int SDL_IMAGE_MINOR_VERSION = 0;
        public const int SDL_IMAGE_PATCHLEVEL = 6;

        public const uint IMG_INIT_JPG = 0x00000001;
        public const uint IMG_INIT_PNG = 0x00000002;
        public const uint IMG_INIT_TIF = 0x00000004;
        public const uint IMG_INIT_WEBP = 0x00000008;
        #endregion
    }
}
