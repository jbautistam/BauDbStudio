using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers.Files
{
	/// <summary>
	///		ViewModel de un árbol de archivos
	/// </summary>
	public class TreeFilesViewModel : BaseTreeViewModel
	{	
		// Variables privadas
		private NodeFileViewModel _nodeToCopy;

		public TreeFilesViewModel(SolutionViewModel solutionViewModel) : base(solutionViewModel)
		{ 
			NewFolderFilesCommand = new BaseCommand(_ => AddFolderToExplorer());
			NewFolderCommand = new BaseCommand(_ => CreateFolder(), _ => CanCreateFileOrFolder())
										.AddListener(this, nameof(SelectedNode));
			NewFileCommand = new BaseCommand(_ => CreateFile(), _ => CanCreateFileOrFolder())
										.AddListener(this, nameof(SelectedNode));
			RenameCommand = new BaseCommand(_ => Rename(), _ => CanRename())
										.AddListener(this, nameof(SelectedNode));
			ProcessScriptCommand = new BaseCommand(_ => ExecuteScript(), _ => CanExecuteScript())
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
				foreach (string path in SolutionViewModel.Solution.Folders)
					if (!string.IsNullOrWhiteSpace(path) && System.IO.Directory.Exists(path))
						paths.Add(path);
				// Ordena por el nombre del directorio
				paths.Sort((first, second) => System.IO.Path.GetFileName(first).CompareTo(System.IO.Path.GetFileName(second)));
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
			SolutionViewModel.MainViewModel.MainController.HostController.DialogsController.OpenDialogSelectPath(string.Empty, out string folder);
			// Añade la carpeta a la solución
			if (!string.IsNullOrWhiteSpace(folder) && System.IO.Directory.Exists(folder))
			{
				// Añade la carpeta a la solución
				SolutionViewModel.Solution.AddFolder(folder);
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
				// Carga el árbol
				Load();
			}
		}

		/// <summary>
		///		Copia archivos desde el explorador
		/// </summary>
		public void CopyFromExplorer(string path, string[] files)
		{
			if (string.IsNullOrWhiteSpace(path) || !System.IO.Directory.Exists(path))
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione la carpeta donde desea copiar los archivos");
			else
			{
				// Copia los archivos
				foreach (string file in files)
					if (!string.IsNullOrWhiteSpace(file) && System.IO.File.Exists(file))
						LibHelper.Files.HelperFiles.CopyFile(file, System.IO.Path.Combine(path, System.IO.Path.GetFileName(file)));
				// Actualiza el árbol
				Load();
			}
		}

		/// <summary>
		///		Comprueba si se puede crear una carpeta o un achivo
		/// </summary>
		private bool CanCreateFileOrFolder()
		{
			return !string.IsNullOrWhiteSpace(GetSelectedFolder());
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción general
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			BaseTreeNodeViewModel.NodeType type = GetSelectedNodeType();
			bool isFolder = SelectedNode is NodeFolderRootViewModel || ((SelectedNode as NodeFileViewModel)?.IsFolder ?? false);

				// Devuelve el valor que indica si puede ejecutar la acción
				switch (action)
				{
					case nameof(OpenCommand):
						return type == BaseTreeNodeViewModel.NodeType.File && !isFolder;
					case nameof(CopyCommand):
						return type == BaseTreeNodeViewModel.NodeType.File;
					case nameof(PasteCommand):
						return _nodeToCopy != null && SelectedNode != null && isFolder;
					case nameof(PasteClipboardImageCommand):
						return isFolder && SolutionViewModel.MainViewModel.MainController.ClipboardContainImage();
					default:
						return true;
				}
		}

		/// <summary>
		///		Abre la ventana de propiedades de un nodo
		/// </summary>
		protected override void OpenProperties()
		{
			switch (GetSelectedNodeType())
			{
				case BaseTreeNodeViewModel.NodeType.File:
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
			{
				if (node.FileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
					SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Files.ParquetFileViewModel(SolutionViewModel, node.FileName));
				else if (node.FileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
					SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Files.CsvFileViewModel(SolutionViewModel, node.FileName));
				else if (node.FileName.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase) ||
						 node.FileName.EndsWith(".xls", StringComparison.CurrentCultureIgnoreCase))
					SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Files.ExcelFileViewModel(SolutionViewModel, node.FileName));
				else if (IsImage(node.FileName))
					SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Files.ImageViewModel(SolutionViewModel, node.FileName));
				else
					SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Files.FileViewModel(SolutionViewModel, node.FileName));
			}
		}

		/// <summary>
		///		Comprueba si es un archivo de imagen
		/// </summary>
		private bool IsImage(string fileName)
		{
			return fileName.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".tiff", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase);
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

						if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowInputString("Nombre del directorio", ref fileName) 
										== BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
						{
							// Quita los espacios
							fileName = fileName.TrimIgnoreNull();
							// Crea el directorio  y actualiza el árbol
							if (!string.IsNullOrWhiteSpace(fileName) &&
									LibHelper.Files.HelperFiles.MakePath(System.IO.Path.Combine(path, fileName)))
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
					Tools.CreateFileViewModel createFileViewModel = new Tools.CreateFileViewModel(SolutionViewModel, path);
					
						if (SolutionViewModel.MainViewModel.MainController.OpenDialog(createFileViewModel)
										== BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes &&
							!string.IsNullOrWhiteSpace(createFileViewModel.FileName))
						{
							// Graba el archivo
							LibHelper.Files.HelperFiles.SaveTextFile(createFileViewModel.FullFileName, string.Empty, GetEncoder(createFileViewModel.GetSelectedEncoding()));
							// Abre la ventana
							SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Files.FileViewModel(SolutionViewModel, createFileViewModel.FullFileName));
							// Actualiza el árbol
							Load();
						}
				}
		}

		/// <summary>
		///		Obtiene la codificación adecuada para el archivo
		/// </summary>
		private System.Text.Encoding GetEncoder(Tools.CreateFileViewModel.Encoding encoding)
		{
			switch (encoding)
			{
				case Tools.CreateFileViewModel.Encoding.Utf8:
					return System.Text.Encoding.UTF8;
				case Tools.CreateFileViewModel.Encoding.Utf8NoBom:
					return new System.Text.UTF8Encoding(false);
				case Tools.CreateFileViewModel.Encoding.Utf7:
					return System.Text.Encoding.UTF7;
				case Tools.CreateFileViewModel.Encoding.Utf32:
					return System.Text.Encoding.UTF32;
				case Tools.CreateFileViewModel.Encoding.Unicode:
					return System.Text.Encoding.Unicode;
				default:
					return System.Text.Encoding.ASCII;
			}
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
							path = System.IO.Path.GetDirectoryName(pathNode.FileName);
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
					if (System.IO.Directory.Exists(source))
					{
						if (target.StartsWith(source, StringComparison.CurrentCultureIgnoreCase))
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage($"No se pude copiar {source} sobre {target}");
						else
						{
							// Obtiene el nombre del directorio destino
							target = LibHelper.Files.HelperFiles.GetConsecutivePath(target, System.IO.Path.GetFileName(source));
							// Copia el directorio
							LibHelper.Files.HelperFiles.CopyPath(source, target);
						}
					}
					else
					{
						// Obtiene el nombre del archivo
						target = LibHelper.Files.HelperFiles.GetConsecutiveFileName(target, System.IO.Path.GetFileName(source));
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
			if (!SolutionViewModel.MainViewModel.MainController.ClipboardContainImage())
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No hay ninguna imagen en el portapapeles");
			else
			{
				string fileName = SolutionViewModel.MainViewModel.MainController.HostController.DialogsController.OpenDialogSave(GetSelectedFolder(),
																																 "Archivos de imagen (*.jpg)|*.jpg",
																																 "NewImage.jpg");

					if (!string.IsNullOrWhiteSpace(fileName))
					{
						if (!SolutionViewModel.MainViewModel.MainController.SaveClipboardImage(fileName))
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No se ha podido grabar la imagen");
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
			if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Desea quitar la carpeta '{item.Text}' de la solución?"))
			{
				// Elimina la carpeta
				SolutionViewModel.Solution.RemoveFolder(item.FileName);
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
				// Actualiza el árbol
				Load();
			}
		}

		/// <summary>
		///		Borra un directorio o archivo del sistema
		/// </summary>
		private void DeleteFile(string fileName)
		{
			if (System.IO.Directory.Exists(fileName))
			{
				if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Realmente desea eliminar el directorio {System.IO.Path.GetFileName(fileName)}?"))
				{
					// Elimina el directorio
					LibHelper.Files.HelperFiles.KillPath(fileName);
					// Cierra las ventanas abiertas en este directorio
					CloseWindows(fileName, true);
					// Actualiza el árbol
					Load();
				}
			}
			else if (System.IO.File.Exists(fileName))
			{
				if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Realmente desea eliminar el archivo {System.IO.Path.GetFileName(fileName)}?"))
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
		///		Ejecuta un script
		/// </summary>
		private void ExecuteScript()
		{
			string fileName = GetSelectedFile();
			bool executeXml = false;

				// Si el archivo seleccionado es XML, lo ejecuta sobre una consola
				if (!string.IsNullOrWhiteSpace(fileName))
				{
					if (fileName.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
					{
						// Ejecuta el archivo XML en una consola
						ExecuteEtlScript(fileName);
						// Indica que se trata de un archivo XML ejecutado sobre consola
						executeXml = true;
					}
				}
				// Si no se trata de un archivo de proyecto XML (que se va a ejecutar en consola), se ejecutan los SQL
				if (!executeXml)
				{
					List<string> files = GetFilesFromPath(fileName, ".sql");

						// Ejecuta los archivos (si ha encontrado alguno)
						if (files.Count == 0)
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No se encuentra ningún archivo SQL para ejecutar");
						else
							SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Connections.ExecuteFilesViewModel(SolutionViewModel, files));
				}
		}

		/// <summary>
		///		Obtiene los archivos SQL de un directorio (o el archivo seleccionado)
		/// </summary>
		private List<string> GetFilesFromPath(string selectedFileName, string extension)
		{
			List<string> files = new List<string>();

				// Obtiene el archivo seleccionado o los archivos de un directorio
				if (!string.IsNullOrWhiteSpace(selectedFileName) && selectedFileName.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
					files.Add(selectedFileName);
				else
				{
					// Obtiene la lista de todos los archivos
					files = LibHelper.Files.HelperFiles.ListRecursive(GetSelectedFolder(), $"*{extension}");
					// Quita los archivos que no coincidan con la máscara
					for (int index = files.Count - 1; index	>= 0; index--)
						if (!files[index].EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
							files.RemoveAt(index);
					// Ordena los archivos
					files.Sort((first, second) => first.CompareIgnoreNullTo(second));
				}
				// Devuelve la colección de archivos
				return files;
		}

		/// <summary>
		///		Ejecuta un script de XML
		/// </summary>
		private void ExecuteEtlScript(string fileName)
		{
			SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.EtlProjects.ExecuteEtlConsoleViewModel(SolutionViewModel, fileName));
		}

		/// <summary>
		///		Comprueba si se puede ejecutar un script
		/// </summary>
		private bool CanExecuteScript()
		{
			string fileName = GetSelectedFile();

				return (!string.IsNullOrWhiteSpace(fileName) && fileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase)) ||
							!string.IsNullOrWhiteSpace(GetSelectedFolder());
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
							path = System.IO.Path.GetDirectoryName(fileNode.FileName);
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
					path = System.IO.Path.GetDirectoryName(file);
				else
					path = GetSelectedPath();
				// Abre el explorador sobre el directorio
				if (!string.IsNullOrWhiteSpace(path) && System.IO.Directory.Exists(path))
					SolutionViewModel.MainViewModel.MainController.OpenExplorer(path);
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
					string newFileName = GetNewFileName(System.IO.Path.GetFileName(oldFileName), isFolder);

						if (!string.IsNullOrWhiteSpace(newFileName))
						{
							// Obtiene el nombre completo del archivo / carpeta
							newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(oldFileName), newFileName);
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
				if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowInputString(title, ref newFileName) 
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
			List<Details.IDetailViewModel> viewModels = SolutionViewModel.MainViewModel.MainController.GetOpenedDetails();

				foreach (Details.IDetailViewModel viewModel in viewModels)
					if (viewModel is Details.Files.FileViewModel fileViewModel)
					{
						string newName = newFileName;
						bool mustUpdate = false;

							// Cambia el nombre si se trata de un directorio
							if (isFolder && System.IO.Path.GetDirectoryName(fileViewModel.FileName).Equals(oldFileName, StringComparison.CurrentCultureIgnoreCase))
							{
								newName = System.IO.Path.Combine(newFileName, System.IO.Path.GetFileName(fileViewModel.FileName));
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
									SolutionViewModel.MainViewModel.MainController.UpdateTabId(oldWindowId, fileViewModel.TabId, fileViewModel.Header);
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
			List<Details.IDetailViewModel> viewModels = SolutionViewModel.MainViewModel.MainController.GetOpenedDetails();

				foreach (Details.IDetailViewModel viewModel in viewModels)
					if (MustClose(viewModel, fileName, isFolder))
					{
						// Indica que no se ha modificado (porque se ha borrado el archivo)
						viewModel.IsUpdated = false;
						// Cierra la ventana
						SolutionViewModel.MainViewModel.MainController.CloseWindow(viewModel.TabId);
					}
		}

		/// <summary>
		///		Comprueba si debe cerrar una ficha de detalles
		/// </summary>
		private bool MustClose(Details.IDetailViewModel viewModel, string fileName, bool isFolder)
		{
			string viewModelFileName = string.Empty;
			bool mustClose = false;

				// Obtiene el nombre de archivos
				switch (viewModel)
				{
					case Details.Files.FileViewModel fileViewModel:
							viewModelFileName = fileViewModel.FileName;
						break;
					case Details.Files.BaseFileViewModel fileViewModel:
							viewModelFileName = fileViewModel.FileName;
						break;
				}
				// Comprueba si se debe cerrar
				if (!string.IsNullOrWhiteSpace(viewModelFileName))
					mustClose = (isFolder && System.IO.Path.GetDirectoryName(viewModelFileName).Equals(fileName, StringComparison.CurrentCultureIgnoreCase)) ||
								(!isFolder && viewModelFileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase));
				// Devuelve el valor que indica si se debe cerrar
				return mustClose;
		}

		/// <summary>
		///		Indica si se puede renombrar un archivo o carpeta
		/// </summary>
		private bool CanRename()
		{
			return !string.IsNullOrEmpty(GetSelectedPath());
		}

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
		///		Comando para procesar un script
		/// </summary>
		public BaseCommand ProcessScriptCommand { get; }

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
}
