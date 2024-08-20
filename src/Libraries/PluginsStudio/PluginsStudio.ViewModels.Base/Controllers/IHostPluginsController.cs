using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers;

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
	List<FileAssignedModel> GetFilesAssigned(bool onlyCanCreate);

	/// <summary>
	///		Obtiene el icono asociado a un archivo
	/// </summary>
	string GetIcon(string fileName);

	/// <summary>
	///		Abre un archivo
	/// </summary>
	void OpenFile(string fileName);

	/// <summary>
	///		Abre la ventana de edición de un archivo
	/// </summary>
	void OpenEditor(Files.BaseTextFileViewModel viewModel);

	/// <summary>
	///		Abre la ventana de edición de un archivo de texto
	/// </summary>
	void OpenTextEditor(string fileName, string mask);

	/// <summary>
	///		Abre un navegador Web
	/// </summary>
	void OpenWebBrowser(string url);

	/// <summary>
	///		Abre un navegador web con un texto HTML
	/// </summary>
	void OpenWebBrowserWithHtml(string html);

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
	Interfaces.IDetailViewModel? SelectedDetailsViewModel { get; }
}
