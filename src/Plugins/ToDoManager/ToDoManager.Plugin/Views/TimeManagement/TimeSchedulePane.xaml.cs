using System.Windows.Controls;
using Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

namespace Bau.Libraries.ToDoManager.Plugin.Views.TimeManagement;

/// <summary>
///		Panel del control de gestión de tiempos
/// </summary>
public partial class TimeSchedulePane : UserControl
{
	public TimeSchedulePane(TimeScheduleViewModel timeScheduleViewModel)
	{
		InitializeComponent();
		InitControl(timeScheduleViewModel);
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	private void InitControl(TimeScheduleViewModel timeScheduleViewModel)
	{
		// Inicializa el contexto de datos
		DataContext = ViewModel = timeScheduleViewModel;
		// Inicializa el control de tiempo
		timeControl.InitControl(ViewModel.ActualTimeViewModel);
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public TimeScheduleViewModel ViewModel { get; private set; } = default!;
}
