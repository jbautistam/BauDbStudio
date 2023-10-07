namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Builders;

/// <summary>
///		Generador de <see cref="FileOptionsModel"/>
/// </summary>
public class FileOptionsBuilder
{
	/// <summary>
	///		Genera una opción
	/// </summary>
	public FileOptionsBuilder WithOption()
	{
		// Añade una opción
		Options.Add(new FileOptionsModel());
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Indica que se debe utilizar una carpeta
	/// </summary>
	public FileOptionsBuilder WithFolder()
	{
		// Indica que se debe utilizar con carpetas
		LastOption.ForFolder = true;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una extensión
	/// </summary>
	public FileOptionsBuilder WithExtension(string extension)
	{
		// Añade la extensión
		if (!string.IsNullOrWhiteSpace(extension))
		{
			// Quita los espacios
			extension = extension.Trim();
			// Añade el punto si no empieza por punto
			if (!extension.StartsWith('.'))
				extension = $".{extension}";
			// Añade la extensión a la colección
			LastOption.FileExtension.Add(extension);
		}
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade un menú
	/// </summary>
	public FileOptionsBuilder WithMenu(MenuModel menu)
	{
		// Asigna el menú
		LastOption.Menu = menu;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Obtiene las opciones generadas
	/// </summary>
	public List<FileOptionsModel> Build() => Options;

	/// <summary>
	///		Opciones
	/// </summary>
	private List<FileOptionsModel> Options { get; } = new();

	/// <summary>
	///		Última opción de la lista
	/// </summary>
	private FileOptionsModel LastOption
	{
		get
		{
			// Añade una opción si no había ninguna
			if (Options.Count == 0)
				Options.Add(new FileOptionsModel());
			// Devuelve la última opción generada
			return Options[Options.Count - 1];
		}
	}
}
