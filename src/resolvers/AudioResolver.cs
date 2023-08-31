using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace odl;

public class AudioResolver : FileResolver
{
    public AudioResolver()
    {
        AddExtension(".wav");
        AddExtension(".ogg");
        AddExtension(".mp3");
        AddExtension(".mid");
        AddExtension(".midi");
    }
}
