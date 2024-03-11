using System.IO;
using System.Windows.Media.Imaging;

namespace Bau.DbStudio.Controllers.Helpers;

/// <summary>
///		Helper para el tratamiento de imágenes
/// </summary>
internal class ImageHelper
{
	/// <summary>
	///		Graba la imagen del portapapeles a un archivo
	/// </summary>
	internal bool SaveImage(BitmapSource image, string fileName)
	{
		bool saved = false;

			// Graba la imagen
			using (FileStream file = new FileStream(fileName, FileMode.Create))
			{
				BitmapEncoder? encoder = GetEncoderFromFilename(fileName);

					if (encoder is not null)
					{
						// Codifica la imagen
						encoder.Frames.Add(BitmapFrame.Create(image));
						// Graba la imagen
						encoder.Save(file);
						// Indica que se ha grabado correctamente
						saved = true;
					}
			}
			// Devuelve el valor que indica si se ha grabado correctamente
			return saved;
	}

	/// <summary>
	///		Obtiene el codificador adecuado para el archivo
	/// </summary>
	private BitmapEncoder? GetEncoderFromFilename(string fileName)
	{
		if (fileName.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) || fileName.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase))
			return new JpegBitmapEncoder();
		else if (fileName.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase))
			return new PngBitmapEncoder();
		else if (fileName.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase))
			return new GifBitmapEncoder();
		else if (fileName.EndsWith(".wmp", StringComparison.CurrentCultureIgnoreCase))
			return new WmpBitmapEncoder();
		else if (fileName.EndsWith(".tiff", StringComparison.CurrentCultureIgnoreCase))
			return new TiffBitmapEncoder();
		else if (fileName.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase))
			return new BmpBitmapEncoder();
		else
			return null;
	}
}