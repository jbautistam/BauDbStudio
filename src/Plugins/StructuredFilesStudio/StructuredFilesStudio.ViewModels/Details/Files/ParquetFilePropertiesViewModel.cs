using System.Collections.ObjectModel;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files;

/// <summary>
///		ViewModel con las propiedades de un archivo Parquet
/// </summary>
public class ParquetFilePropertiesViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private ObservableCollection<ParquetFileListItemColumnViewModel> _columns = default!;

	public ParquetFilePropertiesViewModel(StructuredFilesStudioViewModel solutionViewModel, ParquetFileViewModel fileViewModel)
	{
		// Inicializa las propiedades
		SolutionViewModel = solutionViewModel;
		FileViewModel = fileViewModel;
		// Inicializa el viewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel()
	{
		// Carga la lista de columnas
		Columns = new ObservableCollection<ParquetFileListItemColumnViewModel>();
		if (FileViewModel.DataResults is not null)
			foreach (System.Data.DataColumn column in FileViewModel.DataResults.Columns)
				Columns.Add(new ParquetFileListItemColumnViewModel(this, column.ColumnName, column.DataType.ToString()));
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		// Indica que ya no es nuevo y está grabado
		IsUpdated = false;
		// Cierra la ventana
		RaiseEventClose(true);
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public StructuredFilesStudioViewModel SolutionViewModel { get; }

	/// <summary>
	///		ViewModel del archivo
	/// </summary>
	public ParquetFileViewModel FileViewModel { get; }

	/// <summary>
	///		Lista de columnas
	/// </summary>
	public ObservableCollection<ParquetFileListItemColumnViewModel> Columns
	{
		get { return _columns; }
		set { CheckObject(ref _columns, value); }
	}
}