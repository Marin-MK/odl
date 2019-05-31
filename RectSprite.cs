using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODL
{
    public class RectSprite : MultiSprite
    {
        public Size Size { get; protected set; }
        public Color OuterColor { get; protected set; }
        public bool FillInside { get; protected set; }
        public Color InnerColor { get; protected set; }
        public int Thickness { get; protected set; }
        private SolidBitmap TopBmp;
        private SolidBitmap LeftBmp;
        private SolidBitmap RightBmp;
        private SolidBitmap BottomBmp;
        private SolidBitmap CenterBmp;

        public RectSprite(Viewport viewport, int Width, int Height, Color OuterColor, Color InnerColor, int Thickness = 1)
            : this(viewport, new Size(Width, Height), OuterColor, Thickness) { }
        public RectSprite(Viewport viewport, Size s, Color OuterColor, Color InnerColor, int Thickness = 1)
            : this(viewport, s, OuterColor, Thickness)
        {
            this.FillInside = true;
            this.InnerColor = InnerColor;
        }

        #region Overloads
        public RectSprite(Viewport viewport, Size s, byte R, byte G, byte B, byte A = 255, int Thickness = 1)
            : this(viewport, s, new Color(R, G, B, A), Thickness) { }
        public RectSprite(Viewport viewport, int Width, int Height, Color OuterColor, int Thickness = 1)
            : this(viewport, new Size(Width, Height), OuterColor, Thickness) { }
        public RectSprite(Viewport viewport, int Width, int Height, byte R, byte G, byte B, byte A = 255, int Thickness = 1)
            : this(viewport, new Size(Width, Height), new Color(R, G, B, A), Thickness) { }
        #endregion
        public RectSprite(Viewport viewport, Size s, Color OuterColor, int Thickness = 1)
            : base(viewport)
        {
            this.Size = s;
            this.OuterColor = OuterColor;
            this.FillInside = false;
            this.AddBitmap("top", TopBmp = new SolidBitmap(this.Size.Width, Thickness, OuterColor));
            this.AddBitmap("left", LeftBmp = new SolidBitmap(Thickness, this.Size.Height - 2 * Thickness, OuterColor), 0, Thickness);
            this.AddBitmap("right", RightBmp = new SolidBitmap(Thickness, this.Size.Height - 2 * Thickness, OuterColor), this.Size.Width - Thickness, Thickness);
            this.AddBitmap("bottom", BottomBmp = new SolidBitmap(this.Size.Width, Thickness), 0, this.Size.Height - Thickness);
            this.AddBitmap("center", CenterBmp = new SolidBitmap(this.Size.Width - 2 * Thickness, this.Size.Height - 2 * Thickness), Thickness, Thickness);
        }

        public RectSprite(Viewport viewport)
            : base(viewport)
        {
            this.AddBitmap("top", TopBmp = new SolidBitmap(1, 1));
            this.AddBitmap("left", LeftBmp = new SolidBitmap(1, 1));
            this.AddBitmap("right", RightBmp = new SolidBitmap(1, 1));
            this.AddBitmap("bottom", BottomBmp = new SolidBitmap(1, 1));
            this.AddBitmap("center", CenterBmp = new SolidBitmap(1, 1));
        }

        public void SetSize(int Width, int Height, int Thickness = 1)
        {
            this.SetSize(new Size(Width, Height), Thickness);
        }
        public void SetSize(Size size, int Thickness = 1)
        {
            this.Size = size;
            TopBmp.Unlock();
            TopBmp.SetSize(this.Size.Width, Thickness);
            SpriteList["top"].X = this.X;
            SpriteList["top"].Y = this.Y;
            TopBmp.Lock();
            LeftBmp.Unlock();
            LeftBmp.SetSize(Thickness, this.Size.Height - 2 * Thickness);
            SpriteList["left"].X = this.X;
            SpriteList["left"].Y = this.Y + Thickness;
            LeftBmp.Lock();
            RightBmp.Unlock();
            RightBmp.SetSize(Thickness, this.Size.Height - 2 * Thickness);
            SpriteList["right"].X = this.X + this.Size.Width - Thickness;
            SpriteList["right"].Y = this.Y + Thickness;
            RightBmp.Lock();
            BottomBmp.Unlock();
            BottomBmp.SetSize(this.Size.Width, Thickness);
            SpriteList["bottom"].X = this.X;
            SpriteList["bottom"].Y = this.Y + this.Size.Height - Thickness;
            BottomBmp.Lock();
            CenterBmp.Unlock();
            CenterBmp.SetSize(this.Size.Width - 2 * Thickness, this.Size.Height - 2 * Thickness);
            SpriteList["center"].X = this.X + Thickness;
            SpriteList["center"].Y = this.Y + Thickness;
            CenterBmp.Lock();
        }

        public void SetOuterColor(byte R, byte G, byte B, byte A = 255)
        {
            this.SetOuterColor(new Color(R, G, B, A));
        }
        public void SetOuterColor(Color c)
        {
            this.OuterColor = c;
            TopBmp.Unlock();
            TopBmp.SetColor(c);
            TopBmp.Lock();
            LeftBmp.Unlock();
            LeftBmp.SetColor(c);
            LeftBmp.Lock();
            RightBmp.Unlock();
            RightBmp.SetColor(c);
            RightBmp.Lock();
            BottomBmp.Unlock();
            BottomBmp.SetColor(c);
            BottomBmp.Lock();
        }

        public void SetInnerColor(byte R, byte G, byte B, byte A = 255)
        {
            this.SetInnerColor(new Color(R, G, B, A));
        }
        public void SetInnerColor(Color c)
        {
            this.FillInside = true;
            this.InnerColor = c;
            CenterBmp.Unlock();
            CenterBmp.SetColor(c);
            CenterBmp.Lock();
        }

        public void SetColor(Color Outer, byte IR, byte IG, byte IB, byte IA = 255)
        {
            this.SetOuterColor(Outer);
            this.SetInnerColor(IR, IG, IB, IA);
        }
        public void SetColor(byte OR, byte OG, byte OB, byte OA, Color Inner)
        {
            this.SetOuterColor(OR, OG, OB, OA);
            this.SetInnerColor(Inner);
        }
        public void SetColor(byte OR, byte OG, byte OB, byte IR, byte IG, byte IB)
        {
            this.SetColor(new Color(OR, OG, OB), new Color(IR, IG, IB));
        }
        public void SetColor(Color Outer, Color Inner)
        {
            this.SetOuterColor(Outer);
            this.SetInnerColor(Inner);
        }
    }
}
