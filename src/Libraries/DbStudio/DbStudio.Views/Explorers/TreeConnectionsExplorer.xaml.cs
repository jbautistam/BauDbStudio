using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.Views.Wpf.Forms.Trees;
using Bau.Libraries.DbStudio.ViewModels.Explorers.Connections;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.Views.Explorers;

/// <summary>
///		Arbol del explorador de conexiones
/// </summary>
public partial class TreeConnectionsExplorer : UserControl
{
	// Variables privadas
	private Point _startDrag;
	private DragDropTreeController _dragDropController;

	public TreeConnectionsExplorer(TreeConnectionsViewModel treeViewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = treeViewModel;
		_dragDropController = new DragDropTreeController(this, "BaseNodeViewModel");
	}

	/// <summary>
	///		ViewModel del árbol de soluciones
	/// </summary>
	public TreeConnectionsViewModel ViewModel { get; }

	private void trvExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{ 
		if (trvExplorer.DataContext is TreeConnectionsViewModel && (sender as TreeView)?.SelectedItem is PluginNodeViewModel node)
			ViewModel.SelectedNode = node;
	}

	private void trvExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{ 
		if (trvExplorer.DataContext is TreeConnectionsViewModel && (sender as TreeView)?.SelectedItem is PluginNodeViewModel node)
		{
			ViewModel.SelectedNode = node;
			ViewModel.OpenCommand.Execute(null);
		}
	}

	private void trvExplorer_MouseDown(object sender, MouseButtonEventArgs e)
	{ 
		if (e.ChangedButton == MouseButton.Left)
			ViewModel.SelectedNode = null;
	}

	private void trvExplorer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		_startDrag = e.GetPosition(null);
	}

	private void trvExplorer_PreviewMouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed)
		{
			Point pntMouse = e.GetPosition(null);
			Vector vctDifference = _startDrag - pntMouse;

				if (pntMouse.X < trvExplorer.ActualWidth - 50 &&
					pntMouse.Y < trvExplorer.ActualHeight - 50 &&
						(Math.Abs(vctDifference.X) > SystemParameters.MinimumHorizontalDragDistance ||
						 Math.Abs(vctDifference.Y) > SystemParameters.MinimumVerticalDragDistance))
					_dragDropController.InitDragOperation(trvExplorer, trvExplorer.SelectedItem as ControlHierarchicalViewModel);
		}
	}
}
