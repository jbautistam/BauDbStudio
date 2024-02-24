namespace Bau.Libraries.EbooksReader.ViewModel;

/// <summary>
///		ViewModel principal del lector de eBooks
/// </summary>
public class EBookReaderViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	public EBookReaderViewModel(Controllers.IEBookReaderController mainController)
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
	///		Abre un archivo
	/// </summary>
	public bool OpenFile(string fileName)
	{
		if (!string.IsNullOrWhiteSpace(fileName))
		{
			if (fileName.EndsWith(".epub", StringComparison.CurrentCultureIgnoreCase) ||
					fileName.EndsWith(".mobi", StringComparison.CurrentCultureIgnoreCase))
			{
				// Abre la ventana
				ViewsController.OpenWindow(new Reader.eBooks.EBookContentViewModel(this, fileName));
				// e indica que ha podido abrir el archivo
				return true;
			}
			else if (fileName.EndsWith(".cbr", StringComparison.CurrentCultureIgnoreCase) ||
						fileName.EndsWith(".cbz", StringComparison.CurrentCultureIgnoreCase) ||
						fileName.EndsWith(".zip", StringComparison.CurrentCultureIgnoreCase) ||
						fileName.EndsWith(".rar", StringComparison.CurrentCultureIgnoreCase))
			{
				// Abre la ventana
				ViewsController.OpenWindow(new Reader.Comics.ComicContentViewModel(this, fileName));
				// e indica que ha podido abrir el archivo
				return true;
			}
		}
		// Indica que no se ha abierto el archivo
		return false;
	}

	/// <summary>
	///		Controlador de vistas de aplicación
	/// </summary>
	public Controllers.IEBookReaderController ViewsController { get; }
}
