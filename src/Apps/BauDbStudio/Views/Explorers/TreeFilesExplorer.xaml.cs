using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.Views.Wpf.Forms.Trees;
using Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.DbStudio.Views.Explorers
{
	/// <summary>
	///		Arbol del explorador de archivos
	/// </summary>
	public partial class TreeFilesExplorer : UserControl
	{
		// Variables privadas
		private Point _startDrag;
		private DragDropTreeController _dragDropController;

		public TreeFilesExplorer(TreeFilesViewModel treeViewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = treeViewModel;
			_dragDropController = new DragDropTreeController(this, "BaseNodeViewModel", true);
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
		///		Prepara los menús contextuales
		/// </summary>
		private void PrepareContextualMenus()
		{
			ContextMenu explorerMenus = trvExplorer.ContextMenu;

				// Crea los menús contextuales
				if (explorerMenus != null)
				{
					List<Libraries.PluginsStudio.ViewModels.Base.Models.MenuModel> menuItems = ViewModel.GetFileMenus();
					int lastSeparatorIndex = 0;
					Separator lastSeparator = null;

						// Busca el último separador (el de los plugins)
						for (int index = 0; index < explorerMenus.Items.Count; index++)
							if (explorerMenus.Items[index] is Separator separator && separator.Name.Equals("mnuPluginsSeparator", StringComparison.CurrentCultureIgnoreCase))
							{
								lastSeparatorIndex = index;
								lastSeparator = separator;
							}
						// Si se ha encontrado el separador (que siempre debería estar ahí)
						if (lastSeparator != null)
						{
							// Quita las opciones posteriores al separador
							for (int index = explorerMenus.Items.Count - 1; index > lastSeparatorIndex; index--)
								explorerMenus.Items.RemoveAt(index);
							// Añade las opciones de menú
							if (menuItems.Count == 0)
								lastSeparator.Visibility = Visibility.Collapsed;
							else
							{
								// Muestra el separador
								lastSeparator.Visibility = Visibility.Visible;
								// Añade los menús
								foreach (Libraries.PluginsStudio.ViewModels.Base.Models.MenuModel menuItem in menuItems)
									if (string.IsNullOrWhiteSpace(menuItem.Header))
										explorerMenus.Items.Insert(++lastSeparatorIndex, new Separator());
									else
										explorerMenus.Items.Insert(++lastSeparatorIndex, 
																   CreateMenu(menuItem.Header, menuItem.Icon, menuItem.IsCheckable, menuItem.Command, 
																			  (ViewModel.SelectedNode as NodeFileViewModel)?.FileName, 
																			  menuItem.Tag));
							}
						}
				}
		}

		/// <summary>
		///		Crea una opción de menú
		/// </summary>
		private MenuItem CreateMenu(string text, string icon, bool isCheckable, ICommand command, string fileName, object tag = null)
		{
			MenuItem mnuNewItem = new MenuItem();

				// Asigna las propiedades
				mnuNewItem.Header = text;
				if (!string.IsNullOrWhiteSpace(icon))
					mnuNewItem.Icon = new Libraries.BauMvvm.Views.Wpf.Tools.ToolsWpf().GetImage(icon);
				mnuNewItem.Tag = tag;
				mnuNewItem.IsCheckable = isCheckable;
				// Añade el comando
				mnuNewItem.Command = command;
				mnuNewItem.CommandParameter = fileName; 
				// Devuelve la opción de menú creada
				return mnuNewItem;
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
						pntMouse.Y < trvExplorer.ActualHeight - 50 &&
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
						ViewModel.MainViewModel.PluginsStudioController.MainWindowController.Logger.Default.LogItems.Error("Error when drop files", exception);
					}
		}

		private void trvExplorer_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			PrepareContextualMenus();
		}
	}
}
