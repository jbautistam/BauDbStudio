using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections
{
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
			/// <summary>Raíz de la distribución</summary>
			DeploymentRoot,
			/// <summary>Distribución</summary>
			Deployment,
			/// <summary>Raíz de archivos de proyecto</summary>
			FilesRoot,
			/// <summary>Archivo / directorio</summary>
			File,
			/// <summary>Conexión a storage</summary>
			Storage,
			/// <summary>Contenedor de storage</summary>
			StorageContainer,
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
			Deployment,
			Project,
			Path,
			File,
			Schema,
			Table,
			View,
			Key,
			Field,
			Error,
			Loading,
			Storage,
			Report,
			DataSourceSql,
			Dimension
		}

		public TreeConnectionsViewModel(SolutionViewModel solutionViewModel) : base(solutionViewModel)
		{
			NewConnectionCommand = new BaseCommand(_ => OpenConnection(null), _ => CanExecuteAction(nameof(NewConnectionCommand)))
										.AddListener(this, nameof(SelectedNode));
			NewDeploymentCommand = new BaseCommand(_ => OpenDeployment(null), _ => CanExecuteAction(nameof(NewDeploymentCommand)))
										.AddListener(this, nameof(SelectedNode));
			NewQueryCommand = new BaseCommand(parameter => OpenQuery(null));
			ExecuteDeploymentCommand = new BaseCommand(_ => ExecuteDeployment(), _ => CanExecuteAction(nameof(ExecuteDeploymentCommand)))
										.AddListener(this, nameof(SelectedNode));
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void AddRootNodes()
		{
			Children.Add(new NodeRootViewModel(this, null, NodeType.ConnectionRoot, "Conexiones"));
			Children.Add(new NodeRootViewModel(this, null, NodeType.DeploymentRoot, "Distribución"));
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			TreeConnectionsViewModel.NodeType nodeType = GetSelectedNodeTypeConverted();

				switch (action)
				{
					case nameof(NewConnectionCommand):
					case nameof(NewDeploymentCommand):
						return true;
					case nameof(OpenCommand):
						return nodeType == NodeType.Connection || 
							   nodeType == NodeType.Deployment ||  
							   nodeType == NodeType.Table;
					case nameof(ExecuteDeploymentCommand):
						return nodeType == NodeType.Deployment;
					case nameof(DeleteCommand):
						return nodeType == NodeType.Connection || nodeType == NodeType.Deployment;
					default:
						return false;
				}
		}

		/// <summary>
		///		Abre la ventana de propiedades de un nodo
		/// </summary>
		protected override void OpenProperties()
		{
			switch (GetSelectedNodeTypeConverted())
			{
				case NodeType.Connection:
						OpenConnection((SelectedNode as NodeConnectionViewModel)?.Tag as ConnectionModel);
					break;
				case NodeType.Deployment:
						OpenDeployment((SelectedNode as NodeDeploymentViewModel)?.Tag as Models.Deployments.DeploymentModel);
					break;
				case NodeType.Table:
						OpenQuery((SelectedNode as NodeTableViewModel)?.Tag as ConnectionTableModel);
					break;
			}
		}

		/// <summary>
		///		Abre la ventana con los datos de una conexión
		/// </summary>
		internal void OpenConnection(ConnectionModel connection)
		{
			if (SolutionViewModel.MainController.AppController.OpenDialog(new Details.Connections.ConnectionViewModel(SolutionViewModel, connection)) == 
					BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				Load();
		}

		/// <summary>
		///		Abre la ventana con los datos de una distribución
		/// </summary>
		private void OpenDeployment(Models.Deployments.DeploymentModel deployment)
		{
			if (SolutionViewModel.MainController.AppController.OpenDialog(new Details.Deployments.DeploymentViewModel(SolutionViewModel, deployment)) == 
					BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				Load();
		}

		/// <summary>
		///		Abre una consulta
		/// </summary>
		private void OpenQuery(ConnectionTableModel table)
		{
			SolutionViewModel.MainController.AppController.OpenWindow(new Details.Queries.ExecuteQueryViewModel(SolutionViewModel, table?.Connection.Name, GetQuery(table)));
		}

		/// <summary>
		///		Obtiene una consulta sobre una tabla
		/// </summary>
		private string GetQuery(ConnectionTableModel table)
		{
			if (table == null)
				return string.Empty;
			else
			{
				(string start, string end) separators = GetSqlSeparators(table.Connection);

					// Devuelve la consulta
					return $"SELECT {GetSqlSelectFields(separators, table)}{Environment.NewLine}\tFROM {GetSqlName(separators, table.Schema, table.Name)}{Environment.NewLine}";
			}
		}

		/// <summary>
		///		Obtiene una cadena con los campos
		/// </summary>
		private string GetSqlSelectFields((string start, string end) separators, ConnectionTableModel table)
		{
			string fields = string.Empty;
			int length = 80;

				// Obtiene la cadena con los campos
				foreach (ConnectionTableFieldModel field in table.Fields)
				{
					// Añade un salto de línea cada 80 caracteres (más o menos)
					if (fields.Length > length)
					{
						fields += Environment.NewLine + "\t\t";
						length += 80;
					}
					// Añade el nombre de campo
					fields += $"{GetSqlName(separators, field.Table.Name, field.Name)}";
					// Añade la coma si es necesario (no se hace con AddSeparator porque como tenemos un salto de línea, añadiría la coma después
					// del salto de línea)
					if (table.Fields.IndexOf(field) < table.Fields.Count - 1)
						fields += ", ";
				}
				// Devuelve la cadena con los campos
				return fields;
		}

		/// <summary>
		///		Obtiene los separadores para una conexión
		/// </summary>
		private (string separatorStart, string separatorEnd) GetSqlSeparators(ConnectionModel connection)
		{
			string separatorStart = "`", separatorEnd = "`";

				// Cambia los separadores dependiendo del tipo de conexión
				if (connection.Type != ConnectionModel.ConnectionType.Spark)
				{
					separatorStart = "[";
					separatorEnd = "]";
				}
				// Devuelve los separadores
				return (separatorStart, separatorEnd);
		}

		/// <summary>
		///		Obtiene la cadena SQL de SELECT de una tabla o el nombre de la tabla
		/// </summary>
		internal string GetSqlSelectText(NodeTableViewModel tableViewModel, bool fullSql)
		{
			if (fullSql)
				return GetQuery(tableViewModel.Table);
			else
				return GetSqlName(GetSqlSeparators(tableViewModel.Table.Connection), tableViewModel.Table.Schema, tableViewModel.Table.Name);
		}

		/// <summary>
		///		Obtiene la cadena SQL de SELECT de un campo o todos los campos de una tabla
		/// </summary>
		internal string GetSqlSelect(NodeTableFieldViewModel fieldViewModel, bool fullSql)
		{
			if (fullSql)
				return GetSqlSelectFields(GetSqlSeparators(fieldViewModel.Field.Table.Connection), fieldViewModel.Field.Table);
			else
				return GetSqlName(GetSqlSeparators(fieldViewModel.Field.Table.Connection), fieldViewModel.Field.Table.Name, fieldViewModel.Field.Name);
		}

		/// <summary>
		///		Obtiene un nombre de tabla / campo para una consulta SQL 
		/// </summary>
		private string GetSqlName((string start, string end) separators, string schema, string name)
		{
			string result = string.Empty;

				// Añade el nombre de esquema
				if (!string.IsNullOrWhiteSpace(schema))
					result = $"{separators.start}{schema}{separators.end}.";
				// Devuelve el nombre del campo / tabla
				return $"{result}{separators.start}{name}{separators.end}";
		}

		/// <summary>
		///		Exporta un directorio para Databricks
		/// </summary>
		private void ExecuteDeployment()
		{
			Models.Deployments.DeploymentModel deployment = (SelectedNode as NodeDeploymentViewModel)?.Tag as Models.Deployments.DeploymentModel;

				if (deployment == null)
					SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione un nodeo de distribución");
				else if (string.IsNullOrWhiteSpace(deployment.SourcePath))
					SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione una carpeta de origen");
				else if (!System.IO.Directory.Exists(deployment.SourcePath))
					SolutionViewModel.MainController.SystemController.ShowMessage("No se encuentra el directorio de origen");
				else 
				{
					// Copia los archivos
					try
					{
						// Ejecuta la exportación
						SolutionViewModel.Manager.ExportToDataBricks(deployment);
						// Mensaje
						SolutionViewModel.MainController.MainWindowController
								.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
												  "Distribución", "Fin de la copia de archivos");
					}
					catch (Exception exception)
					{
						SolutionViewModel.Manager.Logger.Default.LogItems.Error($"Error al copiar los archivos: {exception.Message}");
						SolutionViewModel.MainController.MainWindowController
								.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Error,
												  "Distribución", $"Error en la copia de archivos. {exception.Message}");
					}
					// Limpia el log
					SolutionViewModel.Manager.Logger.Flush();
				}
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
				case Models.Deployments.DeploymentModel item:
						DeleteDeployment(item);
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
		///		Borra un proceso de distribución
		/// </summary>
		private void DeleteDeployment(Models.Deployments.DeploymentModel deployment)
		{
			if (SolutionViewModel.MainController.SystemController.ShowQuestion($"¿Realmente desea borrar los datos de la distribución {deployment.Name}?"))
			{
				// Borra el objeto
				SolutionViewModel.Solution.Deployments.Remove(deployment);
				// Graba la solución
				SolutionViewModel.SaveSolution();
				// Actualiza el árbol
				Load();
			}
		}

		/// <summary>
		///		Obtiene el enumerado del tipo de nodo seleccionado
		/// </summary>
		private NodeType GetSelectedNodeTypeConverted()
		{
			return GetSelectedNodeType().GetEnum(NodeType.Unknown);
		}

		/// <summary>
		///		Comando de nueva conexión
		/// </summary>
		public BaseCommand NewConnectionCommand { get; }

		/// <summary>
		///		Comando de nueva distribución
		/// </summary>
		public BaseCommand NewDeploymentCommand { get; }

		/// <summary>
		///		Comando para crear una nueva consulta
		/// </summary>
		public BaseCommand NewQueryCommand { get; }

		/// <summary>
		///		Comando para exportar
		/// </summary>
		public BaseCommand ExecuteDeploymentCommand { get; }
	}
}