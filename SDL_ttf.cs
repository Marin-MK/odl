using System;
using NativeLibraryLoader;

namespace odl.SDL2;

internal class SDL_ttf : NativeLibrary
{
    private static SDL_ttf Main;

    internal static SDL.SDL_Version Version;

    public new static SDL_ttf Load(string Library, params string[] PreloadLibraries)
    {
        Main = new SDL_ttf(Library, PreloadLibraries);
        return Main;
    }

    unsafe protected SDL_ttf(string Library, params string[] PreloadLibraries) : base(Library, PreloadLibraries)
    {
        TTF_Init = GetFunction<TTF_Int>("TTF_Init");
        TTF_Linked_Version = GetFunction<TTF_Version>("TTF_Linked_Version");
        TTF_Quit = GetFunction<Action>("TTF_Quit");
        FUNC_TTF_OpenFont = GetFunction<TTF_PtrPtrInt>("TTF_OpenFont");
        FUNC_TTF_OpenFontDPI = GetFunction<TTF_PtrPtrIntUIntUInt>("TTF_OpenFontDPI");
        TTF_CloseFont = GetFunction<TTF_VoidPtr>("TTF_CloseFont");
        TTF_SetFontStyle = GetFunction<TTF_VoidPtrInt>("TTF_SetFontStyle");
        TTF_GlyphMetrics = GetFunction<TTF_IntPtrUIntOutIntOutIntOutIntOutIntOutInt>("TTF_GlyphMetrics");
        TTF_FontHeight = GetFunction<TTF_IntPtr>("TTF_FontHeight");
        FUNC_TTF_SizeText = GetFunction<TTF_IntPtrPtrOutIntOutInt>("TTF_SizeText");
        FUNC_TTF_RenderText_Solid = GetFunction<TTF_PtrPtrPtrColor>("TTF_RenderText_Solid");
        FUNC_TTF_RenderText_Blended = GetFunction<TTF_PtrPtrPtrColor>("TTF_RenderText_Blended");
        TTF_RenderGlyph_Solid = GetFunction<TTF_PtrPtrUShtColor>("TTF_RenderGlyph_Solid");
        TTF_RenderGlyph_Blended = GetFunction<TTF_PtrPtrUShtColor>("TTF_RenderGlyph_Blended");
        FUNC_TTF_RenderUTF8_Solid = GetFunction<TTF_PtrPtrPtrColor>("TTF_RenderUTF8_Solid");
        FUNC_TTF_RenderUTF8_Blended = GetFunction<TTF_PtrPtrPtrColor>("TTF_RenderUTF8_Blended");
        TTF_SetFontSDF = GetFunction<TTF_IntPtrBool>("TTF_SetFontSDF");
        TTF_SetFontHinting = GetFunction<TTF_VoidPtrInt>("TTF_SetFontHinting");
        TTF_GetFreeTypeVersion = GetFunction<TTF_VoidIntIntInt>("TTF_GetFreeTypeVersion");
        Version = *TTF_Linked_Version();
    }

    #region Function Delegates
    internal delegate int TTF_Int();
    internal delegate IntPtr TTF_Ptr();
    internal unsafe delegate SDL.SDL_Version* TTF_Version();
    internal delegate IntPtr TTF_PtrPtrInt(IntPtr IntPtr, int Int);
    internal delegate IntPtr TTF_PtrPtrIntUIntUInt(IntPtr IntPtr, int Int, uint UInt1, uint UInt2);
    internal delegate void TTF_VoidPtr(IntPtr IntPtr);
    internal delegate void TTF_VoidPtrInt(IntPtr IntPtr, int Int);
    internal delegate int TTF_IntPtrUIntOutIntOutIntOutIntOutIntOutInt(IntPtr IntPtr, uint UInt, out int Int1, out int Int2, out int Int3, out int Int4, out int Int5);
    internal delegate int TTF_IntPtr(IntPtr IntPtr);
    internal delegate int TTF_IntPtrPtrOutIntOutInt(IntPtr IntPtr1, IntPtr IntPtr2, out int Int1, out int Int2);
    internal delegate IntPtr TTF_PtrPtrPtrColor(IntPtr IntPtr1, IntPtr IntPtr2, SDL.SDL_Color Color);
    internal delegate IntPtr TTF_PtrPtrUShtColor(IntPtr IntPtr, ushort USht, SDL.SDL_Color Color);
    internal delegate int TTF_IntPtrBool(IntPtr Ptr, SDL.SDL_bool Bool);
    internal delegate void TTF_VoidIntIntInt(out int Int1, out int Int2, out int Int3);
    #endregion

    #region SDL_ttf Functions
    private static TTF_PtrPtrInt FUNC_TTF_OpenFont;
    private static TTF_PtrPtrIntUIntUInt FUNC_TTF_OpenFontDPI;
    private static TTF_IntPtrPtrOutIntOutInt FUNC_TTF_SizeText;
    private static TTF_PtrPtrPtrColor FUNC_TTF_RenderText_Solid;
    private static TTF_PtrPtrPtrColor FUNC_TTF_RenderText_Blended;
    private static TTF_PtrPtrPtrColor FUNC_TTF_RenderUTF8_Solid;
    private static TTF_PtrPtrPtrColor FUNC_TTF_RenderUTF8_Blended;

    internal static TTF_Int TTF_Init;
    internal static TTF_Version TTF_Linked_Version;
    internal static Action TTF_Quit;
    internal static IntPtr TTF_OpenFont(string Filename, int Size)
    {
        return FUNC_TTF_OpenFont(SDL.StrToPtr(Filename), Size);
    }
    internal static IntPtr TTF_OpenFontDPI(string Filename, int Size, uint HDPI, uint VDPI)
    {
        return FUNC_TTF_OpenFontDPI(SDL.StrToPtr(Filename), Size, HDPI, VDPI);
    }
    internal static TTF_VoidPtr TTF_CloseFont;
    internal static TTF_VoidPtrInt TTF_SetFontStyle;
    internal static TTF_IntPtrUIntOutIntOutIntOutIntOutIntOutInt TTF_GlyphMetrics;
    internal static TTF_IntPtr TTF_FontHeight;
    internal static int TTF_SizeText(IntPtr SDL_Font, string Text, out int Width, out int Height)
    {
        return FUNC_TTF_SizeText(SDL_Font, SDL.StrToPtr(Text), out Width, out Height);
    }
    internal static IntPtr TTF_RenderText_Solid(IntPtr SDL_Font, string Text, SDL.SDL_Color SDL_Color)
    {
        return FUNC_TTF_RenderText_Solid(SDL_Font, SDL.StrToPtr(Text), SDL_Color);
    }
    internal static IntPtr TTF_RenderText_Blended(IntPtr SDL_Font, string Text, SDL.SDL_Color SDL_Color)
    {
        return FUNC_TTF_RenderText_Blended(SDL_Font, SDL.StrToPtr(Text), SDL_Color);
    }
    internal static IntPtr TTF_RenderUTF8_Solid(IntPtr SDL_Font, string Text, SDL.SDL_Color SDL_Color)
    {
        return FUNC_TTF_RenderUTF8_Solid(SDL_Font, SDL.StrUTF8ToPtr(Text), SDL_Color);
    }
    internal static IntPtr TTF_RenderUTF8_Blended(IntPtr SDL_Font, string Text, SDL.SDL_Color SDL_Color)
    {
        return FUNC_TTF_RenderUTF8_Blended(SDL_Font, SDL.StrUTF8ToPtr(Text), SDL_Color);
    }
    internal static TTF_PtrPtrUShtColor TTF_RenderGlyph_Solid;
    internal static TTF_PtrPtrUShtColor TTF_RenderGlyph_Blended;
    internal static TTF_IntPtrBool TTF_SetFontSDF;
    internal static TTF_VoidPtrInt TTF_SetFontHinting;
    internal static TTF_VoidIntIntInt TTF_GetFreeTypeVersion;
    #endregion

    #region Constants
    internal static int TTF_HINTING_NORMAL         = 0;
    internal static int TTF_HINTING_LIGHT          = 1;
    internal static int TTF_HINTING_MONO           = 2;
    internal static int TTF_HINTING_NONE           = 3;
    internal static int TTF_HINTING_LIGHT_SUBPIXEL = 4;
    #endregion
}
