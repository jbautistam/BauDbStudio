using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Dimension;

/// <summary>
///		ViewModel de mantenimiento de un <see cref="DimensionModel"/>
/// </summary>
public class DimensionViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _header = string.Empty, _key = string.Empty, _dataSourceId = string.Empty, _description = string.Empty;
	private bool _isNew;
	private Relations.ListRelationViewModel _listRelationsViewModel = default!;

	public DimensionViewModel(ReportingSolutionViewModel reportingSolutionViewModel, DimensionModel dimension, bool isNew)
	{
		// Inicializa los objetos
		ReportingSolutionViewModel = reportingSolutionViewModel;
		Dimension = dimension;
		// Inicializa las variables
		_isNew = isNew;
		// Inicializa las propiedades
		InitViewModel();
		// Asigna los manejadores de eventos
		ListRelationsViewModel.PropertyChanged += (sender, args) => {
																		if ((args.PropertyName ?? string.Empty).Equals(nameof(ListRelationsViewModel.IsUpdated)))
																			IsUpdated |= ListRelationsViewModel.IsUpdated;
																	};
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		// Asigna las propiedades básicas
		if (_isNew)
			Key = NormalizeName(Dimension.DataSource.Id);
		else
			Key = Dimension.Id;
		Header = Key;
		DataSourceId = Dimension.DataSource.Id;
		// Asigna el resto de propiedades
		Description = Dimension.Description;
		// Carga las relaciones hijas
		ListRelationsViewModel = new Relations.ListRelationViewModel(ReportingSolutionViewModel, Dimension.DataSource, Dimension.Relations);
		ListRelationsViewModel.Load();
		// Indica que por ahora no ha habido modificaciones
		IsUpdated = _isNew;
		ListRelationsViewModel.IsUpdated = false;
	}

	/// <summary>
	///		Normaliza el nombre de la dimensión
	/// </summary>
	private string NormalizeName(string value)
	{
		// Quita los caracteres SQL
		if (!string.IsNullOrWhiteSpace(value))
		{
			value = value.Replace("[", "");
			value = value.Replace("]", "");
			value = value.Replace(".", "");
		}
		// Devuelve el valor
		return value;
	}

	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
	}

	/// <summary>
	///		Obtiene el mensaje de grabación
	/// </summary>
	public string GetSaveAndCloseMessage() => $"¿Desea grabar las modificaciones de la dimensión '{Key}'?";

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos
			if (string.IsNullOrWhiteSpace(Key))
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca la clave de la dimensión");
			else
				validated = true;
			// Devuelve el valor que indica si se ha podido grabar
			return validated;
	}

	/// <summary>
	///		Graba la dimensión
	/// </summary>
	public void SaveDetails(bool newName)
	{
		if (ValidateData())
		{
			// Asigna las propiedades a la dimensión
			Dimension.Id = Key;
			Dimension.Description = Description;
			// Si es nuevo se añade a la colección
			if (_isNew)
			{
				// Añade la dimensión
				Dimension.DataSource.DataWarehouse.Dimensions.Add(Dimension);
				// Indica que ya no es nuevo
				_isNew = false;
			}
			// Asigna las relaciones
			Dimension.Relations.Clear();
			Dimension.Relations.AddRange(ListRelationsViewModel.GetRelations());
			// Graba la solución
			ReportingSolutionViewModel.SaveDataWarehouse(Dimension.DataSource.DataWarehouse);
			// Indica que no ha habido modificaciones
			IsUpdated = false;
			ListRelationsViewModel.IsUpdated = false;
		}
	}

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

	/// <summary>
	///		Dimensión
	/// </summary>
	public DimensionModel Dimension { get; }

	/// <summary>
	///		Cabecera de la ventana
	/// </summary>
	public string Header
	{
		get { return _header; }
		set { CheckProperty(ref _header, value); }
	}

	/// <summary>
	///		Id de la ficha en la aplicación
	/// </summary>
	public string TabId => $"{GetType().ToString()}_{Dimension.Id}";

	/// <summary>
	///		Clave de la dimensión
	/// </summary>
	public string Key
	{
		get { return _key; }
		set 
		{ 
			if (CheckProperty(ref _key, value))
				Header = value;
		}
	}

	/// <summary>
	///		Id del origen de datos
	/// </summary>
	public string DataSourceId
	{
		get { return _dataSourceId; }
		set { CheckProperty(ref _dataSourceId, value); }
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
	///		ViewModel con las relaciones con otras dimensiones
	/// </summary>
	public Relations.ListRelationViewModel ListRelationsViewModel
	{
		get { return _listRelationsViewModel; }
		set { CheckObject(ref _listRelationsViewModel, value); }
	}
}