﻿using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbScripts.Manager.Interpreter;
using Bau.Libraries.DbScripts.Manager.Models;
using Bau.Libraries.LibLogger.Core;

namespace Bau.Libraries.DbScripts.Manager
{
	/// <summary>
	///		Manager para la ejecución de scripts
	/// </summary>
	public class DbScriptsManager
	{
		public DbScriptsManager(LogManager logger)
		{
			Logger = logger;
			ConnectionManager = new Connections.ConnectionManager(this);
		}

		/// <summary>
		///		Carga el esquema de una conexión
		/// </summary>
		public async Task LoadSchemaAsync(ConnectionModel connection, CancellationToken cancellationToken)
		{
			await ConnectionManager.LoadSchemaAsync(connection, cancellationToken);
		}

		/// <summary>
		///		Ejecuta una consulta sobre una conexión
		/// </summary>
		public async Task ExecuteQueryAsync(ConnectionModel connection, string query, ArgumentListModel arguments, 
											TimeSpan timeout, CancellationToken cancellationToken)
		{
			await Task.Run(() => new SqlCommandController(this).ExecuteAsync(GetDbProvider(connection), query, arguments, timeout, cancellationToken));
		}

		/// <summary>
		///		Ejecuta una consulta interpretada sobre una conexión
		/// </summary>
		public async Task ExecuteInterpretedQueryAsync(ConnectionModel connection, string query, ArgumentListModel arguments, CancellationToken cancellationToken)
		{
			await Task.Run(() => new SqlScriptInterpreter(this).ExecuteAsync(GetDbProvider(connection), query, arguments, connection.TimeoutExecuteScript, 
																			 cancellationToken));
		}

		/// <summary>
		///		Obtiene un <see cref="DataTable"/> paginada con una consulta sobre una conexión
		/// </summary>
		public async Task<DataTable> GetDatatableQueryAsync(ConnectionModel connection, string query, ArgumentListModel arguments, 
															int actualPage, int pageSize, TimeSpan timeout, CancellationToken cancellationToken)
		{
			return await Task.Run(() => new SqlCommandController(this).GetDataTableAsync(GetDbProvider(connection), 
																						 query, arguments, 
																						 actualPage, pageSize, timeout, cancellationToken));
		}

		/// <summary>
		///		Obtiene el datareader de una consulta
		/// </summary>
		public async Task<System.Data.Common.DbDataReader> ExecuteReaderAsync(ConnectionModel connection, string query, ArgumentListModel arguments, 
																			  TimeSpan timeout, CancellationToken cancellationToken)
		{
			return await Task.Run(() => new SqlCommandController(this).ExecuteReaderAsync(GetDbProvider(connection), 
																						  query, arguments, timeout, cancellationToken));
		}

		/// <summary>
		///		Obtiene el plan de ejecución de una consulta
		/// </summary>
		public async Task<DataTable> GetExecutionPlanAsync(ConnectionModel connection, string query, ArgumentListModel arguments, TimeSpan timeout, CancellationToken cancellationToken)
		{
			return await Task.Run(() => new SqlCommandController(this).GetExecutionPlanAsync(GetDbProvider(connection), query, arguments, timeout, cancellationToken));
		}

		/// <summary>
		///		Obtiene un proveedor de base de datos
		/// </summary>
		public LibDbProviders.Base.IDbProvider GetDbProvider(ConnectionModel connection)
		{
			return ConnectionManager.GetDbProvider(connection);
		}

		/// <summary>
		///		Obtiene el esquema de una conexión
		/// </summary>
		public async Task<LibDbProviders.Base.Schema.SchemaDbModel> GetDbSchemaAsync(ConnectionModel connection, CancellationToken cancellationToken)
		{
			return await ConnectionManager.GetSchemaAsync(connection, cancellationToken);
		}

		/// <summary>
		///		Manager de log
		/// </summary>
		internal LogManager Logger { get; }

		/// <summary>
		///		Manager de conexiones
		/// </summary>
		internal Connections.ConnectionManager ConnectionManager { get; }
	}
}
