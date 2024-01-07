namespace Bau.Libraries.RestManager.ViewModel;

/// <summary>
///		ViewModel principal del player multimedia
/// </summary>
public class RestManagerViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	public RestManagerViewModel(Controllers.IRestManagerController mainController)
	{
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
	///		Abre un archivo multimedia
	/// </summary>
	public bool OpenFile(string fileName)
	{
		bool isOpen = false;

			// Abre el archivo si está entre los reconocidos
			if (!string.IsNullOrWhiteSpace(fileName) && fileName.EndsWith(".rest", StringComparison.CurrentCultureIgnoreCase))
			{
				Project.RestFileViewModel restFileViewModel = new(this, fileName);

					// Carga el archivo
					restFileViewModel.Load();
					// Abre la ventana
					ViewsController.OpenWindow(restFileViewModel);
					// e indica que ha podido abrir el archivo
					isOpen = true;
			}
			// Devuelve el valor que indica si se ha abierto el archivo
			return isOpen;
	}

	/// <summary>
	///		Controlador de vistas de aplicación
	/// </summary>
	public Controllers.IRestManagerController ViewsController { get; }
}
