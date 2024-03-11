namespace Bau.Libraries.AiTools.Application.Models.Chat;

/// <summary>
///		Modelo con un mensaje de chat
/// </summary>
public class ChatModel
{
	/// <summary>
	///		Origen del mensaje
	/// </summary>
	public enum SourceType
	{
		/// <summary>Humano</summary>
		Human,
		/// <summary>Inteligencia artificial</summary>
		ArtificialIntelligence,
		/// <summary>Mensaje de error del sistema</summary>
		Error
	}

	/// <summary>
	///		Mensaje de chat
	/// </summary>
	public string Message { get; set; } = default!;

	/// <summary>
	///		Tipo de origen
	/// </summary>
	public SourceType Type { get; set; }
}
