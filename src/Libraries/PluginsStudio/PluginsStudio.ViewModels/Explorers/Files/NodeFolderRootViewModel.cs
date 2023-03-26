using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files
{
	/// <summary>
	///		ViewModel de un nodo de carpeta de la solución
	/// </summary>
	public class NodeFolderRootViewModel : BaseTreeNodeViewModel
	{
		// Variables privadas
		private string _fileName;

		public NodeFolderRootViewModel(TreeFilesViewModel trvTree, IHierarchicalViewModel parent, string path) 
					: base(trvTree, parent, path, TreeFilesViewModel.NodeType.FilesRoot.ToString(), string.Empty, path, true, true, MvvmColor.Red)
		{
			FileName = path;
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void LoadNodes()
		{
			string path = Tag?.ToString();

				if (!string.IsNullOrWhiteSpace(path) && System.IO.Directory.Exists(path))
				{
					// Carga los directorios
					foreach (string fileName in System.IO.Directory.EnumerateDirectories(path))
						AddNode(fileName, true);
					// Carga los archivos
					foreach (string fileName in System.IO.Directory.EnumerateFiles(path))
						AddNode(fileName, false);
				}
		}

		/// <summary>
		///		Añade un nodo
		/// </summary>
		private void AddNode(string fileName, bool isFolder)
		{
			Children.Add(new NodeFileViewModel(TreeViewModel as TreeFilesViewModel, this, fileName, isFolder));
		}

		/// <summary>
		///		Directorio
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set 
			{ 
				if (CheckProperty(ref _fileName, value))
				{
					if (!string.IsNullOrWhiteSpace(value))
					{
						Text = System.IO.Path.GetFileName(value);
						ToolTipText = value;
					}
					else
					{
						Text = "...";
						ToolTipText = string.Empty;
					}
				}
			}
		}
	}
}
