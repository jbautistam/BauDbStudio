using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.LibCsvFiles.Models;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.Csv
{
	/// <summary>
	///		Base de los controladores de exportación de datos
	/// </summary>
	internal abstract class BaseExportCsvManager : BaseManager
	{
		protected BaseExportCsvManager(DbScriptProcessor processor) : base(processor) {}

		/// <summary>
		///		Obtiene las columnas asociadas al <see cref="IDataReader"/>
		/// </summary>
		protected List<ColumnModel> GetColumns(IDataReader reader)
		{
			List<ColumnModel> columns = new List<ColumnModel>();
			DataTable schema = reader.GetSchemaTable();

				// Obtiene las columnas del dataReader
				foreach (DataRow dataRow in schema.Rows)
				{
					ColumnModel column = new ColumnModel();

						// Busca las propiedades en las columnas
						foreach (DataColumn readerColumn in schema.Columns)
							if (readerColumn.ColumnName.Equals("ColumnName", StringComparison.CurrentCultureIgnoreCase))
								column.Name = dataRow[readerColumn].ToString();
							else if (readerColumn.ColumnName.Equals("DataType", StringComparison.CurrentCultureIgnoreCase))
								column.Type = GetColumnType((Type) dataRow[readerColumn]);
						// Añade la columna a la lista
						columns.Add(column);
				}
				// Devuelve la colección de columnas
				return columns;
		}

		/// <summary>
		///		Obtiene el tipo de columna
		/// </summary>
		private ColumnModel.ColumnType GetColumnType(Type dataType)
		{
			if (IsDataType(dataType, "byte[]")) // ... no vamos a convertir los arrays de bytes
				return ColumnModel.ColumnType.Unknown;
			else if (IsDataType(dataType, "int"))
				return ColumnModel.ColumnType.Integer;
			else if (IsDataType(dataType, "decimal") || IsDataType(dataType, "double") || IsDataType(dataType, "float"))
				return ColumnModel.ColumnType.Decimal;
			else if (IsDataType(dataType, "date"))
				return ColumnModel.ColumnType.DateTime;
			else if (IsDataType(dataType, "bool"))
				return ColumnModel.ColumnType.Boolean;
			else
				return ColumnModel.ColumnType.String;
		}

		/// <summary>
		///		Comprueba si un nombre de tipo contiene un valor determinado
		/// </summary>
		private bool IsDataType(Type dataType, string search)
		{
			return dataType.FullName.IndexOf("." + search, StringComparison.CurrentCultureIgnoreCase) >= 0;
		}

		/// <summary>
		///		Obtiene las cabeceras a partir de las columnas
		/// </summary>
		protected List<string> GetHeaders(List<ColumnModel> columns, bool typedHeader)
		{
			List<string> headers = new List<string>();

				// Añade los nombres de columnas por las cabeceras
				foreach (ColumnModel column in columns)
					if (typedHeader)
						headers.Add($"{column.Name}|{column.Type.ToString()}");
					else
						headers.Add(column.Name);
				// Devuelve la lista de cabeceras
				return headers;
		}

		/// <summary>
		///		Obtiene los valores del <see cref="IDataReader"/>
		/// </summary>
		protected List<(ColumnModel.ColumnType type, object value)> GetValues(List<ColumnModel> columns, IDataReader reader)
		{
			List<(ColumnModel.ColumnType type, object value)> values = new List<(ColumnModel.ColumnType type, object value)>();

				// Añade los valores de la fila
				for (int index = 0; index < reader.FieldCount; index++)
					values.Add((columns[index].Type, reader.GetValue(index)));
				// Devuelve la colección de valores
				return values;
		}
	}
}