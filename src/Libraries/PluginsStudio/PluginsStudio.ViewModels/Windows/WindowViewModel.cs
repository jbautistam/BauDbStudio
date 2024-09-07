
using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.PluginsStudio.ViewModels.Windows;

/// <summary>
///		ViewModel de una ventana
/// </summary>
public class WindowViewModel : BaseObservableObject
{
	/// <summary>
	///		Tipo de ventana
	/// </summary>
	public enum WindowType
	{
		/// <summary>Documento</summary>
		Document,
		/// <summary>Panel</summary>
		Pane
	}
	// Variables privadas
	private string _id = default!, _documentId = default!, _name = default!;
	private WindowType _type;
	private bool _visible, _active, _firstTime = true;

	public WindowViewModel(PluginsStudioViewModel mainViewModel, string id, string documentId, string name, WindowType type)
	{
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		Id = id;
		DocumentId = documentId;
		Name = name;
		Type = type;
		Visible = true;
		// Asigna los comandos
		ActivateDocumentCommand = new BaseCommand(_ => ActivateDocument(), _ => Visible && Type == WindowType.Document);
	}

	/// <summary>
	///		Activa la ventana de un documento
	/// </summary>
	private void ActivateDocument()
	{
		MainViewModel.MainController.MainWindowController.ActivateDetails(Id, DocumentId);
	}

	/// <summary>
	///		Modifica la visibilidad
	/// </summary>
	private void UpdateVisibility()
	{
		// Modifica la visibilidad si es un panel y no estamos en la carga del panel
		if (Type == WindowType.Pane && !_firstTime)
			if (!MainViewModel.MainController.MainWindowController.ShowPane(Id, DocumentId, Visible))
			{
				// Si no se puede ocultar, deja el valor de Visible como estaba
				_firstTime = true;
				Visible = !Visible;
				_firstTime = false;
			}
		// Indica que no es la primera vez
		_firstTime = false;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Id de la ventana
	/// </summary>
	public string Id
	{
		get { return _id; }
		set { CheckProperty(ref _id, value); }
	}

	/// <summary>
	///		Id del documento / panel
	/// </summary>
	public string DocumentId
	{
		get { return _documentId; }
		set { CheckProperty(ref _documentId, value); }
	}

	/// <summary>
	///		Nombre de la ventana
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Tipo de la ventana
	/// </summary>
	public WindowType Type
	{
		get { return _type; }
		set { CheckProperty(ref _type, value); }
	}

	/// <summary>
	///		Indica si la ventana está activa
	/// </summary>
	public bool Active 
	{ 
		get { return _active; } 
		set { CheckProperty(ref _active, value); }
	}

	/// <summary>
	///		Indica si la ventana está visible
	/// </summary>
	public bool Visible
	{
		get { return _visible; }
		set 
		{ 
			if (CheckProperty(ref _visible, value))
				UpdateVisibility();
		}
	}

	/// <summary>
	///		Comando para activar un documento
	/// </summary>
	public BaseCommand ActivateDocumentCommand { get; }
}
