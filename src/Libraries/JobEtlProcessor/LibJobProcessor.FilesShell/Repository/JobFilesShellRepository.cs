using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Repository
{
	/// <summary>
	///		Clase para cargar datos de proceso
	/// </summary>
	internal class JobFilesShellRepository
	{
		// Constantes privadas
		private const string TagRoot = "ShellScript";
		private const string TagBlock = "Block";
		private const string TagEnabled = "Enabled";
		private const string TagTimeout = "Timeout";
		private const string TagMessage = "Message";
		private const string TagCopy = "Copy";
		private const string TagDelete = "Delete";
		private const string TagFrom = "From";
		private const string TagTo = "To";
		private const string TagMask = "Mask";
		private const string TagFlattenPaths = "FlattenPaths";
		private const string TagPath = "Path";
		private const string TagExecute = "Execute";
		private const string TagProcess = "Process";
		private const string TagArgument = "Argument";
		private const string TagKey = "Key";
		private const string TagValue = "Value";
		private const string TagTransformFileName = "TransformFileName";
		private const string TagConvertFile = "ConvertFile";
		private const string TagColumn = "Column";
		private const string TagName = "Name";
		private const string TagType = "Type";
		private const string TagConvertPath = "ConvertPath";

		/// <summary>
		///		Carga los datos del proceso
		/// </summary>
		internal List<BaseSentence> Load(string fileName)
		{
			List<BaseSentence> program = new List<BaseSentence>();

				// Carga los datos
				if (!string.IsNullOrWhiteSpace(fileName) && System.IO.File.Exists(fileName))
				{
					MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

						if (fileML != null)
							foreach (MLNode rootML in fileML.Nodes)
								if (rootML.Name == TagRoot)
									program.AddRange(LoadSentences(rootML));
				}
				// Devuelve las instrucciones
				return program;
		}

		/// <summary>
		///		Carga las sentencias de un nodo
		/// </summary>
		private List<BaseSentence> LoadSentences(MLNode rootML)
		{
			List<BaseSentence> sentences = new List<BaseSentence>();

				// Carga las sentencias
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagBlock:
								sentences.Add(LoadBlockSentence(nodeML));
							break;
						case TagCopy:
								sentences.Add(LoadCopySentence(nodeML));
							break;
						case TagDelete:
								sentences.Add(LoadDeleteSentence(nodeML));
							break;
						case TagExecute:
								sentences.Add(LoadExecuteSentence(nodeML));
							break;
						case TagConvertFile:
								sentences.Add(LoadConvertFileSentence(nodeML));
							break;
						case TagConvertPath:
								sentences.Add(LoadConvertPathSentence(nodeML));
							break;
					}
				// Devuelve la lista de sentencias
				return sentences;
		}

		/// <summary>
		///		Carga un bloque de sentencias
		/// </summary>
		private BaseSentence LoadBlockSentence(MLNode rootML)
		{
			BlockSentence sentence = new BlockSentence();

				// Asigna las propiedades
				AssignSentence(sentence, rootML);
				sentence.Message = rootML.Attributes[TagMessage].Value;
				// Carga las sentencias del bloque
				sentence.Sentences.AddRange(LoadSentences(rootML));
				// Devuelve la sentencia leida
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

		/// <summary>
		///		Carga la sentencia para descargar un archivo
		/// </summary>
		private BaseSentence LoadCopySentence(MLNode rootML)
		{
			CopySentence sentence = new CopySentence();

				// Carga los datos de la sentencia
				AssignSentence(sentence, rootML);
				sentence.Source = rootML.Attributes[TagFrom].Value.TrimIgnoreNull();
				sentence.Target = rootML.Attributes[TagTo].Value.TrimIgnoreNull();
				sentence.Mask = rootML.Attributes[TagMask].Value.TrimIgnoreNull();
				sentence.FlattenPaths = rootML.Attributes[TagFlattenPaths].Value.GetBool(false);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de borrado
		/// </summary>
		private BaseSentence LoadDeleteSentence(MLNode rootML)
		{
			DeleteSentence sentence = new DeleteSentence();

				// Carga los datos de la sentencia
				AssignSentence(sentence, rootML);
				sentence.Path = rootML.Attributes[TagPath].Value.TrimIgnoreNull();
				sentence.Mask = rootML.Attributes[TagMask].Value.TrimIgnoreNull();
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de ejecución de un proceso
		/// </summary>
		private BaseSentence LoadExecuteSentence(MLNode rootML)
		{
			ExecuteSentence sentence = new ExecuteSentence();

				// Carga los datos
				AssignSentence(sentence, rootML);
				sentence.Process = rootML.Attributes[TagProcess].Value.TrimIgnoreNull();
				// Carga los argumentos
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagArgument)
						sentence.Arguments.Add(new ExecuteSentenceArgument
														{
															Key = nodeML.Attributes[TagKey].Value.TrimIgnoreNull(),
															Value = nodeML.Attributes[TagValue].Value.TrimIgnoreNull(),
															TransformFileName = nodeML.Attributes[TagTransformFileName].Value.GetBool()
														}
											  );
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de conversión de archivo
		/// </summary>
		private BaseSentence LoadConvertFileSentence(MLNode rootML)
		{
			ConvertFileSentence sentence = new ConvertFileSentence();

				// Carga los datos
				AssignSentence(sentence, rootML);
				sentence.FileNameSource = rootML.Attributes[TagFrom].Value.TrimIgnoreNull();
				sentence.FileNameTarget = rootML.Attributes[TagTo].Value.TrimIgnoreNull();
				// Carga las columnas
				sentence.Columns.AddRange(LoadColumns(rootML));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga la definición de columnas de un archivo
		/// </summary>
		private List<FileColumnModel> LoadColumns(MLNode rootML)
		{
			List<FileColumnModel> columns = new List<FileColumnModel>();

				// Carga la lista de columnas
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagColumn)
						columns.Add(new FileColumnModel
											{
												Name = rootML.Attributes[TagName].Value,
												Type = rootML.Attributes[TagType].Value.GetEnum(FileColumnModel.ColumnType.String)
											}
									);
				// Devuelve la lista de columnas
				return columns;
		}

		/// <summary>
		///		Carga la sentencia de conversión de archivos de un directorio
		/// </summary>
		private BaseSentence LoadConvertPathSentence(MLNode rootML)
		{
			ConvertPathSentence sentence = new ConvertPathSentence();

				// Carga los datos
				AssignSentence(sentence, rootML);
				sentence.Path = rootML.Attributes[TagPath].Value.TrimIgnoreNull();
				sentence.Source = rootML.Attributes[TagFrom].Value.GetEnum(ConvertPathSentence.FileType.Unknown);
				sentence.Target = rootML.Attributes[TagTo].Value.GetEnum(ConvertPathSentence.FileType.Unknown);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Asigna los datos de una sentencia asociada con un blob
		/// </summary>
		private void AssignSentence(BaseSentence sentence, MLNode rootML)
		{
			sentence.Enabled = rootML.Attributes[TagEnabled].Value.GetBool(true);
			sentence.Timeout = GetTimeout(rootML, TimeSpan.FromMinutes(5));
		}
	}
}
