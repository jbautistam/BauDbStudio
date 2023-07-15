namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

/// <summary>
///		Clave foránea de una relación
/// </summary>
public class RelationForeignKey
{
	/// <summary>
	///		Columna a relacionar
	/// </summary>
	public string ColumnId { get; set; } = string.Empty;

	/// <summary>
	///		Columna relacionada
	/// </summary>
	public string TargetColumnId { get; set; } = string.Empty;
}
