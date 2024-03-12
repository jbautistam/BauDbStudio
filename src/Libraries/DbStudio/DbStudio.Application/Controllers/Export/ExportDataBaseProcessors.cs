using System.Data;
using Microsoft.Extensions.Logging;
using System.Data.Common;

using Bau.Libraries.LibHelper.Extensors;
using DbBase = Bau.Libraries.LibDbProviders.Base.Models;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.DbScripts.Manager.Models;
using Bau.Libraries.LibCsvFiles.Models;

namespace Bau.Libraries.DbStudio.Application.Controllers.Export;

/// <summary>
///		Exportación de archivos de base de datos
/// </summary>
public class ExportDataBaseProcessors
{
	// Eventos
	public event EventHandler<EventArguments.ProgressEventArgs>? Progress;

	public ExportDataBaseProcessors(SolutionManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Exporta una tabla particionando la consulta en varios <see cref="IDataReader"/> (porque por ejemplo spark carga todo el dataReader en memoria
	///	y da un error de OutOfMemory)
	/// </summary>
	public async Task ExportTableAsync(ConnectionModel connection, ConnectionTableModel table, string path, 
									   SolutionManager.FormatType formatType, long blockSize, CsvFileParameters csvFileParameters, 
									   CancellationToken cancellationToken)
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
					await ExportQueryAsync(connection, 
										   new DbBase.QueryModel(sql, DbBase.QueryModel.QueryType.Text, connection.TimeoutExecuteScript), 
										   fileName, formatType, csvFileParameters, cancellationToken);
			}
	}

	/// <summary>
	///		Exporta el resultado de una consulta a un archivo
	/// </summary>
	public async Task ExportQueryAsync(QueryModel query, string fileName, SolutionManager.FormatType formatType, long blockSize,
									   CsvFileParameters csvFileParameters, CancellationToken cancellationToken)
	{
		await ExportQueryAsync(query.Connection, 
							   new DbBase.QueryModel(query.Sql, DbBase.QueryModel.QueryType.Text, query.Connection.TimeoutExecuteScript),
							   fileName, formatType, csvFileParameters, cancellationToken);
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
			return $"""
						SELECT {sqlFields}
							FROM (SELECT Row_Number() OVER (ORDER BY 1 ASC) AS {nameRowId}, {sqlFields} 
									FROM {provider.SqlHelper.FormatName(table.Schema, table.Name)}) AS tmp
							WHERE {nameRowId} BETWEEN {actualPage * blockSize + 1} AND {(actualPage + 1) * blockSize}
					""";
	}

	/// <summary>
	///		Exporta una tabla
	/// </summary>
	private async Task ExportQueryAsync(ConnectionModel connection, DbBase.QueryModel query, string fileName, 
										SolutionManager.FormatType formatType, CsvFileParameters csvFileParameters, CancellationToken cancellationToken)
	{
		using (DbDataReader reader = await Manager.DbScriptsManager.GetDbProvider(connection).ExecuteReaderAsync(query, cancellationToken))
		{
			switch (formatType)
			{
				case SolutionManager.FormatType.Csv:
						ExportToCsv(fileName, reader, csvFileParameters);
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
	private void ExportToCsv(string fileName, IDataReader reader, CsvFileParameters csvFileParameters)
	{
		LibCsvFiles.Controllers.CsvDataReaderWriter writer = new LibCsvFiles.Controllers.CsvDataReaderWriter(csvFileParameters.GetFileModel());
		
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
