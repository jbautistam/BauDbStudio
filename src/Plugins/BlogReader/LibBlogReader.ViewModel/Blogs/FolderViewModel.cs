using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.LibBlogReader.ViewModel.Blogs;

/// <summary>
///		ViewModel de <see cref="FolderModel"/>
/// </summary>
public class FolderViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private FolderModel? _parent;
	private FolderModel _folder = default!;
	private string _name = default!;

	public FolderViewModel(BlogReaderViewModel mainViewModel, FolderModel? parent, FolderModel? folder)
	{
		MainViewModel = mainViewModel;
		_parent = parent;
		_folder = folder ?? new FolderModel();
		if (_parent is null)
			_parent = MainViewModel.BlogManager.File;
		if (_folder is not null && _folder.Parent is not null)
			_parent = _folder.Parent;
		InitProperties();
	}

	/// <summary>
	///		Inicializa las propiedades
	/// </summary>
	private void InitProperties()
	{
		Name = _folder.Name;
	}

	/// <summary>
	///		Comprueba que los datos introducidos sean correctos
	/// </summary>
	private bool ValidateData()
	{
		bool validate = false;

			// Comprueba los datos introducidos
			if (Name.IsEmpty())
				MainViewModel.ViewsController.SystemController.ShowMessage("Introduzca el nombre de la carpeta");
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
			if (_parent != null && !_parent.Folders.Exists(_folder.GlobalId))
				_parent.Folders.Add(_folder);
			_folder.Parent = _parent;
			// Asigna los datos del formulario al objeto
			_folder.Name = Name;
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
	///		Nombre del blog
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}
}