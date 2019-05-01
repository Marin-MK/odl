using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCS
{
    public class AnimatedSprite : Sprite
    {
        private Size _FrameSize;
        public Size FrameSize { get { return _FrameSize; } set { this._FrameSize = value; this.Viewport.ForceUpdate(); } }
        private int _UpdateDelay;
        public int UpdateDelay { get { return _UpdateDelay; } set { this._UpdateDelay = value; this.Viewport.ForceUpdate(); } }
        public SDL2.SDL.SDL_TimerCallback Callback { get; set; }

        public AnimatedSprite(Viewport Viewport, Bitmap Bitmap, int UpdateDelay = 50, Size FrameSize = null) : base(Viewport, Bitmap)
        {
            if (FrameSize == null)
            {
                this.FrameSize = new Size(this.Bitmap.Height, this.Bitmap.Height);
            }
            else
            {
                this.FrameSize = FrameSize;
            }
            this.UpdateDelay = UpdateDelay;
            this.SrcRect = new Rect(0, 0, FrameSize.Width, FrameSize.Height);
            AddTimer();
        }

        public AnimatedSprite(Viewport Viewport, Bitmap Bitmap, int UpdateDelay = 50, int FrameWidth = -1, int FrameHeight = -1) : base(Viewport, Bitmap)
        {
            if (FrameHeight == -1) FrameHeight = this.Bitmap.Height;
            if (FrameWidth == -1) FrameWidth = FrameHeight;
            this.FrameSize = new Size(FrameWidth, FrameHeight);
            this.UpdateDelay = UpdateDelay;
            this.SrcRect = new Rect(0, 0, FrameWidth, FrameHeight);
            AddTimer();
        }

        public AnimatedSprite(Viewport Viewport, string File, int UpdateDelay, Size FrameSize) : base(Viewport, File)
        {
            this.UpdateDelay = UpdateDelay;
            this.SrcRect = new Rect(0, 0, FrameSize.Width, FrameSize.Height);
            AddTimer();
        }

        public AnimatedSprite(Viewport Viewport, string File, int UpdateDelay = 50, int FrameWidth = -1, int FrameHeight = -1) : base(Viewport, File)
        {
            if (FrameHeight == -1) FrameHeight = this.Bitmap.Height;
            if (FrameWidth == -1) FrameWidth = FrameHeight;
            this.FrameSize = new Size(FrameWidth, FrameHeight);
            this.UpdateDelay = UpdateDelay;
            this.SrcRect = new Rect(0, 0, FrameWidth, FrameHeight);
            AddTimer();
        }
        
        void AddTimer()
        {
            this.Callback = delegate (uint Interval, IntPtr Target)
            {
                this.SrcRect.X += this.FrameSize.Width;
                if (this.SrcRect.X >= this.Bitmap.Width)
                {
                    this.SrcRect.X = 0;
                    this.SrcRect.Y += this.FrameSize.Height;
                    if (this.SrcRect.Y >= this.Bitmap.Height)
                    {
                        this.SrcRect.Y = 0;
                    }
                }
                this.Viewport.ForceUpdate();
                return Interval;
            };
            SDL2.SDL.SDL_AddTimer((uint) this.UpdateDelay, this.Callback, IntPtr.Zero);
        }
    }
}
