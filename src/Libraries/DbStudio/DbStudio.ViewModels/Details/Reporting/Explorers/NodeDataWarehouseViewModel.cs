using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers;

/// <summary>
///		ViewModel de un nodo de conexión
/// </summary>
public class NodeDataWarehouseViewModel : PluginNodeViewModel
{
	public NodeDataWarehouseViewModel(PluginTreeViewModel trvTree, ControlHierarchicalViewModel? parent, DataWarehouseModel dataWarehouse) : 
				base(trvTree, parent, dataWarehouse.Name, TreeReportingViewModel.NodeType.DataWarehouse.ToString(), TreeReportingViewModel.IconType.DataWarehouse.ToString(), 
					 dataWarehouse, true, true, MvvmColor.Red)
	{
		DataWarehouse = dataWarehouse;
	}

	/// <summary>
	///		Carga los nodos
	/// </summary>
	protected override void LoadNodes()
	{
		Children.Add(new NodeRootViewModel(TreeViewModel, this, TreeReportingViewModel.NodeType.DataSourcesRoot, "Orígenes de datos", true));
		Children.Add(new NodeRootViewModel(TreeViewModel, this, TreeReportingViewModel.NodeType.DimensionsRoot, "Dimensiones", true));
		Children.Add(new NodeRootViewModel(TreeViewModel, this, TreeReportingViewModel.NodeType.ReportsRoot, "Informes", true));
	}

	/// <summary>
	///		Almacén de datos asociado al nodo
	/// </summary>
	public DataWarehouseModel DataWarehouse { get; }
}
