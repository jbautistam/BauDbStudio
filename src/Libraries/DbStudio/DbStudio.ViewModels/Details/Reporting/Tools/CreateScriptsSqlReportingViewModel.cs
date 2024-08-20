namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Tools;

/// <summary>
///		ViewModel de creación de archivos de scripts SQL a partir de un archivo de reporting
/// </summary>
public class CreateScriptsSqlReportingViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _schemaFileName = string.Empty, _outputFileName = string.Empty, _maskFiles = string.Empty, _maskSqlFiles = string.Empty;

	public CreateScriptsSqlReportingViewModel(DbStudioViewModel solutionViewModel)
	{
		// Inicializa las propiedades
		SolutionViewModel = solutionViewModel;
		// Inicializa el viewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel()
	{
		// Asigna las propiedades
		MaskFiles = "Archivos XML (*.xml)|*.xml|Todos los archivos (*.*)|*.*";
		MaskSqlFiles = "Archivos SQL (*.sql)|*.sql|Todos los archivos (*.*)|*.*";
		OutputFileName = System.IO.Path.Combine(SolutionViewModel.MainController.DialogsController.LastPathSelected, "ScriptReporting.sql");
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
			if (string.IsNullOrWhiteSpace(SchemaFileName) || !File.Exists(SchemaFileName))
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione el archivo de esquema");
			else if (string.IsNullOrWhiteSpace(OutputFileName))
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione el nombre de archivo");
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
	///		Nombre del archivo de esquema
	/// </summary>
	public string SchemaFileName
	{
		get { return _schemaFileName; }
		set { CheckProperty(ref _schemaFileName, value); }
	}

	/// <summary>
	///		Nombre del archivo de salida
	/// </summary>
	public string OutputFileName
	{
		get { return _outputFileName; }
		set { CheckProperty(ref _outputFileName, value); }
	}

	/// <summary>
	///		Máscara de archivos
	/// </summary>
	public string MaskFiles
	{
		get { return _maskFiles; }
		set { CheckProperty(ref _maskFiles, value); }
	}

	/// <summary>
	///		Máscara de archivos SQL
	/// </summary>
	public string MaskSqlFiles
	{
		get { return _maskSqlFiles; }
		set { CheckProperty(ref _maskSqlFiles, value); }
	}
}