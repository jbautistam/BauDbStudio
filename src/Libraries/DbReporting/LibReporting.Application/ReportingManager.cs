namespace Bau.Libraries.LibReporting.Application;

/// <summary>
///		Manager para reporting
/// </summary>
public class ReportingManager
{
	public ReportingManager()
	{
		Schema = new Models.ReportingSchemaModel();
	}

	/// <summary>
	///		Añade un origen de datos
	/// </summary>
	public void AddDataWarehouse(Models.DataWarehouses.DataWarehouseModel dataWarehouse)
	{
		Schema.DataWarehouses.Add(dataWarehouse);
	}

	/// <summary>
	///		Elimina un origen de datos
	/// </summary>
	public void RemoveDataWarehouse(Models.DataWarehouses.DataWarehouseModel dataWarehouse)
	{
		Schema.DataWarehouses.Remove(dataWarehouse);
	}

	/// <summary>
	///		Obtiene la SQL resultante de procesar una solicitud de informe
	/// </summary>
	public string GetSqlResponse(Requests.Models.ReportRequestModel request) => new Controllers.ReportController(this).GetResponse(request);

	/// <summary>
	///		<see cref="Models.ReportingSchemaModel"/> con el que trabaja la aplicación
	/// </summary>
	public Models.ReportingSchemaModel Schema { get; }
}
