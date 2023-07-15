using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers;

/// <summary>
///		Controlador para informes
/// </summary>
internal class ReportController
{
	internal ReportController(ReportingManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Obtiene la respuesta a una consulta sobre un informe
	/// </summary>
	internal string GetResponse(ReportRequestModel request)
	{
		return SearchReport(request.ReportId) switch
					{ 
						ReportModel report => new Queries.ReportQueryGenerator(Manager.Schema, report, request.Clone()).GetSql(),
						ReportAdvancedModel report => new Queries.ReportQueryAdvancedGenerator(Manager.Schema, report, request.Clone()).GetSql(),
						_ => throw new Models.Exceptions.ReportingException($"Can't find the report {request.ReportId}")
					};
	}

	/// <summary>
	///		Obtiene el informe solicitado
	/// </summary>
	private ReportBaseModel? SearchReport(string reportId)
	{
		// Busca el informe entre los diferentes almacenes del esquema
		foreach (DataWarehouseModel dataWarehouse in Manager.Schema.DataWarehouses.EnumerateValues())
		{
			ReportBaseModel? report = dataWarehouse.Reports[reportId];

				if (report is not null)
					return report;
		}
		// Si ha llegado hasta aquí, no se ha encontrado ningún informe
		return null;
	}

	/// <summary>
	///		Controlador principal
	/// </summary>
	private ReportingManager Manager { get; }
}
