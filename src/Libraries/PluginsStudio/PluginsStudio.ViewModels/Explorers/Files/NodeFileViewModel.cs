using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files
{
	/// <summary>
	///		ViewModel de un nodo de archivo
	/// </summary>
	public class NodeFileViewModel : BaseTreeNodeViewModel
	{
		// Variables privadas
		private string _fileName;
		private bool _isFolder;

		public NodeFileViewModel(TreeFilesViewModel trvTree, IHierarchicalViewModel parent, string fileName, bool isFolder) 
					: base(trvTree, parent, string.Empty, TreeFilesViewModel.NodeType.File.ToString(), 
						   (isFolder ? TreeFilesViewModel.IconType.Path : TreeFilesViewModel.IconType.File).ToString(), 
						   fileName, isFolder, isFolder,
						   isFolder ? MvvmColor.Navy : MvvmColor.Black)
		{
			FileName = fileName;
			IsFolder = isFolder;
			if (!string.IsNullOrWhiteSpace(FileName))
			{
				Text = System.IO.Path.GetFileName(FileName);
				ToolTipText = FileName;
			}
			else
				Text = "...";
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void LoadNodes()
		{
			if (!string.IsNullOrWhiteSpace(FileName) && System.IO.Directory.Exists(FileName))
			{
				// Carga los directorios
				foreach (string fileName in System.IO.Directory.EnumerateDirectories(FileName))
					AddNode(fileName, true);
				// Carga los archivos
				foreach (string fileName in System.IO.Directory.EnumerateFiles(FileName))
					AddNode(fileName, false);
			}
		}

		/// <summary>
		///		Obtiene el texto que se debe lanzar al editor
		/// </summary>
		public override string GetTextForEditor(bool shiftPressed)
		{
			return FileName;
		}

		/// <summary>
		///		Añade un nodo
		/// </summary>
		private void AddNode(string fileName, bool isFolder)
		{
			Children.Add(new NodeFileViewModel(TreeViewModel as TreeFilesViewModel, this, fileName, isFolder));
		}

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

		/// <summary>
		///		Tipo de nodo
		/// </summary>
		public TreeFilesViewModel.NodeType NodeType
		{
			get { return Type.GetEnum(TreeFilesViewModel.NodeType.Unknown); }
		}

		/// <summary>
		///		Tipo de icono
		/// </summary>
		public TreeFilesViewModel.IconType IconType
		{
			get { return Icon.GetEnum(TreeFilesViewModel.IconType.Unknown); }
		}
	}
}