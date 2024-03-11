using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections;

/// <summary>
///		ViewModel para el árbol de conexiones
/// </summary>
public class TreeConnectionsViewModel : TreeSolutionBaseViewModel
{
	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public enum NodeType
	{
		/// <summary>Desconocido. No se debería utilizar</summary>
		Unknown,
		/// <summary>Raíz de la conexión</summary>
		ConnectionRoot,
		/// <summary>Conexión</summary>
		Connection,
		/// <summary>Esquema de una conexión</summary>
		SchemaRoot,
		/// <summary>Tabla</summary>
		Table,
		/// <summary>Raíz de archivos de proyecto</summary>
		FilesRoot,
		/// <summary>Mensaje (transitorio)</summary>
		Message,
		/// <summary>Almacén de datos</summary>
		DataWarehouse,
		/// <summary>Raíz de origen de datos</summary>
		DataSourcesRoot,
		/// <summary>Origen de datos</summary>
		DataSource,
		/// <summary>Raíz de dimensiones</summary>
		DimensionsRoot,
		/// <summary>Dimensión</summary>
		Dimension,
		/// <summary>Raíz de informes</summary>
		ReportsRoot,
		/// <summary>Informe</summary>
		Report
	}
	/// <summary>
	///		Tipo de icono
	/// </summary>
	public enum IconType
	{
		Unknown,
		Connection,
		Project,
		Schema,
		Table,
		View,
		Key,
		Field,
		Error,
		Loading
	}
	// Variables privadas
	private ConnectionModel? copiedConnection = null;

	public TreeConnectionsViewModel(DbStudioViewModel solutionViewModel) : base(solutionViewModel)
	{
		NewConnectionCommand = new BaseCommand(_ => OpenConnection(null), _ => CanExecuteAction(nameof(NewConnectionCommand)))
									.AddListener(this, nameof(SelectedNode));
		NewQueryCommand = new BaseCommand(parameter => OpenQuery(null));
		CopyCommand = new BaseCommand(_ => CopyConnection(), _ => CanExecuteAction(nameof(CopyCommand)))
									.AddListener(this, nameof(SelectedNode));
		PasteCommand = new BaseCommand(_ => PasteConnection(), _ => CanExecuteAction(nameof(PasteCommand)))
									.AddListener(this, nameof(SelectedNode));
		CreateSchemaXmlCommand = new BaseCommand(async _ => await CreateSchemaXmlAsync(CancellationToken.None), _ => CanExecuteAction(nameof(CreateSchemaXmlCommand)))
									.AddListener(this, nameof(SelectedNode));
		ExportDataCommand = new BaseCommand(async _ => await ExportDataAsync(CancellationToken.None), _ => CanExecuteAction(nameof(ExportDataCommand)))
									.AddListener(this, nameof(SelectedNode));
		ImportDataCommand = new BaseCommand(async _ => await ImportDataAsync(CancellationToken.None), _ => CanExecuteAction(nameof(ImportDataCommand)))
									.AddListener(this, nameof(SelectedNode));
	}

	/// <summary>
	///		Carga los nodos hijo
	/// </summary>
	protected override void AddRootNodes()
	{
		// Ordena las conexiones
		SolutionViewModel.Solution.Connections.SortByName();
		// Añade los nodos
		foreach (ConnectionModel connection in SolutionViewModel.Solution.Connections)
			Children.Add(new NodeConnectionViewModel(this, null, connection));
	}

	/// <summary>
	///		Comprueba si se puede ejecutar una acción
	/// </summary>
	protected override bool CanExecuteAction(string action)
	{
		NodeType nodeType = GetSelectedNodeTypeConverted();

			return action switch
				{
					nameof(NewConnectionCommand) => true,
					nameof(OpenCommand) or nameof(ExportDataCommand) or nameof(ImportDataCommand) => nodeType == NodeType.Connection || nodeType == NodeType.Table,
					nameof(CopyCommand) or nameof(DeleteCommand) or nameof(CreateSchemaXmlCommand) => nodeType == NodeType.Connection,
					nameof(PasteCommand) => copiedConnection != null,
					_ => false
				};
	}

	/// <summary>
	///		Abre la ventana de propiedades de un nodo
	/// </summary>
	protected override void OpenProperties()
	{
		switch (GetSelectedNodeTypeConverted())
		{
			case NodeType.Connection:
					if ((SelectedNode as NodeConnectionViewModel)?.Tag is ConnectionModel connection)
						OpenConnection(connection);
				break;
			case NodeType.Table:
					if ((SelectedNode as NodeTableViewModel)?.Tag is ConnectionTableModel table)
						OpenQuery(table);
				break;
		}
	}

	/// <summary>
	///		Abre la ventana con los datos de una conexión
	/// </summary>
	internal void OpenConnection(ConnectionModel? connection)
	{
		if (SolutionViewModel.MainController.OpenDialog(new Details.Connections.ConnectionViewModel(SolutionViewModel, connection)) == 
				BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			Load();
	}

	/// <summary>
	///		Abre una consulta
	/// </summary>
	private void OpenQuery(ConnectionTableModel? table)
	{
		SolutionViewModel.MainController.OpenWindow(new Details.Queries.ExecuteQueryViewModel(SolutionViewModel, table?.Connection.Name, GetQuery(table), true));
	}

	/// <summary>
	///		Obtiene una consulta sobre una tabla
	/// </summary>
	private string GetQuery(ConnectionTableModel? table)
	{
		if (table is null)
			return string.Empty;
		else
			return SolutionViewModel.Manager.GetSqlQuery(table);
	}

	/// <summary>
	///		Obtiene la cadena SQL de SELECT de una tabla o el nombre de la tabla
	/// </summary>
	internal string GetSqlSelectText(NodeTableViewModel tableViewModel, bool fullSql)
	{
		if (fullSql)
			return GetQuery(tableViewModel.Table);
		else
			return SolutionViewModel.Manager.GetSqlTableName(tableViewModel.Table);
	}

	/// <summary>
	///		Obtiene la cadena SQL de SELECT de un campo o todos los campos de una tabla
	/// </summary>
	internal string GetSqlSelect(NodeTableFieldViewModel fieldViewModel, bool fullSql)
	{
		if (fullSql)
			return GetQuery(fieldViewModel.Field.Table);
		else
			return SolutionViewModel.Manager.GetSqlFieldName(fieldViewModel.Field);
	}

	/// <summary>
	///		Borra el elemento seleccionado
	/// </summary>
	protected override void DeleteItem()
	{
		switch (SelectedNode?.Tag)
		{
			case ConnectionModel item:
					DeleteConnection(item);
				break;
		}
	}

	/// <summary>
	///		Borra una conexión
	/// </summary>
	private void DeleteConnection(ConnectionModel connection)
	{
		if (SolutionViewModel.MainController.SystemController.ShowQuestion($"¿Realmente desea borrar los datos de conexión {connection.Name}?"))
		{
			// Borra la conexión
			SolutionViewModel.Solution.Connections.Remove(connection);
			// Graba la solución
			SolutionViewModel.SaveSolution();
			// Actualiza el combo de conexiones
			SolutionViewModel.ConnectionExecutionViewModel.Load();
			// Actualiza el árbol
			Load();
		}
	}

	/// <summary>
	///		Copia la conexión
	/// </summary>
	private void CopyConnection()
	{
		if (SelectedNode is NodeConnectionViewModel node)
			copiedConnection = node.Connection;
	}

	/// <summary>
	///		Pega la conexión
	/// </summary>
	private void PasteConnection()
	{
		if (copiedConnection is null)
			SolutionViewModel.MainController.SystemController.ShowMessage("Copy the connection that you want paste");
		else
		{
			ConnectionModel connection = copiedConnection.Clone();

				// Cambia el nombre y el Id
				connection.Name += " (copy)";
				// Añade la conexión a la solución
				SolutionViewModel.Solution.Connections.Add(connection);
				// Graba la solución
				SolutionViewModel.SaveSolution();
				// Actualiza el combo de conexiones
				SolutionViewModel.ConnectionExecutionViewModel.Load();
				// Actualiza el árbol
				Load();
		}
	}

	/// <summary>
	///		Crea el archivo XML de esquema
	/// </summary>
	private async Task CreateSchemaXmlAsync(CancellationToken cancellationToken)
	{
		if (SelectedNode is NodeConnectionViewModel node)
		{
			string? fileName = SolutionViewModel.MainController.DialogsController.OpenDialogSave(null, 
																								"Archivos XML (*.xml)|*.xml|Todos los archivos (*.*)|*.*", 
																								$"{node.Connection.Name}.xml", 
																								".xml");

				if (!string.IsNullOrWhiteSpace(fileName))
					try
					{
						// Crea los archivos
						await new Application.Controllers.Schema.SchemaManager(SolutionViewModel.Manager).SaveAsync(node.Connection, fileName, cancellationToken);
						// Log
						SolutionViewModel.MainController.SystemController.ShowMessage("Generación del archivo de esquema finalizada");
						SolutionViewModel.MainController.MainWindowController
								.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
													"Generación de archivos de esquema",
													$"Ha terminado correctamente la generación del archivo de esquema de la conexión {node.Connection.Name}");
					}
					catch (Exception exception)
					{
						SolutionViewModel.MainController.MainWindowController
								.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Error,
												  "Generación de archivos de esquema",
												  $"Error en la generación de archivos de esquema de la conexión {node.Connection.Name}. {exception.Message}");
					}
		}
	}

	/// <summary>
	///		Exporta a archivos las tablas del esqumea
	/// </summary>
	private async Task ExportDataAsync(CancellationToken cancellationToken)
	{
		ConnectionModel? connection = null;
		ConnectionTableModel? table = null;

			// Obtiene los datos de conexión y tabla
			if (SelectedNode is NodeConnectionViewModel node)
				connection = node.Connection;
			else if (SelectedNode is NodeTableViewModel nodeTable)
			{
				connection = nodeTable.Table.Connection;
				table = nodeTable.Table;
			}
			// Abre el formulario de exportación
			await SolutionViewModel.ConnectionExecutionViewModel.ExportDataBaseAsync(connection, table, cancellationToken);
	}

	/// <summary>
	///		Inporta archivos de las tablas del esqumea
	/// </summary>
	private async Task ImportDataAsync(CancellationToken cancellationToken)
	{
		ConnectionModel? connection = null;
		ConnectionTableModel? table = null;

			// Obtiene los datos de conexión y tabla
			if (SelectedNode is NodeConnectionViewModel node)
				connection = node.Connection;
			else if (SelectedNode is NodeTableViewModel nodeTable)
			{
				connection = nodeTable.Table.Connection;
				table = nodeTable.Table;
			}
			// Abre el formulario de exportación
			await SolutionViewModel.ConnectionExecutionViewModel.ImportDataBaseAsync(connection, table, cancellationToken);
	}

	/// <summary>
	///		Obtiene el enumerado del tipo de nodo seleccionado
	/// </summary>
	private NodeType GetSelectedNodeTypeConverted() => (GetSelectedNodeType() ?? string.Empty).GetEnum(NodeType.Unknown);

	/// <summary>
	///		Comando de nueva conexión
	/// </summary>
	public BaseCommand NewConnectionCommand { get; }

	/// <summary>
	///		Comando para crear una nueva consulta
	/// </summary>
	public BaseCommand NewQueryCommand { get; }

	/// <summary>
	///		Comando para copiar los datos de una conexión
	/// </summary>
	public BaseCommand CopyCommand { get; }

	/// <summary>
	///		Comando para pegar los datos de una conexión
	/// </summary>
	public BaseCommand PasteCommand { get; }

	/// <summary>
	///		Comando para generar el XML de esquema
	/// </summary>
	public BaseCommand CreateSchemaXmlCommand { get; }

	/// <summary>
	///		Comando para exportar datos a archivos
	/// </summary>
	public BaseCommand ExportDataCommand { get; }

	/// <summary>
	///		Comando para importar datos de archivos
	/// </summary>
	public BaseCommand ImportDataCommand { get; }
}