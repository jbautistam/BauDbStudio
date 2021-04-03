using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.DbStudio.Models.Cloud;
using Bau.Libraries.LibBlobStorage;
using Bau.Libraries.DbStudio.ViewModels.Core.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers.Cloud
{
	/// <summary>
	///		ViewModel de un nodo de storage
	/// </summary>
	public class NodeStorageViewModel : BaseTreeNodeAsyncViewModel
	{
		public NodeStorageViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, StorageModel storage) 
					: base(trvTree, parent, storage.Name, NodeType.Storage, IconType.Storage, storage, true, true, MvvmColor.Navy)
		{
			Storage = storage;
		}

		/// <summary>
		///		Carga los contenedores
		/// </summary>
		protected override async Task<List<BaseTreeNodeViewModel>> GetChildNodesAsync(CancellationToken cancellationToken)
		{
			List<BaseTreeNodeViewModel> nodes = new List<BaseTreeNodeViewModel>();

				// Carga los contenedores
				try
				{
					ICloudStorageManager manager = new StorageManager().OpenAzureStorageBlob(Storage.GetNormalizedConnectionString());
					List<string> containers = await manager.ListContainersAsync(string.Empty);

						// Carga los contenedores
						foreach (string container in containers)
							nodes.Add(new NodeStorageContainerViewModel(TreeViewModel, this, Storage, container));
				}
				catch (Exception exception)
				{
					nodes.Add(new NodeMessageViewModel(TreeViewModel, this, $"Error al cargar los contenedores. {exception.Message}", IconType.Error));
					(TreeViewModel as TreeStorageViewModel).SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error($"Error al cargar los contenedores. {exception.Message}");
				}
				// Devuelve la colección de nodos
				return nodes;
		}

		/// <summary>
		///		Storage
		/// </summary>
		public StorageModel Storage { get; }
	}
}
