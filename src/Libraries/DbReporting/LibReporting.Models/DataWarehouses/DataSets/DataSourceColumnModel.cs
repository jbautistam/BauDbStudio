using System;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets
{
	/// <summary>
	///		Clase con los datos de una columna
	/// </summary>
	public class DataSourceColumnModel : LibDataStructures.Base.BaseExtendedModel
	{
		/// <summary>
		///		Tipo de campo
		/// </summary>
		public enum Fieldtype
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

		/// <summary>
		///		Obtiene el título de la columna: nombre de columna dado por el usuario o column Id
		/// </summary>
		public string GetTitle()
		{
			if (string.IsNullOrEmpty(Name))
				return ColumnId;
			else
				return Name;
		}

		/// <summary>
		///		Origen de datos
		/// </summary>
		public BaseDataSourceModel DataSource { get; }

		/// <summary>
		///		Id de Columna en la base de datos (el mismo valor que en GlobalId para facilitar las búsquedas)
		/// </summary>
		public string ColumnId 
		{ 
			get { return GlobalId; }
			set { GlobalId = value; }
		}

		/// <summary>
		///		Indica si esta columna es clave primaria
		/// </summary>
		public bool IsPrimaryKey { get; set; }

		/// <summary>
		///		Tipo del campo
		/// </summary>
		public Fieldtype Type { get; set; }

		/// <summary>
		///		Indica si la columna es visible
		/// </summary>
		public bool Visible { get; set; } = true;

		/// <summary>
		///		Indica si se puede ordenar por esta columna
		/// </summary>
		public bool CanSort { get; set; } = true;

		/// <summary>
		///		Indica si se puede filtrar por esta columna
		/// </summary>
		public bool CanFilter { get; set; } = true;

		/// <summary>
		///		Indica si se puede agrupar por esta columna
		/// </summary>
		public bool CanGroupBy { get; set; } = true;

		/// <summary>
		///		Indica si es obligatoria
		/// </summary>
		public bool Required { get; set; }
	}
}
