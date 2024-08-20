
namespace Bau.Libraries.AiTools.Application.Models.Categories;

/// <summary>
///		Colección de <see cref="CategoryModel"/>
/// </summary>
public class CategoryCollectionModel : List<CategoryModel>
{
	/// <summary>
	///		Añade un elemento hijo
	/// </summary>
	internal CategoryModel Add(CategoryModel? parent, string name, int level)
	{
		CategoryModel child = new(parent, name, level);

			// Añade el elemento hijo
			Add(child);
			// Devuelve el elemento generado
			return child;
	}

	/// <summary>
	///		Mezcla los elementos
	/// </summary>
	internal void Merge(CategoryCollectionModel categories)
	{
		AddRange(categories);
	}
}
