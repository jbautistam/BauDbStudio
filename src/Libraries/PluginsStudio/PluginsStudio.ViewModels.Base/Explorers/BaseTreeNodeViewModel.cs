using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

/// <summary>
///		ViewModel base de un nodo del árbol de exploración
/// </summary>
public abstract class BaseTreeNodeViewModel : ControlHierarchicalViewModel
{	
	public BaseTreeNodeViewModel(BaseTreeViewModel trvTree, ControlHierarchicalViewModel parent, string text, 
								 string type, string icon, object tag, 
								 bool lazyLoad, bool isBold = false, MvvmColor foreground = null) 
						: base(parent, text, tag, lazyLoad, isBold, foreground)
	{ 
		TreeViewModel = trvTree;
		Type = type;
		Icon = icon;
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
	///		Comprueba si dos nodos son iguales
	/// </summary>
	public virtual bool IsEquals(BaseTreeNodeViewModel node)
	{
		return Text.Equals(node.Text, StringComparison.CurrentCultureIgnoreCase) && Icon.Equals(node.Icon, StringComparison.CurrentCultureIgnoreCase);
	}

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
	public BaseTreeViewModel TreeViewModel { get; }

	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public string Type { get; protected set; }
}