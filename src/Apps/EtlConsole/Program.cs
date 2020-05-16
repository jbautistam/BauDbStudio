using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.Manager;
using Bau.Libraries.LibLogger.Core;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.EtlConsole
{
	/// <summary>
	///		Clase inicial de la aplicación
	/// </summary>
	static class Program
	{
		/// <summary>
		///		Tipo de argumento
		/// </summary>
		private enum ArgumentType
		{
			/// <summary>Archivo de proyecto</summary>
			Project,
			/// <summary>Archivo de contexto</summary>
			Context
		}

		/// <summary>
		///		Método principal de la aplicación
		/// </summary>
		static async Task Main(string[] args)
		{
			Dictionary<ArgumentType, string> parameters = GetParameters(args);

				// Crea el manejador de log
				Logger = CreateLogger();
				// Recoge los argumentos y procesa el archivo
				if (parameters.Count == 2)
					await ProcessAsync(parameters, System.Threading.CancellationToken.None);
				else
				{
					Console.WriteLine("Process parameters:");
					Console.WriteLine($"      --{ArgumentType.Project.ToString()} <Xml project file name>");
					Console.WriteLine($"      --{ArgumentType.Context.ToString()} <Xml context file name>");
				}
				// Espera que se pulse una tecla
				#if DEBUG
					Console.WriteLine("Press any key...");
					Console.ReadKey();
				#endif
		}

		/// <summary>
		///		Obtiene el diccionario de parámetros de los argumentos
		/// </summary>
		private static Dictionary<ArgumentType, string> GetParameters(string [] args)
		{
			Dictionary<ArgumentType, string> parameters = new Dictionary<ArgumentType, string>();

				// Añade los parámetros
				for (int index = 0; index < args.Length; index += 2)
					if (index < args.Length - 1 && !string.IsNullOrEmpty(args[index])) // Si tenemos un argumento después del actual (recuerda: -name "value")
					{
						if (IsArgument(args[index], ArgumentType.Context) && !parameters.ContainsKey(ArgumentType.Context))
							parameters.Add(ArgumentType.Context, args[index + 1]);
						else if (IsArgument(args[index], ArgumentType.Project) && !parameters.ContainsKey(ArgumentType.Project))
							parameters.Add(ArgumentType.Project, args[index + 1]);
					}
				// Devuelve el diccionario de parámetros
				return parameters;
		}

		/// <summary>
		///		Comrpueba si una cadena se corresponde con un argumento de entrada
		/// </summary>
		private static bool IsArgument(string argument, ArgumentType type)
		{
			return argument.Equals($"-{type.ToString()}", StringComparison.CurrentCultureIgnoreCase) ||
				   argument.Equals($"--{type.ToString()}", StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		///		Crea el manejador de log
		/// </summary>
		private static LogManager CreateLogger()
		{
			LogManager logger = new LogManager();

				// Asigna los manejadores de eventos
				logger.Logged += (sender, args) => WriteLog(args.Item.Level, args.Item);
				// Devuelve el manejador de log
				return logger;
		}

		/// <summary>
		///		Procesa un archivo de contexto con los archivos del directorio general
		/// </summary>
		private static async Task ProcessAsync(Dictionary<ArgumentType, string> parameters, System.Threading.CancellationToken cancellationToken)
		{
			string contextFileName = parameters[ArgumentType.Context].TrimIgnoreNull();
			string projectFileName = parameters[ArgumentType.Project].TrimIgnoreNull();

				using (BlockLogModel block = Logger.Default.CreateBlock(LogModel.LogType.Info, $"Process '{projectFileName}'"))
				{
					if (string.IsNullOrWhiteSpace(projectFileName) || !System.IO.File.Exists(projectFileName))
						block.Error($"Can't find the project file '{projectFileName}'");
					else if (string.IsNullOrWhiteSpace(contextFileName) || !System.IO.File.Exists(contextFileName))
						block.Error($"Can't find the context file '{contextFileName}'");
					else 
					{
						JobProjectManager manager = CreateManager();

							// Ejecuta el proceso
							try
							{
								if (!await manager.ProcessAsync(projectFileName, contextFileName, cancellationToken))
								{
									string error = string.Empty;

										// Añade los errores
										foreach (string innerError in manager.Errors)
											error = error.AddWithSeparator(innerError, Environment.NewLine);
										// Muestra el error
										block.Error(error.TrimIgnoreNull());
								}
							}
							catch (Exception exception)
							{
								block.Error($"Error when execute file '{projectFileName}' with context '{contextFileName}'", exception);
							}
							// Log
							block.Info($"End process '{projectFileName}' with context '{contextFileName}'");
					}
				}
		}

		/// <summary>
		///		Crea el manager de los procesos
		/// </summary>
		private static JobProjectManager CreateManager()
		{
			JobProjectManager manager = new JobProjectManager(Logger);

				// Añade los procesadores
				manager.AddProcessor(new Libraries.LibJobProcessor.Cloud.JobCloudManager(Logger));
				manager.AddProcessor(new Libraries.LibJobProcessor.Database.JobDatabaseManager(Logger));
				manager.AddProcessor(new Libraries.LibJobProcessor.Powershell.JobPowershellManager(Logger));
				manager.AddProcessor(new Libraries.LibJobProcessor.FilesShell.JobFileShellManager(Logger));
				// Devuelve el manager de proyecto
				return manager;
		}

		/// <summary>
		///		Escribe el log
		/// </summary>
		private static void WriteLog(int indent, LogModel item)
		{
			string message = $"[{item.CreatedAt:HH:mm:ss} : {item.Type.ToString()}]: {item.Message}";

				// Añade datos adicionales al mensaje
				if (item.Type == LogModel.LogType.Progress && (item.ActualProgress != 0 || item.TotalProgress != 0))
					message += $". {item.ActualProgress:#,##0} / {item.TotalProgress:#,##0'}";
				// Cambia el color
				ChangeColor(item.Type);
				// Escribe el mensaje
				WriteLog(indent, message);
				// Añade las excepciones al log
				if (item.Exception != null)
					WriteLog(indent + 1, $"Exception: {item.Exception.Message}");
				// Reinicia el color
				ResetColor();
		}

		/// <summary>
		///		Escribe un mensaje de log
		/// </summary>
		private static void WriteLog(int indent, string message)
		{
			Console.WriteLine(new string('\t', indent) + message);
		}

		/// <summary>
		///		Cambia el color de la consola
		/// </summary>
		private static void ChangeColor(LogModel.LogType type)
		{
			switch (type)
			{
				case LogModel.LogType.AssertCorrect:
						Console.ForegroundColor = ConsoleColor.DarkGray;
					break;
				case LogModel.LogType.AssertError:
						Console.ForegroundColor = ConsoleColor.DarkRed;
					break;
				case LogModel.LogType.Debug:
						Console.ForegroundColor = ConsoleColor.Gray;
					break;
				case LogModel.LogType.Error:
						Console.ForegroundColor = ConsoleColor.Red;
					break;
				case LogModel.LogType.Info:
						Console.ForegroundColor = ConsoleColor.White;
					break;
				case LogModel.LogType.Trace:
						Console.ForegroundColor = ConsoleColor.Green;
					break;
				case LogModel.LogType.Warning:
						Console.ForegroundColor = ConsoleColor.Blue;
					break;
			}
		}

		/// <summary>
		///		Reinicia el color de la consola
		/// </summary>
		private static void ResetColor()
		{
			Console.ResetColor();
		}

		/// <summary>
		///		Escribe la información de pruebas
		/// </summary>
		private static void WriteAssert(bool isError, string message)
		{
			// Cambia el color
			if (isError)
				Console.ForegroundColor = ConsoleColor.Red;
			// Muestra el mensaje
			Console.WriteLine(message);
			// Deja los colores como estaban
			Console.ResetColor();
		}

		/// <summary>
		///		Procesador de log
		/// </summary>
		private static LogManager Logger { get; set; }
	}
}
