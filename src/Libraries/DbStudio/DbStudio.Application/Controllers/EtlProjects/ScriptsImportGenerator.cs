using System.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Extensors;

namespace Bau.Libraries.DbStudio.Application.Controllers.EtlProjects;

/// <summary>
///		Generador de los archivos SQL de importación
/// </summary>
public class ScriptsImportGenerator
{
	public ScriptsImportGenerator(SolutionManager manager, ScriptsImportOptions options)
	{
		Manager = manager;
		Options = options;
	}

	/// <summary>
	///		Genera los archivos
	/// </summary>
	public async Task<bool> GenerateAsync(CancellationToken cancellationToken)
	{
		IDbProvider provider = Manager.DbScriptsManager.GetDbProvider(Options.Connection);

			// Evita el warning
			await Task.Delay(1);
			// Limpia los errores
			Errors.Clear();
			// Genera el archivo
			if (provider == null)
				Errors.Add($"Cant find provider for connection '{Options.Connection.Name}'");
			else
			{
				EtlFilesGenerator generator = new EtlFilesGenerator(Options.Connection, provider, Path.GetDirectoryName(Options.OutputFileName));
				string content = string.Empty;

					// Recorre los archivos
					foreach (string fileName in Directory.GetFiles(Options.PathInputFiles, "*.parquet"))
						content += await PrepareImportAsync(generator, fileName, cancellationToken);
					// Graba el archivo
					generator.Save(Path.GetFileName(Options.OutputFileName), content);
			}
			// Devuelve el valor que indica si la generación ha sido correcta
			return Errors.Count == 0;
	}

	/// <summary>
	///		Prepara las importaciones del archivo parquet
	/// </summary>
	private async Task<string> PrepareImportAsync(EtlFilesGenerator generator, string fileName, CancellationToken cancellationToken)
	{
		string tableName = $"{Options.PrefixOutputTable}{Path.GetFileNameWithoutExtension(fileName)}";
		string content = string.Empty;
		
			// Instrucción de borrar la tabla
			content += generator.GetSqlDropTable(Options.DataBaseVariable, tableName) + Environment.NewLine;
			content += generator.GetSqlSeparator();
			// Instrucción de crear la tabla
			content += generator.GetSqlCreateTable(Options.DataBaseVariable, tableName,
												   await GetSqlReadFileAsync(generator, fileName, cancellationToken));
			content += generator.GetSqlSeparator();
			// Devuelve el contenido del script
			return content;
	}

	/// <summary>
	///		Obtiene la cadena SQL de lectura de archivo
	/// </summary>
	private async Task<string> GetSqlReadFileAsync(EtlFilesGenerator generator, string fileName, CancellationToken cancellationToken)
	{
		(DataTable table, long records) = await new LibParquetFiles.Readers.ParquetDataTableReader().LoadAsync(fileName, 0, 50, false, null, cancellationToken);
		string sql = string.Empty;
			
			// Si no hay filas para inferir el tipo, añade un comentario
			if (records == 0)
				sql += "-- Can't infer the data types" + Environment.NewLine;
			// Añade la consulta para leer el archivo
			sql += "\tSELECT " + GetSqlTransformFieldNames(table, "\t\t\t") + Environment.NewLine;
			sql += "\t\tFROM " + generator.GetFileNameTable(Options.MountPathVariable, Options.SubPath, 
															Path.GetFileNameWithoutExtension(fileName), SolutionManager.FormatType.Parquet,
															string.Empty);
			sql += Environment.NewLine;
			// Devuelve la consulta SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL que obtiene y transforma los nombres de campos dependiendo de su tipo: los string los transforma en TRIM(), las fechas en CAST(), etc...
	/// </summary>
	private string GetSqlTransformFieldNames(DataTable table, string indent)
	{
		string sql = string.Empty;
		int lastLength = 0;

			foreach (DataColumn column in table.Columns)
			{
				DataTableExtensors.FieldType type = column.GetFieldType();

					// Añade el separador
					if (!string.IsNullOrWhiteSpace(sql))
						sql += ", ";
					// Pasa el nombre del campo a otra línea si es necesario
					if (!string.IsNullOrEmpty(sql) && sql.Length - lastLength >= 80)
					{
						// Añade el salto de línea
						sql += Environment.NewLine + indent;
						// Guarda la longitud actual
						lastLength = sql.Length;
					}
					// Añade la transformación del campo
					//if (table.Rows.Count == 0)
					//{
					//	if (column.ColumnName.EndsWith("Id", StringComparison.CurrentCultureIgnoreCase))
					//		sql += $"CAST(`{column.ColumnName}` AS int) AS `{column.ColumnName}`";
					//	else if (column.ColumnName.IndexOf("Fecha", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
					//			 column.ColumnName.IndexOf("Date", StringComparison.CurrentCultureIgnoreCase) >= 0)
					//		sql += $"CAST(`{column.ColumnName}` AS date) AS `{column.ColumnName}`";
					//	else
					//		sql += $"`{column.ColumnName}`";
					//}
					//else
						switch (type)
						{
							case DataTableExtensors.FieldType.String:
									switch (GetInferedType(table, column))
									{
										case DataTableExtensors.FieldType.String:
												sql += $"TRIM(`{column.ColumnName}`) AS `{column.ColumnName}`";
											break;
										case DataTableExtensors.FieldType.Datetime:
												sql += $"CAST(`{column.ColumnName}` AS date) AS `{column.ColumnName}`";
											break;
										case DataTableExtensors.FieldType.Decimal:
												sql += $"CAST(`{column.ColumnName}` AS float) AS `{column.ColumnName}`";
											break;
										case DataTableExtensors.FieldType.Integer:
												sql += $"CAST(`{column.ColumnName}` AS int) AS `{column.ColumnName}`";
											break;
										default:
												sql += $"`{column.ColumnName}`";
											break;
									}
								break;
							default:
									sql += $"`{column.ColumnName}`";
								break;
						}
			}
			// Devuelve la cadena de campos
			return sql;
	}

	/// <summary>
	///		Infiere el tipo
	/// </summary>
	private DataTableExtensors.FieldType GetInferedType(DataTable table, DataColumn column)
	{
		int numberString = 0, numberInteger = 0, numberDecimal = 0, numberDate = 0, numberNull = 0;

			// Recorre las filas buscando el número de elementos localizados
			foreach (DataRow row in table.Rows)
				if (row[column] is DBNull || row[column] == null)
					numberNull++;
				else 
				{
					string? value = row[column]?.ToString();

						// Obtiene el tipo
						if (string.IsNullOrWhiteSpace(value))
							numberString++;
						else if (value.GetDouble() != null)
						{
							if (value.IndexOf(',') >= 0 || value.IndexOf('.') >= 0)
								numberDecimal++;
							else
								numberInteger++;
						}
						else if (value.GetDateTime() != null)
							numberDate++;
						else
							numberString++;
				}
			// Devuelve el tipo inferido
			if (numberNull == table.Rows.Count)
				return DataTableExtensors.FieldType.Unknown;
			else if (numberString > 0)
				return DataTableExtensors.FieldType.String;
			else if (numberDate > 0)
				return DataTableExtensors.FieldType.Datetime;
			else if (numberDecimal > 0)
				return DataTableExtensors.FieldType.Decimal;
			else if (numberInteger > 0)
				return DataTableExtensors.FieldType.Integer;
			else
				return DataTableExtensors.FieldType.String;
	}

	/// <summary>
	///		Controlador de la solución
	/// </summary>
	public SolutionManager Manager { get; }

	/// <summary>
	///		Parámetros de importación
	/// </summary>
	public ScriptsImportOptions Options { get; }

	/// <summary>
	///		Errores de la generación
	/// </summary>
	public List<string> Errors { get; } = new();
}