using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibDbProviders.Base;

namespace Bau.Libraries.DbStudio.Application.Controllers.EtlProjects
{
	/// <summary>
	///		Generador de los archivos de validación
	/// </summary>
	public class ScriptsValidationGenerator
	{
		public ScriptsValidationGenerator(SolutionManager manager, ScriptsValidationOptions options)
		{
			Manager = manager;
			Options = options;
		}

		/// <summary>
		///		Genera los archivos
		/// </summary>
		public async Task<bool> GenerateAsync(CancellationToken cancellationToken)
		{
			IDbProvider provider = Manager.ConnectionManager.GetDbProvider(Options.Connection);

				if (provider == null)
					Errors.Add($"Cant find provider for connection '{Options.Connection.Name}'");
				else
				{
					EtlFilesGenerator generator = new EtlFilesGenerator(Options.Connection, provider, Options.OutputPath);
					NormalizedDictionary<List<ConnectionTableModel>> schemaTables = GetSchemaTablesSelected();
					int index = 1;

						// Limpia los errores
						Errors.Clear();
						// Genera el archivo de creación de la base de datos
						generator.WriteFileCreateConnection(Options.DataBaseVariable, Options.MountPathVariable, "00. Create database.sql");
						// Carga el esquema
						await Manager.ConnectionManager.LoadSchemaAsync(Options.Connection, cancellationToken);
						// Prepara los archivos de validación de tablas
						foreach ((string schema, List<ConnectionTableModel> tables) in schemaTables.Enumerate())
							if (!cancellationToken.IsCancellationRequested)
							{
								// Prepara los scripts de validación
								PrepareFilesValidation(generator, schema, tables, true, index);
								PrepareFilesValidation(generator, schema, tables, false, index + 1);
								// Incrementa el índice
								index += 2;
							}
						// Prepara el archivo JSON de parámetros
						PrepareParametersFile(generator);
				}
				// Devuelve el valor que indica si la generación ha sido correcta
				return Errors.Count == 0;
		}

		/// <summary>
		///		Prepara los archivos de validación por esquema contando el número de registros
		/// </summary>
		private void PrepareFilesValidation(EtlFilesGenerator generator, string schema, List<ConnectionTableModel> tables, bool prepareCountTables, int index)
		{
			string content = string.Empty;
			string sqlUnion = string.Empty;

				// Crea el contenido de la validación de archivos (con el número de registros distintos)
				foreach (ConnectionTableModel selectedTable in tables)
					foreach (ConnectionTableModel table in Options.Connection.Tables)
						if (selectedTable.FullName.Equals(table.FullName, StringComparison.CurrentCultureIgnoreCase))
						{
							string validateTable = GetValidateTableName(table, prepareCountTables);

								// Añade la instrucción de borrar la tabla
								content += generator.GetSqlDropTable(Options.DataBaseVariable, validateTable) + Environment.NewLine;
								content += generator.GetSqlSeparator();
								// Añade la instrucción de crear la tabla
								content += content += generator.GetSqlCreateTable(Options.DataBaseVariable, validateTable,
																				  GetSqlValidation(generator, table, prepareCountTables));
								content += generator.GetSqlSeparator();
								// Prepara la consulta de unión para la tabla de resultados final (sólo en el caso que sea un archivo de validación del número de registros)
								if (prepareCountTables)
								{
									if (!string.IsNullOrEmpty(sqlUnion))
										sqlUnion += "\tUNION ALL" + Environment.NewLine;
									sqlUnion += "\tSELECT '" + validateTable + "' AS Table, Number FROM " + 
													generator.GetSqlTableName(Options.DataBaseVariable, validateTable) + Environment.NewLine;
								}
						}
				// Añade la consulta de unión a la consulta final
				if (!string.IsNullOrWhiteSpace(sqlUnion))
				{
					string countAllTable = $"Validate_Differences";

						// Añade la instrucción de borrar la tabla final
						content += generator.GetSqlDropTable(Options.DataBaseVariable, countAllTable) + Environment.NewLine;
						content += generator.GetSqlSeparator();
						// Añade la instrucción de crear la tabla de diferencias
						content += generator.GetSqlCreateTable(Options.DataBaseVariable, countAllTable, sqlUnion) + Environment.NewLine;
						content += generator.GetSqlSeparator();
				}
				// Graba el archivo
				generator.Save(GetValidateFileName(schema, index, prepareCountTables), content);
		}

		/// <summary>
		///		Obtiene la cadena SQL de validación de tablas
		/// </summary>
		private string GetSqlValidation(EtlFilesGenerator generator, ConnectionTableModel table, bool prepareCountTables)
		{
			switch (Options.Mode)
			{
				case ScriptsValidationOptions.ValidationMode.Files:
					return generator.GetSqlValidateFile(table, Options.MountPathVariable,
														Options.SubpathValidate, Options.FormatType, prepareCountTables);
				default:
					throw new Exception("Not implemented");
			}
		}

		/// <summary>
		///		Obtiene el nombre de la tabla de validación
		/// </summary>
		private string GetValidateTableName(ConnectionTableModel table, bool prepareCountTables)
		{
			if (prepareCountTables)
				return $"{table.Name}_Validate_Count";
			else
				return $"{table.Name}_Validate";
		}

		/// <summary>
		///		Obtiene el nombre del archivo de validación
		/// </summary>
		private string GetValidateFileName(string schema, int index, bool prepareCountTables)
		{
			if (prepareCountTables)
				return $"{(index * 10).ToString()} Validate number {schema}.sql";
			else
				return $"{(index * 10).ToString()} Validate fields {schema}.sql";
		}

		/// <summary>
		///		Prepara el archivo JSON de parámetors
		/// </summary>
		private void PrepareParametersFile(EtlFilesGenerator generator)
		{
			string json = "{" + Environment.NewLine;

				// Añade los parámetros
				json += GetJsonParameter("Constant." + Options.MountPathVariable, Options.MountPathContent) + "," + Environment.NewLine;
				json += GetJsonParameter("Constant." + Options.DataBaseVariable, Options.DataBaseVariable) + Environment.NewLine;
				// Cierra el JSON
				json += "}";
				// y graba el archivo
				generator.Save("Parameters.json", json);
		}

		/// <summary>
		///		Obtiene la cadena con un parámetro JSON
		/// </summary>
		private string GetJsonParameter(string name, string value)
		{
			return $"\t\"{name}\": \"{value}\"";
		}

/*
		/// <summary>
		///		Prepara los archivos de validación por esquema contando el número de registros
		/// </summary>
		private void PrepareFilesValidationCount(EtlFilesGenerator generator, ConnectionModel connection, 
												 string dataBaseVariable, string mountPathVariable, string pathValidate,
												 string schema, List<ConnectionTableModel> tables, int index)
		{
			string content = string.Empty;
			string sqlUnion = string.Empty;

				// Crea el contenido de la validación de archivos (con el número de registros distintos)
				foreach (ConnectionTableModel selectedTable in tables)
					foreach (ConnectionTableModel table in connection.Tables)
						if (selectedTable.FullName.Equals(table.FullName, StringComparison.CurrentCultureIgnoreCase))
						{
							string validateTable = $"{table.Name}_Validate_Count";

								// Añade la instrucción de borrar la tabla
								content += generator.GetSqlDropTable(dataBaseVariable, validateTable) + Environment.NewLine;
								content += generator.GetSqlSeparator();
								// Añade la instrucción de crear la tabla
								content += generator.GetSqlCreateTable(dataBaseVariable, validateTable,
																	   generator.GetSqlValidateFile(table, mountPathVariable, pathValidate, FormatType, true) + Environment.NewLine);
								content += generator.GetSqlSeparator();
								// Prepara la consulta de unión para la tabla de resultados final
								if (!string.IsNullOrEmpty(sqlUnion))
									sqlUnion += "\tUNION ALL" + Environment.NewLine;
								sqlUnion += "\tSELECT '" + validateTable + "' AS Table, Number FROM {{DbCompute}}.`" + validateTable + "`" + Environment.NewLine;
						}
				// Añade la consulta de unión a la consulta final
				if (!string.IsNullOrWhiteSpace(sqlUnion))
				{
					string countAllTable = $"Validate_Differences";

						// Añade la instrucción de borrar la tabla final
						content += generator.GetSqlDropTable(dataBaseVariable, countAllTable) + Environment.NewLine;
						content += generator.GetSqlSeparator();
						// Añade la instrucción de crear la tabla de diferencias
						content += generator.GetSqlCreateTable(dataBaseVariable, countAllTable, sqlUnion) + Environment.NewLine;
						content += generator.GetSqlSeparator();
				}
				// Graba el archivo
				generator.Save($"{(index * 10).ToString()} Validate number {schema}.sql", content);
		}

		/// <summary>
		///		Prepara los archivos de validación por esquema comparando campos
		/// </summary>
		private void PrepareFilesValidationCompare(EtlFilesGenerator generator, ConnectionModel connection, string dataBaseVariable, 
												   string mountPathVariable, string pathValidate, 
												   string schema, List<ConnectionTableModel> tables, int index)
		{
			string content = string.Empty;

				// Crea el contenido de la validación de archivos (con el número de registros distintos)
				foreach (ConnectionTableModel selectedTable in tables)
					foreach (ConnectionTableModel table in connection.Tables)
						if (selectedTable.FullName.Equals(table.FullName, StringComparison.CurrentCultureIgnoreCase))
						{
							string validateTable = $"{table.Name}_Validate";

								// Añade la instrucción de borrar la tabla
								content += generator.GetSqlDropTable(dataBaseVariable, validateTable) + Environment.NewLine;
								content += generator.GetSqlSeparator();
								// Añade la instrucción de crear la tabla
								content += generator.GetSqlCreateTable(dataBaseVariable, validateTable,
																	   generator.GetSqlValidateFile(table, mountPathVariable, pathValidate, FormatType, false) + Environment.NewLine);
								content += generator.GetSqlSeparator();
						}
				// Graba el archivo
				generator.Save($"{(index * 10).ToString()} Validate fields {schema}.sql", content);
		}
*/
		/// <summary>
		///		Obtiene un diccionario con los esquemas y tablas seleccionados
		/// </summary>
		private NormalizedDictionary<List<ConnectionTableModel>> GetSchemaTablesSelected()
		{
			NormalizedDictionary<List<ConnectionTableModel>> tables = new NormalizedDictionary<List<ConnectionTableModel>>();

				// Añade los esquemas a la lista
				foreach (ConnectionTableModel table in Options.Tables)
				{
					// Si no existe el esquema en el diccionario, lo añade
					if (!tables.ContainsKey(table.Schema))
						tables.Add(table.Schema, new List<ConnectionTableModel>());
					// Añade la tabla a la lista
					tables[table.Schema].Add(table);
				}
				// Devuelve el diccionario de tablas por esquema
				return tables;
		}

		/// <summary>
		///		Controlador de la solución
		/// </summary>
		public SolutionManager Manager { get; }

		/// <summary>
		///		Parámetros de validación
		/// </summary>
		public ScriptsValidationOptions Options { get; }

		/// <summary>
		///		Errores de la generación
		/// </summary>
		public List<string> Errors { get; } = new List<string>();
	}
}