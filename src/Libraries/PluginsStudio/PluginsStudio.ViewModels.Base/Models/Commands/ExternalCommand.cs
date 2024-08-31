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
		Paste
	}

	public ExternalCommand(string name, ExternalCommandType type)
	{
		Name = name;
		Type = type;
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
