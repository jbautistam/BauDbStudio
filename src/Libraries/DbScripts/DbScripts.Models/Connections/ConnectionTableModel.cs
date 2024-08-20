using System;

namespace Bau.Libraries.DbScripts.Models.Connections
{
	/// <summary>
	///		Datos de una tabla
	/// </summary>
	public class ConnectionTableModel : LibDataStructures.Base.BaseExtendedModel
	{
		public ConnectionTableModel(ConnectionModel connection)
		{
			Connection = connection;
		}

		/// <summary>
		///		Conexión a la que pertenece la tabla
		/// </summary>
		public ConnectionModel Connection { get; }

		/// <summary>
		///		Nombre del esquema
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		///		Nombre completo de la tabla
		/// </summary>
		public string FullName
		{
			get 
			{
				if (!string.IsNullOrWhiteSpace(Schema))
					return $"{Schema}.{Name}";
				else
					return Name;
			}
		}

		/// <summary>
		///		Campos
		/// </summary>
		public System.Collections.Generic.List<ConnectionTableFieldModel> Fields { get; } = new System.Collections.Generic.List<ConnectionTableFieldModel>();
	}
}
