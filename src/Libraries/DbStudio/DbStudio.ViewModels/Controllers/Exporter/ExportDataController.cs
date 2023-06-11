using System.Data.Common;
using Microsoft.Extensions.Logging;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.DbScripts.Manager.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Controllers.Exporter;

/// <summary>
///		Controlador para la exportación de datos
/// </summary>
internal class ExportDataController
{
	internal ExportDataController(DbStudioViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
	}

	/// <summary>
	///		Exporta una tabla de datos a un archivo
	/// </summary>
	internal async Task<(bool exported, string error)> ExportAsync(QueryModel query, string fileName, CancellationToken cancellationToken)
	{
		string error = string.Empty;

			// Log
			MainViewModel.MainController.Logger.LogInformation($"Comienzo de grabación del archivo {fileName}");
			// Exporta el archivo
			try
			{
				using (DbDataReader? reader = await MainViewModel.Manager.ExecuteReaderAsync(query, cancellationToken))
				{
					if (reader is null)
						error = "Can't open the query";
					else if (fileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
						await ExportToCsvAsync(fileName, reader, cancellationToken);
					else if (fileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
						await ExportToParquetAsync(fileName, reader, cancellationToken);
					else if (fileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
						await ExportToSqlAsync(fileName, reader, cancellationToken);
					else
						error = "No se reconoce la extensión del archivo";
				}
			}
			catch (Exception exception)
			{
				error = $"Error al grabar el archivo {fileName}. {exception.Message}";
				MainViewModel.MainController.Logger.LogError(exception, error);
			}
			// Log
			MainViewModel.MainController.Logger.LogInformation($"Fin de grabación del archivo {fileName}");
			// Devuelve el valor que indica si se ha exportado el archivo
			return (string.IsNullOrEmpty(error), error);
	}

	/// <summary>
	///		Exporta la tabla a CSV
	/// </summary>
	private async Task ExportToCsvAsync(string fileName, DbDataReader reader, CancellationToken cancellationToken)
	{
		LibCsvFiles.Controllers.CsvDataReaderWriter writer = new LibCsvFiles.Controllers.CsvDataReaderWriter();

			// Asigna el evento de progreso
			writer.Progress += (sender, args) => MainViewModel.MainController.Logger
														.LogInformation(@$"""Exporting to {Path.GetFileName(fileName)} 
																											({args.Records:#,##0} / {args.Records + 1:#,##0}""");
			// Graba el archivo
			await writer.SaveAsync(reader, fileName, cancellationToken);
	}

	/// <summary>
	///		Exporta la tabla a parquet
	/// </summary>
	private async Task ExportToParquetAsync(string fileName, DbDataReader reader, CancellationToken cancellationToken)
	{
		await using (LibParquetFiles.Writers.ParquetDataWriter writer = new(200_000))
		{
			// Asigna el evento de progreso
			writer.Progress += (sender, args) => MainViewModel.MainController.Logger
													.LogInformation(@$"""Exporting to {Path.GetFileName(fileName)} 
																				({args.Records:#,##0} / {args.Records + 1:#,##0}""");
			// Graba el archivo
			await writer.WriteAsync(fileName, reader, cancellationToken);
		}
	}

	/// <summary>
	///		Exporta la tabla a un archivo SQL
	/// </summary>
	private async Task ExportToSqlAsync(string fileName, DbDataReader reader, CancellationToken cancellationToken)
	{
		long records = 0;

			// Borra el archivo si existía
			LibHelper.Files.HelperFiles.KillFile(fileName);
			// Escribe el archivo
			using (FileStream file = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
			{
				using (StreamWriter streamWriter = new StreamWriter(file)) 
				{
					string header = GetHeader(reader);

						// Escribe los registros
						while (await reader.ReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
						{
							string values = GetValues(reader);

								// Escribe el regisro
								streamWriter.WriteLine($"{header} VALUES ({values})");
								// Incrementa el número de registros
								records++;
								// Muestra el mensaje
								if (records % 10_000 == 0)
									MainViewModel.MainController.Logger.LogInformation($"Exporting to {Path.GetFileName(fileName)} ({records:#,##0})");
						}
				}
			}
	}

	/// <summary>
	///		Obtiene la cabecera
	/// </summary>
	private string GetHeader(DbDataReader reader)
	{
		string header = "INSERT INTO Table (";
		string fields = string.Empty;

			// Añade los campos
			for (int index = 0; index < reader.FieldCount; index++)
				fields = fields.AddWithSeparator(reader.GetName(index), ",");
			header += fields + ")";
			// Devuelve la cabecera
			return header;
	}

	/// <summary>
	///		Obtiene los valores
	/// </summary>
	private string GetValues(DbDataReader reader)
	{
		string values = string.Empty;

			// Añade los valores
			for (int index = 0; index < reader.FieldCount; index++)
				values = values.AddWithSeparator(reader.GetValue(index)?.ToString() ?? string.Empty, ",");
			// Devuelve la cadena
			return values;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public DbStudioViewModel MainViewModel { get; }
}
