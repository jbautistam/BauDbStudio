namespace Bau.Libraries.RestManager.Application.Models;

/// <summary>
///		Lista de <see cref="ParameterModel"/>
/// </summary>
public class ParametersCollectionModel : List<ParameterModel>
{
	/// <summary>
	///		Añade un parámetro a la colección
	/// </summary>
	public void Add(string key, string? value)
	{
		Add(new ParameterModel(key, value));
	}
}
