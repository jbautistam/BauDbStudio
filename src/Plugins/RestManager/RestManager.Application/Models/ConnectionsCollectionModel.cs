namespace Bau.Libraries.RestManager.Application.Models;

/// <summary>
///		Conexiones
/// </summary>
public class ConnectionsCollectionModel : List<ConnectionModel>
{
	/// <summary>
	///		Busca un elemento
	/// </summary>
	public ConnectionModel? Search(string id) => this.FirstOrDefault(item => item.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));
}
