namespace StableHorde.Console.Controller.EventArguments;

/// <summary>
///		Argumentos del evento de generación de las imágenes
/// </summary>
public class EndGenerationArgs : EventArgs
{
	public EndGenerationArgs(Models.GenerationModel generation)
	{
		Generation = generation;
	}

	/// <summary>
	///		Datos de la generación finalizada
	/// </summary>
	public Models.GenerationModel Generation { get; }
}
