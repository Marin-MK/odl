using System;

namespace odl.SDL2;

public class SDL_ttf : NativeLibrary
{
    static SDL_ttf Main;

    #region Function Delegates
    public delegate int TTF_Int();
    public delegate IntPtr TTF_PtrPtrInt(IntPtr IntPtr, int Int);
    public delegate void TTF_VoidPtr(IntPtr IntPtr);
    public delegate void TTF_VoidPtrInt(IntPtr IntPtr, int Int);
    public delegate int TTF_IntPtrUIntOutIntOutIntOutIntOutIntOutInt(IntPtr IntPtr, uint UInt, out int Int1, out int Int2, out int Int3, out int Int4, out int Int5);
    public delegate int TTF_IntPtr(IntPtr IntPtr);
    public delegate int TTF_IntPtrPtrOutIntOutInt(IntPtr IntPtr1, IntPtr IntPtr2, out int Int1, out int Int2);
    public delegate IntPtr TTF_PtrPtrPtrColor(IntPtr IntPtr1, IntPtr IntPtr2, SDL.SDL_Color Color);
    public delegate IntPtr TTF_PtrPtrUShtColor(IntPtr IntPtr, ushort USht, SDL.SDL_Color Color);
    #endregion

    public static void Bind(string Library, params string[] PreloadLibraries)
    {
        Main = new SDL_ttf(Library, PreloadLibraries);
    }

    SDL_ttf(string Library, params string[] PreloadLibraries) : base(Library, PreloadLibraries)
    {
        TTF_Init = GetFunction<TTF_Int>("TTF_Init");
        TTF_Quit = GetFunction<Action>("TTF_Quit");
        FUNC_TTF_OpenFont = GetFunction<TTF_PtrPtrInt>("TTF_OpenFont");
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
    }

    #region SDL_ttf Functions
    static TTF_PtrPtrInt FUNC_TTF_OpenFont;
    static TTF_IntPtrPtrOutIntOutInt FUNC_TTF_SizeText;
    static TTF_PtrPtrPtrColor FUNC_TTF_RenderText_Solid;
    static TTF_PtrPtrPtrColor FUNC_TTF_RenderText_Blended;
    static TTF_PtrPtrPtrColor FUNC_TTF_RenderUTF8_Solid;
    static TTF_PtrPtrPtrColor FUNC_TTF_RenderUTF8_Blended;

    public static TTF_Int TTF_Init;
    public static Action TTF_Quit;
    public static IntPtr TTF_OpenFont(string Filename, int Size)
    {
        return FUNC_TTF_OpenFont(SDL.StrToPtr(Filename), Size);
    }
    public static TTF_VoidPtr TTF_CloseFont;
    public static TTF_VoidPtrInt TTF_SetFontStyle;
    public static TTF_IntPtrUIntOutIntOutIntOutIntOutIntOutInt TTF_GlyphMetrics;
    public static TTF_IntPtr TTF_FontHeight;
    public static int TTF_SizeText(IntPtr SDL_Font, string Text, out int Width, out int Height)
    {
        return FUNC_TTF_SizeText(SDL_Font, SDL.StrToPtr(Text), out Width, out Height);
    }
    public static IntPtr TTF_RenderText_Solid(IntPtr SDL_Font, string Text, SDL.SDL_Color SDL_Color)
    {
        return FUNC_TTF_RenderText_Solid(SDL_Font, SDL.StrToPtr(Text), SDL_Color);
    }
    public static IntPtr TTF_RenderText_Blended(IntPtr SDL_Font, string Text, SDL.SDL_Color SDL_Color)
    {
        return FUNC_TTF_RenderText_Blended(SDL_Font, SDL.StrToPtr(Text), SDL_Color);
    }
    public static IntPtr TTF_RenderUTF8_Solid(IntPtr SDL_Font, string Text, SDL.SDL_Color SDL_Color)
    {
        return FUNC_TTF_RenderUTF8_Solid(SDL_Font, SDL.StrUTF8ToPtr(Text), SDL_Color);
    }
    public static IntPtr TTF_RenderUTF8_Blended(IntPtr SDL_Font, string Text, SDL.SDL_Color SDL_Color)
    {
        return FUNC_TTF_RenderUTF8_Blended(SDL_Font, SDL.StrUTF8ToPtr(Text), SDL_Color);
    }
    public static TTF_PtrPtrUShtColor TTF_RenderGlyph_Solid;
    public static TTF_PtrPtrUShtColor TTF_RenderGlyph_Blended;
    #endregion

    #region Constants
    public const int SDL_TTF_MAJOR_VERSION = 2;
    public const int SDL_TTF_MINOR_VERSION = 0;
    public const int SDL_TTF_PATCHLEVEL = 15;
    #endregion
}
