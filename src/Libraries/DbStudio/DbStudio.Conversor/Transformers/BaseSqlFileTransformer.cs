using System;
using System.Collections.Generic;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDbScripts.Parser;
using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.DbStudio.Conversor.Transformers
{
	/// <summary>
	///		Clase base para los traductores de archivos SQL
	/// </summary>
	internal abstract class BaseSqlFileTransformer : BaseTransformer
	{
		internal BaseSqlFileTransformer(DatabrickExporter exporter, string targetPath, string fileName) : base(exporter, targetPath, fileName)
		{
		}

		/// <summary>
		///		Transforma la SQL para un notebook
		/// </summary>
		protected List<SqlSectionModel> GetSections(string sql)
		{
			string content = NormalizeNotebookContent(sql);
			List<SqlSectionModel> scriptSqlParts = new SqlParser().Tokenize(content, Exporter.Options.Parameters.ToDictionary(), out string error);

				if (!string.IsNullOrWhiteSpace(error))
					throw new Exception(error);
				else
					return scriptSqlParts;
		}

		/// <summary>
		///		Obtiene el contenido normalizado de un notebook
		/// </summary>
		private string NormalizeNotebookContent(string content)
		{
			// Reemplaza las constantes
			content = ReplaceConstants(content, Exporter.Options.Parameters);
			// Pasa los nombres de archivo a minúsculas
			if (Exporter.Options.LowcaseFileNames)
				content = ConvertFileNameToLower(content);
			// Devuelve la cadena normalizada
			return content;
		}

		/// <summary>
		///		Reemplaza las constantes
		/// </summary>
		private string ReplaceConstants(string content, NormalizedDictionary<object> parameters)
		{
			// Reemplaza las constantes
			foreach ((string key, object value) in parameters.Enumerate())
			{
				string parameter = (value ?? "").ToString();

					// Reemplaza el contenido
					content = content.ReplaceWithStringComparison("{{" + key + "}}", parameter);
			}
			// Devuelve el contenido
			return content;
		}

		/// <summary>
		///		Pasa los nombre de archivo de la cadena SQL a minúsculas (los nombres de archivo están entre ` y `
		///	Databricks distingue entre mayúsculas y minúsculas. Para evitar problemas, se ha decidido que todos los argumentos estarán en minúsculas
		/// </summary>
		private string ConvertFileNameToLower(string sql)
		{
			return ConvertToLower(sql,  System.Text.RegularExpressions.Regex.Match(sql, "`.*?`",
																				   System.Text.RegularExpressions.RegexOptions.IgnoreCase,
																				   TimeSpan.FromSeconds(10)));
		}

		/// <summary>
		///		Convierte una serie de coincidencias a minúsculas en una cadena
		/// </summary>
		private string ConvertToLower(string value, System.Text.RegularExpressions.Match match)
		{
			string output = string.Empty;
			int lastIndex = 0;

				// Mientras haya una coincidencia
				while (match.Success)
				{
					// Añade la parte anterior a la cadena de salida y cambia el índice de último elemento encontrado
					output += value.Substring(lastIndex, match.Index - lastIndex);
					lastIndex = match.Index + match.Length;
					// Añade el valor del parámetro a la cadena de salida
					output += value.Substring(match.Index, match.Length).ToLower();
					// Pasa a la siguiente coincidencia
					match = match.NextMatch();
				}
				// Añade el resto de la cadena inicial
				if (lastIndex < value.Length)
					output += value.Substring(lastIndex);
				// Devuelve la colección de parámetros para la base de datos
				return output;
		}

		/// <summary>
		///		Pasa los argumentos $xxxx de la cadena SQL a la función getArgument("xxxx")
		/// </summary>
		protected string ConvertArgumentsToGetArgument(string value, string parameterPrefix, NormalizedDictionary<object> arguments,
													   Func<string, NormalizedDictionary<object>, string> convertArgument)
		{
			System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(value, "\\" + parameterPrefix + "\\w*",
																									System.Text.RegularExpressions.RegexOptions.IgnoreCase,
																									TimeSpan.FromSeconds(10));
			string output = string.Empty;
			int lastIndex = 0;

				// Mientras haya una coincidencia
				while (match.Success)
				{
					(string argument, string additional) = NormalizeArgument(value.Substring(match.Index + 1, match.Length).ToLower());

						// Añade la parte anterior a la cadena de salida y cambia el índice de último elemento encontrado
						output += value.Substring(lastIndex, match.Index - lastIndex);
						lastIndex = match.Index + match.Length + 1;
						// Añade el valor convertido del parámetro a la cadena de salida
						output += convertArgument(argument, arguments) + additional;
						// Pasa a la siguiente coincidencia
						match = match.NextMatch();
				}
				// Añade el resto de la cadena inicial
				if (lastIndex < value.Length)
					output += value.Substring(lastIndex);
				// Devuelve el resultado
				return output;
		}

		/// <summary>
		///		Quita del final del argumento todo lo que no sean caracteres alfabéticos / numéricos, por ejemplo las comas
		///	puede que el argumento esté en una cadena del tipo: CONCAT($dateWeek,7), en ese caso, la variable argument contiene
		/// '$dateWeek,' y hay que dejarlo como argument = '$dateweek' y additional = ',7'
		/// </summary>
		private (string argument, string additional) NormalizeArgument(string value)
		{
			string argument = string.Empty;
			string additional = string.Empty;
			bool found = false;
				
				// Separa los datos adicionales del argumento
				foreach (char chr in value)
					if (found)
						additional += chr;
					else 
					{
						if (char.IsLetterOrDigit(chr) || chr == '_')
							argument += chr;
						else
						{
							// Añade el carácter a la cadena adicional
							additional += chr;
							// Indica que se ha encontrado un separador
							found = true;
						}
					}	
				// Devuelve los dos valores
				return (argument, additional);
		}
	}
}