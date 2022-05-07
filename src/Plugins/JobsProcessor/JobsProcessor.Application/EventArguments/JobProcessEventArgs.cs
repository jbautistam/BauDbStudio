using System;

namespace Bau.Libraries.JobsProcessor.Application.EventArguments
{
	/// <summary>
	///		Argumentos del evento de proceso de un trabajo
	/// </summary>
	public class JobProcessEventArgs : EventArgs
	{
		/// <summary>
		///		Estado del proceso
		/// </summary>
		public enum StatusType
		{
			/// <summary>Inicio del proceso</summary>
			Start,
			/// <summary>Fin del proceso</summary>
			End,
			/// <summary>Error en el proceso</summary>
			Error,
			/// <summary>Datos informativos</summary>
			Information
		}

		public JobProcessEventArgs(Models.ContextModel context, Models.CommandModel command, StatusType status, string message = null,
								   int? actual = null, int? total = null)
		{
			Context = context;
			Command = command;
			Status = status;
			Message = message;
			Actual = actual;
			Total = total;
		}

		/// <summary>
		///		Datos de contexto que se está ejecutando
		/// </summary>
		public Models.ContextModel Context { get; }

		/// <summary>
		///		Comando que se está ejecutando
		/// </summary>
		public Models.CommandModel Command { get; }

		/// <summary>
		///		Estado del proceso
		/// </summary>
		public StatusType Status { get; }

		/// <summary>
		///		Mensaje de proceso
		/// </summary>
		public string Message { get; }

		/// <summary>
		///		Progreso actual
		/// </summary>
		public int? Actual { get; }

		/// <summary>
		///		Número de procesos total
		/// </summary>
		public int? Total { get; }

		/// <summary>
		///		Fecha de proceso
		/// </summary>
		public DateTime Date { get; } = DateTime.Now;
	}
}
