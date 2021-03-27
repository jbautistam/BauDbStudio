using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.DbStudio.ViewModels.Core.Tools.Search
{
	/// <summary>
	///		ViewModel base de un nodo del árbol de resultados de búsqueda
	/// </summary>
	public class TreeResultNodeViewModel : ControlHierarchicalViewModel
	{	
		// Variables privadas
		private string _fileName, _textFound;
		private int _line;

		public TreeResultNodeViewModel(TreeSearchFilesResultViewModel trvTree, IHierarchicalViewModel parent, 
									   string text, string fileName, int line, string textFound, bool isBold, MvvmColor foreground) 
						: base(parent, text, null, false, isBold, foreground)
		{ 
			TreeViewModel = trvTree;
			if (!string.IsNullOrWhiteSpace(fileName) && line < 1)
				Text = System.IO.Path.GetFileName(fileName);
			FileName = fileName;
			Line = line;
			TextFound = textFound;
		}

		/// <summary>
		///		ViewModel del árbol
		/// </summary>
		public TreeSearchFilesResultViewModel TreeViewModel { get; }

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { CheckProperty(ref _fileName, value); }
		}

		/// <summary>
		///		Linea en la que se ha encontrado el texto
		/// </summary>
		public int Line
		{
			get { return _line; }
			set { CheckProperty(ref _line, value); }
		}

		/// <summary>
		///		Texto encontrado en la búsqueda
		/// </summary>
		public string TextFound
		{
			get { return _textFound; }
			set { CheckProperty(ref _textFound, value); }
		}
	}
}