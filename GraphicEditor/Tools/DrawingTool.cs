using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Tools;


public abstract class DrawingTool
{
    protected Canvas DrawingSurface { get; }

    protected DrawingTool(Canvas canvas)
    {
        DrawingSurface = canvas ?? throw new ArgumentNullException(nameof(canvas));
    }

  
    public abstract void OnMouseDown(Point position, Color color, double thickness);

    public abstract void OnMouseMove(Point position);


    public abstract void OnMouseUp(Point position);

    protected static SolidColorBrush CreateBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }

    protected static Rect GetBounds(Point start, Point end)
    {
        double left = Math.Min(start.X, end.X);
        double top = Math.Min(start.Y, end.Y);
        double width = Math.Abs(end.X - start.X);
        double height = Math.Abs(end.Y - start.Y);

        return new Rect(left, top, width, height);
    }
}
