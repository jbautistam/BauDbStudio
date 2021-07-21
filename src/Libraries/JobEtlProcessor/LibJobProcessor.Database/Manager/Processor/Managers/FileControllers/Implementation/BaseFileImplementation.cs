using System;
using System.Threading;
using System.Threading.Tasks;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers.Storage;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers.Implementation
{
	/// <summary>
	///		Implementación base para la importación / exportación de archivos a base de datos
	/// </summary>
	internal abstract class BaseFileImplementation
	{
		/// <summary>
		///		Importa un archivo en base de datos
		/// </summary>
		internal abstract void Import(BlockLogModel block, ProviderModel provider, Sentences.Files.SentenceFileImport sentence, System.IO.Stream stream);

		/// <summary>
		///		Exporta un archivo a una tabla base de datos
		/// </summary>
		internal abstract void Export(BlockLogModel block, System.IO.Stream stream, ProviderModel provider, CommandModel command, Sentences.Files.SentenceFileExport sentence);

		/// <summary>
		///		Exporta datos de un comando a un archivo particionado
		/// </summary>
		internal abstract Task ExportPartitionedAsync(BlockLogModel block, BaseFileStorage fileManager, ProviderModel provider, CommandModel command, 
													  SentenceFileExportPartitioned sentence, string baseFileName, CancellationToken cancellationToken);
	}
}
