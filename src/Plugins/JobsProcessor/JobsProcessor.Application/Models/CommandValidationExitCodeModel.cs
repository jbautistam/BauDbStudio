namespace Bau.Libraries.JobsProcessor.Application.Models;

/// <summary>
///		Datos para comprobar si un ExitCode del comando es válido
/// </summary>
public class CommandValidationExitCodeModel
{
	/// <summary>
	///		Comprueba si el código de salida está en los límites de validez
	/// </summary>
	public bool Validate(int exitCode)
	{
		int minimum = Minimum ?? int.MinValue;
		int maximum = Maximum ?? int.MaxValue;

			// Comprueba si el código de salida está en los límites válidos
			return exitCode >= minimum && exitCode <= maximum;
	}

	/// <summary>
	///		Valor mínimo
	/// </summary>
	public int? Minimum { get; set; }

	/// <summary>
	///		Valor máximo
	/// </summary>
	public int? Maximum { get; set; }
}
