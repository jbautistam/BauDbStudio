namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Filters;

/// <summary>
///		Clase con los datos de una columna
/// </summary>
public class ColumnModel
{
	/// <summary>
	///		Tipo de columna
	/// </summary>
	public enum ColumnType
	{
		/// <summary>Cadena</summary>
		String,
		/// <summary>Fecha</summary>
		DateTime,
		/// <summary>Entero</summary>
		Integer,
		/// <summary>Decimal</summary>
		Decimal,
		/// <summary>Valor lógico</summary>
		Boolean
	}

	public ColumnModel(string name, ColumnType type)
	{
		Name = name;
		Type = type;
	}

	/// <summary>
	///		Nombre de la columna
	/// </summary>
	public string Name { get; }

	/// <summary>
	///		Tipo de la columna
	/// </summary>
	public ColumnType Type { get; }
}
