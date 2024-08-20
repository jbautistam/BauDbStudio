namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files;

/// <summary>
///		ViewModel de los datos de una columna de un archivo Parquet
/// </summary>
public class ParquetFileListItemColumnViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private ParquetFilePropertiesViewModel _filePropertiesViewModel = default!;
	private string _column = default!, _type = default!;

	public ParquetFileListItemColumnViewModel(ParquetFilePropertiesViewModel filePropertiesViewModel, string column, string type)
	{
		FilePropertiesViewModel = filePropertiesViewModel;
		Column = column;
		Type = type;
	}

	/// <summary>
	///		ViewModel de propiedades del archivo
	/// </summary>
	public ParquetFilePropertiesViewModel FilePropertiesViewModel
	{
		get { return _filePropertiesViewModel; }
		set {CheckObject(ref _filePropertiesViewModel, value); }
	}

	/// <summary>
	///		Nombre de columna
	/// </summary>
	public string Column
	{
		get { return _column; }
		set { CheckProperty(ref _column, value); }
	}

	/// <summary>
	///		Tipo de la columna
	/// </summary>
	public string Type
	{
		get { return _type; }
		set { CheckProperty(ref _type, value); }
	}
}
