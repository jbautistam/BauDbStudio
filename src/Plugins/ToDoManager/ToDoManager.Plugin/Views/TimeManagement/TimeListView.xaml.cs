using System.Windows.Controls;

using Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

namespace Bau.Libraries.ToDoManager.Plugin.Views.TimeManagement;

/// <summary>
///		Formulario para mostrar el contenido de un grupo de tareas
/// </summary>
public partial class TimeListView : UserControl
{
	public TimeListView(TimeListTasksViewModel viewModel)
	{
		// Inicializa los componentes
		InitializeComponent();
		// Asigna la clase del documento
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel asociado al control
	/// </summary>
	public TimeListTasksViewModel ViewModel { get; }
}