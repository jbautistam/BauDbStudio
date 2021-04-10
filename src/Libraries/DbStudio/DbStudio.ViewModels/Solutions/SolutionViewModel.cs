using System;
using System.Threading.Tasks;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Application.Controllers.EtlProjects;
using Bau.Libraries.DbStudio.Models;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.DbStudio.Application;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions
{
	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public class SolutionViewModel : BaseObservableObject
	{
		// Variables privadas
		private Explorers.Connections.TreeConnectionsViewModel _treeConnectionsViewModel;
		private Explorers.Files.TreeFilesViewModel _treeFoldersViewModel;
		private Explorers.Cloud.TreeStorageViewModel _treeStoragesViewModel;
		private Details.Connections.ConnectionExecutionViewModel _connectionsViewModel;

		public SolutionViewModel(MainViewModel mainViewModel)
		{
			// Asigna las propiedades
			MainViewModel = mainViewModel;
			Manager = new SolutionManager(MainViewModel.MainController.Logger);
			// Asigna las soluciones hija
			ReportingSolutionViewModel = new Details.Reporting.ReportingSolutionViewModel(this);
			// Asigna los árboles de exploración
			TreeConnectionsViewModel = new Explorers.Connections.TreeConnectionsViewModel(this);
			TreeFoldersViewModel = new Explorers.Files.TreeFilesViewModel(this);
			TreeStoragesViewModel = new Explorers.Cloud.TreeStorageViewModel(this);
			ConnectionExecutionViewModel = new Details.Connections.ConnectionExecutionViewModel(this);
			// Asigna los comandos
			CreateTestXmlCommand = new BaseCommand(async _ => await CreateTestXmlAsync());
			CreateValidationScriptsCommand = new BaseCommand(async _ => await CreateValidationScriptsAsync());
			CreateImportFilesScriptsCommand = new BaseCommand(async _ => await CreateImportFilesScriptsAsync());
			CreateSchemaXmlCommand  = new BaseCommand(async _ => await CreateSchemaXmlAsync());
			CreateSchemaReportingXmlCommand = new BaseCommand(_ => CreateSchemaReportingXml());
			CreateSchemaReportingSqlCommand = new BaseCommand(_ => CreateSchemaReportingSql());
		}

		/// <summary>
		///		Carga un archivo de solución
		/// </summary>
		internal void Load(string path)
		{
			// Carga la solución
			Solution = Manager.LoadConfiguration(GetSolutionFileName(path, "DbStudio"));
			// Carga los exploradores
			TreeConnectionsViewModel.Load();
			ConnectionExecutionViewModel.Load();
			TreeFoldersViewModel.Load();
			TreeStoragesViewModel.Load();
			// Carga la solución de informes
			if (!string.IsNullOrWhiteSpace(Solution.FileName))
				ReportingSolutionViewModel.Load(GetSolutionFileName(path, "Reporting"));
			// Carga las carpetas en la ventana de búsqueda
			MainViewModel.SearchFilesViewModel.LoadFolders();
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
		private string GetSolutionFileName(string path, string project)
		{
			return System.IO.Path.Combine(path, $"{project}.xml");
		}

		/// <summary>
		///		Abre la ventana de creación de archivos de pruebas
		/// </summary>
		private async Task CreateTestXmlAsync()
		{
			Details.EtlProjects.CreateTestXmlViewModel viewModel = new Details.EtlProjects.CreateTestXmlViewModel(this);

				if (MainViewModel.MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
					using (BlockLogModel block = MainViewModel.MainController.Logger.Default.CreateBlock(LogModel.LogType.Info, "Comienzo de la creación de proyectos de pruebas"))
					{
						XmlTestProjectGenerator generator = new XmlTestProjectGenerator(Manager, viewModel.ComboConnections.GetSelectedConnection(),
																						viewModel.DataBase, viewModel.OutputPath);

							// Genera los archivos
							try
							{
								if (!await generator.GenerateAsync(block, viewModel.Provider, viewModel.PathVariable, viewModel.DataBaseVariable, viewModel.SufixTestTables,
																   viewModel.FileNameTest, viewModel.FileNameAssert, 
																   System.Threading.CancellationToken.None))
									block.Error($"Error en la generación de los archivos de pruebas. {generator.Errors.Concatenate()}");
								else
								{
									block.Info("Fin de la creación de proyectos de pruebas");
									MainViewModel.MainController.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
																				  "Generación de proyectos XML",
																				  "Ha terminado correctamente la generación del archivo de pruebas");
								}
							}
							catch (Exception exception)
							{
								block.Error($"Error en la generación de los archivos de pruebas {exception.Message}");
							}
							// Log
							MainViewModel.MainController.Logger.Flush();
					}
		}

		/// <summary>
		///		Crea los scripts de validación de archivos
		/// </summary>
		private async Task CreateValidationScriptsAsync()
		{
			Details.EtlProjects.CreateValidationScriptsViewModel viewModel = new Details.EtlProjects.CreateValidationScriptsViewModel(this);

				if (MainViewModel.MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
					using (BlockLogModel block = MainViewModel.MainController.Logger.Default.CreateBlock(LogModel.LogType.Info, "Comienzo de la creación de archivos de validación"))
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
						ScriptsValidationGenerator generator = new ScriptsValidationGenerator(Manager, options);

							// Crea los archivos de prueba
							try
							{
								if (!await generator.GenerateAsync(System.Threading.CancellationToken.None))
									block.Error($"Error en la generación de los archivos de validación. {generator.Errors.Concatenate()}");
								else
								{
									block.Info("Fin de la creación de archivos de validación");
									MainViewModel.MainController.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
																				  "Generación de archivos de validación",
																				  "Ha terminado correctamente la generación de los archivos de validación");
								}
							}
							catch (Exception exception)
							{
								block.Error($"Error en la generación de archivos de validación {exception.Message}");
							}
							// Log
							MainViewModel.MainController.Logger.Flush();
					}
		}

		/// <summary>
		///		Crea los scripts de importación de archivos
		/// </summary>
		private async Task CreateImportFilesScriptsAsync()
		{
			Details.EtlProjects.CreateImportFilesScriptViewModel viewModel = new Details.EtlProjects.CreateImportFilesScriptViewModel(this);

				if (MainViewModel.MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
					using (BlockLogModel block = MainViewModel.MainController.Logger.Default.CreateBlock(LogModel.LogType.Info, "Comienzo de la creación de archivos de importación"))
					{
						ScriptsImportOptions options = new ScriptsImportOptions
																	{
																		Connection = viewModel.ComboConnections.GetSelectedConnection(),
																		DataBaseVariable = viewModel.DataBaseVariable,
																		PrefixOutputTable = viewModel.PrefixOutputTable,
																		MountPathVariable = viewModel.MountPathVariable,
																		SubPath = viewModel.SubPath,
																		PathInputFiles = viewModel.PathInputFiles,
																		OutputFileName = viewModel.OutputFileName
																	};
						ScriptsImportGenerator generator = new ScriptsImportGenerator(Manager, options);

							// Crea los archivos de prueba
							try
							{
								if (!await generator.GenerateAsync(System.Threading.CancellationToken.None))
									block.Error($"Error en la generación de los archivos de importación. {generator.Errors.Concatenate()}");
								else
								{
									block.Info("Fin de la creación de archivos de importación");
									MainViewModel.MainController.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
																				  "Generación de archivos de importación",
																				  "Ha terminado correctamente la generación de los archivos de importación");
								}
							}
							catch (Exception exception)
							{
								block.Error($"Error en la generación de archivos de importación {exception.Message}");
							}
							// Log
							MainViewModel.MainController.Logger.Flush();
					}
		}

		/// <summary>
		///		Crea los archivos XML de un esquema
		/// </summary>
		private async Task CreateSchemaXmlAsync()
		{
			Details.EtlProjects.CreateSchemaXmlViewModel viewModel = new Details.EtlProjects.CreateSchemaXmlViewModel(this);

				if (MainViewModel.MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
					using (BlockLogModel block = MainViewModel.MainController.Logger.Default.CreateBlock(LogModel.LogType.Info, "Comienzo de la creación de archivos de esquema"))
					{
						// Crea los archivos de esquema
						try
						{
							// Crea los archivos
							await new Application.Controllers.Schema.SchemaManager(Manager).SaveAsync(viewModel.ComboConnections.GetSelectedConnection(), viewModel.OutputFileName);
							// Log
							block.Info("Fin de la creación de archivos de esquema");
							MainViewModel.MainController.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
																		  "Generación de archivos de esquema",
																		  "Ha terminado correctamente la generación de los archivos de esquema");
						}
						catch (Exception exception)
						{
							block.Error($"Error en la generación de archivos de esqu{exception.Message}");
						}
						// Log
						MainViewModel.MainController.Logger.Flush();
					}
		}

		/// <summary>
		///		Crea los archivos XML de un esquema para reporting
		/// </summary>
		private void CreateSchemaReportingXml()
		{
			Details.Reporting.Tools.CreateSchemaReportingXmlViewModel viewModel = new Details.Reporting.Tools.CreateSchemaReportingXmlViewModel(this);

				if (MainViewModel.MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
					using (BlockLogModel block = MainViewModel.MainController.Logger.Default.CreateBlock(LogModel.LogType.Info, "Comienzo de la creación de archivos de informes"))
					{
						// Crea los archivos de esquema
						try
						{
							LibReporting.Solution.ReportingSolutionManager manager = new LibReporting.Solution.ReportingSolutionManager();

								// Graba el archivo
								manager.SaveDataWarehouse(manager.ConvertSchemaDbToDataWarehouse(viewModel.Name, viewModel.SchemaFileName), viewModel.OutputFileName);
								// Log
								block.Info("Fin de la creación de archivos de esquema para informes");
								MainViewModel.MainController.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
																			  "Generación de archivos de esquema para informes",
																			  "Ha terminado correctamente la generación de los archivos de esquema para informes");
						}
						catch (Exception exception)
						{
							block.Error($"Error en la generación de archivos de esquema. {exception.Message}");
						}
						// Log
						MainViewModel.MainController.Logger.Flush();
					}
		}

		/// <summary>
		///		Crea los archivos SQL de creación de un esquema para reporting sobre una base de datos
		/// </summary>
		private void CreateSchemaReportingSql()
		{
			Details.Reporting.Tools.CreateScriptsSqlReportingViewModel viewModel = new Details.Reporting.Tools.CreateScriptsSqlReportingViewModel(this);

				if (MainViewModel.MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
					using (BlockLogModel block = MainViewModel.MainController.Logger.Default.CreateBlock(LogModel.LogType.Info, "Comienzo de la creación de scripts SQL de reporting"))
					{
						// Crea los archivos de esquema
						try
						{
							LibReporting.Solution.ReportingSolutionManager manager = new LibReporting.Solution.ReportingSolutionManager();

								// Graba el archivo
								manager.ConvertSchemaReportingToSql(viewModel.SchemaFileName, viewModel.OutputFileName);
								// Log
								block.Info("Fin de la creación de archivos de scripts SQL para informes");
								MainViewModel.MainController.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
																			  "Generación de archivos SQL para informes",
																			  "Ha terminado correctamente la generación de los archivos SQL de esquema para informes");
						}
						catch (Exception exception)
						{
							block.Error($"Error en la generación de archivos de esquema. {exception.Message}");
						}
						// Log
						MainViewModel.MainController.Logger.Flush();
					}
		}

		/// <summary>
		///		ViewModel de la ventana principal
		/// </summary>
		public MainViewModel MainViewModel { get; }

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionModel Solution { get; private set; }

		/// <summary>
		///		Manager de solución
		/// </summary>
		internal SolutionManager Manager { get; }

		/// <summary>
		///		ViewModel para la solución de reporting
		/// </summary>
		public Details.Reporting.ReportingSolutionViewModel ReportingSolutionViewModel { get; }

		/// <summary>
		///		ViewModel del árbol de conexiones
		/// </summary>
		public Explorers.Connections.TreeConnectionsViewModel TreeConnectionsViewModel
		{
			get { return _treeConnectionsViewModel; }
			set { CheckObject(ref _treeConnectionsViewModel, value); }
		}

		/// <summary>
		///		ViewModel del árbol de carpetas
		/// </summary>
		public Explorers.Files.TreeFilesViewModel TreeFoldersViewModel
		{
			get { return _treeFoldersViewModel; }
			set { CheckObject(ref _treeFoldersViewModel, value); }
		}

		/// <summary>
		///		ViewModel del árbol de storage
		/// </summary>
		public Explorers.Cloud.TreeStorageViewModel TreeStoragesViewModel
		{
			get { return _treeStoragesViewModel; }
			set { CheckObject(ref _treeStoragesViewModel, value); }
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

		/// <summary>
		///		Crea los archivos XML de un esquema de base de datos
		/// </summary>
		public BaseCommand CreateSchemaXmlCommand { get; }

		/// <summary>
		///		Crea los archivos XML de reporting a partir de un esquema de base de datos
		/// </summary>
		public BaseCommand CreateSchemaReportingXmlCommand { get; }

		/// <summary>
		///		Crea los scripts SQL de una base de datos de reporting a partir de un esquema XML de reporting
		/// </summary>
		public BaseCommand CreateSchemaReportingSqlCommand { get; }
	}
}