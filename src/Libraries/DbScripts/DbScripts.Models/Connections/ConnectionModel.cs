using System;

namespace Bau.Libraries.DbScripts.Models.Connections
{
	/// <summary>
	///		Clase con los datos de la conexión
	/// </summary>
	public class ConnectionModel : LibDataStructures.Base.BaseExtendedModel
	{
		/// <summary>
		///		Tipo de conexión
		/// </summary>
		public enum ConnectionType
		{
			/// <summary>Spark</summary>
			Spark,
			/// <summary>Sql server</summary>
			SqlServer,
			/// <summary>Odbc</summary>
			Odbc,
			/// <summary>PostgreSql</summary>
			PostgreSql,
			/// <summary>SqLite</summary>
			SqLite,
			/// <summary>MySql</summary>
			MySql
		}

		/// <summary>
		///		Tipo de conexión
		/// </summary>
		public ConnectionType Type { get; set; } = ConnectionType.Spark;

		/// <summary>
		///		Parámetros de la conexión
		/// </summary>
		public LibDataStructures.Collections.NormalizedDictionary<string> Parameters { get; } = new LibDataStructures.Collections.NormalizedDictionary<string>();

		/// <summary>
		///		Timeout para la ejecución de scripts
		/// </summary>
		public TimeSpan TimeoutExecuteScript { get; set; }

		/// <summary>
		///		Tablas
		/// </summary>
		public System.Collections.Generic.List<ConnectionTableModel> Tables { get; } = new System.Collections.Generic.List<ConnectionTableModel>();
	}
}
