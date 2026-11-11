using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphicEditor.Services;
using GraphicEditor.Tools;
using Microsoft.Win32;

namespace GraphicEditor;


public partial class MainWindow : Window
{
    private readonly ToolManager _toolManager;
    private readonly ImageExportService _imageExportService = new();

    private Color _selectedColor = Color.FromRgb(31, 41, 55);
    private double _strokeThickness = 3;
    private Point _lastPosition;

    public MainWindow()
    {
        InitializeComponent();
        _toolManager = new ToolManager(DrawingCanvas);
    }

    private void ToolRadio_Checked(object sender, RoutedEventArgs e)
    {
        // Первый Checked может сработать ещё во время InitializeComponent().
        if (_toolManager is null || sender is not RadioButton radioButton)
            return;

        if (Enum.TryParse(radioButton.Tag?.ToString(), out DrawingToolKind kind))
            _toolManager.SelectTool(kind);
    }

    private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox ||
            comboBox.SelectedItem is not ComboBoxItem item || item.Tag is not string hex)
            return;

        if (HexColorTextBox is not null)
            HexColorTextBox.Text = hex;

        ApplyColor(hex);
    }

    private void ApplyColorButton_Click(object sender, RoutedEventArgs e)
    {
        string hex = HexColorTextBox.Text.Trim();

        if (!Regex.IsMatch(hex, @"^#[0-9a-fA-F]{6}$"))
        {
            MessageBox.Show(this,
                "Введите цвет в формате #RRGGBB, например #2563EB.",
                "Неверный цвет", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ColorComboBox.SelectedIndex = -1;
        ApplyColor(hex);
        HexColorTextBox.Text = hex.ToUpperInvariant();
    }

    private void ApplyColor(string hex)
    {
        _selectedColor = (Color)ColorConverter.ConvertFromString(hex);

        if (ColorPreviewBorder is not null)
            ColorPreviewBorder.Background = new SolidColorBrush(_selectedColor);
    }

    private void ThicknessComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item &&
            double.TryParse(item.Tag?.ToString(), out double thickness) && thickness > 0)
        {
            _strokeThickness = thickness;
        }
    }

    private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_toolManager.IsDrawing)
            return;

        _lastPosition = GetClampedPosition(e.GetPosition(DrawingCanvas));
        _toolManager.StartDrawing(_lastPosition, _selectedColor, _strokeThickness);
        DrawingCanvas.CaptureMouse();
        StatusTextBlock.Text = "Рисование... Отпустите левую кнопку, чтобы завершить.";
        e.Handled = true;
    }

    private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        _lastPosition = GetClampedPosition(e.GetPosition(DrawingCanvas));
        PositionTextBlock.Text = $"X: {(int)_lastPosition.X}   Y: {(int)_lastPosition.Y}";

        if (!_toolManager.IsDrawing)
            return;

        if (e.LeftButton == MouseButtonState.Pressed)
            _toolManager.MoveDrawing(_lastPosition);
        else
            FinishDrawing(_lastPosition);
    }

    private void DrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        Point position = GetClampedPosition(e.GetPosition(DrawingCanvas));
        FinishDrawing(position);
        e.Handled = true;
    }

    private void DrawingCanvas_LostMouseCapture(object sender, MouseEventArgs e)
    {
        // Если захват мыши прервался, корректно завершаем текущую фигуру.
        if (_toolManager.IsDrawing)
            FinishDrawing(_lastPosition);
    }

    private void FinishDrawing(Point position)
    {
        if (!_toolManager.IsDrawing)
            return;

        _toolManager.FinishDrawing(position);

        if (DrawingCanvas.IsMouseCaptured)
            DrawingCanvas.ReleaseMouseCapture();

        UpdateObjectCount();
        StatusTextBlock.Text = "Готово. Можно рисовать следующую фигуру.";
    }

    private Point GetClampedPosition(Point position)
    {
        return new Point(
            Math.Clamp(position.X, 0, DrawingCanvas.ActualWidth),
            Math.Clamp(position.Y, 0, DrawingCanvas.ActualHeight));
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        DrawingCanvas.Children.Clear();
        UpdateObjectCount();
        StatusTextBlock.Text = "Холст очищен.";
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Title = "Сохранить изображение",
            Filter = "PNG изображение (*.png)|*.png",
            DefaultExt = ".png",
            AddExtension = true,
            FileName = "drawing.png",
            OverwritePrompt = true
        };

        if (dialog.ShowDialog(this) != true)
            return;

        try
        {
            _imageExportService.SavePng(DrawingCanvas, dialog.FileName);
            StatusTextBlock.Text = $"Изображение сохранено: {Path.GetFileName(dialog.FileName)}";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
                                   or ArgumentException or InvalidOperationException)
        {
            MessageBox.Show(this, $"Не удалось сохранить изображение:\n{ex.Message}",
                "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateObjectCount()
    {
        ObjectCountTextBlock.Text = $"Объектов: {DrawingCanvas.Children.Count}";
    }
}
