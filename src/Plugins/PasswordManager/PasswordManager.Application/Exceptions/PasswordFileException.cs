namespace Bau.Libraries.PasswordManager.Application.Exceptions;

/// <summary>
///		Excepciones de <see cref="PasswordManager"/>
/// </summary>
public class PasswordFileException : Exception
{
	public PasswordFileException()
	{
	}

	public PasswordFileException(string? message) : base(message)
	{
	}

	public PasswordFileException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}