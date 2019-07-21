using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODL
{
    public interface ISprite
    {
        string Name { get; set; }
        Viewport Viewport { get; }
        string Filename { get; }
        Rect SrcRect { get; set; }
        IBitmap Bitmap { get; set; }
        int X { get; set; }
        int Y { get; set; }
        int Z { get; set; }
        double ZoomX { get; set; }
        double ZoomY { get; set; }
        bool Disposed { get; }
        bool Visible { get; set; }
        int Angle { get; set; }
        bool MirrorX { get; set; }
        bool MirrorY { get; set; }
        int OX { get; set; }
        int OY { get; set; }
        Color Color { get; set; }
        List<Point> MultiplePositions { get; set; }

        void Update();

        void Dispose();
    }
}
