using System;

namespace Bau.Libraries.MultimediaFiles.ViewModel
{
	/// <summary>
	///		ViewModel principal del player multimedia
	/// </summary>
	public class MultimediaFilesViewModel : BauMvvm.ViewModels.BaseObservableObject
	{
		public MultimediaFilesViewModel(Controllers.IMultimediaFilesController mainController)
		{
			// Asigna el controlador de vistas
			ViewsController = mainController;
			MediaFileListViewModel = new Reader.MediaFileListViewModel(this);
		}

		/// <summary>
		///		Inicializa el viewModel
		/// </summary>
		public void Initialize()
		{
			// no hace nada, simplemente implementa la interface
		}

		/// <summary>
		///		Carga el directorio
		/// </summary>
		public void Load(string path)
		{
			// no hace nada, simplemente implementa la interface
		}

		/// <summary>
		///		Abre un archivo multimedia
		/// </summary>
		public bool OpenFile(string fileName)
		{
			bool isOpen = false;

				// Abre el archivo si está entre los reconocidos
				if (!string.IsNullOrWhiteSpace(fileName))
				{
					if (fileName.EndsWith(".mp3", StringComparison.CurrentCultureIgnoreCase) ||
							fileName.EndsWith(".wav", StringComparison.CurrentCultureIgnoreCase))
					{
						// Reproduce el archivo
						MediaFileListViewModel.PlayMediaFile(fileName, true);
						// e indica que ha podido abrir el archivo
						isOpen = true;
					}
					else if (fileName.EndsWith(".mp4", StringComparison.CurrentCultureIgnoreCase) ||
							 fileName.EndsWith(".mkv", StringComparison.CurrentCultureIgnoreCase) ||
							 fileName.EndsWith(".avi", StringComparison.CurrentCultureIgnoreCase))
					{
						// Reproduce el archivo
						MediaFileListViewModel.PlayMediaFile(fileName, false);
						// e indica que ha podido abrir el archivo
						isOpen = true;
					}
				}
				// Devuelve el valor que indica si se ha abierto el archivo
				return isOpen;
		}

		/// <summary>
		///		Controlador de vistas de aplicación
		/// </summary>
		public Controllers.IMultimediaFilesController ViewsController { get; }

		/// <summary>
		///		ViewModel de la lista de archivos multimedia
		/// </summary>
		public Reader.MediaFileListViewModel MediaFileListViewModel { get; }
	}
}
