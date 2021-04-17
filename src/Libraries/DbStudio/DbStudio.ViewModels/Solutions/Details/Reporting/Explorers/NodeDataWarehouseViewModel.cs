using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Explorers
{
	/// <summary>
	///		ViewModel de un nodo de conexión
	/// </summary>
	public class NodeDataWarehouseViewModel : BaseTreeNodeViewModel
	{
		public NodeDataWarehouseViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, DataWarehouseModel dataWarehouse) : 
					base(trvTree, parent, dataWarehouse.Name, NodeType.DataWarehouse, IconType.Connection, dataWarehouse, true, true, MvvmColor.Red)
		{
			DataWarehouse = dataWarehouse;
		}

		/// <summary>
		///		Carga los nodos
		/// </summary>
		protected override void LoadNodes()
		{
			Children.Add(new NodeRootViewModel(TreeViewModel, this, NodeType.DataSourcesRoot, "Orígenes de datos", true));
			Children.Add(new NodeRootViewModel(TreeViewModel, this, NodeType.DimensionsRoot, "Dimensiones", true));
			Children.Add(new NodeRootViewModel(TreeViewModel, this, NodeType.ReportsRoot, "Informes", true));
		}

		/// <summary>
		///		Almacén de datos asociado al nodo
		/// </summary>
		public DataWarehouseModel DataWarehouse { get; }
	}
}
