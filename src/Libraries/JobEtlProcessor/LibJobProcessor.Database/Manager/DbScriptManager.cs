using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.DbAggregator;
using Bau.Libraries.LibLogger.Core;

namespace Bau.Libraries.LibJobProcessor.Database.Manager
{
	/// <summary>
	///		Manager para  la ejecución de scripts
	/// </summary>
	internal class DbScriptManager
	{
		internal DbScriptManager(DbAggregatorManager dataProviderManager, NormalizedDictionary<object> parameters, NormalizedDictionary<string> paths, LogManager logger)
		{
			DataProviderManager = dataProviderManager;
			Parameters = parameters;
			Paths = paths;
			Logger = logger;
		}

		/// <summary>
		///		Ejecuta el script desde un archivo
		/// </summary>
		internal async Task<bool> ProcessByFileAsync(string fileName, CancellationToken cancellationToken)
		{
			return await ProcessAsync(new Repository.DbScriptRepository().LoadByFile(fileName), cancellationToken);
		}

		/// <summary>
		///		Crea el programa para procesar un script SQL
		/// </summary>
		internal async Task<bool> ProcessBySqlScriptAsync(string target, string fileName, CancellationToken cancellationToken)
		{
			Builder.ProgramBuilder builder = new Builder.ProgramBuilder();

				// Añade los datos
				builder.WithBlock($"Process {System.IO.Path.GetFileName(fileName)}")
							.WithScript(target, fileName);
				// Devuelve el programa
				return await ProcessAsync(builder.Build(), cancellationToken);
		}

		/// <summary>
		///		Ejecuta el script desde una cadena XML
		/// </summary>
		internal async Task<bool> ProcessByXmlAsync(string xml, string pathBase, CancellationToken cancellationToken)
		{
			return await ProcessAsync(new Repository.DbScriptRepository().LoadByText(xml, pathBase), cancellationToken);
		}

		/// <summary>
		///		Ejecuta el programa
		/// </summary>
		private async Task<bool> ProcessAsync(Processor.Sentences.ProgramModel program, CancellationToken cancellationToken)
		{
			await Task.Run(() => {
									Processor.DbScriptProcessor processor = new Processor.DbScriptProcessor(this);

										// Ejecuta el script
										processor.Process(program);
								});
				// Devuelve el valor que indica si ha habido errores
				return Errors.Count == 0;
		}

		/// <summary>
		///		Obtiene el nombre de archivo completo: si existe el archivo, devuelve el mismo nombre de archivo, 
		///	si es un nombre de archivo relativo, se convierte el nombre de archivo según los nombres de contexto
		/// </summary>
		internal string GetFullFileName(string fileName)
		{
			if (System.IO.File.Exists(fileName))
				return fileName;
			else
				return ConvertFileNameWihtPaths(fileName);
		}

		/// <summary>
		///		Obtiene el nombre completo de un archivo a partir de los datos de los contextos
		/// </summary>
		private string ConvertFileNameWihtPaths(string fileName)
		{
			string fullFileName = fileName;

				// Sustituye los directorios del contexto global
				foreach ((string key, string value) in Paths.Enumerate())
					fullFileName = fullFileName.ReplaceWithStringComparison("{{" + key + "}}", value);
				// Devuelve el nombre de archivo
				return fullFileName;
		}

		/// <summary>
		///		Controlador de los proveedores de datos
		/// </summary>
		internal DbAggregatorManager DataProviderManager { get; }

		/// <summary>
		///		Parámetros iniciales
		/// </summary>
		internal NormalizedDictionary<object> Parameters { get; }

		/// <summary>
		///		Directorios de trabajo
		/// </summary>
		private NormalizedDictionary<string> Paths { get; } = new NormalizedDictionary<string>();

		/// <summary>
		///		Manager de log
		/// </summary>
		internal LogManager Logger { get; }

		/// <summary>
		///		Errores de proceso
		/// </summary>
		internal List<string> Errors { get; } = new List<string>();
	}
}
