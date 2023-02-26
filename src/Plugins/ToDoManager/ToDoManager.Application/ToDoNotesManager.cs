using System;

namespace Bau.Libraries.ToDoManager.Application
{
	/// <summary>
	///		Manager de notas de la aplicación
	/// </summary>
	public class ToDoNotesManager
	{
		/// <summary>
		///		Carga las notas
		/// </summary>
		public void Load(string folder)
		{
			Folder = folder;
			Notes.Notes.Clear();
			Notes.Notes.AddRange(new Repository.NotesRepository().Load(GetFileName(Folder)));
		}

		/// <summary>
		///		Graba las notas
		/// </summary>
		public void Save()
		{
			new Repository.NotesRepository().Save(GetFileName(Folder), Notes.Notes);
		}

		/// <summary>
		///		Obtiene el nombre del archivo
		/// </summary>
		private string GetFileName(string path)
		{
			return Path.Combine(path, "Notes.Todo.xml");
		}

		/// <summary>
		///		Carpeta
		/// </summary>
		public string Folder { get; private set; } = default!;

		/// <summary>
		///		Notas
		/// </summary>
		public Models.Notes.NotesRootModel Notes { get; } = new();
	}
}
