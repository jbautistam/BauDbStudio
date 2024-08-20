using System;
using System.Runtime.Serialization;

namespace Bau.Libraries.ChessDataBase.Models.Exceptions
{
	/// <summary>
	///		Excepción provocada en la lectura del juego
	/// </summary>
	public class GameReaderException : Exception
	{
		public GameReaderException() { }

		public GameReaderException(string message) : base(message) { }

		public GameReaderException(string message, Exception innerException) : base(message, innerException) { }

		protected GameReaderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
