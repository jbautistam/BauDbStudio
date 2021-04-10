using System;

namespace Bau.Libraries.RestStudio.Models.Rest
{
	/// <summary>
	///		Contexto
	/// </summary>
	public class ContextModel
	{
		/// <summary>
		///		Url de servidor
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		///		Credenciales
		/// </summary>
		public CredentialsModel Credentials { get; } = new();

		/// <summary>
		///		Tiemout
		/// </summary>
		public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(3);
	}
}
