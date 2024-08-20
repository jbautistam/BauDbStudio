namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

/// <summary>
///		Definición de un parámetro para un <see cref="ReportModel"/>
/// </summary>
public class ReportParameterModel
{
	/// <summary>
	///		Clave del parámetro
	/// </summary>
	public string Key { get; set; } = string.Empty;

	/// <summary>
	///		Tipo de parámetro
	/// </summary>
	public DataSets.DataSourceColumnModel.FieldType Type { get; set; }

	/// <summary>
	///		Valor predeterminado
	/// </summary>
	public string DefaultValue { get; set; } = string.Empty;
}
