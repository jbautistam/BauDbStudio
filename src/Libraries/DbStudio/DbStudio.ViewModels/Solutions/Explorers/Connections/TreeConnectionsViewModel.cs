using System;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers.Connections
{
	/// <summary>
	///		ViewModel para el árbol de conexiones
	/// </summary>
	public class TreeConnectionsViewModel : BaseTreeViewModel
	{
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
			Children.Add(new NodeRootViewModel(this, null, BaseTreeNodeViewModel.NodeType.ConnectionRoot, "Conexiones"));
			Children.Add(new NodeRootViewModel(this, null, BaseTreeNodeViewModel.NodeType.DeploymentRoot, "Distribución"));
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			BaseTreeNodeViewModel.NodeType nodeType = GetSelectedNodeType();

				switch (action)
				{
					case nameof(NewConnectionCommand):
					case nameof(NewDeploymentCommand):
						return true;
					case nameof(OpenCommand):
						return nodeType == BaseTreeNodeViewModel.NodeType.Connection || 
							   nodeType == BaseTreeNodeViewModel.NodeType.Deployment ||  
							   nodeType == BaseTreeNodeViewModel.NodeType.Table;
					case nameof(ExecuteDeploymentCommand):
						return nodeType == BaseTreeNodeViewModel.NodeType.Deployment;
					case nameof(DeleteCommand):
						return nodeType == BaseTreeNodeViewModel.NodeType.Connection || nodeType == BaseTreeNodeViewModel.NodeType.Deployment;
					default:
						return false;
				}
		}

		/// <summary>
		///		Abre la ventana de propiedades de un nodo
		/// </summary>
		protected override void OpenProperties()
		{
			switch (GetSelectedNodeType())
			{
				case BaseTreeNodeViewModel.NodeType.Connection:
						OpenConnection((SelectedNode as NodeConnectionViewModel)?.Tag as ConnectionModel);
					break;
				case BaseTreeNodeViewModel.NodeType.Deployment:
						OpenDeployment((SelectedNode as NodeDeploymentViewModel)?.Tag as Models.Deployments.DeploymentModel);
					break;
				case BaseTreeNodeViewModel.NodeType.Table:
						OpenQuery((SelectedNode as NodeTableViewModel)?.Tag as ConnectionTableModel);
					break;
			}
		}

		/// <summary>
		///		Abre la ventana con los datos de una conexión
		/// </summary>
		internal void OpenConnection(ConnectionModel connection)
		{
			if (SolutionViewModel.MainViewModel.MainController.OpenDialog(new Details.Connections.ConnectionViewModel(SolutionViewModel, connection)) == 
					BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				Load();
		}

		/// <summary>
		///		Abre la ventana con los datos de una distribución
		/// </summary>
		private void OpenDeployment(Models.Deployments.DeploymentModel deployment)
		{
			if (SolutionViewModel.MainViewModel.MainController.OpenDialog(new Details.Deployments.DeploymentViewModel(SolutionViewModel, deployment)) == 
					BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				Load();
		}

		/// <summary>
		///		Abre una consulta
		/// </summary>
		private void OpenQuery(ConnectionTableModel table)
		{
			SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Connections.ExecuteQueryViewModel(SolutionViewModel, table?.Connection.Name, GetQuery(table)));
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
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un nodeo de distribución");
				else if (string.IsNullOrWhiteSpace(deployment.SourcePath))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una carpeta de origen");
				else if (!System.IO.Directory.Exists(deployment.SourcePath))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No se encuentra el directorio de origen");
				else 
				{
					// Copia los archivos
					try
					{
						// Ejecuta la exportación
						SolutionViewModel.MainViewModel.Manager.ExportToDataBricks(deployment);
						// Mensaje
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
																														"Distribución", "Fin de la copia de archivos",
																														TimeSpan.FromSeconds(10));
					}
					catch (Exception exception)
					{
						SolutionViewModel.MainViewModel.Manager.Logger.Default.LogItems.Error($"Error al copiar los archivos: {exception.Message}");
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Error,
																														"Distribución", 
																														$"Error en la copia de archivos. {exception.Message}",
																														TimeSpan.FromSeconds(10));
					}
					// Limpia el log
					SolutionViewModel.MainViewModel.Manager.Logger.Flush();
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
			if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Realmente desea borrar los datos de conexión {connection.Name}?"))
			{
				// Borra la conexión
				SolutionViewModel.Solution.Connections.Remove(connection);
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
				// Actualiza el árbol
				Load();
			}
		}

		/// <summary>
		///		Borra un proceso de distribución
		/// </summary>
		private void DeleteDeployment(Models.Deployments.DeploymentModel deployment)
		{
			if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Realmente desea borrar los datos de la distribución {deployment.Name}?"))
			{
				// Borra el objeto
				SolutionViewModel.Solution.Deployments.Remove(deployment);
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
				// Actualiza el árbol
				Load();
			}
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