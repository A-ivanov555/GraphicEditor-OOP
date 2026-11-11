using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphicEditor.Tools;


public sealed class PencilTool : DrawingTool
{
    private Polyline? _currentStroke;
    private Point _startPoint;
    private Color _strokeColor;
    private double _strokeThickness;

    public PencilTool(Canvas canvas) : base(canvas)
    {
    }

    public override void OnMouseDown(Point position, Color color, double thickness)
    {
        _startPoint = position;
        _strokeColor = color;
        _strokeThickness = thickness;

        _currentStroke = new Polyline
        {
            Stroke = CreateBrush(color),
            StrokeThickness = thickness,
            StrokeLineJoin = PenLineJoin.Round,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round
        };

        _currentStroke.Points.Add(position);
        DrawingSurface.Children.Add(_currentStroke);
    }

    public override void OnMouseMove(Point position)
    {
        _currentStroke?.Points.Add(position);
    }

    public override void OnMouseUp(Point position)
    {
        if (_currentStroke is null)
            return;

        if ((position - _startPoint).Length > 0.5)
            _currentStroke.Points.Add(position);

        // Одиночный щелчок карандашом оставляет точку, а не невидимую линию.
        if (_currentStroke.Points.Count == 1)
        {
            DrawingSurface.Children.Remove(_currentStroke);

            var dot = new Ellipse
            {
                Width = _strokeThickness,
                Height = _strokeThickness,
                Fill = CreateBrush(_strokeColor)
            };

            Canvas.SetLeft(dot, _startPoint.X - _strokeThickness / 2);
            Canvas.SetTop(dot, _startPoint.Y - _strokeThickness / 2);
            DrawingSurface.Children.Add(dot);
        }

        _currentStroke = null;
    }
}
