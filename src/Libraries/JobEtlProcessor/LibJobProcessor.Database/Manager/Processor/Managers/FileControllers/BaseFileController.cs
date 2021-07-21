using System;
using System.Collections.Generic;
using System.Text;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers
{
	/// <summary>
	///		Controlador base para los archivos
	/// </summary>
	internal abstract class BaseFileController : BaseManager
	{
		protected BaseFileController(DbScriptProcessor processor) : base(processor)
		{
		}

		/// <summary>
		///		Obtiene la implementación de lectura / escritura de archivos
		/// </summary>
		protected Implementation.BaseFileImplementation GetFileImplementation(Sentences.Files.SentenceFileBase sentence)
		{
			switch (sentence.Type)
			{
				case Sentences.Files.SentenceFileBase.FileType.Csv:
					return new Implementation.CsvFileImplementation();
				case Sentences.Files.SentenceFileBase.FileType.Parquet:
					return new Implementation.ParquetFileImplementation();
				case Sentences.Files.SentenceFileBase.FileType.Json:
					return new Implementation.JsonFileImplementation();
				default:
					throw new NotImplementedException($"Unknown file type {sentence.Type.ToString()}");
			}
		}

		/// <summary>
		///		Obtiene el manager adecuado para el storage
		/// </summary>
		protected Storage.BaseFileStorage GetStorageManager(DbScriptProcessor processor, Sentences.Files.SentenceFileBase sentence, string container)
		{
			bool IsLocalSentence(string source, string container)
			{
				return string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(container);
			}

			switch (sentence)
			{
				case Sentences.Files.SentenceFileExport sentenceFile:
					if (IsLocalSentence(sentenceFile.Target, container))
						return new Storage.LocalFileStorage(processor);
					else
						return new Storage.CloudFileStorage(processor, sentenceFile.Target, container);
				case Sentences.Files.SentenceFileImport sentenceFile:
					if (IsLocalSentence(sentenceFile.Source, container))
						return new Storage.LocalFileStorage(processor);
					else
						return new Storage.CloudFileStorage(processor, sentenceFile.Source, container);
				default:
					throw new NotImplementedException($"Unknown file sentence: {sentence.GetType().ToString()}");
			}
		}
	}
}
