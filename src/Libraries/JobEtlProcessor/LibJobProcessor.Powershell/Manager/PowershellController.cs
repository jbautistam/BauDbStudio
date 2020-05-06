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
		internal void Execute(BlockLogModel block, ExecuteScriptSentence sentence, NormalizedDictionary<object> parameters)
		{
			if (!string.IsNullOrWhiteSpace(sentence.Content))
				ExecuteContent(block, sentence.Content, parameters, sentence.Mappings);
			else if (!string.IsNullOrWhiteSpace(sentence.FileName))
			{
				string fileName = Step.Project.GetFullFileName(sentence.FileName);

					if (!System.IO.File.Exists(fileName))
						Errors.Add($"Can't find the file '{fileName}'");
					else 
						ExecuteFile(block, fileName, parameters, sentence.Mappings);
			}
			else
				Errors.Add("There is no content nor filename at sentence");
		}

		/// <summary>
		///		Ejecuta un archivo de Powershell
		/// </summary>
		private void ExecuteFile(BlockLogModel block, string fileName, NormalizedDictionary<object> parameters, NormalizedDictionary<string> mappings)
		{
			ExecuteContent(block, LibHelper.Files.HelperFiles.LoadTextFile(fileName), parameters, mappings);
		}

		/// <summary>
		///		Ejecuta un script de powershell
		/// </summary>
		private void ExecuteContent(BlockLogModel block, string content, NormalizedDictionary<object> parameters, NormalizedDictionary<string> mappings)
		{
			PowerShellManager manager = new PowerShellManager();

				// Carga el script en memoria
				manager.LoadScript(content);
				// Asigna los parámetros
				if (!AddParameters(manager, parameters, mappings, out string error))
					Errors.Add(error);
				else
				{
					// Ejecuta
					manager.Execute();
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
		private bool AddParameters(PowerShellManager manager, NormalizedDictionary<object> parameters, NormalizedDictionary<string> mappings, out string error)
		{
			// Inicializa los argumentos de salida
			error = string.Empty;
			// Añade los parámetros
			if (mappings != null)
				foreach ((string key, string value) in mappings.Enumerate())
					if (parameters.ContainsKey(key))
						manager.AddParameter(value, parameters[key]);
			// Añade los directorios
			/*
				else
				{
					string path = GetPathConverted(, key, parameters);

						if (string.IsNullOrEmpty(path))
							error = $"Can't find value for parameter {key}";
						else
						{
							// Crea el directorio
							LibHelper.Files.HelperFiles.MakePath(path);
							// Añade el parámetro
							manager.AddParameter(value, path);
						}
				}
			*/
			// Devuelve el valor que indica si ha habido algún error
			return string.IsNullOrEmpty(error);
		}

/*
		/// <summary>
		///		En la clave de los mapeos, se le puede poner una cadena especial de tipo PathXXX/yyyy donde
		///	PathXXX es un nombre de directorio predefinido e yyyy es un subdirectorio
		/// </summary>
		private string GetPathConverted(string fileName, string key, NormalizedDictionary<object> parameters)
		{
			string [] parts = key.Split('\\');
			string path = string.Empty;

				// Obtiene el directorio inicial (sólo con la primera parte)
				if (parts[0].Equals(FixedParameter.PathClient.ToString(), StringComparison.CurrentCultureIgnoreCase))
					path = Interpreter ProcessFileManager.Manager.Project.Context.ClientPath;
				else if (parts[0].EqualsIgnoreCase(FixedParameter.PathSqlScripts.ToString()))
					path = ProcessFileManager.Manager.Project.PathBaseSqlScripts;
				else if (parts[0].EqualsIgnoreCase(FixedParameter.PathStepsScripts.ToString()))
					path = ProcessFileManager.Manager.Project.PathBaseStepsScripts;
				else if (parts[0].EqualsIgnoreCase(FixedParameter.PathActualScript.ToString()))
					path = System.IO.Path.GetDirectoryName(fileName);
				// Añade los subdirectorios si es necesario
				if (!string.IsNullOrWhiteSpace(path))
					for (int index = 1; index < parts.Length; index++)
						if (!string.IsNullOrWhiteSpace(parts[index]))
							path = System.IO.Path.Combine(path, parts[index]);
				// Devuelve el directorio
				return path;
		}
*/

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
