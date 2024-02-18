namespace Bau.Libraries.AiTools.Application.Models.Prompts;

/// <summary>
///		Archivo de prompts
/// </summary>
public class PromptFileModel
{
	public PromptFileModel(string fileName)
	{
		FileName = fileName;
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName { get; set; }

	/// <summary>
	///		Nombre
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	///		Descripción
	/// </summary>
	public string Description { get; set; } = default!;

	/// <summary>
	///		Colección de prompts
	/// </summary>
	public PromptCollectionModel Prompts { get; } = new();
}
