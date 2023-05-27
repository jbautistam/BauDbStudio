using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;

namespace Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects;

/// <summary>
///		ViewModel de creación de archivos SQL de validación
/// </summary>
public class CreateValidationScriptsViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _dataBaseValidateVariable, _dataBaseComputeVariable, _mountPathVariable, _mountPathContent, _pathValidate;
	private string _outputPath, _dataBaseTarget, _tablePrefixes, _bitFields;
	private string _dateFormat, _decimalSeparator, _decimalType;
	private bool _validateFiles, _generateQvs, _compareString, _compareOnlyAlphaAndDigits;
	private Connections.ComboConnectionsViewModel _comboConnections;
	private Explorers.Connections.TreeConnectionTablesViewModel _treeConnection;
	private Application.SolutionManager.FormatType _formatType;
	private ComboViewModel _comboFormat;

	public CreateValidationScriptsViewModel(DbStudioViewModel solutionViewModel)
	{
		// Inicializa las propiedades
		SolutionViewModel = solutionViewModel;
		TreeConnection = new Explorers.Connections.TreeConnectionTablesViewModel(SolutionViewModel);
		ComboConnections = new Connections.ComboConnectionsViewModel(SolutionViewModel, string.Empty);
		ComboConnections.Connections.PropertyChanged += (sender, args) => {
																			if (!string.IsNullOrWhiteSpace(args.PropertyName) && 
																					args.PropertyName.Equals(nameof(ComboConnections.Connections.SelectedItem)))
																				TreeConnection.LoadConnection(ComboConnections.GetSelectedConnection());
																		  };
		// Inicializa el viewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel()
	{
		// Combo de formato de los archivos
		ComboFormat = new ComboViewModel(this);
		ComboFormat.AddItem((int) Application.SolutionManager.FormatType.Parquet, "Parquet");
		ComboFormat.AddItem((int) Application.SolutionManager.FormatType.Csv, "CSV");
		ComboFormat.SelectedItem = ComboFormat.Items[0];
		// Asigna las propiedades
		ValidateFiles = true;
		MountPathVariable = "MountPath";
		MountPathContent = "/mnt/c/Test";
		DataBaseComputeVariable = "DbCompute";
		DataBaseValidateVariable = "DbValidate";
		PathValidate = "Validate";
		TablePrefixes = "SRC_;EXT_;TRN_";
		GenerateQvs = true;
		OutputPath = string.Empty;
		CompareString = true;
		DateFormat = "d/M/yyyy";
		DecimalSeparator = ",";
		DecimalType = "decimal(10, 2)";
		CompareOnlyAlphaAndDigits = true;
		// Carga el árbol de conexiones
		TreeConnection.LoadConnection(ComboConnections.GetSelectedConnection());
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
			if (ComboConnections.GetSelectedConnection() == null)
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione una conexión");
			else if (string.IsNullOrWhiteSpace(OutputPath))
				SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el directorio de grabación de los archivos");
			else if (string.IsNullOrWhiteSpace(DataBaseComputeVariable))
				SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el nombre de la variable de base de datos de cálculo");
			else if (string.IsNullOrWhiteSpace(DataBaseValidateVariable))
				SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el nombre de la variable de base de datos de validación");
			else if (TreeConnection.GetSelectedTables().Count == 0)
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione al menos una tabla");
			else if (ValidateFiles)
			{
				if (string.IsNullOrWhiteSpace(MountPathVariable))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el nombre de la variable con el directorio de archivos");
				else if (string.IsNullOrWhiteSpace(MountPathContent))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el directorio de archivos");
				else if (string.IsNullOrWhiteSpace(PathValidate))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el nombre del subdirectorio donde se encuentran los archivos a validar");
				else 
					validated = true;
			}
			else
			{
				if (string.IsNullOrWhiteSpace(DataBaseTarget))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el nombre de base de datos a comparar");
				else
					validated = true;
			}
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
			// Guarda las propiedades
			FormatType = (Application.SolutionManager.FormatType) (ComboFormat.SelectedId ?? (int) Application.SolutionManager.FormatType.Parquet);
			// Indica que ya no es nuevo y está grabado
			IsUpdated = false;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public DbStudioViewModel SolutionViewModel { get; }

	/// <summary>
	///		Indica si se deben comprobar archivos o base de datos
	/// </summary>
	public bool ValidateFiles
	{
		get { return _validateFiles; }
		set { CheckProperty(ref _validateFiles, value); }
	}

	/// <summary>
	///		Variable con el directorio de montaje
	/// </summary>
	public string MountPathVariable
	{
		get { return _mountPathVariable; }
		set { CheckProperty(ref _mountPathVariable, value); }
	}

	/// <summary>
	///		Directorio al que apunta el directorio de montaje
	/// </summary>
	public string MountPathContent
	{
		get { return _mountPathContent; }
		set { CheckProperty(ref _mountPathContent, value); }
	}

	/// <summary>
	///		Subdirectorio donde se encuentran los archivos de validación
	/// </summary>
	public string PathValidate
	{
		get { return _pathValidate; }
		set { CheckProperty( ref _pathValidate, value); }
	}

	/// <summary>
	///		Variable con el nombre de base de datos de validación
	/// </summary>
	public string DataBaseValidateVariable
	{
		get { return _dataBaseValidateVariable; }
		set { CheckProperty(ref _dataBaseValidateVariable, value); }
	}

	/// <summary>
	///		Variable con el nombre de base de datos de cálculo
	/// </summary>
	public string DataBaseComputeVariable
	{
		get { return _dataBaseComputeVariable; }
		set { CheckProperty(ref _dataBaseComputeVariable, value); }
	}

	/// <summary>
	///		Directorio de salida
	/// </summary>
	public string OutputPath
	{
		get { return _outputPath; }
		set { CheckProperty(ref _outputPath, value); }
	}

	/// <summary>
	///		Indica si se debe generar un QVS de validación
	/// </summary>
	public bool GenerateQvs
	{
		get { return _generateQvs; }
		set { CheckProperty(ref _generateQvs, value); }
	}

	/// <summary>
	///		Combo de conexiones
	/// </summary>
	public Connections.ComboConnectionsViewModel ComboConnections
	{
		get { return _comboConnections; }
		set { CheckObject(ref _comboConnections, value); }
	}

	/// <summary>
	///		Combo de tipos de exportación
	/// </summary>
	public ComboViewModel ComboFormat
	{
		get { return _comboFormat; }
		set { CheckObject(ref _comboFormat, value); }
	}

	/// <summary>
	///		Arbol de una conexión
	/// </summary>
	public Explorers.Connections.TreeConnectionTablesViewModel TreeConnection
	{
		get { return _treeConnection; }
		set { CheckObject(ref _treeConnection, value); }
	}

	/// <summary>
	///		Formato de los archivos de salida
	/// </summary>
	public Application.SolutionManager.FormatType FormatType
	{
		get { return _formatType; }
		set { CheckProperty(ref _formatType, value); }
	}

	/// <summary>
	///		Base de datos destino de la comparación (si se compara por base de datos)
	/// </summary>
	public string DataBaseTarget
	{
		get { return _dataBaseTarget; }
		set { CheckProperty(ref _dataBaseTarget, value); }
	}

	/// <summary>
	///		Prefijos a eliminar en las tablas al compararlas con archivos separadas por punto y coma (por ejemplo, SRC_, TMP_...)
	/// </summary>
	public string TablePrefixes
	{
		get { return _tablePrefixes; }
		set { CheckProperty(ref _tablePrefixes, value); }
	}

	/// <summary>
	///		Indica si en el archivo se van a comparar cadena
	/// </summary>
	public bool CompareString 
	{ 
		get { return _compareString; } 
		set { CheckProperty(ref _compareString, value); }
	}

	/// <summary>
	///		Formato de fechas
	/// </summary>
	public string DateFormat
	{
		get { return _dateFormat; }
		set { CheckProperty(ref _dateFormat, value); }
	}

	/// <summary>
	///		Separador decimal
	/// </summary>
	public string DecimalSeparator
	{
		get { return _decimalSeparator; }
		set { CheckProperty(ref _decimalSeparator, value); }
	}

	/// <summary>
	///		Tipo para los campos decimales
	/// </summary>
	public string DecimalType
	{
		get { return _decimalType; }
		set { CheckProperty(ref _decimalType, value); }
	}

	/// <summary>
	///		Campos de tipo bit (se sustituirán por ABS)
	/// </summary>
	public string BitFields
	{
		get { return _bitFields; }
		set { CheckProperty(ref _bitFields, value); }
	}

	/// <summary>
	///		Indica si en las cadenas se tienen que comparar sólo los caracteres alfabéticos y dígitos
	/// </summary>
	public bool CompareOnlyAlphaAndDigits
	{
		get { return _compareOnlyAlphaAndDigits; }
		set { CheckProperty(ref _compareOnlyAlphaAndDigits, value); }
	}
}