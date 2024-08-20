using Bau.Libraries.ToDoManager.Application.Notes.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.Notes;

/// <summary>
///		ViewModel para <see cref="NoteModel"/>
/// </summary>
public class NoteViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _title = string.Empty, _content = string.Empty;
	private bool _overAllWindows;
	private int _top, _left, _width, _height;
	private BauMvvm.ViewModels.Media.MvvmColor _background = BauMvvm.ViewModels.Media.MvvmColor.Yellow;

	public NoteViewModel(ToDoManagerViewModel mainViewModel, NoteModel note, bool isNew)
	{
		// Inicializa las propiedades
		MainViewModel = mainViewModel;
		Note = note;
		IsNew = isNew;
		// Inicializa la nota
		InitProperties();
		// Inicializa los comandos
		DeleteCommand = new BauMvvm.ViewModels.BaseCommand(_ => Delete());
	}

	/// <summary>
	///		Inicializa las propiedades
	/// </summary>
	private void InitProperties()
	{
		Title = Note.Title;
		Content = Note.Content;
		OverAllWindows = Note.OverAllWindows;
		Top = Note.Top;
		Left = Note.Left;
		Width = Note.Width;
		Height = Note.Height;
		Background = new BauMvvm.ViewModels.Media.MvvmColor(Note.Background);
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		// Borra, modifica o crea la nota
		if (IsDeleted)
			MainViewModel.ToDoManager.NotesManager.Notes.Delete(Note);
		else
		{
			// Asigna las propiedades a la nota
			Note.Title = Title;
			Note.Content = Content;
			Note.OverAllWindows = OverAllWindows;
			Note.Top = Top;
			Note.Left = Left;
			Note.Width = Width;
			Note.Height = Height;
			Note.Background = Background.ToStringRgb();
			// Cambia la fecha de modificación
			Note.UpdatedAt = DateTime.Now;
			// Agrega la nota si es nueva
			if (IsNew)
			{
				MainViewModel.ToDoManager.NotesManager.Notes.Add(Note);
				IsNew = false;
			}
		}
		// Graba los datos
		MainViewModel.ToDoManager.NotesManager.Save();
	}

	/// <summary>
	///		Borra la nota
	/// </summary>
	public void Delete()
	{
		if ((string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Content)) || 
			MainViewModel.ViewsController.HostController.SystemController.ShowQuestion("¿Desea borrar esta nota?"))
		{
			// Indica que se ha borrado
			IsDeleted = true;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }

	/// <summary>
	///		Nota
	/// </summary>
	public NoteModel Note { get; }

	/// <summary>
	///		Indica si es nuevo
	/// </summary>
	public bool IsNew { get; private set; }

	/// <summary>
	///		Indica si se debe borrar
	/// </summary>
	public bool IsDeleted { get; private set; }

	/// <summary>
	///		Título de la nota
	/// </summary>
	public string Title
	{
		get { return _title; }
		set { CheckProperty(ref _title, value); }
	}

	/// <summary>
	///		Contenido de la nota
	/// </summary>
	public string Content
	{
		get { return _content; }
		set { CheckProperty(ref _content, value); }
	}

	/// <summary>
	///		Indica si se debe mostrar sobre todas las ventanas
	/// </summary>
	public bool OverAllWindows
	{
		get { return _overAllWindows; }
		set { CheckProperty(ref _overAllWindows, value); }
	}

	/// <summary>
	///		Coordenada superior de la ventana
	/// </summary>
	public int Top
	{
		get { return _top; }
		set { CheckProperty(ref _top, value); }
	}

	/// <summary>
	///		Coordenada izquierda de la ventana
	/// </summary>
	public int Left
	{
		get { return _left; }
		set { CheckProperty(ref _left, value); }
	}

	/// <summary>
	///		Ancho de la ventana
	/// </summary>
	public int Width
	{
		get { return _width; }
		set { CheckProperty(ref _width, value); }
	}

	/// <summary>
	///		Altura de la ventana
	/// </summary>
	public int Height
	{
		get { return _height; }
		set { CheckProperty(ref _height, value); }
	}

	/// <summary>
	///		Color de fondo
	/// </summary>
	public BauMvvm.ViewModels.Media.MvvmColor Background
	{
		get { return _background; }
		set { CheckObject(ref _background, value); }
	}

	/// <summary>
	///		Comando de borrado
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand DeleteCommand { get; }
}
