using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibCsvFiles.Models;
using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;
using Bau.Libraries.Compiler.LibInterpreter.Context.Variables;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Parameters;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Csv;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Parquet;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Repository
{
	/// <summary>
	///		Clase de lectura de los scripts
	/// </summary>
	internal class DbScriptRepository
	{
		// Constantes privadas
		private const int BatchSizeDefault = 200_000;
		private const string TagRoot = "DbScript";
		private const string TagImport = "Import";
		private const string TagFileName = "FileName";
		private const string TagSentenceBlock = "Block";
		private const string TagSentenceExecute = "Execute";
		private const string TagSentenceCopy = "Copy";
		private const string TagSentenceBulkCopy = "BulkCopy";
		private const string TagTable = "Table";
		private const string TagBatchSize = "BatchSize";
		private const string TagLoad = "Load";
		private const string TagSave = "Save";
		private const string TagSentenceException = "Exception";
		private const string TagProviderCommand = "Command";
		private const string TagArgument = "Argument";
		private const string TagSource = "Source";
		private const string TagTarget = "Target";
		private const string TagType = "Type";
		private const string TagName = "Name";
		private const string TagValue = "Value";
		private const string TagParameter = "Parameter";
		private const string TagDefault = "Default";
		private const string TagEmptyData = "EmptyData";
		private const string TagWithData = "WithData";
		private const string TagSentenceForEach = "ForEach";
		private const string TagSentenceIf = "If";
		private const string TagCondition = "Condition";
		private const string TagThen = "Then";
		private const string TagElse = "Else";
		private const string TagSentenceString = "String";
		private const string TagSentenceNumeric = "Numeric";
		private const string TagSentenceBoolean = "Boolean";
		private const string TagSentenceDate = "Date";
		private const string TagDateNow = "Now";
		private const string TagSentenceLet = "Let";
		private const string TagVariable = "Variable";
		private const string TagSentenceFor = "For";
		private const string TagSentenceWhile = "While";
		private const string TagStart = "Start";
		private const string TagEnd = "End";
		private const string TagStep = "Step";
		private const string TagSentencePrint = "Print";
		private const string TagSentenceIfExists = "IfExists";
		private const string TagSentenceBeginTransaction = "BeginTransaction";
		private const string TagSentenceCommitTransaction = "CommitTransaction";
		private const string TagSentenceRollbackTransaction = "RollbackTransaction";
		private const string TagSentenceAssertExecute = "AssertExecute";
		private const string TagWithError = "WithError";
		private const string TagRecords = "Records";
		private const string TagSentenceAssertScalar = "AssertScalar";
		private const string TagMessage = "Message";
		private const string TagResult = "Result";
		private const string TagSentenceExecuteScript = "ExecuteScript";
		private const string TagMap = "Map";
		private const string TagFrom = "From";
		private const string TagTo = "To";
		private const string TagSentenceImportCsv = "ImportCsv";
		private const string TagSentenceExportCsv = "ExportCsv";
		private const string TagSentenceExportPartitionedCsv = "ExportPartitionCsv";
		private const string TagPartitionBy = "PartitionBy";
		private const string TagSentenceImportSchemaCsv = "ImportSchemaCsv";
		private const string TagSentenceExportSchemaCsv = "ExportSchemaCsv";
		private const string TagPath = "Path";
		private const string TagColumn = "Column";
		private const string TagWithHeader = "WithHeader";
		private const string TagTypedHeader = "TypedHeader";
		private const string TagDeleteOldFiles = "DeleteOldFiles";
		private const string TagFieldsSeparator = "FieldsSeparator";
		private const string TagDecimalSeparator = "DecimalSeparator";
		private const string TagThousandsSeparator = "ThousandsSeparator";
		private const string TagDateFormat = "DateFormat";
		private const string TagValueTrue = "ValueTrue";
		private const string TagValueFalse = "ValueFalse";
		private const string TagExclude = "Exclude";
		private const string TagExcludeMode = "Mode";
		private const string TagExcludeExcept = "Except";
		private const string TagTimeout = "Timeout";
		private const string TagSentenceExportParquet = "ExportParquet";
		private const string TagSentenceImportParquet = "ImportParquet";
		private const string TagRowGroupSize = "RowGroupSize";

		/// <summary>
		///		Carga el programa de un archivo
		/// </summary>
		internal ProgramModel LoadByFile(string fileName)
		{
			return Load(new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName), System.IO.Path.GetDirectoryName(fileName));
		}

		/// <summary>
		///		Carga el programa de un texto
		/// </summary>
		internal ProgramModel LoadByText(string xml, string pathBase)
		{
			return Load(new LibMarkupLanguage.Services.XML.XMLParser().ParseText(xml), pathBase);
		}

		/// <summary>
		///		Carga el programa
		/// </summary>
		private ProgramModel Load(MLFile fileML, string pathBase)
		{
			ProgramModel program = new ProgramModel();

				// Carga las sentencias del programa
				if (fileML != null)
					foreach (MLNode rootML in fileML.Nodes)
						if (rootML.Name == TagRoot)
							program.Sentences.AddRange(LoadSentences(rootML.Nodes, pathBase));
				// Devuelve el programa cargado
				return program;
		}

		/// <summary>
		///		Carga las instrucciones de una serie de nodos
		/// </summary>
		private SentenceCollection LoadSentences(MLNodesCollection nodesML, string pathBase)
		{
			SentenceCollection sentences = new SentenceCollection();

				// Lee las instrucciones
				foreach (MLNode rootML in nodesML)
					switch (rootML.Name)
					{
						case TagImport:
								sentences.AddRange(LoadByFile(System.IO.Path.Combine(pathBase, rootML.Attributes[TagFileName].Value)).Sentences);
							break;
						case TagSentenceBlock:
								sentences.Add(LoadSentenceBlock(rootML, pathBase));
							break;
						case TagSentenceExecute:
								sentences.Add(LoadSentenceExecute(rootML));
							break;
						case TagSentenceException:
								sentences.Add(LoadSentenceException(rootML));
							break;
						case TagSentenceBulkCopy:
								sentences.Add(LoadSentenceBulkCopy(rootML));
							break;
						case TagSentenceCopy:
								sentences.Add(LoadSentenceCopy(rootML));
							break;
						case TagSentenceForEach:
								sentences.Add(LoadSentenceForEach(rootML, pathBase));
							break;
						case TagSentenceIfExists:
								sentences.Add(LoadSentenceIfExists(rootML, pathBase));
							break;
						case TagSentenceIf:
								sentences.Add(LoadSentenceIf(rootML, pathBase));
							break;
						case TagSentenceString:
								sentences.Add(LoadSentenceDeclare(rootML, VariableModel.VariableType.String));
							break;
						case TagSentenceNumeric:
								sentences.Add(LoadSentenceDeclare(rootML, VariableModel.VariableType.Numeric));
							break;
						case TagSentenceBoolean:
								sentences.Add(LoadSentenceDeclare(rootML, VariableModel.VariableType.Boolean));
							break;
						case TagSentenceDate:
								sentences.Add(LoadSentenceDeclare(rootML, VariableModel.VariableType.Date));
							break;
						case TagSentenceLet:
								sentences.Add(LoadSentenceLet(rootML));
							break;
						case TagSentenceFor:
								sentences.Add(LoadSentenceFor(rootML, pathBase));
							break;
						case TagSentenceWhile:
								sentences.Add(LoadSentenceWhile(rootML, pathBase));
							break;
						case TagSentencePrint:
								sentences.Add(LoadSentencePrint(rootML));
							break;
						case TagSentenceBeginTransaction:
								sentences.Add(LoadSentenceBatch(rootML, SentenceDataBatch.BatchCommand.BeginTransaction));
							break;
						case TagSentenceCommitTransaction:
								sentences.Add(LoadSentenceBatch(rootML, SentenceDataBatch.BatchCommand.CommitTransaction));
							break;
						case TagSentenceRollbackTransaction:
								sentences.Add(LoadSentenceBatch(rootML, SentenceDataBatch.BatchCommand.RollbackTransaction));
							break;
						case TagSentenceAssertExecute:
								sentences.Add(LoadSentenceAssertExecute(rootML));
							break;
						case TagSentenceAssertScalar:
								sentences.Add(LoadSentenceAssertScalar(rootML));
							break;
						case TagSentenceExecuteScript:
								sentences.Add(LoadSentenceExecuteScript(rootML));
							break;
						case TagSentenceImportCsv:
								sentences.Add(LoadSentenceImportCsv(rootML));
							break;
						case TagSentenceImportSchemaCsv:
								sentences.Add(LoadSentenceImportSchema(rootML));
							break;
						case TagSentenceExportSchemaCsv:
								sentences.Add(LoadSentenceExportSchema(rootML));
							break;
						case TagSentenceExportCsv:
								sentences.Add(LoadSentenceExportCsv(rootML));
							break;
						case TagSentenceExportPartitionedCsv:
								sentences.Add(LoadSentenceExportPartitionedCsv(rootML));
							break;
						case TagSentenceExportParquet:
								sentences.Add(LoadSentenceExportParquet(rootML));
							break;
						case TagSentenceImportParquet:
								sentences.Add(LoadSentenceImportParquet(rootML));
							break;
					}
				// Devuelve la colección
				return sentences;
		}

		/// <summary>
		///		Carga un bloque de sentencias
		/// </summary>
		private SentenceBase LoadSentenceBlock(MLNode rootML, string pathBase)
		{
			SentenceBlock sentence = new SentenceBlock();

				// Asigna las propiedades
				sentence.Message = rootML.Attributes[TagMessage].Value;
				sentence.Sentences.AddRange(LoadSentences(rootML.Nodes, pathBase));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de inicio o fin de lote
		/// </summary>
		private SentenceBase LoadSentenceBatch(MLNode rootML, SentenceDataBatch.BatchCommand type)
		{
			return new SentenceDataBatch
							{
								Target = rootML.Attributes[TagTarget].Value,
								Type = type
							};
		}

		/// <summary>
		///		Carga una sentencia de impresión
		/// </summary>
		private SentenceBase LoadSentencePrint(MLNode rootML)
		{
			return new SentencePrint
							{
								Message = rootML.Value
							};
		}

		/// <summary>
		///		Carga una sentencia de declaración de variables
		/// </summary>
		private SentenceBase LoadSentenceDeclare(MLNode rootML, VariableModel.VariableType type)
		{
			SentenceDeclare sentence = new SentenceDeclare();

				// Asigna las propiedades
				sentence.Type = type;
				sentence.Name = rootML.Attributes[TagName].Value;
				sentence.Value = ConvertStringValue(type, rootML.Attributes[TagValue].Value);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de asignación
		/// </summary>
		private SentenceBase LoadSentenceLet(MLNode rootML)
		{
			SentenceLet sentence = new SentenceLet();

				// Asigna las propiedades
				sentence.Type = rootML.Attributes[TagType].Value.GetEnum(VariableModel.VariableType.Unknown);
				sentence.Variable = rootML.Attributes[TagName].Value;
				sentence.Expression = rootML.Value;
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia for
		/// </summary>
		private SentenceBase LoadSentenceFor(MLNode rootML, string pathBase)
		{
			SentenceFor sentence = new SentenceFor();

				// Asigna las propiedades
				sentence.Variable = rootML.Attributes[TagVariable].Value;
				sentence.StartExpression = rootML.Attributes[TagStart].Value;
				sentence.EndExpression = rootML.Attributes[TagEnd].Value;
				sentence.StepExpression = rootML.Attributes[TagStep].Value;
				// Carga las sentencias
				sentence.Sentences.AddRange(LoadSentences(rootML.Nodes, pathBase));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga los datos de una sentencia <see cref="SentenceForEach"/>
		/// </summary>
		private SentenceBase LoadSentenceForEach(MLNode rootML, string pathBase)
		{
			SentenceForEach sentence = new SentenceForEach();

				// Carga los datos de la sentencia
				sentence.Source = rootML.Attributes[TagSource].Value;
				sentence.Command = GetProviderCommand(rootML, TagProviderCommand);
				// Carga las instrucciones a ejecutar cuando hay o no hay datos
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagEmptyData:
								sentence.SentencesEmptyData.AddRange(LoadSentences(nodeML.Nodes, pathBase));
							break;
						case TagWithData:
								sentence.SentencesWithData.AddRange(LoadSentences(nodeML.Nodes, pathBase));
							break;
					}
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Sentencia de ejecución sobre el proveedor
		/// </summary>
		private SentenceBase LoadSentenceExecute(MLNode rootML)
		{
			return new SentenceExecute
							{
								Target = rootML.Attributes[TagTarget].Value,
								Command = GetProviderCommand(rootML, TagProviderCommand)
							};
		}
		
		/// <summary>
		///		Sentencia de ejecución de un script
		/// </summary>
		private SentenceBase LoadSentenceExecuteScript(MLNode rootML)
		{
			SentenceExecuteScript sentence = new SentenceExecuteScript();

				// Asigna las propiedades
				sentence.Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull();
				sentence.FileName = rootML.Attributes[TagFileName].Value.TrimIgnoreNull();
				// Asigna los mapeos de variables
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagMap)
						sentence.Mapping.Add((nodeML.Attributes[TagVariable].Value, nodeML.Attributes[TagTo].Value));
				// Devuelve la sentencia creada
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de copia masiva
		/// </summary>
		private SentenceBulkCopy LoadSentenceBulkCopy(MLNode rootML)
		{
			SentenceBulkCopy sentence = new SentenceBulkCopy
												{
													Source = rootML.Attributes[TagSource].Value,
													Target = rootML.Attributes[TagTarget].Value,
													Table = rootML.Attributes[TagTable].Value,
													BatchSize = rootML.Attributes[TagBatchSize].Value.GetInt(30_000),
													Command = GetProviderCommand(rootML, TagProviderCommand)
												};

				// Obtiene las columnas de mapeo
				LoadMappings(sentence.Mappings, rootML);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga el diccionario de mapeo
		/// </summary>
		private void LoadMappings(Dictionary<string, string> mappings, MLNode rootML)
		{
			if (rootML.Nodes.Count != 0)
				foreach (MLNode node in rootML.Nodes)
					if (node.Name == TagMap)
						mappings.Add(node.Attributes[TagFrom].Value.TrimIgnoreNull().ToUpperInvariant(), 
									 node.Attributes[TagTo].Value.TrimIgnoreNull());
		}

		/// <summary>
		///		Carga una sentencia de copia
		/// </summary>
		private SentenceCopy LoadSentenceCopy(MLNode rootML)
		{
			return new SentenceCopy
							{
								Source = rootML.Attributes[TagSource].Value,
								Target = rootML.Attributes[TagTarget].Value,
								LoadCommand = GetProviderCommand(rootML, TagLoad),
								SaveCommand = GetProviderCommand(rootML, TagSave)
							};
		}

		/// <summary>
		///		Carga la sentencia que comprueba si existe un valor
		/// </summary>
		private SentenceBase LoadSentenceIfExists(MLNode rootML, string pathBase)
		{
			SentenceIfExists sentence = new SentenceIfExists();

				// Carga los parámetros de la sentencia
				sentence.Source = rootML.Attributes[TagSource].Value;
				sentence.Command = GetProviderCommand(rootML, TagProviderCommand);
				// Carga las instrucciones a ejecutar cuando existe o no existe el dato
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagThen:
								sentence.SentencesThen.AddRange(LoadSentences(nodeML.Nodes, pathBase));
							break;
						case TagElse:
								sentence.SentencesElse.AddRange(LoadSentences(nodeML.Nodes, pathBase));
							break;
					}
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga los datos de una sentencia de excepción
		/// </summary>
		private SentenceBase LoadSentenceException(MLNode rootML)
		{
			return new SentenceException
							{
								Message = rootML.Value
							};
		}

		/// <summary>
		///		Carga una sentencia If
		/// </summary>
		private SentenceBase LoadSentenceIf(MLNode rootML, string pathBase)
		{
			SentenceIf sentence = new SentenceIf();

				// Carga la condición
				sentence.Condition = rootML.Attributes[TagCondition].Value;
				// Carga las sentencias de la parte then y else
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagThen:
								sentence.SentencesThen.AddRange(LoadSentences(nodeML.Nodes, pathBase));
							break;
						case TagElse:
								sentence.SentencesElse.AddRange(LoadSentences(nodeML.Nodes, pathBase));
							break;
					}
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia While
		/// </summary>
		private SentenceBase LoadSentenceWhile(MLNode rootML, string pathBase)
		{
			SentenceWhile sentence = new SentenceWhile();

				// Carga la condición y las sentencias
				sentence.Condition = rootML.Attributes[TagCondition].Value;
				sentence.Sentences.AddRange(LoadSentences(rootML.Nodes, pathBase));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia para probar un comando de ejecución
		/// </summary>
		private SentenceBase LoadSentenceAssertExecute(MLNode rootML)
		{
			SentenceAssertExecute sentence = new SentenceAssertExecute();

				// Asigna las propiedades
				sentence.Target = rootML.Attributes[TagTarget].Value;
				sentence.Message = rootML.Attributes[TagMessage].Value;
				sentence.Command = GetProviderCommand(rootML, TagProviderCommand);
				sentence.WithError = rootML.Attributes[TagWithError].Value.GetBool();
				sentence.Records = rootML.Attributes[TagRecords].Value.GetInt(0);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de prueba del resultado de una consulta escalar
		/// </summary>
		private SentenceBase LoadSentenceAssertScalar(MLNode rootML)
		{
			return new SentenceAssertScalar
							{
								Target = rootML.Attributes[TagTarget].Value,
								Message = rootML.Attributes[TagMessage].Value,
								Command = GetProviderCommand(rootML, TagProviderCommand),
								Result = rootML.Attributes[TagResult].Value.GetLong(0)
							};
		}

		/// <summary>
		///		Carga una sentencia que se envía a un proveedor
		/// </summary>
		private ProviderSentenceModel GetProviderCommand(MLNode rootML, string tagCommand)
		{
			if (rootML.Nodes.Count == 0 && !string.IsNullOrWhiteSpace(rootML.Value))
				return new ProviderSentenceModel(rootML.Value.TrimIgnoreNull());
			else
			{
				ProviderSentenceModel sentence = null;

					// Primero obtiene el comando
					foreach (MLNode nodeML in rootML.Nodes)
						if (nodeML.Name == tagCommand)
							sentence = new ProviderSentenceModel(nodeML.Value);
					// Si ha generado la sentencia, añade los argumentos
					if (sentence != null)
						foreach (MLNode nodeML in rootML.Nodes)
							if (nodeML.Name == TagArgument)
								sentence.Filters.Add(LoadFilter(nodeML));
					// Devuelve el comando leido
					return sentence;
			}
		}

		/// <summary>
		///		Carga los datos de un filtro
		/// </summary>
		private FilterModel LoadFilter(MLNode rootML)
		{
			FilterModel filter = new FilterModel();

				// Añade los datos del filtro
				filter.ColumnType = rootML.Attributes[TagType].Value.GetEnum(VariableModel.VariableType.Unknown);
				filter.VariableName = rootML.Attributes[TagVariable].Value;
				filter.Parameter = rootML.Attributes[TagParameter].Value;
				if (string.IsNullOrEmpty(filter.VariableName) && !string.IsNullOrEmpty(filter.Parameter))
					filter.VariableName = filter.Parameter.Replace("@", "");
				// Obtiene el valor por defecto
				filter.Default = ConvertStringValue(filter.ColumnType, rootML.Attributes[TagDefault].Value);
				// Devuelve los datos del filtro
				return filter;
		}

		/// <summary>
		///		Convierte una cadena con un valor
		/// </summary>
		private object ConvertStringValue(VariableModel.VariableType type, string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			else
				switch (type)
				{ 
					case VariableModel.VariableType.Boolean:
						return value.GetBool();
					case VariableModel.VariableType.Date:
						if (value.EqualsIgnoreCase(TagDateNow))
							return DateTime.Now;
						else
							return value.GetDateTime();
					case VariableModel.VariableType.Numeric:
						return value.GetDouble();
					default:
						return value;
				}
		}

		/// <summary>
		///		Carga una sentencia para importar CSV
		/// </summary>
		private SentenceBase LoadSentenceImportCsv(MLNode rootML)
		{
			SentenceImportCsv sentence = new SentenceImportCsv();

				// Asigna las propiedades
				sentence.Target = rootML.Attributes[TagTarget].Value;
				sentence.FileName = rootML.Attributes[TagFileName].Value;
				sentence.Table = rootML.Attributes[TagTable].Value;
				sentence.BatchSize = rootML.Attributes[TagBatchSize].Value.GetInt(BatchSizeDefault);
				sentence.Timeout = GetTimeout(rootML, TimeSpan.FromHours(2));
				// Carga la definición
				LoadDefinitionCsv(sentence.Definition, rootML);
				// Carga los mapeos
				LoadMappings(sentence.Mappings, rootML);
				sentence.Columns.AddRange(LoadColumnsCsv(rootML));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia <see cref="SentenceImportCsvSchema"/>
		/// </summary>
		private SentenceImportCsvSchema LoadSentenceImportSchema(MLNode rootML)
		{
			SentenceImportCsvSchema sentence = new SentenceImportCsvSchema();

				// Asigna las propiedades básicas
				sentence.Target = rootML.Attributes[TagTarget].Value;
				sentence.Path = rootML.Attributes[TagPath].Value;
				sentence.BatchSize = rootML.Attributes[TagBatchSize].Value.GetInt(BatchSizeDefault);
				sentence.Timeout = GetTimeout(rootML, TimeSpan.FromHours(2));
				// Obtiene los parámetros del archivo
				LoadDefinitionCsv(sentence.Definition, rootML);
				// Obtiene las tablas excluidas
				sentence.ExcludeRules.AddRange(LoadExcludeRules(rootML));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia para exportar CSV
		/// </summary>
		private SentenceBase LoadSentenceExportCsv(MLNode rootML)
		{
			SentenceExportCsv sentence = new SentenceExportCsv();

				// Asigna las propiedades
				sentence.Source = rootML.Attributes[TagSource].Value;
				sentence.FileName = rootML.Attributes[TagFileName].Value;
				sentence.BatchSize = rootML.Attributes[TagBatchSize].Value.GetInt(BatchSizeDefault);
				sentence.Command = GetProviderCommand(rootML, TagLoad);
				// Carga la definición
				LoadDefinitionCsv(sentence.Definition, rootML);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia para exportar una serie de archivos CSV particionados
		/// </summary>
		private SentenceBase LoadSentenceExportPartitionedCsv(MLNode rootML)
		{
			SentenceExportPartitionedCsv sentence = new SentenceExportPartitionedCsv();

				// Asigna las propiedades
				sentence.Source = rootML.Attributes[TagSource].Value;
				sentence.FileName = rootML.Attributes[TagFileName].Value;
				sentence.BatchSize = rootML.Attributes[TagBatchSize].Value.GetInt(BatchSizeDefault);
				sentence.Command = GetProviderCommand(rootML, TagLoad);
				// Carga la definición
				LoadDefinitionCsv(sentence.Definition, rootML);
				// Carga las columnas de partición
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagPartitionBy && !string.IsNullOrWhiteSpace(nodeML.Attributes[TagColumn].Value))
						sentence.Columns.Add(nodeML.Attributes[TagColumn].Value.TrimIgnoreNull());
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia <see cref="SentenceExportCsvSchema"/>
		/// </summary>
		private SentenceBase LoadSentenceExportSchema(MLNode rootML)
		{
			SentenceExportCsvSchema sentence = new SentenceExportCsvSchema();

				// Asigna las propiedades básicas
				sentence.Source = rootML.Attributes[TagSource].Value;
				sentence.Path = rootML.Attributes[TagPath].Value;
				sentence.DeleteOldFiles = rootML.Attributes[TagDeleteOldFiles].Value.GetBool();
				sentence.BatchSize = rootML.Attributes[TagBatchSize].Value.GetInt(BatchSizeDefault);
				sentence.Timeout = GetTimeout(rootML, TimeSpan.FromHours(2));
				// Carga los parámetros del CSV
				LoadDefinitionCsv(sentence.Definition, rootML);
				// Carga las tablas exluidas
				sentence.ExcludeRules.AddRange(LoadExcludeRules(rootML));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga la definición del archivo CSV
		/// </summary>
		private void LoadDefinitionCsv(FileModel definition, MLNode rootML)
		{
			definition.WithHeader = rootML.Attributes[TagWithHeader].Value.GetBool(true);
			definition.TypedHeader = rootML.Attributes[TagTypedHeader].Value.GetBool(false);
			definition.Separator = rootML.Attributes[TagFieldsSeparator].Value;
			definition.DecimalSeparator = rootML.Attributes[TagDecimalSeparator].Value;
			definition.ThousandsSeparator = rootML.Attributes[TagThousandsSeparator].Value;
			definition.DateFormat = rootML.Attributes[TagDateFormat].Value;
			definition.TrueValue = rootML.Attributes[TagValueTrue].Value;
			definition.FalseValue = rootML.Attributes[TagValueFalse].Value;
		}

		/// <summary>
		///		Carga las tablas excluidas
		/// </summary>
		private SchemaExcludeRuleCollection LoadExcludeRules(MLNode rootML)
		{
			SchemaExcludeRuleCollection rules = new SchemaExcludeRuleCollection();

				// Añade las tablas excluidades
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagExclude && !string.IsNullOrWhiteSpace(nodeML.Attributes[TagTable].Value))
					{
						SchemaExcludeRule rule = new SchemaExcludeRule();

							// Asigna las propiedades
							rule.Name = nodeML.Attributes[TagTable].Value.TrimIgnoreNull();
							rule.Mode = nodeML.Attributes[TagExcludeMode].Value.GetEnum(SchemaExcludeRule.Comparison.Equals);
							rule.Except = nodeML.Attributes[TagExcludeExcept].Value.TrimIgnoreNull();
							// Si el filtro empieza o termina por asterisco, cambia el modo
							if (rule.Name.EndsWith("*") && rule.Name.StartsWith("*"))
								rule.Mode = SchemaExcludeRule.Comparison.Contains;
							else if (rule.Name.EndsWith("*"))
								rule.Mode = SchemaExcludeRule.Comparison.Start;
							else if (rule.Name.StartsWith("*"))
								rule.Mode = SchemaExcludeRule.Comparison.End;
							// Quita los asteriscos
							if (rule.Name.IndexOf("*") >= 0)
								rule.Name = rule.Name.Replace("*", "");
							// Añade la regla a la colección
							if (!string.IsNullOrWhiteSpace(rule.Name))
								rules.Add(rule);
					}
				// Devuelve la colección de tablas
				return rules;
		}

		/// <summary>
		///		Carga las columnas de un archivo CSV
		/// </summary>
		private List<ColumnModel> LoadColumnsCsv(MLNode rootML)
		{
			List<ColumnModel> columns = new List<ColumnModel>();

				// Carga las columnas
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagColumn)
						columns.Add(new ColumnModel
											{
												Name = nodeML.Attributes[TagName].Value,
												Type = nodeML.Attributes[TagType].Value.GetEnum(ColumnModel.ColumnType.String)
											}
									);
				// Devuelve las columnas cargadas
				return columns;
		}

		/// <summary>
		///		Carga una sentencia <see cref="SentenceExportParquet"/>
		/// </summary>
		private SentenceBase LoadSentenceExportParquet(MLNode rootML)
		{
			SentenceExportParquet sentence = new SentenceExportParquet();

				// Asigna las propiedades básicas
				sentence.Source = rootML.Attributes[TagSource].Value.TrimIgnoreNull();
				sentence.FileName = rootML.Attributes[TagFileName].Value.TrimIgnoreNull();
				sentence.Command = GetProviderCommand(rootML, TagLoad);
				sentence.RecordsPerBlock = rootML.Attributes[TagBatchSize].Value.GetInt(BatchSizeDefault);
				sentence.RowGroupSize = rootML.Attributes[TagRowGroupSize].Value.GetInt(45_000);
				sentence.Timeout = GetTimeout(rootML, TimeSpan.FromHours(2));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia <see cref="SentenceImportParquet"/>
		/// </summary>
		private SentenceBase LoadSentenceImportParquet(MLNode rootML)
		{
			SentenceImportParquet sentence = new SentenceImportParquet();

				// Asigna las propiedades básicas
				sentence.Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull();
				sentence.FileName = rootML.Attributes[TagFileName].Value.TrimIgnoreNull();
				sentence.Table = rootML.Attributes[TagTable].Value.TrimIgnoreNull();
				sentence.RecordsPerBlock = rootML.Attributes[TagBatchSize].Value.GetInt(BatchSizeDefault);
				sentence.Timeout = GetTimeout(rootML, TimeSpan.FromHours(2));
				// Obtiene los mapeos
				LoadMappings(sentence.Mappings, rootML);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Obtiene el timeout definido en el nodo
		/// </summary>
		private TimeSpan GetTimeout(MLNode rootML, TimeSpan defaultTimeOut)
		{
			TimeSpan? timeout = null;
			string attribute = rootML.Attributes[TagTimeout].Value;

				// Interpreta la cadena de timeout
				if (!string.IsNullOrWhiteSpace(attribute) && attribute.Length > 1)
				{
					string time = attribute.Substring(attribute.Length - 1).ToUpper();
					int value = attribute.Substring(0, attribute.Length - 1).GetInt(0);

						if (value != 0)
							switch (time)
							{
								case "D":
										if (value < 5)
											timeout = TimeSpan.FromDays(value);
										else
											timeout = TimeSpan.FromDays(4);
									break;
								case "H":
										timeout = TimeSpan.FromHours(value);
									break;
								case "M":
										timeout = TimeSpan.FromMinutes(value);
									break;
								case "S":
										timeout = TimeSpan.FromSeconds(value);
									break;
							}
				}
				// Devuelve el timeout de la sentencia
				return timeout ?? defaultTimeOut;
		}
	}
}
