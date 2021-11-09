using System;

using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibBlogReader.Model
{
	/// <summary>
	///		Clase con los datos de un blog
	/// </summary>
	public class BlogModel : LibDataStructures.Base.BaseExtendedModel
	{ 
		// Variables privadas
		private string path;

		/// <summary>
		///		Borra una entrada
		/// </summary>
		internal bool Delete(EntryModel entry)
		{
			return Entries.Delete(entry);
		}

		/// <summary>
		///		Directorio para grabación de las entradas
		/// </summary>
		public string Path
		{
			get
			{
				// Obtiene el nombre de archivo
				if (string.IsNullOrEmpty(path))
					path = LibHelper.Files.HelperFiles.Normalize(URL, false);
				// Devuelve el nombre de archivo
				return path;
			}
			set { path = value; }
		}

		/// <summary>
		///		URL donde se encuentra el archivo RSS del blog
		/// </summary>
		public string URL { get; set; }

		/// <summary>
		///		Indica si se deben descargar automáticamente los postcasts
		/// </summary>
		public bool DownloadPodcast { get; set; }

		/// <summary>
		///		Indica si el blog está habilitado para descarga
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		///		Fecha de última descarga
		/// </summary>
		public DateTime? DateLastDownload { get; set; }

		/// <summary>
		///		Fecha de la última entrada descargada
		/// </summary>
		public DateTime? DateLastEntry { get; set; }

		/// <summary>
		///		Número de elementos no leídos
		/// </summary>
		public int NumberNotRead 
		{ 
			get
			{
				int count = 0;

					// Cuenta los elementos no leidos
					foreach (EntryModel entry in Entries)
						if (entry.Status == EntryModel.StatusEntry.NotRead)
							count++;
					// Devuelve el número de elementos
					return count;
			}
		}

		/// <summary>
		///		Carpeta a la que pertenece el blog
		/// </summary>
		public FolderModel Folder { get; set; }

		/// <summary>
		///		Indica si se ha modificado
		/// </summary>
		public bool IsDirty { get; set; }

		/// <summary>
		///		Entradas del blog (Lazy)
		/// </summary>
		public LibDataStructures.Base.LazyObject<EntriesModelCollection> LazyEntries { get; } = new LibDataStructures.Base.LazyObject<EntriesModelCollection>();

		/// <summary>
		///		Entradas del blog
		/// </summary>
		public EntriesModelCollection Entries
		{
			get { return LazyEntries.Data; }
			set { CheckProperty<EntriesModelCollection, EntryModel>(LazyEntries, value); }
		}
	}
}
