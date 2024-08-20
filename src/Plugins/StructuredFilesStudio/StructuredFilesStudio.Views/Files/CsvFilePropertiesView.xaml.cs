using System.Windows;

using Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files;

namespace Bau.Libraries.StructuredFilesStudio.Views.Files;

/// <summary>
///		Vista para mostrar las propiedades de un archivo
/// </summary>
public partial class CsvFilePropertiesView : Window
{
	public CsvFilePropertiesView(CsvFilePropertiesViewModel viewModel)
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
	///		ViewModel del archivo
	/// </summary>
	public CsvFilePropertiesViewModel ViewModel { get; }
}
