﻿namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Datos del parámetro de un informe
/// </summary>
public class ParameterRequestModel
{
	/// <summary>
	///		Tipo de parámetro
	/// </summary>
	public enum ParameterType
	{
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

	/// <summary>
	///		Clona los datos de un parámetro
	/// </summary>
	public ParameterRequestModel Clone()
	{
		return new ParameterRequestModel
						{
							Key = Key,
							Type = Type,
							Value = Value
						};
	}

	/// <summary>
	///		Clave del parámetro
	/// </summary>
	public string Key { get; set; } = string.Empty;

	/// <summary>
	///		Tipo de parámetro
	/// </summary>
	public ParameterType Type { get; set; }

	/// <summary>
	///		Valor solicitado en el parámetro
	/// </summary>
	public object? Value { get; set; } = string.Empty;
}
