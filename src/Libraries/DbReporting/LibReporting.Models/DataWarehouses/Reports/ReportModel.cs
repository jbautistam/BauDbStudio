namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

/// <summary>
///		Clase con los datos de un informe
/// </summary>
public class ReportModel : ReportBaseModel
{
	public ReportModel(DataWarehouseModel dataWarehouse) : base(dataWarehouse) {}

	/// <summary>
	///		Orígenes de datos de las expresiones del informe
	/// </summary>
	public List<ReportDataSourceModel> ReportDataSources { get; } = new();
}