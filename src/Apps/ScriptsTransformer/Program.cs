using System;

using Bau.Libraries.DbStudio.Conversor;
using Bau.Libraries.LibLogger.Core;

namespace ScriptsTransformer
{
	/// <summary>
	///		Clase principal de la consola
	/// </summary>
	static class Program
	{
		// Constantes privadas
		private const string SourceParameter = "source";
		private const string TargetParameter = "target";

		static void Main(string[] args)
		{
			ExporterOptions options = GetOptions(args);

				if (ValidateData(options))
					new DatabrickExporter(GetLogger(), options).Export();
		}

		/// <summary>
		///		Crea el logger
		/// </summary>
		private static LogManager GetLogger()
		{
			LogManager manager = new LogManager();

				// Añade los procesos de escritura en el log
				manager.AddWriter(new Bau.Libraries.LibLogger.Writer.Console.LogConsoleWriter());
				// Devuelve el generador
				return manager;
		}

		/// <summary>
		///		Genera las opciones
		/// </summary>
		private static ExporterOptions GetOptions(string[] args)
		{
			ExporterOptions options = new ExporterOptions(GetArgument(SourceParameter, args), GetArgument(TargetParameter, args));

				// Añade los parámetros adicionales
				AssignParameters(options, args);
				// Devuelve los parámetros
				return options;
		}

		/// <summary>
		///		Obtiene un argumento
		/// </summary>
		private static string GetArgument(string key, string[] args)
		{
			// Busca el valor del argumento posterior a la clave
			for (int index = 0; index < args.Length; index++)
				if (!string.IsNullOrWhiteSpace(args[index]) && args[index].Equals($"-{key}", StringComparison.CurrentCultureIgnoreCase) && index < args.Length - 1)
					return args[index + 1];
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return string.Empty;
		}

		/// <summary>
		///		Asigna el resto de parámetros
		/// </summary>
		private static void AssignParameters(ExporterOptions options, string[] args)
		{
			for (int index = 0; index < args.Length; index++)
			{
				string name = GetParameterName(args[index]);

					if (!string.IsNullOrWhiteSpace(name) && 
							!name.Equals(SourceParameter, StringComparison.CurrentCultureIgnoreCase) && 
							!name.Equals(TargetParameter, StringComparison.CurrentCultureIgnoreCase) && 
							index < args.Length - 1)
						options.AddParameter(name, args[index + 1]);
			}
		}

		/// <summary>
		///		Obtiene el nombre de parámetro de un argumento
		/// </summary>
		private static string GetParameterName(string argument)
		{
			if (!string.IsNullOrWhiteSpace(argument) && argument.StartsWith("-") && argument.Length > 1)
				return argument.Substring(1);
			else
				return string.Empty;
		}

		/// <summary>
		///		Comprueba los datos
		/// </summary>
		private static bool ValidateData(ExporterOptions options)
		{
			if (string.IsNullOrWhiteSpace(options.SourcePath))
				throw new Exception("Source path undefined");
			else if (!System.IO.Directory.Exists(options.SourcePath))
				throw new Exception($"Cant find the path '{options.SourcePath}'");
			else if (string.IsNullOrWhiteSpace(options.TargetPath))
				throw new Exception("Target path undefined");
			else
				return true;
		}
	}
}
