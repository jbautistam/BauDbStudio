using System.Data;
using Microsoft.Extensions.Logging;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibDbProviders.Base;

namespace Bau.Libraries.DbStudio.Application.Controllers.Export;

/// <summary>
///		Exportación de archivos de base de datos
/// </summary>
public class ExportDataBaseGenerator
{
	public ExportDataBaseGenerator(SolutionManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Exporta las tablas de una conexión a una serie de archivos
	/// </summary>
	public async Task<bool> ExportAsync(ConnectionModel connection, List<ConnectionTableModel> tables,
										string path, SolutionManager.FormatType formatType, long blockSize, CancellationToken cancellationToken)
	{
		// Limpia los errores
		Errors.Clear();
		// Crea el directorio
		LibHelper.Files.HelperFiles.MakePath(path);
		// Genera los archivos
		foreach (ConnectionTableModel table in tables)
			if (!cancellationToken.IsCancellationRequested)
				try
				{
					// Log
					Manager.Logger.LogInformation($"Start export table {table.FullName}");
					// Exporta la tabla
					await ExportTableAsync(connection, table, path, formatType, blockSize, cancellationToken);
					// Log
					Manager.Logger.LogInformation($"End export table {table.FullName}");
				}
				catch (Exception exception)
				{
					Errors.Add($"Exception when export {table.FullName}. {exception.Message}");
					Manager.Logger.LogError(exception, $"Error when export {table.FullName}");
				}
		// Devuelve el valor que indica si la exportación ha sido correcta
		return Errors.Count == 0;
	}

	/// <summary>
	///		Exporta una tabla particionando la consulta en varios <see cref="IDataReader"/> (porque por ejemplo spark carga todo el dataReader en memoria
	///	y da un error de OutOfMemory)
	/// </summary>
	private async Task ExportTableAsync(ConnectionModel connection, ConnectionTableModel table, string path, 
										SolutionManager.FormatType formatType, long blockSize, CancellationToken cancellationToken)
	{
		IDbProvider provider = Manager.DbScriptsManager.GetDbProvider(connection);
		long records = 0;
		int totalPages = 1;

			// Si es una conexión a Spark, se va a paginar si se supera el número de registros por blogue
			if (connection.Type == ConnectionModel.ConnectionType.Spark)
			{
				records = provider.GetRecordsCount($"SELECT * FROM {provider.SqlHelper.FormatName(table.Schema, table.Name)}", 
												   null, connection.TimeoutExecuteScript) ?? 0;
				totalPages = (int) Math.Ceiling((double) (records / blockSize)) + 1;
			}
			// Obtiene los datos
			for (int actualPage = 0; actualPage < totalPages; actualPage++)
			{
				string fileName;
				string sql;
				
					// Obtiene el nombre de archivo
					if (totalPages == 1)
						fileName = GetFileName(path, table, formatType);
					else
						fileName = GetFileName(path, table, formatType, actualPage + 1);
					// Graba el archivo de la página adecuada
					if (totalPages == 1)
						sql = $"SELECT * FROM {provider.SqlHelper.FormatName(table.Schema, table.Name)}";
					else
						sql = GetSparkPaginatedSql(provider, table, actualPage, blockSize);
					//Log
					Manager.Logger.LogInformation($"Reading page {actualPage + 1} / {records / blockSize + 1:#,##0} ({records:#,##0}) from table {table.Name}");
					// Exporta la tabla
					await ExportTableAsync(provider, sql, fileName, formatType, connection.TimeoutExecuteScript, cancellationToken);
			}
	}

	/// <summary>
	///		Obtiene la SQL para paginar una consulta en Spark
	/// </summary>
	private string GetSparkPaginatedSql(IDbProvider provider, ConnectionTableModel table, int actualPage, long blockSize)
	{
		string nameRowId = provider.SqlHelper.FormatName($"##_Row_Number_{table.Schema}_{table.Name}##");
		string sqlFields = string.Empty;

			// Obtiene los nombres de campo
			foreach (ConnectionTableFieldModel field in table.Fields)
				sqlFields = sqlFields.AddWithSeparator(provider.SqlHelper.FormatName(field.Name), ",");
			// Devuelve la cadena SQL
			return @$"SELECT {sqlFields}
							FROM (SELECT Row_Number() OVER (ORDER BY 1 ASC) AS {nameRowId}, {sqlFields} 
									FROM {provider.SqlHelper.FormatName(table.Schema, table.Name)}) AS tmp
							WHERE {nameRowId} BETWEEN {actualPage * blockSize + 1} AND {(actualPage + 1) * blockSize}";
	}

	/// <summary>
	///		Exporta una tabla
	/// </summary>
	private async Task ExportTableAsync(IDbProvider provider, string sql, string fileName, 
										SolutionManager.FormatType formatType, TimeSpan timeout, CancellationToken cancellationToken)
	{
		using (IDataReader reader = provider.ExecuteReader(sql, null, CommandType.Text, timeout))
		{
			switch (formatType)
			{
				case SolutionManager.FormatType.Csv:
						ExportToCsv(fileName, reader);
					break;
				case SolutionManager.FormatType.Parquet:
						await ExportToParquetAsync(fileName, reader, cancellationToken);
					break;
			}
		}
	}

	/// <summary>
	///		Exporta la tabla a CSV
	/// </summary>
	private void ExportToCsv(string fileName, IDataReader reader)
	{
		LibCsvFiles.Controllers.CsvDataReaderWriter writer = new LibCsvFiles.Controllers.CsvDataReaderWriter();
		
			// Asigna el evento de progreso
			writer.Progress += (sender, args) => Manager.Logger.LogInformation($"Exporting to {Path.GetFileName(fileName)} ({args.Records:#,##0} / {args.Records + 1:#,##0})");
			// Graba el archivo
			writer.Save(reader, fileName);
	}

	/// <summary>
	///		Exporta la tabla a parquet
	/// </summary>
	private async Task ExportToParquetAsync(string fileName, IDataReader reader, CancellationToken cancellationToken)
	{
		await using (LibParquetFiles.Writers.ParquetDataWriter writer = new(200_000))
		{
			// Asigna el evento de progreso
			writer.Progress += (sender, args) => Manager.Logger.LogInformation($"Exporting to {Path.GetFileName(fileName)} ({args.Records:#,##0} / {args.Records + 1:#,##0})");
			// Graba el archivo
			await writer.WriteAsync(fileName, reader, cancellationToken);
		}
	}

	/// <summary>
	///		Obtiene el nombre de archivo de salida
	/// </summary>
	private string GetFileName(string path, ConnectionTableModel table, SolutionManager.FormatType formatType, int? index = null)
	{
		string name = table.Name;

			// Añade el nombre de esquema si existe
			if (!string.IsNullOrWhiteSpace(table.Schema))
				name = $"{table.Schema}.{name}";
			// Añade el índice si es necesario
			if (index != null)
				name += $"_{(index ?? 0).ToString()}";
			// Añade la extensión al nombre de archivo
			switch (formatType)
			{
				case SolutionManager.FormatType.Csv:
						name += ".csv";
					break;
				case SolutionManager.FormatType.Parquet:
						name += ".parquet";
					break;
			}
			// Devuelve el nombre de archivo
			return Path.Combine(path, name);
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	private SolutionManager Manager { get; }

	/// <summary>
	///		Errores
	/// </summary>
	public List<string> Errors { get; } = new();
}
