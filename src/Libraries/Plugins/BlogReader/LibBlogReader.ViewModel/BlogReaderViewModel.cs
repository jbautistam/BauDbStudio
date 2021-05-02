using System;

namespace Bau.Libraries.LibBlogReader.ViewModel
{
	/// <summary>
	///		ViewModel principal del lector de blogs
	/// </summary>
	public class BlogReaderViewModel : BauMvvm.ViewModels.BaseObservableObject
	{
		// Variables privadas
		private Blogs.TreeBlogs.TreeBlogsViewModel _treeBlogsViewModel;

		public BlogReaderViewModel(Controllers.IBlogReaderController mainController)
		{
			// Asigna el manager de blogs y el controlador de vistas
			BlogManager = new Application.BlogReaderManager();
			ViewsController = mainController;
			// Asigna los objetos
			ConfigurationViewModel = new Configuration.ConfigurationViewModel(this);
			TreeBlogs = new Blogs.TreeBlogs.TreeBlogsViewModel(this);
			BlogDownloadProcess = new Controllers.Process.BlogDownloadProcess(this);
		}

		/// <summary>
		///		Inicializa el viewModel
		/// </summary>
		public void Initialize()
		{
			// Carga la configuración
			ConfigurationViewModel.Load();
			// Arranca el procesador de descargas
			BlogDownloadProcess.Start();
		}

		/// <summary>
		///		Carga el directorio (en este caso, siempre es el mismo directorio, por eso no lo cambia)
		/// </summary>
		public void Load(string path)
		{
			// Guarda el directorio
			if (string.IsNullOrWhiteSpace(ConfigurationViewModel.PathBlogs) && !string.IsNullOrWhiteSpace(path))
				ConfigurationViewModel.PathBlogs = path;
			// Carga la solución
			BlogManager.Configuration.PathBlogs = ConfigurationViewModel.PathBlogs;
			BlogManager.Load();
			// Carga los exploradores
			TreeBlogs.Load();
		}

/*

		/// <summary>
		///		Crea los parámetros de un mensaje a partir de una entrada
		/// </summary>
		private ParametersModelCollection GetParametersBlog(Model.EntryModel entry)
		{
			var parameters = new ParametersModelCollection();

				// Añade los datos del blog
				parameters.Add(ModuleName, "BlogName", entry.Blog.Name);
				parameters.Add(ModuleName, "BlogDescription", entry.Blog.Description);
				parameters.Add(ModuleName, "BlogUrl", entry.Blog.URL);
				// Añade los datos de la entrada
				parameters.Add(ModuleName, "EntryName", entry.Name);
				parameters.Add(ModuleName, "EntryDescription", entry.Description);
				parameters.Add(ModuleName, "EntryContent", entry.Content);
				parameters.Add(ModuleName, "EntryUrl", entry.URL);
				// Devuelve la colección de parámetros
				return parameters;
		}
		/// <summary>
		///		Envía un mensaje de cambio de estado
		/// </summary>
		internal void SendMesageChangeStatus(object content = null)
		{
			System.Diagnostics.Debug.WriteLine("Falta ésto");
			// HostController.Messenger.Send(new Controllers.Messengers.MessageChangeStatusNews(content));
		}

*/
		/// <summary>
		///		Manager de blogs
		/// </summary>
		public Application.BlogReaderManager BlogManager { get; }

		/// <summary>
		///		Controlador de vistas de aplicación
		/// </summary>
		public Controllers.IBlogReaderController ViewsController { get; }

		/// <summary>
		///		ViewModel de configuración
		/// </summary>
		public Configuration.ConfigurationViewModel ConfigurationViewModel { get; }

		/// <summary>
		///		Arbol de blogs
		/// </summary>
		public Blogs.TreeBlogs.TreeBlogsViewModel TreeBlogs
		{
			get { return _treeBlogsViewModel; }
			set { CheckObject(ref _treeBlogsViewModel, value); }
		}

		/// <summary>
		///		Proceso de descarga de blogs
		/// </summary>
		internal Controllers.Process.BlogDownloadProcess BlogDownloadProcess { get; }

/*
		{
			get
			{
				string pathBlogs = GetParameter(nameof(PathBlogs));

					// Si no se ha encontrado ningún parámetro crea el nombre del directorio
					if (pathBlogs.IsEmpty())
						pathBlogs = System.IO.Path.Combine(HostController.Configuration.PathBaseData, "BlogsData");
					// Devuelve el directorio
					return pathBlogs;
			}
			set
			{
				SetParameter(nameof(PathBlogs), value);
				BlogManager.Configuration.PathBlogs = value;
			}
		}
*/

/*
		/// <summary>
		///		Minutos entre descargas
		/// </summary>
		public int MinutesBetweenDownload
		{
			get { return GetParameter(nameof(MinutesBetweenDownload)).GetInt(60); }
			set { SetParameter(nameof(MinutesBetweenDownload), value); }
		}

		/// <summary>
		///		Indica si las descargas están activas
		/// </summary>
		public bool DownloadEnabled
		{
			get { return GetParameter(nameof(DownloadEnabled)).GetBool(true); }
			set { SetParameter(nameof(DownloadEnabled), value); }
		}

		/// <summary>
		///		Configuración de registros por páginas
		/// </summary>
		public int RecordsPerPage
		{
			get { return GetParameter(nameof(RecordsPerPage)).GetInt(25); }
			set { SetParameter(nameof(RecordsPerPage), value); }
		}

		/// <summary>
		///		Indica si se deben ver las entradas leídas
		/// </summary>
		public bool SeeEntriesRead
		{
			get { return GetParameter(nameof(SeeEntriesRead)).GetBool(false); }
			set { SetParameter(nameof(SeeEntriesRead), value); }
		}

		/// <summary>
		///		Indica si se deben ver las entradas no leídas
		/// </summary>
		public bool SeeEntriesNotRead
		{
			get { return GetParameter(nameof(SeeEntriesNotRead)).GetBool(true); }
			set { SetParameter(nameof(SeeEntriesNotRead), value); }
		}

		/// <summary>
		///		Indica si se deben ver las entradas interesantes
		/// </summary>
		public bool SeeEntriesInteresting
		{
			get { return GetParameter(nameof(SeeEntriesInteresting)).GetBool(false); }
			set { SetParameter(nameof(SeeEntriesInteresting), value); }
		}
*/
	}
}
