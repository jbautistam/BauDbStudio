using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibBlobStorage;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.DbStudio.ViewModels.Core.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers.Cloud
{
	/// <summary>
	///		ViewModel de un árbol de storage
	/// </summary>
	public class TreeStorageViewModel : BaseTreeViewModel
	{	
		public TreeStorageViewModel(SolutionViewModel solutionViewModel)
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
			BaseTreeNodeViewModel.NodeType type = GetSelectedNodeType();

				// Comprueba la acción
				switch (action)
				{
					case nameof(OpenCommand):
					case nameof(NewContainerCommand):
						return type == BaseTreeNodeViewModel.NodeType.Storage;
					case nameof(DeleteCommand):
						return true;
					case nameof(UploadFileAsyncCommand):
					case nameof(UploadFolderAsyncCommand):
					case nameof(DownloadAsyncCommand):
						return type == BaseTreeNodeViewModel.NodeType.StorageContainer || type == BaseTreeNodeViewModel.NodeType.File;
					default:
						return false;
				}
		}

		/// <summary>
		///		Abre la ventana de propiedades de un nodo
		/// </summary>
		protected override void OpenProperties()
		{
			switch (GetSelectedNodeType())
			{
				case BaseTreeNodeViewModel.NodeType.Storage:
						if (SelectedNode is NodeStorageViewModel node)
							OpenStorage(node);
					break;
			}
		}

		/// <summary>
		///		Abre una ventana de propiedades de un storage
		/// </summary>
		private void OpenStorage(NodeStorageViewModel node)
		{
			if (SolutionViewModel.MainViewModel.MainController.OpenDialog(new Details.Cloud.StorageViewModel(SolutionViewModel, node?.Storage)) == 
							BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
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

				if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController
							.ShowInputString("Introduzca el nombre del contenedor", ref container) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes &&
					!string.IsNullOrWhiteSpace(container))
				{
					ICloudStorageManager manager = GetStorageManager();

						if (manager == null)
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un sistema de almacenamiento en el árbol");
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
								SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error($"Error al crear el contenedor {container}", exception);
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
			if (SolutionViewModel.MainViewModel.MainController.HostController.DialogsController.OpenDialogSelectPath(SolutionViewModel.MainViewModel.LastPathSelected, out string path) 
					== BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				if (string.IsNullOrWhiteSpace(path) || !System.IO.Directory.Exists(path))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un directorio");
				else
				{
					List<string> files = Convert(System.IO.Directory.GetFiles(path));

						if (files.Count > 0)
							await UploadFilesAsync(files);
						else
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No hay ningún archivo en es directorio");
				}
			}
		}

		/// <summary>
		///		Sube archivos
		/// </summary>
		private async Task UploadFileAsync()
		{
			List<string> files = Convert(SolutionViewModel.MainViewModel.MainController.HostController.DialogsController.OpenDialogLoadFiles(SolutionViewModel.MainViewModel.LastPathSelected, string.Empty));

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
			ICloudStorageManager manager = GetStorageManager();

				// Sube los archivos
				if (manager == null)
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una cuenta de almacenamiento");
				else
				{
					(string container, bool isFolder, string remoteFileName) = GetStorageItems();

						if (string.IsNullOrWhiteSpace(container))
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un contenedor");
						else
						{
							// Sube los archivos
							using (BlockLogModel block = SolutionViewModel.MainViewModel.MainController.Logger.Default.CreateBlock(LogModel.LogType.Info, "Comienzo de la subida de archivos"))
							{
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
											block.Info($"Comienza la subida del archivo {file}");
											// Sube el archivo
											await manager.UploadAsync(container, targetFile, file);
											// Log
											block.Info($"Finaliza la subida del archivo {file}");
									}
								// Log
								block.Info("Fin de la subida de archivos");
							}
							// Actualiza el árbol
							Load();
							// Log 
							SolutionViewModel.MainViewModel.MainController.Logger.Flush();
						}
				}
		}

		/// <summary>
		///		Descarga archivos
		/// </summary>
		private async Task DownloadAsync()
		{
			if (SolutionViewModel.MainViewModel.MainController.HostController.DialogsController.OpenDialogSelectPath(SolutionViewModel.MainViewModel.LastPathSelected, out string path) 
					== BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				if (string.IsNullOrWhiteSpace(path) || !System.IO.Directory.Exists(path))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un directorio");
				else
				{
					ICloudStorageManager manager = GetStorageManager();

						// Descarga el contenedor, archivo o directorio
						if (manager == null)
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una cuenta de almacenamiento");
						else 
						{
							// Descarga de archivos
							using (BlockLogModel block = SolutionViewModel.MainViewModel.MainController.Logger.Default.CreateBlock(LogModel.LogType.Info, "Comienza la descarga de archivos"))
							{
								// Descarga el contenedor, directorio o archivo
								try
								{
									if (SelectedNode is NodeStorageContainerViewModel item)
										await DownloadFolderAsync(block, manager, path, item.Container, string.Empty);
									else if (SelectedNode is NodeStorageContainerFileViewModel folder && folder.IsFolder)
										await DownloadFolderAsync(block, manager, path, folder.Blob.Blob.Container, System.IO.Path.GetDirectoryName(folder.Blob.Blob.FullFileName));
									else if (SelectedNode is NodeStorageContainerFileViewModel file && !file.IsFolder)
										await DownloadFileAsync(block, manager, path, file.Blob.Blob.Container, file.Blob.Blob.FullFileName);
									else
										SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un contenedor, directorio o archivo para descargar");
								}
								catch (Exception exception)
								{
									SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error("Error al descargar los archivos", exception);
								}
								// Log
								block.Info("Fin de la descarga de archivos");
							}
							// Log 
							SolutionViewModel.MainViewModel.MainController.Logger.Flush();
							// Guarda el directorio seleccionado
							SolutionViewModel.MainViewModel.LastPathSelected = path;
						}
				}
			}
		}

		/// <summary>
		///		Descarga un contenedor o directorio
		/// </summary>
		private async Task DownloadFolderAsync(BlockLogModel block, ICloudStorageManager manager, string localPath, string container, string folder)
		{
			List<LibBlobStorage.Metadata.BlobModel> blobs = await manager.ListBlobsAsync(container, folder);

				// Log
				block.Info($"Start download '{container}/{folder}' to '{localPath}' ({blobs.Count} archivos)");
				// Crea la carpeta
				LibHelper.Files.HelperFiles.MakePath(localPath);
				// Descarga los archivos
				foreach (LibBlobStorage.Metadata.BlobModel blob in blobs)
					if (blob.Length != 0)
					{
						string fileName = System.IO.Path.Combine(localPath, blob.LocalFileName);

							// Log
							block.Info($"Download '{blob.FullFileName}'");
							// Crea el directorio
							LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(fileName));
							// Descarga el archivo
							await manager.DownloadAsync(container, blob.FullFileName, fileName);
					}
				// Log
				block.Info($"End download '{container}/{folder}' to '{localPath}'");
		}

		/// <summary>
		///		Descarga un archivo del storage
		/// </summary>
		private async Task DownloadFileAsync(BlockLogModel block, ICloudStorageManager manager, string localPath, string container, string fileName)
		{
			// Log
			block.Info($"Comienza la descarga del archivo {container}/{fileName}");
			// Descarga el archivo
			await manager.DownloadAsync(container, fileName, System.IO.Path.Combine(localPath, System.IO.Path.GetFileName(fileName)));
			// Log
			block.Info($"Finaliza la descarga del archivo {container}/{fileName}");
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
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Desea quitar la conexión al storage '{item.Text}' de la solución?"))
			{
				// Elimina el storage
				SolutionViewModel.Solution.Storages.Remove(item.Storage);
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
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
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Desea eliminar el contenedor '{item.Text}'?"))
				try
				{
					ICloudStorageManager manager = new StorageManager().OpenAzureStorageBlob(item.Storage.GetNormalizedConnectionString());

						// Borra el contenedor
						await manager.DeleteAsync(item.Container);
						// Actualiza el árbol
						Load();
				}
				catch (Exception exception)
				{
					SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error($"Error al borrar el contenedor {item.Text}. {exception.Message}");
				}
		}

		/// <summary>
		///		Borra un blob
		/// </summary>
		private async Task DeleteBlobAsync(NodeStorageContainerFileViewModel item)
		{
			if (item != null &&
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Desea eliminar el blob '{item.Text}'?"))
				using (BlockLogModel block = SolutionViewModel.MainViewModel.MainController.Logger.Default.CreateBlock(LogModel.LogType.Info, "Comienza el borrado de blobs"))
				{
						// Borra los elementos
						try
						{
							ICloudStorageManager manager = new StorageManager().OpenAzureStorageBlob(item.Storage.GetNormalizedConnectionString());

								// Borra cada uno de los blobs del nodo
								foreach (BlobNodeModel blob in GetBlobs(item.Blob))
								{
									// Log
									block.Info($"Borrando {blob.Blob.FullFileName}");
									// Borra el blob
									await manager.DeleteAsync(blob.Blob.Container, blob.Blob.FullFileName);
								}
						}
						catch (Exception exception)
						{
							SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error($"Error al borrar el blob. {exception.Message}");
						}
						// Log
						block.Info("Fin del borrado de blobs");
						SolutionViewModel.MainViewModel.MainController.Logger.Flush();
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
		private ICloudStorageManager GetStorageManager()
		{
			Models.Cloud.StorageModel storage = GetSelectedStorage();

				if (storage != null)
					return new StorageManager().OpenAzureStorageBlob(storage.GetNormalizedConnectionString());
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
		public SolutionViewModel SolutionViewModel { get; }

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
