using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;

/// <summary>
///		Clase de ayuda para obtener los nodos de directorios y archivos
/// </summary>
internal class HelperFileNodes(TreeFilesViewModel viewModel, PluginNodeViewModel node)
{
	/// <summary>
	///		Obtiene la lista de nodos hijo
	/// </summary>
	internal List<ControlHierarchicalViewModel> GetChildNodes(string rootFileName)
	{
		List<ControlHierarchicalViewModel> nodes = [];

			// Carga los nodos
			if (!string.IsNullOrWhiteSpace(rootFileName) && Directory.Exists(rootFileName))
			{
				// Carga los directorios
				foreach (string fileName in GetFolders(rootFileName))
					nodes.Add(GetNode(fileName, true));
				// Carga los archivos
				foreach (string fileName in GetFiles(rootFileName))
					nodes.Add(GetNode(fileName, false));
			}
			// Devuelve la lista
			return nodes;

		// Obtiene los nombres de carpetas de forma segura: evita las excepciones como por ejemplo un error de permisos
		List<string> GetFolders(string fileName)
		{
			try
			{
				List<string> paths = [];

					// Añade las carpetas a la lista
					foreach (DirectoryInfo folder in new DirectoryInfo(fileName).GetDirectories())
						if (MustAdd(folder.Attributes))
							paths.Add(folder.FullName);
					// Ordena los directorios
					paths.Sort();
					// Devuelve los directorios
					return paths;
			}
			catch
			{
				return [];
			}
		}
		 
		// Obtiene los nombres de archivo de forma segura: evita las excepciones como por ejemplo un error de permisos
		List<string> GetFiles(string fileName)
		{
			try
			{
				List<string> paths = [];

					// Añade las carpetas a la lista
					foreach (FileInfo file in new DirectoryInfo(fileName).GetFiles())
						if (MustAdd(file.Attributes))
							paths.Add(file.FullName);
					// Ordena los archivos
					paths.Sort();
					// Devuelve los directorios
					return paths;
			}
			catch
			{
				return [];
			}
		}

		// Comprueba si se debe añadir un archivo o directorio atendiendo a sus atributos
		bool MustAdd(FileAttributes attributes) => !attributes.HasFlag(FileAttributes.Hidden) && !attributes.HasFlag(FileAttributes.System);
	}

	/// <summary>
	///		Obtiene un nodo
	/// </summary>
	private NodeFileViewModel GetNode(string fileName, bool isFolder) => new(ViewModel, Node, fileName, isFolder);

	/// <summary>
	///		ViewModel del árbol
	/// </summary>
	private TreeFilesViewModel ViewModel { get; } = viewModel;

	/// <summary>
	///		Nodo a partir del que se generan los archivos
	/// </summary>
	private PluginNodeViewModel Node { get; } = node;
}
