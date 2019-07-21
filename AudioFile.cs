using System;
using System.IO;
using System.Runtime.InteropServices;
using static SDL2.SDL_mixer;

namespace ODL
{
    public class AudioFile
    {
        protected string _Filename;
        public string Filename { get { return _Filename; } }

        protected int _Volume;
        public int Volume { get { return _Volume; } }

        protected bool _Loop;
        public bool Loop { get { return _Loop; } }

        protected int _Fade;
        public int Fade { get { return _Fade; } }

        protected int _Channel;
        public int Channel { get { return _Channel; } }

        public AudioFile(string Filename, int Volume, bool Loop, int Fade = 0)
        {
            _Filename = Filename;
            _Volume = Volume;
            _Loop = Loop;
            _Fade = Fade;
        }

        private IntPtr Chunk;

        public void Start()
        {
            IntPtr ptr = SDL2.SDL_custommixer.SDL_RWFromFile(this.Filename, "rb");
            Console.WriteLine(SDL2.SDL_custommixer.Mix_GetError());
            Chunk = Mix_LoadWAV_RW(ptr, 0);
            _Channel = Mix_FadeInChannel(-1, this.Chunk, this.Loop ? -1 : 0, this.Fade);

        }

        public void Stop(int Fade = 0)
        {
            Mix_FadeOutChannel(this.Channel, this.Fade);
        }

        /*Mix_Chunk *LoadSound(char *resourcefilename, char *soundfilename)
        {
            //Get the sound's buffer and size from the resource file
            int filesize = 0;
            char *buffer = GetBufferFromResource(resourcefilename, soundfilename, &filesize);

            //Load the buffer into a surface using RWops
            SDL_RWops *rw = SDL_RWFromMem(buffer, filesize);
            Mix_Chunk *sound = Mix_LoadWAV_RW(rw, 1);

            //Release the buffer memory
            free(buffer);

            //Return the sound
            return sound;
        }*/

    }

    public enum AudioOptions
    {
        FadeOld,
        LoopPlay
    }
}

namespace SDL2
{
    public static class SDL_custommixer
    {
        [DllImport("SDL2", EntryPoint = "SDL_RWFromFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_RWFromFile(string file, string mode);

        [DllImport("SDL2_mixer", EntryPoint = "Mix_GetError", CallingConvention = CallingConvention.Cdecl)]
        public static extern string Mix_GetError();
    }
}