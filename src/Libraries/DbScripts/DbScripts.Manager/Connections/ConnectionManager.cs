using Bau.Libraries.LibDbProviders.Spark;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibDbProviders.Base.Schema;
using Bau.Libraries.LibDbProviders.Base;

namespace Bau.Libraries.DbScripts.Manager.Connections;

/// <summary>
///		Manager de conexiones
/// </summary>
internal class ConnectionManager
{
	internal ConnectionManager(DbScriptsManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Obtiene el esquema de la conexión
	/// </summary>
	internal async Task<SchemaDbModel> GetSchemaAsync(ConnectionModel connection, bool includeSystemTables, CancellationToken cancellationToken)
	{
		return await GetDbProvider(connection).GetSchemaAsync(includeSystemTables, TimeSpan.FromMinutes(5), cancellationToken);
	}

	/// <summary>
	///		Carga el esquema de la conexión
	/// </summary>
	internal async Task LoadSchemaAsync(ConnectionModel connection, bool includeSystemTables, CancellationToken cancellationToken)
	{
		SchemaDbModel schema = await GetDbProvider(connection).GetSchemaAsync(includeSystemTables, TimeSpan.FromMinutes(5), cancellationToken);

			// Carga las tablas
			LoadSchemaTables(connection, schema);
			// Carga las vistas
			LoadSchemaViews(connection, schema);
	}

	/// <summary>
	///		Carga las tablas del esquema
	/// </summary>
	private void LoadSchemaTables(ConnectionModel connection, SchemaDbModel schema)
	{
		// Limpia las tablas de la conexión
		connection.Tables.Clear();
		// Agrega los campos
		foreach (TableDbModel tableSchema in schema.Tables)
			connection.Tables.Add(Convert(connection, tableSchema));
	}

	/// <summary>
	///		Carga las vistas del esquema
	/// </summary>
	private void LoadSchemaViews(ConnectionModel connection, SchemaDbModel schema)
	{
		// Limpia las vistas de la conexión
		connection.Views.Clear();
		// Agrega los campos
		foreach (ViewDbModel viewSchema in schema.Views)
			connection.Views.Add(Convert(connection, viewSchema));
	}

	/// <summary>
	///		Convierte una vista / tabla
	/// </summary>
	private ConnectionTableModel Convert(ConnectionModel connection, BaseTableDbModel baseTableView)
	{
		ConnectionTableModel tableView = new(connection);

			// Asigna las propiedades
			tableView.Name = baseTableView.Name;
			tableView.Description = baseTableView.Description;
			tableView.Schema = baseTableView.Schema ?? string.Empty;
			tableView.IsSystem = baseTableView.IsSystem;
			// Asigna los campos
			foreach (FieldDbModel fieldSchema in baseTableView.Fields)
			{
				ConnectionTableFieldModel field = new ConnectionTableFieldModel(tableView);

					// Asigna las propiedades
					field.Name = fieldSchema.Name;
					field.Description = fieldSchema.Description;
					field.TypeText = fieldSchema.DbType ?? string.Empty; // fieldSchema.Type.ToString();
					field.Type = Convert(fieldSchema.Type);
					field.Length = fieldSchema.Length;
					field.IsRequired = fieldSchema.IsRequired;
					field.IsKey = fieldSchema.IsKey;
					field.IsIdentity = fieldSchema.IsIdentity;
					// Añade el campo
					tableView.Fields.Add(field);
			}
			// Devuelve los datos
			return tableView;
	}

	/// <summary>
	///		Convierte el tipo de campo
	/// </summary>
	private ConnectionTableFieldModel.Fieldtype Convert(FieldDbModel.Fieldtype type)
	{
		return type switch
			{
				FieldDbModel.Fieldtype.String => ConnectionTableFieldModel.Fieldtype.String,
				FieldDbModel.Fieldtype.Date => ConnectionTableFieldModel.Fieldtype.Date,
				FieldDbModel.Fieldtype.Integer => ConnectionTableFieldModel.Fieldtype.Integer,
				FieldDbModel.Fieldtype.Decimal => ConnectionTableFieldModel.Fieldtype.Decimal,
				FieldDbModel.Fieldtype.Boolean => ConnectionTableFieldModel.Fieldtype.Boolean,
				_ => ConnectionTableFieldModel.Fieldtype.Unknown
			};
	}

	/// <summary>
	///		Obtiene un proveedor de base de datos
	/// </summary>
	internal IDbProvider GetDbProvider(ConnectionModel connection)
	{
		IDbProvider? provider = CacheProviders.GetProvider(connection);

			// Si no se ha encontrado el proveedor en el diccionario, se crea uno ...
			if (provider is null)
			{
				// Crea el proveedor
				switch (connection.Type)
				{
					case ConnectionModel.ConnectionType.Spark:
							provider = new SparkProvider(new SparkConnectionString(connection.Parameters.ToDictionary()));
						break;
					case ConnectionModel.ConnectionType.SqlServer:
							provider = new LibDbProviders.SqlServer.SqlServerProvider(new LibDbProviders.SqlServer.SqlServerConnectionString(connection.Parameters.ToDictionary()));
						break;
					case ConnectionModel.ConnectionType.Odbc:
							provider = new LibDbProviders.ODBC.OdbcProvider(new LibDbProviders.ODBC.OdbcConnectionString(connection.Parameters.ToDictionary()));
						break;
					case ConnectionModel.ConnectionType.SqLite:
							provider = new LibDbProviders.SqLite.SqLiteProvider(new LibDbProviders.SqLite.SqLiteConnectionString(connection.Parameters.ToDictionary()));
						break;
					case ConnectionModel.ConnectionType.MySql:
							provider = new LibDbProviders.MySql.MySqlProvider(new LibDbProviders.MySql.MySqlConnectionString(connection.Parameters.ToDictionary()));
						break;
					case ConnectionModel.ConnectionType.PostgreSql:
							provider = new LibDbProviders.PostgreSql.PostgreSqlProvider(new LibDbProviders.PostgreSql.PostgreSqlConnectionString(connection.Parameters.ToDictionary()));
						break;
					case ConnectionModel.ConnectionType.DuckDb:
							provider = new LibDbProviders.DuckDb.DuckDbProvider(new LibDbProviders.DuckDb.DuckDbConnectionString(connection.Parameters.ToDictionary()));
						break;
					default:
						throw new ArgumentOutOfRangeException($"Cant find provider for '{connection.Name}'");
				}
				// Lo añade a la caché
				CacheProviders.Add(connection, provider);
			}
			// Abre el proveedor
			if (provider != null)
				provider.Open();
			// Devuelve el proveedor
			if (provider is null)
				throw new ArgumentOutOfRangeException($"Cant find provider for '{connection.Name}'");
			else
				return provider;
	}

	/// <summary>
	///		Obtiene la cadena SQL de consulta de una tabla (SELECT Fields FROM Table)
	/// </summary>
	internal string GetSqlQuery(ConnectionTableModel table)
	{
		IDbProvider? provider = GetDbProvider(table.Connection);

			// Obtiene la cadena de los campos
			if (provider is not null)
			{
				(string start, string end) separators = (provider.SqlHelper.SeparatorStart, provider.SqlHelper.SeparatorEnd);

					// Devuelve la consulta
					return $"""
							SELECT {GetSqlSelectFields(separators, table)}
							  FROM {GetSqlName(separators, table.Schema, table.Name)}
							""";
			}
			// Devuelve la cadena SQL
			return string.Empty;
	}

	/// <summary>
	///		Obtiene el nombre completo de una tabla [Schema].[Table] con lo separadores adecuados
	/// </summary>
	internal string GetSqlTableName(ConnectionTableModel table)
	{
		IDbProvider? provider = GetDbProvider(table.Connection);

			// Obtiene la cadena de los campos
			if (provider is not null)
				return GetSqlName((provider.SqlHelper.SeparatorStart, provider.SqlHelper.SeparatorEnd), table.Schema, table.Name);
			else
				return string.Empty;
	}

	/// <summary>
	///		Obtiene el nombre completo de un campo [Table].[Field] con lo separadores adecuados
	/// </summary>
	internal string GetSqlFieldName(ConnectionTableFieldModel field)
	{
		IDbProvider? provider = GetDbProvider(field.Table.Connection);

			// Obtiene la cadena de los campos
			if (provider is not null)
				return GetSqlName((provider.SqlHelper.SeparatorStart, provider.SqlHelper.SeparatorEnd), field.Table.Name, field.Name);
			else
				return string.Empty;
	}

	/// <summary>
	///		Obtiene una cadena con los campos
	/// </summary>
	private string GetSqlSelectFields((string start, string end) separators, ConnectionTableModel table)
	{
		string fields = string.Empty;
		int length = 80;

			// Obtiene la cadena con los campos
			foreach (ConnectionTableFieldModel field in table.Fields)
			{
				// Añade un salto de línea cada 80 caracteres (más o menos)
				if (fields.Length > length)
				{
					fields += Environment.NewLine + "\t\t";
					length += 80;
				}
				// Añade el nombre de campo
				fields += GetSqlName(separators, field.Table.Name, field.Name);
				// Añade la coma si es necesario (no se hace con AddSeparator porque como tenemos un salto de línea, añadiría la coma después
				// del salto de línea)
				if (table.Fields.IndexOf(field) < table.Fields.Count - 1)
					fields += ", ";
			}
			// Devuelve la cadena con los campos
			return fields;
	}

	/// <summary>
	///		Obtiene un nombre de tabla / campo para una consulta SQL 
	/// </summary>
	private string GetSqlName((string start, string end) separators, string schema, string name)
	{
		string result = string.Empty;

			// Añade el nombre de esquema
			if (!string.IsNullOrWhiteSpace(schema))
				result = $"{separators.start}{schema}{separators.end}.";
			// Devuelve el nombre del campo / tabla
			return $"{result}{separators.start}{name}{separators.end}";
	}

	/// <summary>
	///		Manager de scripts
	/// </summary>
	internal DbScriptsManager Manager { get; }

	/// <summary>
	///		Proveedores de datos
	/// </summary>
	private Cache.ProvidersCache CacheProviders = new();
}
