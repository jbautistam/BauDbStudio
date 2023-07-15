namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Colunma solicitada de una dimensión
/// </summary>
public class DimensionColumnRequestModel : BaseColumnRequestModel
{
	/// <summary>
	///		Clona los datos de una columna
	/// </summary>
	public DimensionColumnRequestModel Clone()
	{
		DimensionColumnRequestModel cloned = new()
												{
													ColumnId = ColumnId
												};

			// Clona los datos base
			CopyBase(cloned);
			// Clona los elementos hijo
			foreach (DimensionColumnRequestModel child in Childs)
				cloned.Childs.Add(child.Clone());
			// Devuelve los datos clonados
			return cloned;
	}

	/// <summary>
	///		Código de columna
	/// </summary>
	public string ColumnId { get; set; } = string.Empty;

	/// <summary>
	///		Solicitudes de dimensiones hija
	/// </summary>
	public List<DimensionColumnRequestModel> Childs { get; } = new();
}
