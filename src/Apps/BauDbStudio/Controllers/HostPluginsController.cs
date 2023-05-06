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
	///		Abre el editor de un archivo
	/// </summary>
	public void OpenFile(string fileName)
	{
		DbStudioViewManager.OpenFile(fileName);
	}

	/// <summary>
	///		Abre un editor sobre un archivo de texto
	/// </summary>
	public void OpenEditor(BaseTextFileViewModel viewModel)
	{
		DbStudioViewManager.PluginStudioController.OpenWindow(viewModel);
	}

	/// <summary>
	///		Abre la ventana de edición de un archivo de texto
	/// </summary>
	public void OpenTextEditor(string fileName)
	{
		DbStudioViewManager.PluginStudioController.DbStudioViewManager.PluginsStudioViewModel.OpenFile(fileName);
	}

	/// <summary>
	///		Abre un navegador sobre una URL
	/// </summary>
	public void OpenWebBrowser(string url)
	{
		DbStudioViewManager.PluginsStudioViewModel.OpenWebBrowser(url);
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
	public List<FileAssignedModel> GetFilesAssigned()
	{
		List<FileAssignedModel> files = new()
											{
												new FileAssignedModel
															{
																Name = "Json",
																FileExtension = ".json",
																Icon = "/Resources/Images/FileJson.png"
															},
												new FileAssignedModel
															{
																Name = "Xml",
																FileExtension = ".xml",
																Icon = "/Resources/Images/FileXml.png"
															},
												new FileAssignedModel
															{
																Name = "Python",
																FileExtension = ".py",
																Icon = "/Resources/Images/FilePython.png"
															},
												new FileAssignedModel
															{
																Name = "Markdown",
																FileExtension = ".md",
																Icon = "/Resources/Images/FileMd.png"
															},
												new FileAssignedModel
															{
																Name = "Powershell",
																FileExtension = ".ps",
																Icon = "/Resources/Images/FilePowershell.png"
															},
												new FileAssignedModel
															{
																Name = "C Sharp",
																FileExtension = ".cs",
																Icon = "/Resources/Images/FileCsharp.png"
															},
												new FileAssignedModel
															{
																Name = "Text",
																FileExtension = ".txt",
																Icon = "/Resources/Images/FileTxt.png"
															}
											};

			// Añade los archivos de plugin
			foreach (FileAssignedModel file in DbStudioViewManager.PluginsManager.GetFilesAssigned())
				if (file.CanCreate && !files.Any(item => item.FileExtension.Equals(file.FileExtension, StringComparison.CurrentCultureIgnoreCase)))
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
