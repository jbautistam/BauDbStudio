using System;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files
{
	/// <summary>
	///		ViewModel para un archivo de imagen
	/// </summary>
	public class ImageViewModel : BaseObservableObject, Core.Interfaces.IDetailViewModel
	{
		// Variables privadas
		private string _header, _fileName;

		public ImageViewModel(SolutionViewModel solutionViewModel, string fileName) : base(false)
		{
			SolutionViewModel = solutionViewModel;
			FileName = fileName;
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		public void SaveDetails(bool newName)
		{
			//// Graba el archivo
			//if (string.IsNullOrWhiteSpace(FileName) || newName)
			//{
			//	string newFileName = SolutionViewModel.MainViewModel.OpenDialogSave(FileName, "Script SQL (*.sql)|*.sql|Todos los archivos (*.*)|*.*", ".sql");

			//		// Cambia el nombre de archivo si es necesario
			//		if (!string.IsNullOrWhiteSpace(newFileName))
			//			FileName = newFileName;
			//}
			//// Graba el archivo
			//if (!string.IsNullOrWhiteSpace(FileName))
			//{
			//	// Graba el archivo
			//	LibHelper.Files.HelperFiles.SaveTextFile(FileName, Content, System.Text.Encoding.UTF8);
			//	// Actualiza el árbol
			//	SolutionViewModel.TreeFoldersViewModel.Load();
			//	// Añade el archivo a los últimos archivos abiertos
			//	SolutionViewModel.MainViewModel.LastFilesViewModel.Add(FileName);
				// Indica que no ha habido modificaciones
				IsUpdated = false;
			//}
		}

		/// <summary>
		///		Obtiene el mensaje que se debe mostrar al cerrar la ventana
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			if (string.IsNullOrWhiteSpace(FileName))
				return "¿Desea grabar el archivo antes de continuar?";
			else
				return $"¿Desea grabar el archivo '{System.IO.Path.GetFileName(FileName)}' antes de continuar?";
		}

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Id de la ficha
		/// </summary>
		public string TabId 
		{ 
			get { return GetType().ToString() + "_" + FileName; } 
		}

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header
		{
			get { return _header; }
			set { CheckProperty(ref _header, value); }
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set
			{ 
				if (CheckProperty(ref _fileName, value))
				{
					if (!string.IsNullOrWhiteSpace(value))
						Header = System.IO.Path.GetFileName(value);
					else
						Header = "Archivo";
				}
			}
		}
	}
}
