using System;

using Bau.Libraries.LibDataStructures.Collections;

namespace Bau.Libraries.LibJobProcessor.Cloud.Models
{
	/// <summary>
	///		Parámetros de acceso a la nube
	/// </summary>
	internal class CloudConnection
	{
		/// <summary>
		///		Tipo de servicios de la nube
		/// </summary>
		internal enum CloudType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Servicios de Azure</summary>
			Azure
		}

		/// <summary>
		///		Clave de la configuración
		/// </summary>
		internal string Key { get; set; }

		/// <summary>
		///		Tipo de servicio
		/// </summary>
		internal CloudType Type { get; set; }

		/// <summary>
		///		Cadena de conexión
		/// </summary>
		internal string StorageConnectionString { get; set; }

		/// <summary>
		///		Parámetros de conexión al servicio de la nube
		/// </summary>
		public NormalizedDictionary<string> Parameters { get; } = new NormalizedDictionary<string>();
	}
}
