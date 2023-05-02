namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

/// <summary>
///		ViewModel de un mensaje sobre el árbol
/// </summary>
public class PluginNodeMessageViewModel : PluginNodeViewModel
{
	public PluginNodeMessageViewModel(PluginTreeViewModel trvTree, PluginNodeViewModel parent, string message, string type = "Message", string icon = "Loading") : 
				base(trvTree, parent, message, type, icon, null, false, false, BauMvvm.ViewModels.Media.MvvmColor.DarkGreen)
	{
	}

	/// <summary>
	///		Carga los nodos
	/// </summary>
	protected override void LoadNodes()
	{
		// No hace nada, sólo implementa la interface
	}
}
