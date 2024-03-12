using Bau.Libraries.DbScripts.Manager.Models;
using Bau.Libraries.DbStudio.Application;
using Bau.Libraries.DbStudio.Application.Controllers.Export;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;

namespace Bau.Libraries.DbStudio.ViewModels.Controllers.Exporter;

/// <summary>
///		Procesador de exportación de consultas
/// </summary>
internal class ExportQueryProcessor : ProcessModel
{
	public ExportQueryProcessor(DbStudioViewModel mainViewModel, QueryModel query, string fileName, SolutionManager.FormatType formatType, 
								  long blockSize) : base("DbStudio", "Export query")
	{
		MainViewModel = mainViewModel;
		Query = query;
		FileName = fileName;
		FormatType = formatType;
		BlockSize = blockSize;
	}

	/// <summary>
	///		Ejecuta la exportación
	/// </summary>
	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		bool error = false;

			// Log
			RaiseLog(LogEventArgs.Status.Info, $"Start export query");
			// Crea el directorio
			LibHelper.Files.HelperFiles.MakePath(Path.GetDirectoryName(FileName));
			// Genera el archivo
					try
					{
						// Log
						RaiseProgress(0, 1, "Start export query");
						// Exporta la consulta
						await ExportQueryAsync(cancellationToken);
						// Log
						RaiseProgress(1, 1, "End export query");
					}
					catch (Exception exception)
					{
						RaiseLog(LogEventArgs.Status.Error, $"Error when export query. {exception.Message}");
						error = true;
					}
			// Indica que ha finalizado
			if (cancellationToken.IsCancellationRequested)
				RaiseLog(LogEventArgs.Status.Error, $"Cancel when export query to {FileName}");
			else if (!error)
				RaiseLog(LogEventArgs.Status.Success, $"End export query to {FileName}");
			else
				RaiseLog(LogEventArgs.Status.Error, $"Error when export query to {FileName}");
	}

	/// <summary>
	///		Exporta una consulta
	/// </summary>
	private async Task ExportQueryAsync(CancellationToken cancellationToken)
	{
		ExportDataBaseProcessors generator = new(MainViewModel.Manager);

			// Asocia el manejador de eventos
			generator.Progress += (sender, args) => RaiseLog(LogEventArgs.Status.Info, $"Export query to {Path.GetFileName(FileName)}. {args.Actual:#,##0} records");
			// Ejecuta el proceso
			await generator.ExportQueryAsync(Query, FileName, FormatType, BlockSize, new CsvFileParameters(), cancellationToken);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public DbStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Consulta que se va a ejecutar
	/// </summary>
	internal QueryModel Query { get; }

	/// <summary>
	///		Archivo de salida
	/// </summary>
	internal string FileName { get; }

	/// <summary>
	///		Tipo de formato
	/// </summary>
	internal SolutionManager.FormatType FormatType { get; }

	/// <summary>
	///		Tamaño de bloque
	/// </summary>
	internal long BlockSize { get; }
}
