namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

/// <summary>
///		Conjuntos de campos asociados a una dimensión
/// </summary>
public class ReportAdvancedRequestDimension
{
	/// <summary>
	///		Clave de la dimensión
	/// </summary>
	public string DimensionKey { get; set; } = string.Empty;

	/// <summary>
	///		Indica si la dimensión es obligatoria (aunque no se haya solicitado nada, se debe incluir)
	/// </summary>
	public bool Required { get; set; }

	/// <summary>
	///		Campos que se deben solicitar juntos
	/// </summary>
	public List<ReportAdvancedRequestDimensionField> Fields { get; } = new();
}
