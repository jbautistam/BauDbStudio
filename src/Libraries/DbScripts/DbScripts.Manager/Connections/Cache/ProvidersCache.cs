using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbScripts.Manager.Connections.Cache;

/// <summary>
///		Caché de proveedores de base de datos para las conexiones
/// </summary>
internal class ProvidersCache
{
	/// <summary>
	///		Añade una conexión y un proveedor a la caché
	/// </summary>
	internal void Add(ConnectionModel connection, IDbProvider provider)
	{
		Providers.Add(connection.GlobalId, new ProviderCacheItem(provider, GetHashKey(connection)));
	}

	/// <summary>
	///		Obtiene un proveedor de la caché (o null si ya no existe o se han cambiado los datos de la conexión)
	/// </summary>
	internal IDbProvider? GetProvider(ConnectionModel connection)
	{
		ProviderCacheItem? provider = Providers[connection.GlobalId];

			// Si hemos obtenido un proveedor, comprobamos que la conexión siga siendo la misma
			if (provider is not null)
			{
				// Si no son iguales, se quita de la caché
				if (!provider.ConnectionHash.Equals(GetHashKey(connection), StringComparison.CurrentCultureIgnoreCase))
				{
					// Quita la conexión de la caché
					Providers.Remove(connection.GlobalId);
					// y por supuesto, no lo puede devolver
					provider = null;
				}
			}
			// Devuelve el proveedor
			if (provider is not null)
				return provider.Provider;
			else
				return null;
	}

	/// <summary>
	///		Compara con los datos de otra conexión. No compara el nombre ni la descripción ni la solución
	///	por eso no se sobrescribe Equals
	/// </summary>
	private string GetHashKey(ConnectionModel connection)
	{
		string hash = connection.Type.ToString();
		List<string> parameters = new();

			// Añáde los valores de los parámetros a la lista
			foreach ((string key, string value) in connection.Parameters.Enumerate())
				parameters.Add($"{key}|{value}");
			// Ordena la lista de parámetros
			parameters.Sort((first, second) => first.CompareTo(second));
			// Y añade los parámetros a la clave
			foreach (string parameter in parameters)
				hash += parameter;
			// Devuelve la clave
			return hash;
	}

	/// <summary>
	///		Caché de proveedores y conexiones
	/// </summary>
	internal NormalizedDictionary<ProviderCacheItem> Providers { get; } = new();
}
