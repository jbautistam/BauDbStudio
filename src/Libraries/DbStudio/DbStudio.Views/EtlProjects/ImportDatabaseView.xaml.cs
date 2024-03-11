using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects;

namespace Bau.Libraries.DbStudio.Views.EtlProjects;

/// <summary>
///		Vista para importar un archivo sobre una base de datos
/// </summary>
public partial class ImportDatabaseView : Window
{
	public ImportDatabaseView(ImportDatabaseViewModel viewModel)
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
	///		ViewModel
	/// </summary>
	public ImportDatabaseViewModel ViewModel { get; }
}
