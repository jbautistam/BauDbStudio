using System.Windows.Media.Imaging;

namespace Bau.Libraries.FileTools.Plugin.Controllers.Tools;

/// <summary>
///		Clase base para las herramientas de tratamiento de imágenes
/// </summary>
public abstract class BaseImageController
{
	/// <summary>
	///		Carga una imagen de un archivo
	/// </summary>
	public BitmapImage? LoadImage(string fileName)
	{
		BitmapImage? image = null;

			// Carga una imagen
			if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
				try
				{
					image = new BitmapImage(new Uri(fileName, UriKind.Absolute));
				}
				catch (Exception exception)
				{
					System.Diagnostics.Debug.WriteLine($"Error when load file {fileName}. {exception.Message}");
				}
			// Devuelve la imagen cargada
			return image;
	}

	/// <summary>
	///		Graba una imagen en un archivo
	/// </summary>
	public bool SaveImage(BitmapSource image, string fileName)
	{
		bool saved = false;

			// Graba la imagen
			try
			{
				using (FileStream file = new(fileName, FileMode.Create))
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
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine($"Error when save file {fileName}. {exception.Message}");
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