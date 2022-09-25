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
		///		Compara el valor de dos elementos para ordenarlo
		/// </summary>
		public override int CompareTo(Base.BaseReportingModel item)
		{
			if (item is DataSourceTableModel table)
				return FullName.CompareTo(table.FullName);
			else
				return -1;
		}

		/// <summary>
		///		Clona un <see cref="DataSourceTableModel"/>
		/// </summary>
		public DataSourceTableModel Clone(DataWarehouseModel target)
		{
			DataSourceTableModel dataSource = new DataSourceTableModel(target)
														{
															Schema = Schema,
															Table = Table
														};

				// Clona las columnas
				foreach (DataSourceColumnModel column in Columns.EnumerateValues())
					dataSource.Columns.Add(column.Clone(dataSource));
				// Devuelve el origen de datos clonado
				return dataSource;
		}

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

		/// <summary>
		///		Indica si es un origen de datos establecido sobre una vista
		/// </summary>
		public bool IsView { get; set; }
	}
}
