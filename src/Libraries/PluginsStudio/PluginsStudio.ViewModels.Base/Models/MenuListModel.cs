namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models;

/// <summary>
///		Lista de menús
/// </summary>
public class MenuListModel
{
	/// <summary>
	///		Sección a la que se asocian los menús
	/// </summary>
	public enum SectionType
	{
		/// <summary>Menús hijo de "Nuevo elemento"</summary>
		NewItem = 1,
		/// <summary>Menús hijo del menú de edición</summary>
		Edit,
		/// <summary>Menús hijo de la barra de herramientas</summary>
		Tools
	}

	public MenuListModel(SectionType section)
	{
		Section = section;
	}

	/// <summary>
	///		Crea un menú
	/// </summary>
	public MenuModel Add(string header, BauMvvm.ViewModels.BaseCommand command, string icon, string? gestureText = null)
	{
		MenuModel menu = new() {
									Header = header,
									Command = command,
									InputGestureText = gestureText,
									Icon = icon
							   };

			// Añade el menú a la colección
			Items.Add(menu);
			// Devuelve el menú añadido
			return menu;
	}

	/// <summary>
	///		Crea un separador
	/// </summary>
	public void AddSeparator()
	{
		Items.Add(new MenuModel
						{
							Id = Guid.NewGuid().ToString(),
							Header = string.Empty
						}
				  );
	}

	/// <summary>
	///		Sección a la que pertenece la lista de menús
	/// </summary>
	public SectionType Section { get; }

	/// <summary>
	///		Elementos del menú
	/// </summary>
	public List<MenuModel> Items { get; } = new();
}
