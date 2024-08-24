using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

/// <summary>
///		ViewModel de un comando sobre el árbol
/// </summary>
public abstract class PluginNodeCommandViewModel : PluginNodeViewModel
{
	protected PluginNodeCommandViewModel(PluginTreeViewModel trvTree, ControlHierarchicalViewModel? parent, string text, 
										 string type, string icon, object? tag, bool isBold = true, 
										 BauMvvm.ViewModels.Media.MvvmColor? foreground = null) : 
				base(trvTree, parent, text, type, icon, tag, false, isBold, foreground)
	{
		ExecuteCommand = new BauMvvm.ViewModels.BaseCommand(_ => Execute());
	}

	/// <summary>
	///		Carga los nodos
	/// </summary>
	protected override void LoadNodes()
	{
		// No hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Ejecuta el comando asociado al nodo
	/// </summary>
	protected abstract void Execute();

	/// <summary>
	///		Comando para ejecución
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand ExecuteCommand { get; }
}
