namespace Bau.Libraries.DbScripts.Manager.Models;

/// <summary>
///		Lista de argumentos que se pasa a una conexión para hacer una consulta
/// </summary>
public class ArgumentListModel
{
	/// <summary>
	///		Constantes: identificadas en SQL como {{name}}
	/// </summary>
	public LibDataStructures.Collections.NormalizedDictionary<object> Constants { get; } = new();

	/// <summary>
	///		Parámetros: identificados en SQL como $name
	/// </summary>
	public LibDataStructures.Collections.NormalizedDictionary<object?> Parameters { get; } = new();
}
