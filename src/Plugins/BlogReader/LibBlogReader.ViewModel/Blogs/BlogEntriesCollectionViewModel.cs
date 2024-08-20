using System.Collections.ObjectModel;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.LibBlogReader.ViewModel.Blogs;

/// <summary>
///		Colección de <see cref="BlogEntryViewModel"/>
/// </summary>
public class BlogEntriesCollectionViewModel : ObservableCollection<BlogEntryViewModel>
{
	/// <summary>
	///		Obtiene las entradas de un blog
	/// </summary>
	internal EntriesModelCollection GetEntries(BlogModel blog)
	{
		EntriesModelCollection entries = new();

			// Obtiene el número de elementos no leidos de este blog
			foreach (BlogEntryViewModel entry in Items)
				if (entry.Entry.Blog is not null && entry.Entry.Blog.GlobalId.Equals(blog.GlobalId, StringComparison.CurrentCultureIgnoreCase))
					entries.Add(entry.Entry);
			// Devuelve las entradas
			return entries;
	}

	/// <summary>
	///		Obtiene las entradas seleccionadas
	/// </summary>
	internal BlogEntriesCollectionViewModel GetSelected()
	{
		BlogEntriesCollectionViewModel entries = new BlogEntriesCollectionViewModel();

			// Obtiene las seleccionadas
			foreach (BlogEntryViewModel entry in this)
				if (entry.IsSelected)
					entries.Add(entry);
			// Devuelve las entradas seleccionadas
			return entries;
	}

	/// <summary>
	///		Obtiene las entradas no leídas
	/// </summary>
	internal BlogEntriesCollectionViewModel GetByParameters(bool seeNotRead, bool seeRead, bool seeInteresting)
	{
		BlogEntriesCollectionViewModel entries = new BlogEntriesCollectionViewModel();

			// Obtiene las entradas no leídas
			foreach (BlogEntryViewModel entry in this)
				if ((entry.Status == EntryModel.StatusEntry.NotRead && seeNotRead) ||
						(entry.Status == EntryModel.StatusEntry.Read && seeRead) ||
						(entry.Status == EntryModel.StatusEntry.Interesting && seeInteresting))
					entries.Add(entry);
			// Devuelve la colección de entradas
			return entries;
	}

	/// <summary>
	///		Obtiene las entradas leídas
	/// </summary>
	internal BlogEntriesCollectionViewModel GetRead()
	{
		BlogEntriesCollectionViewModel entries = new BlogEntriesCollectionViewModel();

			// Obtiene las entradas no leídas
			foreach (BlogEntryViewModel entry in this)
				if (entry.Status == EntryModel.StatusEntry.Read)
					entries.Add(entry);
			// Devuelve la colección de entradas
			return entries;
	}

	/// <summary>
	///		Busca una entrada
	/// </summary>
	internal BlogEntryViewModel? Search(string url) => this.FirstOrDefault(entry => entry.Entry.URL.EqualsIgnoreCase(url));

	/// <summary>
	///		Cuenta el número de entradas seleccionadas
	/// </summary>
	public int CountSelected => this.Count(entry => entry.IsSelected);
}
