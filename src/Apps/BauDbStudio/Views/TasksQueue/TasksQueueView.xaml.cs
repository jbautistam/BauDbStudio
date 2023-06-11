using System.Windows.Controls;

using Bau.Libraries.PluginsStudio.ViewModels.TasksQueue;

namespace Bau.DbStudio.Views.TasksQueue;

/// <summary>
///		Ventana para mostrar los procesos en ejecución
/// </summary>
public partial class TasksQueueView : UserControl
{
	public TasksQueueView(TasksQueueListViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public TasksQueueListViewModel ViewModel { get; }
}
