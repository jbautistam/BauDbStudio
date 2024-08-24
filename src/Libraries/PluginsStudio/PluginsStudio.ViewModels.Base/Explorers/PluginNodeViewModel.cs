using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

/// <summary>
///		ViewModel base de un nodo de un árbol
/// </summary>
public abstract class PluginNodeViewModel : ControlHierarchicalViewModel
{	
	protected PluginNodeViewModel(PluginTreeViewModel trvTree, ControlHierarchicalViewModel? parent, string text, 
								  string type, string icon, object? tag, bool lazyLoad, bool isBold = false, MvvmColor? foreground = null) 
						: base(parent, text, type, icon, tag, lazyLoad, isBold, foreground)
	{ 
		TreeViewModel = trvTree;
	}

	/// <summary>
	///		Carga los nodos hijo
	/// </summary>
	public override void LoadChildrenData()
	{
		LoadNodes();
	}

	/// <summary>
	///		Carga los nodos hijo
	/// </summary>
	protected abstract void LoadNodes();

	/// <summary>
	///		Obtiene el calor pasado como parámetro o el color por defecto si no está activo
	/// </summary>
	public MvvmColor GetColor(MvvmColor defaultColor, bool enabled)
	{
		if (!enabled)
			return MvvmColor.Gray;
		else
			return defaultColor;
	}

	/// <summary>
	///		Obtiene el texto para las operaciones de drag & drop sobre el editor
	/// </summary>
	public virtual string GetTextForEditor(bool shiftPressed) => string.Empty;

	/// <summary>
	///		ViewModel del árbol
	/// </summary>
	public PluginTreeViewModel TreeViewModel { get; }
}