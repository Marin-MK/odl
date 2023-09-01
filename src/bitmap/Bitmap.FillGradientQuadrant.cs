using System;

namespace odl;

public partial class Bitmap
{
    public virtual void FillGradientQuadrant(int ox, int oy, int Radius, Location Location, Color c1, Color c2, double Multiplier = 1.0, bool FillOutsideQuadrant = true)
    {
        if (Locked) throw new BitmapLockedException();
        int x = Radius - 1;
        int y = 0;
        int dx = 1;
        int dy = 1;
        if (FillOutsideQuadrant) FillRect(ox, oy, Radius, Radius, c1);
        if (Location == Location.TopLeft)
        {
            ox += Radius - 1;
            oy += Radius - 1;
        }
        else if (Location == Location.TopRight) oy += Radius - 1;
        else if (Location == Location.BottomLeft) ox += Radius - 1;
        Point center = new Point(ox, oy);
        int err = dx - (Radius << 1);
        while (x >= y)
        {
            if (Location == Location.TopRight) // 0 - 90
            {
                for (int i = ox + y; i <= ox + x; i++)
                {
                    SetPixel(i, oy - y, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(i, oy - oy)) / Radius));
                }
                for (int i = oy - x; i <= oy - y; i++)
                {
                    SetPixel(ox + y, i, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(ox + y, i)) / Radius));
                }
            }
            else if (Location == Location.TopLeft) // 90 - 180
            {
                for (int i = ox - x; i <= ox - y; i++)
                {
                    SetPixel(i, oy - y, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(i, oy - y)) / Radius));
                }
                for (int i = oy - x; i <= oy - y; i++)
                {
                    SetPixel(ox - y, i, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(ox - y, i)) / Radius));
                }
            }
            else if (Location == Location.BottomLeft) // 180 - 270
            {
                for (int i = ox - x; i <= ox - y; i++)
                {
                    SetPixel(i, oy + y, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(i, oy + oy)) / Radius));
                }
                for (int i = oy + y; i <= oy + x; i++)
                {
                    SetPixel(ox - y, i, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(ox - y, i)) / Radius));
                }
            }
            else if (Location == Location.BottomRight) // 270 - 360
            {
                for (int i = ox + y; i <= ox + x; i++)
                {
                    SetPixel(i, oy + y, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(i, oy + oy)) / Radius));
                }
                for (int i = oy + y; i <= oy + x; i++)
                {
                    SetPixel(ox + y, i, Interpolate2D(c1, c2, Multiplier * center.Distance(new Point(ox + y, i)) / Radius));
                }
            }
            y++;
            err += dy;
            dy += 2;
            if (err > 0)
            {
                x--;
                dx += 2;
                err += dx - (Radius << 1);
            }
        }
        if (this.Renderer != null) this.Renderer.Update();
    }
}

