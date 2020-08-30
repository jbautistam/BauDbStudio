using System;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibJobProcessor.Manager;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files.ScriptsManager
{
	/// <summary>
	///		Manager para el tratamiento de proyectos XML
	/// </summary>
	internal class JobXmlProjectManager
	{
		internal JobXmlProjectManager(LibLogger.Core.LogManager logger)
		{
			Logger = logger;
		}

		/// <summary>
		///		Ejecuta el script
		/// </summary>
		internal async Task ExecuteAsync(string projectFileName, string contextFileName, CancellationToken cancellationToken)
		{
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

				// Ejecuta el script
				using (BlockLogModel block = Logger.Default.CreateBlock(LogModel.LogType.Info,
																		$"Comienza la ejecución del script XML '{System.IO.Path.GetFileName(projectFileName)}'"))
				{
					// Arranza el temporizador
					stopwatch.Start();
					// Log
					block.Info($"Archivo de contexto: '{contextFileName}'");
					// Ejecuta el proyecto
					await ExecuteProjectAsync(block, projectFileName, contextFileName, cancellationToken);
					// Muestra el tiempo de ejecución
					stopwatch.Stop();
					block.Info($"Tiempo de ejecución: {stopwatch.Elapsed.ToString()}");
				}
		}

		/// <summary>
		///		Ejecuta el proyecto
		/// </summary>
		private async Task ExecuteProjectAsync(BlockLogModel block, string project, string context, CancellationToken cancellationToken)
		{
			JobProjectManager manager = new JobProjectManager(Logger);

				// Añade los procesadores
				manager.AddProcessor(new LibJobProcessor.Cloud.JobCloudManager(Logger));
				manager.AddProcessor(new LibJobProcessor.Database.JobDatabaseManager(Logger));
				manager.AddProcessor(new LibJobProcessor.Powershell.JobPowershellManager(Logger));
				manager.AddProcessor(new LibJobProcessor.FilesShell.JobFileShellManager(Logger));
				manager.AddProcessor(new LibJobProcessor.Rest.JobRestManager(Logger));
				// Ejecuta el proyecto
				await manager.ProcessAsync(project, context, cancellationToken);
				// Asigna los errores
				if (manager.Errors.Count == 0)
					block.Info("La ejecución del proyecto ha terminado correctamente");
				else
				{
					string errorTotal = "Error en la ejecución del proyecto";

						// Añade los errores
						foreach (string error in manager.Errors)
							if (!string.IsNullOrWhiteSpace(error))
								errorTotal += Environment.NewLine + error.Trim();
						// Lanza el mensaje de error
						block.Error(errorTotal);
				}
		}

		/// <summary>
		///		Manager de log
		/// </summary>
		internal LibLogger.Core.LogManager Logger { get; }
	}
}
