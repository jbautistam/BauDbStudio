using Microsoft.Extensions.Logging;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Application;
using Bau.Libraries.DbStudio.Application.Controllers.EtlProjects;
using Bau.Libraries.DbStudio.Models;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.Libraries.DbStudio.ViewModels;

/// <summary>
///		ViewModel de la solución
/// </summary>
public class DbStudioViewModel : BaseObservableObject
{
	// Variables privadas
	private string _text = string.Empty;
	private Explorers.Connections.TreeConnectionsViewModel _treeConnectionsViewModel = default!;
	private Details.Connections.ConnectionExecutionViewModel _connectionsViewModel = default!;

	public DbStudioViewModel(string appName, Controllers.IDbStudioController mainController)
	{
		// Título de la aplicación
		Text = appName;
		// Asigna las propiedades
		MainController = mainController;
		// Asigna el manager de soluciones
		Manager = new SolutionManager(MainController.Logger);
		// Asigna los objetos
		ConfigurationViewModel = new Configuration.ConfigurationViewModel(this);
		ConfigurationViewModel.Load();
		// Asigna las soluciones hija
		ReportingSolutionViewModel = new Details.Reporting.ReportingSolutionViewModel(this);
		// Asigna los árboles de exploración
		TreeConnectionsViewModel = new Explorers.Connections.TreeConnectionsViewModel(this);
		ConnectionExecutionViewModel = new Details.Connections.ConnectionExecutionViewModel(this);
		// Asigna los comandos
		CreateTestXmlCommand = new BaseCommand(async _ => await CreateTestXmlAsync());
		CreateValidationScriptsCommand = new BaseCommand(async _ => await CreateValidationScriptsAsync());
		CreateImportFilesScriptsCommand = new BaseCommand(async _ => await CreateImportFilesScriptsAsync());
	}

	/// <summary>
	///		Carga un archivo de solución
	/// </summary>
	public void Load(string path)
	{
		// Guarda el directorio
		PathData = path;
		// Carga la solución
		Solution = Manager.LoadConfiguration(GetSolutionFileName(path, "DbStudio"));
		// Carga los exploradores
		TreeConnectionsViewModel.Load();
		ConnectionExecutionViewModel.Load();
		// Carga la solución de informes
		if (!string.IsNullOrWhiteSpace(Solution.FileName))
			ReportingSolutionViewModel.Load(GetSolutionFileName(path, "Reporting"));
	}

	/// <summary>
	///		Actualiza el árbol
	/// </summary>
	internal void Refresh()
	{
		Load(PathData);
	}

	/// <summary>
	///		Graba la solución
	/// </summary>
	internal void SaveSolution()
	{
		Save(PathData);
	}

	/// <summary>
	///		Graba la solución
	/// </summary>
	internal void Save(string path)
	{
		// Graba la solución
		Manager.SaveSolution(Solution, GetSolutionFileName(path, "DbStudio"));
		// Carga la solución de informes
		ReportingSolutionViewModel.SaveSolution();
	}

	/// <summary>
	///		Obtiene el nombre del archivo de solución
	/// </summary>
	private string GetSolutionFileName(string path, string project) => Path.Combine(path, $"{project}.xml");

	/// <summary>
	///		Abre la ventana de creación de archivos de pruebas
	/// </summary>
	private async Task CreateTestXmlAsync()
	{
		Details.EtlProjects.CreateTestXmlViewModel viewModel = new Details.EtlProjects.CreateTestXmlViewModel(this);

			if (MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				XmlTestProjectGenerator generator = new(Manager, viewModel.ComboConnections.GetSelectedConnection(), viewModel.DataBase, viewModel.OutputPath);

					// Log
					MainController.Logger.LogInformation("Comienzo de la creación de proyectos de pruebas");
					// Genera los archivos
					try
					{
						if (!await generator.GenerateAsync(viewModel.Provider, viewModel.PathVariable, viewModel.DataBaseVariable, viewModel.SufixTestTables,
															viewModel.FileNameTest, viewModel.FileNameAssert, 
															CancellationToken.None))
							MainController.Logger.LogError($"Error en la generación de los archivos de pruebas. {generator.Errors.Concatenate()}");
						else
						{
							MainController.Logger.LogInformation("Fin de la creación de proyectos de pruebas");
							MainController.MainWindowController
									.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
														"Generación de proyectos XML",
														"Ha terminado correctamente la generación del archivo de pruebas");
						}
					}
					catch (Exception exception)
					{
						MainController.Logger.LogInformation(exception, $"Error en la generación de los archivos de pruebas {exception.Message}");
					}
			}
	}

	/// <summary>
	///		Crea los scripts de validación de archivos
	/// </summary>
	private async Task CreateValidationScriptsAsync()
	{
		Details.EtlProjects.CreateValidationScriptsViewModel viewModel = new(this);

			if (MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				ScriptsValidationOptions options = new ScriptsValidationOptions
															{
																Connection = viewModel.TreeConnection.Connection,
																Tables = viewModel.TreeConnection.GetSelectedTables(),
																OutputPath = viewModel.OutputPath,
																DataBaseComputeVariable = viewModel.DataBaseComputeVariable,
																DataBaseValidateVariable = viewModel.DataBaseValidateVariable,
																Mode = viewModel.ValidateFiles ? ScriptsValidationOptions.ValidationMode.Files : ScriptsValidationOptions.ValidationMode.Database,
																MountPathVariable = viewModel.MountPathVariable,
																MountPathContent = viewModel.MountPathContent,
																FormatType = viewModel.FormatType,
																SubpathValidate = viewModel.PathValidate,
																DatabaseTarget = viewModel.DataBaseTarget,
																GenerateQvs = viewModel.GenerateQvs,
																TablePrefixes = viewModel.TablePrefixes,
																CompareString = viewModel.CompareString,
																DateFormat = viewModel.DateFormat,
																DecimalSeparator = viewModel.DecimalSeparator,
																DecimalType = viewModel.DecimalType,
																BitFields = viewModel.BitFields,
																CompareOnlyAlphaAndDigits = viewModel.CompareOnlyAlphaAndDigits
															};
				ScriptsValidationGenerator generator = new(Manager, options);

					// Log
					MainController.Logger.LogInformation("Comienzo de la creación de archivos de validación");
					// Crea los archivos de prueba
					try
					{
						if (!await generator.GenerateAsync(CancellationToken.None))
							MainController.Logger.LogError($"Error en la generación de los archivos de validación. {generator.Errors.Concatenate()}");
						else
						{
							MainController.Logger.LogInformation("Fin de la creación de archivos de validación");
							MainController.MainWindowController
									.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
														"Generación de archivos de validación",
														"Ha terminado correctamente la generación de los archivos de validación");
						}
					}
					catch (Exception exception)
					{
						MainController.Logger.LogError(exception, $"Error en la generación de archivos de validación {exception.Message}");
					}
			}
	}

	/// <summary>
	///		Crea los scripts de importación de archivos
	/// </summary>
	private async Task CreateImportFilesScriptsAsync()
	{
		Details.EtlProjects.CreateImportFilesScriptViewModel viewModel = new Details.EtlProjects.CreateImportFilesScriptViewModel(this);
		Models.Connections.ConnectionModel? connection = viewModel.ComboConnections.GetSelectedConnection();

			if (connection is null)
				MainController.MainWindowController.SystemController.ShowMessage("Seleccione una conexión");
			else if (MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				ScriptsImportOptions options = new ScriptsImportOptions
															{
																Connection = connection,
																DataBaseVariable = viewModel.DataBaseVariable,
																PrefixOutputTable = viewModel.PrefixOutputTable,
																MountPathVariable = viewModel.MountPathVariable,
																SubPath = viewModel.SubPath,
																PathInputFiles = viewModel.PathInputFiles,
																OutputFileName = viewModel.OutputFileName
															};
				ScriptsImportGenerator generator = new ScriptsImportGenerator(Manager, options);

					// Log
					MainController.Logger.LogInformation("Comienzo de la creación de archivos de importación");
					// Crea los archivos de prueba
					try
					{
						if (!await generator.GenerateAsync(CancellationToken.None))
							MainController.Logger.LogError($"Error en la generación de los archivos de importación. {generator.Errors.Concatenate()}");
						else
						{
							MainController.Logger.LogInformation("Fin de la creación de archivos de importación");
							MainController.MainWindowController
									.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
														"Generación de archivos de importación",
														"Ha terminado correctamente la generación de los archivos de importación");
						}
					}
					catch (Exception exception)
					{
						MainController.Logger.LogError(exception, $"Error en la generación de archivos de importación {exception.Message}");
					}
			}
	}

	/// <summary>
	///		Abre un archivo (si reconoce la extensión)
	/// </summary>
	public bool OpenFile(string fileName)
	{
		bool opened = false;

			if (!string.IsNullOrWhiteSpace(fileName))
			{
				IDetailViewModel? fileViewModel = GetFileViewModel(fileName);

					// Abre la ventana
					if (fileViewModel is not null)
					{
						if (fileViewModel is Details.Files.ScriptFileViewModel scriptFileViewModel)
							MainController.PluginController.HostPluginsController.OpenEditor(scriptFileViewModel);
						else
							MainController.OpenWindow(fileViewModel);
						opened = true;
					}
			}
			// Devuelve el valor que indica si se ha abierto el archivo
			return opened;
	}

	/// <summary>
	///		Obtiene el viewModel adecuado para un archivo
	/// </summary>
	private IDetailViewModel? GetFileViewModel(string fileName)
	{
		if (fileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase) ||
					fileName.EndsWith(".sqlx", StringComparison.CurrentCultureIgnoreCase))
			return new Details.Files.ScriptFileViewModel(this, fileName);
		else
			return null;
	}

	/// <summary>
	///		Solución
	/// </summary>
	public SolutionModel Solution { get; private set; } = default!;

	/// <summary>
	///		Manager de solución
	/// </summary>
	internal SolutionManager Manager { get; }

	/// <summary>
	///		ViewModel para la solución de reporting
	/// </summary>
	public Details.Reporting.ReportingSolutionViewModel ReportingSolutionViewModel { get; }

	/// <summary>
	///		Controlador principal
	/// </summary>
	public Controllers.IDbStudioController MainController { get; }

	/// <summary>
	///		Viewmodel de configuración
	/// </summary>
	public Configuration.ConfigurationViewModel ConfigurationViewModel { get; }

	/// <summary>
	///		Directorio de datos
	/// </summary>
	public string PathData { get; private set; } = default!;

	/// <summary>
	///		Título de la ventana
	/// </summary>
	public string Text 
	{
		get { return _text; }
		set { CheckProperty(ref _text, value); }
	}

	/// <summary>
	///		ViewModel del árbol de conexiones
	/// </summary>
	public Explorers.Connections.TreeConnectionsViewModel TreeConnectionsViewModel
	{
		get { return _treeConnectionsViewModel; }
		set { CheckObject(ref _treeConnectionsViewModel, value); }
	}

	/// <summary>
	///		ViewModel de los datos de conexiones
	/// </summary>
	public Details.Connections.ConnectionExecutionViewModel ConnectionExecutionViewModel
	{
		get { return _connectionsViewModel; }
		set { CheckObject(ref _connectionsViewModel, value); }
	}

	/// <summary>
	///		Crea los archivos XML de proyecto de pruebas
	/// </summary>
	public BaseCommand CreateTestXmlCommand { get; }

	/// <summary>
	///		Crea los archivos SQL de validación
	/// </summary>
	public BaseCommand CreateValidationScriptsCommand { get; }

	/// <summary>
	///		Crea los archivos SQL de importación de archivos a una base de datos
	/// </summary>
	public BaseCommand CreateImportFilesScriptsCommand { get; }
}