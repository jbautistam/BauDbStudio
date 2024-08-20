namespace Bau.Libraries.LibOllama.Api.Models;

/// <summary>
///		Respuesta de finalizaci�n de <see cref="GenerateCompletionRequest"/>
/// </summary>
public class GenerateCompletionDoneResponseStream : GenerateCompletionResponseStream
{
	/// <summary>
	///		Contexto
	/// </summary>
	public List<long> Context { get; set; } = new();

	/// <summary>
	///		Duraci�n total
	/// </summary>
	public long TotalDuration { get; set; }

	/// <summary>
	///		Duraci�n de la carga
	/// </summary>
	public long LoadDuration { get; set; }

	/// <summary>
	///		N�mero de elementos de la evaluaci�n del prompt
	/// </summary>
	public int PromptEvalCount { get; set; }

	/// <summary>
	///		Tiempo de la evaluaci�n del prompt
	/// </summary>
	public long PromptEvalDuration { get; set; }

	/// <summary>
	///		Evaluaci�n del prompt
	/// </summary>
	public int EvalCount { get; set; }

	/// <summary>
	///		Duraci�n del prompt
	/// </summary>
	public long EvalDuration { get; set; }
}