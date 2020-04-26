using System;

namespace Bau.DbStudio.Controls
{
	/// <summary>
	///		Arbumentos del evento de cierre
	/// </summary>
	public class ClosingEventArgs : EventArgs
	{
		public ClosingEventArgs(DockLayoutDocument document)
		{
			Document = document;
		}

		/// <summary>
		///		Documento que lanza el evento de cierre
		/// </summary>
		public DockLayoutDocument Document { get; }

		/// <summary>
		///		Indica si se ha cancelado el cierre
		/// </summary>
		public bool Cancel { get; set; } 
	}
}
