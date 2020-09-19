using System;
using System.Collections.Generic;

using Bau.Libraries.LibDbScripts.Interpreter;
using Bau.Libraries.LibDbScripts.Interpreter.Models.Sentences;
using Bau.Libraries.LibDbScripts.Parser;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;
using Bau.Libraries.LibInterpreter.Models.Expressions;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Symbols;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.DbStudio.Conversor.Transformers
{
	/// <summary>
	///		Traductor de archivos SQL extendidos
	/// </summary>
	/// <remarks>
	/// Ejemplo de un notebook de Python con SparkSql para dataBricks
	/// 
	/// # Databricks notebook source
	///# MAGIC %md 
	///# MAGIC # Manager
	///# COMMAND ----------
	///from pyspark.sql import SQLContext
	///sqlContext = SQLContext(sc)
	///protocols = sqlContext.sql("""
	///                           SELECT protocol_type, count(*) as freq
	///                           FROM connections
	///                           GROUP BY protocol_type
	///                           ORDER BY 2 DESC
	///                           """)
	/// </remarks>
	internal class SqlExtendedFileTransformer : BaseSqlFileTransformer
	{
		// Variables privadas
		private System.Text.StringBuilder _sbBuilder = new System.Text.StringBuilder();
		private int _indent = 0;

		internal SqlExtendedFileTransformer(DatabrickExporter exporter, string targetPath, string fileName) : base(exporter, targetPath, fileName)
		{
		}

		/// <summary>
		///		Transforma el contenido del archivo en un notebook de Python para databricks
		/// </summary>
		internal override void Transform()
		{
			using (BlockLogModel block = Exporter.Logger.Default.CreateBlock(LogModel.LogType.Info, "Start script transpilation"))
			{
				// Limpia el conteto
				Context.Clear();
				// Añade las cabecera
				AddHeaders();
				// Transforma las sentencias
				Transform(Parse(Source).Sentences);
				// Graba el archivo
				SaveFileWithoutBom(GetTargetFileName(".py"), _sbBuilder.ToString());
			}
		}

		/// <summary>
		///		Interpreta un archivo
		/// </summary>
		private ProgramModel Parse(string fileName)
		{
			ProgramModel program = new DbScriptsInterpreter(null, Exporter.Logger).Parse(LibHelper.Files.HelperFiles.LoadTextFile(Source));

				if (program.Errors.Count > 0)
					throw new ArgumentException($"Cant compile {fileName}");
				else
					return program;
		}

		/// <summary>
		///		Añade las cabeceras para un archivo Python
		/// </summary>
		private void AddHeaders()
		{
			AddSentence("# Databricks notebook source");
			AddSentence(string.Empty);
			AddSentence("# COMMAND ----------");
			AddSentence("from pyspark.sql import SQLContext");
			AddSentence("sqlContext = SQLContext(sc)");
			AddSentence(string.Empty);
			AddSentence("# COMMAND ----------");
		}

		/// <summary>
		///		Transforma una serie de sentencias
		/// </summary>
		private void Transform(SentenceCollection sentences)
		{
			// Crea un contexto nuevo
			Context.Add();
			// Añade las sentencias
			foreach (SentenceBase sentenceBase in sentences)
				switch (sentenceBase)
				{
					case SentenceDeclare sentence:
							TransformDeclare(sentence);
						break;
					case SentenceLet sentence:
							TransformLet(sentence);
						break;
					case SentenceFunction sentence:
							TransformFunction(sentence);
						break;
					case SentenceCallFunction sentence:
							TransformCallFunction(sentence);
						break;
					case SentenceIf sentence:
							TransformIf(sentence);
						break;
					case SentenceDo sentence:
							TransformDo(sentence);
						break;
					case SentenceWhile sentence:
							TransformWhile(sentence);
						break;
					case SentenceFor sentence:
							TransformFor(sentence);
						break;
					case SentenceSql sentence:
							TransformSql(sentence);
						break;
					case SentenceReturn sentence:
							TransformReturn(sentence);
						break;
					default:
						throw new NotImplementedException($"Unknown sentence: {sentenceBase.GetType().ToString()}");
				}
			// Limpia el contexto
			Context.Pop();
		}

		/// <summary>
		///		Transforma una declaración
		/// </summary>
		private void TransformDeclare(SentenceDeclare sentence)
		{
			if (sentence.Expressions.Count > 0)
			{
				string result = sentence.Variable.Name + " = ";

					// Añade la variable al contexto
					Context.Actual.VariablesTable.Add(sentence.Variable.Name, ConvertSymbolType(sentence.Variable.Type));
					// Añade las expresiones
					result += TransformExpressions(sentence.Expressions);
					// Añade la sentencia
					AddSentence(result);
			}
		}

		/// <summary>
		///		Convierte el tipo de un símbolo a un tipo de variable
		/// </summary>
		private VariableModel.VariableType ConvertSymbolType(SymbolModel.SymbolType type)
		{
			switch (type)
			{
				case SymbolModel.SymbolType.Boolean:
					return VariableModel.VariableType.Boolean;
				case SymbolModel.SymbolType.Date:
					return VariableModel.VariableType.Date;
				case SymbolModel.SymbolType.Numeric:
					return VariableModel.VariableType.Numeric;
				case SymbolModel.SymbolType.String:
					return VariableModel.VariableType.String;
				default:
					return VariableModel.VariableType.Unknown;
			}
		}

		/// <summary>
		///		Transforma una asignación
		/// </summary>
		private void TransformLet(SentenceLet sentence)
		{
			string result = sentence.VariableName + " = ";

				// Añade las expresiones
				result += TransformExpressions(sentence.Expressions);
				// Añade la sentencia al script
				AddSentence(result);
		}

		/// <summary>
		///		Transforma una sentencia if
		/// </summary>
		private void TransformIf(SentenceIf sentence)
		{
			// Añade la sentencia if
			AddSentence(string.Empty);
			AddSentence("if " + TransformExpressions(sentence.Condition) + ":");
			// Añade las sentencias hija
			AddIndent();
			Transform(sentence.SentencesThen);
			RemoveIndent();
			// Añade el else
			if (sentence.SentencesElse.Count > 0)
			{
				// Añade la sentencia else
				AddSentence("else:");
				// Añade las sentencias hija
				AddIndent();
				Transform(sentence.SentencesElse);
				RemoveIndent();
			}
		}

		/// <summary>
		///		Transforma una sentencia do...while
		/// </summary>
		/// <remarks>
		/// Python no tiene una sentencia do... while, por tanto, lo tenemos que hacer más o menos así
		/// while True:  
		///		print(i)  
		///		i = i + 1  
		///		if(i > 5):  
		///			break  
		/// </remarks>
		private void TransformDo(SentenceDo sentence)
		{
			// Añade la sentencia while true
			AddSentence(string.Empty);
			AddSentence("while True:");
			// Añade las sentencias hija
			AddIndent();
			Transform(sentence.Sentences);
			// Añade el if con el break
			AddSentence("if not (" + TransformExpressions(sentence.Condition) + "):");
			AddIndent();
			AddSentence("break");
			// Quita las indentaciones, dos veces: una para el if y la otra para el while
			RemoveIndent();
			RemoveIndent();
		}

		/// <summary>
		///		Transforma una sentencia while
		/// </summary>
		private void TransformWhile(SentenceWhile sentence)
		{
			// Añade el while
			AddSentence(string.Empty);
			AddSentence("while " + TransformExpressions(sentence.Condition) + ":");
			// Añade las sentencias hija
			AddIndent();
			Transform(sentence.Sentences);
			RemoveIndent();
		}

		/// <summary>
		///		Transforma una sentencia for: como en python se utiliza un rango pero en un script SQL tenemos un for con inicio, fin y paso, lo transformamos
		///	en una sentencia while
		/// </summary>
		private void TransformFor(SentenceFor sentence)
		{
			// Añade la declaración del índice
			AddSentence(string.Empty);
			AddSentence(sentence.Variable.Name + " = " + TransformExpressions(sentence.StartExpression));
			// Añade la sentencia while (con <=)
			AddSentence("while " + sentence.Variable.Name + " <= " + TransformExpressions(sentence.EndExpression) + ":");
			// Añade las sentencias hija
			AddIndent();
			Transform(sentence.Sentences);
			// Añade el step
			if (sentence.StepExpression.Count == 0)
				AddSentence($"{sentence.Variable.Name} = {sentence.Variable.Name} + 1");
			else
				AddSentence($"{sentence.Variable.Name} = {sentence.Variable.Name} + {TransformExpressions(sentence.StepExpression)}");
			// Y quita la indentación
			RemoveIndent();
		}

		/// <summary>
		///		Transforma una sentencia SQL
		/// </summary>
		private void TransformSql(SentenceSql sentence)
		{
			List<SqlSectionModel> sqlSections = GetSections(sentence.Command);

				// Añade las sentencias SQL (se le añade un salto de línea al contenido para que no dé errores de "out of range" al recorrer la cadena)
				//TODO: Corregir TransformSqlCommand para que no sea necesaro el Environment.NewLine
				foreach (SqlSectionModel sqlSection in sqlSections)
					if (sqlSection.Type == SqlSectionModel.SectionType.Sql && !string.IsNullOrWhiteSpace(sqlSection.Content))
						AddSentence("sqlContext.sql(" + TransformSqlCommand(sqlSection.Content + Environment.NewLine, "$") + ")");
		}

		/// <summary>
		///		Transforma un comando SQL en una cadena de tipo string.format en la que reemplazamos los nombres por el valor de los argumentos
		/// </summary>
		/// <remarks>
		///		Tipo de salida:
		///		sqlCommand.format(var1, var2)
		///		
		///		Donde sqlCommand es algo como SELECT * FROM {}.Table WHERE Field > {}
		///		
		///		Es decir, nos dará un resultado de tipo
		///		"""SELECT * FROM {}.Table WHERE Field > {}""".format(var1, var2)
		/// </remarks>
		private string TransformSqlCommand(string sql, string parameterPrefix)
		{
			System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(sql, "\\" + parameterPrefix + "\\w*",
																									System.Text.RegularExpressions.RegexOptions.IgnoreCase,
																									TimeSpan.FromSeconds(10));
			string output = string.Empty;
			int lastIndex = 0;
			List<string> arguments = new List<string>();

				// Mientras haya una coincidencia
				while (match.Success)
				{
					(string argument, string additional) = NormalizeArgument(sql.Substring(match.Index + 1, match.Length).ToLower());

						// Añade la parte anterior a la cadena de salida y cambia el índice de último elemento encontrado
						output += sql.Substring(lastIndex, match.Index - lastIndex);
						lastIndex = match.Index + match.Length + 1;
						// Añade el argumento obtenido a la lista
						arguments.Add(argument);
						// Añade el valor convertido del parámetro a la cadena de salida
						output += "{}" + additional;
						// Pasa a la siguiente coincidencia
						match = match.NextMatch();
				}
				// Añade el resto de la cadena inicial
				if (lastIndex < sql.Length)
					output += sql.Substring(lastIndex);
				// Añade las cadenas de entrada
				output = new string('"', 3) + output.TrimIgnoreNull() + new string('"', 3);
				// Añade las variables de formato
				output += GetFormatArguments(arguments);
				// Devuelve el resultado
				return output.TrimIgnoreNull();
		}

		/// <summary>
		///		Obtiene la parte .format de un comando SQL
		/// </summary>
		private string GetFormatArguments(List<string> arguments)
		{
			string result = string.Empty;
			TableVariableModel variables = Context.Actual.GetVariablesRecursive();

				// Sólo hay un formato si realmente hay argumentos
				if (arguments.Count > 0)
				{
					// Añade el inicio del comando format
					result += ".format(";
					// Añade los argumentos
					for (int index = 0; index < arguments.Count; index++)
					{
						// Separador
						if (index > 0)
							result += ", ";
						// Argumento: si está en la tabla de variables, se pone la variable, si no está en la tabla de variables
						// es un getArgument
						if (variables.Exists(arguments[index]))
							result += arguments[index];
						else
							result += $"getArgument('{arguments[index]}')";
					}
					// Añade el fin del comando format
					result += ")";
				}
				// Devuelve el resultado
				return result;
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
		///		Transforma una definición de función
		/// </summary>
		/// <remarks>
		/// def my_function(fname):
		///		print(fname + " var")
		/// </remarks>
		private void TransformFunction(SentenceFunction sentence)
		{
			// Añade los argumentos a las variables del contexto
			foreach (SymbolModel argument in sentence.Arguments)
				Context.Actual.VariablesTable.Add(argument.Name, ConvertSymbolType(argument.Type));
			// Añade la definición de función
			AddSentence(string.Empty);
			AddSentence("def " + sentence.Definition.Name + "(" + GetArguments(sentence.Arguments) + "):");
			// Añade el cuerpo de la función
			AddIndent();
			Transform(sentence.Sentences);
			RemoveIndent();
			// Añade una línea vacía tras la definición de la función
			AddSentence(string.Empty);
		}

		/// <summary>
		///		Transforma una sentencia return
		/// </summary>
		/// <remarks>
		///		return expresion
		/// </remarks>
		private void TransformReturn(SentenceReturn sentence)
		{
			AddSentence("return " + TransformExpressions(sentence.Expression));
		}

		/// <summary>
		///		Obtiene una cadena con los nombres de los argumentos
		/// </summary>
		private string GetArguments(List<SymbolModel> arguments)
		{
			string result = string.Empty;

				// Añade los argumentos
				foreach	(SymbolModel argument in arguments)
					result = result.AddWithSeparator(argument.Name, ",");
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Transforma una llamada a función
		/// </summary>
		/// <remarks>
		///		functionName(expressions)
		/// </remarks>
		private void TransformCallFunction(SentenceCallFunction sentence)
		{
			AddSentence(sentence.Function + "(" + TransformExpressions(sentence.Arguments) + ")");
		}

		/// <summary>
		///		Transforma una lista de expresiones
		/// </summary>
		private string TransformExpressions(List<ExpressionsCollection> expressions)
		{
			string result = string.Empty;

				// Añade cada una de las expresiones separadas por como
				foreach (ExpressionsCollection expression in expressions)
					result = result.AddWithSeparator(TransformExpressions(expression), ",");
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Transforma las expresiones
		/// </summary>
		private string TransformExpressions(ExpressionsCollection expressions)
		{
			string result = string.Empty;

				// Añade las expresiones
				foreach (ExpressionBase expressionBase in expressions)
					switch (expressionBase)
					{
						case ExpressionConstant expression:
								result += TransformExpressionConstant(expression);
							break;
						case ExpressionFunction expression:
								result += TransformExpressionFunction(expression);
							break;
						case ExpressionParenthesis expression:
								result += TransformExpressionParenthesis(expression);
							break;
						case ExpressionOperatorLogical expression:
								result += TransformExpressionOperatorLogical(expression);
							break;
						case ExpressionOperatorMath expression:
								result += TransformExpressionOperatorMath(expression);
							break;
						case ExpressionOperatorRelational expression:
								result += TransformExpressionOperatorRelational(expression);
							break;
						case ExpressionVariableIdentifier expression:
								result += TransformExpressionVariable(expression);
							break;
					}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Obtiene el valor de una constante
		/// </summary>
		private string TransformExpressionConstant(ExpressionConstant expression)
		{
			if (expression.Value == null)
				return "none";
			else
				switch (expression.Type)
				{
					case SymbolModel.SymbolType.Boolean:
						if ((bool) expression.Value)
							return "True";
						else
							return "False";
					case SymbolModel.SymbolType.Date:
						return $"'{(DateTime) expression.Value:yyyy-MM-dd}'";
					case SymbolModel.SymbolType.Numeric:
						return "" + ((double) expression.Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
					case SymbolModel.SymbolType.String:
						return $"'{expression.Value.ToString()}'";
					default:
						throw new NotImplementedException("Constant expression unknown");
				}
		}

		/// <summary>
		///		Obtiene el valor de una función
		/// </summary>
		private string TransformExpressionFunction(ExpressionFunction expression)
		{
			string result = $"{expression.Function}(";

				// Añade las expresiones
				for (int index = 0; index < expression.Arguments.Count; index++)
				{
					// Añade el separador de parámetros
					if (index > 0)
						result += ", ";
					// Añade la expresión del argumento
					result += TransformExpressions(expression.Arguments[index]).TrimIgnoreNull();
				}
				// Añade el paréntesis final
				result += ")";
				// y devuelve el resultado
				return result;
		}

		/// <summary>
		///		Obtiene el valor de un paréntesis
		/// </summary>
		private string TransformExpressionParenthesis(ExpressionParenthesis expression)
		{
			if (expression.Open)
				return "(";
			else
				return ") ";
		}

		/// <summary>
		///		Obtiene el valor de un operador lógico
		/// </summary>
		private string TransformExpressionOperatorLogical(ExpressionOperatorLogical expression)
		{
			switch (expression.Type)
			{
				case ExpressionOperatorLogical.LogicalType.Equal:
					return " == ";
				case ExpressionOperatorLogical.LogicalType.Distinct:
					return " != ";
				case ExpressionOperatorLogical.LogicalType.Greater:
					return " > ";
				case ExpressionOperatorLogical.LogicalType.Less:
					return " < ";
				case ExpressionOperatorLogical.LogicalType.GreaterOrEqual:
					return " >= ";
				case ExpressionOperatorLogical.LogicalType.LessOrEqual:
					return " <= ";
				default:
					throw new NotImplementedException("Logical expression unknown");
			}
		}

		/// <summary>
		///		Obtiene el valor de un operador matemático
		/// </summary>
		private string TransformExpressionOperatorMath(ExpressionOperatorMath expression)
		{
			switch (expression.Type)
			{
				case ExpressionOperatorMath.MathType.Sum:
					return " + ";
				case ExpressionOperatorMath.MathType.Substract:
					return " - ";
				case ExpressionOperatorMath.MathType.Multiply:
					return " * ";
				case ExpressionOperatorMath.MathType.Divide:
					return " / ";
				case ExpressionOperatorMath.MathType.Modulus:
					return " % ";
				default:
					throw new NotImplementedException("Math expression unknown");
			}
		}

		/// <summary>
		///		Obtiene el valor de un operador relacional
		/// </summary>
		private string TransformExpressionOperatorRelational(ExpressionOperatorRelational expression)
		{
			switch (expression.Type)
			{
				case ExpressionOperatorRelational.RelationalType.And:
					return " and ";
				case ExpressionOperatorRelational.RelationalType.Or:
					return " or ";
				case ExpressionOperatorRelational.RelationalType.Not:
					return " not ";
				default:
					throw new NotImplementedException("Relational expression unknown");
			}
		}

		/// <summary>
		///		Obtiene el valor de una variable
		/// </summary>
		private string TransformExpressionVariable(ExpressionVariableIdentifier expression)
		{
			return expression.Name;
		}

		/// <summary>
		///		Añade una indentación
		/// </summary>
		private void AddIndent()
		{
			_indent++;
		}
		
		/// <summary>
		///		Elimina una indentación
		/// </summary>
		private void RemoveIndent()
		{
			_indent--;
		}

		/// <summary>
		///		Añade la sentencia
		/// </summary>
		private void AddSentence(string sentence)
		{
			_sbBuilder.AppendLine(new string('\t', _indent) + sentence.TrimIgnoreNull());
		}

		/// <summary>
		///		Contexto con las variables
		/// </summary>
		private LibInterpreter.Interpreter.Context.ContextStackModel Context { get; } = new LibInterpreter.Interpreter.Context.ContextStackModel();
	}
}
