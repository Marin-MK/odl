using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VCS
{
    public class Sprite
    {
        // Sprites have a Name property to keep track of which Sprite is which -- totally optional, of course
        public string Name { get; set; }
        public Viewport Viewport { get; set; }
        public string Filename { get; }
        private Rect _SrcRect;
        public Rect SrcRect { get { return _SrcRect; } set { this._SrcRect = value; this.Viewport.ForceUpdate(); } }
        public Bitmap Bitmap { get; set; }
        private int _X = 0;
        public int X { get { return _X; } set { this._X = value; this.Viewport.ForceUpdate(); } }
        private int _Y = 0;
        public int Y { get { return _Y; } set { this._Y = value; this.Viewport.ForceUpdate(); } }
        private int _Z = 0;
        public int Z { get { return _Z; } set { this._Z = value; this.Viewport.ForceUpdate(); } }
        private double _ZoomX = 1;
        public double ZoomX { get { return _ZoomX; } set { this._ZoomX = value; this.Viewport.ForceUpdate(); } }
        private double _ZoomY = 1;
        public double ZoomY { get { return _ZoomY; } set { this._ZoomY = value; this.Viewport.ForceUpdate(); } }
        public bool Disposed { get; set; } = false;
        private bool _Visible = true;
        public bool Visible { get { return _Visible; } set { this._Visible = value; this.Viewport.ForceUpdate(); } }
        private int _Angle = 0;
        public int Angle { get { return _Angle; } set { this._Angle = value; this.Viewport.ForceUpdate(); } }
        private bool _MirrorX = false;
        public bool MirrorX { get { return _MirrorX; } set { this._MirrorX = value; this.Viewport.ForceUpdate(); } }
        private bool _MirrorY = false;
        public bool MirrorY { get { return _MirrorY; } set { this._MirrorY = value; this.Viewport.ForceUpdate(); } }
        private int _OX = 0;
        public int OX { get { return _OX; } set { this._OX = value; this.Viewport.ForceUpdate(); } }
        private int _OY = 0;
        public int OY { get { return _OY; } set { this._OY = value; this.Viewport.ForceUpdate(); } }
        private Color _Color = new Color(255, 255, 255, 255);
        public Color Color { get { return _Color; } set { this._Color = value;  this.Viewport.ForceUpdate(); } }

        public Sprite(Viewport Viewport, string Filename)
        {
            this.Viewport = Viewport;
            this.Viewport.Sprites.Add(this);
            this.Bitmap = new Bitmap(this.Viewport.Renderer, Filename);
            this.SrcRect = new Rect(this.Bitmap.Width, this.Bitmap.Height);
            this.Viewport.ForceUpdate();
        }

        public Sprite(Viewport Viewport, int Width, int Height)
        {
            this.Viewport = Viewport;
            this.Viewport.Sprites.Add(this);
            this.Bitmap = new Bitmap(this.Viewport.Renderer, Width, Height);
            this.SrcRect = new Rect(this.Bitmap.Width, this.Bitmap.Height);
            this.Viewport.ForceUpdate();
        }

        public Sprite(Viewport Viewport, Size Size)
        {
            this.Viewport = Viewport;
            this.Viewport.Sprites.Add(this);
            this.Bitmap = new Bitmap(this.Viewport.Renderer, Size);
            this.SrcRect = new Rect(this.Bitmap.Width, this.Bitmap.Height);
            this.Viewport.ForceUpdate();
        }

        public Sprite(Viewport Viewport, Bitmap bmp)
        {
            this.Viewport = Viewport;
            this.Viewport.Sprites.Add(this);
            this.Bitmap = bmp;
            this.SrcRect = new Rect(this.Bitmap.Width, this.Bitmap.Height);
            this.Viewport.ForceUpdate();
        }

        public Sprite(Viewport Viewport)
        {
            this.Viewport = Viewport;
            this.Viewport.Sprites.Add(this);
            this.Viewport.ForceUpdate();
        }

        public void Update()
        {
            this.Viewport.Update();
        }

        public void Dispose()
        {
            this.Bitmap.Dispose();
            this.Disposed = true;
            this.Viewport.ForceUpdate();
        }
    }
}
