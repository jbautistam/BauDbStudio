using Bau.Libraries.DbStudio.Application;
using Bau.Libraries.DbStudio.Application.Controllers.Export;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;

namespace Bau.Libraries.DbStudio.ViewModels.Controllers.Exporter;

/// <summary>
///		Procesador de exportación de tablas
/// </summary>
internal class ExportTablesProcessor : ProcessModel
{
	public ExportTablesProcessor(DbStudioViewModel mainViewModel, ConnectionModel connection, List<ConnectionTableModel> tables, string outputPath, 
								 SolutionManager.FormatType formatType, long blockSize, CsvFileParameters csvFileParameters) : base("DbStudio", "Export tables")
	{
		MainViewModel = mainViewModel;
		Connection = connection;
		Tables = tables;
		OutputPath = outputPath;
		FormatType = formatType;
		BlockSize = blockSize;
		CsvFileParameters = csvFileParameters;
	}

	/// <summary>
	///		Ejecuta la exportación
	/// </summary>
	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		int errorExported = 0, exported = 0;

			// Log
			RaiseLog(LogEventArgs.Status.Info, $"Start export {Tables.Count:#,##0}");
			// Crea el directorio
			LibHelper.Files.HelperFiles.MakePath(OutputPath);
			// Genera los archivos
			foreach (ConnectionTableModel table in Tables)
				if (!cancellationToken.IsCancellationRequested)
					try
					{
						// Log
						RaiseProgress(Tables.IndexOf(table), Tables.Count, $"Start export table {table.FullName}");
						// Exporta la tabla
						await ExportTableAsync(table, cancellationToken);
						exported++;
						// Log
						RaiseProgress(Tables.IndexOf(table), Tables.Count, $"End export table {table.FullName}");
					}
					catch (Exception exception)
					{
						RaiseLog(LogEventArgs.Status.Error, $"Error when export {table.FullName}. {exception.Message}");
						errorExported++;
					}
			// Informa del resultado
			if (cancellationToken.IsCancellationRequested)
				RaiseLog(LogEventArgs.Status.Warning, $"Cancel when export {Tables.Count:#,##0} tables. (Exported {exported:#,##0} tables. Exported with error {errorExported:#,##0} tables");
			else if (errorExported == 0)
				RaiseLog(LogEventArgs.Status.Info, $"End export {Tables.Count:#,##0} tables");
			else
				RaiseLog(LogEventArgs.Status.Error, $"Error when export {errorExported:#,##0} tables (total {Tables.Count:#,##0} tables)");
			// y lo marca como finalizado
			RaiseLog(LogEventArgs.Status.Success, "End export");
	}

	/// <summary>
	///		Exporta una tabla
	/// </summary>
	private async Task ExportTableAsync(ConnectionTableModel table, CancellationToken cancellationToken)
	{
		ExportDataBaseProcessors generator = new(MainViewModel.Manager);

			// Asocia el manejador de eventos
			generator.Progress += (sender, args) => RaiseLog(LogEventArgs.Status.Info, $"Export table {table.FullName}. {args.Actual:#,##0} records");
			// Ejecuta el proceso
			await generator.ExportTableAsync(Connection, table, OutputPath, FormatType, BlockSize, CsvFileParameters, cancellationToken);
			// Log
			RaiseLog(LogEventArgs.Status.Info, $"End export table {table.FullName}");
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
	///		Tablas que se deben exportar
	/// </summary>
	internal List<ConnectionTableModel> Tables { get; }

	/// <summary>
	///		Directorio de salida
	/// </summary>
	internal string OutputPath { get; }

	/// <summary>
	///		Tipo de formato
	/// </summary>
	internal SolutionManager.FormatType FormatType { get; }

	/// <summary>
	///		Tamaño de bloque
	/// </summary>
	internal long BlockSize { get; }

	/// <summary>
	///		Parámetros del archivo CSV
	/// </summary>
	internal CsvFileParameters CsvFileParameters { get; }
}
