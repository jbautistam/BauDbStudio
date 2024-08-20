using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibBlogReader.Model;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibMarkupLanguage.Services.XML;

namespace Bau.Libraries.LibBlogReader.Repository;

/// <summary>
///		Repository para la estructura de blogs
/// </summary>
internal class BlogsStructureRepository
{   
	// Constantes privadas
	private const string TagRoot = "Blogs";
	private const string TagFolder = "Folder";
	private const string TagBlog = "Blog";
	private const string TagID = "ID";
	private const string TagName = "Name";
	private const string TagDescription = "Description";
	private const string TagPath = "Path";
	private const string TagUrl = "Url";
	private const string TagDownloadPodcast = "DownloadPodcast";
	private const string TagEnabled = "Enabled";
	private const string TagNumberNotRead = "NumberNotRead";
	private const string TagDateLastDownload = "DateLastDownload";
	private const string TagDateLastEntry = "DateLastEntry";
	private const string TagHyperlink = "Hyperlink";
	private const string TagMaximumEntriesRead = "MaximumEntriesRead";

	/// <summary>
	///		Carga una estructura de carpetas
	/// </summary>
	internal FolderModel Load(string path)
	{
		FolderModel folder = new();
		string fileName = GetFileName(path);
		MLFile fileML = new XMLParser().Load(fileName);

			// Si existe el archivo, lo carga
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							if (nodeML.Name == TagFolder)
								folder.Folders.Add(LoadFolder(folder, nodeML));
							else if (nodeML.Name == TagBlog)
								folder.Blogs.Add(LoadBlog(folder, nodeML));
							else if (nodeML.Name == TagHyperlink)
								folder.Hyperlinks.Add(LoadHyperlink(folder, nodeML));
			// Devuelve la carpeta cargada
			return folder;
	}

	/// <summary>
	///		Carga los datos de una carpeta de un nodo
	/// </summary>
	private FolderModel LoadFolder(FolderModel parent, MLNode rootML)
	{
		FolderModel folder = new();

			// Carga los datos de la carpeta
			folder.Name = rootML.Nodes[TagName].Value;
			folder.NumberNotRead = rootML.Attributes[TagNumberNotRead].Value.GetInt(0);
			folder.Parent = parent;
			// Carga los hijos de la carpetas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagFolder)
					folder.Folders.Add(LoadFolder(folder, nodeML));
				else if (nodeML.Name == TagBlog)
					folder.Blogs.Add(LoadBlog(folder, nodeML));
				else if (nodeML.Name == TagHyperlink)
					folder.Hyperlinks.Add(LoadHyperlink(folder, nodeML));
			// Devuelve la carpeta
			return folder;
	}

	/// <summary>
	///		Carga los datos de un blog
	/// </summary>
	private BlogModel LoadBlog(FolderModel parent, MLNode nodeML)
	{
		BlogModel blog = new();

			// Carga los datos
			blog.Folder = parent;
			blog.GlobalId = nodeML.Attributes[TagID].Value;
			blog.Name = nodeML.Nodes[TagName].Value;
			blog.Description = nodeML.Nodes[TagDescription].Value;
			blog.Path = nodeML.Nodes[TagPath].Value;
			blog.URL = nodeML.Nodes[TagUrl].Value;
			blog.NumberNotRead = nodeML.Attributes[TagNumberNotRead].Value.GetInt(0);
			blog.MaximumEntriesRead = nodeML.Attributes[TagMaximumEntriesRead].Value.GetInt(0);
			blog.DateLastDownload = nodeML.Attributes[TagDateLastDownload].Value.GetDateTime();
			blog.DateLastEntry = nodeML.Attributes[TagDateLastEntry].Value.GetDateTime();
			blog.DownloadPodcast = nodeML.Attributes[TagDownloadPodcast].Value.GetBool();
			blog.Enabled = nodeML.Attributes[TagEnabled].Value.GetBool();
			// Devuelve los datos del blog
			return blog;
	}

	/// <summary>
	///		Carga los datos de un <see cref="HyperlinkModel"/>
	/// </summary>
	private HyperlinkModel LoadHyperlink(FolderModel parent, MLNode nodeML)
	{
		HyperlinkModel hyperlink = new();

			// Carga los datos
			hyperlink.Folder = parent;
			hyperlink.GlobalId = nodeML.Attributes[TagID].Value;
			hyperlink.Name = nodeML.Nodes[TagName].Value;
			hyperlink.Description = nodeML.Nodes[TagDescription].Value;
			hyperlink.Url = nodeML.Nodes[TagUrl].Value;
			// Devuelve los datos del hipervínculo
			return hyperlink;
	}

	/// <summary>
	///		Graba una estructura de carpetas
	/// </summary>
	internal void Save(FolderModel folder, string path)
	{
		MLFile fileML = new();
		MLNode nodeML = fileML.Nodes.Add(TagRoot);

			// Añade los nodos de las carpetas
			foreach (FolderModel child in folder.Folders)
				nodeML.Nodes.Add(GetNode(child));
			// Añade los nodos de los blogs
			foreach (BlogModel blog in folder.Blogs)
				nodeML.Nodes.Add(GetNode(blog));
			// Añade los nodos de los hipervínculos
			foreach (HyperlinkModel hyperlink in folder.Hyperlinks)
				nodeML.Nodes.Add(GetNode(hyperlink));
			// Graba el archivo
			new XMLWriter().Save(GetFileName(path), fileML);
	}

	/// <summary>
	///		Obtiene el nodo XML de una carpeta
	/// </summary>
	private MLNode GetNode(FolderModel folder)
	{
		MLNode nodeML = new(TagFolder);

			// Añade los datos de la carpeta
			nodeML.Attributes.Add(TagNumberNotRead, folder.NumberNotRead);
			nodeML.Nodes.Add(TagName, folder.Name);
			// Añade los nodos de las carpetas
			foreach (FolderModel child in folder.Folders)
				nodeML.Nodes.Add(GetNode(child));
			// Añade los nodos de los blogs
			foreach (BlogModel blog in folder.Blogs)
				nodeML.Nodes.Add(GetNode(blog));
			// Añade los nodos de los hipervínculos
			foreach (HyperlinkModel hyperlink in folder.Hyperlinks)
				nodeML.Nodes.Add(GetNode(hyperlink));
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene el nodo XML de un blog
	/// </summary>
	private MLNode GetNode(BlogModel blog)
	{
		MLNode nodeML = new(TagBlog);

			// Añade los datos del nodo
			nodeML.Attributes.Add(TagID, blog.GlobalId);
			nodeML.Attributes.Add(TagNumberNotRead, blog.NumberNotRead);
			nodeML.Attributes.Add(TagMaximumEntriesRead, blog.MaximumEntriesRead);
			nodeML.Attributes.Add(TagDateLastDownload, blog.DateLastDownload ?? DateTime.Now);
			nodeML.Attributes.Add(TagDateLastEntry, blog.DateLastEntry ?? DateTime.Now);
			nodeML.Attributes.Add(TagDownloadPodcast, blog.DownloadPodcast);
			nodeML.Attributes.Add(TagEnabled, blog.Enabled);
			nodeML.Nodes.Add(TagName, blog.Name);
			nodeML.Nodes.Add(TagDescription, blog.Description);
			nodeML.Nodes.Add(TagPath, blog.Path);
			nodeML.Nodes.Add(TagUrl, blog.URL);
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene el nodo XML de un blog
	/// </summary>
	private MLNode GetNode(HyperlinkModel hyperlink)
	{
		MLNode nodeML = new(TagHyperlink);

			// Añade los datos del nodo
			nodeML.Attributes.Add(TagID, hyperlink.GlobalId);
			nodeML.Nodes.Add(TagName, hyperlink.Name);
			nodeML.Nodes.Add(TagDescription, hyperlink.Description);
			nodeML.Nodes.Add(TagUrl, hyperlink.Url);
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene el nombre de archivo
	/// </summary>
	private string GetFileName(string path) => Path.Combine(path, "BlogsStructure.xml");
}
