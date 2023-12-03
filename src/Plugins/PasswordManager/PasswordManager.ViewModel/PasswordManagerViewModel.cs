namespace Bau.Libraries.PasswordManager.ViewModel;

/// <summary>
///		ViewModel principal del administrador de contraseñas
/// </summary>
public class PasswordManagerViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	public PasswordManagerViewModel(Controllers.IPasswordManagerController mainController)
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
	///		Abre un archivo de entradas
	/// </summary>
	public bool OpenFile(string fileName)
	{
		if (!string.IsNullOrWhiteSpace(fileName) && 
			fileName.EndsWith(".bau.enc", StringComparison.CurrentCultureIgnoreCase))
		{
			string password = string.Empty;

				// Se obtiene una contraseña y se intenta abrir el archivo con esa contraseña
				if (ViewsController.HostController.SystemController.ShowPasswordString("Enter the file password", ref password) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				{
					Reader.PasswordFileViewModel viewModel = new(this, fileName, password);

						// Carga el archivo con la contraseña pasada como parámetro y si puede hacerlo, abre el documento
						if (viewModel.LoadFile())
							ViewsController.OpenWindow(viewModel);
				}
				// e indica que ha podido abrir el archivo (para que no se abra ningún documento)
				return true;
		}
		else
			return false;
	}

	/// <summary>
	///		Controlador de vistas de aplicación
	/// </summary>
	public Controllers.IPasswordManagerController ViewsController { get; }
}