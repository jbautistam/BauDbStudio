namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Builders;

/// <summary>
///		Generador de <see cref="FileAssignedModel"/>
/// </summary>
public class FileAssignedBuilder
{
	/// <summary>
	///		Genera un <see cref=" FileAssignedModel"/>
	/// </summary>
	public FileAssignedBuilder WithFile(string name, string fileExtension, string icon)
	{
		// Crea el archivo
		Files.Add(new FileAssignedModel
							{
								Name = name,
								FileExtension = fileExtension,
								Icon = icon
							}
				 );
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una plantilla
	/// </summary>
	public FileAssignedBuilder WithTemplate(string template)
	{
		// Crea el archivo
		LastFile.Template = template;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Indica si se puede crear
	/// </summary>
	public FileAssignedBuilder WithCanCreate(bool canCreate)
	{
		// Indica si se puede crear
		LastFile.CanCreate = canCreate;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Genera los archivos
	/// </summary>
	public List<FileAssignedModel> Build() => Files;

	/// <summary>
	///		Archivos generados
	/// </summary>
	public List<FileAssignedModel> Files { get; } = new();

	/// <summary>
	///		Ultimo archivo generado
	/// </summary>
	private FileAssignedModel LastFile => Files[Files.Count - 1];
}
