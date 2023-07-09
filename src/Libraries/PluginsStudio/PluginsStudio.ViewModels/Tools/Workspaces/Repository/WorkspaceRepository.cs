using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.Workspaces.Repository;

/// <summary>
///		Repositorio para los espacios de trabajo
/// </summary>
internal class WorkspaceRepository
{
	// Constantes privadas
	private const string TagRoot = "Workspace";
	private const string TagFolder = "Folder";
	private const string TagPath = "Path";

	/// <summary>
	///		Carga un archivo
	/// </summary>
	internal List<string> Load(string fileName)
	{
		List<string> folders = new List<string>();
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga el archivo
			if (fileML != null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							switch (nodeML.Name)
							{
								case TagFolder:
										string folder = nodeML.Attributes[TagPath].Value.TrimIgnoreNull();

											if (!string.IsNullOrWhiteSpace(folder))
												folders.Add(folder);
									break;
							}
			// Devuelve las carpetas leidas
			return folders;
	}

	/// <summary>
	///		Graba un archivo
	/// </summary>
	internal void Save(string fileName, List<string> folders)
	{
		MLFile fileML = new MLFile();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Asigna las carpetas
			foreach (string folder in folders)
				if (!string.IsNullOrWhiteSpace(folder))
				{
					MLNode nodeML = rootML.Nodes.Add(TagFolder);

						// Añade el atributo
						nodeML.Attributes.Add(TagPath, folder);
				}
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}
}
