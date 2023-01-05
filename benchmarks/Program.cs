using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicBenchmarks;
using NativeLibraryLoader;

namespace odl.benchmarks;

internal class Program
{
    public static void Main(params string[] Args)
    {
        PathPlatformInfo windows = new PathPlatformInfo(NativeLibraryLoader.Platform.Windows);
        windows.AddPath("libsdl2", "lib/windows/SDL2.dll");
        windows.AddPath("libsdl2_image", "lib/windows/SDL2_image.dll");
        windows.AddPath("libsdl2_ttf", "lib/windows/SDL2_ttf.dll");
        windows.AddPath("libpng", "lib/windows/libpng16-16.dll");
        windows.AddPath("libz", "lib/windows/zlib1.dll");
        windows.AddPath("libfreetype", "lib/windows/libfreetype-6.dll");
        Graphics.Start(PathInfo.Create(windows));

        Window win = new Window();
        win.Initialize();

        BenchmarkRunner.Run<FlipVerticallyBenchmarks>();

        win.Dispose();

        Graphics.Stop();

        Console.ReadKey();
    }
}