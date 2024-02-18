namespace Bau.Libraries.AiTools.Application.Models.Categories;

/// <summary>
///		Elemento de un Prompt
/// </summary>
public class CategoryModel
{
	public CategoryModel(CategoryModel? parent, string name, int level)
	{
		Parent = parent;
		Name = name;
		Level = level;
	}

	/// <summary>
	///		Añade un elemento a la colección de hijos
	/// </summary>
	public CategoryModel Add(string name) => Childs.Add(this, name, Level + 1);

	/// <summary>
	///		Añade una serie de elementos
	/// </summary>
	internal void Merge(CategoryCollectionModel items)
	{
		// TODO: ¿hay que buscar el nodo adecuado?
		Childs.AddRange(items);
		//foreach (PromptItemModel item in items)
		//	Add(item.Name);
	}

	/// <summary>
	///		Elemento padre
	/// </summary>
	public CategoryModel? Parent { get; }

	/// <summary>
	///		Nombre del elemento
	/// </summary>
	public string Name { get; }

	/// <summary>
	///		Nivel
	/// </summary>
	public int Level { get; }

	/// <summary>
	///		Hijos del elemento
	/// </summary>
	public CategoryCollectionModel Childs { get; } = new();
}
