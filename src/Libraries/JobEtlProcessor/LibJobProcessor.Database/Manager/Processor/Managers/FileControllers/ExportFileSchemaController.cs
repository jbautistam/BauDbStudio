using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDbProviders.Base.Schema;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers
{
	/// <summary>
	///		Procesador de las sentencias de exportación de esquema a archivos
	/// </summary>
	internal class ExportFileSchemaController : BaseManager
	{
		internal ExportFileSchemaController(DbScriptProcessor processor) : base(processor) {}

		/// <summary>
		///		Procesa una exportación de las tablas de esquema a archivos CSV
		/// </summary>
		internal async Task<bool> ExecuteAsync(SentenceFileExportSchema sentence, CancellationToken cancellationToken)
		{
			bool exported = false;

				// Exporta los archivos
				using (BlockLogModel block = Processor.Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Start exporting tables from {sentence.Source}"))
				{
					ProviderModel provider = Processor.GetProvider(sentence.Source);

						if (provider == null)
							block.Error($"Can't find the provider '{sentence.Source}'");
						else
						{
							// Supone que se ha exportado correctamente
							exported = true;
							// Elimina los archivos si es necesario
							if (sentence.DeleteOldFiles)
								DeletePathFiles(block, sentence.Container, Processor.Manager.Step.Project.GetFullFileName(sentence.FileName), sentence.GetExtension());
							// Exporta las tablas del esquema (mientras no haya errores)
							foreach (SentenceFileExport exportSentence in GetExportFileSentences(block, provider, sentence))
								if (exported && !cancellationToken.IsCancellationRequested)
									exported = await new FileControllers.ExportFileController(Processor).ExecuteAsync(exportSentence, cancellationToken);
						}
				}
				// Devuelve el valor que indica si se ha exportado
				return exported;
		}

		/// <summary>
		///		Borra los archivos CSV de un directorio
		/// </summary>
		private void DeletePathFiles(BlockLogModel block, string container, string path, string extension)
		{
			if (string.IsNullOrWhiteSpace(container) && System.IO.Directory.Exists(path))
				foreach (string fileName in System.IO.Directory.GetFiles(path, "*" + extension))
					try
					{
						System.IO.File.Delete(fileName);
					}
					catch (Exception exception)
					{
						block.Debug($"Can't delete the file {fileName}. {exception.Message}");
					}
		}

		/// <summary>
		///		Obtiene los comandos para exportación de los archivos asociados a las tablas
		/// </summary>
		private List<SentenceFileExport> GetExportFileSentences(BlockLogModel block, ProviderModel provider, SentenceFileExportSchema sentence)
		{
			List<SentenceFileExport> sentences = new List<SentenceFileExport>();

				// Obtiene las sentencias
				foreach (TableDbModel table in provider.LoadSchema().Tables)
					if (sentence.ExcludeRules.CheckMustExclude(table.Name))
						block.Info($"Skip table {table.Name} because is excluded");
					else
						sentences.Add(CreateSentence(sentence, table));
				// Devuelve la colección de instrucciones
				return sentences;
		}

		/// <summary>
		///		Crea una sentencia de exportación a CSV con los datos de la tabla
		/// </summary>
		private SentenceFileExport CreateSentence(SentenceFileExportSchema sentence, TableDbModel table)
		{
			SentenceFileExport exportSentence = new SentenceFileExport();

				// Asigna las propiedades
				exportSentence.Type = sentence.Type;
				exportSentence.Source = sentence.Source;
				exportSentence.Target = sentence.Target;
				exportSentence.Container = sentence.Container;
				exportSentence.FileName = System.IO.Path.Combine(sentence.FileName, table.Name + sentence.GetExtension());
				exportSentence.Command = GetSelect(table);
				exportSentence.BatchSize = sentence.BatchSize;
				exportSentence.Timeout = sentence.Timeout;
				// Asigna los parámetros de archivo
				exportSentence.Definition = sentence.Definition;
				// Devuelve la sentencia de exportación
				return exportSentence;
		}

		/// <summary>
		///		Obtiene la sentencia SELECT de consulta de datos de una tabla
		/// </summary>
		private Sentences.Parameters.ProviderSentenceModel GetSelect(TableDbModel table)
		{
			string fields = string.Empty;

				// Añade los campos de la tabla
				foreach (FieldDbModel field in table.Fields)
				{
					// Añade el separador
					if (!string.IsNullOrEmpty(fields))
						fields += ", ";
					// Añade el nombre de campo
					fields += $"[{field.Name}]";
				}
				// Añade la cadena SQL al comando
				return new Sentences.Parameters.ProviderSentenceModel($"SELECT {fields} FROM {table.FullName}", TimeSpan.FromMinutes(5));
		}
	}
}
