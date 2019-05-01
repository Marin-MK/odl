using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCS
{
    public static class Input
    {
        public static string ButtonInput = "";
        public static string LastButtonInput = "";
        public static long InputCount = 0;
        public static bool LShift = false;
        public static bool RShift = false;
        public static bool LCtrl = false;
        public static bool RCtrl = false;
        public static bool LAlt = false;
        public static bool RAlt = false;
        public static bool Backspace = false;
        public static bool Enter = false;

        public static bool Ctrl { get { return LCtrl || RCtrl; } }
        public static bool Shift { get { return LShift || RShift; } }
        public static bool Alt { get { return LAlt || RAlt; } }
    }
}
