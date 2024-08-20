﻿namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Colunma solicitada de una dimensión
/// </summary>
public class DimensionRequestModel
{
	/// <summary>
	///		Clona los datos
	/// </summary>
	public DimensionRequestModel Clone()
	{
		DimensionRequestModel cloned = new()
										{
											DimensionId = DimensionId
										};

			// Clona las columnas
			foreach (DimensionColumnRequestModel column in Columns)
				cloned.Columns.Add(column.Clone());
			// Clona las solicitudes hija
			foreach (DimensionRequestModel child in Childs)
				cloned.Childs.Add(child.Clone());
			// Devuelve el objeto clonado
			return cloned;
	}

	/// <summary>
	///		Obtiene la solicitud de columna
	/// </summary>
	public DimensionColumnRequestModel? GetRequestColumn(string columnId)
	{
		// Obtiene la columna asociada
		foreach (DimensionColumnRequestModel column in Columns)
			if (column.ColumnId.Equals(columnId, StringComparison.CurrentCultureIgnoreCase))
				return column;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Código de dimensión
	/// </summary>
	public string DimensionId { get; set; } = string.Empty;

	/// <summary>
	///		Columnas
	/// </summary>
	public List<DimensionColumnRequestModel> Columns { get; } = new();

	/// <summary>
	///		Solicitudes de dimensiones hija
	/// </summary>
	public List<DimensionRequestModel> Childs { get; } = new();
}
