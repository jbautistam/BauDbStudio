using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibDbProviders.Base;

namespace Bau.Libraries.DbStudio.Application.Controllers.Export
{
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
		public async Task<bool> ExportAsync(BlockLogModel block, ConnectionModel connection, string dataBase, string path, 
											SolutionManager.FormatType formatType, long blockSize, CancellationToken cancellationToken)
		{
			// Limpia los errores
			Errors.Clear();
			// Carga el esquema
			await Manager.DbScriptsManager.LoadSchemaAsync(connection, cancellationToken);
			// Crea el directorio
			LibHelper.Files.HelperFiles.MakePath(path);
			// Genera los archivos
			foreach (ConnectionTableModel table in connection.Tables)
				if (!cancellationToken.IsCancellationRequested &&
						(string.IsNullOrWhiteSpace(dataBase) || dataBase.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase)))
					try
					{
						// Log
						block.Info($"Start export table {table.FullName}");
						// Exporta la tabla
						await Task.Run(() => ExportTable(block, connection, table, path, formatType, blockSize));
						// Log
						block.Info($"End export table {table.FullName}");
					}
					catch (Exception exception)
					{
						block.Error($"Error when export {table.FullName}", exception);
					}
			// Devuelve el valor que indica si la exportación ha sido correcta
			return Errors.Count == 0;
		}

		/// <summary>
		///		Exporta una tabla particionando la consulta en varios <see cref="IDataReader"/> (porque por ejemplo spark carga todo el dataReader en memoria
		///	y da un error de OutOfMemory)
		/// </summary>
		private void ExportTable(BlockLogModel block, ConnectionModel connection, ConnectionTableModel table, string path, 
								 SolutionManager.FormatType formatType, long blockSize)
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
							fileName = GetFileName(path, table.Name, formatType);
						else
							fileName = GetFileName(path, table.Name, formatType, actualPage + 1);
						// Graba el archivo de la página adecuada
						if (totalPages == 1)
							sql = $"SELECT * FROM {provider.SqlHelper.FormatName(table.Schema, table.Name)}";
						else
							sql = GetSparkPaginatedSql(provider, table, actualPage, blockSize);
						//Log
						block.Info($"Reading page {actualPage + 1} / {records / blockSize + 1:#,##0} ({records:#,##0}) from table {table.Name}");
						// Exporta la tabla
						ExportTable(block, provider, sql, fileName, formatType, connection.TimeoutExecuteScript);
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
		private void ExportTable(BlockLogModel block, IDbProvider provider, string sql, string fileName, SolutionManager.FormatType formatType, TimeSpan timeout)
		{
			using (IDataReader reader = provider.ExecuteReader(sql, null, CommandType.Text, timeout))
			{
				switch (formatType)
				{
					case SolutionManager.FormatType.Csv:
							ExportToCsv(block, fileName, reader);
						break;
					case SolutionManager.FormatType.Parquet:
							ExportToParquet(block, fileName, reader);
						break;
				}
			}
		}

		/// <summary>
		///		Exporta la tabla a CSV
		/// </summary>
		private void ExportToCsv(BlockLogModel block, string fileName, IDataReader reader)
		{
			LibCsvFiles.Controllers.CsvDataReaderWriter writer = new LibCsvFiles.Controllers.CsvDataReaderWriter();
			
				// Asigna el evento de progreso
				writer.Progress += (sender, args) => block.Progress(System.IO.Path.GetFileName(fileName), args.Records, args.Records + 1);
				// Graba el archivo
				writer.Save(reader, fileName);
		}

		/// <summary>
		///		Exporta la tabla a parquet
		/// </summary>
		private void ExportToParquet(BlockLogModel block, string fileName, IDataReader reader)
		{
			LibParquetFiles.Writers.ParquetWriter writer = new LibParquetFiles.Writers.ParquetWriter();

				// Asigna el evento de progreso
				writer.Progress += (sender, args) => block.Progress(System.IO.Path.GetFileName(fileName), args.Records, args.Records + 1);
				// Graba el archivo
				writer.Write(fileName, reader);
		}

		/// <summary>
		///		Obtiene el nombre de archivo de salida
		/// </summary>
		private string GetFileName(string path, string name, SolutionManager.FormatType formatType, int? index = null)
		{
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
			return System.IO.Path.Combine(path, name);
		}

		/// <summary>
		///		Manager principal
		/// </summary>
		private SolutionManager Manager { get; }

		/// <summary>
		///		Errores
		/// </summary>
		public List<string> Errors { get; } = new List<string>();
	}
}
