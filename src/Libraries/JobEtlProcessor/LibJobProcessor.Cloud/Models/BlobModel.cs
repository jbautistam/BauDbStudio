using System;

namespace Bau.Libraries.LibJobProcessor.Cloud.Models
{
	/// <summary>
	///		Clase con los datos de un blob
	/// </summary>
	internal class BlobModel
	{
		/// <summary>
		///		Comprueba los datos del blob
		/// </summary>
		internal string Validate()
		{
			if (string.IsNullOrWhiteSpace(Container))
				return "Container name undefined";
			else if (string.IsNullOrWhiteSpace(Blob))
				return "Blob name undefined";
			else
				return string.Empty;
		}

		/// <summary>
		///		Sobrescribe ToString
		/// </summary>
		public override string ToString()
		{
			return $"{Container}/{Blob}";
		}

		/// <summary>
		///		Nombre del contenedor
		/// </summary>
		internal string Container { get; set; }

		/// <summary>
		///		Nombre del blob
		/// </summary>
		internal string Blob { get; set; }
	}
}
