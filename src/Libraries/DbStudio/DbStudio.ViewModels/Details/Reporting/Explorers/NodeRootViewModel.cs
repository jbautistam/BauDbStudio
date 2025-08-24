using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers;

/// <summary>
///		ViewModel de un nodo raíz (raíz de origen de datos, informes...)
/// </summary>
public class NodeRootViewModel : PluginNodeViewModel
{
	// Constantes privadas
	private const string PhysicalSchema = "Default schema";
	private const string LogicalSchema = "Logical schema";

	public NodeRootViewModel(PluginTreeViewModel trvTree, ControlHierarchicalViewModel parent, TreeReportingViewModel.NodeType type, 
							 string text, bool lazyLoad = true, bool bold = true, MvvmColor? color = null) :
				base(trvTree, parent, text, type.ToString(), TreeReportingViewModel.IconType.Unknown.ToString(), type, lazyLoad, bold, color ?? MvvmColor.Red)
	{
		switch (NodeType)
		{
			case TreeReportingViewModel.NodeType.DimensionsRoot:
					Icon = TreeReportingViewModel.IconType.Dimension.ToString();
				break;
			case TreeReportingViewModel.NodeType.DataSourcesRoot:
					Icon = TreeReportingViewModel.IconType.DataSourceRoot.ToString();
				break;
			case TreeReportingViewModel.NodeType.DataSourceSchemasRoot:
			case TreeReportingViewModel.NodeType.TransformRulesRoot:
					Icon = TreeReportingViewModel.IconType.Folder.ToString();
				break;
			case TreeReportingViewModel.NodeType.ReportsRoot:
					Icon = TreeReportingViewModel.IconType.Report.ToString();
				break;
			case TreeReportingViewModel.NodeType.Field:
					Icon = TreeReportingViewModel.IconType.Field.ToString();
				break;
			case TreeReportingViewModel.NodeType.Table:
					Icon = TreeReportingViewModel.IconType.DataSourceTable.ToString();
				break;
			case TreeReportingViewModel.NodeType.Error:
					Icon = TreeReportingViewModel.IconType.Error.ToString();
				break;
		}
	}

	/// <summary>
	///		Carga los nodos del tipo
	/// </summary>
	protected override void LoadNodes()
	{
		switch (NodeType)
		{
			case TreeReportingViewModel.NodeType.DataSourcesRoot:
					LoadDataSourceSchemas();
				break;
			case TreeReportingViewModel.NodeType.DataSourceSchemasRoot:
					LoadDataSources();
				break;
			case TreeReportingViewModel.NodeType.DimensionsRoot:
					LoadDimensions();
				break;
			case TreeReportingViewModel.NodeType.ReportsRoot:
					LoadReports();
				break;
			case TreeReportingViewModel.NodeType.TransformRulesRoot:
					LoadTransformRules();
				break;
		}
	}

	/// <summary>
	///		Carga los esquemas de los orígenes de datos
	/// </summary>
	private void LoadDataSourceSchemas()
	{
		DataWarehouseModel? dataWarehouse = GetDataWarehouse();
		List<string> schemas = new();

			// Añade los diferentes esquemas al conjunto
			if (dataWarehouse is not null)
				foreach (BaseDataSourceModel dataSource in dataWarehouse.DataSources.EnumerateValues())
					if (dataSource is DataSourceTableModel dataTable)
					{
						string schema = dataTable.Schema;

							// Asigna el valor por defecto si no hay definido ningúno
							if (string.IsNullOrWhiteSpace(schema))
								schema = PhysicalSchema;
							// Añade el esquema a la lista de esquemas definidos
							if (schemas.IndexOf(schema) < 0)
								schemas.Add(schema);
					}
			// Si no se ha definido ningún esquema, se añade el esquema físico
			if (schemas.Count == 0)
				schemas.Add(PhysicalSchema);
			// Ordena los esquemas
			schemas.Sort((first, second) => first.CompareTo(second));
			// Añade el esquema lógico
			schemas.Add(LogicalSchema);
			// Añade los nodos
			foreach (string schema in schemas)
				Children.Add(new NodeRootViewModel(TreeViewModel, this, TreeReportingViewModel.NodeType.DataSourceSchemasRoot,
												   schema, true, true, MvvmColor.Navy));
	}

	/// <summary>
	///		Carga los orígenes de datos
	/// </summary>
	private void LoadDataSources()
	{
		DataWarehouseModel? dataWarehouse = GetDataWarehouse();

			if (dataWarehouse is not null)
				foreach (BaseDataSourceModel dataSource in dataWarehouse.DataSources.EnumerateValuesSorted())
					if (dataSource is DataSourceTableModel dataTable &&
							((string.IsNullOrWhiteSpace(dataTable.Schema) && Text.Equals(PhysicalSchema, StringComparison.CurrentCultureIgnoreCase)) ||
							 (!string.IsNullOrWhiteSpace(dataTable.Schema) && dataTable.Schema.Equals(Text, StringComparison.CurrentCultureIgnoreCase))
							))
						Children.Add(new NodeDataSourceViewModel(TreeViewModel, this, dataSource));
					else if (Text.Equals(LogicalSchema, StringComparison.CurrentCultureIgnoreCase) &&
								dataSource is DataSourceSqlModel)
						Children.Add(new NodeDataSourceViewModel(TreeViewModel, this, dataSource));
	}

	/// <summary>
	///		Carga las dimensiones
	/// </summary>
	private void LoadDimensions()
	{
		DataWarehouseModel? dataWarehouse = GetDataWarehouse();

			if (dataWarehouse is not null)
				foreach (BaseDimensionModel dimension in dataWarehouse.Dimensions.EnumerateValuesSorted())
					Children.Add(new NodeDimensionViewModel(TreeViewModel, this, dimension));
	}

	/// <summary>
	///		Carga los informes
	/// </summary>
	private void LoadReports()
	{
		DataWarehouseModel? dataWarehouse = GetDataWarehouse();

			if (dataWarehouse is not null)
				foreach (ReportModel report in dataWarehouse.Reports.EnumerateValuesSorted())
					Children.Add(new NodeReportViewModel(TreeViewModel, this, report));
	}

	/// <summary>
	///		Carga las reglas de transfromación
	/// </summary>
	private void LoadTransformRules()
	{
		DataWarehouseModel? dataWarehouse = GetDataWarehouse();

			if (dataWarehouse is not null)
				Children.Add(new NodeTransformRulesViewModel(TreeViewModel, this, dataWarehouse));
	}

	/// <summary>
	///		Obtiene el dataWarehouse padre
	/// </summary>
	private DataWarehouseModel? GetDataWarehouse()
	{
		ControlHierarchicalViewModel? nodeParent = Parent;

			// Sube por el árbol hasta encontrar un nodo que contenga un dataWarehouse
			while (nodeParent is not null)
				if (nodeParent is NodeDataWarehouseViewModel nodeDataWarehouse)
					return nodeDataWarehouse.DataWarehouse;
				else
					nodeParent = nodeParent.Parent;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
	}

	/// <summary>
	///		Tipo de nodo
	/// </summary>
	private TreeReportingViewModel.NodeType NodeType => Type.GetEnum(TreeReportingViewModel.NodeType.Unknown);
}
