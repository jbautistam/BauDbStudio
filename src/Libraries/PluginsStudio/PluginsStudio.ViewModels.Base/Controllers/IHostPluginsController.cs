using System;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers
{
	/// <summary>
	///		Controlador del host de plugins
	/// </summary>
	public interface IHostPluginsController
	{
		/// <summary>
		///		Añade un archivo utilizado a la lista de archivos
		/// </summary>
		void AddFileUsed(string fileName);

		/// <summary>
		///		Abre un archivo
		/// </summary>
		void OpenFile(string fileName);

		/// <summary>
		///		Abre la ventana de edición de un archivo
		/// </summary>
		void OpenEditor(Base.Files.BaseTextFileViewModel viewModel);

		/// <summary>
		///		Actualiza el árbol de archivos
		/// </summary>
		void RefreshFiles();

		/// <summary>
		///		Ejecuta un comando sobre un plugin
		/// </summary>
		void ExecutePluginCommand(string plugin, string viewModel, string command);

		/// <summary>
		///		Comprueba si se puede ejecutar un comando sobre un plugin
		/// </summary>
		bool CheckCanExecutePluginCommand(string plugin, string viewModel, string command);

		/// <summary>
		///		ViewModel del documento de detalles seleccionado
		/// </summary>
		Interfaces.IDetailViewModel SelectedDetailsViewModel { get; }
	}
}
