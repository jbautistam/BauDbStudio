using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.AiTools.Application.Models.Categories;

namespace Bau.Libraries.AiTools.Application.Repository;

/// <summary>
///		Repositorio para archivos de categorías del prompt
/// </summary>
internal class CategoriesRepository
{
	// Constantes privadas
	private const string TagRoot = "Definitions";
	private const string TagDefinition = "Definition";
	private const string TagText = "Text";

	/// <summary>
	///		Carga una lista de categorías de un archivo
	/// </summary>
	internal CategoryCollectionModel Load(string fileName)
	{
		CategoryCollectionModel categories = new();
		MLFile fileML = new Bau.Libraries.LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga los elementos
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						categories.AddRange(LoadCategories(null, 0, rootML));
			// Devuelve los elementos cargados
			return categories;
	}

	/// <summary>
	///		Carga las categorías
	/// </summary>
	private CategoryCollectionModel LoadCategories(CategoryModel? parent, int level, MLNode rootML)
	{
		CategoryCollectionModel categories = new();

			// Carga los nodos de categorías
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagDefinition)
				{
					string text = nodeML.Attributes[TagText].Value.TrimIgnoreNull();
					
						if (!string.IsNullOrWhiteSpace(text))
						{
							string[] parts = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

								if (parts.Length != 0)
									foreach (string part in parts)
									{
										CategoryModel category = new(parent, part, level);

											// Carga las categorías hija
											category.Childs.AddRange(LoadCategories(category, level + 1, nodeML));
											// Añade la categoría a la colección
											categories.Add(category);
									}
						}
				}
			// Devuelve las categorías cargadas
			return categories;
	}
}
