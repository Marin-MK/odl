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
        delegate int IMG_IntUInt(uint UInt);
        delegate IntPtr IMG_PtrPtr(IntPtr IntPtr);
        delegate int IMG_IntPtrPtr(IntPtr IntPtr1, IntPtr IntPtr2);
        #endregion

        public static void Bind(string Library, params string[] PreloadLibraries)
        {
            Main = new SDL_image(Library, PreloadLibraries);
        }

        SDL_image(string Library, params string[] PreloadLibraries) : base(Library, PreloadLibraries)
        {
            FUNC_IMG_Init = GetFunction<IMG_IntUInt>("IMG_Init");
            FUNC_IMG_Quit = GetFunction<Action>("IMG_Quit");
            FUNC_IMG_Load = GetFunction<IMG_PtrPtr>("IMG_Load");
            FUNC_IMG_SavePNG = GetFunction<IMG_IntPtrPtr>("IMG_SavePNG");
        }

        #region Utility
        static unsafe string PtrToStr(IntPtr Pointer)
        {
            if (Pointer == IntPtr.Zero) return null;
            byte* ptr = (byte*) Pointer;
            while (*ptr != 0) ptr++;
            return Encoding.UTF8.GetString(
                (byte*) Pointer,
                (int) (ptr - (byte*) Pointer)
            );
        }

        static unsafe IntPtr StrToPtr(string String)
        {
            byte[] buffer;
            if (String == null)
            {
                return IntPtr.Zero;
            }
            else
            {
                int bufferSize = (String != null ? (String.Length * 4) + 1 : 0);
                buffer = new byte[bufferSize];
                byte[] bytes = Encoding.UTF8.GetBytes(String);
                return Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
            }
        }

        static void IEP(string FunctionName)
        {
            IEP(Main, FunctionName);
        }
        #endregion

        #region SDL_image Functions
        static IMG_IntUInt FUNC_IMG_Init;
        public static int IMG_Init(uint Flags)
        {
            if (FUNC_IMG_Init == null) IEP("IMG_Init");
            return FUNC_IMG_Init(Flags);
        }

        static Action FUNC_IMG_Quit;
        public static void IMG_Quit()
        {
            if (FUNC_IMG_Quit == null) IEP("IMG_Quit");
            FUNC_IMG_Quit();
        }

        static IMG_PtrPtr FUNC_IMG_Load;
        public static IntPtr IMG_Load(string Filename)
        {
            if (FUNC_IMG_Load == null) IEP("IMG_Load");
            return FUNC_IMG_Load(StrToPtr(Filename));
        }

        static IMG_IntPtrPtr FUNC_IMG_SavePNG;
        public static int IMG_SavePNG(IntPtr SDL_Surface, string Filename)
        {
            if (FUNC_IMG_SavePNG == null) IEP("IMG_SavePNG");
            return FUNC_IMG_SavePNG(SDL_Surface, StrToPtr(Filename));
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
