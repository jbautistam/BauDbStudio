using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.LibBlogReader.ViewModel.Blogs;

/// <summary>
///		ViewModel de <see cref="HyperlinkModel"/>
/// </summary>
public class HyperlinkViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{ 
	// Variables privadas
	private FolderModel? _parent;
	private HyperlinkModel _hyperlink = default!;
	private string _name = default!, _description = default!, _url = default!;

	public HyperlinkViewModel(BlogReaderViewModel mainViewModel, FolderModel? parent, HyperlinkModel? hyperlink)
	{
		MainViewModel = mainViewModel;
		if (hyperlink is null)
		{
			_hyperlink = new HyperlinkModel();
			_hyperlink.Folder = parent;
		}
		else
			_hyperlink = hyperlink;
		InitProperties();
	}

	/// <summary>
	///		Inicializa las propiedades
	/// </summary>
	private void InitProperties()
	{
		Parent = _hyperlink.Folder;
		Name = _hyperlink.Name;
		Description = _hyperlink.Description;
		URL = _hyperlink.Url;
	}

	/// <summary>
	///		Comprueba que los datos introducidos sean correctos
	/// </summary>
	private bool ValidateData()
	{
		bool validate = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(Name))
				MainViewModel.ViewsController.SystemController.ShowMessage("Introduzca el nombre del hipervínculo");
			else if (string.IsNullOrWhiteSpace(URL))
				MainViewModel.ViewsController.SystemController.ShowMessage("Introduzca la URL del hipervínculo");
			else
				validate = true;
			// Devuelve el valor que indica si los datos son correctos
			return validate;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		if (ValidateData())
		{ 
			// Asigna la carpeta
			if (_parent is not null && !_parent.Hyperlinks.Exists(_hyperlink.GlobalId))
				_parent.Hyperlinks.Add(_hyperlink);
			// Asigna los datos del formulario al objeto
			_hyperlink.Folder = Parent;
			_hyperlink.Name = Name;
			_hyperlink.Description = Description;
			_hyperlink.Url = URL;
			// Graba el objeto
			MainViewModel.BlogManager.Save();
			// Cierra el formulario
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public BlogReaderViewModel MainViewModel { get; }

	/// <summary>
	///		Nombre del hipervínculo
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Carpeta padre
	/// </summary>
	public FolderModel? Parent
	{
		get { return _parent; }
		set { CheckObject(ref _parent, value); }
	}

	/// <summary>
	///		Descripción del hipervínculo
	/// </summary>
	public string Description
	{
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}

	/// <summary>
	///		URL del hipervínculo
	/// </summary>
	public string URL
	{
		get { return _url; }
		set { CheckProperty(ref _url, value); }
	}
}