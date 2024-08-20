using Bau.Libraries.LibReporting.Models.DataWarehouses;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.DataWarehouses;

/// <summary>
///		ViewModel de un <see cref="DataWarehouseModel"/>
/// </summary>
public class DataWarehouseViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _name = string.Empty, _description = string.Empty;

	public DataWarehouseViewModel(ReportingSolutionViewModel reportingSolutionViewModel, DataWarehouseModel dataWarehouse)
	{
		// Inicializa las propiedades
		ReportingSolutionViewModel = reportingSolutionViewModel;
		DataWarehouse = dataWarehouse;
		// Inicializa el viewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel()
	{
		// Asigna las propiedades
		Name = DataWarehouse.Name;
		Description = DataWarehouse.Description;
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
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el nombre del almacén de datos");
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
			// Asigna los datos al origen de datos
			DataWarehouse.Name = Name;
			DataWarehouse.Description = Description;
			// Indica que ya no es nuevo y está grabado
			IsUpdated = false;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

	/// <summary>
	///		Almacén de datos
	/// </summary>
	public DataWarehouseModel DataWarehouse { get; }

	/// <summary>
	///		Nombre del almacén de datos
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Descripción del almacén de datos
	/// </summary>
	public string Description
	{
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}
}