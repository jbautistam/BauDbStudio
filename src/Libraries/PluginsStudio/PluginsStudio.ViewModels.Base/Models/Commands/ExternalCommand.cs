namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Commands;

/// <summary>
///		Comando externo
/// </summary>
public class ExternalCommand
{
	/// <summary>
	///		Tipo de comando externo
	/// </summary>
	public enum ExternalCommandType
	{
		/// <summary>Desconocido</summary>
		Unknown,
		/// <summary>Borrar</summary>
		Delete,
		/// <summary>Abrir</summary>
		Open,
		/// <summary>Copiar</summary>
		Copy,
		/// <summary>Cortar</summary>
		Cut,
		/// <summary>Pegar</summary>
		Paste,
		/// <summary>Ejecución de comando</summary>
		Execute
	}

	public ExternalCommand(ExternalCommandType type, string? name = null)
	{
		Type = type;
		if (!string.IsNullOrWhiteSpace(name))
			Name = name;
		else
			Name = Type.ToString();
	}

	/// <summary>
	///		Nombre del comando
	/// </summary>
	public string Name { get; }

	/// <summary>
	///		Tipo del comando
	/// </summary>
	public ExternalCommandType Type { get; }
}
