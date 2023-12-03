using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

/// <summary>
///		ViewModel base de un nodo del árbol de exploración de una solución con carga asíncrona de nodos
/// </summary>
public abstract class PluginNodeAsyncViewModel : PluginNodeViewModel
{	
	protected PluginNodeAsyncViewModel(PluginTreeViewModel trvTree, ControlHierarchicalViewModel? parent, string text, 
										 string type, string icon, object? tag, bool lazyLoad, bool isBold = false, MvvmColor? foreground = null) 
						: base(trvTree, parent, text, type, icon, tag, lazyLoad, isBold, foreground)
	{ 
	}

	/// <summary>
	///		Carga los nodos hijo
	/// </summary>
	protected override void LoadNodes()
	{
		// Muestra el nodo de carga
		Children.Add(GetNodeLoading());
		// Carga los nodos
		Task.Run(async () => await LoadNodesAsync(new CancellationToken()));
	}

	/// <summary>
	///		Carga los nodos del árbol de forma asíncrona
	/// </summary>
	protected async Task LoadNodesAsync(CancellationToken cancellationToken)
	{
		object state = new object();
		List<PluginNodeViewModel>? nodes = await GetChildNodesAsync(cancellationToken);

			// Carga los nodos en el árbol
			//? _contexUi mantiene el contexto de sincronización que creó el ViewModel (que debería ser la interface de usuario)
			//? Al generarse las tablas en otro hilo, no se puede añadir a ObservableCollection sin una
			//? excepción del tipo "Este tipo de CollectionView no admite cambios en su SourceCollection desde un hilo diferente del hilo Dispatcher"
			//? Por eso se tiene que añadir el mensaje de log desde el contexto de sincronización de la UI
			TreeViewModel.ContextUI.Send(_ => {
												try
												{
													// Limpia la lista de nodos
													Children.Clear();
													// Añade la lista de contenedores
													if (nodes is not null)
														foreach (PluginNodeViewModel node in nodes)
															Children.Add(node);
												}
												catch (Exception exception)
												{
													System.Diagnostics.Trace.TraceError($"Error al cargar los nodos. {exception.Message}");
												}
											 },
										state);
	}

	/// <summary>
	///		Obtiene el nodo de carga
	/// </summary>
	protected virtual PluginNodeViewModel GetNodeLoading() => new PluginNodeMessageViewModel(TreeViewModel, this, "Loading ...");

	/// <summary>
	///		Obtiene la lista de nodos hijo
	/// </summary>
	protected abstract Task<List<PluginNodeViewModel>?> GetChildNodesAsync(CancellationToken cancellationToken);
}