using System.Windows;

using Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

namespace Bau.Libraries.ToDoManager.Plugin.Views.TimeManagement;

/// <summary>
///		Vista para mostrar el filtro de exportación
/// </summary>
public partial class FilterView : Window
{
	public FilterView(CsvFilterViewModel viewModel)
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
	public CsvFilterViewModel ViewModel { get; }
}
