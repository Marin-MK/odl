using System;

namespace odl;

internal class OpenGL
{
    internal static bool Loaded = false;

    internal static void Load()
    {
        glCreateProgram = SDL2.SDL.GetGLFunction<GL_UInt>("glCreateProgram");
        glBegin = SDL2.SDL.GetGLFunction<GL_VoidInt>("glBegin");
        glEnd = SDL2.SDL.GetGLFunction<Action>("glEnd");
        glTexCoord2f = SDL2.SDL.GetGLFunction<GL_VoidFloatFloat>("glTexCoord2f");
        glVertex2f = SDL2.SDL.GetGLFunction<GL_VoidFloatFloat>("glVertex2f");
        glViewport = SDL2.SDL.GetGLFunction<GL_VoidIntIntIntInt>("glViewport");
        glGetIntegerv = SDL2.SDL.GetGLFunction<GL_VoidUIntPtr>("glGetIntegerv");
        Loaded = true;
    }

    internal delegate uint GL_UInt();
    internal delegate uint GL_VoidInt(int Int);
    internal delegate void GL_VoidFloatFloat(float Float1, float Float2);
    internal delegate void GL_VoidIntIntIntInt(int Int1, int Int2, int Int3, int Int4);
    internal unsafe delegate void GL_VoidUIntPtr(uint UInt, int* Ptr);

    internal static GL_UInt glCreateProgram;
    internal static GL_VoidInt glBegin;
    internal static Action glEnd;
    internal static GL_VoidFloatFloat glTexCoord2f;
    internal static GL_VoidFloatFloat glVertex2f;
    internal static GL_VoidIntIntIntInt glViewport;
    internal static GL_VoidUIntPtr glGetIntegerv;

    internal static int GL_POINTS         = 0x0000;
    internal static int GL_LINES          = 0x0001;
    internal static int GL_LINE_LOOP      = 0x0002;
    internal static int GL_LINE_STRIP     = 0x0003;
    internal static int GL_TRIANGLES      = 0x0004;
    internal static int GL_TRIANGLE_STRIP = 0x0005;
    internal static int GL_TRIANGLE_FAN   = 0x0006;
    internal static int GL_QUADS          = 0x0007;
}
