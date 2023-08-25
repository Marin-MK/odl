namespace odl;

public class Vertex
{
    public Point Point { get; set; }
    public Color Color { get; set; }

    public int X { get { return Point.X; } set { Point.X = value; } }
    public int Y { get { return Point.Y; } set { Point.Y = value; } }

    public byte R { get { return Color.Red; } set { Color.Red = value; } }
    public byte G { get { return Color.Green; } set { Color.Green = value; } }
    public byte B { get { return Color.Blue; } set { Color.Blue = value; } }
    public byte A { get { return Color.Alpha; } set { Color.Alpha = value; } }

    public byte Red { get { return Color.Red; } set { Color.Red = value; } }
    public byte Green { get { return Color.Green; } set { Color.Green = value; } }
    public byte Blue { get { return Color.Blue; } set { Color.Blue = value; } }
    public byte Alpha { get { return Color.Alpha; } set { Color.Alpha = value; } }

    public Vertex(Point Point, Color Color) : this(Point.X, Point.Y, Color.Red, Color.Green, Color.Blue, Color.Alpha) { }
    public Vertex(int X, int Y, Color Color) : this(X, Y, Color.Red, Color.Green, Color.Blue, Color.Alpha) { }
    public Vertex(Point Point, byte R, byte G, byte B, byte A = 255) : this(Point.X, Point.Y, R, G, B, A) { }
    public Vertex(Color Color, Point Point) : this(Point.X, Point.Y, Color.Red, Color.Green, Color.Blue, Color.Alpha) { }
    public Vertex(byte R, byte G, byte B, byte A, Point Point) : this(Point.X, Point.Y, R, G, B, A) { }
    public Vertex(Color Color, int X, int Y) : this(X, Y, Color.Red, Color.Green, Color.Blue, Color.Alpha) { }
    public Vertex(byte R, byte G, byte B, byte A, int X, int Y) : this(X, Y, R, G, B, A) { }
    public Vertex(int X, int Y, byte R, byte G, byte B, byte A = 255)
    {
        this.Point = new Point(X, Y);
        this.Color = new Color(R, G, B, A);
    }
}
