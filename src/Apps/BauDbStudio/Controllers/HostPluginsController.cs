using Bau.Libraries.PluginsStudio.ViewModels.Base.Files;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;

namespace Bau.DbStudio.Controllers;

/// <summary>
///		Controlador del host de plugins
/// </summary>
public class HostPluginsController : Libraries.PluginsStudio.ViewModels.Base.Controllers.IHostPluginsController
{
	public HostPluginsController(DbStudioViewsManager dbStudioViewManager)
	{
		DbStudioViewManager = dbStudioViewManager;
	}

	/// <summary>
	///		Añade el archivo utilizado a la lista
	/// </summary>
	public void AddFileUsed(string fileName)
	{
		DbStudioViewManager.PluginsStudioViewModel.LastFilesViewModel.Add(fileName);
	}

	/// <summary>
	///		Obtiene el icono asociado al archivo
	/// </summary>
	public string GetIcon(string fileName)
	{
		// Obtiene el icono a partir de la lista de archivos asignados
		if (!string.IsNullOrWhiteSpace(fileName))
			foreach (FileAssignedModel file in GetFilesAssigned(false))
				if (!string.IsNullOrWhiteSpace(file.Icon) && fileName.EndsWith(file.FileExtension, StringComparison.CurrentCultureIgnoreCase))
					return file.Icon;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return string.Empty;
	}

	/// <summary>
	///		Abre el editor de un archivo
	/// </summary>
	public void OpenFile(string fileName)
	{
		DbStudioViewManager.OpenFile(fileName);
		AddFileUsed(fileName);
	}

	/// <summary>
	///		Abre un editor sobre un archivo de texto
	/// </summary>
	public void OpenEditor(BaseTextFileViewModel viewModel)
	{
		DbStudioViewManager.PluginStudioController.OpenWindow(viewModel);
		AddFileUsed(viewModel.FileName);
	}

	/// <summary>
	///		Abre la ventana de edición de un archivo de texto
	/// </summary>
	public void OpenTextEditor(string fileName, string mask)
	{
		DbStudioViewManager.PluginStudioController.DbStudioViewManager.PluginsStudioViewModel.OpenFile(fileName, mask);
		AddFileUsed(fileName);
	}

	/// <summary>
	///		Abre un navegador sobre una URL
	/// </summary>
	public void OpenWebBrowser(string url)
	{
		DbStudioViewManager.PluginsStudioViewModel.OpenWebBrowser(url);
	}

	/// <summary>
	///		Abre un navegador web con un texto HTML
	/// </summary>
	public void OpenWebBrowserWithHtml(string html)
	{
		DbStudioViewManager.PluginsStudioViewModel.OpenWebBrowserWithHtml(html);
	}

	/// <summary>
	///		Actualiza el árbol de archivos
	/// </summary>
	public void RefreshFiles()
	{
		DbStudioViewManager.PluginsStudioViewModel.RefreshFiles();
	}

	/// <summary>
	///		Ejecuta un comando sobre un plugin
	/// </summary>
	public void ExecutePluginCommand(string plugin, string viewModel, string command)
	{
		DbStudioViewManager.PluginsManager.ExecutePluginCommand(plugin, viewModel, command);
	}

	/// <summary>
	///		Obtiene los archivos asignados que se pueden crear
	/// </summary>
	public List<FileAssignedModel> GetFilesAssigned(bool onlyCanCreate)
	{
		List<FileAssignedModel> files = new Libraries.PluginsStudio.ViewModels.Base.Models.Builders.FileAssignedBuilder()
													.WithFile("Json", ".json", "/Resources/Images/FileJson.png")
													.WithFile("Xml", ".xml", "/Resources/Images/FileXml.png")
													.WithFile("Python", ".py", "/Resources/Images/FilePython.png")
													.WithFile("Markdown", ".md", "/Resources/Images/FileMd.png")
													.WithFile("Powershell", ".ps", "/Resources/Images/FilePowershell.png")
													.WithFile("C Sharp", ".cs", "/Resources/Images/FileCsharp.png")
													.WithFile("Text", ".txt", "/Resources/Images/FileTxt.png")
												.Build();

			// Añade los archivos de plugin
			foreach (FileAssignedModel file in DbStudioViewManager.PluginsManager.GetFilesAssigned())
				if ((!onlyCanCreate || (onlyCanCreate && file.CanCreate)) && 
						!files.Any(item => item.FileExtension.Equals(file.FileExtension, StringComparison.CurrentCultureIgnoreCase)))
					files.Add(file);
			// Ordena los archivos
			files.Sort((first, second) => $"{first.Name}{first.FileExtension}".CompareTo($"{second.Name}{second.FileExtension}"));
			// Devuelve la lista de archivos
			return files;
	}

	/// <summary>
	///		Comprueba si se puede ejecutar un comando sobre un plugin
	/// </summary>
	public bool CheckCanExecutePluginCommand(string plugin, string viewModel, string command)
	{
		return DbStudioViewManager.PluginsManager.CheckCanExecutePluginCommand(plugin, viewModel, command);
	}

	/// <summary>
	///		Manager de vistas
	/// </summary>
	public DbStudioViewsManager DbStudioViewManager { get; }

	/// <summary>
	///		ViewModel del documento de detalles seleccionado
	/// </summary>
	public IDetailViewModel? SelectedDetailsViewModel => DbStudioViewManager.PluginsStudioViewModel.SelectedDetailsViewModel;
}
