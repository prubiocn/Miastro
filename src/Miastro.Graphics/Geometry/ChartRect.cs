namespace Miastro.Graphics.Geometry;

public readonly record struct ChartRect(
    double X,
    double Y,
    double Width,
    double Height)
{
    public double Left => X;

    public double Top => Y;

    public double Right => X + Width;

    public double Bottom => Y + Height;

    public ChartPoint Center =>
        new(
            X + Width / 2.0,
            Y + Height / 2.0);

    public bool Intersects(
        ChartRect other)
        =>
            Left < other.Right
            && Right > other.Left
            && Top < other.Bottom
            && Bottom > other.Top;

    public bool Contains(
        ChartPoint point)
        =>
            point.X >= Left
            && point.X <= Right
            && point.Y >= Top
            && point.Y <= Bottom;
}
