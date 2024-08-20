using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.ToDoManager.Application.Notes.Models;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.ToDoManager.Application.Notes.Repository;

/// <summary>
///		Repositorio de <see cref="NoteModel"/>
/// </summary>
internal class NotesRepository
{
	// Constantes privadas
	private const string TagRoot = "Notes";
	private const string TagNote = "Note";
	private const string TagTop = "Top";
	private const string TagLeft = "Left";
	private const string TagWidth = "Width";
	private const string TagHeight = "Height";
	private const string TagTitle = "Title";
	private const string TagContent = "Content";
	private const string TagOverAllWindows = "OverAllWindows";
	private const string TagBackground = "Background";
	private const string TagCreatedAt = "CreatedAt";
	private const string TagUpdatedAt = "UpdatedAt";

	/// <summary>
	///		Carga las notas de un archivo
	/// </summary>
	internal List<NoteModel> Load(string fileName)
	{
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);
		List<NoteModel> notes = new();

			// Carga los datos del archivo
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							if (nodeML.Name == TagNote)
								notes.Add(LoadNote(nodeML));
			// Devuelve la colección de notas
			return notes;
	}

	/// <summary>
	///		Carga los datos de la nota
	/// </summary>
	private NoteModel LoadNote(MLNode rootML)
	{
		return new ()
				{
					Title = rootML.Nodes[TagTitle].Value.TrimIgnoreNull(),
					Content = rootML.Nodes[TagContent].Value.TrimIgnoreNull(),
					OverAllWindows = rootML.Attributes[TagOverAllWindows].Value.GetBool(),
					Background = rootML.Attributes[TagBackground].Value.TrimIgnoreNull(),
					Top = rootML.Attributes[TagTop].Value.GetInt(0),
					Left = rootML.Attributes[TagLeft].Value.GetInt(0),
					Width = rootML.Attributes[TagWidth].Value.GetInt(200),
					Height = rootML.Attributes[TagHeight].Value.GetInt(200),
					CreatedAt = rootML.Attributes[TagCreatedAt].Value.GetDateTime(DateTime.Now),
					UpdatedAt = rootML.Attributes[TagUpdatedAt].Value.GetDateTime(DateTime.Now)
				};
	}

	/// <summary>
	///		Carga las notas de un archivo
	/// </summary>
	internal void Save(string fileName, List<NoteModel> notes)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade las notas
			foreach (NoteModel note in notes)
				rootML.Nodes.Add(GetXmlNode(note));
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Obtiene el XML de una nota
	/// </summary>
	private MLNode GetXmlNode(NoteModel note)
	{
		MLNode nodeML = new(TagNote);

			// Añade los datos del nodo
			nodeML.Nodes.Add(TagTitle, note.Title);
			nodeML.Nodes.Add(TagContent, note.Content);
			nodeML.Attributes.Add(TagOverAllWindows, note.OverAllWindows);
			nodeML.Attributes.Add(TagBackground, note.Background);
			nodeML.Attributes.Add(TagTop, note.Top);
			nodeML.Attributes.Add(TagLeft, note.Left);
			nodeML.Attributes.Add(TagWidth, note.Width);
			nodeML.Attributes.Add(TagHeight, note.Height);
			nodeML.Attributes.Add(TagCreatedAt, note.CreatedAt);
			nodeML.Attributes.Add(TagUpdatedAt, note.UpdatedAt);
			// Devuelve los datos del nodo
			return nodeML;
	}
}