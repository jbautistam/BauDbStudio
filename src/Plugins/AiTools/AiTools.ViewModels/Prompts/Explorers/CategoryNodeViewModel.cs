using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.AiTools.Application.Models.Categories;

namespace Bau.Libraries.AiTools.ViewModels.Prompts.Explorers;

/// <summary>
///		Nodo del árbol de categorías
/// </summary>
public class CategoryNodeViewModel : ControlHierarchicalViewModel
{
	public CategoryNodeViewModel(TreeCategoriesViewModel trvTree, CategoryNodeViewModel? parent, CategoryModel category) 
				: base(parent, category.Name, nameof(CategoryModel), string.Empty, category, false)
	{
		TreeCategoriesViewModel = trvTree;
		PromptItem = category;
	}

	/// <summary>
	///		Carga los elementos hijo
	/// </summary>
	public override void LoadChildrenData()
	{
		foreach (CategoryModel category in PromptItem.Childs)
		{
			CategoryNodeViewModel node = new(TreeCategoriesViewModel, this, category);

				// Añade el nodo
				Children.Add(node);
		}
	}

	/// <summary>
	///		Expande todos los nodos
	/// </summary>
	internal void Expand()
	{
		// Carga los datos hijo
		LoadChildrenData();
		// Carga los descendientes
		foreach (ControlHierarchicalViewModel child in Children)
			if (child is CategoryNodeViewModel node)
				node.Expand();
	}

	/// <summary>
	///		Obtiene el texto del nodo para el editor
	/// </summary>
	internal void GetTextForEditor(TreeCategoriesViewModel.GenerationMode mode)
	{
		string text;

			// Se añade el texto de los ascendentes o descendentes dependiendo del modo
			switch (mode)
			{
				case TreeCategoriesViewModel.GenerationMode.CopyDescendants:
						text = $"{Text}, {GetTextChildren(Children)}".TrimIgnoreNull();
					break;
				case TreeCategoriesViewModel.GenerationMode.CopyParents:
						text = GetTextParent(Parent).AddWithSeparator(Text, ", ").TrimIgnoreNull();
					break;
				default:
						text = Text;
					break;
			}
			// Añade los paréntesis
			if (!string.IsNullOrWhiteSpace(text) && text.IndexOf(' ') > 0)
				text = $"({text})";
			// Devuelve el texto
			TreeCategoriesViewModel.PromptFileViewModel.RaiseCategoryTextEvent(text);
	}

	/// <summary>
	///		Obtiene los texto de los nodos hijo
	/// </summary>
	private string GetTextChildren(AsyncObservableCollection<ControlHierarchicalViewModel> children)
	{
		string text = string.Empty;

			// Añade los textos de los nodos hijo
			foreach (ControlHierarchicalViewModel node in children)
			{
				// Añade el texto del nodo hijo
				text = text.AddWithSeparator(node.Text, ", ", false);
				// Añade el texto de todos los nodos hijo
				text = text.AddWithSeparator(GetTextChildren(node.Children), ", ", false);
			}
			// Devuelve el texto
			return text;
	}

	/// <summary>
	///		Obtiene los texto de los nodos padre
	/// </summary>
	private string GetTextParent(ControlHierarchicalViewModel? parent)
	{
		string text = string.Empty;

			// Añade los textos de los nodos padre (empezando por el superior)
			if (parent is not null)
			{
				// Añade el texto de los elementos padres
				text = text.AddWithSeparator(GetTextParent(parent.Parent), " ");
				// Añade el texto del padre
				text = text.AddWithSeparator(parent.Text, " ");
			}
			// Devuelve el texto
			return text;
	}

	/// <summary>
	///		ViewModel del árbol principal
	/// </summary>
	public TreeCategoriesViewModel TreeCategoriesViewModel { get; }

	/// <summary>
	///		Elemento del prompt
	/// </summary>
	public CategoryModel PromptItem { get; }
}
