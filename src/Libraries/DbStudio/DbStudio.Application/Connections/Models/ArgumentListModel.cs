using System;

namespace Bau.Libraries.DbStudio.Application.Connections.Models
{
	/// <summary>
	///		Lista de argumentos que se pasa a una conexión para hacer una consulta
	/// </summary>
	public class ArgumentListModel
	{
		/// <summary>
		///		Constantes: identificadas en SQL como {{name}}
		/// </summary>
		public LibDataStructures.Collections.NormalizedDictionary<object> Constants { get; } = new LibDataStructures.Collections.NormalizedDictionary<object>();

		/// <summary>
		///		Parámetros: indentificados en SQL como $name
		/// </summary>
		public LibDataStructures.Collections.NormalizedDictionary<object> Parameters { get; } = new LibDataStructures.Collections.NormalizedDictionary<object>();
	}
}
