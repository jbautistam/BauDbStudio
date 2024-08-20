using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibBlogReader.Model;
using Bau.Libraries.LibBlogReader.ViewModel.Blogs.TreeBlogs;

namespace Bau.Libraries.LibBlogReader.ViewModel.Blogs.TreeFolders;

/// <summary>
///		ViewModel para un árbol de carpetas
/// </summary>
public class TreeFoldersViewModel : PluginsStudio.ViewModels.Base.Explorers.PluginTreeViewModel
{ 
	// Variables privadas
	private bool _isTreeUpdated;

	public TreeFoldersViewModel(BlogReaderViewModel mainViewModel, BauMvvm.ViewModels.BaseObservableObject? viewModelParent)
	{
		MainViewModel = mainViewModel;
		ViewModelParent = viewModelParent;
	}

	/// <summary>
	///		Carga los nodos del árbol
	/// </summary>
	protected override void AddRootNodes()
	{
		// Carga los nodos de carpetas
		LoadNodes(null, MainViewModel.BlogManager.File.Folders);
		// Indica que no ha habido modificaciones
		if (ViewModelParent is not null)
			ViewModelParent.IsUpdated = false;
	}

	/// <summary>
	///		Carga recursivamente los nodos hijos de un árbol de carpetas
	/// </summary>
	private void LoadNodes(FolderNodeViewModel? parent, FoldersModelCollection folders)
	{
		foreach (FolderModel folder in folders)
		{
			FolderNodeViewModel node = new(this, MainViewModel, parent, folder);

				// Asigna las propiedades al nodo
				node.IsExpanded = true;
				// Lo añade al árbol
				if (parent == null)
					Children.Add(node);
				else
					parent.Children.Add(node);
				// Añade el manejador de eventos
				if (node != null)
					node.PropertyChanged += (sender, evntArgs) =>
													{
														string propertyName = evntArgs.PropertyName ?? string.Empty;

															if (propertyName.EqualsIgnoreCase(nameof(FolderNodeViewModel.IsSelected)) ||
																	propertyName.EqualsIgnoreCase(nameof(FolderNodeViewModel.IsChecked)))
																IsTreeeUpdated = true;
													};
				// Carga los nodos hijo
				if (folder.Folders != null && folder.Folders.Count > 0)
					LoadNodes(node, folder.Folders);
		}
	}

	/// <summary>
	///		Comprueba si puede ejecutar una acción
	/// </summary>
	protected override bool CanExecuteAction(string action) => false;

	/// <summary>
	///		Abre el formulario de propiedades
	/// </summary>
	protected override void OpenProperties()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Borra un elemento
	/// </summary>
	protected override void DeleteItem()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public BlogReaderViewModel MainViewModel { get; }

	/// <summary>
	///		ViewModel padre
	/// </summary>
	public BauMvvm.ViewModels.BaseObservableObject? ViewModelParent { get; }

	/// <summary>
	///		Indica si se ha modificado el árbol
	/// </summary>
	public bool IsTreeeUpdated
	{
		get { return _isTreeUpdated; }
		set { CheckProperty(ref _isTreeUpdated, value); }
	}
}
