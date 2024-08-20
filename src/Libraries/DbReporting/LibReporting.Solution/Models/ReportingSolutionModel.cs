using Bau.Libraries.LibReporting.Models.DataWarehouses;

namespace Bau.Libraries.LibReporting.Solution.Models;

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
	public string GetFileName(DataWarehouseModel dataWarehouse)
	{
		// Busca el nombre de archivo
		foreach ((string solutionDataWarehouse, string file) in DataWarehousesFiles)
			if (solutionDataWarehouse.Equals(dataWarehouse.Id, StringComparison.CurrentCultureIgnoreCase))
				return file;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return string.Empty;
	}

	/// <summary>
	///		Nombres de archivos
	/// </summary>
	public List<string> Files { get; } = new();

	/// <summary>
	///		Dicionario de relación entre almacenes de datos y archivos
	/// </summary>
	public List<(string dataWarehouseId, string fileName)> DataWarehousesFiles { get; } = new();
}