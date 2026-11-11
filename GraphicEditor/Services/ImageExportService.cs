using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphicEditor.Services;


public sealed class ImageExportService
{

    public void SavePng(Canvas canvas, string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Укажите путь к файлу.", nameof(filePath));

        int width = (int)Math.Ceiling(canvas.ActualWidth);
        int height = (int)Math.Ceiling(canvas.ActualHeight);

        if (width <= 0 || height <= 0)
            throw new InvalidOperationException("Холст ещё не готов к сохранению.");

        var bitmap = new RenderTargetBitmap(
            width, height, 96, 96, PixelFormats.Pbgra32);

        bitmap.Render(canvas);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));

        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write);
        encoder.Save(stream);
    }
}
