using System;
using static SDL2.SDL;

namespace ODL
{
    public interface IBitmap
    {
        IntPtr Surface { get; }
        SDL_Surface SurfaceObject { get; }
        IntPtr Texture { get; }
        int Width { get; }
        int Height { get; }
        bool Disposed { get; }
        Renderer Renderer { get; set; }
        Font Font { get; set; }
        bool Locked { get; }

        void Clear();
        void Dispose();

        #region SetPixel Overloads
        void SetPixel(Point p, Color c);
        void SetPixel(int X, int Y, Color c);
        void SetPixel(Point p, byte r, byte g, byte b, byte a = 255);
        #endregion
        void SetPixel(int X, int Y, byte r, byte g, byte b, byte a = 255, bool subcall = false);

        #region GetPixel Overloads
        Color GetPixel(Point p);
        #endregion
        Color GetPixel(int X, int Y);

        #region DrawLine Overloads
        void DrawLine(Point p1, Point p2, Color c);
        void DrawLine(Point p1, Point p2, byte r, byte g, byte b, byte a = 255);
        void DrawLine(Point p1, int x2, int y2, Color c);
        void DrawLine(Point p1, int x2, int y2, byte r, byte g, byte b, byte a = 255);
        void DrawLine(int x1, int y1, Point p2, Color c);
        void DrawLine(int x1, int y1, Point p2, byte r, byte g, byte b, byte a = 255);
        void DrawLine(int x1, int y1, int x2, int y2, Color c);
        #endregion
        void DrawLine(int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a = 255);

        #region DrawLines Overloads
        void DrawLines(Color c, params Point[] points);
        void DrawLines(byte r, byte g, byte b, params Point[] points);
        #endregion
        void DrawLines(byte r, byte g, byte b, byte a, params Point[] points);

        #region DrawCircle
        void DrawCircle(Point c, int Radius, Color color);
        void DrawCircle(int ox, int oy, int Radius, Color c);
        void DrawCircle(Point c, int Radius, byte r, byte g, byte b, byte a = 255);
        #endregion
        void DrawCircle(int ox, int oy, int Radius, byte r, byte g, byte b, byte a = 255);

        #region FillCircle
        void FillCircle(Point c, int Radius, Color color);
        void FillCircle(int ox, int oy, int Radius, Color c);
        void FillCircle(Point c, int Radius, byte r, byte g, byte b, byte a = 255);
        #endregion
        void FillCircle(int ox, int oy, int Radius, byte r, byte g, byte b, byte a = 255);

        #region DrawQuadrant Overloads
        void DrawQuadrant(Point c, int Radius, Location l,Color color);
        void DrawQuadrant(int ox, int oy, int Radius, Location l,Color c);
        void DrawQuadrant(Point c, int Radius, Location l,byte r, byte g, byte b, byte a = 255);
        #endregion
        void DrawQuadrant(int ox, int oy, int Radius, Location l,byte r, byte g, byte b, byte a = 255);

        #region FillQuadrant Overloads
        void FillQuadrant(Point c, int Radius, Location l,Color color);
        void FillQuadrant(int ox, int oy, int Radius, Location l,Color c);
        void FillQuadrant(Point c, int Radius, Location l,byte r, byte g, byte b, byte a = 255);
        #endregion
        void FillQuadrant(int ox, int oy, int Radius, Location l,byte r, byte g, byte b, byte a = 255);

        #region DrawRect Overloads
        void DrawRect(Rect r, Color c);
        void DrawRect(Rect rect, byte r, byte g, byte b, byte a = 255);
        void DrawRect(Point Point, Size Size, Color c);
        void DrawRect(Point Point, Size Size, byte r, byte g, byte b, byte a = 255);
        void DrawRect(Point Point, int Width, int Height, Color c);
        void DrawRect(Point Point, int Width, int Height, byte r, byte g, byte b, byte a = 255);
        void DrawRect(Size size, byte r, byte g, byte b, byte a = 255);
        void DrawRect(Size size, Color c);
        void DrawRect(int Width, int Height, byte r, byte g, byte b, byte a = 255);
        void DrawRect(int Width, int Height, Color c);
        void DrawRect(int X, int Y, Size Size, Color c);
        void DrawRect(int X, int Y, Size Size, byte r, byte g, byte b, byte a = 255);
        void DrawRect(int X, int Y, int Width, int Height, Color c);
        #endregion
        void DrawRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255);

        #region FillRect Overloads
        void FillRect(Rect r, Color c);
        void FillRect(Rect rect, byte r, byte g, byte b, byte a = 255);
        void FillRect(Point Point, Size Size, Color c);
        void FillRect(Point Point, Size Size, byte r, byte g, byte b, byte a = 255);
        void FillRect(Point Point, int Width, int Height, Color c);
        void FillRect(Point Point, int Width, int Height, byte r, byte g, byte b, byte a = 255);
        void FillRect(Size size, byte r, byte g, byte b, byte a = 255);
        void FillRect(Size size, Color c);
        void FillRect(int Width, int Height, byte r, byte g, byte b, byte a = 255);
        void FillRect(int Width, int Height, Color c);
        void FillRect(int X, int Y, Size Size, Color c);
        void FillRect(int X, int Y, Size Size, byte r, byte g, byte b, byte a = 255);
        void FillRect(int X, int Y, int Width, int Height, Color c);
        #endregion
        void FillRect(int X, int Y, int Width, int Height, byte r, byte g, byte b, byte a = 255);

        #region Build Overloads
        void Build(Rect DestRect, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight);
        void Build(Rect DestRect, Bitmap SrcBitmap);
        void Build(Point DP, Size DS, Bitmap SrcBitmap, Rect SrcRect);
        void Build(Point DP, Size DS, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight);
        void Build(Point DP, int DWidth, int DHeight, Bitmap SrcBitmap, Rect SrcRect);
        void Build(Point DP, int DWidth, int DHeight, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight);
        void Build(int DX, int DY, Size DS, Bitmap SrcBitmap, Rect SrcRect);
        void Build(int DX, int DY, Size DS, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight);
        void Build(int DX, int DY, int DWidth, int DHeight, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight);
        void Build(int DX, int DY, int DWidth, int DHeight, Bitmap SrcBitmap, Rect SrcRect);
        void Build(Point DP, Bitmap SrcBitmap, Rect SrcRect);
        void Build(Point DP, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight);
        void Build(int DX, int DY, Bitmap SrcBitmap, Rect SrcRect);
        void Build(int DX, int DY, Bitmap SrcBitmap, int SX, int SY, int SWidth, int SHeight);
        void Build(Point DP, Bitmap SrcBitmap);
        void Build(int DX, int DY, Bitmap SrcBitmap);
        void Build(Bitmap SrcBitmap);
        #endregion
        void Build(Rect DestRect, Bitmap SrcBitmap, Rect SrcRect);

        Size TextSize(string Text, DrawOptions DrawOptions = 0);

        #region DrawText Overloads
        void DrawText(string Text, int X, int Y, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign);
        void DrawText(string Text, Point p, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign);
        void DrawText(string Text, Point p, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign);
        void DrawText(string Text, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign);
        void DrawText(string Text, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign);
        #endregion
        void DrawText(string Text, int X, int Y, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign);

        #region DrawGlyph Overloads
        void DrawGlyph(char c, int X, int Y, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign);
        void DrawGlyph(char c, Point p, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign);
        void DrawGlyph(char c, Point p, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign);
        void DrawGlyph(char c, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign);
        void DrawGlyph(char c, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign);
        #endregion
        void DrawGlyph(char c, int X, int Y, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign);


        void Lock();

        void Unlock();

        void RecreateTexture();
    }
}
