using System;
using System.Collections.Generic;

namespace odl
{
    public class MultiSprite : ISprite, IDisposable
    {
        public Dictionary<string, Sprite> SpriteList = new Dictionary<string, Sprite>();
        public Viewport Viewport { get; protected set; }
        public string Name { get { throw new MethodNotSupportedException(this); } set { throw new MethodNotSupportedException(this); } }
        public string Filename { get { throw new MethodNotSupportedException(this); } }
        public Rect SrcRect { get { throw new MethodNotSupportedException(this); } set { throw new MethodNotSupportedException(this); } }
        public Bitmap Bitmap { get { throw new MethodNotSupportedException(this); } set { throw new MethodNotSupportedException(this); } }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public double ZoomX { get { throw new MethodNotSupportedException(this); } set { throw new MethodNotSupportedException(this); } }
        public double ZoomY { get { throw new MethodNotSupportedException(this); } set { throw new MethodNotSupportedException(this); } }
        public bool Disposed { get { throw new MethodNotSupportedException(this); } }
        private bool _Visible = true;
        public bool Visible { get { return _Visible; } set { _Visible = value; this.Update(); } }
        public int Angle { get { throw new MethodNotSupportedException(this); } set { throw new MethodNotSupportedException(this); } }
        public bool MirrorX { get { throw new MethodNotSupportedException(this); } set { throw new MethodNotSupportedException(this); } }
        public bool MirrorY { get { throw new MethodNotSupportedException(this); } set { throw new MethodNotSupportedException(this); } }
        private double _OX = 0;
        public double OX { get { return _OX; } set { _OX = value; this.Update(); } }
        private double _OY = 0;
        public double OY { get { return _OY; } set { _OY = value; this.Update(); } }
        public Color Color { get { throw new MethodNotSupportedException(this); } set { throw new MethodNotSupportedException(this); } }
        public List<Point> MultiplePositions { get { throw new MethodNotSupportedException(this); } set { throw new MethodNotSupportedException(this); } }
        public byte Opacity { get { throw new MethodNotSupportedException(this); } set { throw new MethodNotSupportedException(this); } }
        private bool _FactorZoomIntoOrigin = true;
        public bool FactorZoomIntoOrigin { get { return _FactorZoomIntoOrigin; } set { _FactorZoomIntoOrigin = value; this.Update(); } }
        public bool DestroyBitmap { get; set; } = true;

        public MultiSprite(Viewport viewport)
        {
            this.Viewport = viewport;
        }

        public void AddBitmap(string Name, Bitmap bmp, int X = 0, int Y = 0)
        {
            Sprite s = new Sprite(this.Viewport);
            s.Bitmap = bmp;
            s.X = X + this.X;
            s.Y = Y + this.Y;
            s.OX = this.OX;
            s.OY = this.OY;
            s.Visible = this.Visible;
            this.SpriteList.Add(Name, s);
            this.Viewport.Sprites.Add(s);
            this.Viewport.Update();
        }

        public void RemoveBitmap(string Name)
        {
            Sprite s = this.SpriteList[Name];
            this.SpriteList.Remove(Name);
            this.Viewport.Sprites.Remove(s);
            this.Viewport.Update();
        }

        public void Update()
        {
            foreach (ISprite s in this.SpriteList.Values)
            {
                s.OX = this.OX;
                s.OY = this.OY;
                s.Visible = this.Visible;
            }
        }

        public void Dispose()
        {
            foreach (string key in this.SpriteList.Keys)
            {
                this.SpriteList[key].Dispose();
            }
            this.SpriteList.Clear();
        }
    }
}
