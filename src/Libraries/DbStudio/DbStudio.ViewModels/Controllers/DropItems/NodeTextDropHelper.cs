using System;

namespace Bau.Libraries.DbStudio.ViewModels.Controllers.DropItems
{
	/// <summary>
	///		Clase de ayuda cuando se arrastra un nodo de explorador a un editor
	/// </summary>
	internal class NodeTextDropHelper
	{
		/// <summary>
		///		Trata el texto pasado a un editor
		/// </summary>
		internal string TreatTextDropped(string content, bool shiftPressed)
		{
			string result = content;

				// Obtiene el texto adecuado dependiendo de la extensión
				if (!string.IsNullOrWhiteSpace(content) && content.IndexOf('\r') < 0 && content.Length < 10_000 && content.IndexOf('.') >= 0)
				{
					if (content.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase) || content.EndsWith(".sqlx", StringComparison.CurrentCultureIgnoreCase))
						result = LibHelper.Files.HelperFiles.LoadTextFile(content);
					else if (content.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
						result = GetParquetSchema(content);
					else if (content.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
						result = GetCsvSchema(content);
				}
				// Devuelve el texto localizado
				return result;
		}

		/// <summary>
		///		Obtiene el esquema de un archivo parquet
		/// </summary>
		private string GetParquetSchema(string fileName)
		{
			string sql = "SELECT ";
			int length = 80;

				// Obtiene el esquema del archivo
				try
				{
					// Añade los nombres de campos
					using (LibParquetFiles.Readers.ParquetDataReader reader = new LibParquetFiles.Readers.ParquetDataReader())
					{
						// Abre el archivo
						reader.Open(fileName);
						// Añade los nombres de campo
						for (int index = 0; index < reader.FieldCount; index++)
						{
							// Añade un salto de línea si es necesario
							if (sql.Length > length)
							{
								sql += Environment.NewLine + "\t\t";
								length += 80;
							}
							// Nombre de campo
							sql += $" `{reader.GetName(index)}`";
							// Añade la coma si es necesario
							if (index < reader.FieldCount -1)
								sql += ", ";
						}
					}
					// Añade el nombre de tabla
					sql += Environment.NewLine + $"\tFROM parquet.`{fileName}`";
				}
				catch (Exception exception)
				{
					sql = $"Error when read schema {fileName}. {exception.Message}";
				}
				// Devuelve la cadena creada
				return sql;
		}

		/// <summary>
		///		Obtiene el esquema de un archivo CSV
		/// </summary>
		private string GetCsvSchema(string fileName)
		{
			//TODO -> Leer el esquema del archivo CSV
			return "Get CSV schema " + fileName;
		}
	}
}