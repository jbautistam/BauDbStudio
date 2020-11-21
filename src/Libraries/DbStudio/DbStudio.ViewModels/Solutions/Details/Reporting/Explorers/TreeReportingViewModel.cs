using System;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Explorers
{
	/// <summary>
	///		ViewModel para el árbol de almacenes de datos
	/// </summary>
	public class TreeReportingViewModel : BaseTreeViewModel
	{
		public TreeReportingViewModel(ReportingSolutionViewModel reportingSolutionViewModel) : base(reportingSolutionViewModel.SolutionViewModel)
		{
			ReportingSolutionViewModel = reportingSolutionViewModel;
			NewDataWarehouseCommand = new BaseCommand(_ => NewDataWarehouse(), _ => CanExecuteAction(nameof(NewDataWarehouseCommand)))
										.AddListener(this, nameof(SelectedNode));
			NewDataSourceCommand = new BaseCommand(_ => OpenDataSource(null), _ => CanExecuteAction(nameof(NewDataSourceCommand)))
										.AddListener(this, nameof(SelectedNode));
			NewDimensionCommand = new BaseCommand(_ => OpenDimension(null), _ => CanExecuteAction(nameof(NewDimensionCommand)))
										.AddListener(this, nameof(SelectedNode));
			NewReportCommand = new BaseCommand(_ => OpenReport(null), _ => CanExecuteAction(nameof(NewReportCommand)))
										.AddListener(this, nameof(SelectedNode));
			QueryCommand = new BaseCommand(_ => OpenQuery(), _ => CanExecuteAction(nameof(QueryCommand)))
										.AddListener(this, nameof(SelectedNode));
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void AddRootNodes()
		{
			foreach (DataWarehouseModel dataWarehouse in ReportingSolutionViewModel.ReportingManager.Schema.DataWarehouses)
				Children.Add(new NodeDataWarehouseViewModel(this, null, dataWarehouse));
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			switch (action)
			{
				case nameof(NewDataWarehouseCommand):
					return true;
				case nameof(NewDataSourceCommand):
				case nameof(NewReportCommand):
					return SelectedNode is NodeDataWarehouseViewModel;
				case nameof(NewDimensionCommand):
					return SelectedNode is NodeDataSourceViewModel;
				case nameof(OpenCommand):
					return SelectedNode is NodeDimensionViewModel || SelectedNode is NodeDataSourceViewModel || SelectedNode is NodeReportViewModel;
				case nameof(QueryCommand):
					return SelectedNode is NodeReportViewModel || SelectedNode is NodeDataSourceViewModel;
				case nameof(DeleteCommand):
					return SelectedNode is NodeDataWarehouseViewModel;
				default:
					return false;
			}
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
			string fileName = ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.DialogsController
										.OpenDialogLoad(ReportingSolutionViewModel.SolutionViewModel.MainViewModel.LastPathSelected,
														"Archivo de esquema (*.xml)|*.xml|Todos los archivos (*.*)|*.*");

				if (!string.IsNullOrWhiteSpace(fileName) && System.IO.File.Exists(fileName))
				{
					// Añade un almacén de datos a la solución
					ReportingSolutionViewModel.ReportingManager.AddDataWarehouse(fileName);
					// Graba los datos de la solución
					ReportingSolutionViewModel.SaveSolution();
					// Y actualiza el árbol
					Load();
				}
		}

		/// <summary>
		///		Abre un origen de datos
		/// </summary>
		private void OpenDataSource(NodeDataSourceViewModel node)
		{
			if (node == null || node.DataSource is DataSourceSqlModel)
			{
				DataSourceSqlModel dataSource = null;

					// Genera el origen de datos
					if (node != null && node.DataSource is DataSourceSqlModel dataSourceSql)
						dataSource = dataSourceSql;
					else
						dataSource = new DataSourceSqlModel(GetSelectedDataWarehouse(SelectedNode));
					// Abre la vista
					ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.OpenWindow(new DataSources.DataSourceSqlViewModel(ReportingSolutionViewModel, dataSource));
			}
			else if (node.DataSource is DataSourceTableModel dataSourceTable)
				ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.OpenWindow(new DataSources.DataSourceTableViewModel(ReportingSolutionViewModel, dataSourceTable));
		}

		/// <summary>
		///		Obtiene el <see cref="DataWarehouseModel"/> seleccionado en el árbol buscando los padres
		/// </summary>
		private DataWarehouseModel GetSelectedDataWarehouse(BaseTreeNodeViewModel node)
		{
			if (node is NodeDataWarehouseViewModel nodeDataWarehouse)
				return nodeDataWarehouse.DataWarehouse;
			else if (node.Parent != null)
				return GetSelectedDataWarehouse(node.Parent as BaseTreeNodeViewModel);
			else
				return null;
		}

		/// <summary>
		///		Abre el formulario de detalles de una dimensión
		/// </summary>
		private void OpenDimension(NodeDimensionViewModel node)
		{
			bool isNew = false;
			LibReporting.Models.DataWarehouses.Dimensions.DimensionModel dimension;

				// Si no se le ha pasado un nodo, si estamos en un nodo de origen de datos, creamos una dimensión a partir del nodo
				if (node == null && SelectedNode is NodeDataSourceViewModel nodeDataSource)
				{
					dimension = new LibReporting.Models.DataWarehouses.Dimensions.DimensionModel(GetSelectedDataWarehouse(nodeDataSource))
										{
											DataSource = nodeDataSource.DataSource
										};
					isNew = true;
				}
				else
					dimension = node.Dimension;
				// Abre el formulario
				ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.OpenWindow(new Dimension.DimensionViewModel(ReportingSolutionViewModel, dimension, isNew));
		}

		/// <summary>
		///		Abre el formulario de detalles de un informe
		/// </summary>
		private void OpenReport(NodeReportViewModel node)
		{
			bool isNew = false;
			LibReporting.Models.DataWarehouses.Reports.ReportModel report;

				// Si no se le ha pasado un nodo, si estamos en un nodo de origen de datos, creamos una dimensión a partir del nodo
				if (node == null)
				{
					// Crea el informe
					report = new LibReporting.Models.DataWarehouses.Reports.ReportModel(GetSelectedDataWarehouse(SelectedNode))
												{
													GlobalId = "RptNewReport",
													Name = "Nuevo informe"
												};
					// Indica que es nuevo
					isNew = true;
				}
				else
					report = node.Report;
				// Abre el formulario
				ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.OpenWindow(new Reports.ReportViewModel(ReportingSolutionViewModel, report, isNew));
		}

		/// <summary>
		///		Abre la ventana de consulta
		/// </summary>
		private void OpenQuery()
		{
			switch (SelectedNode)
			{
				case NodeReportViewModel node:
						ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.OpenWindow
								(new Queries.ReportViewModel(ReportingSolutionViewModel, node.Report));
					break;
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
			}
		}

		/// <summary>
		///		Borra un almacén de datos
		/// </summary>
		private void DeleteDataWarehouse(DataWarehouseModel dataWarehouse)
		{
			if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Realmente desea borrar los datos del almacén de datos {dataWarehouse.Name}?"))
			{
				// Borra el almacén de datos
				ReportingSolutionViewModel.ReportingManager.RemoveDataWarehouse(dataWarehouse);
				// Graba la solución
				ReportingSolutionViewModel.SaveSolution();
				// Actualiza el árbol
				Load();
			}
		}

		/// <summary>
		///		ViewModel de la solución para reporting
		/// </summary>
		public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

		/// <summary>
		///		Comando de nuevo almacén de datos
		/// </summary>
		public BaseCommand NewDataWarehouseCommand { get; }

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
	}
}