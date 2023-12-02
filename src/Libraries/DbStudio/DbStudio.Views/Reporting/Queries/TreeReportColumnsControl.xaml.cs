using System.Windows;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Queries;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.Views.Reporting.Queries;

/// <summary>
///		Arbol con el filtro de columnas del informe
/// </summary>
public partial class TreeReportColumnsControl : UserControl
{
	public TreeReportColumnsControl()
	{
		InitializeComponent();
	}

	/// <summary>
	///		Carga el viewModel del control
	/// </summary>
	public void LoadControl(TreeQueryReportViewModel viewModel)
	{
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel del árbol de columnas
	/// </summary>
	public TreeQueryReportViewModel ViewModel { get; private set; } = default!;

	private void trvExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{ 
		if (trvExplorer.DataContext is TreeQueryReportViewModel && (sender as TreeView)?.SelectedItem is PluginNodeViewModel node)
			ViewModel.SelectedNode = node;
	}
}
