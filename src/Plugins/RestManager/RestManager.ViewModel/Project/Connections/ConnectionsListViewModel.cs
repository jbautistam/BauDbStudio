using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project.Connections;

/// <summary>
///		Lista de conexiones de un proyecto Rest
/// </summary>
public class ConnectionsListViewModel : BauMvvm.ViewModels.Forms.ControlItems.ListView.ControlGenericListViewModel<ConnectionListItemViewModel>
{
	public ConnectionsListViewModel(RestFileViewModel fileViewModel)
	{
		FileViewModel = fileViewModel;
	}

	/// <summary>
	///		Añade una conexión
	/// </summary>
	public void AddConnections(ConnectionsCollectionModel connections)
	{
		foreach (ConnectionModel connection in connections)
			Items.Add(new ConnectionListItemViewModel(this, connection));
	}

	/// <summary>
	///		Añade / modifica la conexión
	/// </summary>
	protected override void UpdateItem(ConnectionListItemViewModel? item)
	{
		ConnectionViewModel updated = new(this, item?.Connection);

			// Abre el cuadro de diálogo y recoge los valores
			if (FileViewModel.MainViewModel.ViewsController.OpenDialog(updated) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				// Añade / modifica el elemento
				if (item is null)
					Items.Add(new ConnectionListItemViewModel(this, updated.Connection));
				else
					item = new ConnectionListItemViewModel(this, updated.Connection);
				// Indica que se ha modificado el proyecto
				FileViewModel.IsUpdated = true;
			}
	}

	/// <summary>
	///		Obtiene los datos de las conexiones
	/// </summary>
	internal ConnectionsCollectionModel GetConnections()
	{
		ConnectionsCollectionModel connections = [];

			// Añade las conexiones
			foreach (ConnectionListItemViewModel item in Items)
				connections.Add(item.Connection);
			// Devuelve la conexiones
			return connections;
	}

	/// <summary>
	///		Borra la conexión
	/// </summary>
	protected override void DeleteItem(ConnectionListItemViewModel item)
	{
		if (FileViewModel.MainViewModel.ViewsController.HostController.
						SystemController.ShowQuestionCancel($"Do you want to delete the connection '{item.Name}'?") == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
		{
			// Elimina el elemento
			Items.Remove(item);
			// Indica que se ha modificado el archivo
			FileViewModel.IsUpdated = true;
		}
	}

	/// <summary>
	///		ViewModel del archivo
	/// </summary>
	public RestFileViewModel FileViewModel { get; }
}