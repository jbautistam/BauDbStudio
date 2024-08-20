using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects;

/// <summary>
///		ViewModel para la importación de archivos de base de datos
/// </summary>
public class ImportDatabaseViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string? _fileName;
	private ComboViewModel _comboTables = default!;
	private BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<ImportDatabaseFieldViewModel> _listFields = default!;
	private List<string> _fileFields = new();
	private long _blockSize;
	private bool _isCsvFile;
	private CsvFileViewModel _csvFileViewModel = default!;

	public ImportDatabaseViewModel(ConnectionModel connection, DbStudioViewModel solutionViewModel)
	{
		SolutionViewModel = solutionViewModel;
		Connection = connection;
		ComboTables = new ComboViewModel(this);
		ComboTables.PropertyChanged += (sender, args) => {
															if (!string.IsNullOrWhiteSpace(args.PropertyName) && 
																args.PropertyName.Equals(nameof(ComboTables.SelectedItem), StringComparison.CurrentCultureIgnoreCase))
																	LoadTableFields();
														 };
		ListFields = new BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<ImportDatabaseFieldViewModel>();
		FileCsvViewModel = new CsvFileViewModel(solutionViewModel);
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	internal async Task InitializeAsync(ConnectionTableModel? selectedTable, CancellationToken cancellationToken)
	{
		// Asigna las propiedades
		BlockSize = 2_000_000;
		// Carga las tablas
		await LoadTablesAsync(selectedTable, cancellationToken);
	}

	/// <summary>
	///		Actualiza los datos asociados a la conexión
	/// </summary>
	private async Task LoadTablesAsync(ConnectionTableModel? selectedTable, CancellationToken cancellationToken)
	{
		// Carga el esquema
		await SolutionViewModel.Manager.LoadSchemaAsync(Connection, false, cancellationToken);
		// Limpia las tablas
		ComboTables.Items.Clear();
		// Carga las tablas
		ComboTables.Items.Add(new BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel("<Seleccione una tabla>", null));
		foreach (ConnectionTableModel table in Connection.Tables)
			if (!table.IsSystem)
			{
				// Añade el elemento
				ComboTables.Items.Add(new BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel(table.FullName, table));
				// Selecciona la tabla
				if (selectedTable is not null && selectedTable.FullName.Equals(table.FullName, StringComparison.CurrentCultureIgnoreCase))
					ComboTables.SelectedTag = table;
			}
		// Selecciona el primer elemento
		if (selectedTable is null)
			ComboTables.SelectedIndex = 0;
		// Limpia la tabla de campos
		LoadTableFields();
	}

	/// <summary>
	///		Carga los campos de la tabla seleccionada
	/// </summary>
	private void LoadTableFields()
	{
		// Limpia la lista de campos
		ListFields.Clear();
		// Carga la lista
		if (ComboTables.SelectedTag is ConnectionTableModel table)
			foreach (ConnectionTableFieldModel field in table.Fields)
				ListFields.Add(new ImportDatabaseFieldViewModel(this, field));
		// Carga los campos
		foreach (ImportDatabaseFieldViewModel item in ListFields)
			item.LoadComboFields(_fileFields);
	}

	/// <summary>
	///		Carga los campos del archivo
	/// </summary>
	private async Task LoadFileFieldsAsync(CancellationToken cancellationToken)
	{
		// Obtiene los campos
		_fileFields = await GetSchemaFileFieldsAsync(cancellationToken);
		// Carga los campos
		foreach (ImportDatabaseFieldViewModel item in ListFields)
			item.LoadComboFields(_fileFields);
	}

	/// <summary>
	///		Obtiene los campos de un archivo
	/// </summary>
	private async Task<List<string>> GetSchemaFileFieldsAsync(CancellationToken cancellationToken)
	{
		List<string> fields = new();

			// Carga el esquema del archivo en la lista de campos
			if (!string.IsNullOrWhiteSpace(FileName) && File.Exists(FileName))
			{
				if (FileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
					fields = await GetParquetSchemaAsync(FileName, cancellationToken);
				else if (FileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
					fields = GetCsvSchema(FileName);
			}
			// Devuelve los campos del esquema
			return fields;
	}

	/// <summary>
	///		Obtiene el esquema de un archivo parquet
	/// </summary>
	private async Task<List<string>> GetParquetSchemaAsync(string fileName, CancellationToken cancellationToken)
	{
		List<string> fields = new();

			// Obtiene el esquema del archivo
			try
			{
				// Añade los nombres de campos
				using (LibParquetFiles.Readers.ParquetDataReader reader = new())
				{
					// Abre el archivo
					await reader.OpenAsync(fileName, cancellationToken);
					// Añade los nombres de campos
					for (int index = 0; index < reader.FieldCount; index++)
						fields.Add(reader.GetName(index));
				}
			}
			catch (Exception exception)
			{
				SolutionViewModel.MainController.SystemController.ShowMessage($"Error when load file schema. {exception.Message}");
			}
			// Devuelve la lista de campos
			return fields;
	}

	/// <summary>
	///		Obtiene el esquema de un archivo CSV
	/// </summary>
	private List<string> GetCsvSchema(string fileName)
	{
		List<string> fields = new();

			// Obtiene el esquema del archivo
			try
			{
				// Añade los nombres de campos
				using (LibCsvFiles.CsvReader reader = new(null, null))
				{
					// Abre el archivo
					reader.Open(fileName);
					// Añade los nombres de campos
					for (int index = 0; index < reader.FieldCount; index++)
						fields.Add(reader.GetName(index));
				}
			}
			catch (Exception exception)
			{
				SolutionViewModel.MainController.SystemController.ShowMessage($"Error when load file schema. {exception.Message}");
			}
			// Devuelve la lista de campos
			return fields;
	}

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(FileName) || !File.Exists(FileName))
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione el archivo a importar");
			else if (ComboTables.SelectedTag is null)
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione la tabla donde se importa el archivo");
			else 
				validated = ValidateSelectedFields();
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Comprueba los campos seleccionados
	/// </summary>
	private bool ValidateSelectedFields()
	{
		bool validated = true;
		bool selected = false;

			// Comprueba los campos
			foreach (ImportDatabaseFieldViewModel field in ListFields)
				if (field.Checked && validated)
				{
					// Comprueba los datos
					if (string.IsNullOrWhiteSpace(field.GetFileField()))
					{
						// Muestra el mensaje al usuario
						SolutionViewModel.MainController.SystemController.ShowMessage($"No ha seleccionado ningún campo del archivo para el campo {field.Column}");
						// Indica que no es correcto
						validated = false;
					}
					// Indica que hay algo seleccionado
					selected = true;
				}
			// Comprueba si se ha seleccionado algo
			if (!selected && validated)
			{
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione al menos un campo");
				validated = false;
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
			// Guarda el último directorio seleccionado
			if (!string.IsNullOrWhiteSpace(FileName))
				SolutionViewModel.MainController.DialogsController.LastPathSelected = Path.GetDirectoryName(FileName) ?? FileName;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		Obtiene la tabla seleccionada
	/// </summary>
	public ConnectionTableModel? GetSelectedTable() => ComboTables.SelectedTag as ConnectionTableModel;

	/// <summary>
	///		Obtiene las columnas que se van a importar
	/// </summary>
	public List<(string column, string fileField)> GetColumnsImported()
	{
		List<(string column, string fileField)> columns = [];

			// Obtiene las columnas
			foreach (ImportDatabaseFieldViewModel field in ListFields)
				if (field.Checked)
				{
					string? selected = field.GetFileField();

						if (!string.IsNullOrWhiteSpace(selected))
							columns.Add((field.Column, selected));
				}
			// Devuelve las columnas
			return columns;
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public DbStudioViewModel SolutionViewModel { get; }

	/// <summary>
	///		Conexión
	/// </summary>
	public ConnectionModel Connection { get; }

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string? FileName
	{
		get { return _fileName; }
		set 
		{ 
			if (CheckProperty(ref _fileName, value))
			{
				// Carga los campos del archivo
				Task.Run(async () => await LoadFileFieldsAsync(CancellationToken.None));
				// Indica si es un archivo de tipo CSV
				IsCsvFile = (FileName ?? string.Empty).EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase);
			}
		}
	}

	/// <summary>
	///		Arbol de una conexión
	/// </summary>
	public ComboViewModel ComboTables
	{
		get { return _comboTables; }
		set { CheckObject(ref _comboTables, value); }
	}

	/// <summary>
	///		Lista de campos
	/// </summary>
	public BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<ImportDatabaseFieldViewModel> ListFields
	{
		get { return _listFields; }
		set { CheckObject(ref _listFields, value); }
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
	///		Tamaño del bloque de escritura
	/// </summary>
	public long BlockSize
	{
		get { return _blockSize; }
		set { CheckProperty(ref _blockSize, value); }
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