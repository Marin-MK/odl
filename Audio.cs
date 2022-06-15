using System;
using System.Collections.Generic;
using NativeLibraryLoader;

namespace odl;

public delegate void SoundCallback(long Position);
public delegate void SlideCallback(int SlideType);

public static class Audio
{
    public static Sound BGM;
    public static bool Initialized;

    internal static bool UsingBassFX = false;
    internal static bool UsingBassMidi = false;

    internal static List<int> Soundfonts = new List<int>();

    public static void Start(PathInfo PathInfo)
    {
        PathPlatformInfo Path = PathInfo.GetPlatform(NativeLibrary.Platform);

        Console.WriteLine("Loading audio components...");

        NativeLibrary bass = NativeLibrary.Load(Path.Get("bass"));

        NativeLibrary bass_fx = null;
        if (Path.Has("bass_fx"))
        {
            bass_fx = NativeLibrary.Load(Path.Get("bass_fx"));
            BASS_FX_GetVersion = bass_fx.GetFunction<BASS_UInt>("BASS_FX_GetVersion");
            UsingBassFX = true;
        }

        NativeLibrary bass_midi = null;
        if (Path.Has("bass_midi"))
        {
            bass_midi = NativeLibrary.Load(Path.Get("bass_midi"));
            UsingBassMidi = true;
        }

        if (Graphics.Platform != Platform.Windows && Graphics.Platform != Platform.Linux)
        {
            throw new NativeLibrary.UnsupportedPlatformException();
        }

        BASS_Init = bass.GetFunction<BASS_BoolIntIntUIntUIntPtr>("BASS_Init");
        BASS_GetVersion = bass.GetFunction<BASS_UInt>("BASS_GetVersion");
        BASS_Free = bass.GetFunction<BASS_Bool>("BASS_Free");
        BASS_PluginLoad = bass.GetFunction<BASS_IntStr>("BASS_PluginLoad");
        BASS_PluginGetInfo = bass.GetFunction<BASS_PtrInt>("BASS_PluginGetInfo");
        BASS_PluginFree = bass.GetFunction<BASS_BoolInt>("BASS_PluginFree");
        BASS_ChannelSetAttribute = bass.GetFunction<BASS_BoolIntAttributeFlt>("BASS_ChannelSetAttribute");
        BASS_ChannelGetPosition = bass.GetFunction<BASS_LngInt>("BASS_ChannelGetPosition");
        BASS_ChannelSetPosition = bass.GetFunction<BASS_BoolIntLong>("BASS_ChannelSetPosition");
        BASS_ChannelIsActive = bass.GetFunction<BASS_IntInt>("BASS_ChannelIsActive");
        BASS_StreamCreateFile = bass.GetFunction<BASS_IntBoolStrLngLngFlag>("BASS_StreamCreateFile");
        BASS_ChannelGetAttribute = bass.GetFunction<BASS_BoolIntAttributeRefFlt>("BASS_ChannelGetAttribute");
        BASS_ChannelGetLength = bass.GetFunction<BASS_LngInt>("BASS_ChannelGetLength");
        BASS_ChannelSetSync = bass.GetFunction<BASS_IntIntSyncLngProcPtr>("BASS_ChannelSetSync");
        BASS_ChannelRemoveSync = bass.GetFunction<BASS_BoolIntInt>("BASS_ChannelRemoveSync");
        BASS_ChannelFlags = bass.GetFunction<BASS_FlagIntFlagFlag>("BASS_ChannelFlags");
        BASS_ChannelSlideAttribute = bass.GetFunction<BASS_BoolIntAttributeFltInt>("BASS_ChannelSlideAttribute");
        BASS_ChannelPause = bass.GetFunction<BASS_BoolInt>("BASS_ChannelPause");
        BASS_ChannelPlay = bass.GetFunction<BASS_BoolIntBool>("BASS_ChannelPlay");
        BASS_ChannelStop = bass.GetFunction<BASS_BoolInt>("BASS_ChannelStop");
        BASS_ErrorGetCode = bass.GetFunction<BASS_BASSError>("BASS_ErrorGetCode");

        if (UsingBassFX)
        {
            BASS_FX_TempoCreate = bass_fx.GetFunction<BASS_IntIntFlag>("BASS_FX_TempoCreate");
        }

        if (UsingBassMidi)
        {
            BASS_MIDI_StreamCreateFile = bass_midi.GetFunction<BASS_IntBoolStrLngLngFlag>("BASS_MIDI_StreamCreateFile");
            BASS_MIDI_FontInit = bass_midi.GetFunction<BASS_IntStrInt>("BASS_MIDI_FontInit");
            BASS_MIDI_FontFree = bass_midi.GetFunction<BASS_BoolInt>("BASS_MIDI_FontFree");
            BASS_MIDI_StreamSetFonts = bass_midi.GetFunction<BASS_BoolIntPtrInt>("BASS_MIDI_StreamSetFonts");
        }

        BASS_Init(-1, 44100, 0, IntPtr.Zero);
        uint bassversion = BASS_GetVersion();
        Console.WriteLine($"Loaded BASS ({StringifyVersion(bassversion)})");
        if (UsingBassFX)
        {
            BASS_PluginLoad(Path.Get("bass_fx"));
            uint fxversion = BASS_FX_GetVersion();
            Console.WriteLine($"Loaded BASS_FX ({StringifyVersion(fxversion)})");
        }
        if (UsingBassMidi)
        {
            int midihandle = BASS_PluginLoad(Path.Get("bass_midi"));
            BASS_PluginInfo info = System.Runtime.InteropServices.Marshal.PtrToStructure<BASS_PluginInfo>(BASS_PluginGetInfo(midihandle));
            Console.WriteLine($"Loaded BASS_MIDI ({StringifyVersion(info.version)})");
        }
        Initialized = true;
    }

    internal static string StringifyVersion(uint version)
    {
        return Convert.ToString((version & 0xFF000000) >> 24) + "." + Convert.ToString((version & 0x00FF0000) >> 16) +
            "." + Convert.ToString((version & 0x0000FF00) >> 8) + "." + Convert.ToString(version & 0x000000FF);
    }

    public static void Stop()
    {
        if (!Initialized) throw new Exception("Audio module was not initialized.");
        for (int i = 0; i < Soundfonts.Count; i++)
        {
            FreeSoundfont(Soundfonts[i]);
            i--;
        }
        BASS_Free();
    }

    public static int LoadSoundfont(string Filename)
    {
        if (!Initialized) throw new Exception("Audio module was not initialized.");
        if (!UsingBassMidi) throw new Exception("Midi module was not initialized.");
        int Handle = BASS_MIDI_FontInit(Filename, 0);
        Soundfonts.Add(Handle);
        return Handle;
    }

    public static void FreeSoundfont(int Handle)
    {
        if (!Initialized) throw new Exception("Audio module was not initialized.");
        BASS_MIDI_FontFree(Handle);
        Soundfonts.Remove(Handle);
    }

    public static void BGMPlay(string Filename, int Volume = 100, int Pitch = 0)
    {
        BGMPlay(new Sound(Filename, Volume, Pitch));
    }
    public static void BGMPlay(Sound Sound)
    {
        if (!Initialized) throw new Exception("Audio module was not initialized.");
        if (BGM != null && BGM.Alive)
        {
            BGM.FadeOut(BGM.SampleRate / 4, delegate (int SlideType)
            {
                if (SlideType == 2)
                {
                    BGM.Stop();
                    BGM = Sound;
                    BGM.Looping = true;
                    BGM.AddEndCallback(delegate (long Position) { BGM = null; });
                    BGM.FadeIn(BGM.SampleRate / 4);
                    BGM.Play();
                }
            });
        }
        else
        {
            BGM = Sound;
            BGM.Looping = true;
            BGM.AddEndCallback(delegate (long Position) { BGM = null; });
            BGM.Play();
        }
    }

    public static void MEPlay(string Filename, int Volume = 100, int Pitch = 0)
    {
        MEPlay(new Sound(Filename, Volume, Pitch));

    }
    public static void MEPlay(Sound Sound)
    {
        if (!Initialized) throw new Exception("Audio module was not initialized.");
        if (BGM != null && BGM.Alive)
        {
            BGM.FadeOut(BGM.SampleRate / 4, delegate (int SlideType)
            {
                if (SlideType == 2)
                {
                    BGM.Pause();
                    Sound.Play();
                    Sound.AddEndCallback(delegate (long _)
                    {
                        BGM.FadeIn(BGM.SampleRate / 4);
                        BGM.Play();
                    });
                }
            });
        }
        else
        {
            Sound.Play();
        }
    }

    public static void SEPlay(string Filename, int Volume = 100, int Pitch = 0)
    {
        SEPlay(new Sound(Filename, Volume, Pitch));
    }
    public static void SEPlay(Sound Sound)
    {
        if (!Initialized) throw new Exception("Audio module was not initialized.");
        Sound.Play();
    }

    public static void Play(string Filename, int Volume = 100, int Pitch = 0)
    {
        Play(new Sound(Filename, Volume, Pitch));
    }
    public static void Play(Sound Sound)
    {
        if (!Initialized) throw new Exception("Audio module was not initialized.");
        Sound.Play();
    }

    internal delegate void BASS_Syncproc(int Int1, int Int2, int Int3, IntPtr IntPtr);
    internal delegate int BASS_IntStr(string IntPtr);
    internal delegate IntPtr BASS_PtrInt(int Int);
    internal delegate bool BASS_BoolIntIntUIntUIntPtr(int Int, uint UInt1, uint UInt2, IntPtr Ptr);
    internal delegate bool BASS_Bool();
    internal delegate bool BASS_BoolInt(int Int);
    internal delegate uint BASS_UInt();
    internal delegate bool BASS_BoolIntAttributeFlt(int Int, BASS_Attribute Attribute, float Flt);
    internal delegate long BASS_LngInt(int Int);
    internal delegate bool BASS_BoolIntLong(int Int, long Lng);
    internal delegate int BASS_IntInt(int Int);
    internal delegate int BASS_IntBoolStrLngLngFlag(bool Bool, string Str, long Lng1, long Lng2, BASS_Flag Flag);
    internal delegate int BASS_IntIntFlag(int Int, BASS_Flag Flag);
    internal delegate bool BASS_BoolIntAttributeRefFlt(int Int, BASS_Attribute Attribute, ref float Flt);
    internal delegate int BASS_IntIntSyncLngProcPtr(int Int, BASS_Sync Sync, long Lng, BASS_Syncproc Proc, IntPtr IntPtr);
    internal delegate bool BASS_BoolIntInt(int Int1, int Int2);
    internal delegate BASS_Flag BASS_FlagIntFlagFlag(int Int, BASS_Flag Flag1, BASS_Flag Flag2);
    internal delegate bool BASS_BoolIntAttributeFltInt(int Int1, BASS_Attribute Attribute, float Flt, int Int2);
    internal delegate bool BASS_BoolIntBool(int Int, bool Bool);
    internal delegate int BASS_IntStrInt(string Str, int Int);
    internal delegate BASS_Error BASS_BASSError();
    internal unsafe delegate bool BASS_BoolIntPtrInt(int Int1, BASS_MIDI_FONT* Ptr, int Int2);

    internal static BASS_UInt BASS_FX_GetVersion;
    internal static BASS_UInt BASS_GetVersion;
    internal static BASS_IntStr BASS_PluginLoad;
    internal static BASS_PtrInt BASS_PluginGetInfo;
    internal static BASS_BoolIntIntUIntUIntPtr BASS_Init;
    internal static BASS_Bool BASS_Free;
    internal static BASS_BoolInt BASS_PluginFree;
    internal static BASS_BoolIntAttributeFlt BASS_ChannelSetAttribute;
    internal static BASS_LngInt BASS_ChannelGetPosition;
    internal static BASS_BoolIntLong BASS_ChannelSetPosition;
    internal static BASS_IntInt BASS_ChannelIsActive;
    internal static BASS_IntBoolStrLngLngFlag BASS_StreamCreateFile;
    internal static BASS_IntIntFlag BASS_FX_TempoCreate;
    internal static BASS_BoolIntAttributeRefFlt BASS_ChannelGetAttribute;
    internal static BASS_LngInt BASS_ChannelGetLength;
    internal static BASS_IntIntSyncLngProcPtr BASS_ChannelSetSync;
    internal static BASS_BoolIntInt BASS_ChannelRemoveSync;
    internal static BASS_FlagIntFlagFlag BASS_ChannelFlags;
    internal static BASS_BoolIntAttributeFltInt BASS_ChannelSlideAttribute;
    internal static BASS_BoolInt BASS_ChannelPause;
    internal static BASS_BoolIntBool BASS_ChannelPlay;
    internal static BASS_BoolInt BASS_ChannelStop;
    internal static BASS_BASSError BASS_ErrorGetCode;

    internal static BASS_IntBoolStrLngLngFlag BASS_MIDI_StreamCreateFile;
    internal static BASS_IntStrInt BASS_MIDI_FontInit;
    internal static BASS_BoolInt BASS_MIDI_FontFree;
    internal static BASS_BoolIntPtrInt BASS_MIDI_StreamSetFonts;

    internal enum BASS_Attribute : uint
    {
        BASS_ATTRIB_FREQ = 1,
        BASS_ATTRIB_VOL = 2,
        BASS_ATTRIB_PAN = 3,
        BASS_ATTRIB_TEMPO_PITCH = 65537
    }

    internal enum BASS_Flag : uint
    {
        BASS_SAMPLE_LOOP = 4,
        BASS_STREAM_DECODE = 2097152,
        BASS_STREAM_AUTOFREE = 262144,
        BASS_FX_FREESOURCE = 65536
    }

    internal enum BASS_Sync : uint
    {
        BASS_SYNC_POS = 0,
        BASS_SYNC_END = 2,
        BASS_SYNC_MIXTIME = 1073741824,
        BASS_SYNC_SLIDE = 1073741825
    }

    internal enum BASS_Error
    {
        BASS_OK = 0,
        BASS_ERROR_MEM = 1,
        BASS_ERROR_FILEOPEN = 2,
        BASS_ERROR_DRIVER = 3,
        BASS_ERROR_BUFLOST = 4,
        BASS_ERROR_HANDLE = 5,
        BASS_ERROR_FORMAT = 6,
        BASS_ERROR_POSITION = 7,
        BASS_ERROR_INIT = 8,
        BASS_ERROR_START = 9,
        BASS_ERROR_SSL = 10,
        BASS_ERROR_ALREADY = 14,
        BASS_ERROR_NOTAUDIO = 17,
        BASS_ERROR_NOCHAN = 18,
        BASS_ERROR_ILLTYPE = 19,
        BASS_ERROR_ILLPARAM = 20,
        BASS_ERROR_NO3D = 21,
        BASS_ERROR_NOEAX = 22,
        BASS_ERROR_DEVICE = 23,
        BASS_ERROR_NOPLAY = 24,
        BASS_ERROR_FREQ = 25,
        BASS_ERROR_NOTFILE = 27,
        BASS_ERROR_NOHW = 29,
        BASS_ERROR_EMPTY = 31,
        BASS_ERROR_NONET = 32,
        BASS_ERROR_CREATE = 33,
        BASS_ERROR_NOFX = 34,
        BASS_ERROR_NOTAVAIL = 37,
        BASS_ERROR_DECODE = 38,
        BASS_ERROR_DX = 39,
        BASS_ERROR_TIMEOUT = 40,
        BASS_ERROR_FILEFORM = 41,
        BASS_ERROR_SPEAKER = 42,
        BASS_ERROR_VERSION = 43,
        BASS_ERROR_CODEC = 44,
        BASS_ERROR_ENDED = 45,
        BASS_ERROR_BUSY = 46,
        BASS_ERROR_UNSTREAMABLE = 47,
        BASS_ERROR_UNKNOWN = -1
    }

    internal struct BASS_MIDI_FONT
    {
        public int font;
        public int preset;
        public int bank;
    }

    internal struct BASS_PluginInfo
    {
        public uint version;
        public uint formatc;
        public IntPtr formats;
    }
}
