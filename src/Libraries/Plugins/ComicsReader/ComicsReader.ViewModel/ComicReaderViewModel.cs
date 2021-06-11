using System;

namespace Bau.Libraries.ComicsReader.ViewModel
{
	/// <summary>
	///		ViewModel principal del lector de cómics
	/// </summary>
	public class ComicReaderViewModel : BauMvvm.ViewModels.BaseObservableObject
	{
		public ComicReaderViewModel(Controllers.IComicReaderController mainController)
		{
			// Asigna el controlador de vistas
			ViewsController = mainController;
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
		///		Abre un archivo de cómic
		/// </summary>
		public bool OpenFile(string fileName)
		{
			if (!string.IsNullOrWhiteSpace(fileName) && 
				(fileName.EndsWith(".cbr", StringComparison.CurrentCultureIgnoreCase) ||
					fileName.EndsWith(".cbz", StringComparison.CurrentCultureIgnoreCase) ||
					fileName.EndsWith(".zip", StringComparison.CurrentCultureIgnoreCase) ||
					fileName.EndsWith(".rar", StringComparison.CurrentCultureIgnoreCase)))
			{
				// Abre la ventana
				ViewsController.OpenWindow(new Reader.ComicContentViewModel(this, fileName));
				// e indica que ha podido abrir el archivo
				return true;
			}
			else
				return false;
		}

		/// <summary>
		///		Controlador de vistas de aplicación
		/// </summary>
		public Controllers.IComicReaderController ViewsController { get; }
	}
}
