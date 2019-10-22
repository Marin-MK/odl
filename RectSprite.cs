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
        private int _X = 0;
        public new int X { get { return _X; } set { _X = value; UpdateSprites(); } }
        private int _Y = 0;
        public new int Y { get { return _Y; } set { _Y = value; UpdateSprites(); } }
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
            this.SetInnerColor(InnerColor);
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
            : this(viewport)
        {
            this.SetOuterColor(OuterColor);
            this.FillInside = false;
            this.SetSize(s, Thickness);
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
            this.Thickness = Thickness;

            TopBmp.SetSize(this.Size.Width, Thickness);
            LeftBmp.SetSize(Thickness, this.Size.Height - 2 * Thickness + 1);
            RightBmp.SetSize(Thickness, this.Size.Height - 2 * Thickness + 1);
            BottomBmp.SetSize(this.Size.Width, Thickness);
            CenterBmp.SetSize(this.Size.Width - 2 * Thickness, this.Size.Height - 2 * Thickness);

            this.UpdateSprites();
        }

        public void UpdateSprites()
        {
            SpriteList["top"].X = this.X;
            SpriteList["top"].Y = this.Y;
            SpriteList["left"].X = this.X;
            SpriteList["left"].Y = this.Y + Thickness;
            SpriteList["right"].X = this.X + this.Size.Width - Thickness;
            SpriteList["right"].Y = this.Y + Thickness;
            SpriteList["bottom"].X = this.X;
            SpriteList["bottom"].Y = this.Y + this.Size.Height - Thickness;
            SpriteList["center"].X = this.X + Thickness;
            SpriteList["center"].Y = this.Y + Thickness;
        }

        public void SetOuterColor(byte R, byte G, byte B, byte A = 255)
        {
            this.SetOuterColor(new Color(R, G, B, A));
        }
        public void SetOuterColor(Color c)
        {
            this.OuterColor = c;
            TopBmp.SetColor(c);
            LeftBmp.SetColor(c);
            RightBmp.SetColor(c);
            BottomBmp.SetColor(c);
        }

        public void SetInnerColor(byte R, byte G, byte B, byte A = 255)
        {
            this.SetInnerColor(new Color(R, G, B, A));
        }
        public void SetInnerColor(Color c)
        {
            this.FillInside = true;
            this.InnerColor = c;
            CenterBmp.SetColor(c);
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