namespace Bau.Libraries.PluginsStudio.ViewModels.Windows;

/// <summary>
///		ViewModel de una ventana
/// </summary>
public class WindowViewModel : BauMvvm.ViewModels.BaseObservableObject
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
	private string _id = default!, _name = default!;
	private WindowType _type;
	private bool _visible;

	public WindowViewModel(PluginsStudioViewModel mainViewModel, string id, string name, WindowType type)
	{
		MainViewModel = mainViewModel;
		Id = id;
		Name = name;
		Type = type;
		Visible = true;
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
	///		Indica si la ventana está visible
	/// </summary>
	public bool Visible
	{
		get { return _visible; }
		set { CheckProperty(ref _visible, value); }
	}
}
