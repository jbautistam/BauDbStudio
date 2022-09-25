using System;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets
{
	/// <summary>
	///		Clase con los datos de una columna
	/// </summary>
	public class DataSourceColumnModel : Base.BaseReportingModel
	{
		/// <summary>
		///		Tipo de campo
		/// </summary>
		public enum FieldType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Cadena</summary>
			String,
			/// <summary>Fecha</summary>
			Date,
			/// <summary>Número entero</summary>
			Integer,
			/// <summary>Número decimal</summary>
			Decimal,
			/// <summary>Valor lógico</summary>
			Boolean,
			/// <summary>Datos binarios</summary>
			Binary
		}

		public DataSourceColumnModel(BaseDataSourceModel dataSource)
		{
			DataSource = dataSource;
		}

		public DataSourceColumnModel Clone(BaseDataSourceModel targetDataSource)
		{
			return new DataSourceColumnModel(targetDataSource)
							{
								IsPrimaryKey = IsPrimaryKey,
								Type = Type,
								Visible = Visible,
								Required = Required
							};
		}

		/// <summary>
		///		Compara el valor de dos elementos para ordenarlo
		/// </summary>
		public override int CompareTo(Base.BaseReportingModel item)
		{
			if (item is DataSourceColumnModel column)
				return Id.CompareTo(column.Id);
			else
				return -1;
		}

		/// <summary>
		///		Origen de datos
		/// </summary>
		public BaseDataSourceModel DataSource { get; }

		/// <summary>
		///		Indica si esta columna es clave primaria
		/// </summary>
		public bool IsPrimaryKey { get; set; }

		/// <summary>
		///		Tipo del campo
		/// </summary>
		public FieldType Type { get; set; }

		/// <summary>
		///		Indica si la columna es visible
		/// </summary>
		public bool Visible { get; set; } = true;

		/// <summary>
		///		Indica si es obligatoria
		/// </summary>
		public bool Required { get; set; }
	}
}
