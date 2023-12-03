namespace Bau.Libraries.PasswordManager.Application.Models;

/// <summary>
///		Clase con los datos de una entrada de contraseña
/// </summary>
public class EntryModel : LibDataStructures.Base.BaseExtendedModel
{ 
	/// <summary>
	///		Url
	/// </summary>
	public string Url { get; set; } = default!;

	/// <summary>
	///		Notas
	/// </summary>
	public string Notes { get; set; } = default!;

	/// <summary>
	///		Usuario
	/// </summary>
	public string User { get; set; } = default!;

	/// <summary>
	///		Contraseña
	/// </summary>
	public string Password { get; set; } = default!;

	/// <summary>
	///		Fecha de creación
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.Now;
}
