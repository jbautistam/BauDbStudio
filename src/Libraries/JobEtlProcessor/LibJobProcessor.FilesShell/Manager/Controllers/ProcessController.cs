using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Processes;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Manager.Controllers
{
	/// <summary>
	///		Controlador para la ejecución de comandos
	/// </summary>
	internal class ProcessController
	{
		internal ProcessController(ScriptInterpreter interpreter)
		{
			Interpreter = interpreter;
		}

		/// <summary>
		///		Ejecuta un proceso
		/// </summary>
		internal void Execute(BlockLogModel block, string process, List<string> arguments)
		{
		}

		/// <summary>
		///		Intérprete que lanza el controlador
		/// </summary>
		internal ScriptInterpreter Interpreter { get; }
	}
}
