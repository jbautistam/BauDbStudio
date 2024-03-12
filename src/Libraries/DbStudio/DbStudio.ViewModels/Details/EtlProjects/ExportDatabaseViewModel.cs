using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects;

/// <summary>
///		ViewModel para la exportación de archivos de base de datos
/// </summary>
public class ExportDatabaseViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private Connections.ComboConnectionsViewModel _comboConnections = default!;
	private ComboViewModel _comboFormat = default!;
	private Explorers.Connections.TreeConnectionTablesViewModel _treeConnection = default!;
	private string _outputPath = string.Empty;
	private Application.SolutionManager.FormatType _formatType;
	private long _blockSize;
	private CsvFileViewModel _csvFileViewModel;
	private bool _isCsvFile;

	public ExportDatabaseViewModel(ConnectionModel? connectionDefault, ConnectionTableModel? tableDefault, DbStudioViewModel solutionViewModel)
	{
		// Inicializa los objetos
		SolutionViewModel = solutionViewModel;
		TreeConnection = new Explorers.Connections.TreeConnectionTablesViewModel(SolutionViewModel);
		ComboConnections = new Connections.ComboConnectionsViewModel(SolutionViewModel, string.Empty);
		ComboConnections.Connections.PropertyChanged += (sender, args) => {
																			if (!string.IsNullOrWhiteSpace(args.PropertyName) && 
																					args.PropertyName.Equals(nameof(ComboConnections.Connections.SelectedItem)))
																				UpdateConnection();
																		  };
		// Inicializa el viewModel
		_csvFileViewModel = new CsvFileViewModel(solutionViewModel);
		InitViewModel(connectionDefault, tableDefault);
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel(ConnectionModel? connectionDefault, ConnectionTableModel? tableDefault)
	{
		// Combo de formato de los archivos
		ComboFormat = new ComboViewModel(this);
		ComboFormat.PropertyChanged += (sender, args) => {
															if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																	args.PropertyName.Equals(nameof(ComboFormat.SelectedItem), StringComparison.CurrentCultureIgnoreCase))
																IsCsvFile = ComboFormat.SelectedId == (int) Application.SolutionManager.FormatType.Csv;
														 };
		ComboFormat.AddItem((int) Application.SolutionManager.FormatType.Parquet, "Parquet");
		ComboFormat.AddItem((int) Application.SolutionManager.FormatType.Csv, "CSV");
		ComboFormat.SelectedItem = ComboFormat.Items[0];
		// Asigna las propiedades
		OutputPath = SolutionViewModel.MainController.DialogsController.LastPathSelected;
		BlockSize = 2_000_000;
		// Actualiza los datos relacionados con la conexión
		UpdateConnection();
		// Selecciona los datos predeterminados
		if (connectionDefault is not null)
			ComboConnections.SelectConnection(connectionDefault);
		if (tableDefault is not null)
			TreeConnection.CheckTable(tableDefault);
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Actualiza los datos asociados a la conexión
	/// </summary>
	private void UpdateConnection()
	{
		ConnectionModel? connection = ComboConnections.GetSelectedConnection();

			if (connection is not null)
				TreeConnection.LoadConnection(connection);
	}

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (ComboConnections.GetSelectedConnection() == null)
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione la conexión");
			else if (string.IsNullOrWhiteSpace(OutputPath))
				SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el directorio de salida de los archivos");
			else if (TreeConnection.GetSelectedTables().Count == 0)
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione al menos una tabla");
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
			// Guarda las propiedades
			FormatType = (Application.SolutionManager.FormatType) (ComboFormat.SelectedId ?? (int) Application.SolutionManager.FormatType.Parquet);
			// Guarda el último directorio seleccionado
			SolutionViewModel.MainController.DialogsController.LastPathSelected = OutputPath;
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
	///		Directorio de salida
	/// </summary>
	public string OutputPath
	{
		get { return _outputPath; }
		set { CheckProperty(ref _outputPath, value); }
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
	///		Tipos de exportación
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
		set 
		{ 
			if (CheckProperty(ref _formatType, value))
				IsCsvFile = value == Application.SolutionManager.FormatType.Csv;
		}
	}

	/// <summary>
	///		Tamaño del bloque de escritura
	/// </summary>
	public long BlockSize
	{
		get { return _blockSize; }
		set { CheckProperty(ref _blockSize, value); }
	}

	/// <summary>
	///		Parámetros del archivo CSV
	/// </summary>
	public CsvFileViewModel FileCsvViewModel
	{
		get { return _csvFileViewModel; }
		set { CheckObject(ref _csvFileViewModel, value); }
	}

	/// <summary>
	///		Indica si es un archivo CSV
	/// </summary>
	public bool IsCsvFile
	{
		get { return _isCsvFile; }
		set { CheckProperty(ref _isCsvFile, value); }
	}
}