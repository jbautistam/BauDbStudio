using System.Windows.Controls;

using Bau.Libraries.ToDoManager.ViewModel.ToDo;

namespace Bau.Libraries.ToDoManager.Plugin.Views.ToDos;

/// <summary>
///		Formulario para mostrar el contenido de un grupo de tareas
/// </summary>
public partial class TodoFileView : UserControl
{
	public TodoFileView(ToDoFileViewModel viewModel)
	{
		// Inicializa los componentes
		InitializeComponent();
		// Asigna la clase del documento
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel asociado al control
	/// </summary>
	public ToDoFileViewModel ViewModel { get; }
}