using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDbProviders.Spark;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibDbProviders.Base.Schema;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.DbScripts.Manager.Connections.Models;

namespace Bau.Libraries.DbScripts.Manager.Connections
{
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
		///		Carga el esquema de la conexión
		/// </summary>
		internal async Task LoadSchemaAsync(ConnectionModel connection, CancellationToken cancellationToken)
		{
			SchemaDbModel schema = await GetDbProvider(connection).GetSchemaAsync(TimeSpan.FromMinutes(5), cancellationToken);

				// Limpia las tablas de la conexión
				connection.Tables.Clear();
				// Agrega los campos
				foreach (TableDbModel tableSchema in schema.Tables)
				{
					ConnectionTableModel table = new ConnectionTableModel(connection);

						// Asigna las propiedades
						table.Name = tableSchema.Name;
						table.Description = tableSchema.Description;
						table.Schema = tableSchema.Schema;
						// Asigna los campos
						foreach (FieldDbModel fieldSchema in tableSchema.Fields)
						{
							ConnectionTableFieldModel field = new ConnectionTableFieldModel(table);

								// Asigna las propiedades
								field.Name = fieldSchema.Name;
								field.Description = fieldSchema.Description;
								field.TypeText = fieldSchema.DbType; // fieldSchema.Type.ToString();
								field.Type = Convert(fieldSchema.Type);
								field.Length = fieldSchema.Length;
								field.IsRequired = fieldSchema.IsRequired;
								field.IsKey = fieldSchema.IsKey;
								field.IsIdentity = fieldSchema.IsIdentity;
								// Añade el campo
								table.Fields.Add(field);
						}
						// Añade la tabla a la colección
						connection.Tables.Add(table);
				}
		}

		/// <summary>
		///		Convierte el tipo de campo
		/// </summary>
		private ConnectionTableFieldModel.Fieldtype Convert(FieldDbModel.Fieldtype type)
		{
			switch (type)
			{
				case FieldDbModel.Fieldtype.String:
					return ConnectionTableFieldModel.Fieldtype.String;
				case FieldDbModel.Fieldtype.Date:
					return ConnectionTableFieldModel.Fieldtype.Date;
				case FieldDbModel.Fieldtype.Integer:
					return ConnectionTableFieldModel.Fieldtype.Integer;
				case FieldDbModel.Fieldtype.Decimal:
					return ConnectionTableFieldModel.Fieldtype.Decimal;
				case FieldDbModel.Fieldtype.Boolean:
					return ConnectionTableFieldModel.Fieldtype.Boolean;
				default:
					return ConnectionTableFieldModel.Fieldtype.Unknown;
			}
		}

		/// <summary>
		///		Obtiene un proveedor de base de datos
		/// </summary>
		internal IDbProvider GetDbProvider(ConnectionModel connection)
		{
			IDbProvider provider = CacheProviders.GetProvider(connection);

				// Si no se ha encontrado el proveedor en el diccionario, se crea uno ...
				if (provider == null)
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
				return provider;
		}

		/// <summary>
		///		Ejecuta una consulta
		/// </summary>
		internal async Task<DataTable> GetDatatableQueryAsync(ConnectionModel connection, string query, 
															  Models.ArgumentListModel arguments, 
															  int actualPage, int pageSize, TimeSpan timeout, CancellationToken cancellationToken)
		{	
			return await Task.Run(() => new ScriptSqlController(this).GetDataTableAsync(GetDbProvider(connection), 
																						query, arguments, 
																						actualPage, pageSize, timeout, cancellationToken));
		}

		/// <summary>
		///		Obtiene el datareader de una consulta
		/// </summary>
		internal async Task<System.Data.Common.DbDataReader> ExecuteReaderAsync(ConnectionModel connection, string query, Models.ArgumentListModel arguments, 
																				TimeSpan timeout, CancellationToken cancellationToken)
		{
			return await Task.Run(() => new ScriptSqlController(this).ExecuteReaderAsync(GetDbProvider(connection), 
																						 query, arguments, timeout, cancellationToken));
		}

		/// <summary>
		///		Ejecuta una consulta
		/// </summary>
		internal async Task ExecuteQueryAsync(ConnectionModel connection, string query, ArgumentListModel arguments, 
											  TimeSpan timeout, CancellationToken cancellationToken)
		{	
			await Task.Run(() => new ScriptSqlController(this).ExecuteAsync(GetDbProvider(connection), query, arguments, timeout, cancellationToken));
		}

		/// <summary>
		///		Ejecuta una consulta SQL interpretada
		/// </summary>
		internal async Task ExecuteInterpretedQueryAsync(ConnectionModel connection, string query, ArgumentListModel arguments, CancellationToken cancellationToken)
		{
			await Task.Run(() => new Interpreter.ScriptSqlInterpreter(this).ExecuteAsync(GetDbProvider(connection), query, arguments, connection.TimeoutExecuteScript, 
																						 cancellationToken));
		}

		/// <summary>
		///		Obtiene el plan de ejecución de una consulta
		/// </summary>
		internal async Task<DataTable> GetExecutionPlanAsync(ConnectionModel connection, string query, ArgumentListModel arguments, TimeSpan timeout, CancellationToken cancellationToken)
		{
			return await Task.Run(() => new ScriptSqlController(this).GetExecutionPlanAsync(GetDbProvider(connection), query, arguments, timeout, cancellationToken));
		}

		/// <summary>
		///		Manager de scripts
		/// </summary>
		internal DbScriptsManager Manager { get; }

		/// <summary>
		///		Proveedores de datos
		/// </summary>
		private Cache.ProvidersCache CacheProviders = new Cache.ProvidersCache();
	}
}
