namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Builders;

/// <summary>
///		Generador de menús
/// </summary>
public class MenuBuilder
{
	/// <summary>
	///		Añade un menú
	/// </summary>
	public MenuBuilder WithMenu(MenuListModel.SectionType section)
	{
		// Añade un menú
		Menus.Add(new MenuListModel(section));
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una opción de menú
	/// </summary>
	public MenuBuilder WithItem(string header, BauMvvm.ViewModels.BaseCommand command, string icon)
	{
		// Añade el elemento
		ActualMenu.Add(header, command, icon);
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una tecla de función
	/// </summary>
	public MenuBuilder WithGesture(string gesture)
	{
		// Cambia las propiedades
		ActualItem.InputGestureText = gesture;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade un separador
	/// </summary>
	public MenuBuilder WithSeparator()
	{
		// Añade el separador
		ActualMenu.AddSeparator();
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Genera la lista
	/// </summary>
	public List<MenuListModel> Build() => Menus;

	/// <summary>
	///		Lista de menús
	/// </summary>
	private List<MenuListModel> Menus { get; } = new();

	/// <summary>
	///		Menú actual
	/// </summary>
	private MenuListModel ActualMenu => Menus[Menus.Count - 1];

	/// <summary>
	///		Elemento de menú actual
	/// </summary>
	private MenuModel ActualItem => ActualMenu.Items[ActualMenu.Items.Count - 1];
}
