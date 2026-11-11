using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphicEditor.Tools;

public sealed class LineTool : DrawingTool
{
    private Line? _currentLine;
    private Point _startPoint;

    public LineTool(Canvas canvas) : base(canvas)
    {
    }

    public override void OnMouseDown(Point position, Color color, double thickness)
    {
        _startPoint = position;
        _currentLine = new Line
        {
            X1 = position.X,
            Y1 = position.Y,
            X2 = position.X,
            Y2 = position.Y,
            Stroke = CreateBrush(color),
            StrokeThickness = thickness,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round
        };

        DrawingSurface.Children.Add(_currentLine);
    }

    public override void OnMouseMove(Point position)
    {
        if (_currentLine is null)
            return;

        _currentLine.X2 = position.X;
        _currentLine.Y2 = position.Y;
    }

    public override void OnMouseUp(Point position)
    {
        if (_currentLine is null)
            return;

        OnMouseMove(position);

        if ((position - _startPoint).Length < 1)
            DrawingSurface.Children.Remove(_currentLine);

        _currentLine = null;
    }
}
