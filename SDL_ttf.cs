using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace odl.SDL2
{
    public class SDL_ttf : NativeLibrary
    {
        static SDL_ttf Main;

        #region Function Delegates
        delegate int TTF_Int();
        delegate IntPtr TTF_PtrPtrInt(IntPtr IntPtr, int Int);
        delegate void TTF_VoidPtr(IntPtr IntPtr);
        delegate void TTF_VoidPtrInt(IntPtr IntPtr, int Int);
        delegate int TTF_IntPtrUIntOutIntOutIntOutIntOutIntOutInt(IntPtr IntPtr, uint UInt, out int Int1, out int Int2, out int Int3, out int Int4, out int Int5);
        delegate int TTF_IntPtr(IntPtr IntPtr);
        delegate int TTF_IntPtrPtrOutIntOutInt(IntPtr IntPtr1, IntPtr IntPtr2, out int Int1, out int Int2);
        delegate IntPtr TTF_PtrPtrPtrColor(IntPtr IntPtr1, IntPtr IntPtr2, SDL.SDL_Color Color);
        delegate IntPtr TTF_PtrPtrUShtColor(IntPtr IntPtr, ushort USht, SDL.SDL_Color Color);
        #endregion

        public static void Bind(string Library, params string[] PreloadLibraries)
        {
            Main = new SDL_ttf(Library, PreloadLibraries);
        }

        SDL_ttf(string Library, params string[] PreloadLibraries) : base(Library, PreloadLibraries)
        {
            FUNC_TTF_Init = GetFunction<TTF_Int>("TTF_Init");
            FUNC_TTF_Quit = GetFunction<Action>("TTF_Quit");
            FUNC_TTF_OpenFont = GetFunction<TTF_PtrPtrInt>("TTF_OpenFont");
            FUNC_TTF_CloseFont = GetFunction<TTF_VoidPtr>("TTF_CloseFont");
            FUNC_TTF_SetFontStyle = GetFunction<TTF_VoidPtrInt>("TTF_SetFontStyle");
            FUNC_TTF_GlyphMetrics = GetFunction<TTF_IntPtrUIntOutIntOutIntOutIntOutIntOutInt>("TTF_GlyphMetrics");
            FUNC_TTF_FontHeight = GetFunction<TTF_IntPtr>("TTF_FontHeight");
            FUNC_TTF_SizeText = GetFunction<TTF_IntPtrPtrOutIntOutInt>("TTF_SizeText");
            FUNC_TTF_RenderText_Solid = GetFunction<TTF_PtrPtrPtrColor>("TTF_RenderText_Solid");
            FUNC_TTF_RenderText_Blended = GetFunction<TTF_PtrPtrPtrColor>("TTF_RenderText_Blended");
            FUNC_TTF_RenderGlyph_Solid = GetFunction<TTF_PtrPtrUShtColor>("TTF_RenderGlyph_Solid");
            FUNC_TTF_RenderGlyph_Blended = GetFunction<TTF_PtrPtrUShtColor>("TTF_RenderGlyph_Blended");
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

        #region SDL_ttf Functions
        static TTF_Int FUNC_TTF_Init;
        public static int TTF_Init()
        {
            if (FUNC_TTF_Init == null) IEP("TTF_Init");
            return FUNC_TTF_Init();
        }

        static Action FUNC_TTF_Quit;
        public static void TTF_Quit()
        {
            if (FUNC_TTF_Quit == null) IEP("TTF_Quit");
            FUNC_TTF_Quit();
        }

        static TTF_PtrPtrInt FUNC_TTF_OpenFont;
        public static IntPtr TTF_OpenFont(string Filename, int Size)
        {
            if (FUNC_TTF_OpenFont == null) IEP("TTF_OpenFont");
            return FUNC_TTF_OpenFont(StrToPtr(Filename), Size);
        }

        static TTF_VoidPtr FUNC_TTF_CloseFont;
        public static void TTF_CloseFont(IntPtr SDL_Font)
        {
            if (FUNC_TTF_CloseFont == null) IEP("TTF_CloseFont");
            FUNC_TTF_CloseFont(SDL_Font);
        }

        static TTF_VoidPtrInt FUNC_TTF_SetFontStyle;
        public static void TTF_SetFontStyle(IntPtr SDL_Font, int Style)
        {
            if (FUNC_TTF_SetFontStyle == null) IEP("TTF_SetFontStyle");
            FUNC_TTF_SetFontStyle(SDL_Font, Style);
        }

        static TTF_IntPtrUIntOutIntOutIntOutIntOutIntOutInt FUNC_TTF_GlyphMetrics;
        public static int TTF_GlyphMetrics(IntPtr SDL_Font, uint Ch, out int MinX, out int MaxX, out int MinY, out int MaxY, out int Advance)
        {
            if (FUNC_TTF_GlyphMetrics == null) IEP("TTF_GlyphMetrics");
            return FUNC_TTF_GlyphMetrics(SDL_Font, Ch, out MinX, out MaxX, out MinY, out MaxY, out Advance);
        }

        static TTF_IntPtr FUNC_TTF_FontHeight;
        public static int TTF_FontHeight(IntPtr SDL_Font)
        {
            if (FUNC_TTF_FontHeight == null) IEP("TTF_FontHeight");
            return FUNC_TTF_FontHeight(SDL_Font);
        }

        static TTF_IntPtrPtrOutIntOutInt FUNC_TTF_SizeText;
        public static int TTF_SizeText(IntPtr SDL_Font, string Text, out int Width, out int Height)
        {
            if (FUNC_TTF_SizeText == null) IEP("TTF_SizeText");
            return FUNC_TTF_SizeText(SDL_Font, StrToPtr(Text), out Width, out Height);
        }

        static TTF_PtrPtrPtrColor FUNC_TTF_RenderText_Solid;
        public static IntPtr TTF_RenderText_Solid(IntPtr SDL_Font, string Text, SDL.SDL_Color SDL_Color)
        {
            if (FUNC_TTF_RenderText_Solid == null) IEP("TTF_RenderText_Solid");
            return FUNC_TTF_RenderText_Solid(SDL_Font, StrToPtr(Text), SDL_Color);
        }

        static TTF_PtrPtrPtrColor FUNC_TTF_RenderText_Blended;
        public static IntPtr TTF_RenderText_Blended(IntPtr SDL_Font, string Text, SDL.SDL_Color SDL_Color)
        {
            if (FUNC_TTF_RenderText_Blended == null) IEP("TTF_RenderText_Blended");
            return FUNC_TTF_RenderText_Blended(SDL_Font, StrToPtr(Text), SDL_Color);
        }

        static TTF_PtrPtrUShtColor FUNC_TTF_RenderGlyph_Solid;
        public static IntPtr TTF_RenderGlyph_Solid(IntPtr SDL_Font, ushort Ch, SDL.SDL_Color SDL_Color)
        {
            if (FUNC_TTF_RenderGlyph_Solid == null) IEP("TTF_RenderGlyph_Solid");
            return FUNC_TTF_RenderGlyph_Solid(SDL_Font, Ch, SDL_Color);
        }

        static TTF_PtrPtrUShtColor FUNC_TTF_RenderGlyph_Blended;
        public static IntPtr TTF_RenderGlyph_Blended(IntPtr SDL_Font, ushort Ch, SDL.SDL_Color SDL_Color)
        {
            if (FUNC_TTF_RenderGlyph_Blended == null) IEP("TTF_RenderGlyph_Blended");
            return FUNC_TTF_RenderGlyph_Blended(SDL_Font, Ch, SDL_Color);
        }
        #endregion

        #region Constants
        public const int SDL_TTF_MAJOR_VERSION = 2;
        public const int SDL_TTF_MINOR_VERSION = 0;
        public const int SDL_TTF_PATCHLEVEL = 16;
        #endregion
    }
}
