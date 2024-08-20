using Bau.Libraries.LibDbProviders.Base;

namespace Bau.Libraries.DbScripts.Manager.Connections.Cache;

/// <summary>
///		Estructura con un proveedor y su conexión (no puede ser una tupla porque el proveedor es un interface)
/// </summary>
internal class ProviderCacheItem
{
	internal ProviderCacheItem(IDbProvider provider, string connectionHash)
	{
		Provider = provider;
		ConnectionHash = connectionHash;
	}

	/// <summary>
	///		Datos del proveedor
	/// </summary>
	internal IDbProvider Provider { get; }

	/// <summary>
	///		Hash con los datos de la conexión
	///	</summary>
	internal string ConnectionHash { get; }
}
