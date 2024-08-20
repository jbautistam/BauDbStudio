using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers;

/// <summary>
///		ViewModel de un nodo de dimensión
/// </summary>
public class NodeDimensionViewModel : PluginNodeViewModel
{
	public NodeDimensionViewModel(PluginTreeViewModel trvTree, PluginNodeViewModel parent, BaseDimensionModel dimension) : 
				base(trvTree, parent, dimension.Id, TreeReportingViewModel.NodeType.Dimension.ToString(), TreeReportingViewModel.IconType.Dimension.ToString(), 
					 dimension, true, true, BauMvvm.ViewModels.Media.MvvmColor.Navy)
	{
		Dimension = dimension;
		if (dimension is DimensionChildModel)
			Icon = TreeReportingViewModel.IconType.DimensionChild.ToString();
	}

	/// <summary>
	///		Carga los nodos de la dimensión
	/// </summary>
	protected override void LoadNodes()
	{
		// Carga el origen de datos
		switch (Dimension)
		{
			case DimensionModel child:
					// Carga las dimensiones hija
					foreach (DimensionRelationModel relation in Dimension.GetRelations())
						if (relation.Dimension != null)
							Children.Add(new NodeDimensionViewModel(TreeViewModel, this, relation.Dimension));
					// Carga el origen de datos
					if (child.DataSource != null)
						Children.Add(new NodeDataSourceViewModel(TreeViewModel, this, child.DataSource));
				break;
			case DimensionChildModel child:
					Children.Add(new NodeDimensionViewModel(TreeViewModel, this, child.GetSourceDimension()));
				break;
		}
	}

	/// <summary>
	///		Dimensión asociada al nodo
	/// </summary>
	public BaseDimensionModel Dimension { get; }
}
