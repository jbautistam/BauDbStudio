using System;

using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers
{
	/// <summary>
	///		ViewModel de un nodo de dimensión
	/// </summary>
	public class NodeDimensionViewModel : BaseTreeNodeViewModel
	{
		public NodeDimensionViewModel(BaseTreeViewModel trvTree, BaseTreeNodeViewModel parent, DimensionModel dimension) : 
					base(trvTree, parent, dimension.Id, NodeType.Dimension, IconType.Dimension, dimension, true, true, BauMvvm.ViewModels.Media.MvvmColor.Navy)
		{
			Dimension = dimension;
		}

		/// <summary>
		///		Carga los nodos de la dimensión
		/// </summary>
		protected override void LoadNodes()
		{
			// Carga las dimensiones hija
			foreach (DimensionRelationModel relation in Dimension.Relations)
				if (relation.Dimension != null)
					Children.Add(new NodeDimensionViewModel(TreeViewModel, this, relation.Dimension));
			// Carga el origen de datos
			if (Dimension.DataSource != null)
				Children.Add(new NodeDataSourceViewModel(TreeViewModel, this, Dimension.DataSource));
		}

		/// <summary>
		///		Tabla asociada al nodo
		/// </summary>
		public DimensionModel Dimension { get; }
	}
}
