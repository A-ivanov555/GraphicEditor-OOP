using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GraphicEditor.Tools;

namespace GraphicEditor.Services;


public sealed class ToolManager
{
    private readonly Dictionary<DrawingToolKind, DrawingTool> _tools;
    private DrawingTool _activeTool;

    public bool IsDrawing { get; private set; }

    public ToolManager(Canvas canvas)
    {
        _tools = new Dictionary<DrawingToolKind, DrawingTool>
        {
            [DrawingToolKind.Pencil] = new PencilTool(canvas),
            [DrawingToolKind.Line] = new LineTool(canvas),
            [DrawingToolKind.Rectangle] = new RectangleTool(canvas),
            [DrawingToolKind.Ellipse] = new EllipseTool(canvas)
        };

        _activeTool = _tools[DrawingToolKind.Pencil];
    }


    public void SelectTool(DrawingToolKind kind)
    {
        if (!IsDrawing)
            _activeTool = _tools[kind];
    }

    public void StartDrawing(Point position, Color color, double thickness)
    {
        if (IsDrawing)
            return;

        _activeTool.OnMouseDown(position, color, thickness);
        IsDrawing = true;
    }

    public void MoveDrawing(Point position)
    {
        if (IsDrawing)
            _activeTool.OnMouseMove(position);
    }

    public void FinishDrawing(Point position)
    {
        if (!IsDrawing)
            return;

        try
        {
            _activeTool.OnMouseUp(position);
        }
        finally
        {
            IsDrawing = false;
        }
    }
}
