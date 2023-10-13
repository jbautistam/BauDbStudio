using Bau.Libraries.CloudStudio.Models.Cloud;

namespace Bau.Libraries.CloudStudio.ViewModels.Details.Cloud;

/// <summary>
///		ViewModel de datos de storage
/// </summary>
public class StorageViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _name = default!, _description = default!, _connectionString = default!;
	private bool _isNew;

	public StorageViewModel(CloudStudioViewModel solutionViewModel, StorageModel storage)
	{
		// Inicializa las propiedades
		SolutionViewModel = solutionViewModel;
		IsNew = storage == null;
		Storage = storage ?? new StorageModel(solutionViewModel.Solution);
		// Inicializa el viewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel()
	{
		// Asigna las propiedades
		Name = Storage.Name;
		if (string.IsNullOrWhiteSpace(Name))
			Name = "Nuevo almacenamiento";
		Description = Storage.Description;
		ConnectionString = Storage.StorageConnectionString;
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(Name))
				SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre del storage");
			else if (string.IsNullOrWhiteSpace(ConnectionString))
				SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione la cadena de conexión");
			else
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		if (ValidateData())
		{
			// Asigna los datos al objeto
			Storage.Name = Name;
			Storage.Description = Description;
			Storage.StorageConnectionString = ConnectionString;
			// Añade los datos a la solución si es necesario
			if (IsNew)
				SolutionViewModel.Solution.Storages.Add(Storage);
			// Indica que ya no es nuevo y está grabado
			IsNew = false;
			IsUpdated = false;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public CloudStudioViewModel SolutionViewModel { get; }

	/// <summary>
	///		Datos de almacenamiento
	/// </summary>
	public StorageModel Storage { get; }

	/// <summary>
	///		Indica si es nuevo
	/// </summary>
	public bool IsNew
	{
		get { return _isNew; }
		set { CheckProperty(ref _isNew, value); }
	}

	/// <summary>
	///		Nombre
	/// </summary>
	public string Name 
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Descripción
	/// </summary>
	public string Description
	{
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}

	/// <summary>
	///		Cadena de conexión al storage
	/// </summary>
	public string ConnectionString
	{
		get { return _connectionString; }
		set { CheckProperty(ref _connectionString, value); }
	}
}