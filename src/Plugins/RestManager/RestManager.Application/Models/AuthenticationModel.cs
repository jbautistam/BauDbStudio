namespace Bau.Libraries.RestManager.Application.Models;

/// <summary>
///		Datos de autenticación
/// </summary>
public class AuthenticationModel
{
	/// <summary>
	///		Tipo de autenticación
	/// </summary>
	public enum AuthenticationType
	{
		/// <summary>Ninguna</summary>
		None,
		/// <summary>Básica</summary>
		Basic,
		/// <summary>Jwt</summary>
		Jwt,
		/// <summary>OAuth</summary>
		OAuth,
		/// <summary>Api key</summary>
		ApiKey,
		/// <summary>Cabecera de autenticación</summary>
		Bearer
	}

	/// <summary>
	///		Tipo de autenticación
	/// </summary>
	public AuthenticationType Type { get; set; }

	/// <summary>
	///		Opciones de seguridad
	/// </summary>
	public Dictionary<string, string?> SecurityOptions { get; } = new(StringComparer.InvariantCultureIgnoreCase);
}