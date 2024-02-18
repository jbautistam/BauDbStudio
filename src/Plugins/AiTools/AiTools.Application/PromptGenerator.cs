using Bau.Libraries.AiTools.Application.Models.Prompts;

namespace Bau.Libraries.AiTools.Application;

/// <summary>
///		Generador de prompts
/// </summary>
public class PromptGenerator
{
	/// <summary>
	///		Carga el archivo de categorías
	/// </summary>
	public void Load(string fileName)
	{
		Models.Categories.CategoryCollectionModel prompItems = new Repository.CategoriesRepository().Load(fileName);

			if (prompItems.Count > 0)
				Categories.Merge(prompItems);
	}

	/// <summary>
	///		Carga un archivo de prompts predefinidos
	/// </summary>
	public PromptFileModel LoadFile(string fileName) => new Repository.PromptsRepository().Load(fileName);

	/// <summary>
	///		Graba un archivo de prompts
	/// </summary>
	public void SaveFile(string fileName, PromptFileModel promptFile)
	{
		new Repository.PromptsRepository().Save(fileName, promptFile);
	}

	/// <summary>
	///		Categorías
	/// </summary>
	public Models.Categories.CategoryCollectionModel Categories { get; } = new();
}
