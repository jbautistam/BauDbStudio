using System;

using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibHelper.Files;

namespace Bau.Libraries.DbStudio.Application.Controllers.EtlProjects
{
	/// <summary>
	///		Clase de ayuda para generación de consultas de validación de archivos
	/// </summary>
	internal class EtlFilesGenerator
	{
		internal EtlFilesGenerator(ConnectionModel connection, IDbProvider provider, string path)
		{
			Connection = connection;
			Provider = provider;
			Path = path;
		}

		/// <summary>
		///		Escribe el archivo de creación de una conexión
		/// </summary>
		internal void WriteFileCreateConnection(string dataBaseVariable, string mountPathVariable, string fileName)
		{
			string content = string.Empty;

				// Añade las líneas de la consulta
				content += "DROP DATABASE IF EXISTS {{" + dataBaseVariable + "}} CASCADE" + Environment.NewLine;
				content += GetSqlSeparator();
				content += "CREATE DATABASE IF NOT EXISTS {{" + dataBaseVariable + "}}" + Environment.NewLine;
				content += "\tLOCATION '{{" + mountPathVariable + "}}/{{" + dataBaseVariable + "}}'" + Environment.NewLine;
				content += GetSqlSeparator();
			// Reemplaza los placeholder
			Save(fileName, content);
		}

		/// <summary>
		///		Obtiene la SQL de borrado de una tabla
		/// </summary>
		internal string GetSqlDropTable(string dataBaseVariable, string table)
		{
			return "DROP TABLE IF EXISTS {{" + dataBaseVariable + "}}." + Provider.SqlHelper.FormatName(table);
		}

		/// <summary>
		///		Obtiene la SQL de creación de una tabla
		/// </summary>
		internal string GetSqlCreateTable(string dataBaseVariable, string table, string sql)
		{
			return "CREATE TABLE {{" + dataBaseVariable + "}}." + Provider.SqlHelper.FormatName(table) + " AS" + Environment.NewLine + sql;
		}

		/// <summary>
		///		Obtiene la cadena de validación sobre un archivo
		/// </summary>
		internal string GetSqlValidateFile(ConnectionTableModel table, string mountPathVariable, string pathValidate, SolutionManager.FormatType formatType, bool countRecords)
		{
			string sql = string.Empty;

				// Crea la cadena SQL de comparación si hay algún campo en la tabla
				if (table.Fields.Count > 0)
				{
					if (countRecords)
						sql = "\tSELECT COUNT(*) AS Number";
					else
						sql = "\t" + GetSqlHeaderCompare(table, "Test", "Target");
					sql += Environment.NewLine + $"\t\tFROM {GetFileNameTable(mountPathVariable, pathValidate, table.Name, formatType)} AS Test";
					sql += Environment.NewLine + $"\t\tFULL OUTER JOIN {Provider.SqlHelper.FormatName(table.Schema,table.Name)} AS Target";
					sql += Environment.NewLine + "\t\t\tON " + GetSqlCompareFields(table).Trim();
					sql += Environment.NewLine + $"\t\tWHERE {Provider.SqlHelper.FormatName("Target", table.Fields[0].Name)} IS NULL";
					sql += Environment.NewLine + $"\t\t\tOR {Provider.SqlHelper.FormatName("Test", table.Fields[0].Name)} IS NULL";
					sql += Environment.NewLine;
				}
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la cadena SQL de la cabecera con los campos a visualizar
		/// </summary>
		private string GetSqlHeaderCompare(ConnectionTableModel table, string sourceAlias, string targetAlias)
		{
			string sql = string.Empty;

				// Crea la cabecera con los nombres de campos
				foreach (ConnectionTableFieldModel field in table.Fields)
				{
					sql = sql.AddWithSeparator(Provider.SqlHelper.FormatName(sourceAlias, field.Name), ",");
					sql = sql.AddWithSeparator(Provider.SqlHelper.FormatName(targetAlias, field.Name), ",");
				}
				// Devuelve la cadena creada
				return "SELECT " + sql;
		}

		/// <summary>
		///		Obtiene el nombre de archivo
		/// </summary>
		private string GetFileNameTable(string mountPathVariable, string pathValidate, string table, SolutionManager.FormatType formatType)
		{
			string extension = "parquet";

				// Cambia la extensión si es necesario
				if (formatType == SolutionManager.FormatType.Csv)
					extension = "csv";
				// Devuelve el nombre de la tabla
				return $"{extension}.`" + "{{" + mountPathVariable + "}}" + $"/{pathValidate}/{table}.{extension}`";
		}

		/// <summary>
		///		Obtiene el nombre de una base de datos y tabla
		/// </summary>
		internal string GetSqlTableName(string dataBaseVariable, string table)
		{
			return "{{" + dataBaseVariable + "}}." + Provider.SqlHelper.FormatName(table);
		}

		/// <summary>
		///		Obtiene la cadena de comparación de campos
		/// </summary>
		private string GetSqlCompareFields(ConnectionTableModel table)
		{
			string sql = string.Empty;

				// Añade la comparación de campos
				foreach (ConnectionTableFieldModel field in table.Fields)
				{
					// Añade el operador si es necesario
					if (!string.IsNullOrWhiteSpace(sql))
						sql += "\t\t\t\tAND ";
					// Añade la comparación
					sql += $"IfNull({Provider.SqlHelper.FormatName("Target", field.Name)}, '') = IfNull({Provider.SqlHelper.FormatName("Test", field.Name)}, '')" + Environment.NewLine;
				}
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene el separador de comandos SQL (GO)
		/// </summary>
		internal string GetSqlSeparator()
		{
			return "GO" + Environment.NewLine + Environment.NewLine;
		}

		/// <summary>
		///		Graba un archivo de texto
		/// </summary>
		internal void Save(string fileName, string content)
		{
			// Crea el directorio de salida
			HelperFiles.MakePath(Path);
			// Graba el contenido
			HelperFiles.SaveTextFile(System.IO.Path.Combine(Path, fileName), content);
		}

		/// <summary>
		///		Conexión
		/// </summary>
		internal ConnectionModel Connection { get; }

		/// <summary>
		///		Proveedor de base de datos
		/// </summary>
		internal IDbProvider Provider { get; }

		/// <summary>
		///		Directorio de salida de los archivos
		/// </summary>
		internal string Path { get; }
	}
}
