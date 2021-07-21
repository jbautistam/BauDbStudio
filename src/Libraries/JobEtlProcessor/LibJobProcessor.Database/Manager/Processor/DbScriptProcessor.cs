using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.Compiler.LibInterpreter.Context.Variables;
using Bau.Libraries.Compiler.LibInterpreter.Processor;
using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibBlobStorage;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDbScripts.Parser;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Context;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Parameters;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor
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
		internal async Task ProcessAsync(ProgramModel program, System.Threading.CancellationToken cancelationToken)
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
			await ExecuteAsync(program.Sentences, cancelationToken);
		}

		/// <summary>
		///		Ejecuta una serie de sentencias
		/// </summary>
		protected override async Task ExecuteAsync(SentenceBase abstractSentence, System.Threading.CancellationToken cancellationToken)
		{
			switch (abstractSentence)
			{
				case SentenceBlock sentence:
						await ExecuteBlockAsync(sentence, cancellationToken);
					break;
				case SentenceForEach sentence:
						await ExecuteForEachAsync(sentence, cancellationToken);
					break;
				case SentenceIfExists sentence:
						await ExecuteIfExistsAsync(sentence, cancellationToken);
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
				case SentenceFileExportPartitioned sentence:
						await ExecuteExportPartitionedAsnc(sentence, cancellationToken);
					break;
				case SentenceFileImport sentence:
						await ExecuteImportFileAsync(sentence, cancellationToken);
					break;
				case SentenceFileExport sentence:
						await ExecuteExportFileAsync(sentence, cancellationToken);
					break;
				case SentenceFileImportSchema sentence:
						await ExecuteImportCsvSchemaAsync(sentence, cancellationToken);
					break;
				case SentenceFileExportSchema sentence:
						await ExecuteExportCsvSchemaAsync(sentence, cancellationToken);
					break;
			}
		}

		/// <summary>
		///		Ejecuta una sentencia de bloque
		/// </summary>
		private async Task ExecuteBlockAsync(SentenceBlock sentence, System.Threading.CancellationToken cancellationToken)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Start block {sentence.Message}"))
			{
				await ExecuteWithContextAsync(sentence.Sentences, cancellationToken);
			}
		}

		/// <summary>
		///		Ejecuta una sentencia foreach
		/// </summary>
		private async Task ExecuteForEachAsync(SentenceForEach sentence, CancellationToken cancellationToken)
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
													await ExecuteAsync(sentence.SentencesWithData, cancellationToken);
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
										await ExecuteAsync(sentence.SentencesEmptyData, cancellationToken);
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
									AddError($"Error when execute bulkCopy. {exception.Message}", exception);
									AddDebug($"Source: {sentence.Source}. Target: {sentence.Target}");
									AddDebug($"Command: {command.Sql}");
								}
					}
			}
		}

		/// <summary>
		///		Ejecuta la sentencia que comprueba si existe un valor en la tabla de datos
		/// </summary>
		private async Task ExecuteIfExistsAsync(SentenceIfExists sentence, CancellationToken cancellationToken)
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
								bool exists = ExistsData(provider, command, sentence.Command.Timeout);

									if (!Stopped)
									{
										if (exists && sentence.SentencesThen.Count > 0)
											await ExecuteWithContextAsync(sentence.SentencesThen, cancellationToken);
										else if (!exists && sentence.SentencesElse.Count > 0)
											await ExecuteWithContextAsync(sentence.SentencesElse, cancellationToken);
									}
							}
					}
			}
		}

		/// <summary>
		///		Comprueba si existen datas para un consulta
		/// </summary>
		private bool ExistsData(ProviderModel provider, CommandModel command, TimeSpan timeout)
		{
			bool exists = false;

				// Comprueba si existen datos
				try
				{
					using (IDataReader reader = provider.OpenReader(command, timeout))
					{
						if (reader.Read())
							exists = true;
					}
				}
				catch (Exception exception)
				{
					AddError($"Error when check data exists {command.Sql}", exception);
				}
				// Devuelve el valor que indica si existen datos
				return exists;
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
						string fileName = Manager.Step.Project.GetFullFileName(sentence.FileName);

							if (!System.IO.File.Exists(fileName))
								AddError($"Cant find the file '{fileName}'");
							else if (!sentence.MustParse)
								ExecuteScriptSqlRaw(provider, fileName, sentence.Timeout, sentence.SkipParameters);
							else
								ExecuteScriptSqlParsed(provider, fileName, sentence);
					}
			}
		}

		/// <summary>
		///		Ejecuta un script SQL (sin interpretarlo)
		/// </summary>
		private void ExecuteScriptSqlRaw(ProviderModel provider, string fileName, TimeSpan timeout, bool skipParameters)
		{
			string error = string.Empty;

				// Ejecuta el comando completo del archivo SQL
				try
				{
					string sql = LibHelper.Files.HelperFiles.LoadTextFile(fileName);

						provider.Execute(CreateDataProviderCommand(sql, timeout, skipParameters));
				}
				catch (Exception exception)
				{
					error = $"Error when execute script {System.IO.Path.GetFileName(fileName)}. {exception.Message}";
				}
				// Añade el error
				if (!string.IsNullOrWhiteSpace(error))
					AddError(error);
		}

		/// <summary>
		///		Ejecuta un script SQL interpretándolo antes
		/// </summary>
		private void ExecuteScriptSqlParsed(ProviderModel provider, string fileName, SentenceExecuteScript sentence)
		{
			List<SqlSectionModel> sections = new SqlParser().TokenizeByFile(fileName, MapVariables(GetVariables(), sentence.Mapping), out string error);

				// Si no hay ningún error, ejecuta el script
				if (string.IsNullOrWhiteSpace(error))
				{
					// Recorre las seccionaes
					foreach (SqlSectionModel section in sections)
						if (string.IsNullOrWhiteSpace(error) && section.Type == SqlSectionModel.SectionType.Sql && 
								!string.IsNullOrWhiteSpace(section.Content))
							try
							{
								provider.Execute(CreateDataProviderCommand(section.Content, sentence.Timeout));
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
			string sql = new SqlParser().ParseCommand(sentence.Sql, GetVariables().ToDictionary(), out error);
			CommandModel command = null;

				// Ejecuta el comando si no ha habido ningún error
				if (string.IsNullOrWhiteSpace(error))
				{
					// Genera el comando
					command = new CommandModel(sql, sentence.Timeout);
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
		internal CommandModel CreateDataProviderCommand(string sql, TimeSpan timeout, bool skipParameters = false)
		{
			CommandModel command = new CommandModel(sql, timeout);

				// Añade las variables del contexto
				if (!skipParameters)
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
						block.Assert(result != sentence.Records, $"{sentence.Message} - Result: {result} - Should be {sentence.Records}");
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

										// Lanza el resultado de la prueba
										block.Assert(result == null || ((result as long?) ?? 0) != sentence.Result,
													 $"{sentence.Message} - Result: {result} - Should be {sentence.Result}");
								}
								catch (Exception exception)
								{
									block.Assert(true, $"Error when execute command. {exception.Message}");
								}
					}
			}
		}

		/// <summary>
		///		Ejecuta la sentencia de importación de un archivo
		/// </summary>
		private async Task ExecuteExportFileAsync(SentenceFileExport sentence, CancellationToken cancellationToken)
		{
			if (!await new Managers.FileControllers.ExportFileController(this).ExecuteAsync(sentence, cancellationToken))
				AddError($"Error when export to file. '{sentence.FileName}'");
		}

		/// <summary>
		///		Ejecuta la sentencia de exportación de un archivo
		/// </summary>
		private async Task ExecuteImportFileAsync(SentenceFileImport sentence, CancellationToken cancellationToken)
		{
			if (!await new Managers.FileControllers.ImportFileController(this).ExecuteAsync(sentence, cancellationToken))
				AddError($"Errow when import file '{sentence.FileName}' to table '{sentence.Table}'");
		}

		/// <summary>
		///		Ejecuta la importación de archivos CSV a las tablas de exquema de base de datos
		/// </summary>
		private async Task ExecuteImportCsvSchemaAsync(SentenceFileImportSchema sentence, CancellationToken cancellationToken)
		{
			if (!await new Managers.FileControllers.ImportFileSchemaController(this).ExecuteAsync(sentence, cancellationToken))
				AddError($"Error when execute import schema files to '{sentence.Target}'");
		}

		/// <summary>
		///		Ejecuta una exportación a una serie de archivos particionando el origen de datos
		/// </summary>
		private async Task ExecuteExportPartitionedAsnc(SentenceFileExportPartitioned sentence, CancellationToken cancellationToken)
		{
			if (!await new Managers.FileControllers.ExportFilePartitionedController(this).ExecuteAsync(sentence, cancellationToken))
				AddError($"Error when execute export partitioned sentence. File: '{sentence.FileName}'");
		}

		/// <summary>
		///		Ejecuta la exportación de archivos CSV a las tablas de exquema de base de datos
		/// </summary>
		private async Task ExecuteExportCsvSchemaAsync(SentenceFileExportSchema sentence, CancellationToken cancellationToken)
		{
			if (!await new Managers.FileControllers.ExportFileSchemaController(this).ExecuteAsync(sentence, cancellationToken))
				AddError($"Error when execute export database to files from '{sentence.Source}'");
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
			key = ApplyVariablesToKey(key);
			// Devuelve el proveedor
			return Manager.DataProviderManager.GetProvider(key);
		}

		/// <summary>
		///		Manager del storage
		/// </summary>
		internal ICloudStorageManager GetCloudStorageManager(string key)
		{
			// Si la clave es una variable, sustituye por el contenido de la variable
			key = ApplyVariablesToKey(key);
			// Devuelve el storage adecuado
			if (!Manager.StorageConnectionStrings.ContainsKey(key))
				throw new NotImplementedException($"Can't find the blob storage connection string with key '{key}'");
			else
				return new StorageManager().OpenAzureStorageBlob(Manager.StorageConnectionStrings[key]);
		}

		/// <summary>
		///		Aplica las variables a la clave solicitada
		/// </summary>
		private string ApplyVariablesToKey(string value)
		{
			// Si la clave es una variable, sustituye por el contenido de la variable
			if (!string.IsNullOrWhiteSpace(value) && value.StartsWith("{{") && value.EndsWith("}}"))
			{
				VariableModel variable = Context.Actual.VariablesTable.GetIfExists(value.Substring(2, value.Length - 4));

					if (variable != null)
						value = variable.Value as string;
			}
			// Devuelve la clave
			return value;
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
			Manager.Logger.Default.LogItems.Debug($"{header}. Sql {command.Sql}", fileName, methodName, lineNumber);
			foreach (KeyValuePair<string, object> parameter in command.Parameters)
				Manager.Logger.Default.LogItems.Debug($"\t{parameter.Key}: {parameter.Value}");
		}

		/// <summary>
		///		Añade la depuración de una sentencia
		/// </summary>
		private void AddDebug(string header, ProviderSentenceModel command, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			Manager.Logger.Default.LogItems.Debug($"{header}. Sql {command.Sql}", fileName, methodName, lineNumber);
			foreach (FilterModel filter in command.Filters)
				Manager.Logger.Default.LogItems.Debug($"\t{filter.Parameter}: {filter.Default}");
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