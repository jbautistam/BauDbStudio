using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.Views.Forms.Trees;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers.Files;

namespace Bau.DbStudio.Views
{
	/// <summary>
	///		Arbol del explorador de archivos
	/// </summary>
	public partial class TreeFilesExplorer : UserControl
	{
		// Variables privadas
		private Point _startDrag;
		private DragDropTreeExplorerController _dragDropController = new DragDropTreeExplorerController();

		public TreeFilesExplorer(TreeFilesViewModel treeViewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = treeViewModel;
		}

		/// <summary>
		///		ViewModel del árbol de archivos
		/// </summary>
		public TreeFilesViewModel ViewModel { get; }

		private void trvExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{ 
			if (trvExplorer.DataContext is TreeFilesViewModel && (sender as TreeView)?.SelectedItem is BaseTreeNodeViewModel node)
				ViewModel.SelectedNode = node;
		}

		private void trvExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{ 
			if (trvExplorer.DataContext is TreeFilesViewModel && (sender as TreeView)?.SelectedItem is BaseTreeNodeViewModel node)
			{
				ViewModel.SelectedNode = node;
				ViewModel.OpenPropertiesCommand.Execute(null);
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
							(Math.Abs(vctDifference.X) > SystemParameters.MinimumHorizontalDragDistance ||
							 Math.Abs(vctDifference.Y) > SystemParameters.MinimumVerticalDragDistance))
						_dragDropController.InitDragOperation(trvExplorer, trvExplorer.SelectedItem as IHierarchicalViewModel);
			}
		}
	}
}
