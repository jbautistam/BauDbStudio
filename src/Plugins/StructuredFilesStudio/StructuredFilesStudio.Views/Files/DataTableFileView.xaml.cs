using System.Windows.Controls;

using Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files;

namespace Bau.Libraries.StructuredFilesStudio.Views.Files;

/// <summary>
///		Ventana para mostrar el contenido de un archivo
/// </summary>
public partial class DataTableFileView : UserControl
{
	public DataTableFileView(BaseFileViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		Inicializa el formulario
	/// </summary>
	private async Task InitFormAsync()
	{
		await ViewModel.LoadFileAsync(CancellationToken.None);
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public BaseFileViewModel ViewModel { get; }

	private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
	{
		await InitFormAsync();
	}

	private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
	{
		e.Row.Header = (e.Row.GetIndex() + 1).ToString(); 
	}

	/// <summary>
	///		Evita que desaparezcan los caracteres "_" de las cabeceras
	/// </summary>
	private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
	{
		if (e.Column is not null)
		{
			string? header = e.Column.Header.ToString();

				if (!string.IsNullOrWhiteSpace(header))
					e.Column.Header = header.Replace("_", "__");
				else
					e.Column.Header = "No header";
		}
	}
}
