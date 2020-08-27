#define WIN

using System;
using static Un4seen.Bass.Bass;
using System.Runtime.InteropServices;

namespace odl
{
    public delegate void SoundCallback(long Position);
    public delegate void SlideCallback(int SlideType);

    public static class Audio
    {
        public static Sound BGM;

        public static bool UsingBassFX = true;
        static int FXPlugin;

        public static void Start(bool UseBassFX = true)
        {
            Audio.UsingBassFX = UseBassFX;
            IntPtr bass = IntPtr.Zero;
            IntPtr bass_fx = IntPtr.Zero;
            if (Graphics.Platform == Platform.Windows)
            {
                bass = Graphics.LoadLibrary("./lib/windows/bass.dll");
                if (bass == IntPtr.Zero) throw new Exception("Failed to load the BASS library.");
                if (UseBassFX)
                {
                    bass_fx = Graphics.LoadLibrary("./lib/windows/bass_fx.dll");
                    if (bass_fx == IntPtr.Zero) throw new Exception("Failed to load the BASS_FX library.");
                    FXPlugin = BASS_PluginLoad("./lib/windows/bass_fx.dll");
                }
            }
            else if (Graphics.Platform == Platform.Linux)
            {
                bass = Graphics.dlopen("./lib/linux/libbass.so", 0x102);
                if (bass == IntPtr.Zero) throw new Exception("Failed to load the BASS library.");
                if (UseBassFX)
                {
                    bass_fx = Graphics.dlopen("./lib/linux/libbass_fx.so", 0x102);
                    if (bass_fx == IntPtr.Zero) throw new Exception("Failed to load the BASS_FX library.");
                    FXPlugin = BASS_PluginLoad("./lib/linux/libbass_fx.so");
                }
            }
            else if (Graphics.Platform == Platform.MacOS)
            {
                throw new Exception("MacOS support has not yet been implemented.");
            }
            else
            {
                throw new Exception("No platform could be detected.");
            }
            BASS_Init(-1, 44100, 0, IntPtr.Zero);
        }

        public static void Stop()
        {
            BASS_Free();
            if (FXPlugin != 0) BASS_PluginFree(FXPlugin);
        }

        public static void BGMPlay(string Filename, int Volume = 100, int Pitch = 0)
        {
            BGMPlay(new Sound(Filename, Volume, Pitch));
        }
        public static void BGMPlay(Sound Sound)
        {
            if (BGM != null)
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

        public static void SEPlay(string Filename, int Volume = 100, int Pitch = 0)
        {
            SEPlay(new Sound(Filename, Volume, Pitch));
        }
        public static void SEPlay(Sound Sound)
        {
            Sound.Play();
        }

        public static void Play(string Filename, int Volume = 100, int Pitch = 0)
        {
            Play(new Sound(Filename, Volume, Pitch));
        }
        public static void Play(Sound Sound)
        {
            Sound.Play();
        }
    }
}
