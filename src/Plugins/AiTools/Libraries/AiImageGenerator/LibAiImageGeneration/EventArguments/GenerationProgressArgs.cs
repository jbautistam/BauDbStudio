namespace Bau.Libraries.LibAiImageGeneration.EventArguments;

/// <summary>
///		Argumentos del mensaje de final de generación
/// </summary>
public class GenerationProgressArgs : EventArgs
{
	public GenerationProgressArgs(Models.GenerationModel generation)
	{
		Generation = generation;
	}

	/// <summary>
	///		Datos de la generación
	/// </summary>
	public Models.GenerationModel Generation { get; }
}
