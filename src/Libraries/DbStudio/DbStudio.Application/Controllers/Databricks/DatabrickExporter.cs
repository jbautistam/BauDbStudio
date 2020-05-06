using System;
using System.Collections.Generic;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDbScripts.Parser;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibHelper.Files;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.DbStudio.Application.Controllers.Databricks
{
	/// <summary>
	///		Exportador de archivos a notebooks de databricks
	/// </summary>
	internal class DatabrickExporter
	{
		internal DatabrickExporter(SolutionManager manager)
		{
			Manager = manager;
		}

		/// <summary>
		///		Exporta los archivos
		/// </summary>
		internal void Export(Models.Deployments.DeploymentModel deployment)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Debug, "Comienzo de la copia de directorios"))
			{
				(NormalizedDictionary<object> constants, string error) = GetParameters(deployment.JsonParameters);

					if (!string.IsNullOrWhiteSpace(error))
						block.Error(error);
					else
					{				
						// Elimina el directorio destino
						HelperFiles.KillPath(deployment.TargetPath);
						// Copia los directorios
						CopyPath(block, deployment.SourcePath, deployment.TargetPath, constants);
						// Borra los directorios vacíos
						HelperFiles.KillEmptyPaths(deployment.TargetPath);
						// Log
						block.Debug("Fin de la copia de directorios");
					}
			}
		}

		/// <summary>
		///		Obtiene los parámetros de una cadena Json
		/// </summary>
		private (NormalizedDictionary<object> parameters, string error) GetParameters(string jsonParameters)
		{
			NormalizedDictionary<object> parameters = new NormalizedDictionary<object>();
			string error = string.Empty;

				// Carga los parámetros si es necesario
				if (!string.IsNullOrWhiteSpace(jsonParameters))
					try
					{
						System.Data.DataTable table = new LibJsonConversor.JsonToDataTableConversor().ConvertToDataTable(jsonParameters);

							// Crea la colección de parámetros a partir de la tabla
							if (table.Rows.Count == 0)
								error = "No se ha encontrado ningún parámetro en el archivo";
							else
								foreach (System.Data.DataColumn column in table.Columns)
									parameters.Add(column.ColumnName, table.Rows[0][column.Ordinal]);
					}
					catch (Exception exception)
					{
						error = $"Error cuando se cargaba el archivo de parámetros. {exception.Message}";
					}
				// Devuelve el resultado
				return (parameters, error);
		}

		/// <summary>
		///		Copia los archivos de un directorio
		/// </summary>
		private void CopyPath(BlockLogModel block, string sourcePath, string targetPath, NormalizedDictionary<object> constants)
		{
			// Log
			block.Debug($"Copiando '{sourcePath}' a '{targetPath}'");
			// Copia los archivos
			CopyFiles(sourcePath, targetPath, constants);
			// Copia recursivamente los directorios
			foreach (string path in System.IO.Directory.EnumerateDirectories(sourcePath))
				CopyPath(block, path, System.IO.Path.Combine(targetPath, System.IO.Path.GetFileName(path)), constants);
		}

		/// <summary>
		///		Copia los archivos
		/// </summary>
		private void CopyFiles(string sourcePath, string targetPath, NormalizedDictionary<object> constants)
		{
			// Crea el directorio 
			//? Sí, está dos veces, no sé porqué si se ejecuta dos veces consecutivas este método, la segunda vez no crea el directorio a menos que
			//? ejecute dos veces la instrucción
			HelperFiles.MakePath(targetPath);
			HelperFiles.MakePath(targetPath);
			// Copia los archivos
			foreach (string file in System.IO.Directory.EnumerateFiles(sourcePath))
				if (MustCopy(file))
				{
					string targetFile = System.IO.Path.Combine(targetPath, System.IO.Path.GetFileName(file));

						// Copia los archivos de Python sin cambios, los de SQL los convierte
						if (file.EndsWith(".py", StringComparison.CurrentCultureIgnoreCase))
							SaveFileWithoutBom(targetFile, HelperFiles.LoadTextFile(file));
						else if (file.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
							SaveFileWithoutBom(targetFile, TransformSql(file, constants));
				}
		}

		/// <summary>
		///		Graba un texto en un archivo con codificación UTF8 pero sin los caracteres iniciales de BOM. 
		/// </summary>
		/// <remarks>
		///		Databricks no reconoce en los notebook los archivos de texto UTF8 que se graban con los caracteres iniciales
		///	que indican que el archivo es UTF8, por eso se tiene que indicar en la codificación que se omitan estos caracteres
		///	<see cref="https://stackoverflow.com/questions/2502990/create-text-file-without-bom"/>
		/// </remarks>
		private void SaveFileWithoutBom(string fileName, string content)
		{
			HelperFiles.SaveTextFile(fileName, content, new System.Text.UTF8Encoding(false));
		}

		/// <summary>
		///		Indica si se debe copiar un archivo
		/// </summary>
		private bool MustCopy(string fileName)
		{
			return fileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".py", StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		///		Transforma la SQL para un notebook
		/// </summary>
		private string TransformSql(string sourceFile, NormalizedDictionary<object> constants)
		{
			System.Text.StringBuilder sbResult = new System.Text.StringBuilder();
			string content = NormalizeNotebookContent(sourceFile, constants);
			List<SqlSectionModel> scriptSqlParts = new SqlParser().Tokenize(content, constants.ToDictionary(), out string error);

				if (!string.IsNullOrWhiteSpace(error))
					throw new Exception(error);
				else
				{
					bool isFirst = true;

						// Cabecera
						sbResult.AppendLine("-- Databricks notebook source");
						// Añade los scripts al resultado
						foreach (SqlSectionModel scriptSqlPart in scriptSqlParts)
							if (!string.IsNullOrWhiteSpace(scriptSqlPart.Content))
							{
								// Añade el separador de comandos
								if (!isFirst)
								{
									sbResult.AppendLine();
									sbResult.AppendLine("-- COMMAND ----------");
									sbResult.AppendLine();
								}
								// Añade el contenido
								switch (scriptSqlPart.Type)
								{
									case SqlSectionModel.SectionType.Sql:
											sbResult.AppendLine(scriptSqlPart.Content);
										break;
									case SqlSectionModel.SectionType.Comment:
											if (!string.IsNullOrWhiteSpace(scriptSqlPart.Content))
											{
												string [] parts = scriptSqlPart.Content.Split('\r', '\n');
												bool firstComment = true;

													foreach (string part in parts)
													{
														string comment = RemoveComment(part);

															if (!string.IsNullOrWhiteSpace(comment))
															{
																// Añade la cadena mágica de markdown
																sbResult.Append("-- MAGIC ");
																if (firstComment)
																	sbResult.Append("%md ");
																// Añade la línea de contenido
																sbResult.AppendLine(comment);
																// Indica que no es la primera vez
																firstComment = false;
															}
													}
											}
										break;
								}
								// Indica que ya no es el primero
								isFirst = false;
							}
				}
				// Devuelve la cadena
				return sbResult.ToString();
		}

		/// <summary>
		///		Obtiene el contenido normalizado de un notebook
		/// </summary>
		private string NormalizeNotebookContent(string fileName, NormalizedDictionary<object> constants)
		{
			string content = HelperFiles.LoadTextFile(fileName);

				// Reemplaza las constantes
				content = ReplaceConstants(content, constants);
				// Sustituye los argumentos de tipo $Xxxx por getArgument("xxxx")
				content = ConvertArgumentsToGetArgument(content, "$", constants);
				// Pasa los nombres de archivo a minúsculas
				content = ConvertFileNameToLower(content);
				// Devuelve la cadena normalizada
				return content;
		}

		/// <summary>
		///		Reemplaza las constantes
		/// </summary>
		private string ReplaceConstants(string content, NormalizedDictionary<object> constants)
		{
			// Reemplaza las constantes
			foreach ((string key, object value) in constants.Enumerate())
			{
				string parameter = (value ?? "").ToString();

					// Reemplaza el contenido
					content = content.ReplaceWithStringComparison("{{" + key + "}}", parameter);
			}
			// Devuelve el contenido
			return content;
		}

		/// <summary>
		///		Pasa los argumentos de la cadena SQL a minúsculas (Databricks distingue entre mayúsculas y minúsculas y se ha decidido que todos los argumentos estarán
		///	en minúsculas
		/// </summary>
		private string ConvertArgumentsToLower(string sql, string parameterPrefix)
		{
			return ConvertToLower(sql, 
								  System.Text.RegularExpressions.Regex.Match(sql, "\\" + parameterPrefix + "\\w*",
																			 System.Text.RegularExpressions.RegexOptions.IgnoreCase,
																			 TimeSpan.FromSeconds(10)));
		}

		/// <summary>
		///		Pasa los argumentos $xxxx de la cadena SQL a la función getArgument("xxxx")
		/// </summary>
		private string ConvertArgumentsToGetArgument(string value, string parameterPrefix, NormalizedDictionary<object> constants)
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
						// Añade el valor del parámetro a la cadena de salida (siempre que no sea una constante que hay que dejarla igual)
						if (!constants.ContainsKey(argument))
							output += " getArgument(\"" + argument + "\") " + additional;
						else
							output += $"${argument.ToLower()}" + additional;
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

		/// <summary>
		///		Pasa los nombre de archivo de la cadena SQL a minúsculas (los nombres de archivo están entre ` y `
		///	Databricks distingue entre mayúsculas y minúsculas y se ha decidido que todos los argumentos estarán en minúsculas
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
		///		Elimina los caracteres de comentario (/* */ --)
		/// </summary>
		private string RemoveComment(string part)
		{
			// Le quita los comentarios
			if (!string.IsNullOrWhiteSpace(part))
			{
				// Quita los espacios
				part = part.Trim();
				// Quita los caracteres iniciales
				if (part.StartsWith("--") || part.StartsWith("/*"))
				{
					if (part.Length > 2)
						part = part.Substring(2);
					else
						part = string.Empty;
				}
				// Quita los caracteres finales
				if (!string.IsNullOrWhiteSpace(part) && part.EndsWith("*/"))
				{
					if (part.Length > 2)
						part = part.Substring(0, part.Length - 2);
					else
						part = string.Empty;
				}
			}
			// Devuelve la cadena
			return part.TrimIgnoreNull();
		}

		/// <summary>
		///		Manager principal
		/// </summary>
		internal SolutionManager Manager { get; }
	}
}
