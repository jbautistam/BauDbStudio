using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibDbProviders.Base;

namespace Bau.Libraries.DbStudio.Application.Controllers.EtlProjects;

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
		IDbProvider provider = Manager.DbScriptsManager.GetDbProvider(Options.Connection);

			// Limpia los errores
			Errors.Clear();
			// Genera los archivos
			if (provider == null)
				Errors.Add($"Cant find provider for connection '{Options.Connection.Name}'");
			else
			{
				EtlFilesGenerator generator = new EtlFilesGenerator(Options.Connection, provider, Options.OutputPath);
				NormalizedDictionary<List<ConnectionTableModel>> schemaTables = GetSchemaTablesSelected();
				int index = 1;

					// Genera el archivo de creación de la base de datos
					generator.WriteFileCreateConnection(Options.DataBaseValidateVariable, Options.MountPathVariable, "00. Create database.sql");
					// Carga el esquema
					await Manager.DbScriptsManager.LoadSchemaAsync(Options.Connection, 
																   new DbScripts.Manager.DbScriptsManager.SchemaOptions(true, true, false, false), 
																   cancellationToken);
					// Prepara los archivos de validación de tablas
					foreach ((string schema, List<ConnectionTableModel> tables) in schemaTables.Enumerate())
						if (!cancellationToken.IsCancellationRequested)
						{
							// Prepara los scripts de validación
							PrepareFilesValidation(generator, schema, tables, true, index);
							PrepareFilesValidation(generator, schema, tables, false, index + 1);
							// Prepara el archivo de validación de QlikView si es necesario
							if (Options.GenerateQvs)
								PrepareFileValidationQvs(generator, schema, tables);
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
							content += generator.GetSqlDropTable(Options.DataBaseValidateVariable, validateTable) + Environment.NewLine;
							content += generator.GetSqlSeparator();
							// Añade la instrucción de crear la tabla
							content += generator.GetSqlCreateTable(Options.DataBaseValidateVariable, validateTable,
																   GetSqlValidation(generator, table, prepareCountTables));
							content += generator.GetSqlSeparator();
							// Prepara la consulta de unión para la tabla de resultados final (sólo en el caso que sea un archivo de validación del número de registros)
							if (prepareCountTables)
							{
								if (!string.IsNullOrEmpty(sqlUnion))
									sqlUnion += "\tUNION ALL" + Environment.NewLine;
								sqlUnion += "\tSELECT '" + validateTable + "' AS Table, Number FROM " + 
												generator.GetSqlTableName(Options.DataBaseValidateVariable, validateTable) + Environment.NewLine;
							}
					}
			// Añade la consulta de unión a la consulta final
			if (!string.IsNullOrWhiteSpace(sqlUnion))
			{
				string countAllTable = $"Validate_Differences";

					// Añade la instrucción de borrar la tabla final
					content += generator.GetSqlDropTable(Options.DataBaseValidateVariable, countAllTable) + Environment.NewLine;
					content += generator.GetSqlSeparator();
					// Añade la instrucción de crear la tabla de diferencias
					content += generator.GetSqlCreateTable(Options.DataBaseValidateVariable, countAllTable, sqlUnion) + Environment.NewLine;
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
				return generator.GetSqlValidateFile(table, prepareCountTables, Options);
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
	///		Obtiene el nombre del archivo de validación de QlikView
	/// </summary>
	private string GetQlikViewValidateFileName(string schema)
	{
		return $"Validate files {schema}.qvs";
	}

	/// <summary>
	///		Prepara el archivo JSON de parámetors
	/// </summary>
	private void PrepareParametersFile(EtlFilesGenerator generator)
	{
		string json = "{" + Environment.NewLine;

			// Añade los parámetros
			json += GetJsonParameter("Constant." + Options.MountPathVariable, Options.MountPathContent) + "," + Environment.NewLine;
			json += GetJsonParameter("Constant." + Options.DataBaseValidateVariable, Options.DataBaseValidateVariable) + "," + Environment.NewLine;
			json += GetJsonParameter("Constant." + Options.DataBaseComputeVariable, Options.DataBaseComputeVariable) + Environment.NewLine;
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

	/// <summary>
	///		Prepara un archivo de validación de QlikView
	/// </summary>
	/// <remarks>
	///		Por ejemplo, para validar una tabla de Spark contra un archivo QVD:
	///	Tiendas:
	///	LOAD Text(CodigoErp1) AS CodigoErp1, 
	///	     Text(NombreTienda) As NombreTienda,
	///	     Flag_Spark;
	///	SQL SELECT CodigoErp1,
	///	    NombreTienda,
	///	    1 As Flag_Spark
	///	FROM SPARK.dbcompute.`Stores`;
	///	Join (Tiendas)
	///	LOAD Text(CodigoErp1) AS CodigoErp1, 
	///	     Text(NombreTienda) As NombreTienda,
	///	     1 As Flag_QV
	///	FROM
	///	\DirectorioQlik\Stores.qvd
	///	(qvd);
	/// </remarks>
	private void PrepareFileValidationQvs(EtlFilesGenerator generator, string schema, List<ConnectionTableModel> tables)
	{
		string content = string.Empty;

			// Crea el contenido de la validación de archivos (con el número de registros distintos)
			foreach (ConnectionTableModel selectedTable in tables)
				foreach (ConnectionTableModel table in Options.Connection.Tables)
					if (selectedTable.FullName.Equals(table.FullName, StringComparison.CurrentCultureIgnoreCase))
					{
						// Cabecera
						content += $"{table.Name}:" + Environment.NewLine;
						// Obtiene la primera tabla para QlikView: la consulta de Spark
						content += GetSqlSelectSparkForQlikView(table);
						// Añade el join
						content += $"Join ({table.Name})" + Environment.NewLine;
						// Obtiene la segunda tabla para QlikView: la consulta sobre archivo
						content += GetSqlQlikView(generator, table) + Environment.NewLine + Environment.NewLine;
					}
			// Graba el archivo
			generator.Save(GetQlikViewValidateFileName(schema), content);
	}

	/// <summary>
	///		Obtiene la SQL de la consulta de la tabla en Spark
	/// </summary>
	private string GetSqlSelectSparkForQlikView(ConnectionTableModel table)
	{
		string sql = "LOAD ";

			// Añade los campos de la tabla
			sql += GetQlikViewFields(table, "Flag_Spark", true) + ";" + Environment.NewLine;
			// Añade la consulta de carga de base de datos
			sql += "SQL SELECT " + GetQlikViewFields(table, "1 AS Flag_Spark", false) + Environment.NewLine;
			// Añade el nombre de tabla
			sql += $" FROM Spark.{Options.DataBaseComputeVariable}.`{table.Name}`;" + Environment.NewLine;
			// Devuelve la cadena
			return sql;
	}

	/// <summary>
	///		Obtiene la SQL de la consulta sobre el archivo de QlikView
	/// </summary>
	private string GetSqlQlikView(EtlFilesGenerator generator, ConnectionTableModel table)
	{
		string sql = "LOAD ";

			// Añade los campos de la tabla
			sql += GetQlikViewFields(table, "1 AS Flag_QV", true) + Environment.NewLine;
			// Añade el nombre de tabla
			sql += $" FROM [{Options.MountPathContent}/{generator.RemovePrefix(table.Name, Options.TablePrefixes)}.qvd] (qvd);" + Environment.NewLine;
			// Devuelve la cadena
			return sql;
	}

	/// <summary>
	///		Obtiene una cadena con los campos
	/// </summary>
	private string GetQlikViewFields(ConnectionTableModel table, string additionalField, bool addText)
	{
		string sql = string.Empty;

			// Añade los campos a la cadena
			foreach (ConnectionTableFieldModel field in table.Fields)
			{
				string fieldText = field.Name;

					// Añade la conversión a texto de QlikView
					if (addText && field.Type == ConnectionTableFieldModel.Fieldtype.Integer)
						fieldText = $"TEXT({fieldText}) AS {fieldText}";
					// Añade el separador
					if (!string.IsNullOrWhiteSpace(sql))
						sql += ", " + Environment.NewLine + "\t";
					// Añade el campo
					sql += fieldText;
			}
			// Añade el campo adicional si es necesario
			if (!string.IsNullOrWhiteSpace(additionalField))
			{
				// Añade el separador
				if (!string.IsNullOrWhiteSpace(sql))
					sql += ", ";
				// Añade el campo adicional
				sql += additionalField;
			}
			// Devuelve la cadnea SQL
			return sql;
	}

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
	public List<string> Errors { get; } = new();
}