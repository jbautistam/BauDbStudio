using Bau.Libraries.DbStudio.Application.Controllers.Export;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;

namespace Bau.Libraries.DbStudio.ViewModels.Controllers.Exporter;

/// <summary>
///		Procesador de importación de archivo a tabla
/// </summary>
internal class ImportFilesProcessor : ProcessModel
{
	public ImportFilesProcessor(DbStudioViewModel mainViewModel, ConnectionModel connection, string fileName, ConnectionTableModel table, 
							   List<(string field, string fileField)> mappings, long blockSize,
							   CsvFileParameters csvFileParameters) : base("DbStudio", "Import file")
	{
		MainViewModel = mainViewModel;
		Connection = connection;
		FileName = fileName;
		Table = table;
		Mappings = mappings;
		BlockSize = blockSize;
		CsvFileParameters = csvFileParameters;
	}

	/// <summary>
	///		Ejecuta la exportación
	/// </summary>
	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		// Log
		RaiseLog(LogEventArgs.Status.Info, $"Start import {FileName}");
		// Genera los archivos
		try
		{
			// Importa los datos
			await ImportAsync(cancellationToken);
			// Informa del resultado
			if (cancellationToken.IsCancellationRequested)
				RaiseLog(LogEventArgs.Status.Error, $"Cancel when import {FileName}");
			else
				RaiseLog(LogEventArgs.Status.Success, $"End import {FileName}");
		}
		catch (Exception exception)
		{
			RaiseLog(LogEventArgs.Status.Error, $"Error when import {FileName}. {exception.Message}");
		}
	}

	/// <summary>
	///		Importa el archivo
	/// </summary>
	private async Task ImportAsync(CancellationToken cancellationToken)
	{
		ImportFileProcessor processor = new(MainViewModel.Manager);

			// Asocia el manejador de eventos
			processor.Progress += (sender, args) => RaiseProgress(args.Actual, args.Total);
			// Importa los datos
			await processor.ImportAsync(Connection, FileName, Table, Mappings, BlockSize, CsvFileParameters, cancellationToken);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public DbStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Conexión de la que se van a exportar los archivos
	/// </summary>
	internal ConnectionModel Connection { get; }

	/// <summary>
	///		Nombre de archivo a importar
	/// </summary>
	internal string FileName { get; }

	/// <summary>
	///		Tabla donde se importan los datos
	/// </summary>
	internal ConnectionTableModel Table { get; }

	/// <summary>
	///		Lista de mapeos entre columnas de la tabla y los campos del archivo
	/// </summary>
	internal List<(string field, string fileField)> Mappings { get; }

	/// <summary>
	///		Tamaño de bloque
	/// </summary>
	internal long BlockSize { get; }

	/// <summary>
	///		Parámetros del archivo CSV
	/// </summary>
	internal CsvFileParameters CsvFileParameters { get; }
}
