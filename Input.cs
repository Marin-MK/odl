using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace VCS
{
    public static class Input
    {
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

        public static List<long> OldKeysDown = new List<long>();
        public static List<long> KeysDown = new List<long>();

        public static void Register(SDL_Keycode code, bool value)
        {
            long key = Convert.ToInt64(code);
            if (value)
            {
                if (!KeysDown.Contains(key)) KeysDown.Add(key);
            }
            else
            {
                KeysDown.Remove(key);
            }
        }

        public static void IterationEnd()
        {
            OldKeysDown = new List<long>(KeysDown);
        }

        public static bool Trigger(SDL_Keycode code)
        {
            long key = Convert.ToInt64(code);
            return !OldKeysDown.Contains(key) && KeysDown.Contains(key);
        }

        public static bool Press(SDL_Keycode code)
        {
            long key = Convert.ToInt64(code);
            return KeysDown.Contains(key);
        }
    }
}
