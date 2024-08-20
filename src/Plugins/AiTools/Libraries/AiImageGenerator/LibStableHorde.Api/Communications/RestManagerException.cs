namespace Bau.Libraries.LibStableHorde.Api.Communications;

/// <summary>
///		Excepciones de comunicación con la API
/// </summary>
internal class RestManagerException : Exception
{
	internal RestManagerException() {}

	internal RestManagerException(string message) : base(message) {}

	internal RestManagerException(string message, Exception innerException) : base(message, innerException) {}
}
