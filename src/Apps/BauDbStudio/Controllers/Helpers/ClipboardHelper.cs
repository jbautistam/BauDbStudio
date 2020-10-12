using System;
using System.IO;

namespace Bau.DbStudio.Controllers.Helpers
{
	/// <summary>
	///		Helper para el tratamiento del portapapeles
	/// </summary>
	internal class ClipboardHelper
	{
		/// <summary>
		///		Comprueba si el portapapeles contiene una imagen
		/// </summary>
		internal bool ContainsImage()
		{
			return System.Windows.Clipboard.ContainsImage();
		}

		/// <summary>
		///		Graba la imagen del portapapeles a un archivo
		/// </summary>
		internal bool SaveImage(string fileName)
		{
			bool saved = false;

				// Pega la imagen
				if (ContainsImage())
				{ 
					System.Windows.Media.Imaging.BitmapSource image = System.Windows.Clipboard.GetImage();

						// Graba la imagen
						using (FileStream file = new FileStream(fileName, FileMode.Create))
						{
							System.Windows.Media.Imaging.BitmapEncoder encoder = new System.Windows.Media.Imaging.JpegBitmapEncoder();

								// Codifica la imagen
								encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(image));
								// Graba la imagen
								encoder.Save(file);
						}
						// Indica que se ha grabado correctamente
						saved = true;
				}
				// Devuelve el valor que indica si se ha grabado correctamente
				return saved;
		}
	}
}