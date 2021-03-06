using System;
using System.Collections.Generic;

using Bau.Libraries.LibReporting.Models.DataWarehouses;

namespace Bau.Libraries.LibReporting.Models
{
	/// <summary>
	///		Clase con los datos de una solución para reporting
	/// </summary>
	public class ReportingSolutionModel
	{
		/// <summary>
		///		Limpia la solución
		/// </summary>
		public void Clear()
		{
			Files.Clear();
			DataWarehousesFiles.Clear();
		}

		/// <summary>
		///		Obtiene el nombre de archivo asociado con un almacén en la solución
		/// </summary>
		internal string GetFileName(DataWarehouseModel dataWarehouse)
		{
			// Busca el nombre de archivo
			foreach ((string solutionDataWarehouse, string file) in DataWarehousesFiles)
				if (solutionDataWarehouse.Equals(dataWarehouse.Id, StringComparison.CurrentCultureIgnoreCase))
					return file;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return string.Empty;
		}

		/// <summary>
		///		Nombre del archivo de solución
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		///		Nombres de archivos
		/// </summary>
		public List<string> Files { get; } = new List<string>();

		/// <summary>
		///		Dicionario de relación entre almacenes de datos y archivos
		/// </summary>
		public List<(string dataWarehouseId, string fileName)> DataWarehousesFiles { get; } = new List<(string dataWarehouseId, string fileName)>();
	}
}