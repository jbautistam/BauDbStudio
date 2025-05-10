using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bau.Libraries.FileTools.Plugin.Controllers.Tools;

/// <summary>
///     Controlador para redimensionar imágenes
/// </summary>
public class ResizeImagesController : BaseImageController
{
    /// <summary>
    ///     Redimensiona la imagen
    /// </summary>
    public void Resize(string sourceFileName, string targetFileName, double newWidth, double newHeight)
    {
        BitmapImage? image = LoadImage(sourceFileName);

            if (image is not null)
            {
                DrawingVisual drawingVisual = new();
                RenderTargetBitmap renderTarget = new((int) newWidth, (int) newHeight, 96, 96, PixelFormats.Pbgra32);

                    // Dibuja la imagen redimensionada
                    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        drawingContext.DrawImage(image, new Rect(0, 0, newWidth, newHeight));
                    }
                    // Renderiza el DrawingVisual sobre RenderTargetBitmap
                    renderTarget.Render(drawingVisual);
                    // Graba la imagen
                    SaveImage(renderTarget, targetFileName);
            }
    }
}
