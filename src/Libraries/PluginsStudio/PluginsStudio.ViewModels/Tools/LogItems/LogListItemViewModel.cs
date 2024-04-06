namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.LogItems;

/// <summary>
///		ViewModel con los datos de un elemento de log
/// </summary>
public class LogListItemViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
{
	// Variables privadas
	private string _type = string.Empty, _content = string.Empty, _formattedHour = string.Empty;
	private DateTime _createdAt;

	public LogListItemViewModel(LogListViewModel listViewModel, string type, string content, DateTime createdAt, BauMvvm.ViewModels.Media.MvvmColor color) 
				: base(content, null, false, color)
	{
		// Asigna las propiedades
		ListViewModel = listViewModel;
		Type = type;
		Content = content;
		if (content.Length > 200)
			Text = content.Substring(0, 200);
		else
			Text = content;
		if (!string.IsNullOrWhiteSpace(Text))
		{
			Text = Text.Replace("\r\n", " ");
			Text = Text.Replace("\r", " ");
			Text = Text.Replace("\n", " ");
		}
		CreatedAt = createdAt;
		// Asigna los comandos
		ShowDetailsCommand = new BauMvvm.ViewModels.BaseCommand(_ => ShowDetails());
	}

	/// <summary>
	///		Muestra los detalles del log
	/// </summary>
	private void ShowDetails()
	{
		ListViewModel.MainViewModel.MainController.MainWindowController.SystemController.ShowMessage(Content);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public LogListViewModel ListViewModel { get; }

	/// <summary>
	///		Tipo de log
	/// </summary>
	public string Type
	{
		get { return _type; }
		set { CheckProperty(ref _type, value); }
	}

	/// <summary>
	///		Contenido completo del mensaje
	/// </summary>
	public string Content
	{
		get { return _content; }
		set { CheckProperty(ref _content, value); }
	}

	/// <summary>
	///		Fecha de creación
	/// </summary>
	public DateTime CreatedAt
	{
		get { return _createdAt; }
		set 
		{ 
			if (CheckProperty(ref _createdAt, value))
				FormattedHour = $"{value:HH:mm:ss}";
		}
	}

	/// <summary>
	///		Hora formateada
	/// </summary>
	public string FormattedHour
	{
		get { return _formattedHour; }
		set { CheckProperty(ref _formattedHour, value); }
	}

	/// <summary>
	///		Comando para mostrar los detalles del log
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand ShowDetailsCommand { get; }
}
