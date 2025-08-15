using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibBlogReader.Model;

/// <summary>
///		Colección de <see cref="EntryModel"/>
/// </summary>
public class EntriesModelCollection : LibDataStructures.Base.BaseExtendedModelCollection<EntryModel>
{   
	// Constantes privadas
	private const int MaxItems = 100;

	/// <summary>
	///		Cambia el estado de una entrada
	/// </summary>
	public void UpdateStatus(BlogModel blog, string globalId, EntryModel.StatusEntry newStatus)
	{
		foreach (EntryModel entry in this)
			if (entry.Blog is not null && entry.Blog.GlobalId.Equals(blog.GlobalId, StringComparison.CurrentCultureIgnoreCase) && 
					entry.GlobalId.Equals(globalId, StringComparison.CurrentCultureIgnoreCase))
				entry.Status = newStatus;
	}

	/// <summary>
	///		Ordena las entradas por nombre de blog y fecha
	/// </summary>
	public void SortByDate(bool ascending = true)
	{
		Sort(new Comparers.EntryComparerByDate(ascending));
	}

	/// <summary>
	///		Obtiene una entrada por su URL
	/// </summary>
	public EntryModel? SearchByURL(string url)
	{
		if (string.IsNullOrWhiteSpace(url))
			return null;
		else
			return this.FirstOrDefault(entry => entry.URL.EqualsIgnoreCase(url));
	}

	/// <summary>
	///		Comprueba si existe una URL
	/// </summary>
	public bool ExistsURL(string url) => SearchByURL(url) is not null;

	/// <summary>
	///		Obtiene el número de elementos no leídos
	/// </summary>
	public int GetNumberNotRead()
	{
		int number = 0;

			// Obtiene el número de elementos no leídos
			foreach (EntryModel entry in this)
				if (entry.Status == EntryModel.StatusEntry.NotRead)
					number++;
			// Devuelve el número de elementos
			return number;
	}

	/// <summary>
	///		Obtiene el número de elementos leídos y borrados
	/// </summary>
	public int GetNumberReadAndDeleted()
	{
		int number = 0;

			// Obtiene el número de elementos no leídos
			foreach (EntryModel entry in this)
				if (entry.Status == EntryModel.StatusEntry.Read || entry.Status == EntryModel.StatusEntry.Deleted)
					number++;
			// Devuelve el número de elementos
			return number;
	}

	/// <summary>
	///		Obtiene las entradas de un blog
	/// </summary>
	public EntriesModelCollection GetFrom(BlogModel blog)
	{
		EntriesModelCollection entries = [];

			// Busca las entradas del blog
			foreach (EntryModel entry in this)
				if (entry.Blog is not null && entry.Blog.GlobalId.Equals(blog.GlobalId, StringComparison.CurrentCultureIgnoreCase))
					entries.Add(entry);
			// Devuelve las entradas
			return entries;
	}

	/// <summary>
	///		Borra los datos antiguos
	/// </summary>
	public void DeleteOldData()
	{
		for (int index = Count - 1; index >= 0; index--)
			if (Count > MaxItems && this[index].DatePublish < DateTime.Now.AddDays(-60) &&
					this[index].Status == EntryModel.StatusEntry.Deleted)
				RemoveAt(index);
	}
	/// <summary>
	///		Obtiene la fecha de última entrada
	/// </summary>
	public DateTime GetDateLastEntry()
	{
		DateTime? dateLastEntry = null;

			// Obtiene la fecha máxima de entrada
			foreach (EntryModel entry in this)
				if (dateLastEntry is null || entry.DatePublish > dateLastEntry)
					dateLastEntry = entry.DatePublish;
			// Devuelve la fecha
			return dateLastEntry ?? DateTime.UtcNow.AddYears(-10);
	}
}
