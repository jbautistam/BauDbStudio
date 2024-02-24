using Bau.Libraries.LibEBooks.Models.eBook;

namespace Bau.Libraries.EbooksReader.ViewModel.Reader.eBooks.Explorer;

/// <summary>
///		ViewModel para el árbol de páginas de un libro
/// </summary>
public class TreeEbookViewModel : PluginsStudio.ViewModels.Base.Explorers.PluginTreeViewModel
{   
	// Enumerados públicos
	/// <summary>Tipo de nodo</summary>
	public enum NodeType
	{
		Unknown,
		Package,
		Page
	}

	public TreeEbookViewModel(EBookContentViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
	}

	/// <summary>
	///		Carga los nodos
	/// </summary>
	protected override void AddRootNodes()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Carga los nodos con las páginas de un libro
	/// </summary>
	internal List<EbookNodeViewModel> Load(Book eBook)
	{
		List<EbookNodeViewModel> nodesPages = new();

			// Añade las páginas de la tabla de contenido
			AddPages(null, eBook.TableOfContent, nodesPages);
			// Añade las páginas del índice
			AddIndexPages(eBook.Index, nodesPages);
			// Devuelve la lista de páginas
			return nodesPages;
	}

	/// <summary>
	///		Añade las páginas del índice
	/// </summary>
	private void AddIndexPages(IndexItemsCollection indexPages, List<EbookNodeViewModel> nodesPages)
	{
		EbookNodeViewModel node = new(this, null, "Index", NodeType.Package, string.Empty, null);

			// Lo añade al árbol (pero no lo expande)
			Children.Add(node);
			// y añade las páginas de índice
			AddPages(node, indexPages, nodesPages);
	}

	/// <summary>
	///		Añade las páginas
	/// </summary>
	private void AddPages(EbookNodeViewModel? parent, IndexItemsCollection indexPages, List<EbookNodeViewModel> nodesPages)
	{
		foreach (IndexItem index in indexPages)
		{
			EbookNodeViewModel node = new(this, parent, index.Name, GetNodeType(index.URL), string.Empty, index);

				// Añade el nodo
				if (parent == null)
					Children.Add(node);
				else
					parent.Children.Add(node);
				// Expande el nodo
				node.IsExpanded = true;
				// Si es una página la añade a la lista de nodos de páginas
				if (node.NodeType == NodeType.Page)
					nodesPages.Add(node);
				// Añade las páginas hija
				AddPages(node, index.Items, nodesPages);
		}
	}

	/// <summary>
	///		Obtiene el tipo de nodo
	/// </summary>
	public NodeType GetNodeType(string url)
	{
		if (string.IsNullOrWhiteSpace(url))
			return NodeType.Package;
		else
			return NodeType.Page;
	}

	/// <summary>
	///		Comprueba si se puede ejecutar una acción
	/// </summary>
	protected override bool CanExecuteAction(string action) => false;

	/// <summary>
	///		Abre la ventana de propiedades
	/// </summary>
	protected override void OpenProperties()
	{
	}

	/// <summary>
	///		Borra un elemento
	/// </summary>
	protected override void DeleteItem()
	{
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public EBookContentViewModel MainViewModel { get; }
}
