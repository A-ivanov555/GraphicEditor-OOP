using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphicEditor.Tools;


public sealed class EllipseTool : DrawingTool
{
    private Ellipse? _currentEllipse;
    private Point _startPoint;

    public EllipseTool(Canvas canvas) : base(canvas)
    {
    }

    public override void OnMouseDown(Point position, Color color, double thickness)
    {
        _startPoint = position;
        _currentEllipse = new Ellipse
        {
            Stroke = CreateBrush(color),
            StrokeThickness = thickness,
            Fill = Brushes.Transparent
        };

        DrawingSurface.Children.Add(_currentEllipse);
        UpdateBounds(position);
    }

    public override void OnMouseMove(Point position)
    {
        UpdateBounds(position);
    }

    public override void OnMouseUp(Point position)
    {
        if (_currentEllipse is null)
            return;

        UpdateBounds(position);
        if (_currentEllipse.Width < 1 || _currentEllipse.Height < 1)
            DrawingSurface.Children.Remove(_currentEllipse);

        _currentEllipse = null;
    }

    private void UpdateBounds(Point position)
    {
        if (_currentEllipse is null)
            return;

        Rect bounds = GetBounds(_startPoint, position);
        Canvas.SetLeft(_currentEllipse, bounds.Left);
        Canvas.SetTop(_currentEllipse, bounds.Top);
        _currentEllipse.Width = bounds.Width;
        _currentEllipse.Height = bounds.Height;
    }
}
