using System;

using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.ChessDataBase.ViewModels.Configuration
{
	/// <summary>
	///		ViewModel de la configuración
	/// </summary>
	public class ConfigurationViewModel : BauMvvm.ViewModels.BaseObservableObject
	{
		// Constantes privadas
		private const string AppName = "ChessDataBase";
		// Variables privadas
		private string _pathBoardImages, _pathPiecesImages;
		private bool _showAnimations;

		public ConfigurationViewModel(MainViewModel mainViewModel)
		{
			MainViewModel = mainViewModel;
		}

		/// <summary>
		///		Carga la configuración
		/// </summary>
		internal void Load()
		{
			PathBoardImages = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(AppName, nameof(PathBoardImages));
			PathPiecesImages = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(AppName, nameof(PathPiecesImages));
			ShowAnimations = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(AppName, nameof(ShowAnimations)).GetBool();
		}

		/// <summary>
		///		Comprueba los datos introducidos en el formulario
		/// </summary>
		public bool ValidateData(out string error)
		{
			error = string.Empty;
			return true;
		}

		/// <summary>
		///		Graba la configuración
		/// </summary>
		public void Save()
		{
			MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(AppName, nameof(PathBoardImages), PathBoardImages);
			MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(AppName, nameof(PathPiecesImages), PathPiecesImages);
			MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(AppName, nameof(ShowAnimations), ShowAnimations.ToString());
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public MainViewModel MainViewModel { get; }

		/// <summary>
		///		Directorio donde se encuentran las imágenes del tablero
		/// </summary>
		public string PathBoardImages 
		{ 
			get { return _pathBoardImages; }
			set { CheckProperty(ref _pathBoardImages, value); }
		}

		/// <summary>
		///		Directorio donde se encuentran las imágenes de las piezas
		/// </summary>
		public string PathPiecesImages
		{ 
			get { return _pathPiecesImages; }
			set { CheckProperty(ref _pathPiecesImages, value); }
		}

		/// <summary>
		///		Indica si se deben mostrar animaciones
		/// </summary>
		public bool ShowAnimations
		{ 
			get { return _showAnimations; }
			set { CheckProperty(ref _showAnimations, value); }
		}
	}
}
