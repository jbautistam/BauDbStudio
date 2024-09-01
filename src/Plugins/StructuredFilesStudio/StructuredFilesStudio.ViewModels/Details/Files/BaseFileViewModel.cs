using System.Data;

using Bau.Libraries.BauMvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files;

/// <summary>
///		ViewModel para visualización de archivos utilizando dataTable
/// </summary>
public abstract class BaseFileViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Constantes públicas
	protected const int RecordsPerBlock = 20_000;
	// Enumerados públicos
	/// <summary>
	///		Tipo del campo
	/// </summary>
	public enum FieldType
	{
		Unknown,
		String,
		Date, 
		Integer, 
		Decimal, 
		Boolean
	}
	// Variables privadas
	private string _header = string.Empty, _fileName = string.Empty, _formattedPage = string.Empty;
	private int _actualPage, _pages, _recordsPerPage;
	private long _records;
	private DataTable? _dataResults;
	private Filters.ListFileFilterViewModel _filters = default!;

	protected BaseFileViewModel(StructuredFilesStudioViewModel solutionViewModel, string fileName) : base(false)
	{
		// Asigna las propiedades
		SolutionViewModel = solutionViewModel;
		FileName = fileName;
		ActualPage = 1;
		Pages = 0;
		Records = 0;
		RecordsPerPage = 10_000;
		// Asigna los objetos
		Filters = new Filters.ListFileFilterViewModel(this);
		// Asigna los comandos
		NextPageCommand = new BaseCommand(async _ => await GoToPageAsync(ActualPage + 1, CancellationToken.None), 
										  _ => ActualPage < Pages)
									.AddListener(this, nameof(Records))
									.AddListener(this, nameof(RecordsPerPage));
		PreviousPageCommand = new BaseCommand(async _ => await GoToPageAsync(ActualPage - 1, CancellationToken.None), 
											  _ => ActualPage > 1)
									.AddListener(this, nameof(Records))
									.AddListener(this, nameof(RecordsPerPage));
		FirstPageCommand = new BaseCommand(async _ => await GoToPageAsync(1, CancellationToken.None), 
										   _ => ActualPage > 1)
									.AddListener(this, nameof(Records))
									.AddListener(this, nameof(RecordsPerPage));
		LastPageCommand = new BaseCommand(async _ => await GoToPageAsync(Pages, CancellationToken.None), 
										  _ => ActualPage < Pages)
									.AddListener(this, nameof(Records))
									.AddListener(this, nameof(RecordsPerPage));
		FilePropertiesCommand = new BaseCommand(async _ => await OpenFilePropertiesAsync(CancellationToken.None));
		FilterCommand = new BaseCommand(async _ => await FilterDataAsync(CancellationToken.None), _ => DataResults is not null)
									.AddListener(this, nameof(DataResults));
	}

	/// <summary>
	///		Carga el archivo
	/// </summary>
	public async Task LoadFileAsync(CancellationToken cancellationToken)
	{
		long totalRecords = 0;

			// Cambia el cursor
			SolutionViewModel.MainController.MainWindowController.ShowWaitingCursor();
			// Carga el archivo
			try
			{
				// Carga el archivo
				(DataResults, totalRecords) = await LoadFileAsync(Records == 0, cancellationToken);
				// Inicializa el ViewModel de los filtros
				if (!Filters.IsInitialized)
					Filters.InitViewModel();
			}
			catch (Exception exception)
			{
				SolutionViewModel.MainController.HostController.SystemController.ShowMessage($"Error when load {FileName}{Environment.NewLine}{exception.Message}");
			}
			// Cambia el cursor
			SolutionViewModel.MainController.MainWindowController.HideWaitingCursor();
			// Asigna el número de registros y páginas
			if (Records == 0)
				Records = totalRecords;
			Pages = (int) Records / RecordsPerPage + 1;
	}

	/// <summary>
	///		Carga un archivo y obtiene una tabla paginada
	/// </summary>
	protected abstract Task<(DataTable table, long totalRecords)> LoadFileAsync(bool countRecords, CancellationToken cancellationToken);

	/// <summary>
	///		Abre las propiedades del archivo
	/// </summary>
	protected abstract Task OpenFilePropertiesAsync(CancellationToken cancellationToken);

	/// <summary>
	///		Filtrado de datos
	/// </summary>
	private async Task FilterDataAsync(CancellationToken cancellationToken)
	{
		if (DataResults is null)
			SolutionViewModel.MainController.HostController.SystemController.ShowMessage("No hay datos a filtrar");
		else
		{
			if (SolutionViewModel.MainController.OpenDialog(Filters) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				// Indica que se deben volver a contar los registros
				Records = 0;
				// Carga la página
				await GoToPageAsync(1, cancellationToken);
			}
		}
	}

	/// <summary>
	///		Calcula el texto de las páginas formateadas
	/// </summary>
	private void ComputeFormattedPage()
	{
		FormattedPage = $"{ActualPage:#,##0} / {Pages:#,##0} - {Records:#,##0}";
	}

	/// <summary>
	///		Carga la siguiente página
	/// </summary>
	private async Task GoToPageAsync(int newPage, CancellationToken cancellationToken)
	{
		// Cambia el número de página
		ActualPage = newPage;
		// Carga el archivo
		await LoadFileAsync(cancellationToken);
	}

	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		if (DataResults is null)
			SolutionViewModel.MainController.HostController.SystemController.ShowMessage("No hay datos para exportar");
		else
		{
			string? fileName = SolutionViewModel.MainController.DialogsController
										.OpenDialogSave(string.Empty, 
														$"Archivos CSV (*.csv)|*.csv|Archivos parquet (*.parquet)|*.parquet|Todos los archivos (*.*)|*.*",
														$"New file.csv", 
														$".csv");

				if (!string.IsNullOrEmpty(fileName))
				{
					if (fileName.Equals(FileName, StringComparison.CurrentCultureIgnoreCase))
						SolutionViewModel.MainController.HostController.SystemController.ShowMessage("No se puede grabar el archivo con el mismo nombre");
					else
					{
						// Log
						SolutionViewModel.MainController.Logger.LogInformation($"Comienzo de grabación del archivo {fileName}");
						// Graba el archivo
						try
						{
							Task.Run(async () => await SaveFileAsync(SolutionViewModel.MainController.Logger, fileName, CancellationToken.None));
						}
						catch (Exception exception)
						{
							SolutionViewModel.MainController.Logger.LogError(exception, $"Error al grabar el archivo {fileName}. {exception.Message}");
							SolutionViewModel.MainController.HostController.SystemController.ShowMessage($"Error al grabar el archivo {fileName}. {exception.Message}");
						}
					}
				}
		}
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	protected abstract Task SaveFileAsync(ILogger logger, string fileName, CancellationToken cancellationToken);

	/// <summary>
	///		Obtiene el mensaje que se debe mostrar al cerrar la ventana
	/// </summary>
	public string GetSaveAndCloseMessage() => "¿Desea grabar el archivo antes de continuar?";

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public abstract void Close();

	/// <summary>
	///		Solución
	/// </summary>
	public StructuredFilesStudioViewModel SolutionViewModel { get; }

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header 
	{
		get { return _header; }
		set { CheckProperty(ref _header, value); }
	}

	/// <summary>
	///		Id de la ficha
	/// </summary>
	public string TabId => GetType().ToString() + "_" + FileName;

	/// <summary>
	///		Resultados de la ejecución de la consulta
	/// </summary>
	public DataTable? DataResults
	{ 
		get { return _dataResults; }
		set { CheckObject(ref _dataResults, value); }
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set 
		{ 
			if (CheckProperty(ref _fileName, value))
			{
				if (!string.IsNullOrWhiteSpace(value))
					Header = Path.GetFileName(value);
				else
					Header = "Parquet";
			}
		}
	}

	/// <summary>
	///		Página actual
	/// </summary>
	public int ActualPage
	{
		get { return _actualPage; }
		set 
		{ 
			if (CheckProperty(ref _actualPage, value))
				ComputeFormattedPage(); 
		}
	}

	/// <summary>
	///		Número de página
	/// </summary>
	public int Pages
	{
		get { return _pages; }
		set 
		{ 
			if (CheckProperty(ref _pages, value))
				ComputeFormattedPage(); 
		}
	}

	/// <summary>
	///		Número total de registros
	/// </summary>
	public long Records
	{
		get { return _records; }
		set 
		{ 
			if (CheckProperty(ref _records, value))
				ComputeFormattedPage();
		}
	}

	/// <summary>
	///		Registros por página
	/// </summary>
	public int RecordsPerPage
	{
		get { return _recordsPerPage; }
		set 
		{ 
			if (CheckProperty(ref _recordsPerPage, value))
				ComputeFormattedPage();
		}
	}

	/// <summary>
	///		Número de páginas formateado
	/// </summary>
	public string FormattedPage
	{
		get { return _formattedPage; }
		set { CheckProperty(ref _formattedPage, value); }
	}

	/// <summary>
	///		Filtros aplicados
	/// </summary>
	public Filters.ListFileFilterViewModel Filters
	{
		get { return _filters; }
		set { CheckObject(ref _filters, value); }
	}

	/// <summary>
	///		Comando para ir a la siguiente página
	/// </summary>
	public BaseCommand NextPageCommand { get; }

	/// <summary>
	///		Comando para ir a la página anterior
	/// </summary>
	public BaseCommand PreviousPageCommand { get; }

	/// <summary>
	///		Comando para ir a la primera página
	/// </summary>
	public BaseCommand FirstPageCommand { get; }

	/// <summary>
	///		Comando para ir a la última página
	/// </summary>
	public BaseCommand LastPageCommand { get; }

	/// <summary>
	///		Comando para ver las propiedades de un archivo
	/// </summary>
	public BaseCommand FilePropertiesCommand { get; }

	/// <summary>
	///		Comando para filtrar datos
	/// </summary>
	public BaseCommand FilterCommand { get; }
}
