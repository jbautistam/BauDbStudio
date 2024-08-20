using System.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibDbProviders.Base;

namespace Bau.Libraries.DbStudio.Application.Controllers.Export;

/// <summary>
///		Procesador para importación de archivos
/// </summary>
public class ImportFileProcessor
{
	// Eventos
	public event EventHandler<EventArguments.ProgressEventArgs>? Progress;

	public ImportFileProcessor(SolutionManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Importa los datos
	/// </summary>
	public async Task ImportAsync(ConnectionModel connection, string fileName, ConnectionTableModel table, List<(string field, string fileField)> mappings, 
								  long blockSize, CsvFileParameters csvFileParameters, CancellationToken cancellationToken)
	{
		long record = 0;

			using (IDbProvider provider = Manager.DbScriptsManager.GetDbProvider(connection))
			{
				System.Text.StringBuilder builder = new();

					// Abre la conexión
					await provider.OpenAsync(cancellationToken);
					// Importa los datos
					using (IDataReader reader = await GetDataReaderAsync(fileName, table, mappings, csvFileParameters, cancellationToken))
					{
						// Guarda los registros en la cadena de inserción
						while (reader.Read())
						{
							// Incrementa al número de registros
							record++;
							// Crea la SQL con los datos del registro
							builder.AppendLine(GetSql(reader, table, mappings));
							// Inserta los datos cuando se llega al límite
							if (builder.Length > blockSize)
							{
								// Ejecuta la cadena
								await provider.ExecuteAsync(new LibDbProviders.Base.Models.QueryModel(builder.ToString(), 
																									  LibDbProviders.Base.Models.QueryModel.QueryType.Text), 
															cancellationToken);
								// Limpia la cadena
								builder.Clear();
								// Lanza el evento
								Progress?.Invoke(this, new EventArguments.ProgressEventArgs(record, record + 1));
							}
						}
						// Ejecuta la cadena restante
						if (builder.Length > 0)
							await provider.ExecuteAsync(new LibDbProviders.Base.Models.QueryModel(builder.ToString(), 
																								  LibDbProviders.Base.Models.QueryModel.QueryType.Text), 
														cancellationToken);
					}
			}
	}

	/// <summary>
	///		Obtiene la cadena SQL
	/// </summary>
	private string GetSql(IDataReader reader, ConnectionTableModel table, List<(string field, string fileField)> mappings)
	{
		return $"INSERT INTO {table.FullName} ({GetSqlHeader(mappings)}) VALUES ({GetSqlValues(reader, table, mappings)})";

			// Obtiene la cadena SQL de la cabecera del INSERT
			string GetSqlHeader(List<(string field, string fileField)> mappings)
			{
				string sql = string.Empty;

					// Añade los nombres de campos
					foreach ((string field, string fileField) in mappings)
						sql = sql.AddWithSeparator($"[{field}]", ",");
					// Devuelve la cadena SQL
					return sql;
			}

			// Obtiene la cadena SQL con los valores del INSERT
			string GetSqlValues(IDataReader reader, ConnectionTableModel table, List<(string field, string fileField)> mappings)
			{
				string sql = string.Empty;

					// Añade los nombres de campos
					foreach ((string field, string fileField) in mappings)
					{
						object? value = reader.GetValue(reader.GetOrdinal(fileField));

							if (value is null || value is DBNull)
								sql = sql.AddWithSeparator("NULL", ",");
							else
								sql = sql.AddWithSeparator(GetSqlValue(table, field, value?.ToString()), ",");
					}
					// Devuelve la cadena SQL
					return sql;
			}

			// Obtiene un valor SQL
			string GetSqlValue(ConnectionTableModel table, string field, string? value)
			{
				ConnectionTableFieldModel? tableField = table.Fields.FirstOrDefault(item => item.Name.Equals(field, StringComparison.CurrentCultureIgnoreCase));
				string sql = "NULL";

					if (value is not null && tableField is not null)
						switch (tableField.Type)
						{
							case ConnectionTableFieldModel.Fieldtype.Date:
									DateTime? dateValue = value.GetDateTime();

										if (dateValue is not null)
											sql = $"'{dateValue:yyyy-MM-dd HH:mm:ss}'";
								break;
							case ConnectionTableFieldModel.Fieldtype.Integer:
									long? longValue = value.GetLong();

										if (longValue is not null)
											sql = longValue.ToString() ?? "NULL";
								break;
							case ConnectionTableFieldModel.Fieldtype.Decimal:
									double? doubleValue = value.GetDouble();

										if (doubleValue is not null)
											sql = (doubleValue ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "NULL";
								break;
							case ConnectionTableFieldModel.Fieldtype.Boolean:
									if (value == "1" || value.Equals("TRUE", StringComparison.CurrentCultureIgnoreCase))
										sql = "1";
									else
										sql = "0";
								break;
							default:
									sql = $"'{value.Replace("'", "''")}'";
								break;
						}
					// Devuelve la cadena SQL
					return sql;
			}
	}

	/// <summary>
	///		Obtiene el <see cref="IDataReader"/> sobre un archivo
	/// </summary>
	private async Task<IDataReader> GetDataReaderAsync(string fileName, ConnectionTableModel table, List<(string field, string fileField)> mappings, 
													   CsvFileParameters csvFileParameters, CancellationToken cancellationToken)
	{
		if (fileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
			return GetCsvReader(fileName, table, mappings, csvFileParameters);
		else if (fileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
			return await GetParquetReaderAsync(fileName, cancellationToken);
		else
			throw new NotImplementedException($"Undefined reader for {fileName}");
	}

	/// <summary>
	///		Obtiene un <see cref="IDataReader"/> sobre un archivo CSV
	/// </summary>
	private IDataReader GetCsvReader(string fileName, ConnectionTableModel table, List<(string field, string fileField)> mappings, CsvFileParameters csvFileParameters)
	{
		LibCsvFiles.CsvReader reader = new LibCsvFiles.CsvReader(csvFileParameters.GetFileModel(), null);

			// Añade las columnas
			reader.Columns.AddRange(GetColumns(table, mappings));
			// Abre el archivo
			reader.Open(fileName);
			// Devuelve el lector
			return reader;

			// Obtiene las columnas
			List<LibCsvFiles.Models.ColumnModel> GetColumns(ConnectionTableModel table, List<(string field, string fileField)> mappings)
			{
				List<LibCsvFiles.Models.ColumnModel> columns = [];

					// Obtiene las columnas que están mapeadas y sus tipos
					foreach ((string field, string fileField) in mappings)
					{
						ConnectionTableFieldModel? tableField = table.Fields.FirstOrDefault(item => item.Name.Equals(field, StringComparison.CurrentCultureIgnoreCase));

							if (tableField is not null)
							{
								LibCsvFiles.Models.ColumnModel.ColumnType type = LibCsvFiles.Models.ColumnModel.ColumnType.String;

									// Convierte el tipo
									switch (tableField.Type)
									{
										case ConnectionTableFieldModel.Fieldtype.Date:
												type = LibCsvFiles.Models.ColumnModel.ColumnType.DateTime;
											break;
										case ConnectionTableFieldModel.Fieldtype.Integer:
												type = LibCsvFiles.Models.ColumnModel.ColumnType.Integer;
											break;
										case ConnectionTableFieldModel.Fieldtype.Decimal:
												type = LibCsvFiles.Models.ColumnModel.ColumnType.Decimal;
											break;
										case ConnectionTableFieldModel.Fieldtype.Boolean:
												type = LibCsvFiles.Models.ColumnModel.ColumnType.Boolean;
											break;
									}
									// Añade la columna
									columns.Add(new LibCsvFiles.Models.ColumnModel
															{
																Name = fileField,
																Type = type
															}
												);
							}
					}
					// Devuelve la lista de columnas
					return columns;
			}
	}

	/// <summary>
	///		Obtiene un <see cref="IDataReader"/> sobre un archivo parquet
	/// </summary>
	private async Task<IDataReader> GetParquetReaderAsync(string fileName, CancellationToken cancellationToken)
	{
		LibParquetFiles.Readers.ParquetDataReader reader = new();

			// Abre el archivo
			await reader.OpenAsync(fileName, cancellationToken);
			// Devuelve el lector
			return reader;
	}

	/// <summary>
	///		Manager de conexiones
	/// </summary>
	public SolutionManager Manager { get; }
}
