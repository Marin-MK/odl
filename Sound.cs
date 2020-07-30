using System;
using Un4seen.Bass;
using static Un4seen.Bass.Bass;
using static Un4seen.Bass.AddOn.Fx.BassFx;
using System.Collections.Generic;
using System.IO;

namespace odl
{
    public class Sound
    {
        protected SoundCallback LoopCallback;

        protected List<SYNCPROC> SYNCPROC_CACHE = new List<SYNCPROC>();

        protected int Stream;
        protected int LoopEndCallback;
        protected int FadeOutCallback;

        protected int OriginalVolume = 0;

        public string Filename { get; }
        protected int _Volume = 100;
        public int Volume
        {
            get
            {
                return _Volume;
            }
            set
            {
                if (value != _Volume && Stream != 0)
                    BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, value / 100f);
                _Volume = value;
                OriginalVolume = value;
            }
        }
        protected double _Pitch = 0;
        public double Pitch
        {
            get
            {
                return _Pitch;
            }
            set
            {
                if (value != 0 && !Audio.UsingBassFX) throw new Exception("Cannot change Pitch when odl Audio is initialized with UsingBassFX set to false.");
                if (value != _Pitch && Stream != 0)
                    BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, (float) value);
                _Pitch = value;
            }
        }
        protected int _SampleRate = 0;
        public int SampleRate
        {
            get
            {
                return _SampleRate;
            }
            set
            {
                if (value != _SampleRate && Stream != 0)
                    BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_FREQ, value);
                _SampleRate = value;
            }
        }
        protected double _Pan = 0;
        public double Pan
        {
            get
            {
                return _Pan;
            }
            set
            {
                if (value != _Pan && Stream != 0)
                    BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_PAN, (float) value);
                _Pan = value;
            }
        }
        public long Position
        {
            get
            {
                return BASS_ChannelGetPosition(this.Stream) / 4;
            }
            set
            {
                BASS_ChannelSetPosition(this.Stream, value * 4);
            }
        }

        protected bool _Looping = false;
        public bool Looping
        {
            get
            {
                return _Looping;
            }
            set
            {
                _Looping = value;
                ReconfigureLooping();
            }
        }
        protected long _LoopStart = 0;
        public long LoopStart
        {
            get
            {
                return _LoopStart;
            }
            set
            {
                _LoopStart = value;
                ReconfigureLooping();
            }
        }
        protected long _LoopEnd = 0;
        public long LoopEnd
        {
            get
            {
                return _LoopEnd;
            }
            set
            {
                _LoopEnd = value;
                ReconfigureLooping();
            }
        }

        protected long _FadeInLength = 0;
        public long FadeInLength
        {
            get
            {
                return _FadeInLength;
            }
            set
            {
                if (value != _FadeInLength && Position == 0)
                {
                    this.Volume = 0;
                    this.SlideVolume(100, value);
                }
                _FadeInLength = value;
            }
        }
        protected long _FadeOutLength = 0;
        public long FadeOutLength
        {
            get
            {
                return _FadeOutLength;
            }
            set
            {
                if (value != _FadeOutLength)
                {
                    if (FadeOutCallback != 0) RemoveCallback(FadeOutCallback);
                    FadeOutCallback = AddPositionCallback(Length - value, delegate (long Position)
                    {
                        SlideVolume(0, FadeOutLength);
                    });
                }
                _FadeOutLength = value;
            }
        }

        public bool Playing
        {
            get
            {
                return BASS_ChannelIsActive(this.Stream) == BASSActive.BASS_ACTIVE_PLAYING;
            }
        }

        public long Length { get; protected set; }

        public Sound(string Filename, int Volume = 100, double Pitch = 0)
        {
            if (Pitch != 0 && !Audio.UsingBassFX) throw new Exception("Cannot change Pitch when odl Audio is initialized with UsingBassFX set to false.");
            string OriginalFilename = Filename;
            if (!File.Exists(Filename))
            {
                if (File.Exists(Filename + ".ogg")) Filename += ".ogg";
                else if (File.Exists(Filename + ".wav")) Filename += ".wav";
                else if (File.Exists(Filename + ".mp3")) Filename += ".mp3";
            }
            if (Audio.UsingBassFX)
            {
                int BaseStream = BASS_StreamCreateFile(Filename, 0, 0, BASSFlag.BASS_STREAM_DECODE);
                this.Stream = BASS_FX_TempoCreate(BaseStream, BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_FX_FREESOURCE);
            }
            else
            {
                this.Stream = BASS_StreamCreateFile(Filename, 0, 0, 0);
            }
            float sr = 0;
            BASS_ChannelGetAttribute(this.Stream, BASSAttribute.BASS_ATTRIB_FREQ, ref sr);
            this.Length = BASS_ChannelGetLength(this.Stream) / 4;
            _SampleRate = (int) sr;
            this.Filename = OriginalFilename;
            this.Volume = Volume;
            this.Pitch = Pitch;
        }

        public int AddPositionCallback(long Sample, SoundCallback Callback)
        {
            SYNCPROC proc = delegate (int Handle, int Channel, int Data, IntPtr User) { Callback(Sample); };
            SYNCPROC_CACHE.Add(proc);
            return BASS_ChannelSetSync(this.Stream, BASSSync.BASS_SYNC_POS, Math.Max(0, Sample * 4 - 18000), proc, IntPtr.Zero);
        }

        public int AddEndCallback(SoundCallback Callback)
        {
            SYNCPROC proc = delegate (int Handle, int Channel, int Data, IntPtr User) { Callback(this.Length); };
            SYNCPROC_CACHE.Add(proc);
            return BASS_ChannelSetSync(this.Stream, BASSSync.BASS_SYNC_END | BASSSync.BASS_SYNC_MIXTIME, 0, proc, IntPtr.Zero);
        }

        public int AddSlideCallback(SlideCallback Callback)
        {
            SYNCPROC proc = delegate (int Handle, int Channel, int Data, IntPtr User)
            {
                Callback(Data);
            };
            SYNCPROC_CACHE.Add(proc);
            return BASS_ChannelSetSync(this.Stream, BASSSync.BASS_SYNC_SLIDE | BASSSync.BASS_SYNC_MIXTIME, 0, proc, IntPtr.Zero);
        }

        public void AddLoopCallback(SoundCallback Callback)
        {
            LoopCallback += Callback;
        }

        public void RemoveCallback(int Handle)
        {
            BASS_ChannelRemoveSync(this.Stream, Handle);
        }

        protected void ReconfigureLooping()
        {
            BASS_ChannelFlags(this.Stream, 0, BASSFlag.BASS_SAMPLE_LOOP);
            if (LoopEndCallback != 0) RemoveCallback(LoopEndCallback);
            if (LoopStart != 0 && LoopEnd == 0)
            {
                LoopEndCallback = AddEndCallback(delegate (long Position) { this.Position = LoopStart; LoopCallback?.Invoke(Position); });
            }
            else if (LoopStart != LoopEnd)
            {
                LoopEndCallback = AddPositionCallback(LoopEnd, delegate (long Position) { this.Position = LoopStart; LoopCallback?.Invoke(Position); });
            }
            else if (Looping)
            {
                LoopEndCallback = AddEndCallback(delegate (long Position) { LoopCallback?.Invoke(Position); });
                BASS_ChannelFlags(this.Stream, BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
            }
        }

        public void SlideVolume(int Volume, long Time)
        {
            _Volume = Volume;
            BASS_ChannelSlideAttribute(this.Stream, BASSAttribute.BASS_ATTRIB_VOL, Volume / 100f, (int) Math.Round((double) Time / SampleRate * 1000));
        }

        public void SlidePan(double Pan, long Time)
        {
            _Pan = Pan;
            BASS_ChannelSlideAttribute(this.Stream, BASSAttribute.BASS_ATTRIB_PAN, (float) Pan, (int) Math.Round((double) Time / SampleRate * 1000));
        }

        public void SlidePitch(double Pitch, long Time)
        {
            _Pitch = Pitch;
            BASS_ChannelSlideAttribute(this.Stream, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, (float) Pitch, (int) Math.Round((double) Time / SampleRate * 1000));
        }

        public void SlideSampleRate(int SampleRate, long Time)
        {
            _Pitch = Pitch;
            BASS_ChannelSlideAttribute(this.Stream, BASSAttribute.BASS_ATTRIB_FREQ, SampleRate, (int) Math.Round((double) Time / SampleRate * 1000));
        }

        public void FadeOut(long Time)
        {
            SlideVolume(0, Time);
        }
        public void FadeOut(long Time, SlideCallback Callback)
        {
            SlideVolume(0, Time);
            int Handle = 0;
            Handle = AddSlideCallback(delegate (int SlideType)
            {
                Callback(SlideType);
                RemoveCallback(Handle);
            });
        }

        public void FadeIn(long Time)
        {
            SlideVolume(OriginalVolume, Time);
        }
        public void FadeIn(long Time, SlideCallback Callback)
        {
            SlideVolume(OriginalVolume, Time);
            int Handle = 0;
            Handle = AddSlideCallback(delegate (int SlideType)
            {
                Callback(SlideType);
                RemoveCallback(Handle);
            });
        }

        public void Pause()
        {
            BASS_ChannelPause(this.Stream);
        }

        public void Resume()
        {
            BASS_ChannelPlay(this.Stream, false);
        }

        public void Play()
        {
            if (BASS_ChannelIsActive(this.Stream) == BASSActive.BASS_ACTIVE_PAUSED)
            {
                BASS_ChannelPlay(this.Stream, false);
            }
            else
            {
                BASS_ChannelPlay(this.Stream, true);
            }
        }

        public void Start()
        {
            BASS_ChannelPlay(this.Stream, true);
        }

        public void Stop()
        {
            BASS_ChannelStop(this.Stream);
        }
    }
}
