using System;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.ChessDataBase.ViewModels
{
	/// <summary>
	///		ViewModel principal del lector de archivos PGN
	/// </summary>
	public class MainViewModel : BaseObservableObject
	{
		public MainViewModel(Controllers.IChessDataBaseController mainController)
		{
			// Asigna el controlador de vistas
			ViewsController = mainController;
			// Asigna los objetos
			ConfigurationViewModel = new Configuration.ConfigurationViewModel(this);
		}

		/// <summary>
		///		Inicializa el viewModel
		/// </summary>
		public void Initialize()
		{
			ConfigurationViewModel.Load();
		}

		/// <summary>
		///		Abre la ventana de un archivo
		/// </summary>
		public bool OpenFile(string fileName)
		{
			if (!string.IsNullOrWhiteSpace(fileName) && fileName.EndsWith(".pgn") && System.IO.File.Exists(fileName))
			{
				// Abre la ventana
				ViewsController.OpenWindow(new Games.GameBoardPgnViewModel(this, fileName));
				// Indica que se ha abierto correctamente
				return true;
			}
			else
				return false;
		}

		/// <summary>
		///		Controlador de vistas de aplicación
		/// </summary>
		public Controllers.IChessDataBaseController ViewsController { get; }

		/// <summary>
		///		ViewModel de configuración
		/// </summary>
		public Configuration.ConfigurationViewModel ConfigurationViewModel { get; }
	}
}
