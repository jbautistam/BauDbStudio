using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project.Connections;

/// <summary>
///		Elemento de <see cref="ConnectionsListViewModel"/>
/// </summary>
public class ConnectionListItemViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
{
	// Variables privadas 
	private string _name = default!, _url = default!;
	private int _timeout;

	public ConnectionListItemViewModel(ConnectionsListViewModel connectionsListViewModel, ConnectionModel connection) : base(connection.Name, connection)
	{
		// Inicializa los objetos
		ConnectionsListViewModel = connectionsListViewModel;
		Connection = connection;
		// Inicializa las propiedades
		InitProperties();
	}

	/// <summary>
	///		Inicializa las propiedades
	/// </summary>
	private void InitProperties()
	{
		Name = Connection.Name;
		Url = Connection.Url?.ToString() ?? string.Empty;
		Timeout = (int) Connection.Timeout.TotalSeconds;
	}

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public ConnectionsListViewModel ConnectionsListViewModel { get; }
	
	/// <summary>
	///		Datos de la conexión
	/// </summary>
	public ConnectionModel Connection { get; }

	/// <summary>
	///		Nombre de la conexión
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Url de la conexión
	/// </summary>
	public string Url
	{
		get { return _url; }
		set { CheckProperty(ref _url, value); }
	}

	/// <summary>
	///		Tiempo de espera
	/// </summary>
	public int Timeout
	{
		get { return _timeout; }
		set { CheckProperty(ref _timeout, value); }
	}
}