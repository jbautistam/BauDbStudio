namespace Bau.Libraries.PluginsStudio.ViewModels.Files.Search;

/// <summary>
///		Elemento de la ejecución de archivos
/// </summary>
public class SearchFileTypeItemViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
{
	// Variables privadas
	private string _extension = default!;

	public SearchFileTypeItemViewModel(SearchFilesViewModel viewModel, string name, string extension, string icon, bool isChecked)  : base(name, null)
	{
		ViewModel = viewModel;
		IsChecked = isChecked;
		Icon = icon;
		Extension = extension;
		PropertyChanged += (sender, args) => {
												if (!string.IsNullOrWhiteSpace(args.PropertyName) && 
														args.PropertyName.Equals(nameof(IsChecked), StringComparison.CurrentCultureIgnoreCase))
													viewModel.UpdateMask();
											 };
	}

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public SearchFilesViewModel ViewModel { get; }

	/// <summary>
	///		Extensión del archivo
	/// </summary>
	public string Extension
	{
		get { return _extension; }
		set { CheckProperty(ref _extension, value); }
	}
}