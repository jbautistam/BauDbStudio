namespace Bau.Libraries.LibAiImageGeneration.Domain.Models.Results;

/// <summary>
///		Definición de modelo
/// </summary>
public class DefinitionResultModel
{
	/// <summary>
	///		Tipo de definición
	/// </summary>
	public enum DefinitionType
	{
		/// <summary>Modelo</summary>
		Model = 1,
		/// <summary>Loras</summary>
		Loras
	}

	public DefinitionResultModel(DefinitionType type, string name)
	{
		Type = type;
		Name = name;
	}

	/// <summary>
	///		Tipo de la definición
	/// </summary>
	public DefinitionType Type { get; }

	/// <summary>
	///		Nombre de la definición
	/// </summary>
	public string Name { get; }
}
