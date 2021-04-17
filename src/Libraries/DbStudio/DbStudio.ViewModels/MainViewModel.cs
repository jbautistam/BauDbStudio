using System;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.DbStudio.ViewModels
{
	/// <summary>
	///		ViewModel principal
	/// </summary>
	public class MainViewModel : BaseObservableObject
	{
		// Constantes privadas
		internal const string MaskFiles = "Archivos de solución (*.dbsln)|*.dbsln|Todos los archivos (*.*)|*.*";
		// Variables privadas
		private string _text;

		public MainViewModel(string appName, Controllers.IDbStudioController mainController)
		{
			// Título de la aplicación
			Text = appName;
			// Asigna las propiedades
			MainController = mainController;
			// Inicializa la solución
			SolutionViewModel = new Solutions.SolutionViewModel(this);
		}

		/// <summary>
		///		Carga los datos
		/// </summary>
		public void Load(string path)
		{
			// Guarda el directorio
			PathData = path;
			// Carga la solución
			SolutionViewModel.Load(path);
		}

		/// <summary>
		///		Graba la solución
		/// </summary>
		internal void SaveSolution()
		{
			SolutionViewModel.Save(PathData);
		}

		/// <summary>
		///		Actualiza el árbol
		/// </summary>
		internal void Refresh()
		{
			Load(PathData);
		}

		/// <summary>
		///		Solicita al usuario un nombre de archivos. Guarda el directorio seleccionado para que la próxima vez que se consulte
		///	por un archivo, se vaya directamente a ese directorio
		/// </summary>
		internal string OpenDialogSave(string suggestedFileName, string mask, string defaultExtension)
		{
			string path = GetPath(suggestedFileName);

				// Obtiene el nombre de archivo
				return MainController.HostController.DialogsController.OpenDialogSave(path, mask, suggestedFileName, defaultExtension);
		}

		/// <summary>
		///		Obtiene el directorio inicial de grabación de un archivo
		/// </summary>
		private string GetPath(string suggestedFileName)
		{
			string path = string.Empty;

				// Obtiene el directorio
				if (!string.IsNullOrWhiteSpace(suggestedFileName))
				{
					path = System.IO.Path.GetDirectoryName(suggestedFileName);
					if (string.IsNullOrWhiteSpace(path) || path.Equals(suggestedFileName, StringComparison.CurrentCultureIgnoreCase))
						path = MainController.MainWindowController.HostController.DialogsController.LastPathSelected;
				}
				// Si no se ha definido ningún directorio, se coge el de la solución
				if (string.IsNullOrWhiteSpace(path))
					path = SolutionViewModel.Solution.Path;
				// Devuelve el directorio
				return path;
		}

		/// <summary>
		///		Controlador principal
		/// </summary>
		public Controllers.IDbStudioController MainController { get; }

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public Solutions.SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Directorio de datos
		/// </summary>
		public string PathData { get; private set; }

		/// <summary>
		///		Título de la ventana
		/// </summary>
		public string Text 
		{
			get { return _text; }
			set { CheckProperty(ref _text, value); }
		}
	}
}
