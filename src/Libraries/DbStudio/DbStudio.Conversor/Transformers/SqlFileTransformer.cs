using System;
using System.Collections.Generic;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDbScripts.Parser;
using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.DbStudio.Conversor.Transformers
{
	/// <summary>
	///		Traductor de archivos SQL
	/// </summary>
	internal class SqlFileTransformer : BaseSqlFileTransformer
	{
		internal SqlFileTransformer(DatabrickExporter exporter, string targetPath, string fileName) : base(exporter, targetPath, fileName)
		{
		}

		/// <summary>
		///		Transforma el contenido del archivo en un notebook de databricks
		/// </summary>
		internal override void Transform()
		{
			SaveFileWithoutBom(GetTargetFileName(".sql"), TransformSql());
		}

		/// <summary>
		///		Transforma la SQL para un notebook
		/// </summary>
		private string TransformSql()
		{
			System.Text.StringBuilder sbResult = new System.Text.StringBuilder();
			List<SqlSectionModel> scriptSqlParts = GetSections(LibHelper.Files.HelperFiles.LoadTextFile(Source));
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
									WriteCommand(sbResult, scriptSqlPart.Content);
								break;
							case SqlSectionModel.SectionType.Comment:
									WriteComments(sbResult, scriptSqlPart.Content);
								break;
						}
						// Indica que ya no es el primero
						isFirst = false;
					}
					// Devuelve la cadena
					return sbResult.ToString();
		}

		/// <summary>
		///		Añade un comando SQL reemplazando los argumentos si es necesario
		/// </summary>
		private void WriteCommand(System.Text.StringBuilder sbResult, string sql)
		{
			if (Exporter.Options.ReplaceArguments)
				sbResult.AppendLine(ConvertArgumentsToGetArgument(sql, "$", Exporter.Options.Parameters, TransformArguments));
			else
				sbResult.AppendLine(sql);
		}

		/// <summary>
		///		Añade los comentarios si es necesario
		/// </summary>
		private void WriteComments(System.Text.StringBuilder sbResult, string comments)
		{
			if (Exporter.Options.WriteComments && !string.IsNullOrWhiteSpace(comments))
			{
				string [] parts = comments.Split('\r', '\n');
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
		}

		/// <summary>
		///		Pasa los argumentos $xxxx de la cadena SQL a la función getArgument("xxxx")
		/// </summary>
		private string TransformArguments(string name, NormalizedDictionary<object> arguments)
		{
			if (!arguments.ContainsKey(name))
				return " getArgument(\"" + name + "\") ";
			else
				return $"${name.ToLower()}";
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
	}
}