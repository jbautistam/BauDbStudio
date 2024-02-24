using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.LibEBooks.Models.eBook;

namespace Bau.Libraries.EbooksReader.ViewModel.Reader.eBooks.Explorer;

/// <summary>
///		Clase base para los nodos del árbol <see cref="TreeEbookViewModel"/>
/// </summary>
public class EbookNodeViewModel : PluginNodeViewModel
{
	public EbookNodeViewModel(TreeEbookViewModel trvTree, ControlHierarchicalViewModel? parent, string text, TreeEbookViewModel.NodeType type, 
							  string urlBase, object? tag)
							: base(trvTree, parent, text, type.ToString(), string.Empty, tag, false, type == TreeEbookViewModel.NodeType.Package)
	{
		UrlBase = urlBase;
		switch (type)
		{
			case TreeEbookViewModel.NodeType.Package:
					Foreground = BauMvvm.ViewModels.Media.MvvmColor.Navy;
				break;
			case TreeEbookViewModel.NodeType.Page:
					Foreground = BauMvvm.ViewModels.Media.MvvmColor.Black;
				break;
		}
	}

	/// <summary>
	///		Carga los nodos
	/// </summary>
	protected override void LoadNodes()
	{
		// en este caso no hace nada, simplemente implementa la interface
	}

	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public TreeEbookViewModel.NodeType NodeType => Type.GetEnum(TreeEbookViewModel.NodeType.Unknown);

	/// <summary>
	///		UrlBase
	/// </summary>
	public string UrlBase { get; }

	/// <summary>
	///		Url de la página asociada al nodo
	/// </summary>
	public string Url
	{
		get
		{
			if (Tag != null && Tag is IndexItem page)
			{
				if (!string.IsNullOrWhiteSpace(UrlBase))
					return Path.Combine(UrlBase, page.URL);
				else
					return page.URL;
			}
			else
				return string.Empty;
		}
	}
}
