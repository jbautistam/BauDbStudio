using System.Windows.Media.Imaging;

namespace Bau.DbStudio.Controllers.Helpers;

/// <summary>
///		Helper para el tratamiento del portapapeles
/// </summary>
internal class ClipboardHelper
{
	/// <summary>
	///		Copia un objeto al portapapeles
	/// </summary>
	internal void SetData(object value)
	{
		if (value is not null)
		{
			if (value is string valueString)
				System.Windows.Clipboard.SetText(valueString);
			else
				System.Windows.Clipboard.SetDataObject(value);
		}
	}

	/// <summary>
	///		Comprueba si el portapapeles contiene una imagen
	/// </summary>
	internal bool ContainsImage() => System.Windows.Clipboard.ContainsImage();

	/// <summary>
	///		Graba la imagen del portapapeles a un archivo
	/// </summary>
	internal bool SaveImage(string fileName) 
	{
		bool saved = false;

			// Pega la imagen
			if (ContainsImage())
			{ 
				BitmapSource image = System.Windows.Clipboard.GetImage();

					// Graba la imagen
					saved = new ImageHelper().SaveImage(image, fileName);
			}
			// Devuelve el valor que indica si se ha grabado correctamente
			return saved;
	}
}