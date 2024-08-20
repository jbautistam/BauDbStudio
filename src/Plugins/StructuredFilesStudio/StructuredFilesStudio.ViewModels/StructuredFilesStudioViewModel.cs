using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels;

/// <summary>
///		ViewModel de la solución
/// </summary>
public class StructuredFilesStudioViewModel : BaseObservableObject
{
	// Variables privadas
	private string _text = string.Empty;

	public StructuredFilesStudioViewModel(string appName, Controllers.IStructuredFilesStudioController mainController)
	{
		// Título de la aplicación
		Text = appName;
		// Asigna las propiedades
		MainController = mainController;
	}

	/// <summary>
	///		Abre un archivo (si reconoce la extensión)
	/// </summary>
	public bool OpenFile(string fileName)
	{
		bool opened = false;

			if (!string.IsNullOrWhiteSpace(fileName))
			{
				IDetailViewModel? fileViewModel = GetFileViewModel(fileName);

					// Abre la ventana
					if (fileViewModel != null)
					{
						// Abre la ventana
						MainController.OpenWindow(fileViewModel);
						// Indica que se ha abierto correctamente
						opened = true;
					}
			}
			// Devuelve el valor que indica si se ha abierto el archivo
			return opened;
	}

	/// <summary>
	///		Obtiene el viewModel adecuado para un archivo
	/// </summary>
	private IDetailViewModel? GetFileViewModel(string fileName)
	{
		if (fileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
			return new Details.Files.ParquetFileViewModel(this, fileName);
		else if (fileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
			return new Details.Files.CsvFileViewModel(this, fileName);
		else if (fileName.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase) ||
				 fileName.EndsWith(".xls", StringComparison.CurrentCultureIgnoreCase))
			return new Details.Files.ExcelFileViewModel(this, fileName);
		else
			return null;
	}

	/// <summary>
	///		Controlador principal
	/// </summary>
	public Controllers.IStructuredFilesStudioController MainController { get; }

	/// <summary>
	///		Título de la ventana
	/// </summary>
	public string Text 
	{
		get { return _text; }
		set { CheckProperty(ref _text, value); }
	}
}