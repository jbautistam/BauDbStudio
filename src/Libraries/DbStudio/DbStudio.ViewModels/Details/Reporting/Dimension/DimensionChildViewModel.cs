using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Dimension;

/// <summary>
///		ViewModel de mantenimiento de un <see cref="DimensionChildModel"/>
/// </summary>
public class DimensionChildViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _name = string.Empty, _description = string.Empty, _columnsPrefix = string.Empty;
	private ComboViewModel _comboDimensions = default!;
	private bool _isNew;

	public DimensionChildViewModel(ReportingSolutionViewModel reportingSolutionViewModel, DimensionChildModel dimension, bool isNew)
	{
		// Inicializa los objetos
		ReportingSolutionViewModel = reportingSolutionViewModel;
		Dimension = dimension;
		// Inicializa las propiedades
		InitViewModel();
		_isNew = isNew;
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		// Carga el combo de dimensiones
		LoadComboDimensions();
		// Asigna las propiedades
		Name = Dimension.Id;
		Description = Dimension.Description;
		ColumnsPrefix = Dimension.ColumnsPrefix;
	}

	/// <summary>
	///		Carga el combo de dimensiones
	/// </summary>
	private void LoadComboDimensions()
	{
		// Inicializa el combo
		ComboDimensions = new ComboViewModel(this);
		// Añade los elementos
		ComboDimensions.AddItem(-1, "<Seleccione una dimensión>");
		foreach (BaseDimensionModel baseDimension in Dimension.DataWarehouse.Dimensions.EnumerateValuesSorted())
			if (baseDimension is DimensionModel dimension)
			{
				// Añade la dimensión
				ComboDimensions.AddItem(ComboDimensions.Items.Count + 1, dimension.Id, dimension);
				// Selecciona el elemento si es la dimensión origen
				if (dimension.Id.Equals(Dimension.SourceDimensionId, StringComparison.CurrentCultureIgnoreCase))
					ComboDimensions.SelectedItem = ComboDimensions.Items[ComboDimensions.Items.Count - 1];
		}
		// Selecciona el primer elemento
		if (ComboDimensions.SelectedItem is null)
			ComboDimensions.SelectedItem = ComboDimensions.Items[0];
	}

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos
			if (string.IsNullOrWhiteSpace(Name))
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el nombre de la dimensión");
			else if (string.IsNullOrWhiteSpace(ColumnsPrefix))
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el prefijo de la columna");
			else if (string.IsNullOrWhiteSpace(GetDimensionSource()))
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione la dimensión base");
			else
				validated = true;
			// Devuelve el valor que indica si se ha podido grabar
			return validated;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		if (ValidateData())
		{
			// Elimina la dimensión si existía
			if (!_isNew)
				Dimension.DataWarehouse.Dimensions.Remove(Dimension.Id);
			// Crea la nueva dimensión
			Dimension.DataWarehouse.Dimensions.Add(new DimensionChildModel(Dimension.DataWarehouse, Name, GetDimensionSource(), ColumnsPrefix)
																{
																	Description = Description
																}
												  );
			// Graba la solución
			ReportingSolutionViewModel.SaveDataWarehouse(Dimension.DataWarehouse);
			// Indica que ya no es nuevo y está grabado
			IsUpdated = false;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		Obtiene la dimensión
	/// </summary>
	private DimensionModel? GetDimension() => ComboDimensions.SelectedItem?.Tag as DimensionModel;

	/// <summary>
	///		Obtiene la dimensión origen
	/// </summary>
	private string GetDimensionSource()
	{
		DimensionModel? dimension = GetDimension();

			if (dimension is null)
				return string.Empty;
			else
				return dimension.Id;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

	/// <summary>
	///		Dimensión
	/// </summary>
	public DimensionChildModel Dimension { get; }

	/// <summary>
	///		Nombre
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Descipción
	/// </summary>
	public string Description
	{
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}

	/// <summary>
	///		Prefijo de columnas
	/// </summary>
	public string ColumnsPrefix
	{
		get { return _columnsPrefix; }
		set { CheckProperty(ref _columnsPrefix, value); }
	}

	/// <summary>
	///		Combo de dimensiones
	/// </summary>
	public ComboViewModel ComboDimensions
	{
		get { return _comboDimensions; }
		set { CheckObject(ref _comboDimensions, value); }
	}
}