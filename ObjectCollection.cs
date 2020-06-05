using System;
using System.Collections.Generic;
using System.Text;

namespace odl
{
    public class ObjectCollection
    {
        public List<ViewportData> Viewports;

        public ObjectCollection(Renderer Renderer)
        {
            Viewports = new List<ViewportData>();
            for (int i = 0; i < Renderer.Viewports.Count; i++)
            {
                Viewports.Add(new ViewportData(Renderer.Viewports[i]));
            }
        }

        public void CompareWith(ObjectCollection Collection)
        {
            for (int i = 0; i < Viewports.Count; i++)
            {
                ViewportData vp1 = Viewports[i];
                ViewportData vp2 = i < Collection.Viewports.Count ? Collection.Viewports[i] : null;
                if (vp2 == null) Console.WriteLine($"One disposed viewport ({vp1.X}, {vp1.Y}, {vp1.Width}, {vp1.Height})");
                else
                {
                    vp1.CompareWith(vp2);
                }
            }
            for (int i = Viewports.Count; i < Collection.Viewports.Count; i++)
            {
                ViewportData vp = Collection.Viewports[i];
                Console.WriteLine($"One new viewport ({vp.X}, {vp.Y}, {vp.Width}, {vp.Height})");
            }
        }

        public class ViewportData
        {
            public List<SpriteData> Sprites;
            public int X;
            public int Y;
            public int Width;
            public int Height;
            public int OX;
            public int OY;
            public ColorData Color;
            public bool Visible;
            public bool Disposed;
            public int Z;

            public ViewportData(Viewport Viewport)
            {
                Sprites = new List<SpriteData>();
                for (int i = 0; i < Viewport.Sprites.Count; i++)
                {
                    Sprites.Add(new SpriteData(this, Viewport.Sprites[i]));
                }
                this.X = Viewport.X;
                this.Y = Viewport.Y;
                this.Width = Viewport.Width;
                this.Height = Viewport.Height;
                this.OX = Viewport.OX;
                this.OY = Viewport.OY;
                this.Color = new ColorData(Viewport.Color);
                this.Visible = Viewport.Visible;
                this.Disposed = Viewport.Disposed;
                this.Z = Viewport.Z;
            }

            public void CompareWith(ViewportData Viewport)
            {
                string Diff = "Viewport properties changed: ";
                if (this.X != Viewport.X) Diff += $"(X {this.X}->{Viewport.X}) ";
                if (this.Y != Viewport.Y) Diff += $"(Y {this.Y}->{Viewport.Y}) ";
                if (this.Width != Viewport.Width) Diff += $"(Width {this.Width}->{Viewport.Width}) ";
                if (this.Height != Viewport.Height) Diff += $"(Height {this.Height}->{Viewport.Height}) ";
                if (this.OX != Viewport.OX) Diff += $"(OX {this.OX}->{Viewport.OX}) ";
                if (this.OY != Viewport.OY) Diff += $"(OY {this.OY}->{Viewport.OY}) ";
                if (this.Visible != Viewport.Visible) Diff += $"(Visible {this.Visible}->{Viewport.Visible}) ";
                if (this.Disposed != Viewport.Disposed) Diff += $"(Disposed {this.Disposed}->{Viewport.Disposed}) ";
                if (this.Z != Viewport.Z) Diff += $"(Z {this.Z}->{Viewport.Z}) ";
                Diff += this.Color.CompareWith(Viewport.Color);
                if (Diff != "Viewport properties changed: ") Console.WriteLine(Diff.Substring(0, Diff.Length - 1) + ")");
                for (int i = 0; i < Sprites.Count; i++)
                {
                    SpriteData s1 = Sprites[i];
                    SpriteData s2 = i < Viewport.Sprites.Count ? Viewport.Sprites[i] : null;
                    if (s2 == null) Console.WriteLine($"One disposed sprite ({s1.X}, {s1.Y}, bmp({s1.Bitmap.Width},{s1.Bitmap.Height}))");
                    else
                    {
                        s1.CompareWith(s2);
                    }
                }
                for (int i = Sprites.Count; i < Viewport.Sprites.Count; i++)
                {
                    SpriteData s = Viewport.Sprites[i];
                    Console.WriteLine($"One new sprite ({s.X}, {s.Y}, bmp({s.Bitmap.Width}, {s.Bitmap.Height}))");
                }
            }
        }

        public class SpriteData
        {
            public int X;
            public int Y;
            public int OX;
            public int OY;
            public BitmapData Bitmap;
            public RectData SrcRect;
            public double ZoomX;
            public double ZoomY;
            public byte Opacity;
            public int Angle;
            public bool MirrorX;
            public bool MirrorY;
            public ColorData Color;
            public ToneData Tone;
            public bool Disposed;
            public bool Visible;
            public ViewportData Viewport;
            public int Z;

            public SpriteData(ViewportData Viewport, Sprite Sprite)
            {
                this.X = Sprite.X;
                this.Y = Sprite.Y;
                this.OX = Sprite.OX;
                this.OY = Sprite.OY;
                this.ZoomX = Sprite.ZoomX;
                this.ZoomY = Sprite.ZoomY;
                this.Opacity = Sprite.Opacity;
                this.Angle = Sprite.Angle;
                this.MirrorX = Sprite.MirrorX;
                this.MirrorY = Sprite.MirrorY;
                this.Bitmap = new BitmapData(Sprite.Bitmap);
                this.SrcRect = new RectData(Sprite.SrcRect);
                this.Color = new ColorData(Sprite.Color);
                this.Tone = new ToneData(Sprite.Tone);
                this.Disposed = Sprite.Disposed;
                this.Visible = Sprite.Visible;
                this.Viewport = Viewport;
                this.Z = Sprite.Z;
            }

            public void CompareWith(SpriteData Sprite)
            {
                string Diff = "Sprite properties changed: ";
                if (this.X != Sprite.X) Diff += $"(X {this.X}->{Sprite.X}) ";
                if (this.Y != Sprite.Y) Diff += $"(Y {this.Y}->{Sprite.Y}) ";
                if (this.OX != Sprite.OX) Diff += $"(OX {this.OX}->{Sprite.OX}) ";
                if (this.OY != Sprite.OY) Diff += $"(OY {this.OY}->{Sprite.OY}) ";
                if (this.Visible != Sprite.Visible) Diff += $"(Visible {this.Visible}->{Sprite.Visible}) ";
                if (this.Disposed != Sprite.Disposed) Diff += $"(Disposed {this.Disposed}->{Sprite.Disposed}) ";
                if (this.Z != Sprite.Z) Diff += $"(Z {this.Z}->{Sprite.Z}) ";
                if (this.Opacity != Sprite.Opacity) Diff += $"(Opacity {this.Opacity}->{Sprite.Opacity}) ";
                Diff += this.Color.CompareWith(Sprite.Color);
                Diff += this.Tone.CompareWith(Sprite.Tone);
                if (this.ZoomX != Sprite.ZoomX) Diff += $"(ZoomX {this.ZoomX}->{Sprite.ZoomX}) ";
                if (this.ZoomY != Sprite.ZoomY) Diff += $"(ZoomY {this.ZoomY}->{Sprite.ZoomY}) ";
                if (this.Angle != Sprite.Angle) Diff += $"(Angle {this.Angle}->{Sprite.Angle}) ";
                if (this.MirrorX != Sprite.MirrorX) Diff += $"(MirrorX {this.MirrorX}->{Sprite.MirrorX}) ";
                if (this.MirrorY != Sprite.MirrorY) Diff += $"(MirrorY {this.MirrorY}->{Sprite.MirrorY}) ";
                Diff += this.SrcRect.CompareWith(Sprite.SrcRect);
                Diff += this.Bitmap.CompareWith(Sprite.Bitmap);
                if (Diff != "Sprite properties changed: ") Console.WriteLine(Diff.Substring(0, Diff.Length - 1) + ")");
            }
        }

        public class BitmapData
        {
            public bool Null = false;
            public int Width;
            public int Height;
            public ColorData Color;

            public BitmapData(Bitmap Bitmap)
            {
                if (Bitmap == null) Null = true;
                else
                {
                    this.Width = Bitmap.Width;
                    this.Height = Bitmap.Height;
                    if (Bitmap is SolidBitmap) this.Color = new ColorData(((SolidBitmap) Bitmap).Color);
                }
            }

            public string CompareWith(BitmapData Bitmap)
            {
                string Diff = "(Bitmap ";
                if (this.Null && !Bitmap.Null) Diff += $"null -> ({Bitmap.Width},{Bitmap.Height}) ";
                else if (!this.Null && Bitmap.Null) Diff += $"({Bitmap.Width},{Bitmap.Height}) -> null";
                else if (!this.Null && !Bitmap.Null)
                {
                    if (this.Width != Bitmap.Width) Diff += $"(Width {this.Width}->{Bitmap.Width}) ";
                    if (this.Height != Bitmap.Height) Diff += $"(Height {this.Height}->{Bitmap.Height}) ";
                    if (this.Color != null && Bitmap.Color == null) Diff += "Solid->Normal ";
                    else if (this.Color == null && Bitmap.Color != null) Diff += "Normal->Solid ";
                    else if (this.Color != null && Bitmap.Color != null) Diff += this.Color.CompareWith(Bitmap.Color);
                }
                return Diff == "(Bitmap " ? "" : Diff.Substring(0, Diff.Length - 1) + ")";
            }
        }

        public class RectData
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;

            public RectData(Rect Rect)
            {
                this.X = Rect.X;
                this.Y = Rect.Y;
                this.Width = Rect.Width;
                this.Height = Rect.Height;
            }

            public string CompareWith(RectData Rect)
            {
                string Diff = "(Rect ";
                if (this.X != Rect.X) Diff += $"(X {this.X}->{Rect.X}) ";
                if (this.Y != Rect.Y) Diff += $"(Y {this.Y}->{Rect.Y}) ";
                if (this.Width != Rect.Width) Diff += $"(Width {this.Width}->{Rect.Width}) ";
                if (this.Height != Rect.Height) Diff += $"(Height {this.Height}->{Rect.Height}) ";
                return Diff == "(Rect " ? "" : Diff.Substring(0, Diff.Length - 1) + ")";
            }
        }

        public class ColorData
        {
            public byte Red;
            public byte Green;
            public byte Blue;
            public byte Alpha;

            public ColorData(Color c)
            {
                this.Red = c.Red;
                this.Green = c.Green;
                this.Blue = c.Blue;
                this.Alpha = c.Alpha;
            }

            public string CompareWith(ColorData Color)
            {
                string Diff = "(Color ";
                if (this.Red != Color.Red) Diff += $"(Red {this.Red}->{Color.Red}) ";
                if (this.Green != Color.Green) Diff += $"(Green {this.Green}->{Color.Green}) ";
                if (this.Blue != Color.Blue) Diff += $"(Blue {this.Blue}->{Color.Blue}) ";
                if (this.Alpha != Color.Alpha) Diff += $"(Alpha {this.Alpha}->{Color.Alpha}) ";
                return Diff == "(Color " ? "" : Diff.Substring(0, Diff.Length - 1) + ")";
            }
        }

        public class ToneData
        {
            public sbyte Red;
            public sbyte Green;
            public sbyte Blue;
            public byte Grey;

            public ToneData(Tone t)
            {
                this.Red = t.Red;
                this.Green = t.Green;
                this.Blue = t.Blue;
                this.Grey = t.Gray;
            }

            public string CompareWith(ToneData Tone)
            {
                string Diff = "(Tone ";
                if (this.Red != Tone.Red) Diff += $"(Red {this.Red}->{Tone.Red}) ";
                if (this.Green != Tone.Green) Diff += $"(Green {this.Green}->{Tone.Green}) ";
                if (this.Blue != Tone.Blue) Diff += $"(Blue {this.Blue}->{Tone.Blue}) ";
                if (this.Grey != Tone.Grey) Diff += $"(Grey {this.Grey}->{Tone.Grey}) ";
                return Diff == "(Tone " ? "" : Diff.Substring(0, Diff.Length - 1) + ")";
            }
        }
    }
}
