using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

using Bau.Libraries.Compiler.LibInterpreter.Context.Variables;
using Bau.Libraries.Compiler.LibInterpreter.Processor;
using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDbScripts.Manager.Processor.Context;
using Bau.Libraries.LibDbScripts.Manager.Processor.Sentences;
using Bau.Libraries.LibDbScripts.Manager.Processor.Sentences.Csv;
using Bau.Libraries.LibDbScripts.Manager.Processor.Sentences.Parameters;
using Bau.Libraries.LibDbScripts.Manager.Processor.Sentences.Parquet;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibDbScripts.Manager.Processor
{
	/// <summary>
	///		Clase para lectura y ejecución de un script sobre base de datos
	/// </summary>
	internal class DbScriptProcessor : ProgramProcessor
	{   
		internal DbScriptProcessor(DbScriptManager manager)
		{
			Manager = manager;
		}

		/// <summary>
		///		Procesa el programa
		/// </summary>
		internal void Process(ProgramModel program)
		{
			// Inicializa el generador base
			Initialize();
			// Inicializa las transacciones
			Transactions.Clear();
			// Añade las variables iniciales
			foreach ((string key, object value) in Manager.Parameters.Enumerate())
				Context.Actual.VariablesTable.Add(key, value);
			Context.Actual.VariablesTable.Add("Today", DateTime.Now);
			// Ejecuta el programa
			Execute(program.Sentences);
		}

		/// <summary>
		///		Ejecuta una serie de sentencias
		/// </summary>
		protected override void Execute(SentenceBase abstractSentence)
		{
			switch (abstractSentence)
			{
				case SentenceBlock sentence:
						ExecuteBlock(sentence);
					break;
				case SentenceForEach sentence:
						ExecuteForEach(sentence);
					break;
				case SentenceIfExists sentence:
						ExecuteIfExists(sentence);
					break;
				case SentenceExecute sentence:
						ExecuteDataCommand(sentence);
					break;
				case SentenceExecuteScript sentence:
						ExecuteScriptSql(sentence);
					break;
				case SentenceDataBatch sentence:
						ExecuteDataBatch(sentence);
					break;
				case SentenceBulkCopy sentence:
						ExecuteBulkCopy(sentence);
					break;
				case SentenceAssertExecute sentence:
						ExecuteAssertExecute(sentence);
					break;
				case SentenceAssertScalar sentence:
						ExecuteAssertScalar(sentence);
					break;
				case SentenceImportCsv sentence:
						ExecuteImportCsv(sentence);
					break;
				case SentenceImportCsvSchema sentence:
						ExecuteImportCsvSchema(sentence);
					break;
				case SentenceExportCsv sentence:
						ExecuteExportCsv(sentence);
					break;
				case SentenceExportPartitionedCsv sentence:
						ExecuteExportPartitionedCsv(sentence);
					break;
				case SentenceExportCsvSchema sentence:
						ExecuteExportCsvSchema(sentence);
					break;
				case SentenceExportParquet sentence:
						ExecuteExportParquet(sentence);
					break;
				case SentenceImportParquet sentence:
						ExecuteImportParquet(sentence);
					break;
			}
		}

		/// <summary>
		///		Ejecuta una sentencia de bloque
		/// </summary>
		private void ExecuteBlock(SentenceBlock sentence)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Start block {sentence.Name}"))
			{
				ExecuteWithContext(sentence.Sentences);
			}
		}

		/// <summary>
		///		Ejecuta una sentencia foreach
		/// </summary>
		private void ExecuteForEach(SentenceForEach sentence)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Start for each"))
			{
				ProviderModel provider = GetProvider(sentence.Source);

					if (provider == null)
						AddError($"Can't find provider {sentence.Source}");
					else
					{
						CommandModel command = ConvertProviderCommand(sentence.Command, out string error);

							if (!string.IsNullOrWhiteSpace(error))
								AddError($"Error when convert command. {error}");
							else
							{
								int startRow = 0;

									// Ejecuta la consulta sobre el proveedor
									try
									{
										foreach (DataTable table in GetDataTable(provider, command))
											if (table.Rows.Count > 0)
											{
												// Ejecuta las instrucciones
												foreach (DataRow row in table.Rows)
												{
													// Crea un contexto
													Context.Add();
													// Añade el índice de fila a la lista de variables
													Context.Actual.VariablesTable.Add("RowIndex", startRow + table.Rows.IndexOf(row));
													// Añade las columnas
													for (int index = 0; index < table.Columns.Count; index++)
														if (row.IsNull(index) || row[index] == null)
															Context.Actual.VariablesTable.Add(table.Columns[index].ColumnName, null);
														else
															Context.Actual.VariablesTable.Add(table.Columns[index].ColumnName, row[index]);
													// Ejecuta las sentencias
													Execute(sentence.SentencesWithData);
													// Limpia el contexto
													Context.Pop();
												}
												// Añade el total de filas
												startRow += table.Rows.Count;
											}
									}
									catch (Exception exception)
									{
										AddError($"Error when load data. {exception.Message}");
									}
									// Si no se han encontrado datos, ejecuta las sentencias adecuadas
									if (startRow == 0)
										Execute(sentence.SentencesEmptyData);
							}
					}
			}
		}

		/// <summary>
		///		Ejecuta una sentencia de copia masiva
		/// </summary>
		private void ExecuteBulkCopy(SentenceBulkCopy sentence)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Start bulckcopy to table {sentence.Table}"))
			{
				ProviderModel source = GetProvider(sentence.Source);
				ProviderModel target = GetProvider(sentence.Target);

					if (source == null)
						AddError($"Can't find source provider: {sentence.Source}");
					else if (target == null)
						AddError($"Can't find target provider: {sentence.Target}");
					else if (string.IsNullOrWhiteSpace(sentence.Table))
						AddError("The target table is undefined at BulkCopy sentence");
					else
					{
						CommandModel command = ConvertProviderCommand(sentence.Command, out string error);

							if (!string.IsNullOrWhiteSpace(error))
								AddError($"Error when convert command. {error}");
							else // Ejecuta la consulta sobre el proveedor
								try
								{
									using (IDataReader reader = source.OpenReader(command, TimeSpan.FromMinutes(10)))
									{
										target.BulkCopy(reader, sentence.Table, sentence.Mappings, sentence.BatchSize, TimeSpan.FromMinutes(15));
									}
								}
								catch (Exception exception)
								{
									AddError($"Error when execute bulkCopy. {exception.Message}");
								}
					}
			}
		}

		/// <summary>
		///		Ejecuta la sentencia que comprueba si existe un valor en la tabla de datos
		/// </summary>
		private void ExecuteIfExists(SentenceIfExists sentence)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Start if exists"))
			{
				ProviderModel provider = GetProvider(sentence.Source);

					if (provider == null)
						AddError($"Can't find provider {sentence.Source}");
					else
					{
						CommandModel command = ConvertProviderCommand(sentence.Command, out string error);

							// Log
							AddDebug("ExecuteIfExists", sentence.Command);
							// Ejecuta el comando
							if (!string.IsNullOrWhiteSpace(error))
								AddError($"Error when convert command. {error}");
							else
							{
								IEnumerator<DataTable> tableEnumerator = GetDataTable(provider, command).GetEnumerator();

									if (tableEnumerator.MoveNext())
										try
										{
											DataTable table = tableEnumerator.Current;

												if (table.Rows.Count > 0 && sentence.SentencesThen.Count > 0)
													ExecuteWithContext(sentence.SentencesThen);
												else if (table.Rows.Count == 0 && sentence.SentencesElse.Count > 0)
													ExecuteWithContext(sentence.SentencesElse);
										}
										catch (Exception exception)
										{
											AddError($"Error when load data. {exception.Message}");
										}
									else if (sentence.SentencesElse.Count > 0) // ... no había ningún recordset para la tabla
										ExecuteWithContext(sentence.SentencesElse);
							}
					}
			}
		}

		/// <summary>
		///		Obtiene la tabla de datos
		/// </summary>
		private IEnumerable<DataTable> GetDataTable(ProviderModel provider, CommandModel command)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Read datatable"))
			{
				int pageIndex = 0;

					// Carga los datos
					foreach (DataTable table in provider.LoadData(command))
					{
						// Log
						AddDebug($"Reading page {++pageIndex}", command);
						// Devuelve la tabla
						yield return table;
					}
			}
		}

		/// <summary>
		///		Ejecuta una sentencia de ejecución de comando de datos sobre el proveedor
		/// </summary>
		private void ExecuteDataCommand(SentenceExecute sentence)
		{
			(int _, string error) = ExecuteDataCommand(sentence.Target, sentence.Command);

				if (!string.IsNullOrWhiteSpace(error))
					AddError(error);
		}

		/// <summary>
		///		Ejecuta un archivo de script SQL
		/// </summary>
		private void ExecuteScriptSql(SentenceExecuteScript sentence)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Execute script {sentence.FileName}"))
			{
				ProviderModel provider = GetProvider(sentence.Target);

					if (provider == null)
						AddError($"Can't find the provider. Key: '{sentence.Target}'");
					else if (string.IsNullOrWhiteSpace(sentence.FileName))
						AddError($"The script filename is not defined");
					else
					{
						string fileName = Manager.GetFullFileName(sentence.FileName);

							if (!System.IO.File.Exists(fileName))
								AddError($"Cant find the file '{fileName}'");
							else 
							{
								List<Parser.SqlSectionModel> sections = new Parser.SqlParser().TokenizeByFile(fileName, MapVariables(GetVariables(), sentence.Mapping), out string error);

									// Si no hay ningún error, ejecuta el script
									if (string.IsNullOrWhiteSpace(error))
									{
										// Recorre las seccionaes
										foreach (Parser.SqlSectionModel section in sections)
											if (string.IsNullOrWhiteSpace(error) && section.Type == Parser.SqlSectionModel.SectionType.Sql && 
													!string.IsNullOrWhiteSpace(section.Content))
												try
												{
													provider.Execute(CreateDataProviderCommand(section.Content));
												}
												catch (Exception exception)
												{
													error = $"Error when execute script {System.IO.Path.GetFileName(fileName)}. {exception.Message}";
												}
									}
									// Añade el error
									if (!string.IsNullOrWhiteSpace(error))
										AddError(error);
							}
					}
			}
		}

		/// <summary>
		///		Mapea las variables
		/// </summary>
		private Dictionary<string, object> MapVariables(NormalizedDictionary<object> parameters, List<(string variable, string to)> mappings)
		{
			Dictionary<string, object> result = new Dictionary<string, object>();

				// Mapea las variables al nuevo diccionario
				if (mappings == null || mappings.Count == 0)
					result = parameters.ToDictionary();
				else
					foreach ((string variable, string to) in mappings)
						if (parameters.TryGetValue(variable, out object value))
							result.Add(to, value);
						else
							result.Add(to, null);
				// Devuelve el diccionario convertido
				return result;
		}

		/// <summary>
		///		Convierte el comando del proveedor
		/// </summary>
		internal CommandModel ConvertProviderCommand(ProviderSentenceModel sentence, out string error)
		{
			string sql = new Parser.SqlParser().ParseCommand(sentence.Sql, GetVariables().ToDictionary(), out error);
			CommandModel command = null;

				if (string.IsNullOrWhiteSpace(error))
				{
					// Genera el comando
					command = new CommandModel(sql);
					// Añade los filtros y las variables del contexto
					if (string.IsNullOrWhiteSpace(error))
					{
						// Añade los filtros
						foreach (FilterModel filter in sentence.Filters)
							command.Parameters.Add(filter.Parameter, Context.Actual.VariablesTable.Get(filter.VariableName).Value ?? filter.Default);
						// Añade las variables del contexto
						foreach (KeyValuePair<string, VariableModel> variable in Context.Actual.GetVariablesRecursive().GetAll())
							if (!command.Parameters.ContainsKey(variable.Key))
								command.Parameters.Add(variable.Key, variable.Value.Value);
					}
				}
				// Devuelve el comando del proveedor
				return command;
		}

		/// <summary>
		///		Convierte una cadena SQL en un comando del proveedor
		/// </summary>
		internal CommandModel CreateDataProviderCommand(string sql)
		{
			CommandModel command = new CommandModel(sql);

				// Añade las variables del contexto
				foreach (KeyValuePair<string, VariableModel> variable in Context.Actual.GetVariablesRecursive().GetAll())
					if (!command.Parameters.ContainsKey(variable.Key))
						command.Parameters.Add(variable.Key, variable.Value.Value);
				// Devuelve el comando del proveedor
				return command;
		}

		/// <summary>
		///		Ejecuta una sentencia de lote
		/// </summary>
		private void ExecuteDataBatch(SentenceDataBatch sentence)
		{
			switch (sentence.Type)
			{
				case SentenceDataBatch.BatchCommand.BeginTransaction:
						ExecuteBeginTransaction(sentence.Target);
					break;
				case SentenceDataBatch.BatchCommand.CommitTransaction:
						ExecuteCommitTransaction(sentence.Target);
					break;
				case SentenceDataBatch.BatchCommand.RollbackTransaction:
						ExecuteRollbackTransaction(sentence.Target);
					break;
			}
		}

		/// <summary>
		///		Abre una transacción
		/// </summary>
		private void ExecuteBeginTransaction(string providerKey)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Opening transaction for provider {providerKey}"))
			{
				ProviderModel provider = GetProvider(providerKey);

					// Abre las transacciones para el proveedor
					if (provider == null)
						AddError($"Can't find the provider. Key: '{providerKey}'");
					else
						Transactions.Add(provider);
			}
		}

		/// <summary>
		///		Ejecuta el commit de una transacción
		/// </summary>
		private void ExecuteCommitTransaction(string providerKey)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Commit transaction for provider {providerKey}"))
			{
				BatchTransactionModel transaction = Transactions.Get(providerKey);

					if (transaction == null)
						AddError($"Can't find an open transaction for provider {providerKey}");
					else
					{
						// Ejecuta los comandos
						try
						{
							transaction.Provider.Execute(transaction.Commands);
						}
						catch (Exception exception)
						{
							AddError($"Error when execute commands batch. {exception.Message}");
						}
						// Borra la transacción
						Transactions.Remove(providerKey);
					}
			}
		}

		/// <summary>
		///		Ejecuta el rollback de una transacción
		/// </summary>
		private void ExecuteRollbackTransaction(string providerKey)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Rollback transaction for provider {providerKey}"))
			{
				BatchTransactionModel transaction = Transactions.Get(providerKey);

					if (transaction == null)
						AddError($"Can't find an open transaction for provider {providerKey}");
					else
						Transactions.Remove(providerKey);
			}
		}

		/// <summary>
		///		Ejecuta una sentencia de prueba de ejecución
		/// </summary>
		private void ExecuteAssertExecute(SentenceAssertExecute sentence)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Start assert command execute"))
			{
				(int result, string error) = ExecuteDataCommand(sentence.Target, sentence.Command);

					// Comprueba la ejecución
					if (!string.IsNullOrWhiteSpace(error))
						block.Assert(!sentence.WithError, $"Error found at sentence: {error}");
					else
						block.Assert(result != sentence.Records, $"Result: {result} - Should be {sentence.Records}");
			}
		}

		/// <summary>
		///		Ejecuta una sentencia de prueba de una consulta con un resultado escalar
		/// </summary>
		private void ExecuteAssertScalar(SentenceAssertScalar sentence)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Start assert scalar"))
			{
				ProviderModel provider = GetProvider(sentence.Target);

					if (provider == null)
						block.Assert(true, $"Can't find the provider. Key: '{sentence.Target}'");
					else
					{
						CommandModel command = ConvertProviderCommand(sentence.Command, out string error);

							// Log
							AddDebug("Execute", sentence.Command);
							// Ejecuta el comando
							if (!string.IsNullOrEmpty(error))
								block.Assert(true, $"Error when convert command {error}");
							else
								try
								{
									object result = provider.ExecuteScalar(command);

										// Lanza le resultado de la prueba
										block.Assert(result == null || ((result as long?) ?? 0) != sentence.Result, 
													 $"Result: {result} - Should be {sentence.Result}");
								}
								catch (Exception exception)
								{
									block.Assert(true, $"Error when execute command. {exception.Message}");
								}
					}
			}
		}

		/// <summary>
		///		Ejecuta una importación de CSV
		/// </summary>
		private void ExecuteImportCsv(SentenceImportCsv sentence)
		{
			if (!new Managers.Csv.ImportCsvManager(this).Execute(sentence))
				AddError($"Error when execute import sentence. File: {sentence.FileName}");
		}

		/// <summary>
		///		Ejecuta la importación de archivos CSV a las tablas de exquema de base de datos
		/// </summary>
		private void ExecuteImportCsvSchema(SentenceImportCsvSchema sentence)
		{
			if (!new Managers.Csv.ImportSchemaCsvManager(this).Execute(sentence))
				AddError($"Error when execute import CSV files to '{sentence.Target}'");
		}

		/// <summary>
		///		Ejecuta una exportación a un archivo CSV
		/// </summary>
		private void ExecuteExportCsv(SentenceExportCsv sentence)
		{
			if (!new Managers.Csv.ExportCsvManager(this).Execute(sentence))
				AddError($"Error when execute export sentence. File: '{sentence.FileName}'");
		}

		/// <summary>
		///		Ejecuta una exportación a una serie de archivos CSV particionando el origen de datos
		/// </summary>
		private void ExecuteExportPartitionedCsv(SentenceExportPartitionedCsv sentence)
		{
			if (!new Managers.Csv.ExportPartitionedCsvManager(this).Execute(sentence))
				AddError($"Error when execute export sentence. File: '{sentence.FileName}'");
		}

		/// <summary>
		///		Ejecuta la exportación de archivos CSV a las tablas de exquema de base de datos
		/// </summary>
		private void ExecuteExportCsvSchema(SentenceExportCsvSchema sentence)
		{
			if (!new Managers.Csv.ExportSchemaCsvManager(this).Execute(sentence))
				AddError($"Error when execute export database to CSV from '{sentence.Source}'");
		}

		/// <summary>
		///		Ejecuta la sentencia de importación de un archivo parquet
		/// </summary>
		private void ExecuteExportParquet(SentenceExportParquet sentence)
		{
			if (!new Managers.ParquetFiles.ExportParquetController(this).Execute(sentence))
				AddError($"Error when export to parquet file. '{sentence.FileName}'");
		}

		/// <summary>
		///		Ejecuta la sentencia de exportación de un archivo parquet
		/// </summary>
		private void ExecuteImportParquet(SentenceImportParquet sentence)
		{
			if (!new Managers.ParquetFiles.ImportParquetController(this).Execute(sentence))
				AddError($"Errow when import to parquet file from '{sentence.Table}'");
		}

		/// <summary>
		///		Ejecuta un comando sobre el proveedor
		/// </summary>
		private (int result, string error) ExecuteDataCommand(string target, ProviderSentenceModel providerCommand)
		{
			int result = 0;
			string error;

				// Ejecuta el comando
				using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Start execute data command"))
				{
					ProviderModel provider = GetProvider(target);

						// Ejecuta el comando
						if (provider == null)
							error = $"Can't find the provider. Key: '{target}'";
						else
						{
							CommandModel command = ConvertProviderCommand(providerCommand, out error);

								// Log
								AddDebug("Execute", providerCommand);
								// Ejecuta el comando
								if (string.IsNullOrEmpty(error))
								{
									if (Transactions.Exists(provider))
									{
										AddDebug("Add command to batch", command);
										Transactions.Add(provider, command);
									}
									else
									{
										AddDebug("Execute (converted)", command);
										try
										{
											result = provider.Execute(command);
										}
										catch (Exception exception)
										{
											error = $"Error when execute command. {exception.Message}";
										}
									}
								}
					}
				}
				// Devuelve el error
				return (result, error);
		}

		/// <summary>
		///		Obtiene el proveedor asociado a una clave
		/// </summary>
		internal ProviderModel GetProvider(string key)
		{
			// Si la clave es una variable, sustituye por el contenido de la variable
			if (!string.IsNullOrWhiteSpace(key) && key.StartsWith("{{") && key.EndsWith("}}"))
			{
				VariableModel variable = Context.Actual.VariablesTable.GetIfExists(key.Substring(2, key.Length - 4));

					if (variable != null)
						key = variable.Value as string;
			}
			// Devuelve el proveedor
			return Manager.DataProviderManager.GetProvider(key);
		}

		/// <summary>
		///		Convierte las variables del contexto
		/// </summary>
		private NormalizedDictionary<object> GetVariables()
		{
			NormalizedDictionary<object> variables = new NormalizedDictionary<object>();

				// Convierte las variables del contexto actual
				foreach (KeyValuePair<string, VariableModel> variable in Context.Actual.GetVariablesRecursive().GetAll())
					variables.Add(variable.Key, variable.Value.Value);
				// Devuelve las variables
				return variables;
		}

		/// <summary>
		///		Añade un mensaje de depuración
		/// </summary>
		protected override void AddDebug(string message, [CallerFilePath] string fileName = null, 
										 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			Manager.Logger.Default.LogItems.Debug(message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade el mensaje de depuración de una sentencia de comando sobre el proveedor
		/// </summary>
		private void AddDebug(string header, CommandModel command, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			Manager.Logger.Default.LogItems.Debug(header, fileName, methodName, lineNumber);
			Manager.Logger.Default.LogItems.AddParameter("Sql", command.Sql);
			foreach (KeyValuePair<string, object> paramenter in command.Parameters)
				Manager.Logger.Default.LogItems.AddParameter("\t" + paramenter.Key, paramenter.Value);
		}

		/// <summary>
		///		Añade la depuración de una sentencia
		/// </summary>
		private void AddDebug(string header, ProviderSentenceModel command, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			Manager.Logger.Default.LogItems.Debug(header, fileName, methodName, lineNumber);
			Manager.Logger.Default.LogItems.AddParameter("Sql", command.Sql);
			foreach (FilterModel filter in command.Filters)
				Manager.Logger.Default.LogItems.AddParameter("\t" + filter.Parameter, filter.Default);
		}

		/// <summary>
		///		Añade un mensaje informativo
		/// </summary>
		protected override void AddInfo(string message, [CallerFilePath] string fileName = null, 
										[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			Manager.Logger.Default.LogItems.Info(message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje a la consola de salida
		/// </summary>
		protected override void AddConsoleOutput(string message, [CallerFilePath] string fileName = null, 
												 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			Manager.Logger.Default.LogItems.Console(message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un error y detiene la compilación si es necesario
		/// </summary>
		protected override void AddError(string error, Exception exception = null, [CallerFilePath] string fileName = null, 
										 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			// Añade el mensaje de error
			Manager.Logger.Default.LogItems.Error(error, exception, fileName, methodName, lineNumber);
			Manager.Errors.Add(error);
			// Detiene la compilación
			Stopped = true;
		}

		/// <summary>
		///		Manager principal de ejecución de scripts
		/// </summary>
		internal DbScriptManager Manager { get; }

		/// <summary>
		///		Transacciones activas en la ejecución del script
		/// </summary>
		private BatchTransactionModelCollection Transactions { get; } = new BatchTransactionModelCollection();
	}
}