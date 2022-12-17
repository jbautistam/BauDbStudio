using System.Runtime.Serialization;

namespace Bau.Libraries.ToDoManager.Application.Exceptions;

/// <summary>
///		Excepciones de <see cref="ToDoManager"/>
/// </summary>
public class ToDoFileException : Exception
{
	public ToDoFileException()
	{
	}

	public ToDoFileException(string? message) : base(message)
	{
	}

	public ToDoFileException(string? message, Exception? innerException) : base(message, innerException)
	{
	}

	protected ToDoFileException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}