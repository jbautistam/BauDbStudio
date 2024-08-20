using System.Runtime.Serialization;

namespace Bau.Libraries.ToDoManager.Application.Exceptions;

/// <summary>
///		Excepciones de <see cref="ToDoManager"/>
/// </summary>
public class ToDoManagerException : Exception
{
	public ToDoManagerException()
	{
	}

	public ToDoManagerException(string? message) : base(message)
	{
	}

	public ToDoManagerException(string? message, Exception? innerException) : base(message, innerException)
	{
	}

	protected ToDoManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}