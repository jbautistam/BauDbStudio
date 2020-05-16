using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.DbStudio.ViewModels.Tools
{
	/// <summary>
	///		Lista de últimos archivos abiertos
	/// </summary>
	public class LastFilesListViewModel : BauMvvm.ViewModels.BaseObservableObject
	{
		// Variables privadas
		private ObservableCollection<LastFileViewModel> _files;

		public LastFilesListViewModel(MainViewModel mainViewModel)
		{
			MainViewModel = mainViewModel;
			Files = new ObservableCollection<LastFileViewModel>();
		}

		/// <summary>
		///		Añade un archivo
		/// </summary>
		public void Add(string fileName)
		{
			if (!string.IsNullOrWhiteSpace(fileName))
			{
				List<string> temporalFiles = SelectFiles();
				string [] parts = fileName.Split(';');

					// Añade los nombres de archivos al inicio
					foreach (string part in parts)
						if (!string.IsNullOrWhiteSpace(part))
							if (!Exists(part, temporalFiles))
								temporalFiles.Insert(0, part.TrimIgnoreNull());
					// Limpia la colección de archivos
					Files.Clear();
					// Añade los archivos
					foreach (string file in temporalFiles)
						if (temporalFiles.IndexOf(file) < 10 && System.IO.File.Exists(file))
							Files.Add(new LastFileViewModel(MainViewModel, file, temporalFiles.IndexOf(file) + 1));
			}
		}

		/// <summary>
		///		Obtiene una lista de archivos seleccionados
		/// </summary>
		private List<string> SelectFiles()
		{
			List<string> temporalFiles = new List<string>();

				// Obtiene los archivos de la colección
				foreach (LastFileViewModel fileViewModel in Files)
					temporalFiles.Add(fileViewModel.FileName);
				// Devuelve la lista de archivos
				return temporalFiles;
		}

		/// <summary>
		///		Compureba si existe un archivo en la lista
		/// </summary>
		private bool Exists(string file, List<string> files)
		{
			// Busca el archivo en la colección
			foreach (string fileName in files)
				if (fileName.Equals(file, StringComparison.CurrentCultureIgnoreCase))
					return true;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return false;
		}

		/// <summary>
		///		Obtiene los archivos
		/// </summary>
		public string GetFiles()
		{
			string files = string.Empty;

				// Añade los archivos
				foreach (LastFileViewModel file in Files)
					files = files.AddWithSeparator(file.FileName, ";");
				// Devuelve la lista de archivos
				return files;
		}

		/// <summary>
		///		ViewModel de la ventana principal
		/// </summary>
		public MainViewModel MainViewModel { get; }

		/// <summary>
		///		Archivos
		/// </summary>
		public ObservableCollection<LastFileViewModel> Files
		{
			get { return _files; }
			set { CheckObject(ref _files, value); }
		}
	}
}
