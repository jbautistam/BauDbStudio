using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.DbStudio.Models.Cloud;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Cloud
{
	/// <summary>
	///		ViewModel de un nodo de archivo de un contenedor
	/// </summary>
	public class NodeStorageContainerFileViewModel : BaseTreeNodeViewModel
	{
		// Variables privadas
		private string _fileName;
		private bool _isFolder;

		public NodeStorageContainerFileViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, 
												 StorageModel storage, BlobNodeModel blob, bool isFolder)
					: base(trvTree, parent, blob.Name, NodeType.File, isFolder ? IconType.Path : IconType.File, 
						   blob, false, isFolder,
						   isFolder ? MvvmColor.Navy : MvvmColor.Black)
		{
			IsFolder = isFolder;
			Storage = storage;
			Blob = blob;
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void LoadNodes()
		{
			// No hace nada, simplemente implementa la interface
		}

		/// <summary>
		///		Storage asociado al nodo
		/// </summary>
		public StorageModel Storage { get; }

		/// <summary>
		///		Blob asociado al nodo
		/// </summary>
		public BlobNodeModel Blob { get; }

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { CheckProperty(ref _fileName, value); }
		}

		/// <summary>
		///		Indica si se trata de una carpeta
		/// </summary>
		public bool IsFolder
		{
			get { return _isFolder; }
			set { CheckProperty(ref _isFolder, value); }
		}
	}
}
