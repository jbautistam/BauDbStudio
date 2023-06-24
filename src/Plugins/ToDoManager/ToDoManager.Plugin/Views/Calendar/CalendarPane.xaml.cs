using System.Windows.Controls;

using Bau.Libraries.ToDoManager.ViewModel.Notes;

namespace Bau.Libraries.ToDoManager.Plugin.Views.Notes;

/// <summary>
///		Panel del calendario
/// </summary>
public partial class CalendarPane : UserControl
{
	public CalendarPane(NoteViewModel treeViewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = treeViewModel;
	}

	/// <summary>
	///		ViewModel del calendario
	/// </summary>
	public NoteViewModel ViewModel { get; }
}