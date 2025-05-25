using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers;

/// <summary>
///		ViewModel para el árbol de almacenes de datos
/// </summary>
public class TreeReportingViewModel : PluginTreeViewModel, IPaneViewModel
{
	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public enum NodeType
	{
		/// <summary>Desconocido. No se debería utilizar</summary>
		Unknown,
		/// <summary>Almacén de datos</summary>
		DataWarehouse,
		/// <summary>Raíz de origen de datos</summary>
		DataSourcesRoot,
		/// <summary>Raíz de esquemas de origen de datos</summary>
		DataSourceSchemasRoot,
		/// <summary>Origen de datos</summary>
		DataSource,
		/// <summary>Tabla</summary>
		Table,
		/// <summary>Campo</summary>
		Field,
		/// <summary>Raíz de dimensiones</summary>
		DimensionsRoot,
		/// <summary>Dimensión</summary>
		Dimension,
		/// <summary>Raíz de informes</summary>
		ReportsRoot,
		/// <summary>Informe</summary>
		Report,
		/// <summary>Nodo con error</summary>
		Error
	}
	/// <summary>
	///		Tipo de icono
	/// </summary>
	public enum IconType
	{
		Unknown,
		DataWarehouse,
		DataSourceRoot,
		DataSourceTable,
		DataSourceView,
		Key,
		Field,
		Error,
		Report,
		DataSourceSql,
		Dimension,
		DimensionChild,
		Folder
	}

	public TreeReportingViewModel(ReportingSolutionViewModel reportingSolutionViewModel)
	{
		ReportingSolutionViewModel = reportingSolutionViewModel;
		NewDataWarehouseCommand = new BaseCommand(_ => NewDataWarehouse(), _ => CanExecuteAction(nameof(NewDataWarehouseCommand)))
									.AddListener(this, nameof(SelectedNode));
		NewDataWarehouseFromFileCommand = new BaseCommand(_ => NewDataWarehouseFromFile(), _ => CanExecuteAction(nameof(NewDataWarehouseFromFileCommand)))
												.AddListener(this, nameof(SelectedNode));
		NewDataSourceCommand = new BaseCommand(_ => OpenDataSource(null), _ => CanExecuteAction(nameof(NewDataSourceCommand)))
									.AddListener(this, nameof(SelectedNode));
		NewDimensionCommand = new BaseCommand(_ => OpenDimension(null), _ => CanExecuteAction(nameof(NewDimensionCommand)))
									.AddListener(this, nameof(SelectedNode));
		NewReportCommand = new BaseCommand(_ => OpenReport(null), _ => CanExecuteAction(nameof(NewReportCommand)))
									.AddListener(this, nameof(SelectedNode));
		QueryCommand = new BaseCommand(_ => OpenQuery(), _ => CanExecuteAction(nameof(QueryCommand)))
									.AddListener(this, nameof(SelectedNode));
		OpenExplorerCommand = new BaseCommand(_ => OpenExplorer(), _ => CanExecuteAction(nameof(OpenExplorerCommand)))
									.AddListener(this, nameof(SelectedNode));
		OpenXmlCommand = new BaseCommand(_ => OpenXmlEditor(), _ => CanExecuteAction(nameof(OpenXmlCommand)))
									.AddListener(this, nameof(SelectedNode));
		MergeSchemaCommand = new BaseCommand(_ => MergeSchema(), _ => CanExecuteAction(nameof(MergeSchemaCommand)))
									.AddListener(this, nameof(SelectedNode));
	}

	/// <summary>
	///		Abre el archivo en el explorador
	/// </summary>
	private void OpenExplorer()
	{
		if (SelectedNode is NodeDataWarehouseViewModel node)
		{
			string fileName = ReportingSolutionViewModel.ReportingSolutionManager.ReportingSolution.GetFileName(node.DataWarehouse);

				if (!File.Exists(fileName))
					ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage($"Can't find the file {fileName}");
				else
					ReportingSolutionViewModel.SolutionViewModel.MainController.MainWindowController.OpenExplorer(Path.GetDirectoryName(fileName) ?? string.Empty);
		}
	}

	/// <summary>
	///		Abre el archivo Xml en el editor
	/// </summary>
	private void OpenXmlEditor()
	{
		string? fileName = null;

			// Obtiene el nombre de archivo
			switch (SelectedNode)
			{
				case NodeDataWarehouseViewModel node:
						fileName = ReportingSolutionViewModel.ReportingSolutionManager.ReportingSolution.GetFileName(node.DataWarehouse);
					break;
				case NodeReportViewModel node:
						fileName = node.Report.FileName;
					break;
			}
			// Abre el archivo
			if (!string.IsNullOrEmpty(fileName))
			{
				if (!File.Exists(fileName))
					ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage($"Can't find the file {fileName}");
				else
					ReportingSolutionViewModel.SolutionViewModel.MainController.PluginController.HostPluginsController.OpenFile(fileName);
			}
	}

	/// <summary>
	///		Carga los nodos hijo
	/// </summary>
	protected override void AddRootNodes()
	{
		foreach (DataWarehouseModel dataWarehouse in ReportingSolutionViewModel.ReportingSolutionManager.Manager.Schema.DataWarehouses.EnumerateValuesSorted())
			Children.Add(new NodeDataWarehouseViewModel(this, null, dataWarehouse));
	}

	/// <summary>
	///		Comprueba si se puede ejecutar una acción
	/// </summary>
	protected override bool CanExecuteAction(string action)
	{
		return action switch
			{
				nameof(NewDataWarehouseCommand) or nameof(NewDataWarehouseFromFileCommand) => true,
				nameof(NewDataSourceCommand) or nameof(NewReportCommand)
						=> ReportingSolutionViewModel.ReportingSolutionManager.Manager.Schema.DataWarehouses.Count > 0,
				nameof(NewDimensionCommand) => SelectedNode is NodeDataSourceViewModel || SelectedNode is NodeDimensionViewModel,
				nameof(OpenCommand) => SelectedNode is NodeDimensionViewModel || SelectedNode is NodeDataSourceViewModel || SelectedNode is NodeReportViewModel,
				nameof(QueryCommand) => SelectedNode is NodeDataSourceViewModel || SelectedNode is NodeReportViewModel,
				nameof(DeleteCommand) => SelectedNode is NodeDataWarehouseViewModel || SelectedNode is NodeReportViewModel || SelectedNode is NodeDimensionViewModel ||
										   (SelectedNode is NodeDataSourceViewModel nodeDataSource && nodeDataSource.DataSource is DataSourceSqlModel),
				nameof(OpenXmlCommand) => SelectedNode is NodeDataWarehouseViewModel || SelectedNode is NodeReportViewModel,
				nameof(OpenExplorerCommand) or nameof(MergeSchemaCommand) => SelectedNode is NodeDataWarehouseViewModel,
				_ => false,
			};
	}

	/// <summary>
	///		Abre la ventana de propiedades de un nodo
	/// </summary>
	protected override void OpenProperties()
	{
		switch (SelectedNode)
		{
			case NodeDimensionViewModel node:
					OpenDimension(node);
				break;
			case NodeDataSourceViewModel node:
					OpenDataSource(node);
				break;
			case NodeReportViewModel node:
					OpenReport(node);
				break;
		}
	}

	/// <summary>
	///		Crea un nuevo almacén de datos
	/// </summary>
	private void NewDataWarehouse()
	{
		string? fileName = ReportingSolutionViewModel.SolutionViewModel.MainController.DialogsController
									.OpenDialogLoad(string.Empty,
													"Archivo de esquema (*.xml)|*.xml|Todos los archivos (*.*)|*.*");

			if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
				AddDataWarehouseToSolution(fileName);
	}

	/// <summary>
	///		Crea un nuevo dataWarehouse a partir de un archivo XML de esquema de conexión
	/// </summary>
	private void NewDataWarehouseFromFile()
	{
		string? schemaFile = ReportingSolutionViewModel.SolutionViewModel.MainController.DialogsController
								.OpenDialogLoad(string.Empty, "Archivos de esquema (*.xml)|*.xml|Todos los archivos (*.*)|*.*");

			if (!string.IsNullOrWhiteSpace(schemaFile) && File.Exists(schemaFile))
			{
				string fileName = Path.Combine(Path.GetDirectoryName(schemaFile) ?? string.Empty,
											   $"Reporting-{Path.GetFileNameWithoutExtension(schemaFile)}.xml");

					if (File.Exists(fileName))
						ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController
								.ShowMessage($"Ya existe un archivo de reporting llamado {fileName}");
					else
						try
						{
							DataWarehouseModel dataWarehouse = ReportingSolutionViewModel.ReportingSolutionManager
																	.ConvertSchemaDbToDataWarehouse(Path.GetFileNameWithoutExtension(schemaFile),
																									schemaFile);

								// Graba los datos del almacén en un archivo
								ReportingSolutionViewModel.ReportingSolutionManager.SaveDataWarehouse(dataWarehouse, fileName);
								// Añade el almacén de datos a la colección
								AddDataWarehouseToSolution(fileName);
								// Mensaje
								ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController
										.ShowMessage("Se ha añadido correctamente el almacén de datos a la solución");
						}
						catch (Exception exception)
						{
							ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController
									.ShowMessage($"Error al crear el esquema de reporting {exception.Message}");
						}
			}
	}

	/// <summary>
	///		Añade un almacén de datos a la solución
	/// </summary>
	private void AddDataWarehouseToSolution(string fileName)
	{
		// Añade un almacén de datos a la solución
		ReportingSolutionViewModel.ReportingSolutionManager.AddDataWarehouse(fileName);
		// Graba la solución y actualiza el árbol
		SaveSolution();
	}

	/// <summary>
	///		Combina el esquema
	/// </summary>
	private void MergeSchema()
	{
		if (SelectedNode is NodeDataWarehouseViewModel node)
		{
			string dataWarehouseFileName = ReportingSolutionViewModel.ReportingSolutionManager.ReportingSolution.GetFileName(node.DataWarehouse);

				if (!File.Exists(dataWarehouseFileName))
					ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage($"Can't find the file {dataWarehouseFileName}");
				else
				{
					string? schemaFileName = ReportingSolutionViewModel.SolutionViewModel.MainController.DialogsController
												.OpenDialogLoad(string.Empty,
																"Archivo de esquema (*.xml)|*.xml|Todos los archivos (*.*)|*.*");

						if (!string.IsNullOrWhiteSpace(schemaFileName) && File.Exists(schemaFileName))
							try
							{
								// Mezcla el datawarehouse con el archivo de esquema
								ReportingSolutionViewModel.ReportingSolutionManager.Merge(node.DataWarehouse, schemaFileName);
								// Graba la solución y actualiza el árbol
								ReportingSolutionViewModel.ReportingSolutionManager.SaveDataWarehouse(node.DataWarehouse, dataWarehouseFileName);
								SaveSolution();
								// Mensaje
								ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController
									.ShowMessage($"Final de la actualización de {dataWarehouseFileName} con {schemaFileName}");
							}
							catch (Exception exception)
							{
								ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController
									.ShowMessage($"Error al actualizar {node.DataWarehouse.Name} con {schemaFileName}. {exception.Message}");
							}
				}
		}
	}

	/// <summary>
	///		Abre un origen de datos
	/// </summary>
	private void OpenDataSource(NodeDataSourceViewModel? node)
	{
		if (node == null || node.DataSource is DataSourceSqlModel)
		{
			DataSourceSqlModel? dataSource;

				// Genera el origen de datos
				if (node is not null && node.DataSource is DataSourceSqlModel dataSourceSql)
					dataSource = dataSourceSql;
				else
					dataSource = new DataSourceSqlModel(GetSelectedDataWarehouse(SelectedNode));
				// Abre la vista
				ReportingSolutionViewModel.SolutionViewModel.MainController.OpenWindow(new DataSources.DataSourceSqlViewModel(ReportingSolutionViewModel, dataSource));
		}
		else if (node.DataSource is DataSourceTableModel dataSourceTable)
			ReportingSolutionViewModel.SolutionViewModel.MainController.OpenWindow(new DataSources.DataSourceTableViewModel(ReportingSolutionViewModel, dataSourceTable));
	}

	/// <summary>
	///		Obtiene el <see cref="DataWarehouseModel"/> seleccionado en el árbol buscando los padres
	/// </summary>
	private DataWarehouseModel? GetSelectedDataWarehouse(ControlHierarchicalViewModel? node)
	{
		if (node is NodeDataWarehouseViewModel nodeDataWarehouse)
			return nodeDataWarehouse.DataWarehouse;
		else if (node?.Parent != null)
			return GetSelectedDataWarehouse(node.Parent);
		else
			return null;
	}

	/// <summary>
	///		Abre el formulario de detalles de una dimensión
	/// </summary>
	private void OpenDimension(NodeDimensionViewModel? node)
	{
		bool isNew = false;
		LibReporting.Models.DataWarehouses.Dimensions.BaseDimensionModel? baseDimension = null;

			// Si no se le ha pasado un nodo, si estamos en un nodo de origen de datos, creamos una dimensión a partir del nodo
			if (node is not null)
				baseDimension = node.Dimension;
			else 
			{
				if (SelectedNode is NodeDataSourceViewModel nodeDataSource)
				{
					DataWarehouseModel? dataWarehouse = GetSelectedDataWarehouse(nodeDataSource);

						if (dataWarehouse is not null)
						{
							baseDimension = new LibReporting.Models.DataWarehouses.Dimensions.DimensionModel(dataWarehouse, nodeDataSource.DataSource);
							isNew = true;
						}
				}
				else if (SelectedNode is NodeDimensionViewModel nodeDimension)
				{
					DataWarehouseModel? dataWarehouse = GetSelectedDataWarehouse(nodeDimension);

						if (dataWarehouse is not null)
						{
							baseDimension = new LibReporting.Models.DataWarehouses.Dimensions.DimensionChildModel(dataWarehouse, string.Empty, 
																												  nodeDimension.Id, string.Empty);
							isNew = true;
						}				
				}
			}
			// Abre el formulario
			switch (baseDimension)
			{
				case null:
						ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Can't find dimension");
					break;
				case LibReporting.Models.DataWarehouses.Dimensions.DimensionModel dimension:
						ReportingSolutionViewModel.SolutionViewModel.MainController.OpenWindow(new Dimension.DimensionViewModel(ReportingSolutionViewModel, dimension, isNew));
					break;
				case LibReporting.Models.DataWarehouses.Dimensions.DimensionChildModel dimension:
						if (ReportingSolutionViewModel.SolutionViewModel.MainController.OpenDialog(new Dimension.DimensionChildViewModel(ReportingSolutionViewModel, dimension, isNew))
								== BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
							Load();
					break;
			}
	}

	/// <summary>
	///		Abre un informe
	/// </summary>
	private void OpenReport(NodeReportViewModel? node)
	{
		if (node is not null)
			ReportingSolutionViewModel.SolutionViewModel.MainController.HostPluginsController.OpenFile(node.Report.FileName);
		else
		{
			DataWarehouseModel? dataWarehouse = GetSelectedDataWarehouse(SelectedNode);

				if (dataWarehouse is null)
					ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("You must select a datawarehouse");
				else
				{
					string dataWarehouseFile = ReportingSolutionViewModel.ReportingSolutionManager.ReportingSolution.GetFileName(dataWarehouse);

						if (string.IsNullOrWhiteSpace(dataWarehouseFile) || !File.Exists(dataWarehouseFile))
							ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Can't find the datawarehouse file name");
						else
						{
							string path = CreateReportFolder(Path.GetDirectoryName(dataWarehouseFile)!);
							string? fileName = ReportingSolutionViewModel.SolutionViewModel.MainController.DialogsController.OpenDialogSave
													(path, "Report file xml (*.report.xml)|*.report.xml|All files (*.*)|*.*",
													 "New report.report.xml", ".report.xml");

								if (!string.IsNullOrWhiteSpace(fileName))
								{
									// Graba el archivo
									File.WriteAllText(fileName, 
																$@"<?xml version=""1.0"" encoding=""utf-8""?>
																		<Report>
																			<DataWarehouse Name = ""{dataWarehouse.Name}""/>
																			<Name>{Path.GetFileName(fileName)}</Name>
																			<Description>
																			</Description>
																		</Report>
																	".Replace("\t", string.Empty).Trim()
																);
									// y lo abre en el editor
									ReportingSolutionViewModel.SolutionViewModel.MainController.HostPluginsController.OpenFile(fileName);
								}
						}
				}
		}

		// Crea el directorio de informes
		string CreateReportFolder(string folder)
		{
			// Añade el subdirectorio
			folder = Path.Combine(folder, "Reports");
			// Crea la carpeta si no existía
			LibHelper.Files.HelperFiles.MakePath(folder);
			// Devuelve la carpeta creada
			return folder;
		}
	}

	/// <summary>
	///		Abre la ventana de consulta
	/// </summary>
	private void OpenQuery()
	{
		switch (SelectedNode)
		{
			case NodeReportViewModel node:
					ReportingSolutionViewModel.SolutionViewModel.MainController.OpenWindow(new Queries.ReportQueryViewModel(ReportingSolutionViewModel, node.Report));
				break;
			case NodeDataSourceViewModel node:
					OpenQueryDataSource(node.DataSource);
				break;
		}
	}

	/// <summary>
	///		Graba los datos de un objeto como un archivo Json
	/// </summary>
	private void SaveJson<TypeData>(TypeData domain) where TypeData : LibReporting.Models.Base.BaseReportingModel
	{
		string? fileName = ReportingSolutionViewModel.SolutionViewModel.MainController.DialogsController.OpenDialogSave
								(string.Empty, 
								 "Archivos Json (*.json)|*.json|Todos los archivos (*.*)|*.*",
								 domain.Id + ".json", ".json");

			if (!string.IsNullOrWhiteSpace(fileName))
			{
				// Graba el archivo
				File.WriteAllText(fileName, Newtonsoft.Json.JsonConvert.SerializeObject(domain, Newtonsoft.Json.Formatting.Indented));
				// y lo abre en el editor
				ReportingSolutionViewModel.SolutionViewModel.MainController.HostPluginsController.OpenFile(fileName);
			}
	}

	/// <summary>
	///		Borra el elemento seleccionado
	/// </summary>
	protected override void DeleteItem()
	{
		switch (SelectedNode?.Tag)
		{
			case DataWarehouseModel item:
					DeleteDataWarehouse(item);
				break;
			case LibReporting.Models.DataWarehouses.Dimensions.BaseDimensionModel item:
					DeleteDimension(item);
				break;
			case DataSourceSqlModel item:
					DeleteDataSourceSql(item);
				break;
			case LibReporting.Models.DataWarehouses.Reports.ReportModel item:
					DeleteReport(item);
				break;
		}
	}

	/// <summary>
	///		Borra un almacén de datos
	/// </summary>
	private void DeleteDataWarehouse(DataWarehouseModel dataWarehouse)
	{
		if (ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowQuestion($"¿Realmente desea borrar los datos del almacén de datos {dataWarehouse.Name}?"))
		{
			// Borra el almacén de datos
			ReportingSolutionViewModel.ReportingSolutionManager.RemoveDataWarehouse(dataWarehouse);
			// Graba la solución y actualiza el árbol
			SaveSolution();
		}
	}

	/// <summary>
	///		Borra un origen de datos de SQL
	/// </summary>
	private void DeleteDataSourceSql(DataSourceSqlModel dataSource)
	{
		if (ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowQuestion($"¿Realmente desea borrar los datos del origen de datos {dataSource.Id}?"))
		{
			// Borra el origen de datos
			dataSource.DataWarehouse.DataSources.Remove(dataSource);
			// Graba la solución y actualiza el árbol
			SaveDataWarehouse(dataSource.DataWarehouse);
		}
	}

	/// <summary>
	///		Borra los datos de una dimensión
	/// </summary>
	private void DeleteDimension(LibReporting.Models.DataWarehouses.Dimensions.BaseDimensionModel dimension)
	{
		if (ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowQuestion($"¿Realmente desea borrar los datos de la dimensión {dimension.Id}?"))
		{
			// Borra la dimensión
			dimension.DataWarehouse.Dimensions.Remove(dimension);
			// Graba la solución y actualiza el árbol
			SaveDataWarehouse(dimension.DataWarehouse);
		}
	}

	/// <summary>
	///		Elimina un informe avanzado
	/// </summary>
	private void DeleteReport(LibReporting.Models.DataWarehouses.Reports.ReportModel report)
	{
		if (ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowQuestion($"¿Realmente desea borrar los datos del informe {report.Id}?"))
		{
			// Borra el informe
			LibHelper.Files.HelperFiles.KillFile(report.FileName, true);
			report.DataWarehouse.Reports.Remove(report);
			// Graba la solución y actualiza el árbol
			SaveDataWarehouse(report.DataWarehouse);
		}
	}

	/// <summary>
	///		Graba la solución y actualiza el árbol
	/// </summary>
	private void SaveSolution()
	{
		// Graba la solución
		ReportingSolutionViewModel.SaveSolution();
		// Actualiza el árbol
		Load();
	}

	/// <summary>
	///		Graba el archivo de esquema del dataWarehouse
	/// </summary>
	private void SaveDataWarehouse(DataWarehouseModel dataWarehouse)
	{
		// Graba la solución
		ReportingSolutionViewModel.SaveDataWarehouse(dataWarehouse);
		// Actualiza el árbol
		Load();
	}

	/// <summary>
	///		Abre una ventana de consulta de un origen de datos
	/// </summary>
	private void OpenQueryDataSource(BaseDataSourceModel dataSource)
	{
		string sql = string.Empty;

			// Obtiene la cadena SQL para la consulta
			if (dataSource is DataSourceSqlModel dataSourceSql)
				sql = dataSourceSql.Sql;
			else if (dataSource is DataSourceTableModel dataSourceTable)
				sql = GetSql(dataSourceTable);
			// Abre la consulta
			if (!string.IsNullOrWhiteSpace(sql))
				ReportingSolutionViewModel.SolutionViewModel.MainController
						.OpenWindow(new Details.Queries.ExecuteQueryViewModel(ReportingSolutionViewModel.SolutionViewModel, string.Empty, sql, false));
	}

	/// <summary>
	///		Obtiene la cadena SQL asociada a una tabla
	/// </summary>
	private string GetSql(DataSourceTableModel dataSourceTable)
	{
		string fields = string.Empty;
		int charsFromLastNewLine = 0, index = 0;

			// Añade las columnas
			foreach (DataSourceColumnModel column in dataSourceTable.Columns.EnumerateValuesSorted())
			{
				// Cuenta el número de caracteres
				charsFromLastNewLine += dataSourceTable.Table.Length + column.Id.Length + 2;
				// Añade el nombre de columna
				fields += $"[{dataSourceTable.Table}].[{column.Id}]";
				// Añade la coma si es necesario
				if (index++ < dataSourceTable.Columns.Count - 1)
					fields += ", ";
				// Añade un salto de línea si es necesario
				if (charsFromLastNewLine > 80)
				{
					// Añade el salto de línea
					fields += Environment.NewLine + "\t\t";
					// Inicializa el contador
					charsFromLastNewLine = 0;
				}
			}
			// Devuelve la cadena SQL
			return $"SELECT {fields} {Environment.NewLine}\tFROM [{dataSourceTable.Schema}].[{dataSourceTable.Table}]";
	}

	/// <summary>
	///		Ejecuta un comando externo: sólo implementa el interface
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		// No hace nada sólo implementa el interface
	}

	/// <summary>
	///		Cierra el panel: sólo implementa el interface
	/// </summary>
	public void Close()
	{
		// No hace nada sólo implementa el interface
	}

	/// <summary>
	///		ViewModel de la solución para reporting
	/// </summary>
	public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

	/// <summary>
	///		Cabecera del panel
	/// </summary>
	public string Header => "Reporting";

	/// <summary>
	///		Id del panel
	/// </summary>
	public string TabId => GetType().ToString();

	/// <summary>
	///		Comando de nuevo almacén de datos
	/// </summary>
	public BaseCommand NewDataWarehouseCommand { get; }

	/// <summary>
	///		Comando de creación de un nuevo almacén de datos a partir de un archivo de conexión
	/// </summary>
	public BaseCommand NewDataWarehouseFromFileCommand { get; }

	/// <summary>
	///		Comando de nueva dimensión
	/// </summary>
	public BaseCommand NewDimensionCommand { get; }

	/// <summary>
	///		Comando de nuevo origen de datos
	/// </summary>
	public BaseCommand NewDataSourceCommand { get; }

	/// <summary>
	///		Comando de nuevo informe
	/// </summary>
	public BaseCommand NewReportCommand { get; }

	/// <summary>
	///		Comando de consulta
	/// </summary>
	public BaseCommand QueryCommand { get; }

	/// <summary>
	///		Comando para abrir el archivo en el explorador
	/// </summary>
	public BaseCommand OpenExplorerCommand { get; }

	/// <summary>
	///		Comando para abrir el archivo XML en el editor
	/// </summary>
	public BaseCommand OpenXmlCommand { get; }

	/// <summary>
	///		Comando para combinar con un archivo de esquema
	/// </summary>
	public BaseCommand MergeSchemaCommand { get; }
}