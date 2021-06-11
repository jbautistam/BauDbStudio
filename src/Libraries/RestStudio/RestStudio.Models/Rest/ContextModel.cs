using System;

namespace Bau.Libraries.RestStudio.Models.Rest
{
	/// <summary>
	///		Contexto
	/// </summary>
	public class ContextModel
	{
		/// <summary>
		///		Nombre
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Url de servidor
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		///		Credenciales
		/// </summary>
		public CredentialsModel Credentials { get; } = new();

		/// <summary>
		///		Timeout
		/// </summary>
		public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(3);
	}
}
