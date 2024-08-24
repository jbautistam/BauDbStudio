using System.Windows;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.ToDoManager.Plugin.Controllers;

/// <summary>
///		Controlador del manager de tareas
/// </summary>
public class ToDoManagerController : ViewModel.Controllers.IToDoManagerController
{
	public ToDoManagerController(ToDoManagerPlugin taskPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		TaskManagerPlugin = taskPlugin;
		PluginController = pluginController;
	}

	/// <summary>
	///		Abre una ventana
	/// </summary>
	public SystemControllerEnums.ResultType OpenWindow(PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel detailsViewModel)
	{
		// Abre la ventana
		switch (detailsViewModel)
		{
			case ViewModel.TimeManagement.TimeListTasksViewModel viewModel:
					TaskManagerPlugin.AppViewsController.OpenDocument(new Views.TimeManagement.TimeListView(viewModel), viewModel);
				break;
			case ViewModel.ToDo.ToDoFileViewModel viewModel:
					TaskManagerPlugin.AppViewsController.OpenDocument(new Views.ToDos.TodoFileView(viewModel), viewModel);
				break;
		}
		// Devuelve el valor predeterminado
		return SystemControllerEnums.ResultType.Yes;
	}

	/// <summary>
	///		Abre un cuadro de diálogo
	/// </summary>
	public SystemControllerEnums.ResultType OpenDialog(BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel dialogViewModel)
	{
		SystemControllerEnums.ResultType result = SystemControllerEnums.ResultType.No;

			// Abre la ventana
			switch (dialogViewModel)
			{
				case ViewModel.Notes.NoteViewModel viewModel:
						Window view = new Views.Notes.NoteView(viewModel);

							// Abre la ventana de forma no modal
							TaskManagerPlugin.AppViewsController.OpenNoModalDialog(view, true);
							// Añade la ventana a la vista
							WindowNotes.Add(view);
					break;
				case ViewModel.Appointments.AppointmentViewModel viewModel:
						result = TaskManagerPlugin.AppViewsController.OpenDialog(new Views.Appointments.AppointmentView(viewModel));
					break;
				case ViewModel.ToDo.ToDoTaskViewModel viewModel:
						result = TaskManagerPlugin.AppViewsController.OpenDialog(new Views.ToDos.TaskView(viewModel));
					break;
				case ViewModel.TimeManagement.TimeViewModel viewModel:
						result = TaskManagerPlugin.AppViewsController.OpenDialog(new Views.TimeManagement.TimeView(viewModel));
					break;
				case ViewModel.TimeManagement.CsvFilterViewModel viewModel:
						result = TaskManagerPlugin.AppViewsController.OpenDialog(new Views.TimeManagement.FilterView(viewModel));
					break;
				case ViewModel.TimeManagement.ConsolidateViewModel viewModel:
						result = TaskManagerPlugin.AppViewsController.OpenDialog(new Views.TimeManagement.ConsolidateView(viewModel));
					break;
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Oculta las ventanas de notas
	/// </summary>
	public void HideNotes()
	{
		// Ocultas las ventanas de notas
		foreach (Window window in WindowNotes)
			window.Close();
		// Limpia la lista
		WindowNotes.Clear();
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public ToDoManagerPlugin TaskManagerPlugin { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }

	/// <summary>
	///		Ventanas de notas
	/// </summary>
	private List<Window> WindowNotes { get; } = new();
}