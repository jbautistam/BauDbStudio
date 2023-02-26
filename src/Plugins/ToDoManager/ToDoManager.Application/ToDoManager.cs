using System;

namespace Bau.Libraries.ToDoManager.Application
{
	/// <summary>
	///		Manager de la aplicación
	/// </summary>
	public class ToDoManager
	{
		/// <summary>
		///		Manager de notas
		/// </summary>
		public ToDoNotesManager NotesManager { get; } = new();
	}
}
