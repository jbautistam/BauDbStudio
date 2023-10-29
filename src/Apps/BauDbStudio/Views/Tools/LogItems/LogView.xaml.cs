using System.Windows.Controls;

using Bau.Libraries.PluginsStudio.ViewModels.Tools.LogItems;

namespace Bau.DbStudio.Views.Tools.Log;

/// <summary>
///		Vista para los log
/// </summary>
public partial class LogView : UserControl
{
	public LogView(LogListViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel de log
	/// </summary>
	public LogListViewModel ViewModel { get; }

	private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if ((sender as ListView)?.SelectedItem is LogListItemViewModel logItem)
			logItem.ShowDetailsCommand.Execute(null);
	}
}
