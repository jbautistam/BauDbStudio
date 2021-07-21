using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibCsvFiles.Models;
using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;
using Bau.Libraries.Compiler.LibInterpreter.Context.Variables;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Parameters;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files;

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
		private const string TagMustParse = "MustParse";
		private const string TagSkipParameters = "SkipParameters";
		private const string TagMap = "Map";
		private const string TagFrom = "From";
		private const string TagTo = "To";
		private const string TagSentenceImportFile = "ImportFile";
		private const string TagSentenceExportFile = "ExportFile";
		private const string TagSentenceExportPartitioned = "ExportPartitioned";
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
		private const string TagRequired = "Required";
		private const string TagExclude = "Exclude";
		private const string TagExcludeMode = "Mode";
		private const string TagExcludeExcept = "Except";
		private const string TagTimeout = "Timeout";
		//private const string TagSentenceExportParquet = "ExportParquet";
		//private const string TagSentenceImportParquet = "ImportParquet";
		private const string TagRowGroupSize = "RowGroupSize";
		private const string TagContainer = "Container";
		private const string TagImportFolder = "ImportFolder";

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
				foreach (MLNode nodeML in nodesML)
					switch (nodeML.Name)
					{
						case TagImport:
								sentences.AddRange(LoadByFile(System.IO.Path.Combine(pathBase, nodeML.Attributes[TagFileName].Value)).Sentences);
							break;
						case TagSentenceBlock:
								sentences.Add(LoadSentenceBlock(nodeML, pathBase));
							break;
						case TagSentenceExecute:
								sentences.Add(LoadSentenceExecute(nodeML));
							break;
						case TagSentenceException:
								sentences.Add(LoadSentenceException(nodeML));
							break;
						case TagSentenceBulkCopy:
								sentences.Add(LoadSentenceBulkCopy(nodeML));
							break;
						case TagSentenceCopy:
								sentences.Add(LoadSentenceCopy(nodeML));
							break;
						case TagSentenceForEach:
								sentences.Add(LoadSentenceForEach(nodeML, pathBase));
							break;
						case TagSentenceIfExists:
								sentences.Add(LoadSentenceIfExists(nodeML, pathBase));
							break;
						case TagSentenceIf:
								sentences.Add(LoadSentenceIf(nodeML, pathBase));
							break;
						case TagSentenceString:
								sentences.Add(LoadSentenceDeclare(nodeML, VariableModel.VariableType.String));
							break;
						case TagSentenceNumeric:
								sentences.Add(LoadSentenceDeclare(nodeML, VariableModel.VariableType.Numeric));
							break;
						case TagSentenceBoolean:
								sentences.Add(LoadSentenceDeclare(nodeML, VariableModel.VariableType.Boolean));
							break;
						case TagSentenceDate:
								sentences.Add(LoadSentenceDeclare(nodeML, VariableModel.VariableType.Date));
							break;
						case TagSentenceLet:
								sentences.Add(LoadSentenceLet(nodeML));
							break;
						case TagSentenceFor:
								sentences.Add(LoadSentenceFor(nodeML, pathBase));
							break;
						case TagSentenceWhile:
								sentences.Add(LoadSentenceWhile(nodeML, pathBase));
							break;
						case TagSentencePrint:
								sentences.Add(LoadSentencePrint(nodeML));
							break;
						case TagSentenceBeginTransaction:
								sentences.Add(LoadSentenceBatch(nodeML, SentenceDataBatch.BatchCommand.BeginTransaction));
							break;
						case TagSentenceCommitTransaction:
								sentences.Add(LoadSentenceBatch(nodeML, SentenceDataBatch.BatchCommand.CommitTransaction));
							break;
						case TagSentenceRollbackTransaction:
								sentences.Add(LoadSentenceBatch(nodeML, SentenceDataBatch.BatchCommand.RollbackTransaction));
							break;
						case TagSentenceAssertExecute:
								sentences.Add(LoadSentenceAssertExecute(nodeML));
							break;
						case TagSentenceAssertScalar:
								sentences.Add(LoadSentenceAssertScalar(nodeML));
							break;
						case TagSentenceExecuteScript:
								sentences.Add(LoadSentenceExecuteScript(nodeML));
							break;
						case TagSentenceImportSchemaCsv:
								sentences.Add(LoadSentenceImportSchema(nodeML));
							break;
						case TagSentenceExportSchemaCsv:
								sentences.Add(LoadSentenceExportSchema(nodeML));
							break;
						case TagSentenceExportPartitioned:
								sentences.Add(LoadSentenceExportPartitionedCsv(nodeML));
							break;
						case TagSentenceImportFile:
								sentences.Add(LoadSentenceImportFile(nodeML));
							break;
						case TagSentenceExportFile:
								sentences.Add(LoadSentenceExportFile(nodeML));
							break;
						default:
							throw new ArgumentException($"Node unkwnown: {nodeML.Name}");
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
								Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull(),
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
				sentence.MustParse = rootML.Attributes[TagMustParse].Value.GetBool();
				sentence.SkipParameters = rootML.Attributes[TagSkipParameters].Value.GetBool();
				// Asigna los mapeos de variables
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagMap)
						sentence.Mapping.Add((nodeML.Attributes[TagVariable].Value.TrimIgnoreNull(), nodeML.Attributes[TagTo].Value.TrimIgnoreNull()));
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
													Source = rootML.Attributes[TagSource].Value.TrimIgnoreNull(),
													Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull(),
													Table = rootML.Attributes[TagTable].Value.TrimIgnoreNull(),
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
								Source = rootML.Attributes[TagSource].Value.TrimIgnoreNull(),
								Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull(),
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
				sentence.Source = rootML.Attributes[TagSource].Value.TrimIgnoreNull();
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
								Message = rootML.Value.TrimIgnoreNull()
							};
		}

		/// <summary>
		///		Carga una sentencia If
		/// </summary>
		private SentenceBase LoadSentenceIf(MLNode rootML, string pathBase)
		{
			SentenceIf sentence = new SentenceIf();

				// Carga la condición
				sentence.Condition = rootML.Attributes[TagCondition].Value.TrimIgnoreNull();
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
				sentence.Condition = rootML.Attributes[TagCondition].Value.TrimIgnoreNull();
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
				sentence.Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull();
				sentence.Message = rootML.Attributes[TagMessage].Value.TrimIgnoreNull();
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
								Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull(),
								Message = rootML.Attributes[TagMessage].Value.TrimIgnoreNull(),
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
				return new ProviderSentenceModel(rootML.Value.TrimIgnoreNull(), GetTimeout(rootML, TimeSpan.FromMinutes(5)));
			else
			{
				ProviderSentenceModel sentence = null;

					// Primero obtiene el comando
					foreach (MLNode nodeML in rootML.Nodes)
						if (nodeML.Name == tagCommand)
							sentence = new ProviderSentenceModel(nodeML.Value.TrimIgnoreNull(), GetTimeout(rootML, TimeSpan.FromMinutes(5)));
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
				filter.VariableName = rootML.Attributes[TagVariable].Value.TrimIgnoreNull();
				filter.Parameter = rootML.Attributes[TagParameter].Value.TrimIgnoreNull();
				if (string.IsNullOrEmpty(filter.VariableName) && !string.IsNullOrEmpty(filter.Parameter))
					filter.VariableName = filter.Parameter.Replace("@", "");
				// Obtiene el valor por defecto
				filter.Default = ConvertStringValue(filter.ColumnType, rootML.Attributes[TagDefault].Value.TrimIgnoreNull());
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
		///		Carga una sentencia <see cref="SentenceFileImport"/>
		/// </summary>
		private SentenceBase LoadSentenceImportFile(MLNode rootML)
		{
			SentenceFileImport sentence = new SentenceFileImport();

				// Asigna las propiedades
				sentence.Type = rootML.Attributes[TagType].Value.GetEnum(SentenceFileBase.FileType.Csv);
				sentence.Source = rootML.Attributes[TagSource].Value.TrimIgnoreNull();
				sentence.Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull();
				sentence.Container = rootML.Attributes[TagContainer].Value.TrimIgnoreNull();
				sentence.FileName = rootML.Attributes[TagFileName].Value.TrimIgnoreNull();
				sentence.ImportFolder = rootML.Attributes[TagImportFolder].Value.GetBool();
				sentence.Table = rootML.Attributes[TagTable].Value.TrimIgnoreNull();
				sentence.Required = rootML.Attributes[TagRequired].Value.GetBool(true);
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
		///		Carga una sentencia <see cref="SentenceFileExport"/>
		/// </summary>
		private SentenceBase LoadSentenceExportFile(MLNode rootML)
		{
			SentenceFileExport sentence = new SentenceFileExport();

				// Asigna las propiedades
				sentence.Type = rootML.Attributes[TagType].Value.GetEnum(SentenceFileBase.FileType.Csv);
				sentence.Source = rootML.Attributes[TagSource].Value.TrimIgnoreNull();
				sentence.Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull();
				sentence.Container = rootML.Attributes[TagContainer].Value.TrimIgnoreNull();
				sentence.FileName = rootML.Attributes[TagFileName].Value.TrimIgnoreNull();
				sentence.BatchSize = rootML.Attributes[TagBatchSize].Value.GetInt(BatchSizeDefault);
				sentence.RowGroupSize = rootML.Attributes[TagRowGroupSize].Value.GetInt(45_000);
				sentence.Command = GetProviderCommand(rootML, TagLoad);
				// Carga la definición
				LoadDefinitionCsv(sentence.Definition, rootML);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia para exportar una serie de archivos particionados
		/// </summary>
		private SentenceBase LoadSentenceExportPartitionedCsv(MLNode rootML)
		{
			SentenceFileExportPartitioned sentence = new SentenceFileExportPartitioned();

				// Asigna las propiedades
				sentence.Type = rootML.Attributes[TagType].Value.GetEnum(SentenceFileBase.FileType.Csv);
				sentence.Source = rootML.Attributes[TagSource].Value.TrimIgnoreNull();
				sentence.Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull();
				sentence.Container = rootML.Attributes[TagContainer].Value.TrimIgnoreNull();
				sentence.FileName = rootML.Attributes[TagFileName].Value.TrimIgnoreNull();
				sentence.BatchSize = rootML.Attributes[TagBatchSize].Value.GetInt(BatchSizeDefault);
				sentence.Command = GetProviderCommand(rootML, TagLoad);
				// Carga la definición
				LoadDefinitionCsv(sentence.Definition, rootML);
				// Carga las columnas de partición
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagPartitionBy && !string.IsNullOrWhiteSpace(nodeML.Attributes[TagColumn].Value))
						sentence.PartitionColumns.Add(nodeML.Attributes[TagColumn].Value.TrimIgnoreNull());
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia <see cref="SentenceFileImportSchema"/>
		/// </summary>
		private SentenceFileImportSchema LoadSentenceImportSchema(MLNode rootML)
		{
			SentenceFileImportSchema sentence = new SentenceFileImportSchema();

				// Asigna las propiedades básicas
				sentence.Type = rootML.Attributes[TagType].Value.GetEnum(SentenceFileBase.FileType.Csv);
				sentence.Source = rootML.Attributes[TagSource].Value.TrimIgnoreNull();
				sentence.Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull();
				sentence.Container = rootML.Attributes[TagContainer].Value.TrimIgnoreNull();
				sentence.FileName = rootML.Attributes[TagPath].Value.TrimIgnoreNull();
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
		///		Carga una sentencia <see cref="SentenceFileExportSchema"/>
		/// </summary>
		private SentenceBase LoadSentenceExportSchema(MLNode rootML)
		{
			SentenceFileExportSchema sentence = new SentenceFileExportSchema();

				// Asigna las propiedades básicas
				sentence.Type = rootML.Attributes[TagType].Value.GetEnum(SentenceFileBase.FileType.Csv);
				sentence.Source = rootML.Attributes[TagSource].Value.TrimIgnoreNull();
				sentence.Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull();
				sentence.Container = rootML.Attributes[TagContainer].Value.TrimIgnoreNull();
				sentence.FileName = rootML.Attributes[TagPath].Value.TrimIgnoreNull();
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
			definition.Separator = rootML.Attributes[TagFieldsSeparator].Value.TrimIgnoreNull();
			definition.DecimalSeparator = rootML.Attributes[TagDecimalSeparator].Value.TrimIgnoreNull();
			definition.ThousandsSeparator = rootML.Attributes[TagThousandsSeparator].Value.TrimIgnoreNull();
			definition.DateFormat = rootML.Attributes[TagDateFormat].Value.TrimIgnoreNull();
			definition.TrueValue = rootML.Attributes[TagValueTrue].Value.TrimIgnoreNull();
			definition.FalseValue = rootML.Attributes[TagValueFalse].Value.TrimIgnoreNull();
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
												Name = nodeML.Attributes[TagName].Value.TrimIgnoreNull(),
												Type = nodeML.Attributes[TagType].Value.GetEnum(ColumnModel.ColumnType.String)
											}
									);
				// Devuelve las columnas cargadas
				return columns;
		}

		/// <summary>
		///		Obtiene el timeout definido en el nodo
		/// </summary>
		private TimeSpan GetTimeout(MLNode rootML, TimeSpan defaultTimeOut)
		{
			TimeSpan? timeout = null;
			string attribute = rootML.Attributes[TagTimeout].Value.TrimIgnoreNull();

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
