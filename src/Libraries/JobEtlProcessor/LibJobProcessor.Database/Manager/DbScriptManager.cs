using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.DbAggregator;
using Bau.Libraries.LibLogger.Core;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

namespace Bau.Libraries.LibJobProcessor.Database.Manager
{
	/// <summary>
	///		Manager para  la ejecución de scripts
	/// </summary>
	internal class DbScriptManager
	{
		internal DbScriptManager(JobStepModel step, DbAggregatorManager dataProviderManager, NormalizedDictionary<string> storageConnectionStrings, 
								 NormalizedDictionary<object> parameters, LogManager logger)
		{
			Step = step;
			DataProviderManager = dataProviderManager;
			StorageConnectionStrings = storageConnectionStrings ?? new NormalizedDictionary<string>();
			Parameters = parameters;
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
			Processor.DbScriptProcessor processor = new Processor.DbScriptProcessor(this);

				// Ejecuta el script
				await processor.ProcessAsync(program, cancellationToken);
				// Devuelve el valor que indica si ha habido errores
				return Errors.Count == 0;
		}

		/// <summary>
		///		Datos del paso
		/// </summary>
		internal JobStepModel Step { get; }

		/// <summary>
		///		Controlador de los proveedores de datos
		/// </summary>
		internal DbAggregatorManager DataProviderManager { get; }

		/// <summary>
		///		Cadenas de conexión al storage
		/// </summary>
		internal NormalizedDictionary<string> StorageConnectionStrings { get; }

		/// <summary>
		///		Parámetros iniciales
		/// </summary>
		internal NormalizedDictionary<object> Parameters { get; }

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
