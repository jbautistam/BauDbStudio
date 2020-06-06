using System;
using System.Collections.Generic;

using Bau.Libraries.LibDbProviders.Base.Schema;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Csv;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.Csv
{
	/// <summary>
	///		Procesador de las sentencias de importación de archivos de todo el esquema de base de datos
	/// </summary>
	internal class ImportSchemaCsvManager : BaseManager
	{
		internal ImportSchemaCsvManager(DbScriptProcessor processor) : base(processor) {}

		/// <summary>
		///		Procesa una importación de archivos CSV para las tablas de esquema de base de datos
		/// </summary>
		internal bool Execute(SentenceImportCsvSchema sentence)
		{
			bool imported = false;

				// Importa los archivos
				using (BlockLogModel block = Processor.Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Start importing tables from {sentence.Target}"))
				{
					ProviderModel provider = Processor.GetProvider(sentence.Target);

						if (provider == null)
							block.Error($"Can't find the provider '{sentence.Target}'");
						else
						{
							// Supone que todo se importa correctamente
							imported = true;
							// Ejecuta las sentencias de importación obtenidas a partir del esquema y los archivos
							foreach (SentenceImportCsv sentenceCsv in GetImportFileSentences(block, provider, sentence))
								if (imported)
								{
									// Log
									block.Info($"Importing '{sentenceCsv.FileName}' in '{sentenceCsv.Table}'");
									// Importa el archivo
									imported = new ImportCsvManager(Processor).Execute(sentenceCsv);
								}
						}
				}
				// Devuelve el valor que indica si se ha importado
				return imported;
		}

		/// <summary>
		///		Obtiene los comandos para importación de los archivos asociados a las tablas
		/// </summary>
		private List<SentenceImportCsv> GetImportFileSentences(BlockLogModel block, ProviderModel provider, SentenceImportCsvSchema sentence)
		{
			List<SentenceImportCsv> sentences = new List<SentenceImportCsv>();

				// Obtiene las sentencias
				foreach (TableDbModel table in provider.LoadSchema().Tables)
					if (sentence.ExcludeRules.CheckMustExclude(table.Name))
						block.Debug($"Skip table {table.Name} because is excluded");
					else 
					{
						string fileName = System.IO.Path.Combine(Processor.Manager.Step.Project.GetFullFileName(sentence.Path), $"{table.Name}.csv");

							if (System.IO.File.Exists(fileName))
								sentences.Add(CreateSentence(sentence, table));
					}
				// Devuelve la colección de instrucciones
				return sentences;
		}

		/// <summary>
		///		Crea una sentencia de importación de una tabla con los datos de un archivo
		/// </summary>
		private SentenceImportCsv CreateSentence(SentenceImportCsvSchema sentence, TableDbModel table)
		{
			SentenceImportCsv importSentence = new SentenceImportCsv();

				// Asigna las propiedades
				importSentence.Target = sentence.Target;
				importSentence.FileName = table.Name + ".csv";
				importSentence.Table = table.Name;
				importSentence.BatchSize = sentence.BatchSize;
				importSentence.Timeout = sentence.Timeout;
				// Asigna los parámetros de archivo
				importSentence.Definition = sentence.Definition;
				// Devuelve la sentencia de importación
				return importSentence;
		}
	}
}
