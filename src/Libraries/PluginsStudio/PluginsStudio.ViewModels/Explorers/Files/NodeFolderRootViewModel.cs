﻿using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;

/// <summary>
///		ViewModel de un nodo de carpeta de la solución
/// </summary>
public class NodeFolderRootViewModel : PluginNodeViewModel
{
	// Variables privadas
	private string _fileName = string.Empty;

	public NodeFolderRootViewModel(TreeFilesViewModel trvTree, ControlHierarchicalViewModel? parent, string path) 
				: base(trvTree, parent, path, TreeFilesViewModel.NodeType.FilesRoot.ToString(), string.Empty, path, true, true, MvvmColor.Red)
	{
		ViewModel = trvTree;
		FileName = path;
	}

	/// <summary>
	///		Carga los nodos hijo
	/// </summary>
	protected override void LoadNodes()
	{
		string path = Tag?.ToString() ?? string.Empty;

			// Limpia los nodos
			Children.Clear();
			// Carga losnodos del directorio
			if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
				Children.AddRange(new HelperFileNodes(ViewModel, this).GetChildNodes(path));
	}

	/// <summary>
	///		Comprueba si este nodo es igual a otro
	/// </summary>
	public override bool IsEquals(ControlHierarchicalViewModel node)
	{
		if (node is NodeFolderRootViewModel target)
			return FileName.Equals(target.FileName, StringComparison.CurrentCultureIgnoreCase);
		else
			return base.IsEquals(node);
	}

	/// <summary>
	///		Directorio / nombre de archivo
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
					// Asigna el texto y el toolTip
					Text = Path.GetFileName(value);
					ToolTipText = value;
					// Si está vacío, guarda el valor entero (si el nombre de archivo es C:\, Path.GetFileName devuelve un valor vacío)
					if (string.IsNullOrWhiteSpace(Text))
						Text = value;
				}
				else
				{
					Text = "...";
					ToolTipText = string.Empty;
				}
			}
		}
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public TreeFilesViewModel ViewModel { get; }
}
