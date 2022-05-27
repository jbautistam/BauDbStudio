using System;
using System.Collections.Generic;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;

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
		///		Obtiene los archivos asignados que se pueden crear
		/// </summary>
		public List<FileAssignedModel> GetFilesAssigned();

		/// <summary>
		///		Abre un archivo
		/// </summary>
		void OpenFile(string fileName);

		/// <summary>
		///		Abre la ventana de edición de un archivo
		/// </summary>
		void OpenEditor(Files.BaseTextFileViewModel viewModel);

		/// <summary>
		///		Abre un navegador Web
		/// </summary>
		void OpenWebBrowser(string url);

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
