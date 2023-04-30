using System.Collections.Generic;
using System.Windows.Input;

using Bau.Libraries.ToDoManager.ViewModel;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;
using Bau.Libraries.LibSystem.Windows.KeyboardHook;

namespace Bau.Libraries.ToDoManager.Plugin;

/// <summary>
///		Plugin para el gestor de tareas
/// </summary>
public class ToDoManagerPlugin : IPlugin
{ 
	/// <summary>
	///		Inicializa el manager de vistas del gestor de tareas
	/// </summary>
	public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		// Inicializa el controlador
		AppViewsController = appViewsController;
		MainViewModel = new ToDoManagerViewModel(new Controllers.ToDoManagerController(this, pluginController));
		MainViewModel.Initialize();
		// Inicializa el Hook de teclado
		InitHookManager();
	}

	/// <summary>
	///		Inicializa el manager de teclado
	/// </summary>
	private void InitHookManager()
	{
		if (MainViewModel.ConfigurationViewModel.HookGlobal)
		{
			// Inicializa el manager de teclado
			KeyboardHookManager = new KeyboardHookManager();
			// Registra las teclas
			KeyboardHookManager.RegisterHotkey(KeyboardHookManager.ModifierKeys.Control | KeyboardHookManager.ModifierKeys.Alt, 
											   KeyInterop.VirtualKeyFromKey(Key.F1), CreateNewNote);
			KeyboardHookManager.RegisterHotkey(KeyboardHookManager.ModifierKeys.Control | KeyboardHookManager.ModifierKeys.Alt, 
											   KeyInterop.VirtualKeyFromKey(Key.F2), ShowNotes);

			// Arranca el manejador
			KeyboardHookManager.Start();
		}
	}

	/// <summary>
	///		Crea una nueva nota
	/// </summary>
	private void CreateNewNote()
	{
		System.Windows.Application.Current.Dispatcher.Invoke(MainViewModel.CreateNewNote);
	}

	/// <summary>
	///		Muestra las ventanas de notas
	/// </summary>
	private void ShowNotes()
	{
		System.Windows.Application.Current.Dispatcher.Invoke(MainViewModel.ShowNotes);
	}

	/// <summary>
	///		Carga los datos del directorio
	/// </summary>
	public void Load(string path)
	{
		MainViewModel.Load(path);
	}

	/// <summary>
	///		Actualiza los exploradores y ventanas
	/// </summary>
	public void Refresh()
	{
		MainViewModel.Load(string.Empty);
	}

	/// <summary>
	///		Intenta abrir un archivo en un plugin
	/// </summary>
	public bool OpenFile(string fileName) => MainViewModel.OpenFile(fileName);

	/// <summary>
	///		Obtiene los paneles del plugin
	/// </summary>
	public List<PaneModel> GetPanes() => new();

	/// <summary>
	///		Obtiene las barras de herramientas del plugin
	/// </summary>
	public List<ToolBarModel> GetToolBars() => new();

	/// <summary>
	///		Obtiene los menús del plugin
	/// </summary>
	public List<MenuListModel> GetMenus()
	{
		return new PluginsStudio.ViewModels.Base.Models.Builders.MenuBuilder()
							.WithMenu(MenuListModel.SectionType.Tools)
								.WithItem("Create note", MainViewModel.CreateNewNoteCommand, GetIcon("Pin.png"))
								.WithItem("Show notes", MainViewModel.ShowNotesCommand, GetIcon("Task.png"))
						.Build();
	}

	/// <summary>
	///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
	/// </summary>
	public List<FileOptionsModel> GetFilesOptions() => null;

	/// <summary>
	///		Obtiene las extensiones de archivo asociadas al plugin
	/// </summary>
	public List<FileAssignedModel> GetFilesAssigned()
	{
		return new()
				{
					new FileAssignedModel
							{
								Name = "ToDo file",
								FileExtension = ToDoManagerViewModel.ToDoFileExtension,
								Icon = GetIcon("ToDoFile.png")
							},
					new FileAssignedModel
							{
								Name = "Pattern text file",
								FileExtension = ToDoManagerViewModel.PatternFileExtension,
								Icon = GetIcon("PatternFile.png")
							}
				};
	}

	/// <summary>
	///		Obtiene la ruta de un icono
	/// </summary>
	private string GetIcon(string name) => $"/ToDoManager.Plugin;component/Resources/{name}";

	/// <summary>
	///		Obtiene la vista de configuración del plugin
	/// </summary>
	public IPluginConfigurationView GetConfigurationView()
	{
		return new Views.Configuration.ctlConfiguration(MainViewModel.ConfigurationViewModel);
	}

	/// <summary>
	///		Controlador de aplicación
	/// </summary>
	internal IAppViewsController AppViewsController { get; private set; }

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; private set; }

	/// <summary>
	///		Manager del Hook de teclado
	/// </summary>
	internal KeyboardHookManager? KeyboardHookManager { get; private set; }
}
