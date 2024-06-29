using System.Windows;

using Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

namespace Bau.Libraries.ToDoManager.Plugin.Views.TimeManagement;

/// <summary>
///		Vista para mostrar los parámetros de consolidación de tareas
/// </summary>
public partial class ConsolidateView : Window
{
	public ConsolidateView(ConsolidateViewModel viewModel)
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
	///		ViewModel del elemento
	/// </summary>
	public ConsolidateViewModel ViewModel { get; }
}
