using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;

namespace Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;

/// <summary>
///		ViewModel de un árbol de archivos
/// </summary>
public class TreeFilesViewModel : BaseTreeViewModel
{
	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public enum NodeType
	{
		/// <summary>Desconocido. No se debería utilizar</summary>
		Unknown,
		/// <summary>Raíz de archivos de proyecto</summary>
		FilesRoot,
		/// <summary>Archivo / directorio</summary>
		File
	}

	// Variables privadas
	private NodeFileViewModel? _nodeToCopy;

	public TreeFilesViewModel(PluginsStudioViewModel solutionViewModel)
	{ 
		// Inicializa las propiedades
		MainViewModel = solutionViewModel;
		PluginsFileOptions = new List<FileOptionsModel>();
		// Inicializa los comandos
		NewFolderFilesCommand = new BaseCommand(_ => AddFolderToExplorer());
		NewFolderCommand = new BaseCommand(_ => CreateFolder(), _ => CanCreateFileOrFolder())
									.AddListener(this, nameof(SelectedNode));
		NewFileCommand = new BaseCommand(_ => CreateFile(), _ => CanCreateFileOrFolder())
									.AddListener(this, nameof(SelectedNode));
		RenameCommand = new BaseCommand(_ => Rename(), _ => CanRename())
									.AddListener(this, nameof(SelectedNode));
		CopyCommand = new BaseCommand(_ => CopyFile(), _ => CanExecuteAction(nameof(CopyCommand)))
									.AddListener(this, nameof(SelectedNode));
		PasteCommand = new BaseCommand(_ => PasteFile(), _ => CanExecuteAction(nameof(PasteCommand)))
									.AddListener(this, nameof(SelectedNode));
		PasteClipboardImageCommand = new BaseCommand(_ => PasteClipboardImage(), _ => CanExecuteAction(nameof(PasteClipboardImageCommand)))
										.AddListener(this, nameof(SelectedNode));
		SeeAtExplorerCommand = new BaseCommand(_ => OpenFileExplorer());
	}

	/// <summary>
	///		Carga los nodos hijo
	/// </summary>
	protected override void AddRootNodes()
	{
		List<string> paths = new List<string>();

			// Añade los directorios
			if (MainViewModel.WorkspacesViewModel.SelectedItem != null)
				foreach (string path in MainViewModel.WorkspacesViewModel.SelectedItem.Folders)
					if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
						paths.Add(path);
			// Ordena por el nombre del directorio
			paths.Sort((first, second) => Path.GetFileName(first).CompareTo(Path.GetFileName(second)));
			// Recarga los nodos
			foreach (string path in paths)
				Children.Add(new NodeFolderRootViewModel(this, null, path));
	}

	/// <summary>
	///		Añade una carpeta al explorador
	/// </summary>
	internal void AddFolderToExplorer()
	{
		// Selecciona la carpeta
		MainViewModel.PluginsStudioController.MainWindowController.DialogsController.OpenDialogSelectPath(string.Empty, out string folder);
		// Añade la carpeta a la solución
		if (!string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder))
		{
			// Añade la carpeta a la solución
			MainViewModel.WorkspacesViewModel.SelectedItem?.AddFolder(folder);
			// Carga el árbol
			Load();
		}
	}

	/// <summary>
	///		Copia archivos desde el explorador
	/// </summary>
	public void CopyFromExplorer(string path, string[] files)
	{
		if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
			MainViewModel.PluginsStudioController.MainWindowController.SystemController.ShowMessage("Seleccione la carpeta donde desea copiar los archivos");
		else
		{
			// Copia los archivos y/o directorios
			foreach (string file in files)
				if (!string.IsNullOrWhiteSpace(file))
				{
					if (File.Exists(file))
						LibHelper.Files.HelperFiles.CopyFile(file, Path.Combine(path, Path.GetFileName(file)));
					else if (Directory.Exists(file))
						LibHelper.Files.HelperFiles.CopyPath(file, Path.Combine(path, Path.GetFileName(file)));
				}
			// Actualiza el árbol
			Load();
		}
	}

	/// <summary>
	///		Comprueba si se puede crear una carpeta o un achivo
	/// </summary>
	private bool CanCreateFileOrFolder() => !string.IsNullOrWhiteSpace(GetSelectedFolder());

	/// <summary>
	///		Comprueba si se puede ejecutar una acción general
	/// </summary>
	protected override bool CanExecuteAction(string action)
	{
		NodeType type = GetSelectedNodeType().GetEnum(NodeType.Unknown);
		bool isFolder = SelectedNode is NodeFolderRootViewModel || ((SelectedNode as NodeFileViewModel)?.IsFolder ?? false);

			// Devuelve el valor que indica si puede ejecutar la acción
			switch (action)
			{
				case nameof(OpenCommand):
					return type == NodeType.File && !isFolder;
				case nameof(CopyCommand):
					return type == NodeType.File;
				case nameof(PasteCommand):
					return _nodeToCopy != null && SelectedNode != null && isFolder;
				case nameof(DeleteCommand):
					return SelectedNode is not null;
				case nameof(PasteClipboardImageCommand):
					return isFolder && MainViewModel.PluginsStudioController.MainWindowController.ClipboardContainImage();
				default:
					return true;
			}
	}

	/// <summary>
	///		Abre la ventana de propiedades de un nodo
	/// </summary>
	protected override void OpenProperties()
	{
		switch (GetSelectedNodeType().GetEnum(NodeType.Unknown))
		{
			case NodeType.File:
					OpenFile();
				break;
		}
	}

	/// <summary>
	///		Abre un archivo
	/// </summary>
	private void OpenFile()
	{
		if (SelectedNode is NodeFileViewModel node && !node.IsFolder)
			MainViewModel.PluginsStudioController.HostPluginsController.OpenFile(node.FileName);
	}

	/// <summary>
	///		Crea una carpeta
	/// </summary>
	private void CreateFolder()
	{
		string path = GetSelectedFolder();

			if (!string.IsNullOrWhiteSpace(path))
			{
				string fileName = "Nuevo directorio";

					if (MainViewModel.PluginsStudioController.MainWindowController.SystemController.ShowInputString("Nombre del directorio", ref fileName) 
									== BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
					{
						// Quita los espacios
						fileName = fileName.TrimIgnoreNull();
						// Crea el directorio  y actualiza el árbol
						if (!string.IsNullOrWhiteSpace(fileName) &&
								LibHelper.Files.HelperFiles.MakePath(Path.Combine(path, fileName)))
							Load();
					}
			}
	}

	/// <summary>
	///		Crea un archivo
	/// </summary>
	private void CreateFile()
	{
		string path = GetSelectedFolder();

			if (!string.IsNullOrWhiteSpace(path))
			{
				Tools.CreateFileViewModel createFileViewModel = new Tools.CreateFileViewModel(MainViewModel, path, 
																							  MainViewModel.PluginsStudioController.PluginsController.HostPluginsController.GetFilesAssigned());

					if (MainViewModel.PluginsStudioController.OpenDialog(createFileViewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes &&
						!string.IsNullOrWhiteSpace(createFileViewModel.FileName))
					{
						// Graba el archivo
						LibHelper.Files.HelperFiles.SaveTextFile(createFileViewModel.FullFileName, 
																 GetTemplate(createFileViewModel.FileName, createFileViewModel.FilesAssigned), 
																 GetEncoder(createFileViewModel.GetSelectedEncoding()));
						// Abre la ventana
						MainViewModel.PluginsStudioController.HostPluginsController.OpenFile(createFileViewModel.FullFileName);
						// Actualiza el árbol
						Load();
					}
			}
	}

	/// <summary>
	///		Obtiene la plantilla asociada a una extensión
	/// </summary>
	private string GetTemplate(string fileName, List<FileAssignedModel> filesAssigned)
	{
		// Busca la extensión entre los archivos asignados
		foreach (FileAssignedModel fileAssigned in filesAssigned)
			if (fileName.EndsWith(fileAssigned.FileExtension, StringComparison.CurrentCultureIgnoreCase) &&
					!string.IsNullOrWhiteSpace(fileAssigned.Template))
				return fileAssigned.Template;
		// Si ha llegado hasta aquí es porque no ha encontrado ninguna plantilla
		return string.Empty;
	}

	/// <summary>
	///		Obtiene la codificación adecuada para el archivo
	/// </summary>
	private System.Text.Encoding GetEncoder(Tools.CreateFileViewModel.Encoding encoding)
	{
		return encoding switch
					{
						Tools.CreateFileViewModel.Encoding.Utf8 => System.Text.Encoding.UTF8,
						Tools.CreateFileViewModel.Encoding.Utf8NoBom => new System.Text.UTF8Encoding(false),
						Tools.CreateFileViewModel.Encoding.Utf32 => System.Text.Encoding.UTF32,
						Tools.CreateFileViewModel.Encoding.Unicode => System.Text.Encoding.Unicode,
						_ => System.Text.Encoding.ASCII
					};
	}

	/// <summary>
	///		Obtiene la carpeta seleccionada
	/// </summary>
	private string GetSelectedFolder()
	{
		string path = string.Empty;

			// Obtiene la carpeta del nodo
			if (SelectedNode != null)
			{
				if (SelectedNode is NodeFolderRootViewModel nodeFolder)
					path = nodeFolder.FileName;
				else if (SelectedNode is NodeFileViewModel pathNode)
				{
					if (pathNode.IsFolder)
						path = pathNode.FileName;
					else
						path = Path.GetDirectoryName(pathNode.FileName) ?? string.Empty;
				}
			}
			// Devuelve la carpeta
			return path;
	}

	/// <summary>
	///		Copia un archivo
	/// </summary>
	private void CopyFile()
	{
		if (SelectedNode is NodeFileViewModel node)
			_nodeToCopy = node;
		else
			_nodeToCopy = null;
	}

	/// <summary>
	///		Pega una carpeta / archivo
	/// </summary>
	private void PasteFile()
	{
		if (_nodeToCopy != null)
		{
			string target = GetSelectedPath();
			string source = _nodeToCopy.FileName;

				// Copia el directorio o el archivo
				if (Directory.Exists(source))
				{
					if (target.StartsWith(source, StringComparison.CurrentCultureIgnoreCase))
						MainViewModel.PluginsStudioController.MainWindowController.SystemController.ShowMessage($"No se pude copiar {source} sobre {target}");
					else
					{
						// Obtiene el nombre del directorio destino
						target = LibHelper.Files.HelperFiles.GetConsecutivePath(target, Path.GetFileName(source));
						// Copia el directorio
						LibHelper.Files.HelperFiles.CopyPath(source, target);
					}
				}
				else
				{
					// Obtiene el nombre del archivo
					target = LibHelper.Files.HelperFiles.GetConsecutiveFileName(target, Path.GetFileName(source));
					// Copia el archivo
					LibHelper.Files.HelperFiles.CopyFile(source, target);
				}
				// Actualiza el árbol
				Load();
				// ... y vacía el nodo de copia
				_nodeToCopy = null;
		}
	}

	/// <summary>
	///		Comando para pegar la imagen del portapapeles
	/// </summary>
	private void PasteClipboardImage()
	{
		if (!MainViewModel.PluginsStudioController.MainWindowController.ClipboardContainImage())
			MainViewModel.PluginsStudioController.MainWindowController.SystemController.ShowMessage("No hay ninguna imagen en el portapapeles");
		else
		{
			string fileName = MainViewModel.PluginsStudioController.MainWindowController.DialogsController
									.OpenDialogSave(GetSelectedFolder(),
													"Archivos PNG (*.png)|*.png|Archivos JPG (*.jpg)|*.jpg|Archivos BMP (*.bmp)|*.bmp|Archivos GIF (*.gif)|*.gif|Archivos TIFF (*.tiff)|*.tiff",
													"NewImage.png");

				if (!string.IsNullOrWhiteSpace(fileName))
				{
					if (!MainViewModel.PluginsStudioController.MainWindowController.SaveClipboardImage(fileName))
						MainViewModel.PluginsStudioController.MainWindowController.SystemController.ShowMessage("No se ha podido grabar la imagen");
					else
						Load();
				}
		}
	}

	/// <summary>
	///		Borra el elemento seleccionado
	/// </summary>
	protected override void DeleteItem()
	{
		switch (SelectedNode)
		{
			case NodeFolderRootViewModel item:
					DeleteRoot(item);
				break;
			case NodeFileViewModel item:
					DeleteFile(item.FileName);
				break;
		}
	}

	/// <summary>
	///		Borra una carpeta de la solución
	/// </summary>
	private void DeleteRoot(NodeFolderRootViewModel item)
	{
		if (MainViewModel.PluginsStudioController.MainWindowController.SystemController.ShowQuestion($"¿Desea quitar la carpeta '{item.Text}' de la solución?"))
		{
			// Elimina la carpeta
			MainViewModel.WorkspacesViewModel.SelectedItem?.RemoveFolder(item.FileName);
			// Actualiza el árbol
			Load();
		}
	}

	/// <summary>
	///		Borra un directorio o archivo del sistema
	/// </summary>
	private void DeleteFile(string fileName)
	{
		if (Directory.Exists(fileName))
		{
			if (MainViewModel.PluginsStudioController.MainWindowController.SystemController.ShowQuestion($"¿Realmente desea eliminar el directorio {Path.GetFileName(fileName)}?"))
			{
				// Elimina el directorio
				LibHelper.Files.HelperFiles.KillPath(fileName);
				// Cierra las ventanas abiertas en este directorio
				CloseWindows(fileName, true);
				// Actualiza el árbol
				Load();
			}
		}
		else if (File.Exists(fileName))
		{
			if (MainViewModel.PluginsStudioController.MainWindowController.SystemController.ShowQuestion($"¿Realmente desea eliminar el archivo {Path.GetFileName(fileName)}?"))
			{
				// Elimina el archivo
				LibHelper.Files.HelperFiles.KillFile(fileName);
				// Cierra la ventana abierta de este archivo
				CloseWindows(fileName, false);
				// Actualiza el árbol
				Load();
			}
		}
	}

	/// <summary>
	///		Guarda las opciones de los plugins asociados a los archivos
	/// </summary>
	public void AddPluginOptions(List<FileOptionsModel> fileOptions)
	{
		if (fileOptions is not null)
			PluginsFileOptions.AddRange(fileOptions);
	}

	/// <summary>
	///		Obtiene los menús asociados al archivo
	/// </summary>
	public List<MenuModel> GetFileMenus()
	{
		List<MenuModel> menus = new();

			// Obtiene las opciones de menú asociadas al nodo
			if (SelectedNode is not null && SelectedNode is NodeFileViewModel node)
			{
				// Asocia las opciones de los plugins
				if (PluginsFileOptions is not null)
					foreach (FileOptionsModel option in PluginsFileOptions)
						if (option.Check(node.IsFolder, node.FileName))
							menus.Add(option.Menu);
				// Asocia las opciones propias
				// ... por ahora no tiene ninguna opción
			}
			// Devuelve la lista de menús
			return menus;
	}

	/// <summary>
	///		Obtiene el archivo seleccionado
	/// </summary>
	private string GetSelectedFile()
	{
		if (SelectedNode != null && SelectedNode is NodeFileViewModel fileNode && !fileNode.IsFolder)
			return fileNode.FileName;
		else
			return string.Empty;
	}

	/// <summary>
	///		Obtiene el directorio seleccionado
	/// </summary>
	private string GetSelectedPath()
	{
		string path = string.Empty;

			// Obtiene el directorio
			if (SelectedNode != null)
			{
				if (SelectedNode is NodeFileViewModel fileNode)
				{
					if (fileNode.IsFolder)
						path = fileNode.FileName;
					else
						path = Path.GetDirectoryName(fileNode.FileName) ?? string.Empty;
				}
				else if (SelectedNode is NodeFolderRootViewModel filePath)
					path = filePath.FileName;
			}
			// Devuelve el directorio seleccionado
			return path;
	}

	/// <summary>
	///		Abre el archivo en el explorador
	/// </summary>
	private void OpenFileExplorer()
	{
		string file = GetSelectedFile();
		string path;

			// Obtiene el directorio a abrir
			if (!string.IsNullOrWhiteSpace(file))
				path = Path.GetDirectoryName(file) ?? string.Empty;
			else
				path = GetSelectedPath();
			// Abre el explorador sobre el directorio
			if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
				MainViewModel.PluginsStudioController.MainWindowController.OpenExplorer(path);
	}

	/// <summary>
	///		Cambia el nombre de archivo o carpeta
	/// </summary>
	private void Rename()
	{
		string oldFileName = GetSelectedFile();
		bool isFolder = false;

			// Si no se ha seleccionado un archivo, se coge la carpeta
			if (string.IsNullOrWhiteSpace(oldFileName))
			{
				oldFileName = GetSelectedPath();
				isFolder = true;
			}
			// Obtiene el nombre de archivo
			if (!string.IsNullOrWhiteSpace(oldFileName))
			{
				string newFileName = GetNewFileName(Path.GetFileName(oldFileName), isFolder);

					if (!string.IsNullOrWhiteSpace(newFileName))
					{
						// Obtiene el nombre completo del archivo / carpeta
						newFileName = Path.Combine(Path.GetDirectoryName(oldFileName) ?? string.Empty, newFileName);
						// Cambia el nombre
						if (LibHelper.Files.HelperFiles.Rename(oldFileName, newFileName))
						{
							// Cambia los nombres de los viewModel de archivos abiertos
							RenameOpenViewModels(oldFileName, isFolder, newFileName);
							// Recarga el árbol
							Load();
						}
					}
			}
	}

	/// <summary>
	///		Obtiene un nombre de archivo
	/// </summary>
	private string GetNewFileName(string fileName, bool isFolder)
	{
		string title = "Introduzca el nuevo nombre de archivo";
		string newFileName = fileName;

			// Cambia el título si es necesario
			if (isFolder)
				title = "Introduzca el nuevo nombre de directorio";
			// Obtiene el nuevo nombre de archivo
			if (MainViewModel.PluginsStudioController.MainWindowController.SystemController.ShowInputString(title, ref newFileName) 
						== BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				// Si no se ha cambiado el nombre, vacía el nombre del archivo de salida (no tiene en cuenta si sólo se han cambiado las mayúsculas)
				if (!string.IsNullOrWhiteSpace(newFileName))
				{
					newFileName = newFileName.TrimIgnoreNull();
					if (newFileName.Equals(fileName))
						newFileName = string.Empty;
				}
			}
			else
				newFileName = string.Empty;
			// Devuelve el nuevo nombre de archivo
			return newFileName;
	}

	/// <summary>
	///		Renombra los viewModels abiertos
	/// </summary>
	private void RenameOpenViewModels(string oldFileName, bool isFolder, string newFileName)
	{
		foreach (Base.Interfaces.IDetailViewModel viewModel in MainViewModel.PluginsStudioController.MainWindowController.GetOpenedDetails())
			if (viewModel is Base.Files.BaseFileViewModel fileViewModel)
			{
				string newName = newFileName;
				bool mustUpdate = false;

					// Cambia el nombre si se trata de un directorio
					if (isFolder && (Path.GetDirectoryName(fileViewModel.FileName) ?? string.Empty).Equals(oldFileName, StringComparison.CurrentCultureIgnoreCase))
					{
						newName = Path.Combine(newFileName, Path.GetFileName(fileViewModel.FileName));
						mustUpdate = true;
					}
					else if (!isFolder && fileViewModel.FileName.Equals(oldFileName, StringComparison.CurrentCultureIgnoreCase))
						mustUpdate = true;
					// Si se debe modificar el nombre
					if (mustUpdate)
					{
						bool isUpdated = fileViewModel.IsUpdated;
						string oldWindowId = fileViewModel.TabId;

							// Cambia el viewModel
							fileViewModel.FileName = newName;
							// Avisa a la ventana principal para cambiar los datos del documento abierto
							MainViewModel.PluginsStudioController.MainWindowController.UpdateTabId(oldWindowId, fileViewModel.TabId, fileViewModel.Header);
							// Recupera en el viewModel si ha habido modificaciones
							fileViewModel.IsUpdated = isUpdated;
					}
			}
	}

	/// <summary>
	///		Cierra las ventanas abiertas de un archivo o directorio
	/// </summary>
	private void CloseWindows(string fileName, bool isFolder)
	{
		foreach (Base.Interfaces.IDetailViewModel viewModel in MainViewModel.PluginsStudioController.MainWindowController.GetOpenedDetails())
			if (MustClose(viewModel, fileName, isFolder))
			{
				// Indica que no se ha modificado (porque se ha borrado el archivo)
				viewModel.IsUpdated = false;
				// Cierra la ventana
				MainViewModel.PluginsStudioController.MainWindowController.CloseWindow(viewModel.TabId);
			}
	}

	/// <summary>
	///		Comprueba si debe cerrar una ficha de detalles
	/// </summary>
	private bool MustClose(Base.Interfaces.IDetailViewModel viewModel, string fileName, bool isFolder)
	{
		string viewModelFileName = string.Empty;
		bool mustClose = false;

			// Obtiene el nombre de archivo
			if (viewModel is Base.Files.BaseFileViewModel fileViewModel)
				viewModelFileName = fileViewModel.FileName;
			// Comprueba si se debe cerrar
			if (!string.IsNullOrWhiteSpace(viewModelFileName))
				mustClose = (isFolder && (Path.GetDirectoryName(viewModelFileName) ?? string.Empty).Equals(fileName, StringComparison.CurrentCultureIgnoreCase)) ||
							(!isFolder && viewModelFileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase));
			// Devuelve el valor que indica si se debe cerrar
			return mustClose;
	}

	/// <summary>
	///		Indica si se puede renombrar un archivo o carpeta
	/// </summary>
	private bool CanRename() => !string.IsNullOrEmpty(GetSelectedPath());

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Opciones de los plugins asociadas a los archivos
	/// </summary>
	public List<FileOptionsModel> PluginsFileOptions { get; private set; }

	/// <summary>
	///		Comando para añadir un directorio de archivos a la solución
	/// </summary>
	public BaseCommand NewFolderFilesCommand { get; }

	/// <summary>
	///		Comando para crear una nueva carpeta
	/// </summary>
	public BaseCommand NewFolderCommand { get; }

	/// <summary>
	///		Comando para crear un nuevo archivo
	/// </summary>
	public BaseCommand NewFileCommand { get; }

	/// <summary>
	///		Comando para renombrar un archivo o carpeta
	/// </summary>
	public BaseCommand RenameCommand { get; }

	/// <summary>
	///		Comando para copiar un nodo
	/// </summary>
	public BaseCommand CopyCommand { get; }

	/// <summary>
	///		Comando para pegar un nodo
	/// </summary>
	public BaseCommand PasteCommand { get; }

	/// <summary>
	///		Comando para pegar una imagen
	/// </summary>
	public BaseCommand PasteClipboardImageCommand { get; }

	/// <summary>
	///		Comando para abrir en el explorador
	/// </summary>
	public BaseCommand SeeAtExplorerCommand { get; }
}
