using System;
using System.Text;
using static odl.SDL2.SDL_ttf;

namespace odl;

public partial class Bitmap
{
    /// <summary>
    /// Returns the size the given character would take up when rendered.
    /// </summary>
    /// <param name="Char">The character to find the size of.</param>
    /// <param name="DrawOptions">Additional options for drawing the character.</param>
    public virtual Size TextSize(char Char, DrawOptions DrawOptions = 0)
    {
        return Font.TextSize(Char, DrawOptions);
    }

    /// <summary>
    /// Returns the size the given string would take up when rendered.
    /// </summary>
    /// <param name="Text">The string to find the size of.</param>
    /// <param name="DrawOptions">Additional options for drawing the string.</param>
    public virtual Size TextSize(string Text, DrawOptions DrawOptions = 0)
    {
        return Font.TextSize(Text, DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="R">The Red component of the color.</param>
    /// <param name="G">The Green component of the color.</param>
    /// <param name="B">The Blue component of the color.</param>
    /// <param name="A">The Alpha component of the color.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, int X, int Y, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawText(Text, X, Y, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="c">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, Point p, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawText(Text, p.X, p.Y, c, DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="R">The Red component of the color.</param>
    /// <param name="G">The Green component of the color.</param>
    /// <param name="B">The Blue component of the color.</param>
    /// <param name="A">The Alpha component of the color.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, Point p, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawText(Text, p.X, p.Y, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a string of text at (0, 0).
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="c">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawText(Text, 0, 0, c, DrawOptions);
    }

    /// <summary>
    /// Draws a string of text at (0, 0).
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="R">The Red component of the color.</param>
    /// <param name="G">The Green component of the color.</param>
    /// <param name="B">The Blue component of the color.</param>
    /// <param name="A">The Alpha component of the color.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawText(Text, 0, 0, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="c">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public virtual void DrawText(string Text, int X, int Y, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        if (Text.Length == 1)
        {
            DrawGlyph(Convert.ToChar(Text), X, Y, c, DrawOptions);
            return;
        }
        if (Locked) throw new BitmapLockedException();
        if (this.Font == null)
        {
            throw new Exception("No Font specified for this Bitmap.");
        }
        Text = Text.Replace("\r", "").Replace("\n", "");
        if (string.IsNullOrEmpty(Text)) return;
        IntPtr SDL_Font = this.Font.SDL_Font;
        bool aliased = (DrawOptions & DrawOptions.Aliased) == DrawOptions.Aliased;
        bool leftalign = (DrawOptions & DrawOptions.LeftAlign) == DrawOptions.LeftAlign;
        bool centeralign = (DrawOptions & DrawOptions.CenterAlign) == DrawOptions.CenterAlign;
        bool rightalign = (DrawOptions & DrawOptions.RightAlign) == DrawOptions.RightAlign;
        if (leftalign && centeralign || leftalign && rightalign || centeralign && rightalign)
        {
            throw new Exception("Multiple alignments specified in DrawText DrawOptions - can only contain one alignment setting");
        }
        if (!leftalign && !centeralign && !rightalign) leftalign = true;
        TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
        Bitmap TextBitmap;
        Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(Text));
        if (aliased) TextBitmap = new Bitmap(TTF_RenderUTF8_Solid(SDL_Font, Text, c.SDL_Color));
        else TextBitmap = new Bitmap(TTF_RenderUTF8_Blended(SDL_Font, Text, c.SDL_Color));
        if (centeralign) X -= TextBitmap.Width / 2;
        if (rightalign) X -= TextBitmap.Width;
        this.Build(new Rect(X, Y, TextBitmap.Width, TextBitmap.Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
        TextBitmap.Dispose();
        this.BlendMode = BlendMode.Addition;
        if (this.Renderer != null) this.Renderer.Update();
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="r">The rectangle in which to draw the text.</param>
    /// <param name="c">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, Rect r, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        DrawText(Text, r.X, r.Y, r.Width, r.Height, c, DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="r">The rectangle in which to draw the text.</param>
    /// <param name="R">The Red component of the color.</param>
    /// <param name="G">The Green component of the color.</param>
    /// <param name="B">The Blue component of the color.</param>
    /// <param name="A">The Alpha component of the color.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, Rect r, byte R, byte G, byte B, byte A, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        DrawText(Text, r.X, r.Y, r.Width, r.Height, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="p">The position at which to draw the text.</param>
    /// <param name="s">The size within which to draw the text.</param>
    /// <param name="c">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, Point p, Size s, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        DrawText(Text, p.X, p.Y, s.Width, s.Height, c, DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="p">The position at which to draw the text.</param>
    /// <param name="s">The size within which to draw the text.</param>
    /// <param name="R">The Red component of the color.</param>
    /// <param name="G">The Green component of the color.</param>
    /// <param name="B">The Blue component of the color.</param>
    /// <param name="A">The Alpha component of the color.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, Point p, Size s, byte R, byte G, byte B, byte A, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        DrawText(Text, p.X, p.Y, s.Width, s.Height, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="X">The X position at which to draw the text.</param>
    /// <param name="Y">The Y position at which to draw the text.</param>
    /// <param name="s">The size within which to draw the text.</param>
    /// <param name="c">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, int X, int Y, Size s, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        DrawText(Text, X, Y, s.Width, s.Height, c, DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="X">The X position at which to draw the text.</param>
    /// <param name="Y">The Y position at which to draw the text.</param>
    /// <param name="s">The size within which to draw the text.</param>
    /// <param name="R">The Red component of the color.</param>
    /// <param name="G">The Green component of the color.</param>
    /// <param name="B">The Blue component of the color.</param>
    /// <param name="A">The Alpha component of the color.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, int X, int Y, Size s, byte R, byte G, byte B, byte A, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        DrawText(Text, X, Y, s.Width, s.Height, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="p">The position at which to draw the text.</param>
    /// <param name="Width">The width within which to draw the text.</param>
    /// <param name="Height">The height within which to draw the text.</param>
    /// <param name="c">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, Point p, int Width, int Height, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        DrawText(Text, p.X, p.Y, Width, Height, c, DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="p">The position at which to draw the text.</param>
    /// <param name="Width">The width within which to draw the text.</param>
    /// <param name="Height">The height within which to draw the text.</param>
    /// <param name="R">The Red component of the color.</param>
    /// <param name="G">The Green component of the color.</param>
    /// <param name="B">The Blue component of the color.</param>
    /// <param name="A">The Alpha component of the color.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawText(string Text, Point p, int Width, int Height, byte R, byte G, byte B, byte A, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        DrawText(Text, p.X, p.Y, Width, Height, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a string of text.
    /// </summary>
    /// <param name="Text">The text to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="Width">The width within which to draw the text.</param>
    /// <param name="Height">The height within which to draw the text.</param>
    /// <param name="c">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public virtual void DrawText(string Text, int X, int Y, int Width, int Height, Color c, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        if (Text.Length == 1)
        {
            DrawGlyph(Convert.ToChar(Text), X, Y, Width, Height, c, DrawOptions);
            return;
        }
        if (Locked) throw new BitmapLockedException();
        if (this.Font == null)
        {
            throw new Exception("No Font specified for this Bitmap.");
        }
        Text = Text.Replace("\r", "").Replace("\n", "");
        if (string.IsNullOrEmpty(Text)) return;
        IntPtr SDL_Font = this.Font.SDL_Font;
        bool aliased = (DrawOptions & DrawOptions.Aliased) == DrawOptions.Aliased;
        bool leftalign = (DrawOptions & DrawOptions.LeftAlign) == DrawOptions.LeftAlign;
        bool centeralign = (DrawOptions & DrawOptions.CenterAlign) == DrawOptions.CenterAlign;
        bool rightalign = (DrawOptions & DrawOptions.RightAlign) == DrawOptions.RightAlign;
        if (leftalign && centeralign || leftalign && rightalign || centeralign && rightalign)
        {
            throw new Exception("Multiple alignments specified in DrawText DrawOptions - can only contain one alignment setting");
        }
        if (!leftalign && !centeralign && !rightalign) leftalign = true;
        TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
        Bitmap TextBitmap;
        if (aliased) TextBitmap = new Bitmap(TTF_RenderText_Solid(SDL_Font, Text, c.SDL_Color));
        else TextBitmap = new Bitmap(TTF_RenderText_Blended(SDL_Font, Text, c.SDL_Color));
        if (centeralign) X -= TextBitmap.Width / 2;
        if (rightalign) X -= TextBitmap.Width;
        this.Build(new Rect(X, Y, Width, Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
        TextBitmap.Dispose();
        this.BlendMode = BlendMode.Addition;
        if (this.Renderer != null) this.Renderer.Update();
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, int X, int Y, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, X, Y, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, color, DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a character at (0, 0).
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, 0, 0, color, DrawOptions);
    }

    /// <summary>
    /// Draws a character at (0, 0).
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, 0, 0, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public virtual void DrawGlyph(char c, int X, int Y, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        if (Locked) throw new BitmapLockedException();
        if (this.Font == null)
        {
            throw new Exception("No Font specified for this Bitmap.");
        }
        if (c == '\x00') return;
        IntPtr SDL_Font = this.Font.SDL_Font;
        bool aliased = (DrawOptions & DrawOptions.Aliased) == DrawOptions.Aliased;
        bool leftalign = (DrawOptions & DrawOptions.LeftAlign) == DrawOptions.LeftAlign;
        bool centeralign = (DrawOptions & DrawOptions.CenterAlign) == DrawOptions.CenterAlign;
        bool rightalign = (DrawOptions & DrawOptions.RightAlign) == DrawOptions.RightAlign;
        if (leftalign && centeralign || leftalign && rightalign || centeralign && rightalign)
        {
            throw new Exception("Multiple alignments specified in DrawGlyph DrawOptions - can only contain one alignment setting");
        }
        if (!leftalign && !centeralign && !rightalign) leftalign = true;
        TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
        Bitmap TextBitmap;
        if (aliased) TextBitmap = new Bitmap(TTF_RenderGlyph_Solid(SDL_Font, c, color.SDL_Color));
        else TextBitmap = new Bitmap(TTF_RenderGlyph_Blended(SDL_Font, c, color.SDL_Color));
        if (centeralign) X -= TextBitmap.Width / 2;
        if (rightalign) X -= TextBitmap.Width;
        this.Build(new Rect(X, Y, TextBitmap.Width, TextBitmap.Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
        TextBitmap.Dispose();
        this.BlendMode = BlendMode.Addition;
        if (this.Renderer != null) this.Renderer.Update();
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="rect">The rectangle within which to draw the text.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Rect rect, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, rect.X, rect.Y, rect.Width, rect.Height, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="s">The size of the rectangle within which to draw the text.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, Size s, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, s.Width, s.Height, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="Width">The width within which to draw the text.</param>
    /// <param name="Height">The height within which to draw the text.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, int Width, int Height, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, Width, Height, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="s">The size of the rectangle within which to draw the text.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, int X, int Y, Size s, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, X, Y, s.Width, s.Height, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="Width">The width within which to draw the text.</param>
    /// <param name="Height">The height within which to draw the text.</param>
    /// <param name="R">The Red component of the color of the text to draw.</param>
    /// <param name="G">The Green component of the color of the text to draw.</param>
    /// <param name="B">The Blue component of the color of the text to draw.</param>
    /// <param name="A">The Alpha component of the color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, int X, int Y, int Width, int Height, byte R, byte G, byte B, byte A = 255, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, X, Y, Width, Height, new Color(R, G, B, A), DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="rect">The rectangle within which to draw the text.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Rect rect, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, rect.X, rect.Y, rect.Width, rect.Height, color, DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="s">The size of the rectangle within which to draw the text.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, Size s, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, s.Width, s.Height, color, DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="p">The position to draw the text at.</param>
    /// <param name="Width">The width within which to draw the text.</param>
    /// <param name="Height">The height within which to draw the text.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, Point p, int Width, int Height, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, p.X, p.Y, Width, Height, color, DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="s">The size of the rectangle within which to draw the text.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public void DrawGlyph(char c, int X, int Y, Size s, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        this.DrawGlyph(c, X, Y, s.Width, s.Height, color, DrawOptions);
    }

    /// <summary>
    /// Draws a character.
    /// </summary>
    /// <param name="c">The character to draw.</param>
    /// <param name="X">The X position to draw the text at.</param>
    /// <param name="Y">The Y position to draw the text at.</param>
    /// <param name="Width">The width within which to draw the text.</param>
    /// <param name="Height">The height within which to draw the text.</param>
    /// <param name="color">The color of the text to draw.</param>
    /// <param name="DrawOptions">Additional options for drawing the text.</param>
    public virtual void DrawGlyph(char c, int X, int Y, int Width, int Height, Color color, DrawOptions DrawOptions = DrawOptions.LeftAlign)
    {
        if (Locked) throw new BitmapLockedException();
        if (this.Font == null)
        {
            throw new Exception("No Font specified for this Bitmap.");
        }
        if (c == '\x00') return;
        IntPtr SDL_Font = this.Font.SDL_Font;
        bool aliased = (DrawOptions & DrawOptions.Aliased) == DrawOptions.Aliased;
        bool leftalign = (DrawOptions & DrawOptions.LeftAlign) == DrawOptions.LeftAlign;
        bool centeralign = (DrawOptions & DrawOptions.CenterAlign) == DrawOptions.CenterAlign;
        bool rightalign = (DrawOptions & DrawOptions.RightAlign) == DrawOptions.RightAlign;
        if (leftalign && centeralign || leftalign && rightalign || centeralign && rightalign)
        {
            throw new Exception("Multiple alignments specified in DrawGlyph DrawOptions - can only contain one alignment setting");
        }
        if (!leftalign && !centeralign && !rightalign) leftalign = true;
        TTF_SetFontStyle(SDL_Font, Convert.ToInt32(DrawOptions));
        Bitmap TextBitmap;
        if (aliased) TextBitmap = new Bitmap(TTF_RenderGlyph_Solid(SDL_Font, c, color.SDL_Color));
        else TextBitmap = new Bitmap(TTF_RenderGlyph_Blended(SDL_Font, c, color.SDL_Color));
        if (centeralign) X -= TextBitmap.Width / 2;
        if (rightalign) X -= TextBitmap.Width;
        this.Build(new Rect(X, Y, Width, Height), TextBitmap, new Rect(0, 0, TextBitmap.Width, TextBitmap.Height));
        TextBitmap.Dispose();
        this.BlendMode = BlendMode.Addition;
        if (this.Renderer != null) this.Renderer.Update();
    }
}

