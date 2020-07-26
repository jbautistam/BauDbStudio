using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.Views.Wpf.Forms.Trees;
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
		///		Obtiene el directorio del nodo más cercano a un elemento (en este caso, el que viene en un evento de drop)
		/// </summary>
		private string GetPathUnderElement(UIElement uiElement)
		{
			string path = string.Empty;
			TreeViewItem targetNode = GetNearestTreeNode(uiElement);

				// Obtiene el directorio del nodo seleccionado
				if (targetNode != null) 
					switch (targetNode.DataContext)
					{
						case NodeFileViewModel viewModel:
								path = viewModel.FileName;
							break;
						case NodeFolderRootViewModel viewModel:
								path = viewModel.FileName;
							break;
					}
				// Convierte el nombre de archivo en su nombre de directorio
				if (!System.IO.Directory.Exists(path))
					path = System.IO.Path.GetDirectoryName(path);
				// Devuelve el directorio del nodo
				return path;
		}

		/// <summary>
		///		Obtiene el contenedor más cercano a un elemento
		/// </summary>
        private TreeViewItem GetNearestTreeNode(UIElement element)
        {
            TreeViewItem treeItem = element as TreeViewItem;

				// Recorre el árbol hasta encontrar el nodo más cercano al elemento
				while ((treeItem == null) && (element != null))
				{
					element = VisualTreeHelper.GetParent(element) as UIElement;
					treeItem = element as TreeViewItem;
				}
				// Devuelve el nodo
				return treeItem;
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
							(Math.Abs(vctDifference.X) > SystemParameters.MinimumHorizontalDragDistance ||
							 Math.Abs(vctDifference.Y) > SystemParameters.MinimumVerticalDragDistance))
						_dragDropController.InitDragOperation(trvExplorer, trvExplorer.SelectedItem as IHierarchicalViewModel);
			}
		}

		private void trvExplorer_Drop(object sender, DragEventArgs e)
		{
			string path = GetPathUnderElement(e.OriginalSource as UIElement);

				if (e.Data.GetDataPresent(DataFormats.FileDrop) && !string.IsNullOrWhiteSpace(path) && System.IO.Directory.Exists(path))
					try
					{
						string [] files = (string []) e.Data.GetData(DataFormats.FileDrop);

							// Copia los archivos
							ViewModel.CopyFromExplorer(path, files);
							// Indica que se ha tratao
							e.Handled = true;
					}
					catch (Exception exception)
					{
						ViewModel.SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error("Error when drop files", exception);
					}
		}
	}
}
