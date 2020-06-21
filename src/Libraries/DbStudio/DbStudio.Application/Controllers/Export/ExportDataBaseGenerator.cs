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
		// Enumerados privados
		/// <summary>
		///		Tipo de formato de los archivos de salida
		/// </summary>
		public enum FormatType
		{
			/// <summary>Archivos CSV</summary>
			Csv,
			/// <summary>Archivos parquet</summary>
			Parquet
		}

		public ExportDataBaseGenerator(SolutionManager manager)
		{
			Manager = manager;
		}

		/// <summary>
		///		Exporta las tablas de una conexión a una serie de archivos
		/// </summary>
		public async Task<bool> ExportAsync(BlockLogModel block, ConnectionModel connection, string dataBase, string path, 
											FormatType formatType, long blockSize, CancellationToken cancellationToken)
		{
			// Limpia los errores
			Errors.Clear();
			// Carga el esquema
			await Manager.ConnectionManager.LoadSchemaAsync(connection, cancellationToken);
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
						if (connection.Type == ConnectionModel.ConnectionType.Spark)
							await Task.Run(() => ExportTablePartitioned(block, connection, table, path, formatType, blockSize));
						else
							await Task.Run(() => ExportTable(block, connection, table, path, formatType));
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
		///		Exporta una tabla
		/// </summary>
		private void ExportTable(BlockLogModel block, ConnectionModel connection, ConnectionTableModel table, string path, FormatType formatType)
		{
			IDbProvider provider = Manager.ConnectionManager.GetDbProvider(connection);

				using (IDataReader reader = provider.ExecuteReader($"SELECT * FROM {provider.SqlHelper.FormatName(table.Schema, table.Name)}", 
																   null, CommandType.Text, connection.timeoutExecuteScript))
				{
					switch (formatType)
					{
						case FormatType.Csv:
								ExportToCsv(block, GetFileName(path, table.Name, formatType), reader);
							break;
						case FormatType.Parquet:
								ExportToParquet(block, GetFileName(path, table.Name, formatType), reader);
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
			LibParquetFiles.Writers.ParquetWriter writer = new LibParquetFiles.Writers.ParquetWriter(fileName);

				// Asigna el evento de progreso
				writer.Progress += (sender, args) => block.Progress(System.IO.Path.GetFileName(fileName), args.Records, args.Records + 1);
				// Graba el archivo
				writer.Write(reader);
		}

		/// <summary>
		///		Exporta una tabla particionando la consulta en varios <see cref="IDataReader"/> (porque por ejemplo spark carga todo el dataReader en memoria
		///	y da un error de OutOfMemory)
		/// </summary>
		private void ExportTablePartitioned(BlockLogModel block, ConnectionModel connection, ConnectionTableModel table, string path, FormatType formatType, long blockSize)
		{
			IDbProvider provider = Manager.ConnectionManager.GetDbProvider(connection);
			string fileName = GetFileName(path, table.Name, formatType);

				using (LibParquetFiles.Writers.ParquetListWriter writer = new LibParquetFiles.Writers.ParquetListWriter(fileName))
				{
					// Asigna el evento de progreso
					writer.Progress += (sender, args) => block.Progress(fileName, args.Records, args.Records + 1);
					// Graba el archivo
					writer.Write(GetPaginatedData(block, provider, connection, table, blockSize));
				}
		}

		/// <summary>
		///		Obtiene los <see cref="IDataReader"/> de la consulta particionada
		/// </summary>
		private IEnumerable<IDataReader> GetPaginatedData(BlockLogModel block, IDbProvider provider, ConnectionModel connection, ConnectionTableModel table, long blockSize)
		{
			long records = provider.GetRecordsCount($"SELECT * FROM {provider.SqlHelper.FormatName(table.Schema, table.Name)}", 
													null, connection.timeoutExecuteScript) ?? 0;

				if (records <= blockSize)
					yield return provider.ExecuteReader($"SELECT * FROM {provider.SqlHelper.FormatName(table.Schema, table.Name)}", 
														null, CommandType.Text, connection.timeoutExecuteScript);
				else
				{
					int actualPage = 0;

						// Obtiene los datos paginados
						do
						{
							//Log
							block.Info($"Reading page {actualPage + 1} / {records / blockSize + 1:#,##0} ({records:#,##0}) from table {table.Name}");
							// Obtiene la consulta paginada
							yield return provider.ExecuteReader(GetSparkPaginatedSql(provider, table, actualPage++, blockSize), 
																null, CommandType.Text, connection.timeoutExecuteScript);
						}
						while (actualPage * blockSize < records);
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
		///		Obtiene el nombre de archivo de salida
		/// </summary>
		private string GetFileName(string path, string name, FormatType formatType)
		{
			// Añade la extensión al nombre de archivo
			switch (formatType)
			{
				case FormatType.Csv:
						name += ".csv";
					break;
				case FormatType.Parquet:
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
