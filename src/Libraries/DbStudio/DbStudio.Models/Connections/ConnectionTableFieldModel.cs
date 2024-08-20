namespace Bau.Libraries.DbStudio.Models.Connections;

/// <summary>
///		Datos del campo
/// </summary>
public class ConnectionTableFieldModel : LibDataStructures.Base.BaseExtendedModel
{
	// Enumerados públicos
	/// <summary>
	///		Tipo de campo
	/// </summary>
	public enum Fieldtype
	{
		/// <summary>Desconocido. No se debería utilizar</summary>
		Unknown,
		/// <summary>Cadena</summary>
		String,
		/// <summary>Fecha</summary>
		Date,
		/// <summary>Número entero</summary>
		Integer,
		/// <summary>Número decimal</summary>
		Decimal,
		/// <summary>Valor lógico</summary>
		Boolean
	}

	public ConnectionTableFieldModel(ConnectionTableModel table)
	{
		Table = table;
	}

	/// <summary>
	///		Tabla a la que se asocia el campo
	/// </summary>
	public ConnectionTableModel Table { get; }

	/// <summary>
	///		Tipo normalizado del campo
	/// </summary>
	public Fieldtype Type { get; set; }

	/// <summary>
	///		Tipo de campo (texto)
	/// </summary>
	public string TypeText { get; set; } = string.Empty;

	/// <summary>
	///		Longitud del campo
	/// </summary>
	public int Length { get; set; }

	/// <summary>
	///		Nombre completo
	/// </summary>
	public string FullName
	{
		get
		{
			string length = Length > 0 ? $"({Length:#,##0})" : string.Empty;

				return $"{Name} [{TypeText}{length}]";
		}
	}

	/// <summary>
	///		Indica si el campo es obligatorio
	/// </summary>
	public bool IsRequired { get; set; }

	/// <summary>
	///		Indica si el campo es clave
	/// </summary>
	public bool IsKey { get; set; }

	/// <summary>
	///		India si el campo es identidad
	/// </summary>
	public bool IsIdentity { get; set; }
}
