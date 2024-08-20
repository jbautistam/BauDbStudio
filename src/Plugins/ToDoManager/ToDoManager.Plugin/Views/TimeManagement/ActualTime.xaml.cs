using System.Windows.Controls;
using Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

namespace Bau.Libraries.ToDoManager.Plugin.Views.TimeManagement;

/// <summary>
///		Control para mostrar el tiempo actual
/// </summary>
public partial class ActualTime : UserControl
{
	public ActualTime()
	{
		InitializeComponent();
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	public void InitControl(TimeEditableViewModel timeEditableViewModel)
	{
		DataContext = TimeEditableViewModel = timeEditableViewModel;
	}

	/// <summary>
	///		ViewModel de la edición de horas
	/// </summary>
	public TimeEditableViewModel? TimeEditableViewModel { get; private set; }
}
