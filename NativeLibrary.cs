using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace odl;

public class NativeLibrary
{
    // Windows
    [DllImport("kernel32")]
    static extern IntPtr LoadLibrary(string Filename);

    [DllImport("kernel32")]
    static extern IntPtr GetProcAddress(IntPtr Handle, string FunctionName);

    // Linux
    [DllImport("libdl.so")]
    public static extern IntPtr dlopen(string filename, int flags);

    [DllImport("libdl.so")]
    public static extern IntPtr dlsym(IntPtr Handle, string FunctionName);

    public static List<NativeLibrary> LoadedLibraries = new List<NativeLibrary>();

    public string Name;
    public IntPtr Handle;

    public NativeLibrary(string Library, params string[] PreloadLibraries)
    {
        foreach (string PreloadLibrary in PreloadLibraries)
        {
            if (LoadedLibraries.Find(l => l.Name == PreloadLibrary) != null) continue;
            LoadedLibraries.Add(new NativeLibrary(PreloadLibrary));
        }
        Name = Library;
        if (Graphics.Platform == Platform.Windows) Handle = LoadLibrary(Library);
        else if (Graphics.Platform == Platform.Linux) Handle = dlopen(Library, 0x102);
        else if (Graphics.Platform == Platform.MacOS) throw new Exception("MacOS is not currently supported.");
        else throw new Exception("Platform could not be determined.");
        if (Handle == IntPtr.Zero) throw new Exception($"Failed to load library '{Library}'.");
    }

    public TDelegate GetFunction<TDelegate>(string FunctionName)
    {
        IntPtr funcaddr = IntPtr.Zero;
        if (Graphics.Platform == Platform.Windows) funcaddr = GetProcAddress(Handle, FunctionName);
        else if (Graphics.Platform == Platform.Linux) funcaddr = dlsym(Handle, FunctionName);
        else if (Graphics.Platform == Platform.MacOS) throw new Exception("MacOS is not currently supported.");
        else throw new Exception("Platform could not be determined.");
        if (funcaddr == IntPtr.Zero) throw new InvalidEntryPoint(Name, FunctionName);
        return Marshal.GetDelegateForFunctionPointer<TDelegate>(funcaddr);
    }

    public static void IEP(NativeLibrary Library, string FunctionName)
    {
        throw new InvalidEntryPoint(Library.Name, FunctionName);
    }

    public class InvalidEntryPoint : Exception
    {
        public InvalidEntryPoint(string Library, string FunctionName) : base($"No entry point by the name of '{FunctionName}' could be found in '{Library}'.")
        {

        }
    }
}
