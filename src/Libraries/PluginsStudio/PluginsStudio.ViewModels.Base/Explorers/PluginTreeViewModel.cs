using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

/// <summary>
///		ViewModel base de los árboles para los plugins
/// </summary>
public abstract class PluginTreeViewModel : TreeViewModel
{
	protected PluginTreeViewModel()
	{
		OpenCommand = new BaseCommand(_ => OpenProperties(), _ => CanExecuteAction(nameof(OpenCommand)))
									.AddListener(this, nameof(SelectedNode));
		DeleteCommand = new BaseCommand(_ => DeleteItem(), _ => CanExecuteAction(nameof(DeleteCommand)))
									.AddListener(this, nameof(SelectedNode));
	}

	/// <summary>
	///		Comprueba si se puede ejecutar una acción
	/// </summary>
	protected abstract bool CanExecuteAction(string action);

	/// <summary>
	///		Abre la ventana de propiedades de un nodo
	/// </summary>
	protected abstract void OpenProperties();

	/// <summary>
	///		Borra el elemento seleccionado
	/// </summary>
	protected abstract void DeleteItem();

	/// <summary>
	///		Comando para abrir la ventana de propiedades
	/// </summary>
	public BaseCommand OpenCommand { get; }

	/// <summary>
	///		Comando para borrar un nodo
	/// </summary>
	public BaseCommand DeleteCommand { get; }
}