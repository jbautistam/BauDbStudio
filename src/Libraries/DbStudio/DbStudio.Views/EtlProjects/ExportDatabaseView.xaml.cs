using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects;

namespace Bau.Libraries.DbStudio.Views.EtlProjects;

/// <summary>
///		Vista para exportar archivos de una base de datos
/// </summary>
public partial class ExportDatabaseView : Window
{
	public ExportDatabaseView(ExportDatabaseViewModel viewModel)
	{
		// Inicializa los componentes
		InitializeComponent();
		// Inicializa el viewmodel
		DataContext = ViewModel = viewModel;
		ViewModel.Close += (sender, eventArgs) => 
								{
									DialogResult = eventArgs.IsAccepted; 
									Close();
								};
		// Inicializa el control de parámetros del archivo CSV
		udtCsvParameters.Initialize(viewModel.FileCsvViewModel);
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ExportDatabaseViewModel ViewModel { get; }
}
