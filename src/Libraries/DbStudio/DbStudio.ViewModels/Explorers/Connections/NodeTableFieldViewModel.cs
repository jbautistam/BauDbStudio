using System;

using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections
{
	/// <summary>
	///		ViewModel de un nodo de un campo de una tabla
	/// </summary>
	public class NodeTableFieldViewModel : BaseTreeNodeViewModel
	{
		public NodeTableFieldViewModel(TreeSolutionBaseViewModel trvTree, NodeTableViewModel parent, ConnectionTableFieldModel field) : 
					base(trvTree, parent, field.FullName, NodeType.Table, IconType.Field, field, false)
		{
			Field = field;
			if (field.IsKey)
				Icon = IconType.Key;
		}

		/// <summary>
		///		Carga los nodos del campo
		/// </summary>
		protected override void LoadNodes()
		{
			// No hace nada, simplemente implementa la clase
		}

		/// <summary>
		///		Obtiene el texto de la cadena SQL asociada al campo
		/// </summary>
		public string GetSqlSelect(bool fullSql)
		{
			if (TreeViewModel is TreeConnectionsViewModel trvTree)
				return trvTree.GetSqlSelect(this, fullSql);
			else
				return string.Empty;
		}

		/// <summary>
		///		Campo asociado al nodo
		/// </summary>
		public ConnectionTableFieldModel Field { get; }
	}
}
