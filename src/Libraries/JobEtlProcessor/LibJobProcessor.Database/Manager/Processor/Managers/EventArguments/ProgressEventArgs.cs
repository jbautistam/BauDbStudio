using System;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.EventArgument
{
	/// <summary>
	///		Argumentos del evento de progreso
	/// </summary>
	internal class ProgressEventArgs : EventArgs
	{
		internal ProgressEventArgs(string message, long actual)
		{
			Message = message;
			Actual = actual;
		}

		/// <summary>
		///		Mensaje
		/// </summary>
		internal string Message { get; }

		/// <summary>
		///		Progreso actual
		/// </summary>
		internal long Actual { get; }
	}
}
