using System;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files.EventArguments
{
	/// <summary>
	///		Argumentos del evento de solicitud de texto seleccionado en un editor
	/// </summary>
	public class EditorSelectedTextRequiredEventArgs : EventArgs
	{
		/// <summary>
		///		Texto seleccionado
		/// </summary>
		public string SelectedText { get; set; }
	}
}
