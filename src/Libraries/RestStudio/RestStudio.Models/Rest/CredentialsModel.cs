using System;

namespace Bau.Libraries.RestStudio.Models.Rest
{
	/// <summary>
	///		Clase con los datos de credenciales
	/// </summary>
	public class CredentialsModel
	{
		/// <summary>
		///		Tipo de autentificación
		/// </summary>
		public enum AuthenticationType
		{
			/// <summary>Sin autentificación</summary>
			Noauthentication = 1,
			/// <summary>Básica</summary>
			Basic,
			/// <summary>Jwt</summary>
			Jwt
		}

		/// <summary>
		///		Modo de autentificación
		/// </summary>
		public AuthenticationType Authentication { get; set; } = AuthenticationType.Noauthentication;

		/// <summary>
		///		Url de la entidad de autorización
		/// </summary>
		public string UrlAuthority { get; set; }

		/// <summary>
		///		Usuario
		/// </summary>
		public string User { get; set; }

		/// <summary>
		///		Contraseña
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		///		Ambito de autentificación
		/// </summary>
		public string Scope { get; set; }
	}
}
