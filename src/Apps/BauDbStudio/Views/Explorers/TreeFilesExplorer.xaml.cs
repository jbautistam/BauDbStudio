using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.Logging;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.Views.Wpf.Forms.Trees;
using Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.DbStudio.Views.Explorers;

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
	private string GetPathUnderElement(UIElement? uiElement)
	{
		string path = string.Empty;
		TreeViewItem? targetNode = GetNearestTreeNode(uiElement);

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
				path = System.IO.Path.GetDirectoryName(path) ?? string.Empty;
			// Devuelve el directorio del nodo
			return path;
	}

	/// <summary>
	///		Obtiene el contenedor más cercano a un elemento
	/// </summary>
    private TreeViewItem? GetNearestTreeNode(UIElement? element)
    {
        TreeViewItem? treeItem = element as TreeViewItem;

			// Recorre el árbol hasta encontrar el nodo más cercano al elemento
			while ((treeItem is null) && (element is not null))
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
				Separator? lastSeparator = null;

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
	private MenuItem CreateMenu(string text, string icon, bool isCheckable, ICommand? command, string? fileName, object? tag = null)
	{
		MenuItem mnuNewItem = new();

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
		if (trvExplorer.DataContext is TreeFilesViewModel)
			ViewModel.SelectedNode = (sender as TreeView)?.SelectedItem as PluginNodeViewModel;
	}

	private void trvExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{ 
		if (trvExplorer.DataContext is TreeFilesViewModel && (sender as TreeView)?.SelectedItem is PluginNodeViewModel node)
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

				if (pntMouse.X < trvExplorer.ActualWidth - 50 && pntMouse.Y < trvExplorer.ActualHeight - 50 &&
						(Math.Abs(vctDifference.X) > SystemParameters.MinimumHorizontalDragDistance ||
						 Math.Abs(vctDifference.Y) > SystemParameters.MinimumVerticalDragDistance))
					_dragDropController.InitDragOperation(trvExplorer, trvExplorer.SelectedItem as ControlHierarchicalViewModel);
		}
	}

	private void trvExplorer_Drop(object sender, DragEventArgs e)
	{
		string pathTarget = GetPathUnderElement(e.OriginalSource as UIElement);

			if (!string.IsNullOrWhiteSpace(pathTarget) && System.IO.Directory.Exists(pathTarget))
			{
				bool move = e.KeyStates.HasFlag(DragDropKeyStates.ShiftKey) || e.KeyStates.HasFlag(DragDropKeyStates.ControlKey);

					if (e.Data.GetDataPresent(DataFormats.FileDrop))
					{	
							// Indica que se ha manejado el evento
							e.Handled = true;
							// Copia los datos
							try
							{
								string [] files = (string []) e.Data.GetData(DataFormats.FileDrop);

									// Copia los archivos
									ViewModel.CopyFiles(pathTarget, files, move);
							}
							catch (Exception exception)
							{
								ViewModel.MainViewModel.MainController.MainWindowController.Logger.LogError(exception, "Error when drop files");
							}
					}
					else
					{
						ControlHierarchicalViewModel? source = _dragDropController.GetDragDropFileNode(e.Data);
						string fileSource = string.Empty;

							// Indica que se ha manejado el evento
							e.Handled = true;
							// Dependiendo del tipo de nodo obtiene el nombre de archivo / directorio donde se deben copiar los datos
							switch (source)
							{
								case NodeFileViewModel node:
										fileSource = node.FileName;
									break;
								case NodeFolderRootViewModel node:
										fileSource = node.FileName;
									break;
							}
							// Si no está vacío
							if (!string.IsNullOrWhiteSpace(fileSource))
								ViewModel.CopyFiles(pathTarget, [fileSource], move);
				}
			}
	}

	private void trvExplorer_ContextMenuOpening(object sender, ContextMenuEventArgs e)
	{
		PrepareContextualMenus();
	}

	private void RenameCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
	{
		ViewModel.RenameCommand.Execute(null);
	}

	private void RenameCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
	{
		e.CanExecute = ViewModel.RenameCommand.CanExecute(null);
	}
}