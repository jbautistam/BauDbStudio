using System;

namespace Bau.Libraries.DbStudio.Models.Cloud
{
	/// <summary>
	///		Modelo con los datos de un storage
	/// </summary>
	public class StorageModel : LibDataStructures.Base.BaseExtendedModel
	{
		public StorageModel(SolutionModel solution)
		{
			Solution = solution;
		}

		/// <summary>
		///		Obtiene la cadena de conexión normalizada
		/// </summary>
		public string GetNormalizedConnectionString()
		{
			string connectionString = StorageConnectionString;

				// Normaliza la cadena
				if (!string.IsNullOrWhiteSpace(connectionString))
				{
					connectionString = connectionString.Replace(Environment.NewLine, string.Empty);
					connectionString = connectionString.Replace("\n", string.Empty);
					connectionString = connectionString.Replace("\r", string.Empty);
					connectionString = connectionString.Replace("\t", string.Empty);
					while (!string.IsNullOrWhiteSpace(connectionString) && connectionString.IndexOf("; ") >= 0)
						connectionString = connectionString.Replace("; ", ";");
				}
				// Devuelve la cadena de conexión
				return connectionString;
		}

		/// <summary>
		///		Solución a la que se asocia el storage
		/// </summary>
		public SolutionModel Solution { get; }

		/// <summary>
		///		Cadena de conexión al storage
		/// </summary>
		public string StorageConnectionString { get; set; }
	}
}
