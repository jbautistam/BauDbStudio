using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Managers
{
	/// <summary>
	///		Base para los manager de sentencias
	/// </summary>
	internal abstract class BaseManager
	{
		// Evento de progreso
		public event EventHandler<EventArgument.ProgressEventArgs> Progress;

		protected BaseManager(DbScriptProcessor processor)
		{
			Processor = processor;
		}

		/// <summary>
		///		Lanza un evento de medición
		/// </summary>
		protected void RaiseProgress(string message, long records)
		{
			Progress?.Invoke(this, new EventArgument.ProgressEventArgs(message, records));
		}

		/// <summary>
		///		Manager
		/// </summary>
		protected DbScriptProcessor Processor { get; }
	}
}
