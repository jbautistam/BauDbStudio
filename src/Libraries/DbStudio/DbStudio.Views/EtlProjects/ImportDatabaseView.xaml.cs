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
		// Inicializa los componentes
		InitializeComponent();
		// Inicializa el viewmodel
		DataContext = ViewModel = viewModel;
		ViewModel.Close += (sender, eventArgs) => 
								{
									DialogResult = eventArgs.IsAccepted; 
									Close();
								};
		// Inicializa el control de los parámetros del archivo CSV
		udtCsvParameters.Initialize(viewModel.FileCsvViewModel);
	}

	/// <summary>
	///		Abre el popup de parámetros
	/// </summary>
	private void OpenPopupParameters()
	{
		wndPopUp.IsOpen = true;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ImportDatabaseViewModel ViewModel { get; }

	private void cmdOpenCsvParameters_Click(object sender, RoutedEventArgs e)
	{
		OpenPopupParameters();
    }
}
