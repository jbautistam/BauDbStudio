using System.Windows;

using Bau.Libraries.ToDoManager.ViewModel.ToDo;

namespace Bau.Libraries.ToDoManager.Plugin.Views.ToDos;

/// <summary>
///		Vista para mostrar los datos de una tarea
/// </summary>
public partial class TaskView : Window
{
	public TaskView(ToDoTaskViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
		ViewModel.Close += (sender, eventArgs) => 
								{
									DialogResult = eventArgs.IsAccepted; 
									Close();
								};
	}

	/// <summary>
	///		ViewModel de la tarea
	/// </summary>
	public ToDoTaskViewModel ViewModel { get; }
}
