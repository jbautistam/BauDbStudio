using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibBlogReader.Model;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;

namespace Bau.Libraries.LibBlogReader.ViewModel.Blogs.TreeBlogs
{
	/// <summary>
	///		ViewModel para el árbol de blogs
	/// </summary>
	public class TreeBlogsViewModel : PluginsStudio.ViewModels.Base.Explorers.BaseTreeViewModel
	{   
		// Constantes privadas
		private const string OpmlFilter = "Archivos OPML (*.opml)|*.opml|Todos los archivos (*.*)|*.*";
		// Enumerados públicos
		/// <summary>Tipo de nodo</summary>
		public enum NodeType
		{
			Unknown,
			Folder,
			Blog
		}

		public TreeBlogsViewModel(BlogReaderViewModel mainViewModel)
		{
			// Asigna las propiedades
			MainViewModel = mainViewModel;
			// Inicializa los comandos
			OpenOpmlCommand = new BaseCommand(_ => ExecuteAction(nameof(OpenOpmlCommand)));
			NewFolderCommand = new BaseCommand(_ => ExecuteAction(nameof(NewFolderCommand)), _ => CanExecuteAction(nameof(NewFolderCommand)))
									.AddListener(this, nameof(SelectedNode));
			NewBlogCommand = new BaseCommand(_ => ExecuteAction(nameof(NewBlogCommand)), _ => CanExecuteAction(nameof(NewBlogCommand)))
									.AddListener(this, nameof(SelectedNode));
			DownloadCommand = new BaseCommand(_ => ExecuteAction(nameof(DownloadCommand)), _ => CanExecuteAction(nameof(DownloadCommand)))
									.AddListener(this, nameof(SelectedNode));
			SeeNewsCommand = new BaseCommand(_ => ExecuteAction(nameof(SeeNewsCommand)), _ => CanExecuteAction(nameof(SeeNewsCommand)))
									.AddListener(this, nameof(SelectedNode));
		}
		/// <summary>
		///		Carga los nodos
		/// </summary>
		protected override void AddRootNodes()
		{
			// Añade los blogs y carpetas
			AddChildFolderNodes(null, MainViewModel.BlogManager.File);
			// Actualiza el número de elementos leidos (y el texto, por tanto)
			foreach (BaseBlogsNodeViewModel node in Children)
				node.ComputeNumberNotRead();
		}

		/// <summary>
		///		Añade los nodos de una carpeta
		/// </summary>
		private void AddChildFolderNodes(FolderNodeViewModel parent, FolderModel folder)
		{
			// Ordena las carpetas
			folder.Folders.SortByName();
			// Añade las carpetas
			foreach (FolderModel child in folder.Folders)
			{
				FolderNodeViewModel node = new(this, MainViewModel, parent, child);

					// Añade el nodo
					if (parent != null)
						parent.Children.Add(node);
					else
						Children.Add(node);
					// Añade las carpetas hijo
					AddChildFolderNodes(node, child);
			}
			// Ordena los blogs
			folder.Blogs.SortByName();
			// Añade los blogs
			foreach (BlogModel blog in folder.Blogs)
			{
				BlogNodeViewModel node = new(this, MainViewModel, null, blog);

					// Añade el nodo
					if (parent != null)
						parent.Children.Add(node);
					else
						Children.Add(node);
			}
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		private void ExecuteAction(string action)
		{
			switch (action)
			{
				case nameof(OpenOpmlCommand):
						OpenFileOpml();
					break;
				case nameof(NewFolderCommand):
						OpenFormUpdateFolder(null);
					break;
				case nameof(NewBlogCommand):
						OpenFormUpdateBlog(null);
					break;
				case nameof(DownloadCommand):
						DownloadItems();
					break;
				case nameof(SeeNewsCommand):
						SeeNews();
					break;
				case nameof(SaveOpml):
						SaveOpml();
					break;
				case nameof(DeleteCommand):
						if (SelectedNode != null)
						{
							if (SelectedNode is FolderNodeViewModel folderNode)
								DeleteFolder(folderNode.Folder);
							else if (SelectedNode is BlogNodeViewModel blogNode)
								DeleteBlog(blogNode.Blog);
						}
					break;
			}
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			switch (action)
			{
				case nameof(NewFolderCommand):
				case nameof(NewBlogCommand):
					return IsSelectedFolder || SelectedNode == null;
				case nameof(OpenCommand):
				case nameof(DeleteCommand):
				case nameof(DownloadCommand):
				case nameof(SeeNewsCommand):
					return SelectedNode != null;
				case nameof(SaveOpml):
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		///		Abre la ventana de propiedades
		/// </summary>
		protected override void OpenProperties()
		{
			if (SelectedNode != null)
			{
				if (SelectedNode is FolderNodeViewModel folderNode)
					OpenFormUpdateFolder(folderNode.Folder);
				else if (SelectedNode is BlogNodeViewModel blogNode)
					OpenFormUpdateBlog(blogNode.Blog);
			}

		}
		/// <summary>
		///		Chequea un nodo
		/// </summary>
		public void CheckNode(int? id)
		{
			CheckNode(ConvertNodes(Children), id);
		}

		/// <summary>
		///		Convierte la colección de <see cref="Children"/> a <see cref="ObservableCollection{T}"/>
		/// </summary>
		private List<BaseBlogsNodeViewModel> ConvertNodes(ObservableCollection<IHierarchicalViewModel> nodes) 
		{
			List<BaseBlogsNodeViewModel> converted = new List<BaseBlogsNodeViewModel>();

				// Convierte los elementos
				foreach (BaseBlogsNodeViewModel node in nodes)
					converted.Add(node);
				// Devuelve la lista de elementos convertida
				return converted;
		}

		/// <summary>
		///		Chequea un nodo de los existentes
		/// </summary>
		private void CheckNode(List<BaseBlogsNodeViewModel> nodes, int? id)
		{
			foreach (BaseBlogsNodeViewModel node in nodes)
				if (node is BlogNodeViewModel blogNode)
				{
					if (blogNode.Blog.ID == id)
						blogNode.IsChecked = true;
				}
				else
				{
					node.IsExpanded = true;
					CheckNode(ConvertNodes(node.Children), id);
				}
		}

		/// <summary>
		///		Obtiene los nodos chequeados
		/// </summary>
		public List<int> GetCheckedNodes()
		{
			return GetCheckedNodes(ConvertNodes(Children));
		}

		/// <summary>
		///		Obtiene los nodos chequeados
		/// </summary>
		private List<int> GetCheckedNodes(List<BaseBlogsNodeViewModel> nodes)
		{
			List<int> ids = new List<int>();

				// Obtiene los nodos
				foreach (IHierarchicalViewModel node in nodes)
					if (node is BlogNodeViewModel && node.IsChecked)
						ids.Add((node as BlogNodeViewModel).Blog.ID ?? 0);
					else
						ids.AddRange(GetCheckedNodes(ConvertNodes(node.Children)));
				// Devuelve la colección de IDs
				return ids;
		}

		/// <summary>
		///		Abre un archivo OPML
		/// </summary>
		private void OpenFileOpml()
		{
			string fileName = MainViewModel.ViewsController.DialogsController.OpenDialogLoad(null, OpmlFilter);

				if (!fileName.IsEmpty() && System.IO.File.Exists(fileName))
				{ 
					// Carga el archivo
					 MainViewModel.BlogManager.LoadOpml(fileName);
					// Graba la configuración
					MainViewModel.BlogManager.Save();
					// Actualiza el árbol
					Refresh();
				}
		}

		/// <summary>
		///		Guarda el archivo como OPML
		/// </summary>
		private void SaveOpml()
		{
			string fileName = MainViewModel.ViewsController.DialogsController.OpenDialogSave(null, OpmlFilter);

				if (!fileName.IsEmpty())
					MainViewModel.BlogManager.SaveOpml(fileName);
		}

		/// <summary>
		///		Abre el formulario de modificación / creación de una carpeta
		/// </summary>
		private void OpenFormUpdateFolder(FolderModel folder)
		{
			FolderModel parent = null;

				// Obtiene la carpeta seleccionada
				if (folder == null)
				{
					if (SelectedNode is FolderNodeViewModel node)
						parent = node.Folder;
					else
						parent = MainViewModel.BlogManager.File;
				}
				// Abre la ventana de modificación
				if (MainViewModel.ViewsController.AppController.OpenDialog
										(new FolderViewModel(MainViewModel, parent, folder)) == SystemControllerEnums.ResultType.Yes)
					Refresh();
		}

		/// <summary>
		///		Abre el formulario de modificación / creación de una carpeta
		/// </summary>
		private void OpenFormUpdateBlog(BlogModel blog)
		{
			FolderModel folder = null;

				// Obtiene la carpeta a la que se añade el blog
				if (blog == null)
				{
					if (SelectedNode is FolderNodeViewModel node)
						folder = node.Folder;
					else
						folder = MainViewModel.BlogManager.File;
				}
				else
					folder = blog.Folder;
				// Abre la ventana de modificación
				if (MainViewModel.ViewsController.AppController.OpenDialog(new BlogViewModel(MainViewModel, folder, blog)) == SystemControllerEnums.ResultType.Yes)
					Refresh();
		}

		/// <summary>
		///		Borra un elemento
		/// </summary>
		protected override void DeleteItem()
		{
			switch (SelectedNode)
			{
				case FolderNodeViewModel node:
						DeleteFolder(node.Folder);
					break;
				case BlogNodeViewModel node:
						DeleteBlog(node.Blog);
					break;
			}
		}


		/// <summary>
		///		Borra una carpeta
		/// </summary>
		private void DeleteFolder(FolderModel folder)
		{
			if (folder != null && MainViewModel.ViewsController.SystemController.ShowQuestion($"¿Realmente desea eliminar la carpeta '{folder.Name}'?"))
			{ 
				BlogsModelCollection blogs = folder.GetBlogsRecursive();

					// Borra los directorios de los blogs
					KillPaths(blogs);
					// Borra la carpeta
					MainViewModel.BlogManager.File.Delete(folder);
					// Borra la carpeta
					MainViewModel.BlogManager.Save();
					// Actualiza
					Refresh();
			}
		}

		/// <summary>
		///		Borra un blog
		/// </summary>
		private void DeleteBlog(BlogModel blog)
		{
			if (blog != null && MainViewModel.ViewsController.SystemController.ShowQuestion($"¿Realmente desea eliminar el blog '{blog.Name}'?"))
			{ 
				// Borra el directorio del blog
				KillPaths(new BlogsModelCollection { blog } );
				// Borra el blog
				MainViewModel.BlogManager.File.Delete(blog);
				// Graba los datos
				MainViewModel.BlogManager.Save();
				// Actualiza
				Refresh();
			}
		}

		/// <summary>
		///		Elimina los directorios de una serie de blogs
		/// </summary>
		private void KillPaths(BlogsModelCollection blogs)
		{
			foreach (BlogModel blog in blogs)
				LibHelper.Files.HelperFiles.KillPath(System.IO.Path.Combine(MainViewModel.ConfigurationViewModel.PathBlogs, blog.Path));
		}

		/// <summary>
		///		Descarga los elementos
		/// </summary>
		private void DownloadItems()
		{
			if (SelectedNode is BaseBlogsNodeViewModel node && node != null)
			{
				BlogsModelCollection blogs = node.GetBlogs();

					// Descarga los blogs
					new Application.Services.Reader.RssDownload(MainViewModel.BlogManager).Download(true, blogs);
			}
		}

		/// <summary>
		///		Muestra las noticias
		/// </summary>
		private void SeeNews()
		{
			MainViewModel.ViewsController.AppController.OpenWindow(new BlogSeeNewsViewModel(MainViewModel, (SelectedNode as BaseBlogsNodeViewModel).GetBlogs()));
		}

		///// <summary>
		/////		Chequea un nodo
		///// </summary>
		//public void CheckNode(int? id)
		//{
		//	CheckNode(id);
		//}

		///// <summary>
		/////		Obtiene los nodos chequeados
		///// </summary>
		//public List<int> GetCheckedNodes()
		//{
		//	return GetCheckedNodes();
		//}

		/// <summary>
		///		Actualiza el árbol
		/// </summary>
		public void Refresh()
		{   
			// Recalcula el número de elementos
			MainViewModel.BlogManager.File.GetNumberNotRead();
			// Llama al método base
			Load();
		}

		/// <summary>
		///		Indica si está seleccionada una carpeta
		/// </summary>
		public bool IsSelectedFolder
		{
			get { return SelectedNode != null && SelectedNode is FolderNodeViewModel; }
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public BlogReaderViewModel MainViewModel { get; }

		/// <summary>
		///		Comando de abrir archivo OPML
		/// </summary>
		public BaseCommand OpenOpmlCommand { get; }

		/// <summary>
		///		Comando de nueva carpeta
		/// </summary>
		public BaseCommand NewFolderCommand { get; }

		/// <summary>
		///		Comando de nuevo blog
		/// </summary>
		public BaseCommand NewBlogCommand { get; }

		/// <summary>
		///		Comando de descarga
		/// </summary>
		public BaseCommand DownloadCommand { get; }

		/// <summary>
		///		Comando de ver el contenido de un blog
		/// </summary>
		public BaseCommand SeeNewsCommand { get; }
	}
}
