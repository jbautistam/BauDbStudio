using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibBlobStorage;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers.Cloud
{
	/// <summary>
	///		ViewModel de un árbol de storage
	/// </summary>
	public class TreeStorageViewModel : BaseTreeViewModel
	{	
		public TreeStorageViewModel(SolutionViewModel solutionViewModel) : base(solutionViewModel)
		{ 
			NewStorageCommand = new BaseCommand(_ => OpenStorage(null));
			DeleteAsyncCommand = new BaseCommand(async _ => await DeleteItemAsync(), _ => SelectedNode != null)
										.AddListener(this, nameof(SelectedNode));
			UploadAsyncCommand = new BaseCommand(async _ => await UploadAsync(), _ => CanExecuteAction(nameof(UploadAsyncCommand)))
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
					case nameof(OpenPropertiesCommand):
						return type == BaseTreeNodeViewModel.NodeType.Storage;
					case nameof(DeleteCommand):
						return true;
					case nameof(UploadAsyncCommand):
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
		///		Borra el elemento seleccionado
		/// </summary>
		protected override void DeleteItem()
		{	
			// ... no hace nada, en este caso va por DeleteItemAsync
		}

		/// <summary>
		///		Sube archivos
		/// </summary>
		private async Task UploadAsync()
		{
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
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage($"Descargar en el directorio {path}");
			}
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
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Desea elimina el blob '{item.Text}'?"))
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
		///		Comando para crear una nueva conexión a storage
		/// </summary>
		public BaseCommand NewStorageCommand { get; }

		/// <summary>
		///		Comando para borrar un elemento (asíncrono)
		/// </summary>
		public BaseCommand DeleteAsyncCommand { get; }

		/// <summary>
		///		Comando para subir archivos (asíncrono)
		/// </summary>
		public BaseCommand UploadAsyncCommand { get; }

		/// <summary>
		///		Comando para descargar archivos (asíncrono)
		/// </summary>
		public BaseCommand DownloadAsyncCommand { get; }
	}
}
