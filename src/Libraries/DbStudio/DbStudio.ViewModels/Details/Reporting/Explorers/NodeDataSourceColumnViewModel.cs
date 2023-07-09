using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers;

/// <summary>
///		ViewModel de un nodo de un campo de un origen de datos
/// </summary>
public class NodeDataSourceColumnViewModel : PluginNodeViewModel
{
	public NodeDataSourceColumnViewModel(PluginTreeViewModel trvTree, PluginNodeViewModel parent, DataSourceColumnModel column) : 
				base(trvTree, parent, column.Id, TreeReportingViewModel.NodeType.Table.ToString(), TreeReportingViewModel.IconType.Field.ToString(), column, false)
	{
		Column = column;
		if (column.IsPrimaryKey)
			Icon = TreeReportingViewModel.IconType.Key.ToString();
		if (!column.Visible)
			Foreground = BauMvvm.ViewModels.Media.MvvmColor.Gray;
		if (!string.IsNullOrWhiteSpace(column.Alias) && !column.Alias.Equals(column.Id, StringComparison.CurrentCultureIgnoreCase))
			Text = $"{column.Id} ({column.Alias})";
	}

	/// <summary>
	///		Carga los nodos del campo
	/// </summary>
	protected override void LoadNodes()
	{
		// No hace nada, simplemente implementa la clase
	}

	/// <summary>
	///		Campo asociado al nodo
	/// </summary>
	public DataSourceColumnModel Column { get; }
}
