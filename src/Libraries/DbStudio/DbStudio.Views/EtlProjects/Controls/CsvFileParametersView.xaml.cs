using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects;

namespace Bau.Libraries.DbStudio.Views.EtlProjects.Controls;

/// <summary>
///		Control de usuario para mostrar los parámetros de un archivo Csv
/// </summary>
public partial class CsvFileParametersView : UserControl
{
	public CsvFileParametersView()
	{
		InitializeComponent();
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	public void Initialize(CsvFileViewModel viewModel)
	{
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public CsvFileViewModel ViewModel { get; private set; } = default!;
}
