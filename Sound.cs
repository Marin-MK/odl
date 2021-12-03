using System;
using System.Collections.Generic;
using System.IO;

namespace odl;

public class Sound
{
    protected SoundCallback LoopCallback;

    internal List<Audio.BASS_Syncproc> SYNCPROC_CACHE = new List<Audio.BASS_Syncproc>();

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
                Audio.BASS_ChannelSetAttribute(Stream, Audio.BASS_Attribute.BASS_ATTRIB_VOL, value / 100f);
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
                Audio.BASS_ChannelSetAttribute(Stream, Audio.BASS_Attribute.BASS_ATTRIB_TEMPO_PITCH, (float)value);
            _Pitch = value;
        }
    }
    public int OriginalSampleRate { get; protected set; }
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
                Audio.BASS_ChannelSetAttribute(Stream, Audio.BASS_Attribute.BASS_ATTRIB_FREQ, value);
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
                Audio.BASS_ChannelSetAttribute(Stream, Audio.BASS_Attribute.BASS_ATTRIB_PAN, (float)value);
            _Pan = value;
        }
    }
    public long Position
    {
        get
        {
            return Audio.BASS_ChannelGetPosition(this.Stream) / 4;
        }
        set
        {
            Audio.BASS_ChannelSetPosition(this.Stream, value * 4);
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
            return Audio.BASS_ChannelIsActive(this.Stream) == 1;
        }
    }

    public bool Alive
    {
        get
        {
            return Audio.BASS_ChannelIsActive(this.Stream) != 0;
        }
    }

    public long Length { get; protected set; }

    public unsafe Sound(string Filename, int Volume = 100, double Pitch = 0)
    {
        if (Pitch != 0 && !Audio.UsingBassFX) throw new Exception("Cannot change Pitch when Audio is initialized with UsingBassFX set to false.");
        string OriginalFilename = Filename;
        if (!File.Exists(Filename))
        {
            if (File.Exists(Filename + ".ogg")) Filename += ".ogg";
            else if (File.Exists(Filename + ".wav")) Filename += ".wav";
            else if (File.Exists(Filename + ".mp3")) Filename += ".mp3";
            else if (File.Exists(Filename + ".mid")) Filename += ".mid";
        }
        if (Filename.EndsWith(".mid"))
        {
            if (Audio.UsingBassMidi)
            {
                this.Stream = Audio.BASS_MIDI_StreamCreateFile(false, Filename, 0, 0, Audio.BASS_Flag.BASS_STREAM_AUTOFREE);
                Audio.BASS_MIDI_FONT[] list = new Audio.BASS_MIDI_FONT[Audio.Soundfonts.Count];
                for (int i = 0; i < Audio.Soundfonts.Count; i++)
                {
                    list[i] = new Audio.BASS_MIDI_FONT();
                    list[i].font = Audio.Soundfonts[i];
                    list[i].preset = -1;
                    list[i].bank = 0;
                }
                fixed (Audio.BASS_MIDI_FONT* sPtr = list) Audio.BASS_MIDI_StreamSetFonts(this.Stream, sPtr, Audio.Soundfonts.Count);
            }
            else Console.WriteLine($"No MIDI support for file {Filename}");
        }
        else if (Audio.UsingBassFX)
        {
            int BaseStream = Audio.BASS_StreamCreateFile(false, Filename, 0, 0, Audio.BASS_Flag.BASS_STREAM_DECODE);
            this.Stream = Audio.BASS_FX_TempoCreate(BaseStream, Audio.BASS_Flag.BASS_STREAM_AUTOFREE | Audio.BASS_Flag.BASS_FX_FREESOURCE);
        }
        else
        {
            this.Stream = Audio.BASS_StreamCreateFile(false, Filename, 0, 0, Audio.BASS_Flag.BASS_STREAM_AUTOFREE);
        }
        float sr = 0;
        Audio.BASS_ChannelGetAttribute(this.Stream, Audio.BASS_Attribute.BASS_ATTRIB_FREQ, ref sr);
        this.OriginalSampleRate = (int)sr;
        this.Length = Audio.BASS_ChannelGetLength(this.Stream) / 4;
        _SampleRate = (int)sr;
        this.Filename = OriginalFilename;
        this.Volume = Volume;
        this.Pitch = Pitch;
    }

    public int AddPositionCallback(long Sample, SoundCallback Callback)
    {
        Audio.BASS_Syncproc proc = delegate (int Handle, int Channel, int Data, IntPtr User) { Callback(Sample); };
        SYNCPROC_CACHE.Add(proc);
        return Audio.BASS_ChannelSetSync(this.Stream, Audio.BASS_Sync.BASS_SYNC_POS, Math.Max(0, Sample * 4 - 18000), proc, IntPtr.Zero);
    }

    public int AddEndCallback(SoundCallback Callback)
    {
        Audio.BASS_Syncproc proc = delegate (int Handle, int Channel, int Data, IntPtr User) { Callback(this.Length); };
        SYNCPROC_CACHE.Add(proc);
        return Audio.BASS_ChannelSetSync(this.Stream, Audio.BASS_Sync.BASS_SYNC_END | Audio.BASS_Sync.BASS_SYNC_MIXTIME, 0, proc, IntPtr.Zero);
    }

    public int AddSlideCallback(SlideCallback Callback)
    {
        Audio.BASS_Syncproc proc = delegate (int Handle, int Channel, int Data, IntPtr User)
        {
            Callback(Data);
        };
        SYNCPROC_CACHE.Add(proc);
        return Audio.BASS_ChannelSetSync(this.Stream, Audio.BASS_Sync.BASS_SYNC_SLIDE | Audio.BASS_Sync.BASS_SYNC_MIXTIME, 0, proc, IntPtr.Zero);
    }

    public void AddLoopCallback(SoundCallback Callback)
    {
        LoopCallback += Callback;
    }

    public void RemoveCallback(int Handle)
    {
        Audio.BASS_ChannelRemoveSync(this.Stream, Handle);
    }

    protected void ReconfigureLooping()
    {
        Audio.BASS_ChannelFlags(this.Stream, 0, Audio.BASS_Flag.BASS_SAMPLE_LOOP);
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
            Audio.BASS_ChannelFlags(this.Stream, Audio.BASS_Flag.BASS_SAMPLE_LOOP, Audio.BASS_Flag.BASS_SAMPLE_LOOP);
        }
    }

    public void SlideVolume(int Volume, long Time)
    {
        _Volume = Volume;
        Audio.BASS_ChannelSlideAttribute(this.Stream, Audio.BASS_Attribute.BASS_ATTRIB_VOL, Volume / 100f, (int)Math.Round((double)Time / SampleRate * 1000));
    }

    public void SlidePan(double Pan, long Time)
    {
        _Pan = Pan;
        Audio.BASS_ChannelSlideAttribute(this.Stream, Audio.BASS_Attribute.BASS_ATTRIB_PAN, (float)Pan, (int)Math.Round((double)Time / SampleRate * 1000));
    }

    public void SlidePitch(double Pitch, long Time)
    {
        _Pitch = Pitch;
        Audio.BASS_ChannelSlideAttribute(this.Stream, Audio.BASS_Attribute.BASS_ATTRIB_TEMPO_PITCH, (float)Pitch, (int)Math.Round((double)Time / SampleRate * 1000));
    }

    public void SlideSampleRate(int SampleRate, long Time)
    {
        _Pitch = Pitch;
        Audio.BASS_ChannelSlideAttribute(this.Stream, Audio.BASS_Attribute.BASS_ATTRIB_FREQ, SampleRate, (int)Math.Round((double)Time / SampleRate * 1000));
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
        Audio.BASS_ChannelPause(this.Stream);
    }

    public void Resume()
    {
        Audio.BASS_ChannelPlay(this.Stream, false);
    }

    public void Play()
    {
        if (Audio.BASS_ChannelIsActive(this.Stream) == 3)
        {
            Audio.BASS_ChannelPlay(this.Stream, false);
        }
        else
        {
            Audio.BASS_ChannelPlay(this.Stream, true);
        }
    }

    public void Start()
    {
        Audio.BASS_ChannelPlay(this.Stream, true);
    }

    public void Stop()
    {
        Audio.BASS_ChannelStop(this.Stream);
    }
}
