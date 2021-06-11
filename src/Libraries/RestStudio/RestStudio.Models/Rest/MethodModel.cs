using System;
using System.Collections.Generic;

namespace Bau.Libraries.RestStudio.Models.Rest
{
	/// <summary>
	///		Clase con los datos de un método
	/// </summary>
	public class MethodModel
	{
		/// <summary>
		///		Métodos posibles
		/// </summary>
		public enum MethodType
		{
			Get,
			Post,
			Put,
			Delete
		}

		/// <summary>
		///		Nombre del método
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Método
		/// </summary>
		public MethodType Method { get; set; }

		/// <summary>
		///		Url
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		///		Cabeceras
		/// </summary>
		public Dictionary<string, string> Headers { get; } = new();

		/// <summary>
		///		Cuerpo
		/// </summary>
		public string Body { get; set; }
	}
}
