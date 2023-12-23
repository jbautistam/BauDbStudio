namespace Bau.Libraries.RestManager.Application.Models;

/// <summary>
///		Parámetro de un proyecto
/// </summary>
public class ParameterModel
{
	public ParameterModel(string key, string? value)
	{
		Key = key;
		Value = value;
	}

	/// <summary>
	///		Clave del parámetro
	/// </summary>
	public string Key { get; }

	/// <summary>
	///		Valor del parámetro
	/// </summary>
	public string? Value { get; set; }
}
