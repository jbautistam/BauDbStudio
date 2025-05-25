using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.JobsProcessor.Application.Models;

/// <summary>
///		Clase con los datos de un comando
/// </summary>
public class CommandModel
{
	/// <summary>
	///		Obtiene una cadena con los argumentos
	/// </summary>
	public string GetArguments(ContextModel context)
	{
		string arguments = string.Empty;

			// Añade los argumentos
			foreach (ArgumentModel argument in Arguments)
				if (argument.Position == ArgumentModel.ArgumentPosition.CommandLine)
				{
					string argumentValue = argument.GetArgumentValue(context);

						// Añade comillas si es necesario en el valor del argumento
						if (string.IsNullOrWhiteSpace(argumentValue))
							argumentValue = "\"\"";
						else if (argumentValue.IndexOf(' ') >= 0)
							argumentValue = $"\"{argumentValue}\"";
						// Añade el agumento a la línea de ocmandos
						arguments = arguments.AddWithSeparator($"{argument.Parameter.Name} {argumentValue}", " ");
				}
			// Devuelve la cadena con los argumentos
			return arguments;
	}

	/// <summary>
	///		Obtiene un diccionario con los argumentos convertidos con los datos del contexto
	/// </summary>
	public List<ArgumentModel> GetConvertedArguments(ContextModel context)
	{
		List<ArgumentModel> arguments = [];

			// Añade los argumentos
			foreach (ArgumentModel argument in Arguments)
				arguments.Add(argument.GetConverted(context));
			// Devuelve la cadena con los argumentos
			return arguments;
	}

	/// <summary>
	///		Obtiene las variables de entorno
	/// </summary>
	public Dictionary<string, string> GetEnvironmentVariables(ContextModel context)
	{
		Dictionary<string, string> variables = [];

			// Asigna las variables
			foreach (ArgumentModel argument in Arguments)
				if (argument.Position == ArgumentModel.ArgumentPosition.Environment)
					variables.Add(argument.Parameter.Name, argument.GetArgumentValue(context));
			// Devuelve el diccionario
			return variables;
	}

	/// <summary>
	///		Comprueba si el código de salida del proceso es válido según los parámetros
	/// </summary>
	public bool ValidateExitCode(int exitCode)
	{
		bool validated = false;

			// Comprueba si el código de salida está entre los códigos de acceso válidos
			if (ValidationExitCode.Count == 0)
				validated = true;
			else
				foreach (CommandValidationExitCodeModel exitCodeValid in ValidationExitCode)
					if (!validated)
						validated = exitCodeValid.Validate(exitCode);
			// Devuelve el valor que indica si los códigos son válidos
			return validated;
	}

	/// <summary>
	///		Id del comando
	/// </summary>
	public string Id { get; } = Guid.NewGuid().ToString();

	/// <summary>
	///		Nombre del ejecutable
	/// </summary>
	public string FileName { get; set; } = default!;

	/// <summary>
	///		Argumentos de la consola
	/// </summary>
	public List<ArgumentModel> Arguments { get; } = [];

	/// <summary>
	///		Indica si se debe detener con el primer error
	/// </summary>
	public bool StopWhenError { get; set; } = true;

	/// <summary>
	///		Códigos de acceso válidos
	/// </summary>
	public List<CommandValidationExitCodeModel> ValidationExitCode { get; } = [];
}
