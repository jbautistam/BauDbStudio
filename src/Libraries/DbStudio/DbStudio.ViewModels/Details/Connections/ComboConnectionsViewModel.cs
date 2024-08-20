using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Connections;

/// <summary>
///		ViewModel con un combo de conexiones
/// </summary>
public class ComboConnectionsViewModel : BaseObservableObject
{
	// Variables privadas
	private ComboViewModel _connections = default!;

	public ComboConnectionsViewModel(DbStudioViewModel solutionViewModel, string? selectedConnection)
	{
		SolutionViewModel = solutionViewModel;
		LoadComboConnections(selectedConnection);
	}

	/// <summary>
	///		Carga el combo de conexiones
	/// </summary>
	private void LoadComboConnections(string? selectedConnection)
	{
		// Inicializa el combo
		Connections = new ComboViewModel(this);
		// Carga las conexiones
		if (SolutionViewModel.Solution.Connections.Count == 0)
			Connections.AddItem(null, "<Seleccione una conexión>", null);
		else
		{
			// Ordena las conexiones
			SolutionViewModel.Solution.Connections.SortByName();
			// Carga las conexiones
			foreach (ConnectionModel connection in SolutionViewModel.Solution.Connections)
			{
				// Añade la conexión
				Connections.AddItem(null, connection.Name, connection);
				// Selecciona la conexión que se acaba de añadir si es la conexión solicitada en el parámetro o si coincide con la conexión global
				if (!string.IsNullOrWhiteSpace(selectedConnection) && selectedConnection.Equals(connection.Name, StringComparison.CurrentCultureIgnoreCase))
					Connections.SelectedItem = Connections.Items[Connections.Items.Count - 1];
				else if (string.IsNullOrWhiteSpace(selectedConnection) &&
						 !string.IsNullOrWhiteSpace(SolutionViewModel.Solution.LastConnectionSelectedGlobalId) &&
						 connection.GlobalId.Equals(SolutionViewModel.Solution.LastConnectionSelectedGlobalId, StringComparison.CurrentCultureIgnoreCase))
					Connections.SelectedItem = Connections.Items[Connections.Items.Count - 1];
			}
		}
		// Si no se ha seleccionado nada, selecciona el primer elemento
		if (Connections.SelectedItem is null)
			Connections.SelectedItem = Connections.Items[0];
	}

	/// <summary>
	///		Selecciona una conexión
	/// </summary>
	public void SelectConnection(ConnectionModel connection)
	{
		Connections.SelectedTag = connection;
	}

	/// <summary>
	///		Obtiene la conexión seleccionada en el combo
	/// </summary>
	public ConnectionModel? GetSelectedConnection()
	{
		if (Connections.SelectedItem?.Tag is ConnectionModel connection)
			return connection;
		else
			return null;
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public DbStudioViewModel SolutionViewModel { get; }

	/// <summary>
	///		Combo de conexiones
	/// </summary>
	public ComboViewModel Connections
	{
		get { return _connections; }
		set { CheckObject(ref _connections, value); }
	}
}
