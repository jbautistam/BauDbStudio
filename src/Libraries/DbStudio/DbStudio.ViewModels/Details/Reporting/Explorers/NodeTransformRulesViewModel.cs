using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.LibReporting.Models.DataWarehouses;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers;

/// <summary>
///		ViewModel de un nodo de reglas de transformación
/// </summary>
public class NodeTransformRulesViewModel : PluginNodeViewModel
{
	public NodeTransformRulesViewModel(PluginTreeViewModel trvTree, PluginNodeViewModel parent, DataWarehouseModel dataWarehouse) : 
				base(trvTree, parent, "Rules file", TreeReportingViewModel.NodeType.Report.ToString(), TreeReportingViewModel.IconType.Report.ToString(), 
					 dataWarehouse, false, false, BauMvvm.ViewModels.Media.MvvmColor.Navy)
	{
		DataWarehouse = dataWarehouse;
	}

	/// <summary>
	///		Carga los nodos del informe
	/// </summary>
	protected override void LoadNodes()
	{
	}

	/// <summary>
	///		Almacén al que se asocia el nodo
	/// </summary>
	public DataWarehouseModel DataWarehouse { get; }
}
