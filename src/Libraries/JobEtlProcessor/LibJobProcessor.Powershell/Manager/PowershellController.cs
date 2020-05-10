using System;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibJobProcessor.Powershell.Models;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibPowerShellManager;

namespace Bau.Libraries.LibJobProcessor.Powershell.Manager
{
	/// <summary>
	///		Controlador para ejecutar sentencias de PowerShell
	/// </summary>
	internal class PowerShellController
	{
		internal PowerShellController(ScriptInterpreter interpreter, JobStepModel step)
		{
			Interpreter = interpreter;
			Step = step;
		}

		/// <summary>
		///		Ejecuta una sentencia de un script de Powershell
		/// </summary>
		internal async Task ExecuteAsync(BlockLogModel block, ExecuteScriptSentence sentence, NormalizedDictionary<object> parameters)
		{
			if (!string.IsNullOrWhiteSpace(sentence.Content))
				await ExecuteContentAsync(block, sentence.Content, parameters, sentence);
			else if (!string.IsNullOrWhiteSpace(sentence.FileName))
			{
				string fileName = Step.Project.GetFullFileName(sentence.FileName);

					if (!System.IO.File.Exists(fileName))
						Errors.Add($"Can't find the file '{fileName}'");
					else 
						await ExecuteFileAsync(block, fileName, parameters, sentence);
			}
			else
				Errors.Add("There is no content nor filename at sentence");
		}

		/// <summary>
		///		Ejecuta un archivo de Powershell
		/// </summary>
		private async Task ExecuteFileAsync(BlockLogModel block, string fileName, NormalizedDictionary<object> parameters, ExecuteScriptSentence sentence)
		{
			await ExecuteContentAsync(block, LibHelper.Files.HelperFiles.LoadTextFile(fileName), parameters, sentence);
		}

		/// <summary>
		///		Ejecuta un script de powershell
		/// </summary>
		private async Task ExecuteContentAsync(BlockLogModel block, string content, NormalizedDictionary<object> parameters, ExecuteScriptSentence sentence)
		{
			PowerShellManager manager = new PowerShellManager();

				// Carga el script en memoria
				manager.LoadScript(content);
				// Asigna los parámetros
				if (!AddParameters(manager, parameters, sentence, out string error))
					Errors.Add(error);
				else
				{
					// Ejecuta
					await manager.ExecuteAsync();
					// Comprueba los errores
					if (manager.Errors.Count > 0)
						Errors.AddRange(manager.Errors);
					else if (manager.OutputItems.Count > 0)
						foreach (object output in manager.OutputItems)
							block.Info($"{manager.OutputItems.IndexOf(output).ToString()}: {output?.ToString()}");
				}
		}

		/// <summary>
		///		Añade los parámetros al powershell que se va a ejecutar
		/// </summary>
		private bool AddParameters(PowerShellManager manager, NormalizedDictionary<object> parameters, ExecuteScriptSentence sentence, out string error)
		{
			// Inicializa los argumentos de salida
			error = string.Empty;
			// Añade los parámetros
			foreach ((string key, string value) in sentence.Mappings.Enumerate())
				if (parameters.ContainsKey(key))
					manager.AddParameter(value, parameters[key]);
			// Añade los directorios
			foreach ((string key, string value) in sentence.Paths.Enumerate())
				manager.AddParameter(key, Step.Project.GetFullFileName(value));
			// Devuelve el valor que indica si ha habido algún error
			return string.IsNullOrEmpty(error);
		}

		/// <summary>
		///		Intérprete
		/// </summary>
		internal ScriptInterpreter Interpreter { get; }

		/// <summary>
		///		Paso para el que se ejecuta el powersehll
		/// </summary>
		internal JobStepModel Step { get; }

		/// <summary>
		///		Errores del proceso
		/// </summary>
		internal System.Collections.Generic.List<string> Errors { get; } = new System.Collections.Generic.List<string>();
	}
}