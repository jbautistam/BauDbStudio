using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibBlobStorage;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Microsoft.Extensions.Logging;

namespace Bau.Libraries.CloudStudio.ViewModels.Explorers.Cloud
{
	/// <summary>
	///		ViewModel de un árbol de storage
	/// </summary>
	public class TreeStorageViewModel : BaseTreeViewModel
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
			File,
			/// <summary>Conexión a storage</summary>
			Storage,
			/// <summary>Contenedor de storage</summary>
			StorageContainer,
			/// <summary>Mensaje (transitorio)</summary>
			Message
		}
		/// <summary>
		///		Tipo de icono
		/// </summary>
		public enum IconType
		{
			Unknown,
			Deployment,
			Folder,
			File,
			Loading,
			Storage,
			Error
		}

		public TreeStorageViewModel(CloudStudioViewModel solutionViewModel)
		{ 
			SolutionViewModel = solutionViewModel;
			NewStorageCommand = new BaseCommand(_ => OpenStorage(null));
			NewContainerCommand = new BaseCommand(async _ => await NewContainerAsync(), _ => CanExecuteAction(nameof(NewContainerCommand)))
										.AddListener(this, nameof(SelectedNode));
			DeleteAsyncCommand = new BaseCommand(async _ => await DeleteItemAsync(), _ => SelectedNode != null)
										.AddListener(this, nameof(SelectedNode));
			UploadFileAsyncCommand = new BaseCommand(async _ => await UploadFileAsync(), _ => CanExecuteAction(nameof(UploadFileAsyncCommand)))
										.AddListener(this, nameof(SelectedNode));
			UploadFolderAsyncCommand = new BaseCommand(async _ => await UploadFolderAsync(), _ => CanExecuteAction(nameof(UploadFolderAsyncCommand)))
										.AddListener(this, nameof(SelectedNode));
			DownloadAsyncCommand = new BaseCommand(async _ => await DownloadAsync(), _ => CanExecuteAction(nameof(DownloadAsyncCommand)))
										.AddListener(this, nameof(SelectedNode));
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void AddRootNodes()
		{
			foreach (Models.Cloud.StorageModel storage in SolutionViewModel.Solution.Storages)
				Children.Add(new NodeStorageViewModel(this, null, storage));
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción general
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			NodeType type = GetSelectedNodeTypeConverted();

				// Comprueba la acción
				switch (action)
				{
					case nameof(OpenCommand):
					case nameof(NewContainerCommand):
						return type == NodeType.Storage;
					case nameof(DeleteCommand):
						return true;
					case nameof(UploadFileAsyncCommand):
					case nameof(UploadFolderAsyncCommand):
					case nameof(DownloadAsyncCommand):
						return type == NodeType.StorageContainer || type == NodeType.File;
					default:
						return false;
				}
		}

		/// <summary>
		///		Abre la ventana de propiedades de un nodo
		/// </summary>
		protected override void OpenProperties()
		{
			switch (GetSelectedNodeTypeConverted())
			{
				case NodeType.Storage:
						if (SelectedNode is NodeStorageViewModel node)
							OpenStorage(node);
					break;
			}
		}

		/// <summary>
		///		Obtiene el tipo de nodo del nodo seleccionado
		/// </summary>
		private NodeType GetSelectedNodeTypeConverted()
		{
			return GetSelectedNodeType().GetEnum(NodeType.Unknown);
		}

		/// <summary>
		///		Abre una ventana de propiedades de un storage
		/// </summary>
		private void OpenStorage(NodeStorageViewModel node)
		{
			if (SolutionViewModel.MainController.OpenDialog(new Details.Cloud.StorageViewModel(SolutionViewModel, node?.Storage)) == 
							BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				// Graba la solución
				SolutionViewModel.SaveSolution();
				// Actualiza el árbol
				Load();
			}
		}

		/// <summary>
		///		Crea un nuevo contenedor
		/// </summary>
		private async Task NewContainerAsync()
		{
			string container = string.Empty;

				if (SolutionViewModel.MainController.SystemController
							.ShowInputString("Introduzca el nombre del contenedor", ref container) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes &&
					!string.IsNullOrWhiteSpace(container))
				{
					AzureStorageBlobManager manager = GetStorageManager();

						if (manager == null)
							SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione un sistema de almacenamiento en el árbol");
						else
							try
							{
								// Crea el contenedor
								await manager.CreateContainerAsync(container);
								// Actualiza el árbol
								Load();
							}
							catch (Exception exception)
							{
								SolutionViewModel.MainController.Logger.LogError(exception, $"Error al crear el contenedor {container}");
							}
				}
		}

		/// <summary>
		///		Borra el elemento seleccionado
		/// </summary>
		protected override void DeleteItem()
		{	
			// ... no hace nada, en este caso va por DeleteItemAsync
		}

		/// <summary>
		///		Sube un directorio
		/// </summary>
		private async Task UploadFolderAsync()
		{
			if (SolutionViewModel.MainController.DialogsController.OpenDialogSelectPath(string.Empty, out string path) 
					== BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				if (string.IsNullOrWhiteSpace(path) || !System.IO.Directory.Exists(path))
					SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione un directorio");
				else
				{
					List<string> files = Convert(System.IO.Directory.GetFiles(path));

						if (files.Count > 0)
							await UploadFilesAsync(files);
						else
							SolutionViewModel.MainController.SystemController.ShowMessage("No hay ningún archivo en es directorio");
				}
			}
		}

		/// <summary>
		///		Sube archivos
		/// </summary>
		private async Task UploadFileAsync()
		{
			List<string> files = Convert(SolutionViewModel.MainController.DialogsController.OpenDialogLoadFiles(string.Empty, string.Empty));

				if (files.Count > 0)
					await UploadFilesAsync(files);
		}

		/// <summary>
		///		Convierte un array en una lista
		/// </summary>
		private List<string> Convert(string[] values)
		{
			List<string> result = new List<string>();

				// Añade los valores
				foreach (string value in values)
					result.Add(value);
				// Devuelve la lista de resultados
				return result;
		}
		
		/// <summary>
		///		Sube una lista de archivos
		/// </summary>
		private async Task UploadFilesAsync(List<string> files)
		{
			AzureStorageBlobManager manager = GetStorageManager();

				// Sube los archivos
				if (manager == null)
					SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione una cuenta de almacenamiento");
				else
				{
					(string container, bool isFolder, string remoteFileName) = GetStorageItems();

						if (string.IsNullOrWhiteSpace(container))
							SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione un contenedor");
						else
						{
							// Sube los archivos
							SolutionViewModel.MainController.Logger.LogInformation("Comienzo de la subida de archivos");
							// Sube cada archivo
							foreach (string file in files)
								if (!string.IsNullOrWhiteSpace(file) && System.IO.File.Exists(file))
								{
									string targetFile = System.IO.Path.GetFileName(file);

										// Si es un directorio, añade el directorio
										if (isFolder && !string.IsNullOrWhiteSpace(remoteFileName))
											targetFile = System.IO.Path.Combine(remoteFileName, targetFile);
										else if (!isFolder && !string.IsNullOrWhiteSpace(remoteFileName))
											targetFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(remoteFileName), targetFile);
										// Log
										SolutionViewModel.MainController.Logger.LogInformation($"Comienza la subida del archivo {file}");
										// Sube el archivo
										await manager.UploadAsync(container, targetFile, file);
										// Log
										SolutionViewModel.MainController.Logger.LogInformation($"Finaliza la subida del archivo {file}");
								}
							// Log
							SolutionViewModel.MainController.Logger.LogInformation("Fin de la subida de archivos");
							// Actualiza el árbol
							Load();
						}
				}
		}

		/// <summary>
		///		Descarga archivos
		/// </summary>
		private async Task DownloadAsync()
		{
			if (SolutionViewModel.MainController.DialogsController.OpenDialogSelectPath(string.Empty, out string path) 
					== BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				if (string.IsNullOrWhiteSpace(path) || !System.IO.Directory.Exists(path))
					SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione un directorio");
				else
				{
					AzureStorageBlobManager manager = GetStorageManager();

						// Descarga el contenedor, archivo o directorio
						if (manager == null)
							SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione una cuenta de almacenamiento");
						else 
						{
							// Descarga el contenedor, directorio o archivo
							try
							{
								if (SelectedNode is NodeStorageContainerViewModel item)
									await DownloadFolderAsync(manager, path, item.Container, string.Empty);
								else if (SelectedNode is NodeStorageContainerFileViewModel folder && folder.IsFolder)
									await DownloadFolderAsync(manager, path, folder.Blob.Blob.Container, System.IO.Path.GetDirectoryName(folder.Blob.Blob.FullFileName));
								else if (SelectedNode is NodeStorageContainerFileViewModel file && !file.IsFolder)
									await DownloadFileAsync(manager, path, file.Blob.Blob.Container, file.Blob.Blob.FullFileName);
								else
									SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione un contenedor, directorio o archivo para descargar");
							}
							catch (Exception exception)
							{
								SolutionViewModel.MainController.Logger.LogError(exception, "Error al descargar los archivos");
							}
							// Log
							SolutionViewModel.MainController.Logger.LogInformation("Fin de la descarga de archivos");
						}
				}
			}
		}

		/// <summary>
		///		Descarga un contenedor o directorio
		/// </summary>
		private async Task DownloadFolderAsync(AzureStorageBlobManager manager, string localPath, string container, string folder)
		{
			List<LibBlobStorage.Metadata.BlobModel> blobs = await manager.ListBlobsAsync(container, folder);

				// Log
				SolutionViewModel.MainController.Logger.LogInformation($"Start download '{container}/{folder}' to '{localPath}' ({blobs.Count} archivos)");
				// Crea la carpeta
				LibHelper.Files.HelperFiles.MakePath(localPath);
				// Descarga los archivos
				foreach (LibBlobStorage.Metadata.BlobModel blob in blobs)
					if (blob.Length != 0)
					{
						string fileName = System.IO.Path.Combine(localPath, blob.LocalFileName);

							// Log
							SolutionViewModel.MainController.Logger.LogInformation($"Download '{blob.FullFileName}'");
							// Crea el directorio
							LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(fileName));
							// Descarga el archivo
							await manager.DownloadAsync(container, blob.FullFileName, fileName);
					}
				// Log
				SolutionViewModel.MainController.Logger.LogInformation($"End download '{container}/{folder}' to '{localPath}'");
		}

		/// <summary>
		///		Descarga un archivo del storage
		/// </summary>
		private async Task DownloadFileAsync(AzureStorageBlobManager manager, string localPath, string container, string fileName)
		{
			// Log
			SolutionViewModel.MainController.Logger.LogInformation($"Comienza la descarga del archivo {container}/{fileName}");
			// Descarga el archivo
			await manager.DownloadAsync(container, fileName, System.IO.Path.Combine(localPath, System.IO.Path.GetFileName(fileName)));
			// Log
			SolutionViewModel.MainController.Logger.LogInformation($"Finaliza la descarga del archivo {container}/{fileName}");
		}

		/// <summary>
		///		Borra el elemento seleccionado
		/// </summary>
		private async Task DeleteItemAsync()
		{	
			switch (SelectedNode)
			{
				case NodeStorageViewModel item:
						DeleteStorage(item);
					break;
				case NodeStorageContainerViewModel item:
						await DeleteContainerAsync(item);
					break;
				case NodeStorageContainerFileViewModel item:
						await DeleteBlobAsync(item);
					break;
			}
		}

		/// <summary>
		///		Borra los datos de storage
		/// </summary>
		private void DeleteStorage(NodeStorageViewModel item)
		{
			if (item != null &&
				SolutionViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Desea quitar la conexión al storage '{item.Text}' de la solución?"))
			{
				// Elimina el storage
				SolutionViewModel.Solution.Storages.Remove(item.Storage);
				// Graba la solución
				SolutionViewModel.SaveSolution();
				// Actualiza el árbol
				Load();
			}
		}

		/// <summary>
		///		Borra un contenedor de un storage
		/// </summary>
		private async Task DeleteContainerAsync(NodeStorageContainerViewModel item)
		{
			if (item != null &&
					SolutionViewModel.MainController.SystemController.ShowQuestion($"¿Desea eliminar el contenedor '{item.Text}'?"))
				try
				{
					AzureStorageBlobManager manager = GetStorageManager();

						// Borra el contenedor
						await manager.DeleteAsync(item.Container);
						// Actualiza el árbol
						Load();
				}
				catch (Exception exception)
				{
					SolutionViewModel.MainController.Logger.LogError(exception, $"Error al borrar el contenedor {item.Text}. {exception.Message}");
				}
		}

		/// <summary>
		///		Borra un blob
		/// </summary>
		private async Task DeleteBlobAsync(NodeStorageContainerFileViewModel item)
		{
			if (item != null &&
				SolutionViewModel.MainController.SystemController.ShowQuestion($"¿Desea eliminar el blob '{item.Text}'?"))
			{
				// Borra los elementos
				try
				{
					AzureStorageBlobManager manager = GetStorageManager();

						// Borra cada uno de los blobs del nodo
						foreach (BlobNodeModel blob in GetBlobs(item.Blob))
						{
							// Log
							SolutionViewModel.MainController.Logger.LogInformation($"Borrando {blob.Blob.FullFileName}");
							// Borra el blob
							await manager.DeleteAsync(blob.Blob.Container, blob.Blob.FullFileName);
						}
				}
				catch (Exception exception)
				{
					SolutionViewModel.MainController.Logger.LogError(exception, $"Error al borrar el blob. {exception.Message}");
				}
				// Log
				SolutionViewModel.MainController.Logger.LogInformation("Fin del borrado de blobs");
				// Actualiza el árbol
				Load();
			}
		}

		/// <summary>
		///		Obtiene los hijos de un blob
		/// </summary>
		private List<BlobNodeModel> GetBlobs(BlobNodeModel node)
		{
			List<BlobNodeModel> blobs = new List<BlobNodeModel>();

				// Obtiene los nodos hijo (si hay alguno)
				if (node.Children.Count == 0)
					blobs.Add(node);
				else
					foreach (BlobNodeModel child in node.Children)
						blobs.AddRange(GetBlobs(child));
				// Devuelve los nodos
				return blobs;
		}

		/// <summary>
		///		Obtiene el manager del storage
		/// </summary>
		private AzureStorageBlobManager GetStorageManager()
		{
			Models.Cloud.StorageModel storage = GetSelectedStorage();

				if (storage != null)
					return new AzureStorageBlobManager(storage.StorageConnectionString);
				else
					return null;
		}

		/// <summary>
		///		Obtiene el storage seleccionado en el árbol
		/// </summary>
		private Models.Cloud.StorageModel GetSelectedStorage()
		{
			switch (SelectedNode)
			{
				case null:
					return null;
				case NodeStorageViewModel item:
					return item.Storage;
				case NodeStorageContainerViewModel item:
					return item.Storage;
				case NodeStorageContainerFileViewModel item:
					return item.Storage;
				default:
					return null;
			}
		}

		/// <summary>
		///		Obtiene los elementos seleccionados en el storage
		/// </summary>
		private (string container, bool isFolder, string fileName) GetStorageItems()
		{
			switch (SelectedNode)
			{
				case NodeStorageContainerViewModel item:
					return (item.Container, false, string.Empty);
				case NodeStorageContainerFileViewModel item:
					return (item.Blob.Blob.Container, item.IsFolder, System.IO.Path.GetDirectoryName(item.Blob.Blob.FullFileName));
				default:
					return (string.Empty, false, string.Empty);
			}
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public CloudStudioViewModel SolutionViewModel { get; }

		/// <summary>
		///		Comando para crear una nueva conexión a storage
		/// </summary>
		public BaseCommand NewStorageCommand { get; }

		/// <summary>
		///		Comando para crear un nuevo contenedor sobre un storage
		/// </summary>
		public BaseCommand NewContainerCommand { get; }

		/// <summary>
		///		Comando para borrar un elemento (asíncrono)
		/// </summary>
		public BaseCommand DeleteAsyncCommand { get; }

		/// <summary>
		///		Comando para subir archivos (asíncrono)
		/// </summary>
		public BaseCommand UploadFileAsyncCommand { get; }

		/// <summary>
		///		Comando para subir directorios (asíncrono)
		/// </summary>
		public BaseCommand UploadFolderAsyncCommand { get; }

		/// <summary>
		///		Comando para descargar archivos (asíncrono)
		/// </summary>
		public BaseCommand DownloadAsyncCommand { get; }
	}
}