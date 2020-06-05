using System;
using System.Collections.Generic;
using IrrKlang;
using System.IO;
using System.Threading;

namespace odl
{
    public static class Audio
    {
        static bool Stopped = false;
        public static ISoundEngine SoundEngine;

        public static List<Sound> Sounds = new List<Sound>();

        private static int __system_volume__ = 100;
        public static int SystemVolume { get { return __system_volume__; } set { __system_volume__ = value; if (SoundEngine != null) SoundEngine.SoundVolume = value / 100f; } }

        public static void Start()
        {
            SoundEngine = new ISoundEngine();
            SoundEngine.SoundVolume = SystemVolume / 100f;
            Thread thread = null;
            thread = new Thread(delegate ()
            {
                if (Stopped) thread.Abort();
                else Update();
            });
        }

        public static void Stop()
        {
            SoundEngine.StopAllSounds();
            SoundEngine.Dispose();
            SoundEngine = null;
            Stopped = true;
        }

        public static void Play(string Filename, int Volume = 100)
        {
            Play(new Sound(Filename, Volume));
        }

        public static void Play(Sound Sound)
        {
            Sound.GuardDisposed();
            string filename = Sound.Filename;
            if (!File.Exists(filename))
            {
                if (File.Exists(filename + ".wav")) filename += ".wav";
                else if (File.Exists(filename + ".ogg")) filename += ".ogg";
                else if (File.Exists(filename + ".mp3")) filename += ".mp3";
                else if (File.Exists(filename + ".flac")) filename += ".flac";
                else if (File.Exists(filename + ".mod")) filename += ".mod";
                else if (File.Exists(filename + ".it")) filename += ".it";
                else if (File.Exists(filename + ".s3d")) filename += ".s3d";
                else if (File.Exists(filename + ".xm")) filename += ".xm";
            }
            if (Sound.__sound__ != null)
            {
                Sound.__sound__.Stop();
                Sound.__sound__.Dispose();
            }
            Sound.__sound__ = SoundEngine.Play2D(filename, Sound.Loop);
            if (Sound.__sound__ == null)
            {
                throw new Exception($"File not found - {Sound.Filename}");
            }
            Sound.Volume = Sound.__volume__;
            Sound.Position = Sound.__position__;
            Sound.Pan = Sound.__pan__;
            //Sound.__sound__.Volume = Sound.Volume / 100f;
            //Sound.__sound__.PlayPosition = Sound.Position;
            //Sound.__sound__.Pan = Sound.Pan / -100f;
            //Console.WriteLine($"Volume({Sound.__sound__.Volume}) Position({Sound.__sound__.PlayPosition}) Pan({Sound.__sound__.Pan})");
            Sound.__sound__.setSoundStopEventReceiver(new InternalStopReceiver(Sound));
            Sounds.Add(Sound);
        }

        public static void Update()
        {
            for (int i = 0; i < Sounds.Count; i++)
            {
                Sound Sound = Sounds[i];
                if (Sound.__sound__ == null) throw new Exception("Shoulda been removed error");
                if (Sound.Loop && (Sound.LoopTimes == -1 || Sound.TimesLooped < Sound.LoopTimes))
                {
                    if (Sound.__sound__.PlayPosition < Sound.__oldpos__ ||
                        Sound.LoopEndPosition != 0 && Sound.__sound__.PlayPosition >= Sound.LoopEndPosition)
                    {
                        // Finished playing or reached loop end point
                        Sound.OnLoop?.Invoke(new BaseEventArgs());
                        Sound.__sound__.PlayPosition = Sound.LoopStartPosition;
                        Sound.TimesLooped++;
                        if (Sound.LoopTimes != -1 && Sound.TimesLooped >= Sound.LoopTimes)
                        {
                            Sound.__sound__.Looped = false;
                        }
                    }
                    Sound.__oldpos__ = Sound.__sound__.PlayPosition;
                }
                if (Sound.FadeInLength != 0 && Sound.Position < Sound.FadeInLength && Sound.TimesLooped == 0)
                {
                    float fraction = (float)Sound.Position / Sound.FadeInLength;
                    Sound.__sound__.Volume = Sound.__volume__ / 100f * fraction;
                    Sound.__fade_in__ = true;
                }
                else if (Sound.__fade_in__)
                {
                    Sound.__fade_in__ = false;
                    Sound.__sound__.Volume = Sound.__volume__ / 100f;
                    Sound.OnFadedIn?.Invoke(new BaseEventArgs());
                }
                if (Sound.FadeOutLength != 0 && Sound.Position >= Sound.__sound__.PlayLength - Sound.FadeOutLength &&
                    (Sound.LoopTimes != -1 && Sound.TimesLooped == Sound.LoopTimes || Sound.LoopTimes == -1 && !Sound.Loop))
                {
                    float fraction = (float)(Sound.__sound__.PlayLength - Sound.Position) / Sound.FadeOutLength;
                    Sound.__sound__.Volume = Sound.__volume__ / 100f * fraction;
                    if (!Sound.__fade_out__)
                    {
                        Sound.OnFadingOut?.Invoke(new BaseEventArgs());
                        Sound.__fade_out__ = true;
                        Sound.__sound__.Volume = 0;
                    }
                }
                Sound.OnUpdate?.Invoke(new BaseEventArgs());
            }
        }

        public class InternalStopReceiver : ISoundStopEventReceiver
        {
            public Sound Sound;

            public InternalStopReceiver(Sound Sound)
            {
                this.Sound = Sound;
            }

            public void OnSoundStopped(ISound sound, StopEventCause reason, object userData)
            {
                this.Sound.OnStopped?.Invoke(new BaseEventArgs());
                //this.Sound.__sound__ = null;
                //Sounds.Remove(this.Sound);
            }
        }
    }
}
