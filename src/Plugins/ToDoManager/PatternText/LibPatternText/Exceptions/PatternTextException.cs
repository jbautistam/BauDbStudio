using System.Runtime.Serialization;

namespace Bau.Libraries.LibPatternText.Exceptions;

/// <summary>
///		Excepción del intérprete
/// </summary>
public class PatternTextException : Exception
{
	public PatternTextException() {}

	public PatternTextException(string? message) : base(message) {}

	public PatternTextException(string? message, Exception? innerException) : base(message, innerException) {}

	protected PatternTextException(SerializationInfo info, StreamingContext context) : base(info, context) {}
}
