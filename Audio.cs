using System;
using System.Collections.Generic;
using static SDL2.SDL_mixer;

namespace ODL
{
    public static class Audio
    {
        public const int MAX_CHANNELS = 64;

        public static AudioFile BGM;

        public static void Start()
        {
            MIX_InitFlags Flags = MIX_InitFlags.MIX_INIT_FLAC |
                                  MIX_InitFlags.MIX_INIT_MP3 |
                                  MIX_InitFlags.MIX_INIT_OGG |
                                  MIX_InitFlags.MIX_INIT_FLUIDSYNTH;
            int FlagsInt = Convert.ToInt32(Flags);
            if ((Mix_Init(Flags) & FlagsInt) != FlagsInt)
            {
                throw new Exception("Failed to initialize one or more audio formats.");
            }
            if (Mix_OpenAudio(MIX_DEFAULT_FREQUENCY, MIX_DEFAULT_FORMAT, 2, 1024) != 0)
            {
                throw new Exception("Failed to open audio device.");
            }
            Mix_AllocateChannels(MAX_CHANNELS);
        }

        public static void PlayBGM(string Filename, int Volume = 100, int Fade = 0, bool Loop = true)
        {
            PlayBGM(new AudioFile(Filename, Volume, Loop, Fade));
        }
        public static void PlayBGM(AudioFile bgm)
        {
            BGM = bgm;
            BGM.Start();
        }

        public static void StopBGM(int Fade = 0)
        {
            BGM.Stop(Fade);
        }

        public static void Stop()
        {
            Mix_CloseAudio();
            Mix_Quit();
        }
    }
}
