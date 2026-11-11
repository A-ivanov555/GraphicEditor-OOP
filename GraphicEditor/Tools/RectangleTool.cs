using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphicEditor.Tools;


public sealed class RectangleTool : DrawingTool
{
    private Rectangle? _currentRectangle;
    private Point _startPoint;

    public RectangleTool(Canvas canvas) : base(canvas)
    {
    }

    public override void OnMouseDown(Point position, Color color, double thickness)
    {
        _startPoint = position;
        _currentRectangle = new Rectangle
        {
            Stroke = CreateBrush(color),
            StrokeThickness = thickness,
            Fill = Brushes.Transparent
        };

        DrawingSurface.Children.Add(_currentRectangle);
        UpdateBounds(position);
    }

    public override void OnMouseMove(Point position)
    {
        UpdateBounds(position);
    }

    public override void OnMouseUp(Point position)
    {
        if (_currentRectangle is null)
            return;

        UpdateBounds(position);
        if (_currentRectangle.Width < 1 || _currentRectangle.Height < 1)
            DrawingSurface.Children.Remove(_currentRectangle);

        _currentRectangle = null;
    }

    private void UpdateBounds(Point position)
    {
        if (_currentRectangle is null)
            return;

        Rect bounds = GetBounds(_startPoint, position);
        Canvas.SetLeft(_currentRectangle, bounds.Left);
        Canvas.SetTop(_currentRectangle, bounds.Top);
        _currentRectangle.Width = bounds.Width;
        _currentRectangle.Height = bounds.Height;
    }
}
