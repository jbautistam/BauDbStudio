using System.Windows;
using System.Windows.Media.Imaging;

namespace Bau.Libraries.FileTools.Plugin.Controllers.Tools;

/// <summary>
///     Controlador para la división de imágenes
/// </summary>
public class SplitImagesController : BaseImageController
{
    /// <summary>
    ///     Divide una imagen en diferentes archivos
    /// </summary>
    public void Split(string sourceFileName, string outputFolder, string targetFileName, int rows, int columns)
    {
        BitmapImage? sourceImage = LoadImage(sourceFileName);

            if (sourceImage is not null)
            {
                int top = 0;
                int width = (int) sourceImage.Width / columns, height = (int) sourceImage.Height / rows;

                    // Recorre las filas
                    for (int row = 0; row < rows; row++)
                    {
                        int left = 0;

                            // Recorre las columnas
                            for (int column = 0; column < columns; column++)
                            {
                                CroppedBitmap croppedBitmap = Crop(sourceImage, top, left, width, height);

                                    // Guardar la imagen recortada
                                    SaveCroppedBitmap(croppedBitmap, Path.Combine(outputFolder, GetFileName(targetFileName, row, column)));
                                    // Incrementa la posición izquierda
                                    left += width;
                            }
                            // Incrementa la posición superior
                            top += height;
                    }
            }

        // Obtiene el nombre de archivo
        string GetFileName(string fileName, int row, int column)
        {
            return $"{Path.GetFileNameWithoutExtension(fileName)}_{row + 1:000}_{column + 1:000}{Path.GetExtension(fileName)}";
        }
    }

    /// <summary>
    ///     Extrae una parte de la imagen
    /// </summary>
    public CroppedBitmap Crop(BitmapImage image, int top, int left, int width, int height) 
    {
        // Normaliza los datos
        if (top + height >= image.Height)
            height = (int) image.Height - top - 1;
        if (left + width >= image.Width)
            width = (int) image.Width - left - 1;
        // Devuelve la imagen cortada
        return new CroppedBitmap(image, new Int32Rect(left, top, width, height));
    }

    /// <summary>
    ///     Graba una imagen
    /// </summary>
    private void SaveCroppedBitmap(CroppedBitmap image, string outputPath) => SaveImage(image, outputPath);
}