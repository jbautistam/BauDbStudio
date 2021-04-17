using System;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.Workspaces
{
	/// <summary>
	///		ViewModel con los datos de un espacio de trabajo
	/// </summary>
	public class WorkSpaceViewModel : BauMvvm.ViewModels.BaseObservableObject
	{
		// Variables privadas
		private string _name, _fileName;

		public WorkSpaceViewModel(WorkspaceListViewModel listViewModel, string name, string fileName)
		{
			ListViewModel = listViewModel;
			Name = name;
			FileName = fileName;
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public WorkspaceListViewModel ListViewModel { get; }

		/// <summary>
		///		Nombre del espacio de trabajo
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { CheckProperty(ref _name, value); }
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
		///		Directorio del archivo
		/// </summary>
		public string Path
		{
			get { return System.IO.Path.GetDirectoryName(FileName); }
		}
	}
}
