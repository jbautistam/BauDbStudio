using System.Windows;

using Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files;

namespace Bau.Libraries.StructuredFilesStudio.Views.Files;

/// <summary>
///		Vista para mostrar las propiedades de un archivo parquet
/// </summary>
public partial class ParquetFilePropertiesView : Window
{
	public ParquetFilePropertiesView(ParquetFilePropertiesViewModel viewModel)
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
	public ParquetFilePropertiesViewModel ViewModel { get; }
}
