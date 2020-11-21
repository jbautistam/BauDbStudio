using System;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets
{
	/// <summary>
	///		Origen de datos a partir de una tabla
	/// </summary>
	public class DataSourceTableModel : BaseDataSourceModel
	{
		public DataSourceTableModel(DataWarehouseModel dataWarehouse) : base(dataWarehouse) {}

		/// <summary>
		///		Esquema origen
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		///		Tabla origen
		/// </summary>
		public string Table { get; set; }

		/// <summary>
		///		Nombre completo
		/// </summary>
		public string FullName
		{
			get 
			{ 
				string fullName = string.Empty;

					// Añade el esquema
					if (!string.IsNullOrWhiteSpace(Schema))
						fullName += $"[{Schema}]";
					// Añade el nombre
					if (!string.IsNullOrWhiteSpace(Table))
					{
						// Añade el separador
						if (!string.IsNullOrWhiteSpace(fullName))
							fullName += ".";
						// Añade el nombre
						fullName += $"[{Table}]";
					}
					// Devuelve el nombre completo
					return fullName; 
			}
		}
	}
}
