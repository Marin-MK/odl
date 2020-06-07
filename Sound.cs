using IrrKlang;
using System;
using System.Collections.Generic;
using System.Text;

namespace odl
{
    public class Sound
    {
        public delegate void BaseEvent(BaseEventArgs e);

        // General
        public string Filename;
        public int __volume__ = 100;
        public int Volume { get { return __sound__ == null ? __volume__ : (int) Math.Round(__sound__.Volume * 100); } set { if (__sound__ != null) __sound__.Volume = value / 100f; __volume__ = value; } }
        public uint __position__ = 0;
        public uint Position { get { return __sound__ == null ? __position__ : __sound__.PlayPosition; } set { if (__sound__ != null) __sound__.PlayPosition = value; __position__ = value; __oldpos__ = value; } }
        public int __pan__ = 0;
        public int Pan { get { return __sound__ == null ? __pan__ : (int) Math.Round(__sound__.Pan * -100); } set { if (__sound__ != null) __sound__.Pan = value / -100f; __pan__ = value; } }
        public bool Paused { get { return __sound__ == null ? false : __sound__.Paused; } }
        public bool Playing { get { return __sound__ != null && !Paused && !__sound__.Finished; } }
        public bool Disposed = false;

        // Looping
        public bool Loop = false;
        public int LoopTimes = -1;
        public uint LoopStartPosition = 0;
        public uint LoopEndPosition = 0;
        public int TimesLooped = 0;

        // Fading
        public uint FadeInLength = 0;
        public uint FadeOutLength = 0;

        // Events
        public BaseEvent OnFadedIn;
        public BaseEvent OnFadingOut;
        public BaseEvent OnLoop;
        public BaseEvent OnStopped;
        public BaseEvent OnUpdate;

        // Internal
        public bool __fade_in__ = false;
        public bool __fade_out__ = false;
        public uint __oldpos__ = 0;
        public ISound __sound__;

        public Sound(string Filename, int Volume = 100)
        {
            this.Filename = Filename;
            this.Volume = Volume;
        }

        public void Play()
        {
            GuardDisposed();
            Audio.Play(this);
        }

        public void Pause()
        {
            GuardDisposed();
            __sound__.Paused = true;
        }

        public void Resume()
        {
            GuardDisposed();
            __sound__.Paused = false;
        }

        public void Stop()
        {
            GuardDisposed();
            __sound__.Stop();
        }

        public void Dispose()
        {
            if (__sound__ != null) __sound__.Dispose();
            __sound__ = null;
            this.Disposed = true;
        }

        public void GuardDisposed()
        {
            if (this.Disposed) throw new Exception($"Sound already disposed.");
        }
    }
}
