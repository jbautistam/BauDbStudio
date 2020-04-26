using System;

using Bau.Libraries.LibDataStructures.Collections;

namespace Bau.Libraries.LibJobProcessor.Database.Models
{
	/// <summary>
	///		Conexión a base de datos
	/// </summary>
	public class DataBaseConnectionModel : LibDataStructures.Base.BaseExtendedModel
	{
		/// <summary>
		///		Tipo de base de datos
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		///		Parámetros de conexión a la base de datos
		/// </summary>
		public NormalizedDictionary<string> Parameters { get; } = new NormalizedDictionary<string>();
	}
}
